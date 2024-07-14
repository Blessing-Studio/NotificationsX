using NotificationsX.Enums;
using NotificationsX.EventArgs;
using NotificationsX.Interfaces;

namespace NotificationsX.Platforms.MacOS;

public sealed class MacOSNotificationManager : INotificationManager {
    private readonly NotificationService _notificationService;

    public string LaunchActionId => _notificationService.LaunchActionId;
    public NotificationCapabilities Capabilities => _notificationService.Capabilities;

    public event EventHandler<NotificationActivatedEventArgs> NotificationActivated;
    public event EventHandler<NotificationDismissedEventArgs> NotificationDismissed;

    public MacOSNotificationManager() {
        _notificationService = new NotificationService();

        _notificationService.NotificationActivated += (_, args) => NotificationActivated?.Invoke(this, args);
        _notificationService.NotificationDismissed += (_, args) => NotificationDismissed?.Invoke(this, args);
    }

    public void Dispose() => GC.Collect();
    public Task Initialize() => Task.CompletedTask;
    public Task HideNotification(Notification notification) => _notificationService.HideNotification(notification);

    public Task ShowNotification(Notification notification, DateTimeOffset? expirationTime = null) =>
        _notificationService.ShowNotification(notification, expirationTime);

    public Task ScheduleNotification(Notification notification, DateTimeOffset deliveryTime, DateTimeOffset? expirationTime = null) =>
        _notificationService.ScheduleNotification(notification, deliveryTime, expirationTime);

}