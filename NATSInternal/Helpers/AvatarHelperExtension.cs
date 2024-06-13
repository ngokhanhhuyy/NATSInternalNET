namespace NATSInternal.Helpers;

public static class AvatarHelperExtension
{
    public static string DefaultAvatar(this IHtmlHelper _, string fullName)
    {
        return "https://ui-avatars.com/api/?name=" +
            fullName.Replace(" ", "+") +
            "&background=random&size=256";
    }
}
