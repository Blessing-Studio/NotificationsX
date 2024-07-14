
#import <Foundation/Foundation.h>

__attribute__ ((visibility("default"))) void ShowNotification() {
    
    [NSBundle mainBundle];
    
    NSUserNotificationCenter* notificationCenter = [NSUserNotificationCenter defaultUserNotificationCenter];
    //notificationCenter.delegate = _notificationCenterDelegate;
    
    NSUserNotification* cocoaNotification = [NSUserNotification new];
    
    cocoaNotification.identifier = @"efef";
    cocoaNotification.title = @"Hello World";
    cocoaNotification.informativeText = @"informative";
    
    [notificationCenter deliverNotification:cocoaNotification];
}