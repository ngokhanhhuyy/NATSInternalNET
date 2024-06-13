namespace NATSInternal.Models;

public class NotificationModel
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string EmittedDeltaText { get; set; }
    public bool IsRead { get; set; }
}
