using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.DTO;
using Store.Extensions;
using Store.Middlewares;
using Store.Models;

namespace Store.Controllers
{
    public class VendorController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public VendorController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        // GET: VendorController
        public async Task<IActionResult> Index()
        {
            var vendors = await unitOfWork.VendorRepository.GetAll();
            var vendorsDTO = mapper.Map<List<GetVendorDTO>>(vendors);
            return View("Index", vendorsDTO);
        }

        // GET: VendorController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var Vendor = await unitOfWork.VendorRepository.Get(v => v.Id == id).ValidateNotFound();
            var VendorDTO = mapper.Map<GetVendorDTO>(Vendor);
            return View(VendorDTO);
        }

        // GET: VendorController/Create
        public ActionResult Create()
        {
            return View(nameof(Create));
        }

        // POST: VendorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddVendorDTO vendorDTO)
        {
            if (ModelState.IsValid)
            {
                var vendor = mapper.Map<Vendor>(vendorDTO);
                vendor.CreatedById = (long)CurrentUserId;
                vendor.CreatedAt = DateTime.UtcNow.ToLocalTime();
                await unitOfWork.VendorRepository.Insert(vendor);
                await unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            throw new HusseinErrorResponseException("");
        }

        // GET: VendorController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var Vendor = await unitOfWork.VendorRepository.Get(v => v.Id == id).ValidateNotFound();
            var VendorDTO = mapper.Map<GetVendorDTO>(Vendor);
            return View(VendorDTO);
        }

        // POST: VendorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id,UpdateVendorDTO vendorDTO)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index));
            var vendor = await unitOfWork.VendorRepository.Get(v => v.Id==id).ValidateNotFound();
            vendor.Name = vendorDTO.Name;
            vendor.UpdatedAt = DateTime.UtcNow.ToLocalTime();
            vendor.UpdatedById = CurrentUserId;
            await unitOfWork.VendorRepository.Update(vendor);
            await unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: VendorController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vendor = await unitOfWork.VendorRepository.Get(v => v.Id == id).ValidateNotFound();
            var vendorDTO = mapper.Map<GetVendorDTO>(vendor);
            return View(vendorDTO);
        }

        // POST: VendorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var vendor = await unitOfWork.VendorRepository.Get(v => v.Id == id).ValidateNotFound();

            await unitOfWork.VendorRepository.Delete(vendor);
            await unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
