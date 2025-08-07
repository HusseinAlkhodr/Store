using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.DTO;
using Store.DTO.Pagination;
using Store.Models;
using Store.Models.Invoice;
using Store.Specification.SaleBillItemSpecification;
using Store.Specification.SaleBillSpecification;
using System.Globalization;

namespace Store.Controllers
{
    public class SaleBillController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SaleBillFilterBuilder filterBuilder;
        private readonly SaleBillItemFilterBuilder itemFilterBuilder;

        public SaleBillController(IUnitOfWork unitOfWork, IMapper mapper,
            SaleBillFilterBuilder filterBuilder, SaleBillItemFilterBuilder itemFilterBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.filterBuilder = filterBuilder;
            this.itemFilterBuilder = itemFilterBuilder;
        }
        public async Task<IActionResult> Index([FromQuery] PaginationParams paginationParams, [FromQuery] SaleBillFilterDTO filterDTO)
        {
            var specification = unitOfWork.SaleBillRepository.InjectSpecification(filterBuilder, filterDTO);
            var saleBills = await unitOfWork.SaleBillRepository.GetAllWithPagination(
                extendQuery: specification,
                paginationParams: paginationParams,
                filter: a => a.IsArchived != 1,
                orderBy: o => o.OrderByDescending(a => a.CreatedAt),
                includeProperties: nameof(SaleBill.CreatedBy));
            var returnedBills = new PagedList<GetSaleBillDTO>
            {
                Items = new List<GetSaleBillDTO>(),
                TotalPages = saleBills.TotalPages
            };
            foreach (var bill in saleBills.Items)
            {
                var returnedBill = new GetSaleBillDTO
                {
                    Id = bill.Id,
                    CustomerName = bill.CustomerName,
                    CreatedAt = bill.CreatedAt.ToString("dd/MM/yyyy hh:mm tt"),
                    CreatedBy = bill.CreatedBy.FullName,
                    ExchangeRate = bill.ExchangeRate,
                    Total = bill.Total,
                };
                returnedBills.Items.Add(returnedBill);
            }
            ViewBag.TotalPages = saleBills.TotalPages;
            ViewBag.Filter = filterDTO;
            return View(returnedBills);
        }
        public async Task<IActionResult> Details(long Id)
        {
            var saleBill = await unitOfWork.SaleBillRepository.Get(expression: s => s.Id == Id, includeProperties: nameof(SaleBill.CreatedBy));
            var saleBillItem = await unitOfWork.SaleBillItemRepository.GetAll(s => s.SaleBillId == Id,
                includeProperties:
                nameof(SaleBillItem.Item) + "," + nameof(SaleBillItem.ItemType)
                );
            var SaleBillItems = new List<GetSaleBillItemDetailsDTO>();
            foreach (var item in saleBillItem)
            {
                var itemDTO = new GetSaleBillItemDetailsDTO
                {
                    Code = item.Item.Code,
                    ItemDescription = item.Item.Description,
                    ItemType = item.ItemType.Name,
                    ItemPrice = item.ItemPrice,
                    QTY = item.QTY,
                    Total = item.TotalPrice

                };
                SaleBillItems.Add(itemDTO);
            }
            var saleBillDetails = new GetSaleBillDetailsForView
            {
                Id = saleBill.Id,
                CreatedAt = saleBill.CreatedAt.ToString("dd/MMM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                CreatedBy = saleBill.CreatedBy.FullName,
                CustomerName = saleBill.CustomerName,
                ExchangeRate = saleBill.ExchangeRate,
                Total = saleBill.Total,
                Items = SaleBillItems
            };
            return View(saleBillDetails);
        }
        [HttpGet]
        public IActionResult AddSaleBill()
        {
            return View(nameof(AddSaleBill));
        }
        [HttpPost]
        public async Task<IActionResult> AddSaleBill([FromBody] AddSaleBillDTO model)
        {
            var exchangeRate = (await unitOfWork.CurrencyExchangeRateRepository.GetAll(orderBy: o => o.OrderByDescending(e => e.CreatedAt))).Select(x => x.ExchangeRate).FirstOrDefault();

            var rate = 1;
            if (model.payType != 0)
                rate = (int)exchangeRate;

            var SaleBillItems = new List<SaleBillItem>();
            var purchaseBillItemDTO = model.Items;
            foreach (var itemDTO in purchaseBillItemDTO)
            {
                var typeQTY = (await unitOfWork.ItemTypeRepository.Get(t => t.Id == itemDTO.TypeId))?.QTY;
                var itemPrices = await unitOfWork.PriceListRepository.GetAll(
                filter: p => p.ItemId == itemDTO.ItemId,
                orderBy: o => o.OrderByDescending(p => p.CreatedAt));
                var price = 0L;
                if (itemPrices.Count > 0)
                {
                    price = itemPrices.Select(p => p.Price).FirstOrDefault();
                }
                var item = new SaleBillItem
                {
                    ItemId = itemDTO.ItemId,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedById = (long)CurrentUserId,
                    TypeId = itemDTO.TypeId,
                    ItemPrice = (long)(itemDTO.ItemPrice * rate),
                    QTY = itemDTO.QTY,
                    TotalPrice = (long)((itemDTO.ItemPrice * rate) * itemDTO.QTY * (typeQTY ?? 1))
                };
                SaleBillItems.Add(item);
                var orginalItem = await unitOfWork.ItemRepository.Get(i => i.Id == itemDTO.ItemId);
                if (orginalItem != null)
                {
                    orginalItem.QTY -= itemDTO.QTY * typeQTY ?? 1;
                    if (price == 0)
                    {
                        await unitOfWork.PriceListRepository.Insert(new PriceList
                        {
                            ItemId = orginalItem.Id,
                            CreatedById = (long)CurrentUserId,
                            CreatedAt = DateTime.UtcNow.ToLocalTime(),
                            Price = (long)(itemDTO.ItemPrice * rate),
                            ExchangeRate = exchangeRate
                        });
                    }
                    await unitOfWork.ItemRepository.Update(orginalItem);
                }
            }
            var SaleBill = new SaleBill
            {
                CreatedAt = DateTime.UtcNow.ToLocalTime(),
                CreatedById = (long)CurrentUserId,
                ExchangeRate = exchangeRate,
                Total = SaleBillItems.Sum(t => t.TotalPrice),
                CustomerName = model.CustomerName,
            };
            await unitOfWork.SaleBillRepository.Insert(SaleBill);
            await unitOfWork.Save();
            foreach (var SaleBillItem in SaleBillItems)
            {
                SaleBillItem.SaleBillId = SaleBill.Id;
            }
            await unitOfWork.SaleBillItemRepository.InsertRange(SaleBillItems);
            await unitOfWork.Save();
            return Ok(SaleBill.Id);
        }

        [HttpGet]
        public async Task<IActionResult> Update(long id)
        {
            var saleBill = await unitOfWork.SaleBillRepository.Get(s => s.Id == id, includeProperties:
                nameof(SaleBill.CreatedBy));
            var saleBillItem = await unitOfWork.SaleBillItemRepository.GetAll(i => i.SaleBillId == id,
                includeProperties:
                nameof(SaleBillItem.Item) + "," + nameof(SaleBillItem.ItemType) + "," + nameof(SaleBillItem.CreatedBy)
                );
            var returnSaleBill = new GetSaleBillDetails
            {
                Id = saleBill.Id,
                CreatedAt = saleBill.CreatedAt.ToString("dd/MM/yyyy - hh:mm tt"),
                CreatedBy = saleBill.CreatedBy.FullName,
                CustomerName = saleBill.CustomerName,
                Items = saleBillItem.Select(
                    x => new GetSaleBillItemDTO
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
                        ItemPrice = x.ItemPrice
                    }
                ).ToList()
            };

            return View(nameof(Update), returnSaleBill);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSaleBillDTO model)
        {
            var exchangeRate = (await unitOfWork.CurrencyExchangeRateRepository
                .GetAll(orderBy: o => o.OrderByDescending(e => e.CreatedAt), take: 1))
                .Select(x => x.ExchangeRate).FirstOrDefault();

            var rate = model.payType != 0 ? (int)exchangeRate : 1;

            var bill = await unitOfWork.SaleBillRepository.Get(b => b.Id == id);
            if (bill == null)
                return NotFound("لم يتم العثور على الفاتورة");

            var billItems = await unitOfWork.SaleBillItemRepository.GetAll(a => a.SaleBillId == id);
            var newbillItemId = model.Items.Select(a => a.ItemId);
            var deletedItems = await unitOfWork.SaleBillItemRepository
                .GetAll(a => a.SaleBillId == id && !newbillItemId.Contains(a.ItemId));
            foreach (var realItemToDelete in deletedItems)
            {
                var RealItem = await unitOfWork.ItemRepository.Get(a => a.Id == realItemToDelete.ItemId);
                var OldType = await unitOfWork.ItemTypeRepository.Get(a => a.Id == realItemToDelete.TypeId);
                RealItem.QTY += realItemToDelete.QTY * OldType.QTY;
                await unitOfWork.ItemRepository.Update(RealItem);
            }

            var newBillItems = new List<SaleBillItem>();

            foreach (var item in model.Items)
            {
                if (item.QTY <= 0 || item.ItemPrice < 0)
                    return BadRequest("قيمة غير صحيحة للكمية أو السعر");

                var typeQTY = (await unitOfWork.ItemTypeRepository.Get(t => t.Id == item.TypeId))?.QTY ?? 1;

                var realItem = await unitOfWork.ItemRepository.Get(i => i.Id == item.ItemId);
                if (realItem == null)
                    return BadRequest($"المادة برقم {item.ItemId} غير موجودة");

                var oldBillItem = await unitOfWork.SaleBillItemRepository
                    .Get(i => i.SaleBillId == id && i.ItemId == item.ItemId);

                if (oldBillItem != null)
                {
                    var oldType = await unitOfWork.ItemTypeRepository.Get(t => t.Id == oldBillItem.TypeId);
                    realItem.QTY += (long)(oldBillItem.QTY * oldType.QTY);
                }

                realItem.QTY -= (long)(item.QTY * typeQTY);
                var newItem = new SaleBillItem
                {
                    QTY = item.QTY,
                    SaleBillId = id,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedById = (long)CurrentUserId,
                    ItemId = item.ItemId,
                    TypeId = item.TypeId,
                    ItemPrice = (long)(item.ItemPrice * rate),
                    TotalPrice = (long)(item.ItemPrice * rate * item.QTY * typeQTY)
                };
                newBillItems.Add(newItem);
                await unitOfWork.ItemRepository.Update(realItem);
            }
            foreach (var item in billItems)
                await unitOfWork.SaleBillItemRepository.Delete(item.Id);
            await unitOfWork.SaleBillItemRepository.InsertRange(newBillItems);
            bill.ExchangeRate = exchangeRate;
            bill.Total = newBillItems.Sum(t => t.TotalPrice);
            bill.UpdatedAt = DateTime.UtcNow.ToLocalTime();
            bill.UpdatedById = (long)CurrentUserId;
            await unitOfWork.SaleBillRepository.Update(bill);
            await unitOfWork.Save();
            return Ok(bill.Id);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(long Id)
        {
            var saleBill = await unitOfWork.SaleBillRepository.Get(expression: s => s.Id == Id, includeProperties: nameof(SaleBill.CreatedBy));
            var saleBillItem = await unitOfWork.SaleBillItemRepository.GetAll(s => s.SaleBillId == Id,
                includeProperties:
                nameof(SaleBillItem.Item) + "," + nameof(SaleBillItem.ItemType)
                );
            var SaleBillItems = new List<GetSaleBillItemDetailsDTO>();
            foreach (var item in saleBillItem)
            {
                var itemDTO = new GetSaleBillItemDetailsDTO
                {
                    Code = item.Item.Code,
                    ItemDescription = item.Item.Description,
                    ItemType = item.ItemType.Name,
                    ItemPrice = item.ItemPrice,
                    QTY = item.QTY,
                    Total = item.TotalPrice

                };
                SaleBillItems.Add(itemDTO);
            }
            var saleBillDetails = new GetSaleBillDetailsForView
            {
                Id = saleBill.Id,
                CreatedAt = saleBill.CreatedAt.ToString("dd/MMM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                CreatedBy = saleBill.CreatedBy.FullName,
                CustomerName = saleBill.CustomerName,
                ExchangeRate = saleBill.ExchangeRate,
                Total = saleBill.Total,
                Items = SaleBillItems
            };
            return View(saleBillDetails);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(long Id)
        {
            var bill = await unitOfWork.SaleBillRepository.Get(a => a.Id == Id);
            var billItems = await unitOfWork.SaleBillItemRepository.GetAll(a => a.SaleBillId == Id);
            foreach (var billItem in billItems)
            {
                var item = await unitOfWork.ItemRepository.Get(a => a.Id == billItem.ItemId);
                var type = await unitOfWork.ItemTypeRepository.Get(t => t.Id == billItem.TypeId);
                item.QTY += billItem.QTY * type.QTY;
                await unitOfWork.ItemRepository.Update(item);
            }
            bill.ArchivedById = (long)CurrentUserId;
            await unitOfWork.SaleBillRepository.Update(bill);
            await unitOfWork.Save();
            await unitOfWork.SaleBillRepository.Delete(Id);
            await unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetSaleItem([FromQuery] PaginationParams paginationParams, [FromQuery] SaleBillItemFilterDTO itemFilterDTO)
        {
            var specification = unitOfWork.SaleBillItemRepository.InjectSpecification(itemFilterBuilder, itemFilterDTO);

            // استدعاء GetAllWithPagination يفترض أنه يعيد PagedList<SaleBillItem>
            var saledItemPaged = await unitOfWork.SaleBillItemRepository.GetAllWithPagination(
                paginationParams: paginationParams,
                includeProperties:
                    nameof(SaleBillItem.ItemType) + "," +
                    nameof(SaleBillItem.SaleBill) + "," +
                    nameof(SaleBillItem.Item) + "," +
                    nameof(SaleBillItem.Item) + "." + nameof(Item.Division) + "," +
                    nameof(SaleBillItem.Item) + "." + nameof(Item.Vendor),
                extendQuery: specification,
                filter: a => a.SaleBill.IsArchived != 1
            );
            var pagedListResult = new PagedList<GetSaledItem>
            {
                Items = new List<GetSaledItem>(),
                TotalPages = saledItemPaged.TotalPages
            };
            foreach (var item in saledItemPaged.Items)
            {
                var x = new GetSaledItem
                {
                    Code = item.Item.Code,
                    Description = item.Item.Description,
                    Division = item.Item.Division.Name,
                    Vendor = item.Item.Vendor.Name,
                    Price = item.ItemPrice,
                    Type = item.ItemType.Name,
                    QTY = item.QTY,
                    Total = item.TotalPrice,
                    CreatedAt = item.CreatedAt.ToString("dd/MMM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                    BillId = item.SaleBillId
                };
                pagedListResult.Items.Add(x);
            }

            // نعيد View مع PagedList
            return View(pagedListResult);
        }

        public async Task<IActionResult> GetDeletedBill()
        {
            var bills = await unitOfWork.SaleBillRepository.GetAll(a => a.IsArchived == 1);
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
        public async Task<IActionResult> DeleteDetails(long Id)
        {
            var saleBill = await unitOfWork.SaleBillRepository.Get(expression: s => s.Id == Id, includeProperties: nameof(SaleBill.CreatedBy));
            var saleBillItem = await unitOfWork.SaleBillItemRepository.GetAll(s => s.SaleBillId == Id,
                includeProperties:
                nameof(SaleBillItem.Item) + "," + nameof(SaleBillItem.ItemType)
                );
            var SaleBillItems = new List<GetSaleBillItemDetailsDTO>();
            foreach (var item in saleBillItem)
            {
                var itemDTO = new GetSaleBillItemDetailsDTO
                {
                    Code = item.Item.Code,
                    ItemDescription = item.Item.Description,
                    ItemType = item.ItemType.Name,
                    ItemPrice = item.ItemPrice,
                    QTY = item.QTY,
                    Total = item.TotalPrice

                };
                SaleBillItems.Add(itemDTO);
            }
            var saleBillDetails = new GetSaleBillDetailsForView
            {
                Id = saleBill.Id,
                CreatedAt = saleBill.CreatedAt.ToString("dd/MMM/yyyy - hh:mm tt", new CultureInfo("ar-SY")),
                CreatedBy = saleBill.CreatedBy.FullName,
                CustomerName = saleBill.CustomerName,
                ExchangeRate = saleBill.ExchangeRate,
                Total = saleBill.Total,
                Items = SaleBillItems
            };
            return View(saleBillDetails);
        }
    }
}
