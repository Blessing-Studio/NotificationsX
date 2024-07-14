using NotificationsX.Enums;
using NotificationsX.EventArgs;
using NotificationsX.Interfaces;
using System.Runtime.InteropServices;

namespace NotificationsX.Platforms.MacOS;

public sealed partial class NotificationService : INotificationManager {
    public void Dispose() {
    }

    public NotificationCapabilities Capabilities => NotificationCapabilities.None;

    public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;
    public event EventHandler<NotificationDismissedEventArgs>? NotificationDismissed;

    public string? LaunchActionId { get; }

    public Task Initialize() {
        return Task.CompletedTask;
    }

    public Task ShowNotification(Notification notification, DateTimeOffset? expirationTime = null) {
        ShowNotification();

        return Task.CompletedTask;
    }

    public Task ScheduleNotification(Notification notification, DateTimeOffset deliveryTime,
        DateTimeOffset? expirationTime = null) {
        return Task.CompletedTask;
    }

    public Task HideNotification(Notification notification) {
        return Task.CompletedTask;
    }

    [LibraryImport("NotificationsX.MacOS.Native.dylib")]
    private static partial void ShowNotification();
}