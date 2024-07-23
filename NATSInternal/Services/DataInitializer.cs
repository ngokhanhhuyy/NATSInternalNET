using System.Security.Claims;

namespace NATSInternal.Services;

public sealed class DataInitializer
{
    private DatabaseContext _context;
    private UserManager<User> _userManager;
    private RoleManager<Role> _roleManager;

    public void InitializeData(IApplicationBuilder builder)
    {
        Randomizer.Seed = new Random(8675309);
        using IServiceScope serviceScope = builder.ApplicationServices.CreateScope();
        _context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
        _userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
        _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();
        using var transaction = _context.Database.BeginTransaction();
        InitializeRoles();
        InitializeUsers();
        InitializeRoleClaims();
        InitializeCustomers();
        InitializeCountries();
        InitializeBrands();
        InitializeProductCategories();
        InitializeProducts();
        InitializeStats();
        InitializeSupply();
        InitializeExpense();
        // InitializeOrders();
        _context.SaveChanges();
        transaction.Commit();
    }

    private void InitializeRoles()
    {
        if (!_roleManager.Roles.Any())
        {
            Console.WriteLine("Initializing roles");
            List<Role> roles = new List<Role>
            {
                new Role
                {
                    Name = "Developer",
                    DisplayName = "Nhà phát triển",
                    PowerLevel = 40
                },
                new Role
                {
                    Name = "Manager",
                    DisplayName = "Quản lý",
                    PowerLevel = 40
                },
                new Role
                {
                    Name = "Accountant",
                    DisplayName = "Kế toán",
                    PowerLevel = 30
                },
                new Role
                {
                    Name = "Staff",
                    DisplayName = "Nhân viên",
                    PowerLevel = 20
                },
            };
            foreach (Role role in roles)
            {
                IdentityResult result = _roleManager.CreateAsync(role).GetAwaiter().GetResult();
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(result.Errors.FirstOrDefault()?.Description);
                }
                _context.SaveChanges();
            }
        }
    }

    private void InitializeRoleClaims()
    {
        if (!_context.RoleClaims.Any())
        {
            Console.WriteLine("Initializing role claims");
            Dictionary<string, string[]> permissionsByRoles;
            permissionsByRoles = new Dictionary<string, string[]>
            {
                {
                    "Developer",
                    new string[] {
                        PermissionConstants.CreateUser,
                        PermissionConstants.GetOtherUserNote,
                        PermissionConstants.EditSelfPersonalInformation,
                        PermissionConstants.EditSelfUserInformation,
                        PermissionConstants.EditOtherUserPersonalInformation,
                        PermissionConstants.EditOtherUserUserInformation,
                        PermissionConstants.AssignRole,
                        PermissionConstants.ResetOtherUserPassword,
                        PermissionConstants.DeleteUser,
                        PermissionConstants.GetCustomerDetail,
                        PermissionConstants.CreateCustomer,
                        PermissionConstants.EditCustomer,
                        PermissionConstants.DeleteCustomer,
                        PermissionConstants.CreateBrand,
                        PermissionConstants.EditBrand,
                        PermissionConstants.DeleteBrand,
                        PermissionConstants.CreateProductCategory,
                        PermissionConstants.EditProductCategory,
                        PermissionConstants.DeleteProductCategory,
                        PermissionConstants.CreateProduct,
                        PermissionConstants.EditProduct,
                        PermissionConstants.DeleteProduct,
                        PermissionConstants.CreateSupply,
                        PermissionConstants.EditSupply,
                        PermissionConstants.EditLockedSupply,
                        PermissionConstants.DeleteSupply,
                        PermissionConstants.SetSupplyPaidDateTime,
                        PermissionConstants.AccessSupplyUpdateHistories,
                        PermissionConstants.CreateExpense,
                        PermissionConstants.EditExpense,
                        PermissionConstants.EditLockedExpense,
                        PermissionConstants.DeleteExpense,
                        PermissionConstants.SetExpensePaidDateTime,
                        PermissionConstants.AccessExpenseUpdateHistories,
                        PermissionConstants.CreateOrder,
                        PermissionConstants.EditOrder,
                        PermissionConstants.EditLockedOrder,
                        PermissionConstants.SetOrderPaidDateTime,
                        PermissionConstants.AccessOrderUpdateHistories,
                        PermissionConstants.DeleteOrder,
                        PermissionConstants.CreateTreatment,
                        PermissionConstants.EditTreatment,
                        PermissionConstants.EditLockedTreatment,
                        PermissionConstants.DeleteTreatment,
                        PermissionConstants.SetTreatmentPaidDateTime,
                        PermissionConstants.AccessTreatmentUpdateHistories,
                        PermissionConstants.CreateDebt,
                        PermissionConstants.EditDebt,
                        PermissionConstants.EditLockedDebt,
                        PermissionConstants.DeleteDebt,
                        PermissionConstants.SetDebtIncurredDateTime,
                        PermissionConstants.AccessDebtUpdateHistories,
                        PermissionConstants.CreateDebtPayment,
                        PermissionConstants.EditDebtPayment,
                        PermissionConstants.EditLockedDebtPayment,
                        PermissionConstants.DeleteDebtPayment,
                        PermissionConstants.SetDebtPaymentPaidDateTime,
                        PermissionConstants.AccessDebtPaymentUpdateHistories,
                        PermissionConstants.CreateConsultant,
                        PermissionConstants.EditConsultant,
                        PermissionConstants.EditLockedConsultant,
                        PermissionConstants.DeleteConsultant,
                        PermissionConstants.SetConsultantPaidDateTime,
                        PermissionConstants.AccessConsultantUpdateHistories,
                    }
                },
                {
                    "Manager",
                    new string[] {
                        PermissionConstants.CreateUser,
                        PermissionConstants.GetOtherUserNote,
                        PermissionConstants.EditSelfPersonalInformation,
                        PermissionConstants.EditSelfUserInformation,
                        PermissionConstants.EditOtherUserPersonalInformation,
                        PermissionConstants.EditOtherUserUserInformation,
                        PermissionConstants.AssignRole,
                        PermissionConstants.ResetOtherUserPassword,
                        PermissionConstants.DeleteUser,
                        PermissionConstants.GetCustomerDetail,
                        PermissionConstants.CreateCustomer,
                        PermissionConstants.EditCustomer,
                        PermissionConstants.DeleteCustomer,
                        PermissionConstants.CreateBrand,
                        PermissionConstants.EditBrand,
                        PermissionConstants.DeleteBrand,
                        PermissionConstants.CreateProductCategory,
                        PermissionConstants.EditProductCategory,
                        PermissionConstants.DeleteProductCategory,
                        PermissionConstants.CreateProduct,
                        PermissionConstants.EditProduct,
                        PermissionConstants.DeleteProduct,
                        PermissionConstants.CreateSupply,
                        PermissionConstants.EditSupply,
                        PermissionConstants.SetSupplyPaidDateTime,
                        PermissionConstants.AccessSupplyUpdateHistories,
                        PermissionConstants.DeleteSupply,
                        PermissionConstants.CreateExpense,
                        PermissionConstants.EditExpense,
                        PermissionConstants.DeleteExpense,
                        PermissionConstants.SetExpensePaidDateTime,
                        PermissionConstants.AccessExpenseUpdateHistories,
                        PermissionConstants.CreateOrder,
                        PermissionConstants.EditOrder,
                        PermissionConstants.SetOrderPaidDateTime,
                        PermissionConstants.AccessOrderUpdateHistories,
                        PermissionConstants.DeleteOrder,
                        PermissionConstants.CreateTreatment,
                        PermissionConstants.EditTreatment,
                        PermissionConstants.DeleteTreatment,
                        PermissionConstants.SetTreatmentPaidDateTime,
                        PermissionConstants.AccessTreatmentUpdateHistories,
                        PermissionConstants.CreateDebt,
                        PermissionConstants.EditDebt,
                        PermissionConstants.DeleteDebt,
                        PermissionConstants.SetDebtIncurredDateTime,
                        PermissionConstants.AccessDebtUpdateHistories,
                        PermissionConstants.CreateDebtPayment,
                        PermissionConstants.EditDebtPayment,
                        PermissionConstants.DeleteDebtPayment,
                        PermissionConstants.SetDebtPaymentPaidDateTime,
                        PermissionConstants.AccessDebtPaymentUpdateHistories,
                        PermissionConstants.CreateConsultant,
                        PermissionConstants.EditConsultant,
                        PermissionConstants.DeleteConsultant,
                        PermissionConstants.SetConsultantPaidDateTime,
                        PermissionConstants.AccessConsultantUpdateHistories,
                    }
                },
                {
                    "Accountant",
                    new string[]
                    {
                        PermissionConstants.GetOtherUserPersonalInformation,
                        PermissionConstants.GetOtherUserUserInformation,
                        PermissionConstants.EditSelfPersonalInformation,
                        PermissionConstants.GetCustomerDetail,
                        PermissionConstants.CreateCustomer,
                        PermissionConstants.CreateBrand,
                        PermissionConstants.CreateProduct,
                        PermissionConstants.EditProduct,
                        PermissionConstants.CreateSupply,
                        PermissionConstants.EditSupply,
                        PermissionConstants.CreateExpense,
                        PermissionConstants.EditExpense,
                        PermissionConstants.CreateOrder,
                        PermissionConstants.EditOrder,
                        PermissionConstants.CreateTreatment,
                        PermissionConstants.EditTreatment,
                        PermissionConstants.CreateDebt,
                        PermissionConstants.EditDebt,
                        PermissionConstants.CreateDebtPayment,
                        PermissionConstants.EditDebtPayment,
                        PermissionConstants.CreateConsultant,
                        PermissionConstants.EditConsultant,
                    }
                },
                {
                    "Staff",
                    new string[]
                    {
                        PermissionConstants.GetOtherUserPersonalInformation,
                        PermissionConstants.GetOtherUserUserInformation,
                        PermissionConstants.EditSelfPersonalInformation,
                        PermissionConstants.GetCustomerDetail,
                        PermissionConstants.CreateCustomer,
                        PermissionConstants.CreateSupply,
                        PermissionConstants.CreateExpense,
                        PermissionConstants.CreateOrder,
                        PermissionConstants.CreateTreatment,
                        PermissionConstants.CreateDebt,
                        PermissionConstants.CreateDebtPayment,
                        PermissionConstants.CreateConsultant
                    }
                }
            };

            List<Role> roles = _context.Roles.ToList();
            foreach (Role role in roles)
            {
                role.Claims = new List<IdentityRoleClaim<int>>();
                string[] permissions = permissionsByRoles[role.Name!];
                foreach (string permission in permissions)
                {
                    _roleManager
                        .AddClaimAsync(role, new Claim("Permission", permission))
                        .GetAwaiter()
                        .GetResult();
                }
            }

            _context.SaveChanges();
        }
    }

    private void InitializeUsers()
    {
        if (!_userManager.Users.Any())
        {
            Console.WriteLine("Initializing users");
            Dictionary<User, (string Password, string RoleName)> users;
            users = new Dictionary<User, (string Password, string RoleName)>
            {
                {
                    new User
                    {
                        UserName = "ngokhanhhuyy",
                        FirstName = "Ngô",
                        NormalizedFirstName = "NGO",
                        MiddleName = "Khánh",
                        NormalizedMiddleName = "KHANH",
                        LastName = "Huy",
                        NormalizedLastName = "HUY",
                        FullName = "Ngô Khánh Huy",
                        NormalizedFullName = "NGO KHANH HUY",
                        Gender = Gender.Male,
                        Birthday = new DateOnly(1997, 8, 30),
                        JoiningDate = DateOnly.FromDateTime(DateTime.Today),
                    },
                    ("Huyy47b1", "Developer")
                },
                {
                    new User
                    {
                        UserName = "thuytrangnguyen",
                        FirstName = "Nguyễn",
                        NormalizedFirstName = "NGUYEN",
                        MiddleName = "Thuỳ",
                        NormalizedMiddleName = "THUY",
                        LastName = "Trang",
                        NormalizedLastName = "TRANG",
                        FullName = "Nguyễn Thuỳ Trang",
                        NormalizedFullName = "NGUYEN THUY TRANG",
                        Gender = Gender.Female,
                        Birthday = new DateOnly(1975, 1, 17),
                        JoiningDate = DateOnly.FromDateTime(DateTime.Today),
                    },
                    ("trang123", "Manager")
                },
                {
                    new User
                    {
                        UserName = "quynhheo358",
                        FirstName = "Trần",
                        NormalizedFirstName = "TRAN",
                        MiddleName = "Nguyễn Đan",
                        NormalizedMiddleName = "NGUYEN DAN",
                        LastName = "Quỳnh",
                        NormalizedLastName = "QUYNH",
                        FullName = "Trần Nguyễn Đan Quỳnh",
                        NormalizedFullName = "TRAN NGUYEN DAN QUYNH",
                        Gender = Gender.Female,
                        Birthday = new DateOnly(2000, 10, 10),
                        JoiningDate = DateOnly.FromDateTime(DateTime.Today),
                    },
                    ("quynh123", "Accountant")
                },
                {
                    new User
                    {
                        UserName = "anhtaingo",
                        FirstName = "Ngô",
                        NormalizedFirstName = "NGO",
                        MiddleName = "Anh",
                        NormalizedMiddleName = "ANH",
                        LastName = "Tài",
                        NormalizedLastName = "TAI",
                        FullName = "Ngô Anh Tài",
                        NormalizedFullName = "NGO ANH TAI",
                        Gender = Gender.Male,
                        Birthday = new DateOnly(2007, 1, 22),
                        JoiningDate = DateOnly.FromDateTime(DateTime.Today),
                    },
                    ("tai123", "Staff")
                }
            };
            foreach (KeyValuePair<User, (string Password, string RoleName)> pair in users)
            {
                IdentityResult result = _userManager
                    .CreateAsync(pair.Key, pair.Value.Password)
                    .GetAwaiter()
                    .GetResult();
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(result.Errors.FirstOrDefault()?.Description);
                }
                result = _userManager
                    .AddToRoleAsync(pair.Key, pair.Value.RoleName)
                    .GetAwaiter()
                    .GetResult();
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(result.Errors.FirstOrDefault()?.Description);
                }
            }
        }
        _context.SaveChanges();
    }

    private void InitializeCustomers()
    {
        if (!_context.Customers.Any())
        {
            Console.WriteLine("Initializing customers");
            List<int> userIds = _context.Users.Select(u => u.Id).ToList();
            DateTime creatingDateTime = DateTime.UtcNow.ToApplicationTime().AddYears(-5);
            Faker faker = new Faker("vi");
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                int genderInt = random.Next(2);
                Bogus.DataSets.Name.Gender fakerGender;
                fakerGender = genderInt == 0
                    ? Bogus.DataSets.Name.Gender.Male
                    : Bogus.DataSets.Name.Gender.Female;
                string fullName = faker.Name.FullName(fakerGender);
                PersonNameElementsDto nameElements = PersonNameUtility
                    .GetNameElementsFromFullName(fullName);
                fullName = PersonNameUtility.GetFullNameFromNameElements(
                    nameElements.LastName,
                    nameElements.MiddleName,
                    nameElements.FirstName);
                string phoneNumber = faker.Phone.PhoneNumber();

                Customer customer = new Customer
                {
                    FirstName = nameElements.LastName,
                    NormalizedFirstName = nameElements.LastName
                        .ToNonDiacritics()
                        .ToUpper(),
                    MiddleName = nameElements.MiddleName,
                    NormalizedMiddleName = nameElements.MiddleName?
                        .ToNonDiacritics()
                        .ToUpper(),
                    LastName = nameElements.FirstName,
                    NormalizedLastName = nameElements.FirstName
                        .ToNonDiacritics()
                        .ToUpper(),
                    FullName = fullName,
                    NormalizedFullName = fullName
                        .ToNonDiacritics()
                        .ToUpper(),
                    NickName = nameElements.LastName + " " + faker.Lorem.Word(),
                    Gender = genderInt == 0 ? Gender.Male : Gender.Female,
                    Birthday = DateOnly.FromDateTime(faker.Date.Between(
                        DateTime.UtcNow.ToApplicationTime().AddYears(-20),
                        DateTime.UtcNow.ToApplicationTime().AddYears(-80))),
                    PhoneNumber = phoneNumber,
                    ZaloNumber = new string[] { phoneNumber, faker.Phone.PhoneNumber() }
                        .Skip(random.Next(3)).Take(1).SingleOrDefault(),
                    FacebookUrl = "https://facebook.com/" + faker.Internet.UserName().ToLower(),
                    Email = faker.Internet.Email(),
                    Address = faker.Address.FullAddress(),
                    CreatedDateTime = creatingDateTime,
                    Note = faker.Lorem.Paragraph(),
                    CreatedUserId = userIds.Skip(random.Next(userIds.Count)).Take(1).Single()
                };
                _context.Customers.Add(customer);
            }
            _context.SaveChanges();
        }
    }

    private void InitializeCountries()
    {
        if (!_context.Countries.Any())
        {
            Console.WriteLine("Initializing countries");
            List<Country> countries = new List<Country>
            {
                new Country { Code = "ABW", Name = "Aruba" },
                new Country { Code = "AFG", Name = "Afghanistan" },
                new Country { Code = "AGO", Name = "Angola" },
                new Country { Code = "AIA", Name = "Anguilla" },
                new Country { Code = "ALA", Name = "Quần đảo Åland" },
                new Country { Code = "ALB", Name = "Albania" },
                new Country { Code = "AND", Name = "Andorra" },
                new Country { Code = "ARE", Name = "CTVQ Ả Rập Thống nhất" },
                new Country { Code = "ARG", Name = "Argentina" },
                new Country { Code = "ARM", Name = "Armenia" },
                new Country { Code = "ASM", Name = "Samoa thuộc Mỹ" },
                new Country { Code = "ATA", Name = "Nam Cực" },
                new Country { Code = "ATF", Name = "Các vùng lãnh thổ phía Nam (Pháp)" },
                new Country { Code = "ATG", Name = "Antigua và Barbuda" },
                new Country { Code = "AUS", Name = "Úc" },
                new Country { Code = "AUT", Name = "Áo" },
                new Country { Code = "AZE", Name = "Azerbaijan" },
                new Country { Code = "BDI", Name = "Burundi" },
                new Country { Code = "BEL", Name = "Bỉ" },
                new Country { Code = "BEN", Name = "Bénin" },
                new Country { Code = "BES", Name = "Bonaire, Sint Eustatius và Saba" },
                new Country { Code = "BFA", Name = "Burkina Faso" },
                new Country { Code = "BGD", Name = "Bangladesh" },
                new Country { Code = "BGR", Name = "Bulgaria" },
                new Country { Code = "BHR", Name = "Bahrain" },
                new Country { Code = "BHS", Name = "Bahamas" },
                new Country { Code = "BIH", Name = "Bosna và Hercegovina" },
                new Country { Code = "BLM", Name = "Saint-Barthélemy" },
                new Country { Code = "BLR", Name = "Belarus" },
                new Country { Code = "BLZ", Name = "Belize" },
                new Country { Code = "BMU", Name = "Bermuda" },
                new Country { Code = "BOL", Name = "Bolivia" },
                new Country { Code = "BRA", Name = "Brasil" },
                new Country { Code = "BRB", Name = "Barbados" },
                new Country { Code = "BRN", Name = "Brunei" },
                new Country { Code = "BTN", Name = "Bhutan" },
                new Country { Code = "BVT", Name = "Đảo Bouvet" },
                new Country { Code = "BWA", Name = "Botswana" },
                new Country { Code = "CAF", Name = "Cộng hòa Trung Phi" },
                new Country { Code = "CAN", Name = "Canada" },
                new Country { Code = "CCK", Name = "Quần đảo Cocos (Keeling)" },
                new Country { Code = "CHE", Name = "Thụy Sĩ" },
                new Country { Code = "CHL", Name = "Chile" },
                new Country { Code = "CHN", Name = "Trung Quốc" },
                new Country { Code = "CIV", Name = "Bờ Biển Ngà" },
                new Country { Code = "CMR", Name = "Cameroon" },
                new Country { Code = "COD", Name = "Cộng hòa Dân chủ Congo" },
                new Country { Code = "COG", Name = "Cộng hòa Congo" },
                new Country { Code = "COK", Name = "Quần đảo Cook" },
                new Country { Code = "COL", Name = "Colombia" },
                new Country { Code = "COM", Name = "Comoros" },
                new Country { Code = "CPV", Name = "Cabo Verde" },
                new Country { Code = "CRI", Name = "Costa Rica" },
                new Country { Code = "CUB", Name = "Cuba" },
                new Country { Code = "CUW", Name = "Curaçao" },
                new Country { Code = "CXR", Name = "Đảo Giáng Sinh" },
                new Country { Code = "CYM", Name = "Quần đảo Cayman" },
                new Country { Code = "CYP", Name = "Síp" },
                new Country { Code = "CZE", Name = "Cộng hòa Séc" },
                new Country { Code = "DEU", Name = "Đức" },
                new Country { Code = "DJI", Name = "Djibouti" },
                new Country { Code = "DMA", Name = "Dominica" },
                new Country { Code = "DNK", Name = "Đan Mạch" },
                new Country { Code = "DOM", Name = "Cộng hòa Dominica" },
                new Country { Code = "DZA", Name = "Algérie" },
                new Country { Code = "ECU", Name = "Ecuador" },
                new Country { Code = "EGY", Name = "Ai Cập" },
                new Country { Code = "ERI", Name = "Eritrea" },
                new Country { Code = "ESH", Name = "Tây Sahara" },
                new Country { Code = "ESP", Name = "Tây Ban Nha" },
                new Country { Code = "EST", Name = "Estonia" },
                new Country { Code = "ETH", Name = "Ethiopia" },
                new Country { Code = "FIN", Name = "Phần Lan" },
                new Country { Code = "FJI", Name = "Fiji" },
                new Country { Code = "FLK", Name = "Quần đảo Falkland" },
                new Country { Code = "FRA", Name = "Pháp" },
                new Country { Code = "FRO", Name = "Quần đảo Faroe" },
                new Country { Code = "FSM", Name = "Liên bang Micronesia" },
                new Country { Code = "GAB", Name = "Gabon" },
                new Country { Code = "GBR", Name = "Anh Quốc" },
                new Country { Code = "GEO", Name = "Gruzia" },
                new Country { Code = "GGY", Name = "Guernsey" },
                new Country { Code = "GHA", Name = "Ghana" },
                new Country { Code = "GIB", Name = "Gibraltar" },
                new Country { Code = "GIN", Name = "Guinée" },
                new Country { Code = "GLP", Name = "Guadeloupe" },
                new Country { Code = "GMB", Name = "Gambia" },
                new Country { Code = "GNB", Name = "Guiné-Bissau" },
                new Country { Code = "GNQ", Name = "Guinea Xích Đạo" },
                new Country { Code = "GRC", Name = "Hy Lạp" },
                new Country { Code = "GRD", Name = "Grenada" },
                new Country { Code = "GRL", Name = "Greenland" },
                new Country { Code = "GTM", Name = "Guatemala" },
                new Country { Code = "GUF", Name = "Guyane thuộc Pháp" },
                new Country { Code = "GUM", Name = "Guam" },
                new Country { Code = "GUY", Name = "Guyana" },
                new Country { Code = "HKG", Name = "Hồng Kông" },
                new Country { Code = "HMD", Name = "Đảo Heard và quần đảo McDonald" },
                new Country { Code = "HND", Name = "Honduras" },
                new Country { Code = "HRV", Name = "Croatia" },
                new Country { Code = "HTI", Name = "Haiti" },
                new Country { Code = "HUN", Name = "Hungary" },
                new Country { Code = "IDN", Name = "Indonesia" },
                new Country { Code = "IMN", Name = "Đảo Man" },
                new Country { Code = "IND", Name = "Ấn Độ" },
                new Country { Code = "IOT", Name = "Lãnh thổ Ấn Độ Dương thuộc Anh" },
                new Country { Code = "IRL", Name = "Cộng hòa Ireland" },
                new Country { Code = "IRN", Name = "Iran" },
                new Country { Code = "IRQ", Name = "Iraq" },
                new Country { Code = "ISL", Name = "Iceland" },
                new Country { Code = "ISR", Name = "Israel" },
                new Country { Code = "ITA", Name = "Ý" },
                new Country { Code = "JAM", Name = "Jamaica" },
                new Country { Code = "JEY", Name = "Jersey" },
                new Country { Code = "JOR", Name = "Jordan" },
                new Country { Code = "JPN", Name = "Nhật Bản" },
                new Country { Code = "KAZ", Name = "Kazakhstan" },
                new Country { Code = "KEN", Name = "Kenya" },
                new Country { Code = "KGZ", Name = "Kyrgyzstan" },
                new Country { Code = "KHM", Name = "Campuchia" },
                new Country { Code = "KIR", Name = "Kiribati" },
                new Country { Code = "KNA", Name = "Saint Kitts và Nevis" },
                new Country { Code = "KOR", Name = "Hàn Quốc" },
                new Country { Code = "KWT", Name = "Kuwait" },
                new Country { Code = "LAO", Name = "Lào" },
                new Country { Code = "LBN", Name = "Liban" },
                new Country { Code = "LBR", Name = "Liberia" },
                new Country { Code = "LBY", Name = "Libya" },
                new Country { Code = "LCA", Name = "Saint Lucia" },
                new Country { Code = "LIE", Name = "Liechtenstein" },
                new Country { Code = "LKA", Name = "Sri Lanka" },
                new Country { Code = "LSO", Name = "Lesotho" },
                new Country { Code = "LTU", Name = "Litva" },
                new Country { Code = "LUX", Name = "Luxembourg" },
                new Country { Code = "LVA", Name = "Latvia" },
                new Country { Code = "MAC", Name = "Ma Cao" },
                new Country { Code = "MAF", Name = "Saint-Martin" },
                new Country { Code = "MAR", Name = "Maroc" },
                new Country { Code = "MCO", Name = "Monaco" },
                new Country { Code = "MDA", Name = "Moldova" },
                new Country { Code = "MDG", Name = "Madagascar" },
                new Country { Code = "MDV", Name = "Maldives" },
                new Country { Code = "MEX", Name = "México" },
                new Country { Code = "MHL", Name = "Quần đảo Marshall" },
                new Country { Code = "MKD", Name = "Bắc Macedonia" },
                new Country { Code = "MLI", Name = "Mali" },
                new Country { Code = "MLT", Name = "Malta" },
                new Country { Code = "MMR", Name = "Myanmar" },
                new Country { Code = "MNE", Name = "Montenegro" },
                new Country { Code = "MNG", Name = "Mông Cổ" },
                new Country { Code = "MNP", Name = "Quần đảo Bắc Mariana" },
                new Country { Code = "MOZ", Name = "Mozambique" },
                new Country { Code = "MRT", Name = "Mauritanie" },
                new Country { Code = "MSR", Name = "Montserrat" },
                new Country { Code = "MTQ", Name = "Martinique" },
                new Country { Code = "MUS", Name = "Mauritius" },
                new Country { Code = "MWI", Name = "Malawi" },
                new Country { Code = "MYS", Name = "Malaysia" },
                new Country { Code = "MYT", Name = "Mayotte" },
                new Country { Code = "NAM", Name = "Namibia" },
                new Country { Code = "NCL", Name = "Nouvelle-Calédonie" },
                new Country { Code = "NER", Name = "Niger" },
                new Country { Code = "NFK", Name = "Đảo Norfolk" },
                new Country { Code = "NGA", Name = "Nigeria" },
                new Country { Code = "NIC", Name = "Nicaragua" },
                new Country { Code = "NIU", Name = "Niue" },
                new Country { Code = "NLD", Name = "Hà Lan" },
                new Country { Code = "NOR", Name = "Na Uy" },
                new Country { Code = "NPL", Name = "Nepal" },
                new Country { Code = "NRU", Name = "Nauru" },
                new Country { Code = "NZL", Name = "New Zealand" },
                new Country { Code = "OMN", Name = "Oman" },
                new Country { Code = "PAK", Name = "Pakistan" },
                new Country { Code = "PAN", Name = "Panama" },
                new Country { Code = "PCN", Name = "Quần đảo Pitcairn" },
                new Country { Code = "PER", Name = "Peru" },
                new Country { Code = "PHL", Name = "Philippines" },
                new Country { Code = "PLW", Name = "Palau" },
                new Country { Code = "PNG", Name = "Papua New Guinea" },
                new Country { Code = "POL", Name = "Ba Lan" },
                new Country { Code = "PRI", Name = "Puerto Rico" },
                new Country { Code = "PRK", Name = "Bắc Triều Tiên" },
                new Country { Code = "PRT", Name = "Bồ Đào Nha" },
                new Country { Code = "PRY", Name = "Paraguay" },
                new Country { Code = "PSE", Name = "Palestine" },
                new Country { Code = "PYF", Name = "Polynésie thuộc Pháp" },
                new Country { Code = "QAT", Name = "Qatar" },
                new Country { Code = "REU", Name = "Réunion" },
                new Country { Code = "ROU", Name = "România" },
                new Country { Code = "RUS", Name = "Nga" },
                new Country { Code = "RWA", Name = "Rwanda" },
                new Country { Code = "SAU", Name = "Ả Rập Xê Út" },
                new Country { Code = "SDN", Name = "Sudan" },
                new Country { Code = "SEN", Name = "Sénégal" },
                new Country { Code = "SGP", Name = "Singapore" },
                new Country { Code = "SGS", Name = "Nam Georgia và Q.đ. Nam Sandwich" },
                new Country { Code = "SHN", Name = "Saint Helena, Ascension và T.d.C." },
                new Country { Code = "SJM", Name = "Svalbard và Jan Mayen" },
                new Country { Code = "SLB", Name = "Quần đảo Solomon" },
                new Country { Code = "SLE", Name = "Sierra Leone" },
                new Country { Code = "SLV", Name = "El Salvador" },
                new Country { Code = "SMR", Name = "San Marino" },
                new Country { Code = "SOM", Name = "Somalia" },
                new Country { Code = "SPM", Name = "Saint-Pierre và Miquelon" },
                new Country { Code = "SRB", Name = "Serbia" },
                new Country { Code = "SSD", Name = "Nam Sudan" },
                new Country { Code = "STP", Name = "São Tomé và Príncipe" },
                new Country { Code = "SUR", Name = "Suriname" },
                new Country { Code = "SVK", Name = "Slovakia" },
                new Country { Code = "SVN", Name = "Slovenia" },
                new Country { Code = "SWE", Name = "Thụy Điển" },
                new Country { Code = "SWZ", Name = "Eswatini" },
                new Country { Code = "SXM", Name = "Sint Maarten" },
                new Country { Code = "SYC", Name = "Seychelles" },
                new Country { Code = "SYR", Name = "Syria" },
                new Country { Code = "TCA", Name = "Quần đảo Turks và Caicos" },
                new Country { Code = "TCD", Name = "Tchad" },
                new Country { Code = "TGO", Name = "Togo" },
                new Country { Code = "THA", Name = "Thái Lan" },
                new Country { Code = "TJK", Name = "Tajikistan" },
                new Country { Code = "TKL", Name = "Tokelau" },
                new Country { Code = "TKM", Name = "Turkmenistan" },
                new Country { Code = "TLS", Name = "Đông Timor" },
                new Country { Code = "TON", Name = "Tonga" },
                new Country { Code = "TTO", Name = "Trinidad và Tobago" },
                new Country { Code = "TUN", Name = "Tunisia" },
                new Country { Code = "TUR", Name = "Thổ Nhĩ Kỳ" },
                new Country { Code = "TUV", Name = "Tuvalu" },
                new Country { Code = "TWN", Name = "Đài Loan" },
                new Country { Code = "TZA", Name = "Tanzania" },
                new Country { Code = "UGA", Name = "Uganda" },
                new Country { Code = "UKR", Name = "Ukraina" },
                new Country { Code = "UMI", Name = "Các tiểu đảo xa của Hoa Kỳ" },
                new Country { Code = "URY", Name = "Uruguay" },
                new Country { Code = "USA", Name = "Hoa Kỳ" },
                new Country { Code = "UZB", Name = "Uzbekistan" },
                new Country { Code = "VAT", Name = "Thành Vatican" },
                new Country { Code = "VCT", Name = "Saint Vincent và Grenadines" },
                new Country { Code = "VEN", Name = "Venezuela" },
                new Country { Code = "VGB", Name = "Quần đảo Virgin thuộc Anh" },
                new Country { Code = "VIR", Name = "Quần đảo Virgin thuộc Mỹ" },
                new Country { Code = "VNM", Name = "Việt Nam" },
                new Country { Code = "VUT", Name = "Vanuatu" },
                new Country { Code = "WLF", Name = "Wallis và Futuna" },
                new Country { Code = "WSM", Name = "Samoa" },
                new Country { Code = "YEM", Name = "Yemen" },
                new Country { Code = "ZAF", Name = "Cộng hòa Nam Phi" },
                new Country { Code = "ZMB", Name = "Zambia" },
                new Country { Code = "ZWE", Name = "Zimbabwe" },
            };

            foreach (Country country in countries)
            {
                _context.Countries.Add(country);
                _context.SaveChanges();
            }
        }
    }

    private void InitializeBrands()
    {
        if (!_context.Brands.Any())
        {
            Console.WriteLine("Initializing brands");
            Faker faker = new Faker("vi");
            Random random = new Random();
            List<int> countryIds = _context.Countries.Select(c => c.Id).ToList();
            for (int i = 0; i < 10; i++)
            {
                string name;

                do
                {
                    name = faker.Company.CompanyName();
                    if (!_context.Brands.Any(b => b.Name == name))
                    {
                        break;
                    }
                } while (true);

                Brand brand = new Brand
                {
                    Name = name,
                    Website = ValueOrNull(faker.Internet.Url().ToNonDiacritics()),
                    SocialMediaUrl = ValueOrNull(RandomSocialMediaUrl(faker)),
                    PhoneNumber = ValueOrNull(faker.Phone.PhoneNumber()),
                    Email = ValueOrNull(faker.Internet.Email()),
                    Address = ValueOrNull(faker.Address.StreetAddress()),
                    CountryId = countryIds
                        .Skip(random.Next(countryIds.Count()))
                        .Take(1)
                        .Single()
                };
                _context.Brands.Add(brand);
                _context.SaveChanges();
            }
        }
    }

    private void InitializeProductCategories()
    {
        if (!_context.ProductCategories.Any())
        {
            Console.WriteLine("Initializing product categories");
            Faker faker = new Faker();
            for (int i = 0; i < 5; i++)
            {
                // Generate unique name.
                string name;
                do
                {
                    string randomName = faker.Commerce.Categories(1).Single();
                    name = CapitalizeFirstLetterEachWord(randomName);


                    // Ensure the name is unique.
                    if (!_context.ProductCategories.Any(pc => pc.Name == name))
                    {
                        break;
                    }
                } while (true);

                ProductCategory category = new ProductCategory
                {
                    Name = name,
                    CreatedDateTime = DateTime.UtcNow.ToApplicationTime()
                };

                _context.ProductCategories.Add(category);
            }

            _context.SaveChanges();
        }
    }

    private void InitializeProducts()
    {
        if (!_context.Products.Any())
        {
            string[] units = { "Cái", "Chai", "Lọ", "Hộp", "Vĩ" };
            Console.WriteLine("Initializing products");
            Faker enFaker = new Faker();
            Faker viFaker = new Faker("vi");
            Random random = new Random();
            List<int> categoryIds = _context.ProductCategories
                .Select(pc => pc.Id)
                .ToList();
            List<int> brandIds = _context.Brands
                .Select(b => b.Id)
                .ToList();
            for (int i = 0; i < 30; i++)
            {
                string name;
                do
                {
                    string randomName = enFaker.Lorem
                        .Sentence(2, 2)
                        .Replace(".", "");
                    name = CapitalizeFirstLetterEachWord(randomName);

                    if (!_context.Products.Any(p => p.Name == name))
                    {
                        break;
                    }
                } while (true);

                string description = SliceIfTooLong(viFaker.Lorem.Paragraphs(5), 1000);
                Product product = new Product
                {
                    Name = name,
                    Description = ValueOrNull(description),
                    Unit = units.Skip(random.Next(units.Length)).Take(1).Single(),
                    Price = random.Next(75, 500) * 1000,
                    VatFactor = 0.1M,
                    IsForRetail = random.Next(10) < 7,
                    IsDiscontinued = false,
                    CreatedDateTime = DateTime.UtcNow.ToApplicationTime(),
                    BrandId = brandIds
                        .Skip(random.Next(brandIds.Count))
                        .Take(1)
                        .Single(),
                    CategoryId = categoryIds
                        .Skip(random.Next(categoryIds.Count))
                        .Take(1)
                        .Single()
                };
                _context.Products.Add(product);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine(product.BrandId);
                    throw;
                }
            }
        }
    }

    private void InitializeSupply()
    {
        if (!_context.Supplies.Any())
        {
            Console.WriteLine("Initializing supplies");
            Random random = new();
            Faker faker = new Faker("vi");
            DateTime startingDateTime = new(
                DateOnly.FromDateTime(DateTime.UtcNow.ToApplicationTime().AddMonths(-6)),
                new TimeOnly(8, 0, 0));
            DateTime endingDateTime = DateTime.UtcNow.ToApplicationTime();
            DateTime currentDateTime = startingDateTime;
            List<Product> products = _context.Products.ToList();
            List<int> userIds = _context.Users
                .Include(u => u.Roles).ThenInclude(r => r.Claims)
                .Where(u => u.Roles.Any(r => r.Claims.Select(c => c.ClaimValue).Contains(PermissionConstants.CreateSupply)))
                .Select(u => u.Id)
                .ToList();
            while (currentDateTime <= endingDateTime)
            {
                List<SupplyItem> supplyItems = [];
                int itemCount = random.Next(4, products.Count);
                for (int i = 0; i < itemCount; i++)
                {
                    Product product;
                    product = products
                        .Where(p => !supplyItems.Select(si => si.ProductId).Contains(p.Id))
                        .OrderBy(_ => random.Next(1000))
                        .Take(1)
                        .Single();

                    SupplyItem supplyItem = new()
                    {
                        Amount = product.Price,
                        SuppliedQuantity = random.Next(50),
                        ProductId = product.Id
                    };

                    product.StockingQuantity += supplyItem.SuppliedQuantity;
                    supplyItems.Add(supplyItem);
                }

                Supply supply = new()
                {
                    PaidDateTime = currentDateTime,
                    ShipmentFee = 0,
                    Note = faker.Lorem.Sentences(5),
                    CreatedDateTime = currentDateTime,
                    CreatedUserId = userIds.Skip(random.Next(userIds.Count)).Take(1).Single(),
                    Items = supplyItems
                };

                _context.Supplies.Add(supply);

                DateTime nextDateTime;
                do
                {
                    nextDateTime = currentDateTime.AddHours(random.Next(24 * 9, 24 * 10));
                }
                while (!(nextDateTime.Hour >= 8 && nextDateTime.Hour <= 17));
                currentDateTime = nextDateTime;
            }
            _context.SaveChanges();
        }
    }

    private void InitializeExpense()
    {
        if (!_context.Expenses.Any())
        {
            Console.WriteLine("Initializing expenses.");
            Random random = new Random();
            Faker faker = new Faker();
            List<int> userIds = _context.Users.Select(u => u.Id).ToList();
            DateTime endingDateTime = DateTime.UtcNow.ToApplicationTime();
            DateTime currentDateTime = endingDateTime.AddMonths(-6);
            while (currentDateTime < endingDateTime)
            {
                // Generating category.
                ExpenseCategory category = Enum.GetValues(typeof(ExpenseCategory))
                    .OfType<ExpenseCategory>()
                    .MinBy(_ => Guid.NewGuid());
                
                // Generating company name.
                string companyName = faker.Company.CompanyName();
                ExpensePayee payee = _context.ExpensePayees.SingleOrDefault(e => e.Name == companyName);
                if (payee is null)
                {
                    payee = new ExpensePayee
                    {
                        Name = companyName
                    };
                    _context.ExpensePayees.Add(payee);
                }
                
                // Generating expense.
                Expense expense = new Expense
                {
                    Amount = random.Next(500, 5000) * 1000,
                    PaidDateTime = currentDateTime,
                    Category = category,
                    Note = null,
                    IsClosed = ShouldOfficiallyClose(currentDateTime),
                    CreatedUserId = userIds.Skip(random.Next(userIds.Count)).Take(1).Single(),
                    Payee = payee
                };
                _context.Expenses.Add(expense);
                
                do
                {
                    currentDateTime = currentDateTime.AddHours(random.Next(36, 72));
                } while (currentDateTime.Hour is < 8 or > 17);
            }
            
            _context.SaveChanges();
        }
    }

    private void InitializeOrders()
    {
        if (!_context.Orders.Any())
        {
            Console.WriteLine("Initializing orders.");
            Random random = new Random();
            DateTime maxOrderedDateTime = DateTime.UtcNow.ToApplicationTime();
            DateTime currentDateTime = maxOrderedDateTime
                .AddMonths(-6);
            List<int> customerIds = _context.Customers.Select(c => c.Id).ToList();
            List<Product> products = _context.Products.ToList();
            List<int> userIds = _context.Users
                .Include(u => u.Roles).ThenInclude(r => r.Claims)
                .Where(u => u.Roles
                    .Single()
                    .Claims
                    .Select(c => c.ClaimValue)
                    .Contains(PermissionConstants.CreateOrder))
                .Select(u => u.Id)
                .ToList();
            while (currentDateTime < maxOrderedDateTime)
            {
                // Determine datetime
                do
                {
                    currentDateTime = currentDateTime.AddMinutes(random.Next(120, 360));
                } while (currentDateTime.Hour < 8 || currentDateTime.Hour > 17);

                // Initialize order.
                Order order = new Order
                {
                    PaidDateTime = currentDateTime,
                    Note = null,
                    IsClosed = ShouldOfficiallyClose(currentDateTime),
                    CustomerId = customerIds.MinBy(_ => Guid.NewGuid()),
                    CreatedUserId = userIds.MinBy(_ => Guid.NewGuid()),
                    Items = new List<OrderItem>()
                };
                _context.Orders.Add(order);

                // Initialize order items.
                int orderItemCount = random.Next(3, 10);
                List<int> pickedProductIds = new List<int>();
                for (int i = 0; i < orderItemCount; i++)
                {
                    // Determine product.
                    Product product = products
                        .OrderBy(_ => Guid.NewGuid())
                        .Where(p => p.StockingQuantity > 0 && !pickedProductIds.Contains(p.Id))
                        .First();
                    pickedProductIds.Add(product.Id);

                    OrderItem item = new OrderItem
                    {
                        Amount = product.Price,
                        VatFactor = 0,
                        Quantity = Math.Min(5, product.StockingQuantity),
                        ProductId = product.Id
                    };
                    order.Items.Add(item);
                }
            }

            _context.SaveChanges();
        }
    }

    private void InitializeStats()
    {
        Console.WriteLine("Initializing stats ...");
        DateOnly minimumDate = DateOnly.FromDateTime(GetMinimumResourceDateTimeToBeOpened());
        DateOnly maximumDate = DateOnly.FromDateTime(DateTime.UtcNow.ToApplicationTime().AddMonths(3));

        // Generating a list of date to check if there is any date not existing in the database.
        List<DateOnly> dateList = new List<DateOnly>();
        DateOnly generatingDate = minimumDate;
        while (generatingDate <= maximumDate)
        {
            dateList.Add(generatingDate);
            generatingDate = generatingDate.AddDays(1);
        }

        // Fetch a list of existing dates in the database.
        List<DateOnly> existingDates = _context.DailyStats.Select(ds => ds.RecordedDate).ToList();

        // Check if there is any date which doesn't exist in the database.
        IEnumerable<DateOnly> notExistingDates = dateList.Except(existingDates);

        // Generating stats of dates which doesn't exist in the database.
        foreach (DateOnly date in notExistingDates)
        {
            // Determining temporarily closed datetime.
            DateTime? temporarilyClosedDateTime = null;
            if (ShouldTemporarilyCloseStats(date))
            {
                temporarilyClosedDateTime = new DateTime(date, new TimeOnly(2, 30, 0));
            }

            // Determining officially closed datetime.
            DateTime? officiallyClosedDateTime = null;
            if (ShouldOfficiallyCloseStats(date))
            {
                officiallyClosedDateTime = new DateTime(date, new TimeOnly(2, 30, 0));
            }

            // Initialize daily stats.
            DailyStats dailyStats = new DailyStats
            {
                RecordedDate = date,
                TemporarilyClosedDateTime = temporarilyClosedDateTime,
                OfficiallyClosedDateTime = officiallyClosedDateTime
            };

            // Initialize monthly stats if not exists.
            MonthlyStats monthlyStats = _context.MonthlyStats
                .Where(ms => ms.RecordedYear == date.Year)
                .Where(ms => ms.RecordedMonth == date.Month)
                .SingleOrDefault();
            if (monthlyStats == null)
            {
                monthlyStats = new MonthlyStats
                {
                    RecordedYear = date.Year,
                    RecordedMonth = date.Month,
                    DailyStats = new List<DailyStats>()
                };
                _context.MonthlyStats.Add(monthlyStats);
                _context.SaveChanges();
            }

            // Link the daily stats to the monthly stats.
            if (monthlyStats.DailyStats == null)
            {
                monthlyStats.DailyStats = new List<DailyStats>();
            }
            monthlyStats.DailyStats.Add(dailyStats);

            // Close the monthly stats if the daily stats is closed.
            if (dailyStats.TemporarilyClosedDateTime.HasValue)
            {
                monthlyStats.TemporarilyClosedDateTime = dailyStats.TemporarilyClosedDateTime;
            }

            if (dailyStats.OfficiallyClosedDateTime.HasValue)
            {
                monthlyStats.OfficiallyClosedDateTime = dailyStats.OfficiallyClosedDateTime;
            }
        }

        _context.SaveChanges();
    }

    private static T ValueOrNull<T>(T value)
    {
        Random random = new Random();

        // For value types, handle nullable explicitly
        if (typeof(T).IsValueType)
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // T is already a nullable value type (e.g., int?)
                return random.Next(2) == 0 ? value : default;
            }
            else
            {
                // T is a non-nullable value type (e.g., int)
                return random.Next(2) == 0 ? value : default;
            }
        }
        else
        {
            // For reference types, return null directly
            return random.Next(2) == 0 ? value : default;
        }
    }

    private static string RandomSocialMediaUrl(Faker faker)
    {
        string[] socialMediaDomains =
        {
            "https://facebook.com/",
            "https://twitter.com/",
            "https://thread.com/",
            "https://instagram.com/"
        };

        string domain = socialMediaDomains
            .Skip(new Random().Next(socialMediaDomains.Length))
            .Take(1)
            .Single();

        return domain + faker.Internet.UserName().ToLower();
    }

    private static string CapitalizeFirstLetterEachWord(string value)
    {

        string[] nameSegments = value.Split(" ");
        List<string> capitalizedSegments = new List<string>();
        foreach (string segment in nameSegments)
        {
            string capitalizedNameSegment = segment[0].ToString();
            if (segment.Length > 1)
            {
                capitalizedNameSegment += segment.Substring(1, segment.Length - 1);
            }
            capitalizedSegments.Add(capitalizedNameSegment);
        }
        return string.Join(" ", capitalizedSegments);
    }

    private static string SliceIfTooLong(string value, int maxLength)
    {
        if (value.Length < maxLength)
        {
            return value;
        }

        return value[..maxLength];
    }

    private static DateTime GetMinimumResourceDateTimeToBeOpened()
    {
        DateTime minimumOpenedDateTime;
        if (DateTime.UtcNow.ToApplicationTime().Day >= 4 && DateTime.UtcNow.ToApplicationTime().Hour >= 1)
        {
            minimumOpenedDateTime = new DateTime(
                DateTime.UtcNow.ToApplicationTime().AddMonths(-1).Year,
                DateTime.UtcNow.ToApplicationTime().AddMonths(-1).Month,
                1,
                0, 0, 0);
        }
        else
        {
            minimumOpenedDateTime = new DateTime(
                DateTime.UtcNow.ToApplicationTime().AddMonths(-2).Year,
                DateTime.UtcNow.ToApplicationTime().AddMonths(-2).Month,
                1,
                0, 0, 0);
        }

        return minimumOpenedDateTime;
    }

    private static DateTime GetMaximumTemporarilyClosedDateTime()
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        DateTime closingDateTime = new DateTime(
            currentDateTime.Year, currentDateTime.Month, 4,
            2, 30, 0);
        
        return closingDateTime;
    }

    private static bool ShouldOfficiallyClose(DateTime resouceDateTime)
    {
        if (resouceDateTime < GetMinimumResourceDateTimeToBeOpened())
        {
            return true;
        }
        return false;
    }

    private static bool ShouldTemporarilyCloseStats(DateOnly recordedDate)
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        DateTime closingDateTime = new DateTime(
            currentDateTime.Year, currentDateTime.Month, 4,
            2, 30, 0);
        
        // Check if the closing operation in this month has been executed.
        // If it HASN'T, closing all stats of the months which are equal to or earlier than 2 months ago.
        if (currentDateTime < closingDateTime)
        {
            if (recordedDate.Year < currentDateTime.AddMonths(-2).Year)
            {
                return true;
            }

            if (recordedDate.Year == currentDateTime.AddMonths(-2).Year &&
                recordedDate.Month <= currentDateTime.AddMonths(-2).Month)
            {
                return true;
            }
            
            return false;
        }

        // If the closing operation HAS been executed, closing all stats of the months which are
        // equal to or earlier than 1 month ago.
        if (recordedDate.Year < currentDateTime.AddMonths(-1).Year)
        {
            return true;
        }

        if (recordedDate.Year == currentDateTime.AddMonths(-1).Year &&
            recordedDate.Month <= currentDateTime.AddMonths(-1).Month)
        {
            return true;
        }
        
        return false;
    }

    private static bool ShouldOfficiallyCloseStats(DateOnly recordedDate)
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        DateTime closingDateTime = new DateTime(
            currentDateTime.Year, currentDateTime.Month, 4,
            2, 30, 0);
        
        // Check if the closing operation in this month has been executed.
        // If it HASN'T, closing all stats of the months which are equal to or earlier than 3 months ago.
        if (currentDateTime < closingDateTime)
        {
            if (recordedDate.Year < currentDateTime.AddMonths(-3).Year)
            {
                return true;
            }

            if (recordedDate.Year == currentDateTime.AddMonths(-3).Year &&
                recordedDate.Month <= currentDateTime.AddMonths(-3).Month)
            {
                return true;
            }
            
            return false;
        }

        // If the closing operation HAS been executed, closing all stats of the months which are
        // equal to or earlier than 2 month ago.
        if (recordedDate.Year < currentDateTime.AddMonths(-2).Year)
        {
            return true;
        }

        if (recordedDate.Year == currentDateTime.AddMonths(-2).Year &&
            recordedDate.Month <= currentDateTime.AddMonths(-2).Month)
        {
            return true;
        }
        
        return false;
    }
}