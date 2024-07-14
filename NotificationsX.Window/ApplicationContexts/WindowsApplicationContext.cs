using System.Diagnostics;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

#if WINDOWS10_0_17763_0
using NotificationsX.Platforms.Windows.Win32;
#endif

namespace NotificationsX.Platforms.Windows.ApplicationContexts;

[SupportedOSPlatform("Windows")]
public sealed partial record WindowsApplicationContext : ApplicationContext {
    private WindowsApplicationContext(string name, string appUserModelId) : base(name) {
        AppUserModelId = appUserModelId;
    }

    public string AppUserModelId { get; }

    [LibraryImport("shell32.dll", SetLastError = true)]
    private static partial void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string appId);

    public static WindowsApplicationContext FromCurrentProcess(
        string customName = null,
        string appUserModelId = null) {
#if WINDOWS10_0_17763_0
        var mainModule = Process.GetCurrentProcess().MainModule;

        if (mainModule?.FileName == null) {
            throw new InvalidOperationException("No valid process module found.");
        }

        var appName = customName ?? Path.GetFileNameWithoutExtension(mainModule.FileName);
        var aumid = appUserModelId ?? appName;

        SetCurrentProcessExplicitAppUserModelID(aumid);

        using var shortcut = new WindowsShortcutManager {
            TargetPath = mainModule.FileName,
            Arguments = string.Empty,
            AppUserModelID = aumid
        };

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var startMenuPath = Path.Combine(appData, @"Microsoft\Windows\Start Menu\Programs");
        var shortcutFile = Path.Combine(startMenuPath, $"{appName}.lnk");

        shortcut.Save(shortcutFile);
        return new WindowsApplicationContext(appName, aumid);
#endif
        throw new MissingMethodException();
    }
}
