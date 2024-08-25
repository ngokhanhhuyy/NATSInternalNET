namespace NATSInternal.Services.Dtos;

public class NotificationResponseDto
{
    public int Id { get; set; }
    public NotificationType Type { get; set; }
    public DateTime DateTime { get; set; }
    public string DeltaText
    {
        get
        {
            DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
            return currentDateTime.DeltaTextFromDateTime(DateTime);
        }
    }
    
    public List<int> ResourceIds { get; set; }
    public string ResourceUrl { get; set; }
    public UserBasicResponseDto CreatedUser { get; set; }
    public bool IsRead { get; set; }
    
    public NotificationResponseDto(Notification notification, int currentUserId)
    {
        Id = notification.Id;
        Type = notification.Type;
        DateTime = notification.DateTime;
        ResourceIds = notification.ResourceIds;
        IsRead = notification.ReadUsers.Select(u => u.Id).Contains(currentUserId);
        
        if (notification.CreatedUser != null)
        {
            CreatedUser = new UserBasicResponseDto(notification.CreatedUser);
        }
    }
    
    /// <summary>
    /// Get the resource url of the notification with the specified resource ids.
    /// </summary>
    /// <param name="urlHelper">
    /// An instance of <c>IUrlHelper</c> to generate the URL.
    /// </param>
    public void GenerateResourceUrl(IUrlHelper urlHelper)
    {
        string typeName = Type.ToString();
        if (typeName.EndsWith("Creation") || typeName.EndsWith("Modification"))
        {
            string resourceType = typeName
                .Replace("Creation", "")
                .Replace("Modification", "");
            int primaryId = ResourceIds[0];
            int secondaryId;
            switch (resourceType)
            {
                case "User":
                    ResourceUrl = urlHelper
                        .Action("UserDetail", "User", new { id = primaryId });
                    break;
                case "Customer":
                    ResourceUrl = urlHelper
                        .Action("CustomerDetail", "Customer", new { id = primaryId });
                    break;
                case "Brand":
                    ResourceUrl = urlHelper
                        .Action("BrandDetail", "Brand", new { id = primaryId });
                    break;
                case "Product":
                    ResourceUrl = urlHelper
                        .Action("ProductDetail", "Product", new { id = primaryId });
                    break;
                case "ProductCategory":
                    ResourceUrl = urlHelper.Action(
                        "ProductCategoryDetail",
                        "ProductCategory",
                        new { id = primaryId });
                    break;
                case "Expense":
                    ResourceUrl = urlHelper
                        .Action("ExpenseDetail", "Expense", new { id = primaryId });
                    break;
                case "Supply":
                    ResourceUrl = urlHelper
                        .Action("SupplyDetail", "Supply", new { id = primaryId });
                    break;
                case "Consultant":
                    ResourceUrl = urlHelper.
                        Action("ConsultantDetail", "Consultant", new { id = primaryId });
                    break;
                case "Order":
                    urlHelper.Action("OrderDetail", "Order", new { id = primaryId });
                    break;
                case "Treatment":
                    ResourceUrl = urlHelper
                        .Action("TreatmentDetail", "Treatment", new { id = primaryId });
                    break;
                case "DebtIncurrence":
                    secondaryId = ResourceIds[1];
                    ResourceUrl = urlHelper.Action(
                        "DebtIncurrenceDetail",
                        "CustomerDebtIncurrence",
                        new
                        {
                            customerId = primaryId,
                            debtIncurrenceId = secondaryId
                        });
                    break;
                case "DebtPayment":
                    secondaryId = ResourceIds[1];
                    ResourceUrl = urlHelper.Action(
                        "DebtPaymentDetail",
                        "CustomerDebtPayment",
                        new
                        {
                            customerId = primaryId,
                            debtPaymentId = secondaryId
                        });
                    break;
                case "Announcement":
                    ResourceUrl = urlHelper.Action(
                        "AnnouncementDetail",
                        "Announcement",
                        primaryId);
                    break;
            }
        }
    }
}