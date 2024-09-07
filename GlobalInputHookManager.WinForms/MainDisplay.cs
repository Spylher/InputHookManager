using System.Diagnostics;
using System.Globalization;
using Keys = GlobalInputHookManager.Enums.Keys;
using GlobalInputHookManager.Utils;
using System;

namespace GlobalInputHookManager.WinForms
{
    public partial class MainDisplay : Form
    {
        InputHookManager InputHookManager = new();

        public MainDisplay()
        {
            InitializeComponent();

            InputHookManager.Install();
            var keyStart = new HotKey(Keys.A);

            InputHookManager.AllowedKeys.Add(keyStart);
            InputHookManager.RegisterAction(keyStart, ProcessCommand);
        }

        public void ProcessCommand(object sender)
        {
            var process = Process.Start(new ProcessStartInfo("cmd.exe"));
            var exitProcessKey = new HotKey(Keys.S);

            InputHookManager.AllowedKeys.Add(exitProcessKey);
            InputHookManager.RegisterAction(exitProcessKey, (obj) => process.Kill());
        }
    }
}
