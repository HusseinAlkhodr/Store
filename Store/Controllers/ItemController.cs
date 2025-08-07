using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.Core.Unit;
using Store.DTO;
using Store.DTO.Pagination;
using Store.Extensions;
using Store.Models;

namespace Store.Controllers
{
    public class ItemController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public ItemController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        // GET: ItemController
        public async Task<IActionResult> Index([FromQuery] PaginationParams paginationParams)
        {
            var items = await unitOfWork.ItemRepository.GetAllWithPagination(
                paginationParams: paginationParams,
                includeProperties: nameof(Item.Vendor) + "," + nameof(Item.Division)
                ).Validate_Null();
            var itemList = mapper.Map<PagedList<GetItemDTO>>(items);
            foreach (var item in itemList.Items)
            {
                item.Price = (await unitOfWork.PriceListRepository.GetAll(p => p.ItemId == item.Id,
                    orderBy: o => o.OrderByDescending(p => p.CreatedAt))).Select(p => p.Price).FirstOrDefault();
            }
            var Divisions = await unitOfWork.DivisionRepository.GetAll();
            var Vendors = await unitOfWork.VendorRepository.GetAll();
            ViewBag.Divisions = Divisions.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();
            ViewBag.Vendors = Vendors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();
            return View(itemList);
        }

        // GET: ItemController/Details/5
        public async Task<ActionResult> Details(long id)
        {
            var item = await unitOfWork.ItemRepository.Get(i => i.Id == id, includeProperties: nameof(Item.Vendor) + "," + nameof(Item.Division))
                .ValidateNotFound();

            var itemPrice = (await unitOfWork.PriceListRepository.GetAll(
                filter: p => p.ItemId == id,
                orderBy: o => o.OrderByDescending(p => p.CreatedAt))).Select(s => s.Price).FirstOrDefault();

            var itemDTO = mapper.Map<GetItemDTO>(item);
            itemDTO.Price = itemPrice;
            return View(itemDTO);
        }
        // GET: ItemController/Create
        public async Task<ActionResult> Create()
        {

            var Divisions = await unitOfWork.DivisionRepository.GetAll();
            var Vendors = await unitOfWork.VendorRepository.GetAll();
            ViewBag.Divisions = Divisions.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();
            ViewBag.Vendors = Vendors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();
            return View(nameof(Create));
        }

        // POST: ItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddItemDTO itemDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(); // إعادة عرض النموذج مع الأخطاء
            }
            var item = mapper.Map<Item>(itemDTO);
            item.CreatedById = (long)CurrentUserId;
            item.CreatedAt = DateTime.UtcNow.ToLocalTime();

            await unitOfWork.ItemRepository.Insert(item);
            await unitOfWork.Save();
            TempData["SuccessMessage"] = "تم اضافة المادة بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ItemController/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            var item = await unitOfWork.ItemRepository.Get(i => i.Id == id, includeProperties: nameof(Item.Vendor) + "," + nameof(Item.Division))
                .ValidateNotFound();

            var itemPrice = (await unitOfWork.PriceListRepository.GetAll(
                filter: p => p.ItemId == id,
                orderBy: o => o.OrderByDescending(p => p.CreatedAt))).Select(s => s.Price).FirstOrDefault();

            var itemDTO = mapper.Map<GetItemDTO>(item);
            itemDTO.Price = itemPrice;

            var Divisions = await unitOfWork.DivisionRepository.GetAll();
            var Vendors = await unitOfWork.VendorRepository.GetAll();
            ViewBag.Divisions = Divisions.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();
            ViewBag.Vendors = Vendors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();
            return View(itemDTO);
        }

        // POST: ItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateItemDTO itemDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { message = "Invalid input", errors });
            }

            var item = await unitOfWork.ItemRepository.Get(i => i.Id == itemDTO.Id).ValidateNotFound();

            var itemPrice = (await unitOfWork.PriceListRepository.GetAll(
                filter: p => p.ItemId == itemDTO.Id,
                orderBy: o => o.OrderByDescending(p => p.CreatedAt))).Select(s => s.Price).FirstOrDefault();

            item = mapper.Map(itemDTO, item);
            item.UpdatedAt = DateTime.UtcNow.ToLocalTime();
            item.UpdatedById = CurrentUserId;

            if (itemDTO.Price != itemPrice)
            {
                var rate = (await unitOfWork.CurrencyExchangeRateRepository.GetAll(orderBy: o => o.OrderByDescending(c => c.CreatedAt)))
                    .Select(r => r.ExchangeRate).FirstOrDefault();
                var Price = new PriceList
                {
                    ExchangeRate = rate,
                    Price = itemDTO.Price,
                    ItemId = item.Id,
                    CreatedAt = DateTime.UtcNow.ToLocalTime(),
                    CreatedById = (long)CurrentUserId
                };
                await unitOfWork.PriceListRepository.Insert(Price);
            }

            await unitOfWork.ItemRepository.Update(item);
            await unitOfWork.Save();
            TempData["SuccessMessage"] = "تم تعديل المادة بنجاح.";

            return RedirectToAction(nameof(Index));
        }
        // GET: ItemController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var item = await unitOfWork.ItemRepository.Get(i => i.Id == id, includeProperties: nameof(Item.Vendor) + "," + nameof(Item.Division))
                .ValidateNotFound();

            var itemPrice = (await unitOfWork.PriceListRepository.GetAll(
                filter: p => p.ItemId == id,
                orderBy: o => o.OrderByDescending(p => p.CreatedAt))).Select(s => s.Price).FirstOrDefault();

            var itemDTO = mapper.Map<GetItemDTO>(item);
            itemDTO.Price = itemPrice;

            return View(itemDTO);
        }
        // POST: ItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await unitOfWork.ItemRepository.Get(u => u.Id == id).ValidateNotFound();
            var salebillitem = await unitOfWork.SaleBillItemRepository.GetAll(a => a.ItemId == id);
            var purchasebillitem = await unitOfWork.PurchaseBillItemRepository.GetAll(a => a.ItemId == id);
            if (salebillitem.Any() || purchasebillitem.Any())
            {
                TempData["ErrorMessage"] = "لا يمكن حذف مادة موجود في فواتير مسبقة.";
                return RedirectToAction(nameof(Index));
            }
            var priceList = await unitOfWork.PriceListRepository.GetAll(p => p.ItemId == id);
            await unitOfWork.PriceListRepository.DeleteRange(priceList);

            await unitOfWork.ItemRepository.Delete(item);

            await unitOfWork.Save();

            TempData["SuccessMessage"] = "تم حذف المادة بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        // GetForPurchase
        public async Task<IActionResult> GetForPurchase(string barcode)
        {
            var item = await unitOfWork.ItemRepository.Get(i => i.Code == barcode).ValidateNotFound();

            var itemDTO = new GetPurchaseItemDTO
            {
                Id = item.Id,
                Code = item.Code,
                Description = item.Description,
                Cost = item.Cost
            };
            return Ok(itemDTO);
        }
        // GetForSale
        public async Task<IActionResult> GetForSale(string barcode)
        {
            var item = await unitOfWork.ItemRepository.Get(i => i.Code == barcode).ValidateNotFound();
            var itemPrices = await unitOfWork.PriceListRepository.GetAll(
                filter: p => p.ItemId == item.Id,
                orderBy: o => o.OrderByDescending(p => p.CreatedAt));
            var price = 0L;
            if (itemPrices.Count > 0)
            {
                price = itemPrices.Select(p => p.Price).FirstOrDefault();
            }
            var itemDTO = new GetSaleItemDTO
            {
                Id = item.Id,
                Code = item.Code,
                Description = item.Description,
                Price = price,
                QTY = (int)item.QTY
            };
            return Ok(itemDTO);
        }
        // GetSaleFromText
        public async Task<IActionResult> GetSaleFromText(string Text)
        {
            var items = await unitOfWork.ItemRepository.GetAll(i => i.Code.Contains(Text) || i.Description.Contains(Text)).ValidateNotFound();
            var itemsDTO = new List<GetSaleItemDTO>();
            foreach (var item in items)
            {
                var itemPrice = await unitOfWork.PriceListRepository.GetAll(
                filter: p => p.ItemId == item.Id,
                orderBy: o => o.OrderByDescending(p => p.CreatedAt));
                var price = 0L;
                if (itemPrice.Count > 0)
                {
                    price = itemPrice.Select(p => p.Price).FirstOrDefault();
                }
                var itemDTO = new GetSaleItemDTO
                {
                    Id = item.Id,
                    Code = item.Code,
                    Description = item.Description,
                    Price = price,
                    QTY = (int)item.QTY
                };
                itemsDTO.Add(itemDTO);
            }


            return Ok(itemsDTO);
        }
        // GetPurchaseFromText
        public async Task<IActionResult> GetPurchaseFromText(string Text)
        {
            var items = await unitOfWork.ItemRepository.GetAll(i => i.Code.Contains(Text) || i.Description.Contains(Text)).ValidateNotFound();
            var itemDTOs = items.Select(item => new GetPurchaseItemDTO
            {
                Id = item.Id,
                Code = item.Code,
                Description = item.Description,
                Cost = item.Cost
            }).ToList();

            return Ok(itemDTOs);
        }
    }
}
