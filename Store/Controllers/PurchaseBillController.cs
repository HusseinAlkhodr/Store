using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.DTO;
using Store.DTO.Pagination;
using Store.Models;
using Store.Models.Invoice;
using Store.Specification.PurchaseBillItemSpecificatoin;
using Store.Specification.PurchaseBillSpecification;
using System.Globalization;

namespace Store.Controllers
{
    public class PurchaseBillController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly PurchaseBillFilterBuilder filterBuilder;
        private readonly PurchaseBillItemFilterBuilder itemFilterBuilder;

        public PurchaseBillController(IUnitOfWork unitOfWork, IMapper mapper,
            PurchaseBillFilterBuilder filterBuilder, PurchaseBillItemFilterBuilder itemFilterBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.filterBuilder = filterBuilder;
            this.itemFilterBuilder = itemFilterBuilder;
        }
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PaginationParams paginationParams, [FromQuery] PurchaseBillFilterDTO filterDTO)
        {
            var specification = unitOfWork.PurchaseBillRepository.InjectSpecification(filterBuilder, filterDTO);
            var bills = await unitOfWork.PurchaseBillRepository.GetAllWithPagination(
                paginationParams: paginationParams,
                includeProperties: nameof(PurchaseBill.CreatedBy),
                filter: p => p.IsArchived != 1,
                extendQuery: specification);

            var returnedBills = new List<GetPurchaseBillDTO>();
            foreach (var bill in bills.Items)
            {
                var billDto = new GetPurchaseBillDTO
                {
                    Id = bill.Id,
                    CreatedAt = bill.CreatedAt.ToString("dd/MM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                    CreatedBy = bill.CreatedBy.FullName,
                    SourceName = bill.CustomerName,
                    Total = bill.Total,
                    ExchangeRate = bill.ExchangeRate,
                };
                returnedBills.Add(billDto);
            }

            var pagedResult = new PagedList<GetPurchaseBillDTO>
            {
                Items = returnedBills,
                TotalPages = bills.TotalPages
            };

            return View(pagedResult);
        }

        public async Task<IActionResult> Details(long id)
        {
            var purchaseBill = await unitOfWork.PurchaseBillRepository.Get(p => p.Id == id, includeProperties: nameof(PurchaseBill.CreatedBy));
            var purchaseBillItems = await unitOfWork.PurchaseBillItemRepository.GetAll(
                filter: a => a.PurchaseBillId == id,
                includeProperties:
                nameof(PurchaseBillItem.Item) + "," + nameof(PurchaseBillItem.ItemType));
            var purchaseBillDTO = new GetPurchaseBillDetailsDTO
            {
                Id = purchaseBill.Id,
                SourceName = purchaseBill.CustomerName,
                CreatedAt = purchaseBill.CreatedAt.ToString("dd/MM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                CreatedBy = purchaseBill.CreatedBy.FullName,
                ExchangeRate = purchaseBill.ExchangeRate,
                Total = purchaseBill.Total
            };
            var purchaseBillItemsDTO = new List<GetPurchaseBillItemDTO>();
            foreach (var item in purchaseBillItems)
            {
                var oitem = new GetPurchaseBillItemDTO
                {
                    Code = item.Item.Code,
                    ItemDescription = item.Item.Description,
                    ItemCost = item.ItemCost,
                    QTY = item.QTY,
                    ItemType = item.ItemType.Name,
                    Total = item.TotalCost,
                };
                purchaseBillItemsDTO.Add(oitem);
            }
            purchaseBillDTO.Items = purchaseBillItemsDTO;
            return View(purchaseBillDTO);
        }
        public IActionResult AddPurchaseBill()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddPurchaseBill([FromBody] AddPurchaseBillDTO model)
        {
            var exchangeRate = (await unitOfWork.CurrencyExchangeRateRepository.GetAll(orderBy: o => o.OrderByDescending(e => e.CreatedAt))).Select(x => x.ExchangeRate).FirstOrDefault();

            var rate = 1;
            if (model.payType != 0)
                rate = (int)exchangeRate;

            var PurchaseBillItems = new List<PurchaseBillItem>();
            var purchaseBillItemDTO = model.Items;
            foreach (var itemDTO in purchaseBillItemDTO)
            {
                var typeQTY = (await unitOfWork.ItemTypeRepository.Get(t => t.Id == itemDTO.TypeId))?.QTY;
                var cost = (long)(itemDTO.ItemCost * rate);
                var item = new PurchaseBillItem
                {
                    ItemId = itemDTO.ItemId,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedById = (long)CurrentUserId,
                    TypeId = itemDTO.TypeId,
                    ItemCost = (long)(itemDTO.ItemCost * rate),
                    QTY = itemDTO.QTY,
                    TotalCost = (long)cost * itemDTO.QTY * (typeQTY ?? 1)
                };
                PurchaseBillItems.Add(item);
                var orginalItem = await unitOfWork.ItemRepository.Get(i => i.Id == itemDTO.ItemId);
                if (orginalItem != null)
                {
                    orginalItem.QTY += itemDTO.QTY * typeQTY ?? 1;
                    orginalItem.Cost = orginalItem.Cost == item.ItemCost ? orginalItem.Cost : item.ItemCost;
                    await unitOfWork.ItemRepository.Update(orginalItem);
                }
            }
            var purchaseBill = new PurchaseBill
            {
                CreatedAt = DateTime.UtcNow.ToLocalTime(),
                CreatedById = (long)CurrentUserId,
                ExchangeRate = exchangeRate,
                Total = PurchaseBillItems.Sum(t => t.TotalCost),
                CustomerName = model.CustomerName,
            };
            await unitOfWork.PurchaseBillRepository.Insert(purchaseBill);
            await unitOfWork.Save();
            foreach (var PurchaseBillItem in PurchaseBillItems)
            {
                PurchaseBillItem.PurchaseBillId = purchaseBill.Id;
            }
            await unitOfWork.PurchaseBillItemRepository.InsertRange(PurchaseBillItems);
            await unitOfWork.Save();
            purchaseBill.Items = PurchaseBillItems;
            await unitOfWork.PurchaseBillRepository.Update(purchaseBill);

            await unitOfWork.Save();
            return Ok();
        }
        public async Task<IActionResult> Update(long id)
        {
            var purchaseBill = await unitOfWork.PurchaseBillRepository.Get(s => s.Id == id, includeProperties:
                nameof(PurchaseBill.CreatedBy));
            var purchaseBillItem = await unitOfWork.PurchaseBillItemRepository.GetAll(i => i.PurchaseBillId == id,
                includeProperties:
                nameof(PurchaseBillItem.Item) + "," + nameof(PurchaseBillItem.ItemType) + "," + nameof(PurchaseBillItem.CreatedBy)
                );
            var returnPurchaseBill = new GetPurchaseBillUpdateDTO
            {
                Id = purchaseBill.Id,
                CreatedAt = purchaseBill.CreatedAt.ToString("dd/MM/yyyy - hh:mm tt"),
                CreatedBy = purchaseBill.CreatedBy.FullName,
                SourceName = purchaseBill.CustomerName,
                Items = purchaseBillItem.Select(
                    x => new GetPurchaseBillItemForUpdateDTO
                    {
                        Id = x.Id,
                        Code = x.Item.Code,
                        CreatedAt = x.CreatedAt.ToString("dd/MM/yyyy - hh:mm tt"),
                        CreatedBy = x.CreatedBy.FullName,
                        ItemId = x.ItemId,
                        ItemDescription = x.Item.Description,
                        TypeId = x.TypeId,
                        ItemType = x.ItemType,
                        QTY = x.QTY,
                        ItemCost = x.ItemCost
                    }
                ).ToList()
            };

            return View(nameof(Update), returnPurchaseBill);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody] AddPurchaseBillDTO model)
        {
            var exchangeRate = (await unitOfWork.CurrencyExchangeRateRepository
                .GetAll(orderBy: o => o.OrderByDescending(e => e.CreatedAt), take: 1))
                .Select(x => x.ExchangeRate).FirstOrDefault();

            var rate = model.payType != 0 ? (int)exchangeRate : 1;

            var bill = await unitOfWork.PurchaseBillRepository.Get(b => b.Id == id);
            if (bill == null)
                return NotFound("لم يتم العثور على الفاتورة");

            var billItems = await unitOfWork.PurchaseBillItemRepository.GetAll(a => a.PurchaseBillId == id);
            var newbillItemId = model.Items.Select(a => a.ItemId);
            var deletedItems = await unitOfWork.PurchaseBillItemRepository
                .GetAll(a => a.PurchaseBillId == id && !newbillItemId.Contains(a.ItemId));
            foreach (var realItemToDelete in deletedItems)
            {
                var RealItem = await unitOfWork.ItemRepository.Get(a => a.Id == realItemToDelete.ItemId);
                var OldType = await unitOfWork.ItemTypeRepository.Get(a => a.Id == realItemToDelete.TypeId);
                RealItem.QTY -= realItemToDelete.QTY * OldType.QTY;
                await unitOfWork.ItemRepository.Update(RealItem);
            }

            var newBillItems = new List<PurchaseBillItem>();

            foreach (var item in model.Items)
            {
                if (item.QTY <= 0 || item.ItemCost < 0)
                    return BadRequest("قيمة غير صحيحة للكمية أو السعر");

                var typeQTY = (await unitOfWork.ItemTypeRepository.Get(t => t.Id == item.TypeId))?.QTY ?? 1;

                var realItem = await unitOfWork.ItemRepository.Get(i => i.Id == item.ItemId);
                if (realItem == null)
                    return BadRequest($"المادة برقم {item.ItemId} غير موجودة");

                var oldBillItem = await unitOfWork.PurchaseBillItemRepository
                    .Get(i => i.PurchaseBillId == id && i.ItemId == item.ItemId);

                if (oldBillItem != null)
                {
                    var oldType = await unitOfWork.ItemTypeRepository.Get(t => t.Id == oldBillItem.TypeId);
                    realItem.QTY -= (long)(oldBillItem.QTY * oldType.QTY);
                }

                realItem.QTY += (long)(item.QTY * typeQTY);

                var newItem = new PurchaseBillItem
                {
                    QTY = item.QTY,
                    PurchaseBillId = id,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedById = (long)CurrentUserId,
                    ItemId = item.ItemId,
                    TypeId = item.TypeId,
                    ItemCost = (long)(item.ItemCost * rate),
                    TotalCost = (long)(item.ItemCost * rate * item.QTY * typeQTY)
                };
                newBillItems.Add(newItem);
                await unitOfWork.ItemRepository.Update(realItem);
            }
            foreach (var item in billItems)
                await unitOfWork.PurchaseBillItemRepository.Delete(item.Id);
            await unitOfWork.PurchaseBillItemRepository.InsertRange(newBillItems);
            bill.ExchangeRate = exchangeRate;
            bill.UpdatedById = (long)CurrentUserId;
            bill.UpdatedAt = DateTime.UtcNow.ToLocalTime();
            bill.Total = newBillItems.Sum(t => t.TotalCost);
            await unitOfWork.PurchaseBillRepository.Update(bill);
            await unitOfWork.Save();
            return Ok();
        }
        public async Task<IActionResult> Delete(long id)
        {
            var purchaseBill = await unitOfWork.PurchaseBillRepository.Get(p => p.Id == id, includeProperties: nameof(PurchaseBill.CreatedBy));
            var purchaseBillItems = await unitOfWork.PurchaseBillItemRepository.GetAll(
                filter: a => a.PurchaseBillId == id,
                includeProperties:
                nameof(PurchaseBillItem.Item) + "," + nameof(PurchaseBillItem.ItemType));
            var purchaseBillDTO = new GetPurchaseBillDetailsDTO
            {
                Id = purchaseBill.Id,
                SourceName = purchaseBill.CustomerName,
                CreatedAt = purchaseBill.CreatedAt.ToString("dd/MM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                CreatedBy = purchaseBill.CreatedBy.FullName,
                ExchangeRate = purchaseBill.ExchangeRate,
                Total = purchaseBill.Total
            };
            var purchaseBillItemsDTO = new List<GetPurchaseBillItemDTO>();
            foreach (var item in purchaseBillItems)
            {
                var oitem = new GetPurchaseBillItemDTO
                {
                    Code = item.Item.Code,
                    ItemDescription = item.Item.Description,
                    ItemCost = item.ItemCost,
                    QTY = item.QTY,
                    ItemType = item.ItemType.Name,
                    Total = item.TotalCost,
                };
                purchaseBillItemsDTO.Add(oitem);
            }
            purchaseBillDTO.Items = purchaseBillItemsDTO;
            return View(purchaseBillDTO);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(long Id)
        {
            var bill = await unitOfWork.PurchaseBillRepository.Get(a => a.Id == Id);
            var billItems = await unitOfWork.PurchaseBillItemRepository.GetAll(a => a.PurchaseBillId == Id);
            foreach (var billItem in billItems)
            {
                var item = await unitOfWork.ItemRepository.Get(a => a.Id == billItem.ItemId);
                var type = await unitOfWork.ItemTypeRepository.Get(t => t.Id == billItem.TypeId);
                item.QTY -= billItem.QTY * type.QTY;
                await unitOfWork.ItemRepository.Update(item);
            }
            bill.ArchivedById = (long)CurrentUserId;
            await unitOfWork.PurchaseBillRepository.Update(bill);
            await unitOfWork.Save();
            await unitOfWork.PurchaseBillRepository.Delete(Id);
            await unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> GetPurchaseItem([FromQuery] PaginationParams paginationParams, [FromQuery] PurchaseBillItemFilterDTO itemFilterDTO)
        {
            // إنشاء المواصفات للفلترة
            var specification = unitOfWork.PurchaseBillItemRepository.InjectSpecification(itemFilterBuilder, itemFilterDTO);

            // جلب البيانات مع Pagination والفلترة والتضمين Include
            var pagedPurchasedItems = await unitOfWork.PurchaseBillItemRepository.GetAllWithPagination(
                paginationParams: paginationParams,
                includeProperties:
                    nameof(PurchaseBillItem.PurchaseBill) + "," +
                    nameof(PurchaseBillItem.ItemType) + "," +
                    nameof(PurchaseBillItem.Item) + "," +
                    nameof(PurchaseBillItem.Item) + "." + nameof(Item.Division) + "," +
                    nameof(PurchaseBillItem.Item) + "." + nameof(Item.Vendor),
                extendQuery: specification,
                filter: a => a.PurchaseBill.IsArchived != 1
            );
            var pagedListResult = new PagedList<GetPurchasedItem>
            {
                Items = new List<GetPurchasedItem>(),
                TotalPages = pagedPurchasedItems.TotalPages
            };
            foreach (var item in pagedPurchasedItems.Items)
            {
                var x = new GetPurchasedItem
                {
                    Code = item.Item.Code,
                    Description = item.Item.Description,
                    Division = item.Item.Division.Name,
                    Vendor = item.Item.Vendor.Name,
                    Cost = item.ItemCost,
                    Type = item.ItemType.Name,
                    QTY = item.QTY,
                    Total = item.TotalCost,
                    CreatedAt = item.CreatedAt.ToString("dd/MMM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                    BillId = item.PurchaseBillId
                };
                pagedListResult.Items.Add(x);
            }
            // إرجاع الفيو مع الموديل الكامل (PagedList)
            return View(pagedListResult);
        }


        public async Task<IActionResult> GetDeletedBill()
        {
            var bills = await unitOfWork.PurchaseBillRepository.GetAll(a => a.IsArchived == 1);
            var billsDTO = new List<GetDeletedDTO>();
            foreach (var bill in bills)
            {
                var username = await unitOfWork.AccountRepository.Get(a => a.Id == bill.ArchivedById);
                var billDTO = new GetDeletedDTO
                {
                    Id = bill.Id,
                    SourceName = bill.CustomerName,
                    DeletedAt = bill.ArchiveDate?.ToString("dd/MMM/yyy - hh:mm tt", new CultureInfo("ar-SY")),
                    DeletedBy = username.FullName,
                    ExchangeRate = bill.ExchangeRate,
                    Total = bill.Total,
                };
                billsDTO.Add(billDTO);
            }
            return View(billsDTO);
        }
        public async Task<IActionResult> DeleteDetails(long id)
        {
            var purchaseBill = await unitOfWork.PurchaseBillRepository.Get(p => p.Id == id, includeProperties: nameof(PurchaseBill.CreatedBy));
            var purchaseBillItems = await unitOfWork.PurchaseBillItemRepository.GetAll(
                filter: a => a.PurchaseBillId == id,
                includeProperties:
                nameof(PurchaseBillItem.Item) + "," + nameof(PurchaseBillItem.ItemType));
            var purchaseBillDTO = new GetPurchaseBillDetailsDTO
            {
                Id = purchaseBill.Id,
                SourceName = purchaseBill.CustomerName,
                CreatedAt = purchaseBill.CreatedAt.ToString("dd/MM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                CreatedBy = purchaseBill.CreatedBy.FullName,
                ExchangeRate = purchaseBill.ExchangeRate,
                Total = purchaseBill.Total
            };
            var purchaseBillItemsDTO = new List<GetPurchaseBillItemDTO>();
            foreach (var item in purchaseBillItems)
            {
                var oitem = new GetPurchaseBillItemDTO
                {
                    Code = item.Item.Code,
                    ItemDescription = item.Item.Description,
                    ItemCost = item.ItemCost,
                    QTY = item.QTY,
                    ItemType = item.ItemType.Name,
                    Total = item.TotalCost,
                };
                purchaseBillItemsDTO.Add(oitem);
            }
            purchaseBillDTO.Items = purchaseBillItemsDTO;
            return View(purchaseBillDTO);
        }
    }
}
