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
            TopMost = true;

            var visibilityKey = new HotKey(InputKey.V);
            var closeFormKey = new HotKey(InputKey.W, true);

            //global
            InputController.RegisterAction(visibilityKey, VisibleCommand, true, ActionMode.Global);
            InputController.RegisterAction(closeFormKey, CloseApp, true, ActionMode.Global);

            //target window
            InputController.Attach(Handle);
            InputController.RegisterAction(new HotKey(InputKey.Right), MoveRightWindow, keyState: KeyState.Pressed);
            InputController.RegisterAction(new HotKey(InputKey.Left), MoveLeftWindow, keyState: KeyState.Pressed);
            InputController.RegisterAction(new HotKey(InputKey.Down), MoveDownWindow, keyState: KeyState.Pressed);
            InputController.RegisterAction(new HotKey(InputKey.Up), MoveUpWindow, keyState: KeyState.Pressed);

            //var startPaintKey = new HotKey(InputKey.S);
            //InputController.RegisterAction(startPaintKey, (_) =>
            //{
            //    var msPaint = Process.Start(new ProcessStartInfo("mspaint.exe"));
            //    InputController.Attach(msPaint);
            //});

            //InputController.RegisterAction(new HotKey(InputKey.LShiftKey), (_) => Console.WriteLine(InputKey.LShiftKey), true);
        }

        public void MoveRightWindow(object sender) => Location = new Point(Location.X + 10, Location.Y);

        public void MoveLeftWindow(object sender) => Location = new Point(Location.X - 10, Location.Y);

        public void MoveDownWindow(object sender) => Location = new Point(Location.X, Location.Y + 10);

        public void MoveUpWindow(object sender) => Location = new Point(Location.X - 10, Location.Y - 10);

        public void VisibleCommand(object sender) => Visible = !Visible;

        public void CloseApp(object sender) => Application.Exit();
    }
}
