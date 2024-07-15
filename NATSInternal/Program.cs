using Microsoft.IdentityModel.Tokens;
using NATSInternal.Middlewares;
using NATSInternal.Services.Identity;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
//if (environment == Environments.Development)
//{
//    builder.Services.AddControllersWithViews()
//        .AddRazorRuntimeCompilation();
//}
//else
//{
//    builder.Services.AddControllersWithViews();
//}

// Add signalR
builder.Services.AddSignalR();

// Connection string - EF Core.
string connectionString = builder.Configuration.GetConnectionString("Mysql");
builder.Services.AddDbContext<DatabaseContext>(options => options
    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .AddInterceptors(new VietnamTimeInterceptor()));

// Identity
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddErrorDescriber<VietnameseIdentityErrorDescriber>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 0;
});

// Authentication by cookies strategies.
//builder.Services.ConfigureApplicationCookie(options => {
//    options.Cookie.HttpOnly = true;
//    options.ExpireTimeSpan = TimeSpan.FromDays(7);
//    options.LoginPath = "/Authentication/SignIn";
//    options.LogoutPath = "/Authentication/SignOut";
//    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
//    options.SlidingExpiration = true;
//    options.Events.OnValidatePrincipal = async context =>
//    {
//        IAuthorizationService authorizationService = context.HttpContext
//                .RequestServices
//                .GetRequiredService<IAuthorizationService>();
//        string userIdAsString = context.Principal?
//            .FindFirst(ClaimTypes.NameIdentifier)?
//            .Value;

//        // Validate user id in the token.
//        int userId;
//        try
//        {
//            userId = int.Parse(userIdAsString);
//            await authorizationService.SetUserId(userId);
//        }
//        catch
//        {
//            // If an exception occurs, sign out the user and redirect to the login page
//            context.RejectPrincipal();
//            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            context.HttpContext.Response.Redirect(options.LoginPath);
//        }
//    };
//});

// Authentication by JWT strategies.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                IAuthorizationService authorizationService = context.HttpContext
                    .RequestServices
                    .GetRequiredService<IAuthorizationService>();
                string userIdAsString = context.Principal?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                // Validate user id in the token.
                try
                {
                    if (userIdAsString == null)
                    {
                        throw new Exception();
                    }
                    int userId = int.Parse(userIdAsString);
                    await authorizationService.SetUserId(userId);
                }
                catch (Exception exception)
                {
                    context.Fail(exception);
                }
            }
        };
    });

// Authorization policies.
builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy("CanCreateUser", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateUser))
    .AddPolicy("CanResetPassword", policy =>
        policy.RequireClaim("Permission", PermissionConstants.ResetOtherUserPassword))
    .AddPolicy("CanDeleteUser", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteUser))
    .AddPolicy("CanRestoreUser", policy =>
        policy.RequireClaim("Permission", PermissionConstants.RestoreUser))
    .AddPolicy("CanGetCustomerDetail", policy =>
        policy.RequireClaim("Permission", PermissionConstants.GetCustomerDetail))
    .AddPolicy("CanCreateCustomer", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateCustomer))
    .AddPolicy("CanEditCustomer", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditCustomer))
    .AddPolicy("CanDeleteCustomer", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteCustomer))
    .AddPolicy("CanCreateBrand", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateBrand))
    .AddPolicy("CanEditBrand", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditBrand))
    .AddPolicy("CanDeleteBrand", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteBrand))
    .AddPolicy("CanCreateProduct", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateProduct))
    .AddPolicy("CanEditProduct", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditProduct))
    .AddPolicy("CanDeleteProduct", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteProduct))
    .AddPolicy("CanCreateProductCategory", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateProductCategory))
    .AddPolicy("CanEditProductCategory", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditProductCategory))
    .AddPolicy("CanDeleteProductCategory", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteProductCategory))
    .AddPolicy("CanCreateSupply", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateSupply))
    .AddPolicy("CanEditSupply", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditSupply))
    .AddPolicy("CanDeleteSupply", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteSupply))
    .AddPolicy("CanEditSupplyItem", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditSupplyItem))
    .AddPolicy("CanDeleteSupplyItem", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteSupplyItem))
    .AddPolicy("CanEditSupplyPhoto", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditSupplyPhoto))
    .AddPolicy("CanDeleteSupplyPhoto", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteSupplyPhoto))
    .AddPolicy("CanCreateExpense", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateExpense))
    .AddPolicy("CanEditExpense", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditExpense))
    .AddPolicy("CanDeleteExpense", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteExpense))
    .AddPolicy("CanCreateOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateOrder))
    .AddPolicy("CanEditOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditOrder))
    .AddPolicy("CanDeleteOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteOrder))
    .AddPolicy("CanCreateOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateOrder))
    .AddPolicy("CanEditOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditOrder))
    .AddPolicy("CanDeleteOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteOrder))
    .AddPolicy("CanCreateDebt", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateDebt))
    .AddPolicy("CanEditDebt", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditDebt))
    .AddPolicy("CanDeleteDebt", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteDebt))
    .AddPolicy("CanCreateDebtPayment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateDebtPayment))
    .AddPolicy("CanEditDebtPayment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditDebtPayment))
    .AddPolicy("CanDeleteDebtPayment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteDebtPayment));


// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<SignInValidator>();
ValidatorOptions.Global.LanguageManager.Enabled = true;
ValidatorOptions.Global.LanguageManager = new ValidatorLanguageManager
{
    Culture = new CultureInfo("vi")
};
ValidatorOptions.Global.PropertyNameResolver = (_, b, _) => b.Name
    .First()
    .ToString()
    .ToLower() + b.Name[1..];

// Add controllers with json serialization policy.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

// Dependancy injection
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<RoleManager<Role>>();
builder.Services.AddScoped<DatabaseContext>();
builder.Services.AddScoped<SqlExceptionHandler>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<ISupplyService, SupplyService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDebtService, DebtService>();
builder.Services.AddScoped<IDebtPaymentService, DebtPaymentService>();
builder.Services.AddScoped<IConsultantService, ConsultantService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddSingleton<IStatsTaskService, StatsTaskService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Background tasks.
builder.Services.AddHostedService<RefreshTokenCleanerTask>();
builder.Services.AddHostedService<StatsTask>();

// Add CORS.
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "LocalhostDevelopment",
        policyBuilder => policyBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

WebApplication app = builder.Build();
DataInitializer dataInitializer;
dataInitializer = new DataInitializer();
dataInitializer.InitializeData(app);

app.UseCors("LocalhostDevelopment");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.UseMiddleware<RequestLoggingMiddleware>();
// app.MapControllerRoute("default", "{controller=Home}/{action=Index}");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.MapFallbackToFile("/index.html");
app.Run();
