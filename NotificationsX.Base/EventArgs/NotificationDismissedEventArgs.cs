using NotificationsX.Enums;

namespace NotificationsX.EventArgs;

public sealed class NotificationDismissedEventArgs {
    public NotificationDismissedEventArgs(Notification notification, NotificationDismissCause cause) {
        Notification = notification;
        Cause = cause;
    }

    public Notification Notification { get; }
    public NotificationDismissCause Cause { get; }
}