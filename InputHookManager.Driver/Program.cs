namespace InterceptionDotNet.Driver;

using System.Diagnostics;
using System.Reflection;

internal class Program
{
    static void Main()
    {
        foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            Console.WriteLine("Found resource: " + name);

        const string driverName = "InterceptionDriver.exe";

        var driverResourceName = Assembly.GetExecutingAssembly()
            .GetManifestResourceNames().First(c => c.Contains(driverName));

        var tempExePath = Path.Combine(Path.GetTempPath(), driverName);

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(driverResourceName))
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
            Arguments = "/install", // ajust as needed uninstall, install, etc.
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