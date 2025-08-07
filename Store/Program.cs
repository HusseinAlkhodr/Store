using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Store.Core.Unit;
using Store.DataBaseContext;
using Store.Mapper;
using Store.Middlewares;
using Store.Models;
using Store.Models.Authenitication;
using Store.Specification.PurchaseBillItemSpecificatoin;
using Store.Specification.PurchaseBillSpecification;
using Store.Specification.SaleBillItemSpecification;
using Store.Specification.SaleBillSpecification;
using Store.Specification.SaleReportSpecification;
using System.Globalization;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        //builder.Services.AddControllersWithViews();

        builder.Services.AddHttpContextAccessor();

        ConfigurationManager _appConfiguration = builder.Configuration; // allows both to access and to set up the config
        IWebHostEnvironment environment = builder.Environment;
        const string CorsPolicyName = "AllowOrigin";

        builder.Services.AddDbContext<StoreDbContext>(
            options =>
            options.UseSqlServer(
                _appConfiguration.GetConnectionString("AppDb"),
                sqlServerOptionsAction: sqlServerOptions =>
                {
                    sqlServerOptions.CommandTimeout(60);
                    sqlServerOptions.EnableRetryOnFailure
                    (
                        maxRetryCount: 5
                    );
                })
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
            ServiceLifetime.Transient);
        //Initialize Identity System
        builder.Services.AddIdentity<Account, Role>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
        }).AddEntityFrameworkStores<StoreDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });
        builder.Services.AddAuthorization();

        builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddControllersWithViews(options =>
        {
            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }).AddViewOptions(options =>
        {
            options.HtmlHelperOptions.ClientValidationEnabled = true;
        })
        .AddViewLocalization()
        .AddDataAnnotationsLocalization(); ;

        builder.Services.AddMvc();

        builder.Services.AddTransient<SaleBillFilterBuilder>();
        builder.Services.AddTransient<PurchaseBillFilterBuilder>();
        builder.Services.AddTransient<SaleBillItemFilterBuilder>();
        builder.Services.AddTransient<PurchaseBillItemFilterBuilder>();
        builder.Services.AddTransient<SaleReportFilterBuilder>();
        builder.Services.AddTransient<SaleReportMonthlyFilterBuilder>();
        builder.Services.AddTransient<SaleReportWeeklyFilterBuilder>();
        builder.Services.AddScoped<IUserClaimsPrincipalFactory<Account>, UserClaimsPrincipalFactory>();
        builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        builder.Services.AddAutoMapper(typeof(MapperInit));


        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: CorsPolicyName,
                policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        var app = builder.Build();
        var supportedCultures = new[] { new CultureInfo("ar"), new CultureInfo("en-US") };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture("ar")
            .AddSupportedCultures(supportedCultures.Select(c => c.Name).ToArray())
            .AddSupportedUICultures(supportedCultures.Select(c => c.Name).ToArray());

        app.UseRequestLocalization(localizationOptions);

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseHsts();
        app.UseCors(CorsPolicyName);
        app.UseMiddleware(typeof(ExceptionHandlerMiddleWare));

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");
        await InitializeDbAsync(app);
        await InitialUser(app);
        await InitialType(app);
        app.Run();
    }

    static async Task InitializeDbAsync(WebApplication host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

        await context.Database.MigrateAsync();
    }
    static async Task InitialUser(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        var users = await userManager.Users.ToListAsync();

        if (!users.Any())
        {
            var roleName = "Admin";

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var adminRole = new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                await roleManager.CreateAsync(adminRole);
            }

            var account = new Account
            {
                FirstName = "System",
                LastName = "Admin",
                UserName = "system@store.com",
                NormalizedUserName = "SYSTEM@STORE.COM",
                Email = "system@store.com",
                NormalizedEmail = "SYSTEM@STORE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                IsApproved = true,
                Status = AccountStatus.Active,
                AccountType = AccountType.Admin
            };

            var result = await userManager.CreateAsync(account, "P@$$w.rd");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"❌ {error.Description}");
                }
                return;
            }

            await userManager.AddToRoleAsync(account, roleName);

            Console.WriteLine("✅ تم إنشاء المستخدم Admin بنجاح");
        }
        else
        {
            Console.WriteLine("ℹ يوجد مستخدمون بالفعل، لم يتم إنشاء أي مستخدم جديد");
        }
    }

    static async Task InitialType(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var type = await unitOfWork.ItemTypeRepository.GetAll();
        if (!type.Any())
            await unitOfWork.ItemTypeRepository.InsertRange(new List<ItemType>
            {
                new () { Name = "قطعة", QTY = 1, CreatedById = 1 , CreatedAt = DateTime.UtcNow.ToLocalTime()},
                new () {Name = "كروز", QTY = 10, CreatedById = 1 , CreatedAt = DateTime.UtcNow.ToLocalTime()},
                new () { Name = "طرد", QTY = 20 , CreatedById = 1 , CreatedAt = DateTime.UtcNow.ToLocalTime()}
            });
        var division = await unitOfWork.DivisionRepository.GetAll();
        if (!division.Any())
            await unitOfWork.DivisionRepository.InsertRange(new List<Division>
            {
                new () {Name = "دخان", CreatedById = 1 , CreatedAt = DateTime.UtcNow.ToLocalTime()},
                new () { Name = "غذائيات", CreatedById = 1 , CreatedAt = DateTime.UtcNow.ToLocalTime()},
            });
        var rate = await unitOfWork.CurrencyExchangeRateRepository.GetAll();
        if (!rate.Any())
            await unitOfWork.CurrencyExchangeRateRepository.Insert(
                new() { ExchangeRate = 10000, CreatedById = 1, CreatedAt = DateTime.UtcNow.ToLocalTime() });
        await unitOfWork.Save();
    }
}