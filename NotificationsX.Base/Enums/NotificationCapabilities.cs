namespace NotificationsX.Enums;

[Flags]
public enum NotificationCapabilities {
    None = 0,
    BodyText = 1 << 0,
    BodyImages = 1 << 1,
    BodyMarkup = 1 << 2,
    Audio = 1 << 3,
    Icon = 1 << 4
}