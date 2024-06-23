using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;

namespace NATSInternal.Services;

/// <inheritdoc />
public class SupplyService : ISupplyService
{
    private readonly DatabaseContext _context;
    private readonly IPhotoService _photoService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IStatsService _statsService;

    public SupplyService(
            DatabaseContext context,
            IPhotoService photoservice,
            IAuthorizationService authorizationService,
            IStatsService statsService)
    {
        _context = context;
        _photoService = photoservice;
        _authorizationService = authorizationService;
        _statsService = statsService;
    }

    /// <inheritdoc />
    public async Task<SupplyListResponseDto> GetListAsync(SupplyListRequestDto requestDto)
    {
        // Query initialization.
        IQueryable<Supply> query = _context.Supplies
            .Include(s => s.User).ThenInclude(u => u.Roles)
            .Include(s => s.Items)
            .Include(s => s.Photos);

        // Sorting directing and sorting by field.
        switch (requestDto.OrderByField)
        {
            case nameof(SupplyListRequestDto.FieldOptions.TotalAmount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.TotalAmount)
                        .ThenBy(s => s.SuppliedDateTime)
                    : query.OrderByDescending(s => s.Items.Sum(i => i.Amount))
                        .ThenByDescending(s => s.SuppliedDateTime);
                break;
            case nameof(SupplyListRequestDto.FieldOptions.SuppliedDateTime):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.SuppliedDateTime)
                        .ThenBy(s => s.Items.Sum(i => i.Amount))
                    : query.OrderByDescending(s => s.SuppliedDateTime)
                        .ThenByDescending(s => s.Items.Sum(i => i.Amount));
                break;
            case nameof(SupplyListRequestDto.FieldOptions.ItemAmount):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.ItemAmount)
                        .ThenBy(s => s.SuppliedDateTime)
                    : query.OrderByDescending(s => s.ItemAmount)
                        .ThenByDescending(s => s.SuppliedDateTime);
                break;
            case nameof(SupplyListRequestDto.FieldOptions.ShipmentFee):
                query = requestDto.OrderByAscending
                    ? query.OrderBy(s => s.ShipmentFee)
                        .ThenBy(s => s.SuppliedDateTime)
                    : query.OrderByDescending(s => s.ShipmentFee)
                        .ThenByDescending(s => s.SuppliedDateTime);
                break;
        }

        // Filter from range if specified.
        if (requestDto.RangeFrom.HasValue)
        {
            DateTime rangeFromDateTime;
            rangeFromDateTime = new DateTime(requestDto.RangeFrom.Value, new TimeOnly(0, 0, 0));
            query = query.Where(s => s.SuppliedDateTime >= rangeFromDateTime);
        }

        // Filter to range if specified.
        if (requestDto.RangeTo.HasValue)
        {
            DateTime rangeToDateTime;
            rangeToDateTime = new DateTime(requestDto.RangeTo.Value, new TimeOnly(0, 0, 0));
            query = query.Where(s => s.SuppliedDateTime <= rangeToDateTime);
        }

        // Filter by user id if specified.
        if (requestDto.UserId.HasValue)
        {
            query = query.Where(s => s.UserId == requestDto.UserId.Value);
        }

        // Initialize response dto.
        SupplyListResponseDto responseDto = new SupplyListResponseDto();
        int resultCount = await query.CountAsync();
        if (resultCount == 0)
        {
            responseDto.PageCount = 0;
            return responseDto;
        }
        responseDto.PageCount = (int)Math.Ceiling((double)resultCount / requestDto.ResultsPerPage);
        responseDto.Items = await query
            .Select(s => new SupplyBasicResponseDto
            {
                Id = s.Id,
                SuppliedDateTime = s.SuppliedDateTime,
                TotalAmount = s.Items.Sum(i => i.Amount) + s.ShipmentFee,
                IsClosed = s.IsClosed,
                User = new UserBasicResponseDto
                {
                    Id = s.User.Id,
                    UserName = s.User.UserName,
                    FirstName = s.User.FirstName,
                    MiddleName = s.User.MiddleName,
                    LastName = s.User.LastName,
                    FullName = s.User.FullName,
                    Gender = s.User.Gender,
                    Birthday = s.User.Birthday,
                    JoiningDate = s.User.JoiningDate,
                    AvatarUrl = s.User.AvatarUrl,
                    Role = new RoleBasicResponseDto
                    {
                        Id = s.User.Role.Id,
                        Name = s.User.Role.Name,
                        DisplayName = s.User.Role.DisplayName,
                        PowerLevel = s.User.Role.PowerLevel
                    }
                },
                FirstPhotoUrl = s.Photos
                    .OrderBy(p => p.Id)
                    .Select(p => p.Url)
                    .FirstOrDefault()
            }).Skip(requestDto.ResultsPerPage * (requestDto.Page - 1))
            .Take(requestDto.ResultsPerPage)
            .ToListAsync();

        return responseDto;
    }

    /// <inheritdoc />
    public async Task<SupplyDetailResponseDto> GetDetailAsync(int id)
    {
        return await _context.Supplies
            .Include(s => s.Items).ThenInclude(si => si.Product)
            .Include(s => s.Photos)
            .Include(s => s.User).ThenInclude(u => u.Roles)
            .Include(s => s.UpdateHistories)
            .Where(s => s.Id == id)
            .Select(s => new SupplyDetailResponseDto
            {
                Id = s.Id,
                SuppliedDateTime = s.SuppliedDateTime,
                ShipmentFee = s.ShipmentFee,
                ItemAmount = s.ItemAmount,
                TotalAmount = s.TotalAmount,
                Note = s.Note,
                IsClosed = s.IsClosed,
                CreatedDateTime = s.CreatedDateTime,
                UpdatedDateTime = s.UpdatedDateTime,
                Items = s.Items
                    .OrderBy(i => i.Id)
                    .Select(i => new SupplyItemResponseDto
                    {
                        Id = i.Id,
                        Amount = i.Amount,
                        SuppliedQuantity = i.SuppliedQuantity,
                        Product = new ProductBasicResponseDto
                        {
                            Id = i.Product.Id,
                            Name = i.Product.Name,
                            Unit = i.Product.Unit,
                            Price = i.Product.Price,
                            ThumbnailUrl = i.Product.ThumbnailUrl
                        }
                    }).ToList(),
                Photos = s.Photos
                    .OrderBy(p => p.Id)
                    .Select(p => new SupplyPhotoResponseDto
                    {
                        Id = p.Id,
                        Url = p.Url
                    }).ToList(),
                User = new UserBasicResponseDto
                {
                    Id = s.User.Id,
                    UserName = s.User.UserName,
                    FirstName = s.User.FirstName,
                    MiddleName = s.User.MiddleName,
                    LastName = s.User.LastName,
                    FullName = s.User.FullName,
                    Gender = s.User.Gender,
                    Birthday = s.User.Birthday,
                    JoiningDate = s.User.JoiningDate,
                    AvatarUrl = s.User.AvatarUrl,
                    Role = new RoleBasicResponseDto
                    {
                        Id = s.User.Role.Id,
                        Name = s.User.Role.Name,
                        DisplayName = s.User.Role.DisplayName,
                        PowerLevel = s.User.Role.PowerLevel
                    }
                },
                Authorization = _authorizationService.GetSupplyDetailAuthorization(s),
            }).AsSplitQuery()
            .SingleOrDefaultAsync()
        ?? throw new ResourceNotFoundException(
            nameof(Supply),
            nameof(id),
            id.ToString());
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(SupplyUpsertRequestDto requestDto)
    {
        // Use a transaction for data integrity.
        using var transaction = await _context.Database.BeginTransactionAsync();

        // Fetch a list of products given by id in the request, ensure all of the products exists.
        List<int> productIdsInRequest = requestDto.Items
            .OrderBy(i => i.ProductId)
            .Select(i => i.ProductId)
            .ToList();
        List<Product> products = await _context.Products
            .Where(p => productIdsInRequest.Contains(p.Id))
            .ToListAsync();
        HashSet<int> fetchedProductIds = products.Select(p => p.Id).ToHashSet();
        for (int i = 0; i < productIdsInRequest.Count; i++)
        {
            int idInRequest = productIdsInRequest[i];
            if (!fetchedProductIds.Contains(idInRequest))
            {
                string errorMessage = ErrorMessages.NotFoundByProperty
                    .ReplaceResourceName(DisplayNames.Product)
                    .ReplacePropertyName(DisplayNames.Id)
                    .ReplaceAttemptedValue(idInRequest.ToString());
                throw new OperationException($"items[{i}].productId", errorMessage);
            }
        }

        // Initialize supply.
        Supply supply = new()
        {
            SuppliedDateTime = requestDto.SuppliedDateTime ?? DateTime.UtcNow.ToApplicationTime(),
            ShipmentFee = requestDto.ShipmentFee,
            Note = requestDto.Note,
            CreatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            UserId = _authorizationService.GetUserId(),
            Items = [],
            Photos = []
        };

        // Initialize items
        foreach (SupplyItemRequestDto itemRequestDto in requestDto.Items)
        {
            Product product = products.Single(i => i.Id == itemRequestDto.ProductId);
            SupplyItem supplyItem = new()
            {
                Amount = itemRequestDto.Amount,
                SuppliedQuantity = itemRequestDto.SuppliedQuantity,
                ProductId = product.Id
            };
            product.StockingQuantity += itemRequestDto.SuppliedQuantity;
            supply.Items.Add(supplyItem);
        }

        // Initialize photos
        if (requestDto.Photos != null)
        {
            foreach (SupplyPhotoRequestDto photoRequestDto in requestDto.Photos)
            {
                string url = await _photoService
                    .CreateAsync(photoRequestDto.File, "supplies", false);
                SupplyPhoto supplyPhoto = new()
                {
                    Url = url
                }; ;
                supply.Photos.Add(supplyPhoto);
            }
        }

        // Save changes and handle errors.
        _context.Supplies.Add(supply);
        try
        {
            await _context.SaveChangesAsync();
            await _statsService.IncrementSupplyCostAsync(supply.ItemAmount);
            await _statsService.IncrementShipmentCostAsync(supply.ShipmentFee);
            await transaction.CommitAsync();
            return supply.Id;
        }
        catch (DbUpdateException exception) when (exception.InnerException is MySqlException)
        {
            await transaction.RollbackAsync();
            // Delete all created photos.
            foreach (SupplyPhoto supplyPhoto in supply.Photos)
            {
                _photoService.Delete(supplyPhoto.Url);
            }

            HandleCreateOrUpdateException(exception.InnerException as MySqlException);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, SupplyUpsertRequestDto requestDto)
    {
        // Fetch the entity from the database and ensure it exists.
        Supply supply = await _context.Supplies
            .Include(s => s.Items).ThenInclude(i => i.Product)
            .Include(s => s.Photos)
            .Include(s => s.UpdateHistories)
            .Where(s => s.Id == id)
            .AsSplitQuery()
            .SingleOrDefaultAsync()
            ?? throw new ResourceNotFoundException(
                nameof(Supply),
                nameof(id),
                id.ToString());

        if (!_authorizationService.CanEditSupply(supply))
        {
            throw new AuthorizationException();
        }

        string oldDataJson = GenerateSupplyUpdateHistoryJson(supply);

        // Use transaction to ensure data integrity.
        using var transaction = await _context.Database.BeginTransactionAsync();

        // Update supply properties.
        long originalItemAmount = supply.ItemAmount;
        long originalShipmentFee = supply.ShipmentFee;
        supply.SuppliedDateTime = requestDto.SuppliedDateTime ?? DateTime.UtcNow.ToApplicationTime();
        supply.ShipmentFee = requestDto.ShipmentFee;
        supply.Note = requestDto.Note;

        // Update supply items.
        if (_authorizationService.CanEditSupplyItems())
        {
            supply.Items ??= [];
            for (int i = 0; i < requestDto.Items.Count; i++)
            {
                SupplyItemRequestDto itemRequestDto = requestDto.Items[i];
                if (itemRequestDto.HasBeenChanged)
                {
                    SupplyItem supplyItem = supply.Items
                        .SingleOrDefault(i => i.Id == itemRequestDto.Id);
                    if (supplyItem == null)
                    {
                        string errorMessage = ErrorMessages.NotFoundByProperty
                            .ReplaceResourceName(DisplayNames.SupplyItem)
                            .ReplacePropertyName(DisplayNames.Id)
                            .ReplaceAttemptedValue(itemRequestDto.Id.ToString());
                        throw new OperationException($"items[{i}].id", errorMessage);
                    }
                    if (itemRequestDto.HasBeenDeleted)
                    {
                        supply.Items.Remove(supplyItem);
                        continue;
                    }

                    supplyItem.Amount = itemRequestDto.Amount;

                    int suppliedQuantityDifference = itemRequestDto.SuppliedQuantity - supplyItem.SuppliedQuantity;
                    supplyItem.SuppliedQuantity = itemRequestDto.SuppliedQuantity;
                    supplyItem.Product.StockingQuantity += suppliedQuantityDifference;
                }
            }
        }

        // Update photos.
        supply.Photos ??= [];
        List<string> shouldDeleteWhenSucceedUrls = [];
        List<string> shouldDeleteWhenFailUrls = [];
        if (_authorizationService.CanEditSupplyPhotos())
        {
            for (int i = 0; i < requestDto.Photos.Count; i++)
            {
                SupplyPhotoRequestDto photoRequestDto = requestDto.Photos[i];
                if (photoRequestDto.HasBeenChanged)
                {
                    SupplyPhoto supplyPhoto = supply.Photos.SingleOrDefault(p => p.Id == photoRequestDto.Id);
                    if (supplyPhoto == null)
                    {
                        string errorMessage = ErrorMessages.NotFoundByProperty
                            .ReplaceResourceName(DisplayNames.SupplyPhoto)
                            .ReplacePropertyName(DisplayNames.Id)
                            .ReplaceAttemptedValue(photoRequestDto.Id.ToString());
                        throw new OperationException($"photos[{i}].id", errorMessage);
                    }
                    // Add to list to be deleted later if the transaction succeeds.
                    shouldDeleteWhenSucceedUrls.Add(supplyPhoto.Url);
                    supply.Photos.Remove(supplyPhoto);

                    if (photoRequestDto.HasBeenChanged)
                    {
                        string url = await _photoService
                            .CreateAsync(photoRequestDto.File, "supplies", false);
                        // Add to list to be deleted later if the transaction fails.
                        shouldDeleteWhenFailUrls.Add(url);
                        supplyPhoto.Url = url;
                    }
                }
            }
        }

        // Create update history.
        supply.UpdateHistories ??= [];
        string newDataJson = GenerateSupplyUpdateHistoryJson(supply);
        SupplyUpdateHistories supplyHistory = new()
        {
            UpdatedDateTime = DateTime.UtcNow.ToApplicationTime(),
            Reason = requestDto.UpdateReason,
            OldData = oldDataJson,
            NewData = newDataJson,
            UserId = _authorizationService.GetUserId()
        };
        supply.UpdateHistories.Add(supplyHistory);

        // Save changes.
        try
        {
            await _context.SaveChangesAsync();
            await _statsService.IncrementSupplyCostAsync(supply.ItemAmount - originalItemAmount);
            await _statsService.IncrementShipmentCostAsync(supply.ShipmentFee - originalShipmentFee);
            await transaction.CommitAsync();
            foreach (string url in shouldDeleteWhenSucceedUrls)
            {
                _photoService.Delete(url);
            }
        }
        catch (DbUpdateException exception) when (exception.InnerException is MySqlException)
        {
            await transaction.RollbackAsync();
            foreach (string url in shouldDeleteWhenFailUrls)
            {
                _photoService.Delete(url);
            }
            HandleCreateOrUpdateException(exception.InnerException as MySqlException);
            throw;
        }
        
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        // Fetch the entity with given id from the database and ensure it exists.
        Supply supply = await _context.Supplies
            .Include(s => s.Items).ThenInclude(si => si.Product)
            .Include(s => s.Photos)
            .SingleOrDefaultAsync(s => s.Id == id)
            ?? throw new ResourceNotFoundException(
                nameof(Supply),
                nameof(id),
                id.ToString());

        if (!_authorizationService.CanDeleteSupply(supply))
        {
            throw new AuthorizationException();
        }

        // Using transaction for atomic operations.
        await using IDbContextTransaction transaction = await _context.Database
            .BeginTransactionAsync();

        try
        {
            long originalItemAmount = supply.ItemAmount;
            long originalShipmentFee = supply.ShipmentFee;
            _context.Supplies.Remove(supply);
            await _context.SaveChangesAsync();
            // Adjust product stocking quantity.
            foreach (SupplyItem item in supply.Items)
            {
                item.Product.StockingQuantity -= item.SuppliedQuantity;
            }
            // Adjust stats.
            await _statsService.IncrementSupplyCostAsync(-originalItemAmount);
            await _statsService.IncrementSupplyCostAsync(-originalShipmentFee);
            // Commit transaction.
            await transaction.CommitAsync();
            // Delete all supply photos after transaction succeeded.
            if (supply.Photos != null)
            {
                foreach (string url in supply.Photos.Select(p => p.Url))
                {
                    _photoService.Delete(url);
                }
            }
        }
        catch (DbUpdateException exception) when (exception.InnerException is MySqlException)
        {
            HandleDeleteExeption(exception.InnerException as MySqlException);
            throw;
        }

    }

    /// <summary>
    /// Convert all the exceptions those are thrown by the database during the creating
    /// or updating operation into the appropriate execptions.
    /// </summary>
    /// <param name="exception">The exeception thrown by the database.</param>
    /// <exception cref="OperationException"></exception>
    private static void HandleCreateOrUpdateException(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        if (exceptionHandler.IsForeignKeyNotFound)
        {
            if (exceptionHandler.ViolatedFieldName == "product_id")
            {
                string errorMessage = ErrorMessages.NotFound
                    .ReplaceResourceName(DisplayNames.Product);
                throw new OperationException($"items.productId", errorMessage);
            }
        }
    }

    /// <summary>
    /// Convert all the exceptions those are thrown by the database during the deleting
    /// operation into the appropriate exceptions.
    /// </summary>
    /// <param name="exception">The exception thrown by the database.</param>
    /// <exception cref="OperationException"></exception>
    private static void  HandleDeleteExeption(MySqlException exception)
    {
        SqlExceptionHandler exceptionHandler = new SqlExceptionHandler();
        exceptionHandler.Handle(exception);
        if (exceptionHandler.IsDeleteOrUpdateRestricted)
        {
            string errorMessage = ErrorMessages.DeleteRestricted
                .ReplaceResourceName(DisplayNames.Supply);
            throw new OperationException(errorMessage);
        }
    }

    /// <summary>
    /// Generate a json string containing the important data of the given supply for supply update history.
    /// </summary>
    /// <param name="supply">The supply entity to be converted into json.</param>
    /// <returns>A json string containing the supply data.</returns>
    private static string GenerateSupplyUpdateHistoryJson(Supply supply)
    {
        var payload = new
        {
            supply.Id,
            supply.SuppliedDateTime,
            supply.ShipmentFee,
            supply.Note,
            supply.CreatedDateTime,
            supply.UserId,
            Items = supply.Items
                .Select(supplyItem => new
                {
                    supplyItem.Id,
                    supplyItem.Amount,
                    supplyItem.SuppliedQuantity,
                    supplyItem.ProductId,
                }).ToList(),
            PhotoCount = supply.Photos.Count
        };

        return JsonSerializer.Serialize(payload);
    }
}
