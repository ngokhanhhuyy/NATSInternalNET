using NATSInternal.Services.Dtos;

namespace NATSInternal.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly DatabaseContext _context;
    private User _user;

    public AuthorizationService(DatabaseContext context)
    {
        _context = context;
    }
    public async Task SetUserId(int id)
    {
        _user = await _context.Users
            .Include(u => u.Roles).ThenInclude(r => r.Claims)
            .Where(u => u.Id == id)
            .SingleAsync();
    }
    public int GetUserId()
    {
        return _user.Id;
    }

    public UserDetailResponseDto GetUserDetail()
    {
        UserDetailResponseDto responseDto = new UserDetailResponseDto
        {
            Id = _user.Id,
            UserName = _user.UserName,
            PersonalInformation = new UserPersonalInformationResponseDto
            {

                FirstName = _user.FirstName,
                MiddleName = _user.MiddleName,
                LastName = _user.LastName,
                FullName = _user.FullName,
                Gender = _user.Gender,
                Birthday = _user.Birthday,
                PhoneNumber = _user.PhoneNumber,
                Email = _user.Email,
                AvatarUrl = _user.AvatarUrl,
            },
            UserInformation = new UserUserInformationResponseDto
            {
                JoiningDate = _user.JoiningDate,
                CreatedDateTime = _user.CreatedDateTime,
                UpdatedDateTime = _user.UpdatedDateTime,
                Note = _user.Note,
                Role = new RoleDetailResponseDto
                {
                    Id = _user.Role.Id,
                    Name = _user.Role.Name,
                    DisplayName = _user.Role.DisplayName,
                    PowerLevel = _user.Role.PowerLevel,
                    Permissions = _user.Role.Claims
                    .Where(c => c.ClaimType == "Permission")
                    .Select(c => c.ClaimValue)
                    .ToList()
                }
            }
        };

        return responseDto;
    }

    // Authorization for users.
    public UserListAuthorizationResponseDto GetUserListAuthorization()
    {
        return new()
        {
            CanCreate = _user.HasPermission(PermissionConstants.CreateUser)
        };
    }

    public UserBasicAuthorizationResponseDto GetUserBasicAuthorization(User targetUser)
    {
        return new()
        {
            CanEdit = CanEditUserPersonalInformation(targetUser) ||
                CanEditUserUserInformation(targetUser),
            CanChangePassword = CanChangeUserPassword(targetUser),
            CanResetPassword = CanResetUserPassword(targetUser),
            CanDelete = CanDeleteUser(targetUser)
        };
    }

    public UserDetailAuthorizationResponseDto GetUserDetailAuthorization(User targetUser)
    {
        UserBasicAuthorizationResponseDto basicResponseDto;
        basicResponseDto = GetUserBasicAuthorization(targetUser);
        return new()
        {
            CanGetNote = CanGetNote(targetUser.PowerLevel),
            CanEdit = basicResponseDto.CanEdit,
            CanEditUserPersonalInformation = CanEditUserPersonalInformation(targetUser),
            CanEditUserUserInformation = CanEditUserUserInformation(targetUser),
            CanAssignRole = CanAssignRole(),
            CanChangePassword = CanChangeUserPassword(targetUser),
            CanResetPassword = CanResetUserPassword(targetUser),
            CanDelete = CanDeleteUser(targetUser)
        };
    }

    // Authorization for customers.
    public CustomerListAuthorizationResponseDto GetCustomerListAuthorization()
    {
        return new CustomerListAuthorizationResponseDto
        {
            CanCreate = _user.HasPermission(PermissionConstants.CreateCustomer)
        };
    }

    public CustomerAuthorizationResponseDto GetCustomerAuthorization(Customer customer)
    {
        return new CustomerAuthorizationResponseDto
        {
            CanEdit = _user.HasPermission(PermissionConstants.EditCustomer),
            CanDelete = _user.HasPermission(PermissionConstants.DeleteCustomer)
        };
    }

    // Authorization for brands.
    public BrandListAuthorizationResponseDto GetBrandListAuthorization()
    {
        return new BrandListAuthorizationResponseDto
        {
            CanCreate = _user.HasPermission(PermissionConstants.CreateBrand)
        };
    }

    public BrandAuthorizationResponseDto GetBrandAuthorization()
    {
        return new()
        {
            CanEdit = _user.HasPermission(PermissionConstants.EditBrand),
            CanDelete = _user.HasPermission(PermissionConstants.DeleteBrand)
        };
    }

    // Authorization for products.
    public ProductListAuthorizationResponseDto GetProductListAuthorization()
    {
        return new ProductListAuthorizationResponseDto
        {
            CanCreate = _user.HasPermission(PermissionConstants.CreateProduct)
        };
    }

    public ProductAuthorizationResponseDto GetProductAuthorization(Product product)
    {
        return new ProductAuthorizationResponseDto
        {
            CanEdit = _user.HasPermission(PermissionConstants.EditProduct),
            CanDelete = _user.HasPermission(PermissionConstants.DeleteProduct)
        };
    }

    // Authorization for product categories.
    public ProductCategoryAuthorizationResponseDto GetProductCategoryAuthorization()
    {
        return new()
        {
            CanCreate = _user.HasPermission(PermissionConstants.CreateProductCategory),
            CanEdit = _user.HasPermission(PermissionConstants.EditProductCategory),
            CanDelete = _user.HasPermission(PermissionConstants.DeleteProductCategory)
        };
    }

    // Authorization for supplies.
    public SupplyListAuthorizationResponseDto GetSupplyListAuthorization()
    {
        return new SupplyListAuthorizationResponseDto
        {
            CanCreate = CanCreateSupply()
        };
    }

    public SupplyAuthorizationResponseDto GetSupplyAuthorization(Supply supply)
    {
        return new()
        {
            CanEdit = CanEditSupply(supply),
            CanDelete = CanEditSupply(supply)
        };
    }
    
    // Authorization for expenses.
    public ExpenseListAuthorizationResponseDto GetExpenseListAuthorization()
    {
        return new ExpenseListAuthorizationResponseDto
        {
            CanCreate = _user.HasPermission(PermissionConstants.CreateExpense)
        };
    }

    public ExpenseAuthorizationResponseDto GetExpenseAuthorization(Expense expense)
    {
        return new ExpenseAuthorizationResponseDto
        {
            CanEdit = CanEditExpense(expense),
            CanDelete = CanDeleteExpense(expense),
            CanSetPaidDateTime = CanSetExpensePaidDateTime()
        };
    }

    // Authorization for orders.
    public OrderListAuthorizationResponseDto GetOrderListAuthorization()
    {
        return new OrderListAuthorizationResponseDto
        {
            CanCreate = CanCreateOrder()
        };
    }

    public OrderAuthorizationResponseDto GetOrderAuthorization(Order order)
    {
        return new OrderAuthorizationResponseDto
        {
            CanEdit = CanEditOrder(order),
            CanDelete = CanDeleteOrder(order)
        };
    }
    
    // Authorization for treatments.
    public TreatmentListAuthorizationResponseDto GetTreatmentListAuthorization()
    {
        return new TreatmentListAuthorizationResponseDto
        {
            CanCreate = CanCreateTreatment()
        };
    }
    
    public TreatmentAuthorizationResponseDto GetTreatmentAuthorization(Treatment treatment)
    {
        return new TreatmentAuthorizationResponseDto
        {
            CanEdit = CanEditTreatment(treatment),
            CanDelete = CanDeleteTreatment(),
            CanSetOrderedDateTime = CanSetTreatmentOrderedDateTime()
        };
    }

    // Authorization for debts.
    public DebtListAuthorizationResponseDto GetDebtListAuthorization()
    {
        return new DebtListAuthorizationResponseDto
        {
            CanCreate = CanCreateDebt()
        };
    }
    
    public DebtAuthorizationResponseDto GetDebtAuthorization(Debt debt)
    {
        return new DebtAuthorizationResponseDto
        {
            CanEdit = CanEditDebt(debt),
            CanDelete = CanDeleteDebt(),
            CanSetCreatedDateTime = CanSetDebtCreatedDateTime()
        };
    }

    // Authorization for debt payments.
    public DebtPaymentListAuthorizationResponseDto GetDebtPaymentListAuthorization()
    {
        return new DebtPaymentListAuthorizationResponseDto
        {
            CanCreate = CanCreateDebtPayment()
        };
    }

    public DebtPaymentAuthorizationResponseDto GetDebtPaymentAuthorization(DebtPayment debtPayment)
    {
        return new DebtPaymentAuthorizationResponseDto
        {
            CanEdit = CanEditDebtPayment(debtPayment),
            CanDelete = CanDeleteDebtPayment()
        };
    }

    // Authorization for consultants.
    public ConsultantListAuthorizationResponseDto GetConsultantListAuthorization()
    {
        return new ConsultantListAuthorizationResponseDto
        {
            CanCreate = CanCreateConsultant()
        };
    }

    public ConsultantAuthorizationResponseDto GetConsultantAuthorization(Consultant consultant)
    {
        return new ConsultantAuthorizationResponseDto
        {
            CanEdit = CanEditConsultant(consultant),
            CanDelete = CanDeleteConsultant(),
            CanSetPaidDateTime = CanSetConsultantPaidDateTime()
        };
    }

    // Permissions to interact with users.
    public bool CanEditUserPersonalInformation(User targetUser)
    {
        // Check permission when the user is editing himself.
        if (_user.Id == targetUser.Id &&
            _user.HasPermission(PermissionConstants.EditSelfPersonalInformation))
        {
            return true;
        }

        // Check permission when the user is editing another user.
        else if (_user.HasPermission(PermissionConstants.EditOtherUserPersonalInformation) &&
                _user.PowerLevel > targetUser.PowerLevel)
        {
            return true;
        }

        return false;
    }

    public bool CanEditUserUserInformation(User targetUser)
    {
        // Check permission when the user is editing himself.
        if (_user.Id == targetUser.Id &&
            _user.HasPermission(PermissionConstants.EditSelfUserInformation))
        {
            return true;
        }

        // Check permission when the user is editing another user.
        else if (_user.HasPermission(PermissionConstants.EditOtherUserUserInformation) &&
                _user.PowerLevel > targetUser.PowerLevel)
        {
            return true;
        }

        return false;
    }

    public bool CanChangeUserPassword(User targetUser)
    {
        return _user.Id == targetUser.Id;
    }

    public bool CanResetUserPassword(User targetUser)
    {
        return _user.Id != targetUser.Id &&
            _user.HasPermission(PermissionConstants.ResetOtherUserPassword) &&
            _user.PowerLevel > targetUser.PowerLevel;
    }

    public bool CanDeleteUser(User targetUser)
    {
        return _user.Id != targetUser.Id &&
            _user.HasPermission(PermissionConstants.DeleteUser) &&
            !_user.IsDeleted &&
            _user.PowerLevel > targetUser.PowerLevel;
    }

    public bool CanRestoreUser(User targetUser)
    {
        return _user.Id != targetUser.Id &&
                _user.IsDeleted &&
                _user.HasPermission(PermissionConstants.RestoreUser);
    }

    public bool CanAssignToRole(Role role)
    {
        return _user.Role.Name == RoleConstants.Developer ||
            _user.Role.Name == RoleConstants.Manager ||
            _user.PowerLevel > role.PowerLevel;
    }

    public bool CanAssignRole()
    {
        return _user.HasPermission(PermissionConstants.AssignRole);
    }

    public bool CanGetNote(int powerLevel)
    {
        return _user.HasPermission(PermissionConstants.GetOtherUserNote) &&
            _user.PowerLevel > powerLevel;
    }

    // Permissions to interact with supplies.
    public bool CanCreateSupply()
    {
        return _user.HasPermission(PermissionConstants.CreateSupply);
    }

    public bool CanEditSupply(Supply supply)
    {
        if (!_user.HasPermission(PermissionConstants.EditSupply))
        {
            return false;
        }

        if (supply.IsClosed && !_user.HasPermission(PermissionConstants.EditClosedSupply))
        {
            return false;
        }

        return true;
    }

    public bool CanDeleteSupply(Supply supply)
    {
        if (!_user.HasPermission(PermissionConstants.DeleteSupply))
        {
            return false;
        }

        if (supply.IsClosed)
        {
            return false;
        }

        return true;
    }

    public bool CanSetSupplySuppliedDateTime()
    {
        return _user.HasPermission(PermissionConstants.SetSupplySuppliedDateTime);
    }

    public bool CanEditSupplyItems()
    {
        return _user.HasPermission(PermissionConstants.EditSupplyItem);
    }

    public bool CanEditSupplyPhotos()
    {
        return _user.HasPermission(PermissionConstants.EditSupplyPhoto);
    }
    
    // Permissions to interact with expenses.
    public bool CanEditExpense(Expense expense)
    {
        if (!_user.HasPermission(PermissionConstants.EditExpense))
        {
            return false;
        }
        
        if (expense.IsClosed && !_user.HasPermission(PermissionConstants.EditClosedExpense))
        {
            return false;
        }
        
        return true;
    }
    
    public bool CanDeleteExpense(Expense expense)
    {
        return _user.HasPermission(PermissionConstants.DeleteExpense) && !expense.IsClosed;
    }

    public bool CanSetExpensePaidDateTime()
    {
        return _user.HasPermission(PermissionConstants.SetExpensePaidDateTime);
    }
    
    // Permissions to interact with orders.
    public bool CanCreateOrder()
    {
        return _user.HasPermission(PermissionConstants.CreateOrder);
    }

    public bool CanEditOrder(Order order)
    {
        if (!_user.HasPermission(PermissionConstants.EditOrder))
        {
            return false;
        }
        
        if (order.IsClosed && !_user.HasPermission(PermissionConstants.EditClosedOrder))
        {
            return false;
        }
        
        return true;
    }
    
    public bool CanDeleteOrder(Order order)
    {
        return !order.IsClosed && _user.HasPermission(PermissionConstants.DeleteOrder);
    }

    public bool CanSetOrderOrderedDateTime()
    {
        return _user.HasPermission(PermissionConstants.SetOrderOrderedDateTime);
    }
    
    // Permissions to interact with treatments.
    public bool CanCreateTreatment()
    {
        return _user.HasPermission(PermissionConstants.CreateTreatment);
    }
    
    public bool CanEditTreatment(Treatment treatment)
    {
        if (!_user.HasPermission(PermissionConstants.EditTreatment))
        {
            return false;
        }
        
        if (treatment.IsClosed && !_user.HasPermission(PermissionConstants.EditClosedTreatment))
        {
            return false;
        }
        
        return true;
    }
    
    public bool CanDeleteTreatment()
    {
        return _user.HasPermission(PermissionConstants.DeleteTreatment);
    }

    public bool CanSetTreatmentOrderedDateTime()
    {
        return _user.HasPermission(PermissionConstants.SetTreatmentOrderedDateTime);
    }
    
    // Permisisons to interact with debts.
    public bool CanCreateDebt()
    {
        return _user.HasPermission(PermissionConstants.CreateDebt);
    }

    public bool CanEditDebt(Debt debt)
    {
        if (!_user.HasPermission(PermissionConstants.EditDebt))
        {
            return false;
        }
        
        if (debt.IsClosed && !_user.HasPermission(PermissionConstants.EditClosedDebt))
        {
            return false;
        }
        
        return true;
    }
    
    public bool CanDeleteDebt()
    {
        return _user.HasPermission(PermissionConstants.DeleteDebt);
    }
    
    public bool CanSetDebtCreatedDateTime()
    {
        return _user.HasPermission(PermissionConstants.SetDebtCreatedDateTime);
    }

    // Permissions to interact with debt payments.
    public bool CanCreateDebtPayment()
    {
        return _user.HasPermission(PermissionConstants.CreateDebtPayment);
    }

    public bool CanEditDebtPayment(DebtPayment debtPayment)
    {
        if (!_user.HasPermission(PermissionConstants.EditDebt))
        {
            return false;
        }

        if (debtPayment.IsClosed && !_user.HasPermission(PermissionConstants.EditClosedDebt))
        {
            return false;
        }

        return true;
    }

    public bool CanDeleteDebtPayment()
    {
        return !_user.HasPermission(PermissionConstants.DeleteDebt);
    }

    public bool CanSetDebtPaymentPaidDateTime()
    {
        return _user.HasPermission(PermissionConstants.SetDebtPaymentPaidDateTime);
    }

    // Permissions to interact with consultant.
    public bool CanCreateConsultant()
    {
        return _user.HasPermission(PermissionConstants.CreateConsultant);
    }

    public bool CanEditConsultant(Consultant consultant)
    {
        if (!_user.HasPermission(PermissionConstants.EditConsultant))
        {
            return false;
        }

        if (consultant.IsClosed &&
            !_user.HasPermission(PermissionConstants.EditClosedConsultant))
        {
            return false;
        }

        return true;
    }

    public bool CanDeleteConsultant()
    {
        return _user.HasPermission(PermissionConstants.DeleteConsultant);
    }
    
    public bool CanSetConsultantPaidDateTime()
    {
        return _user.HasPermission(PermissionConstants.SetConsultantPaidDateTime);
    }
}
