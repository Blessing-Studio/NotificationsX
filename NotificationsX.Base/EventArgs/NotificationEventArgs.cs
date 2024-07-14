namespace NotificationsX.EventArgs;

public record NotificationEventArgs {
    public NotificationEventArgs(Notification notification) {
        Notification = notification;
    }

    public Notification Notification { get; }
}