using System.Diagnostics;
using System.Security.Cryptography;
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
            
            var visibilityKey = new HotKey(KeyInput.V);
            InputController.Attach(4252);
            InputController.RegisterAction(visibilityKey, VisibleCommand, true);
            InputController.RegisterAction(new HotKey(KeyInput.S), VisibleCommand, true, ActionMode.Global, KeyState.Pressed);
        }
        
        public void VisibleCommand(object sender)
        {
            //Visible = Visible? Visible = false : Visible = true;vvvvv

            if (Visible)
                Visible = false;
            else
                Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var a = Process.GetProcessById(4252);
            MessageBox.Show(a.MainWindowTitle.ToString());
            //MessageBox.Show(InputController.Hwnd.ToString());
            
            //var process = Process.Start(new ProcessStartInfo("mspaint.exe"))!;
            //process.WaitForInputIdle();
            //var exitProcessKey = new HotKey(KeyInput.S);

            //InputController.Attach(process.MainWindowHandle);
            //InputController.AllowedKeys.Add(exitProcessKey);
            //InputController.RegisterAction(exitProcessKey, (_) =>
            //{
            //    process!.Kill();
            //    Application.Exit();
            //});
        }
    }
}
