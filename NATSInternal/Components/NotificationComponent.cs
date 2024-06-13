namespace NATSInternal.Components;

public class NotificationComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        NotificationListModel model = new NotificationListModel
        {
            Items = new List<NotificationModel>
            {
                new NotificationModel
                {
                    Id = 1,
                    Content = "Đơn hàng mới đã được tạo",
                    EmittedDeltaText = "3 phút trước",
                    IsRead = false,
                },
                new NotificationModel
                {
                    Id = 2,
                    Content = "Liệu trình mới đã được tạo",
                    EmittedDeltaText = "2 giờ trước",
                    IsRead = false,
                },
                new NotificationModel
                {
                    Id = 3,
                    Content = "Khách hàng đã được chỉnh sửa",
                    EmittedDeltaText = "Hôm qua",
                    IsRead = false,
                },
                new NotificationModel
                {
                    Id = 4,
                    Content = "Đơn hàng đã được thanh toán",
                    EmittedDeltaText = "3 ngày trước",
                    IsRead = true,
                },
            }
        };
        return View("/Views/Shared/_Notifications.cshtml", model);
    }
}