using NotificationsX.Enums;
using NotificationsX.EventArgs;
using NotificationsX.Extensions;
using NotificationsX.Interfaces.Platforms.Linux;
using NotificationsX.Platforms.Linux.ApplicationContexts;
using System.Text;
using Tmds.DBus;

namespace NotificationsX.Platforms.Linux;

public sealed class NotificationService {
    private const string NotificationsService = "org.freedesktop.Notifications";
    private static readonly ObjectPath NotificationsPath = new("/org/freedesktop/Notifications");

    private Connection? _connection;
    private IDisposable? _notificationCloseSubscription;
    private IDisposable? _notificationActionSubscription;
    private readonly LinuxApplicationContext _appContext;
    private readonly Dictionary<uint, Notification> _activeNotifications;

    private static Dictionary<string, NotificationCapabilities> CapabilitiesMapping =
        new Dictionary<string, NotificationCapabilities> {
            { "body", NotificationCapabilities.BodyText },
            { "body-images", NotificationCapabilities.BodyImages },
            { "body-markup", NotificationCapabilities.BodyMarkup },
            { "sound", NotificationCapabilities.Audio },
            { "icon", NotificationCapabilities.Icon }
        };

    private IFreeDesktopNotificationsProxy? _proxy;

    public NotificationService(LinuxApplicationContext? appContext = null) {
        _appContext = appContext ?? LinuxApplicationContext.FromCurrentProcess();
        _activeNotifications = new Dictionary<uint, Notification>();
    }

    public void Dispose() {
        _notificationActionSubscription?.Dispose();
        _notificationCloseSubscription?.Dispose();
    }

    public event EventHandler<NotificationActivatedEventArgs>? NotificationActivated;
    public event EventHandler<NotificationDismissedEventArgs>? NotificationDismissed;

    public string? LaunchActionId { get; }
    public NotificationCapabilities Capabilities { get; private set; } = NotificationCapabilities.None;

    public async Task Initialize() {
        _connection = Connection.Session;

        await _connection.ConnectAsync();

        _proxy = _connection.CreateProxy<IFreeDesktopNotificationsProxy>(
            NotificationsService,
            NotificationsPath
        );

        _notificationActionSubscription = await _proxy.WatchActionInvokedAsync(
            OnNotificationActionInvoked,
            OnNotificationActionInvokedError
        );

        _notificationCloseSubscription = await _proxy.WatchNotificationClosedAsync(
            OnNotificationClosed,
            OnNotificationClosedError
        );

        foreach (var cap in await _proxy.GetCapabilitiesAsync()) {
            if (CapabilitiesMapping.TryGetValue(cap, out var capE)) {
                Capabilities |= capE;
            }
        }
    }

    public async Task ShowNotification(Notification notification, DateTimeOffset? expirationTime = null) {
        CheckConnection();

        if (expirationTime < DateTimeOffset.Now) {
            throw new ArgumentException(nameof(expirationTime));
        }

        var duration = expirationTime - DateTimeOffset.Now;
        var actions = GenerateActions(notification);

        var id = await _proxy!.NotifyAsync(
            _appContext.Name,
            0,
            _appContext.AppIcon ?? string.Empty,
            notification.Title ?? throw new ArgumentException(),
            GenerateNotificationBody(notification),
            actions.ToArray(),
            new Dictionary<string, object> { { "urgency", 1 } },
            (int?)duration?.TotalMilliseconds ?? 0
        ).ConfigureAwait(false);

        _activeNotifications[id] = notification;
    }

    public async Task HideNotification(Notification notification) {
        CheckConnection();

        if (_activeNotifications.TryGetKey(notification, out var id)) {
            await _proxy!.CloseNotificationAsync(id);
        }
    }

    public async Task ScheduleNotification(
        Notification notification,
        DateTimeOffset deliveryTime,
        DateTimeOffset? expirationTime = null) {
        CheckConnection();

        if (deliveryTime < DateTimeOffset.Now || deliveryTime > expirationTime) {
            throw new ArgumentException(nameof(deliveryTime));
        }

        //Note: We could consider spawning some daemon that sends the notification at the specified time.
        //For now we only allow to schedule notifications while the application is running.
        await Task.Delay(deliveryTime - DateTimeOffset.Now);

        await ShowNotification(notification, expirationTime);
    }

    private string GenerateNotificationBody(Notification notification) {
        if (notification.Body == null) {
            throw new ArgumentException();
        }

        var sb = new StringBuilder();

        sb.Append(notification.Body);

        if (Capabilities.HasFlag(NotificationCapabilities.BodyImages) &&
            notification.BodyImagePath is { } img) {
            sb.Append($@"\n<img src=""{img}"" alt=""{notification.BodyImageAltText}""/>");
        }

        return sb.ToString();
    }

    private void CheckConnection() {
        if (_connection == null || _proxy == null) {
            throw new InvalidOperationException("Not connected. Call Initialize() first.");
        }
    }

    private static IEnumerable<string> GenerateActions(Notification notification) {
        foreach (var (title, actionId) in notification.Buttons) {
            yield return actionId;
            yield return title;
        }
    }

    private static void OnNotificationClosedError(Exception obj) {
        throw obj;
    }

    private static NotificationDismissCause GetReason(uint reason) {
        return reason switch {
            1 => NotificationDismissCause.Expired,
            2 => NotificationDismissCause.User,
            3 => NotificationDismissCause.Application,
            _ => NotificationDismissCause.Unknown
        };
    }

    private void OnNotificationClosed((uint id, uint reason) @event) {
        if (!_activeNotifications.TryGetValue(@event.id, out var notification)) return;

        _activeNotifications.Remove(@event.id);
        if (notification == null) {
            return;
        }

        var dismissReason = GetReason(@event.reason);

        NotificationDismissed?.Invoke(this,
            new NotificationDismissedEventArgs(notification, dismissReason));
    }

    private static void OnNotificationActionInvokedError(Exception obj) {
        throw obj;
    }

    private void OnNotificationActionInvoked((uint id, string actionKey) @event) {
        if (!_activeNotifications.TryGetValue(@event.id, out var notification)) return;

        NotificationActivated?.Invoke(this,
            new NotificationActivatedEventArgs(notification, @event.actionKey));
    }
}