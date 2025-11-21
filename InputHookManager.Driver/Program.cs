using System.Diagnostics;
using System.Reflection;

namespace InputHookManager.Driver;

public class Program
{
    internal const string DriverName = "InterceptionDriver.exe";

    public static void Main()
    {
        foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            Console.WriteLine("Found resource: " + name);
    }

    public static void UninstallInterception()
    {
        var driverResourceName = Assembly.GetExecutingAssembly()
            .GetManifestResourceNames().FirstOrDefault(c => c.Contains(DriverName));

        var tempExePath = Path.Combine(Path.GetTempPath(), DriverName);

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(driverResourceName ?? ""))
        {
            if (stream == null)
            {
                Console.WriteLine("Resource not found!");
                return;
            }

            using (var file = new FileStream(tempExePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(file);
            }
        }

        Console.WriteLine("Run: " + tempExePath);

        var processInfo = new ProcessStartInfo
        {
            FileName = tempExePath,
            Arguments = "/uninstall",
            Verb = "runas",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        using (var process = Process.Start(processInfo))
        {
            process!.WaitForExit();
            Console.WriteLine(process.StandardOutput.ReadToEnd());
            Console.WriteLine(process.StandardError.ReadToEnd());
        }

        Console.WriteLine("End of program. Press any key.");
        Console.ReadKey();
    }

    public static void InstallInterception()
    {
        var driverResourceName = Assembly.GetExecutingAssembly()
            .GetManifestResourceNames().FirstOrDefault(c => c.Contains(DriverName));

        var tempExePath = Path.Combine(Path.GetTempPath(), DriverName);

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(driverResourceName ?? ""))
        {
            if (stream == null)
            {
                Console.WriteLine("Resource not found!");
                return;
            }

            using (var file = new FileStream(tempExePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(file);
            }
        }

        Console.WriteLine("Run: " + tempExePath);

        var processInfo = new ProcessStartInfo
        {
            FileName = tempExePath,
            Arguments = "/install",
            Verb = "runas",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        using (var process = Process.Start(processInfo))
        {
            process!.WaitForExit();
            Console.WriteLine(process.StandardOutput.ReadToEnd());
            Console.WriteLine(process.StandardError.ReadToEnd());
        }

        Console.WriteLine("End of program. Press any key.");
        Console.ReadKey();
    }
}