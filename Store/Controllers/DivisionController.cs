using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.DTO;
using Store.Extensions;
using Store.Models;

namespace Store.Controllers
{
    public class DivisionController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DivisionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        // GET: DivisionController
        public async Task<IActionResult> Index()
        {
            var Divisions = await unitOfWork.DivisionRepository.GetAll();
            var DivisionsList = mapper.Map<List<GetDivisionDTO>>(Divisions);
            return View(DivisionsList);
        }

        // GET: DivisionController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var Division = await unitOfWork.DivisionRepository.Get(d => d.Id == id).ValidateNotFound();
            var DivisionDTO = mapper.Map<GetDivisionDTO>(Division);
            return View(DivisionDTO);
        }

        // GET: DivisionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DivisionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddDivisionDTO divisionDTO)
        {
            if (!ModelState.IsValid)
                return View(nameof(Create));

            var division = mapper.Map<Division>(divisionDTO);
            division.CreatedById = (long)CurrentUserId;
            division.CreatedAt = DateTime.UtcNow.ToLocalTime();

            await unitOfWork.DivisionRepository.Insert(division);
            await unitOfWork.Save();
            TempData["SuccessMessage"] = "تم اضافة القسم بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        // GET: DivisionController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var Division = await unitOfWork.DivisionRepository.Get(d => d.Id == id).ValidateNotFound();
            var DivisionDTO = mapper.Map<GetDivisionDTO>(Division);
            return View(DivisionDTO);
        }

        // POST: DivisionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, UpdateDivisionDTO divisionDTO)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index));
            var division = await unitOfWork.DivisionRepository.Get(d => d.Id == id).ValidateNotFound();
            division.Name = divisionDTO.Name;
            division.UpdatedAt = DateTime.UtcNow.ToLocalTime();
            division.UpdatedById = CurrentUserId;
            await unitOfWork.DivisionRepository.Update(division);
            await unitOfWork.Save();
            TempData["SuccessMessage"] = "تم تعديل القسم بنجاح.";

            return RedirectToAction(nameof(Index));
        }

        // GET: DivisionController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var Division = await unitOfWork.DivisionRepository.Get(d => d.Id == id).ValidateNotFound();

            var DivisionDTO = mapper.Map<GetDivisionDTO>(Division);

            return View(DivisionDTO);
        }

        // POST: DivisionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id)
        {
            var Division = await unitOfWork.DivisionRepository.Get(d => d.Id == id).ValidateNotFound();
            var item = await unitOfWork.ItemRepository.GetAll(a => a.DivisionId == id);
            if (item.Any())
            {
                TempData["ErrorMessage"] = "غير قادر على حذف قسم له مواد مرتبطة به.";
                return RedirectToAction(nameof(Index));
            }
            await unitOfWork.DivisionRepository.Delete(Division);
            await unitOfWork.Save();
            TempData["SuccessMessage"] = "تم حذف القسم بنجاح.";

            return RedirectToAction(nameof(Index));
        }
    }
}
