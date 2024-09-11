using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using InputHookManager;
using InputHookManager.Enums;
using InputHookManager.Utils;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main()
        {
            InputController inputController = new();

            foreach (InputKey key in Enum.GetValues(typeof(InputKey)))
            {
                var hotkey = new HotKey(key);
                inputController.RegisterAction(hotkey, KeyReleaseLog, false);
                inputController.RegisterAction(hotkey, KeyPressedLog, false, keyState: KeyState.Pressed);
            }

            var clearKey = new HotKey(InputKey.C);
            var closeAppKey = new HotKey(InputKey.C, true);
            inputController.RegisterAction(clearKey, Clear, true);
            inputController.RegisterAction(closeAppKey, Close, true);

            Console.ReadLine();
        }

        private static void KeyReleaseLog(object sender)
        {
            var key = sender as HotKey;
            Console.WriteLine($"{key} is released");
        }

        private static void KeyPressedLog(object sender)
        {
            var key = sender as HotKey;
            Console.WriteLine($"{key} is pressed");
        }

        private static void Clear(object sender) => Console.Clear();
        
        private static void Close(object sender) => Environment.Exit(0);
    }
}
