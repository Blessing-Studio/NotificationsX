using System.Diagnostics;

namespace NotificationsX.Platforms.Linux.ApplicationContexts;

public sealed record LinuxApplicationContext : ApplicationContext {
    public string? AppIcon { get; }

    private LinuxApplicationContext(string name, string? appIcon) : base(name) {
        AppIcon = appIcon;
    }

    public static LinuxApplicationContext FromCurrentProcess(string? appIcon = null) {
        var mainModule = Process.GetCurrentProcess().MainModule;

        if (mainModule?.FileName == null) {
            throw new InvalidOperationException("No valid process module found.");
        }

        return new LinuxApplicationContext(
            Path.GetFileNameWithoutExtension(mainModule.FileName), appIcon);
    }
}