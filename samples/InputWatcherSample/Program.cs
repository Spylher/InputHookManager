using InputHookManager;
using InputHookManager.Enums;
using InputHookManager.Utils;

namespace InputWatcherSample
{
    internal class Program
    {
        private static void Main()
        {
            var inputManager = new InputManager();

            // Uncomment the following line to use kernel mode (requires admin privileges and Interception Driver Installed)
            //var inputManager = new InputManager(ExecutionMode.KernelMode);

            //Registering global actions for all keys
            foreach (InputKey key in Enum.GetValues(typeof(InputKey)))
            {
                if (key == InputKey.None || key == InputKey.MouseLeft || key == InputKey.LControl || key == InputKey.LShift || key == InputKey.LAlt)
                    continue;

                inputManager.RegisterAction([key], KeyPressedLog, actionMode: ActionMode.Global);
                inputManager.RegisterAction([key], KeyReleaseLog, KeyMode.Released, actionMode: ActionMode.Global);

                // Registering combinations with control, shift, and alt keys
                inputManager.RegisterAction([InputKey.LControl, key], KeyPressedLog, actionMode: ActionMode.Global);
                inputManager.RegisterAction([InputKey.LControl, key], KeyReleaseLog, KeyMode.Released, actionMode: ActionMode.Global);

                inputManager.RegisterAction([InputKey.LShift, key], KeyPressedLog, actionMode: ActionMode.Global);
                inputManager.RegisterAction([InputKey.LShift, key], KeyReleaseLog, KeyMode.Released, actionMode: ActionMode.Global);

                inputManager.RegisterAction([InputKey.LAlt, key], KeyPressedLog, actionMode: ActionMode.Global);
                inputManager.RegisterAction([InputKey.LAlt, key], KeyReleaseLog, KeyMode.Released, actionMode: ActionMode.Global);
            }

            // Registering specific actions for some keys
            inputManager.RegisterAction([InputKey.C], ClearConsole, KeyMode.Released, actionMode: ActionMode.Global);
            inputManager.RegisterAction([InputKey.LControl, InputKey.C], CloseApp, KeyMode.Released, actionMode: ActionMode.Global);
            
            // Wait for the user to press keys
            Console.WriteLine("Input Watcher Sample Application");
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadLine();
        }

        private static void KeyPressedLog(object sender)
        {
            var inputKey = sender as InputKey[];
            var hotKey = new HotKey(inputKey!);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] - {DateTime.Now}: The key '{hotKey}' was pressed.");
            Console.ResetColor();
        }

        private static void KeyReleaseLog(object sender)
        {
            var inputKey = sender as InputKey[];
            var hotKey = new HotKey(inputKey!);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[INFO] - {DateTime.Now}: The key '{hotKey}' was released.");
            Console.ResetColor();
        }

        private static void ClearConsole(object sender)
        {
            Console.Clear();
            Console.WriteLine("Input Watcher Sample Application");
        }

        private static void CloseApp(object sender) => Environment.Exit(0);
    }
}