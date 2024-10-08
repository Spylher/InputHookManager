﻿using InputHookManager;
using InputHookManager.Enums;
using InputHookManager.Utils;

namespace InputWatcherSample
{
    internal class Program
    {
        private static void Main()
        {
            InputController inputController = new();

            foreach (InputKey key in Enum.GetValues(typeof(InputKey)))
            {
                var hotkey = new HotKey(key);

                if (key == InputKey.LButton)
                    continue;

                inputController.RegisterAction(hotkey, KeyReleaseLog, true, keyState: KeyState.Released);
                inputController.RegisterAction(hotkey, KeyPressedLog, true);
            }

            var clearKey = new HotKey(InputKey.C);
            var closeAppKey = new HotKey(InputKey.C, true);
            inputController.RegisterAction(clearKey, ClearConsole, true);
            inputController.RegisterAction(closeAppKey, CloseApp, true, keyState: KeyState.Pressed);

            Console.ReadLine();
        }

        private static void KeyPressedLog(object sender)
        {
            var key = sender as HotKey;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] - {DateTime.Now}: The key '{key}' was pressed.");
            Console.ResetColor(); 
        }

        private static void KeyReleaseLog(object sender)
        {
            var key = sender as HotKey;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[INFO] - {DateTime.Now}: The key '{key}' was released.");
            Console.ResetColor();
        }

        private static void ClearConsole(object sender) => Console.Clear();

        private static void CloseApp(object sender) => Environment.Exit(0);
    }
}