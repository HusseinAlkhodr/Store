using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.DTO;
using Store.Models;
using System.Globalization;

namespace Store.Controllers
{
    public class ItemTypeController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ItemTypeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var types = await unitOfWork.ItemTypeRepository.GetAll();
            return Ok(types);
        }
        public async Task<IActionResult> Index()
        {
            var types = await unitOfWork.ItemTypeRepository.GetAll();
            var typesDTO = new List<GetItemTypeDTO>();
            foreach (var type in types)
            {
                var x = new GetItemTypeDTO
                {
                    Id = type.Id,
                    Name = type.Name,
                    QTY = type.QTY,
                    CreatedAt = type.CreatedAt.ToString("dd/MM/yyyy - hh:mm tt", new CultureInfo("ar-SY"))
                };
                typesDTO.Add(x);
            }
            return View(typesDTO);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ItemTypeDTO model)
        {
            var type = await unitOfWork.ItemTypeRepository.GetAll(a => a.Name == model.Name || a.QTY == model.QTY);
            if (type.Any())
            {
                return BadRequest("يوجد نوع له نفس الاسم او نفس الكمية");
            }
            await unitOfWork.ItemTypeRepository.Insert(new ItemType
            {
                Name = model.Name,
                QTY = model.QTY,
                CreatedAt = DateTime.Now.ToLocalTime(),
                CreatedById = (long)CurrentUserId
            });
            await unitOfWork.Save();
            TempData["SuccessMessage"] = "تم اضافة النوع بنجاح.";
            return Ok("تم اضافة النوع بنجاح");
        }
        public async Task<IActionResult> Edit(long id, [FromBody] ItemTypeDTO model)
        {
            var item = await unitOfWork.ItemTypeRepository.Get(a => a.Id == id);
            item.Name = model.Name;
            item.QTY = model.QTY;
            await unitOfWork.ItemTypeRepository.Update(item);
            await unitOfWork.Save();
            return Ok("تم تعديل النوع بنجاح");
        }
        public async Task<IActionResult> Delete(long id)
        {
            var sale = await unitOfWork.SaleBillItemRepository.GetAll(a => a.TypeId == id);
            var purchase = await unitOfWork.PurchaseBillItemRepository.GetAll(a => a.TypeId == id);
            if (sale.Any() || purchase.Any())
            {
                return BadRequest("لا يمكن حذف نوع مرتبط بفواتير سابقة");
            }
            await unitOfWork.ItemTypeRepository.Delete(id);
            await unitOfWork.Save();
            TempData["SuccessMessage"] = "تم الحذف بنجاح.";

            return Ok("");
        }
    }
}
