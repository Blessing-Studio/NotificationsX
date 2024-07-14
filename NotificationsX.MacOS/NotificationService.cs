using NotificationsX.Enums;
using NotificationsX.EventArgs;
using NotificationsX.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

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
        RunScriptProcess(notification);
        return Task.CompletedTask;
    }

    public Task ScheduleNotification(Notification notification, DateTimeOffset deliveryTime,
        DateTimeOffset? expirationTime = null) {
        return Task.CompletedTask;
    }

    public Task HideNotification(Notification notification) {
        return Task.CompletedTask;
    }

    private void RunScriptProcess(Notification notification) {
        StringBuilder stringBuilder = new("-c \"osascript -e 'display notification ");
        stringBuilder.Append($"\"{notification.Body}\" ");
        stringBuilder.Append($"with title \"{notification.Title}\"'");

        using Process process = new();
        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = "-c \"osascript -e 'display notification \\\"你好，世界！\\\"'\"";
        process.StartInfo.UseShellExecute = false;
        process.Start();
    }
}