using System.Diagnostics;
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
            inputController.RegisterAction(new HotKey(InputKey.A), (_) => Console.WriteLine("DALE"), true);

            foreach (InputKey key in Enum.GetValues(typeof(InputKey)))
                inputController.RegisterAction(new HotKey(key), (_) => Console.WriteLine(new HotKey(key)), true);
            
            
            inputController.RegisterAction(new HotKey("C"), (_) => Console.Clear(), true);
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
