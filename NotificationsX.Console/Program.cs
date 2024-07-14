using NotificationsX;
using NotificationsX.Platforms.MacOS;

using MacOSNotificationManager notificationManager = new();

await notificationManager.Initialize();

notificationManager.NotificationActivated += (_, args) => {
    Console.WriteLine($"Notification activated: {args.ActionId}");
};

notificationManager.NotificationDismissed += (_, args) => {
    Console.WriteLine($"Notification dismissed: {args.Cause}");
};

var notification = new Notification {
    Title = "信息测试",
    Body = "哼哼哼啊啊啊啊啊啊啊啊啊啊啊",
    BodyImagePath = "C:\\Users\\w\\Downloads\\blessing-studio-w.png",
    Buttons = {
        ("不", "no"),
        ("好", "yes"),
    }

};

await notificationManager.ShowNotification(notification);