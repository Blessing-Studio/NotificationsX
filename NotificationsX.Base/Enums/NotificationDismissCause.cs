namespace NotificationsX.Enums;

/// <summary>
/// Causes why a notification was dismissed.
/// </summary>
public enum NotificationDismissCause {
    /// <summary>
    /// The user closed the notification.
    /// </summary>
    User,

    /// <summary>
    /// The notification expired.
    /// </summary>
    Expired,

    /// <summary>
    /// The notification was explicitly removed by application code.
    /// </summary>
    Application,
    Unknown
}