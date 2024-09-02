using Microsoft.IdentityModel.Tokens;
using NATSInternal.Middlewares;
using NATSInternal.Services.Identity;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);
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
            OnMessageReceived = context =>
            {
                StringValues accessToken = context.Request.Query["access_token"];

                // Check if the request is for the signalR hub.
                PathString path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/Api/NotificationHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
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
    
    // Users.
    .AddPolicy("CanCreateUser", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateUser))
    .AddPolicy("CanResetPassword", policy =>
        policy.RequireClaim("Permission", PermissionConstants.ResetOtherUserPassword))
    .AddPolicy("CanDeleteUser", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteUser))
    .AddPolicy("CanRestoreUser", policy =>
        policy.RequireClaim("Permission", PermissionConstants.RestoreUser))
    
    // Customers.
    .AddPolicy("CanGetCustomerDetail", policy =>
        policy.RequireClaim("Permission", PermissionConstants.GetCustomerDetail))
    .AddPolicy("CanCreateCustomer", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateCustomer))
    .AddPolicy("CanEditCustomer", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditCustomer))
    .AddPolicy("CanDeleteCustomer", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteCustomer))
    
    // Brands.
    .AddPolicy("CanCreateBrand", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateBrand))
    .AddPolicy("CanEditBrand", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditBrand))
    .AddPolicy("CanDeleteBrand", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteBrand))
    
    // Products.
    .AddPolicy("CanCreateProduct", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateProduct))
    .AddPolicy("CanEditProduct", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditProduct))
    .AddPolicy("CanDeleteProduct", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteProduct))
    
    // ProductCategory.
    .AddPolicy("CanCreateProductCategory", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateProductCategory))
    .AddPolicy("CanEditProductCategory", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditProductCategory))
    .AddPolicy("CanDeleteProductCategory", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteProductCategory))
    
    // Supplies.
    .AddPolicy("CanCreateSupply", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateSupply))
    .AddPolicy("CanEditSupply", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditSupply))
    .AddPolicy("CanDeleteSupply", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteSupply))
    
    // Expenses.
    .AddPolicy("CanCreateExpense", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateExpense))
    .AddPolicy("CanEditExpense", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditExpense))
    .AddPolicy("CanDeleteExpense", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteExpense))
    
    // Orders.
    .AddPolicy("CanCreateOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateOrder))
    .AddPolicy("CanEditOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditOrder))
    .AddPolicy("CanDeleteOrder", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteOrder))
    
    // Treatments.
    .AddPolicy("CanCreateTreatment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateTreatment))
    .AddPolicy("CanEditTreatment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditTreatment))
    .AddPolicy("CanDeleteTreatment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteTreatment))
    
    // DebtIncurrence.
    .AddPolicy("CanCreateDebtIncurrence", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateDebtIncurrence))
    .AddPolicy("CanEditDebtIncurrence", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditDebtIncurrence))
    .AddPolicy("CanDeleteDebtIncurrence", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteDebtIncurrence))
    
    // DebtPayments.
    .AddPolicy("CanCreateDebtPayment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateDebtPayment))
    .AddPolicy("CanEditDebtPayment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditDebtPayment))
    .AddPolicy("CanDeleteDebtPayment", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteDebtPayment))
    
    // Consultants.
    .AddPolicy("CanCreateConsultant", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateConsultant))
    .AddPolicy("CanEditConsultant", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditConsultant))
    .AddPolicy("CanDeleteConsultant", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteConsultant))
    
    // Announcements.
    .AddPolicy("CanCreateAnnouncement", policy =>
        policy.RequireClaim("Permission", PermissionConstants.CreateAnnouncement))
    .AddPolicy("CanEditAnnouncement", policy =>
        policy.RequireClaim("Permission", PermissionConstants.EditAnnouncement))
    .AddPolicy("CanDeleteAnnouncement", policy =>
        policy.RequireClaim("Permission", PermissionConstants.DeleteAnnouncement));


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
builder.Services.AddScoped<ITreatmentService, TreatmentService>();
builder.Services.AddScoped<IDebtIncurrenceService, DebtIncurrenceService>();
builder.Services.AddScoped<IDebtPaymentService, DebtPaymentService>();
builder.Services.AddScoped<IConsultantService, ConsultantService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotifier, Notifier>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddSingleton<IStatsTaskService, StatsTaskService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Background tasks.
builder.Services.AddHostedService<RefreshTokenCleanerTask>();

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
app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoint =>
{
    endpoint.MapControllers();
    endpoint.MapHub<ApplicationHub>("/Api/NotificationHub");
    endpoint.MapHub<ResourceAccessingUsersHub>("Api/ResourceAccessHub");
});
app.UseStaticFiles();
// app.MapFallbackToFile("/index.html");
app.Run();
