using System.Diagnostics;
using InputHookManager.Enums;
using InputHookManager.Utils;

namespace InputHookManager.Forms
{
    public partial class MainDisplay : Form
    {
        internal readonly InputController InputController = new();

        public MainDisplay()
        {
            InitializeComponent();

            var keyStart = new HotKey(KeyInput.A);

            InputController.AllowedKeys.Add(keyStart);
            InputController.RegisterAction(keyStart, ProcessCommand);
        }

        public void ProcessCommand(object sender)
        {
            var process = Process.Start(new ProcessStartInfo("cmd.exe"));
            var exitProcessKey = new HotKey(KeyInput.S);

            InputController.AllowedKeys.Add(exitProcessKey);
            InputController.RegisterAction(exitProcessKey, (_) =>
            {
                process!.Kill();
                Application.Exit();
            });
        }
    }
}
