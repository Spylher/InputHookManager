using GlobalInputHookManager.Enums;
using System.Collections.Generic;
using System.Globalization;

namespace GlobalInputHookManager.ConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            InputHookManager InputHookManager = new();
            InputHookManager.Install();
            var keyStart = new HotKey(Keys.A);
            InputHookManager.AllowedKeys.Add(keyStart);
            InputHookManager.RegisterAction(keyStart, (obj) => Console.WriteLine("Hello, World!"));

            //Console.ReadLine();
            
            InputHookManager.Uninstall();

        }
    }
}
