using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.DTO;
using Store.Models.Authenitication;

namespace Store.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SignInManager<Account> signInManager;
        private readonly UserManager<Account> userManager;
        private readonly RoleManager<Role> roleManager;

        public AccountController(IUnitOfWork unitOfWork, IMapper mapper,
                                SignInManager<Account> signInManager,
                                UserManager<Account> userManager,
                                RoleManager<Role> roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            // إذا لم يكن مسجل الدخول، عرض صفحة تسجيل الدخول
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (await userManager.CheckPasswordAsync(user, model.Password))
                {
                    await signInManager.SignInAsync(user, model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["ErrorMessage"] = "بيانات الدخول غير صحيحة.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "تم تسجيل الخروج بنجاح.";
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied() => View("AccessDenied");
        [AllowAnonymous]

        public IActionResult Signup() => View();

        [AllowAnonymous]

        [HttpPost("Signup")]
        public async Task<IActionResult> Signup(RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return View(model); // عرض الصفحة مع الأخطاء

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "كلمة المرور وتأكيدها غير متطابقين");
                return View(model);
            }
            var newUser = new Account
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Status = AccountStatus.Active,
                AccountType = AccountType.User,
                IsApproved = true
            };
            await newUser.Validiate(unitOfWork);
            newUser.PasswordHash = userManager.PasswordHasher.HashPassword(newUser, model.Password);
            await unitOfWork.AccountRepository.Insert(newUser);
            await unitOfWork.Save();
            // تأكد من وجود دور "User"
            var userRoleExists = await roleManager.RoleExistsAsync("User");
            if (!userRoleExists)
            {
                await roleManager.CreateAsync(new Role { Name = "User" });
            }
            if (string.IsNullOrEmpty(newUser.SecurityStamp))
            {
                // يضيف SecurityStamp إذا لم يتم توليده
                await userManager.UpdateSecurityStampAsync(newUser);
            }
            // إضافة الدور للمستخدم
            await userManager.AddToRoleAsync(newUser, "User");

            return RedirectToAction(nameof(Login));
        }
        public IActionResult AddUser() => View();
        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "البيانات غير مكتملة.";
                return RedirectToAction("AddUser");
            }

            if (model.Password != model.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "كلمة المرور وتأكيدها غير متطابقين";
                return View();
            }
            var newUser = new Account
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Status = AccountStatus.Active,
                AccountType = AccountType.User,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow.ToLocalTime(),
                CreatedById = CurrentUserId
            };
            await newUser.Validiate(unitOfWork);
            newUser.PasswordHash = userManager.PasswordHasher.HashPassword(newUser, model.Password);
            await unitOfWork.AccountRepository.Insert(newUser);
            await unitOfWork.Save();
            // تأكد من وجود دور "User"
            var userRoleExists = await roleManager.RoleExistsAsync("User");
            if (!userRoleExists)
            {
                await roleManager.CreateAsync(new Role { Name = "User" });
            }
            if (string.IsNullOrEmpty(newUser.SecurityStamp))
            {
                // يضيف SecurityStamp إذا لم يتم توليده
                await userManager.UpdateSecurityStampAsync(newUser);
            }
            // إضافة الدور للمستخدم
            await userManager.AddToRoleAsync(newUser, "User");
            TempData["SuccessMessage"] = $"تم اضافة المستخدم {newUser.FullName} بنجاح .";

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await unitOfWork.AccountRepository.GetAll(a => a.AccountType != AccountType.Admin);
            var accountsDTO = new List<GetAccountDTO>();
            foreach (var account in accounts)
            {
                var accountDTO = new GetAccountDTO
                {
                    Id = account.Id,
                    Email = account.Email,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    AccountType = account.AccountType,
                    Status = account.Status
                };
                accountsDTO.Add(accountDTO);
            }
            return View(accountsDTO);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var x = await unitOfWork.AccountRepository.Get(a => a.Id == id);
            if (x != null)
            {
                if (x.Id != CurrentUserId || x.CreatedById != CurrentUserId)
                {

                    TempData["ErrorMessage"] = "انت غير مخول للقيام بالعملية.";
                    return RedirectToAction("Index");
                }
            }
            await unitOfWork.AccountRepository.Delete((long)id);
            await unitOfWork.Save();
            TempData["SuccessMessage"] = $"تم حذف المستخدم بنجاح .";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(GetAccountDTO model)
        {
            var account = await unitOfWork.AccountRepository.Get(a => a.Id == model.Id);
            if (account.Id != CurrentUserId || account.CreatedById != CurrentUserId)
            {
                TempData["ErrorMessage"] = "انت غير مخول للقيام بالعملية.";
                return RedirectToAction("Index");
            }
            account.FirstName = model.FirstName;
            account.LastName = model.LastName;
            account.Email = model.Email;
            account.UpdatedAt = DateTime.UtcNow.ToLocalTime();
            account.UpdatedById = CurrentUserId;
            await unitOfWork.AccountRepository.Update(account);
            await unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "البيانات غير مكتملة.";
                return RedirectToAction(nameof(Index));
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                TempData["ErrorMessage"] = "كلمة السر الجديدة وتأكيدها غير متطابقين.";
                return RedirectToAction(nameof(Index));
            }

            if (model.NewPassword == model.OldPassword)
            {
                TempData["ErrorMessage"] = "كلمة السر الجديدة يجب أن تكون مختلفة عن القديمة.";
                return RedirectToAction(nameof(Index));
            }
            var user = await userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "المستخدم غير موجود.";
                return RedirectToAction(nameof(Index));
            }
            if (user.Id != CurrentUserId || user.CreatedById != CurrentUserId)
            {
                TempData["ErrorMessage"] = "ليس لديك صلاحية للقيام بهذه العملية.";
                return RedirectToAction(nameof(Index));
            }
            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model); // أو رجعها داخل مودال برسالة خطأ
        }

    }
}
