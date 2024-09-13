using InputHookManager;
using InputHookManager.Enums;
using InputHookManager.Utils;

namespace WindowShortcutSample
{
    public partial class MainDisplay : Form
    {
        //set runInSepareteThread = false, for do NOT use Invokes...
        internal readonly InputController InputController = new(false);

        public MainDisplay()
        {
            InitializeComponent();

            //set keys
            var closeFormKey = new HotKey(InputKey.W, true); //"Ctrl+W"
            var visibilityKey = new HotKey(InputKey.V); //"V"
            var moveRightKey = new HotKey(InputKey.Right); //arrow_right
            var moveLeftKey = new HotKey(InputKey.Left); //arrow_left
            var moveDownKey = new HotKey(InputKey.Down); //arrow_down
            var moveUpKey = new HotKey(InputKey.Up); //arrow_up

            //set global actions
            InputController.RegisterAction(visibilityKey, VisibleCommand, true, ActionMode.Global, KeyState.Released);
            InputController.RegisterAction(closeFormKey, CloseApp, true, ActionMode.Global);

            //set windowed actions - just works on form
            InputController.Attach(Handle);
            InputController.RegisterAction(moveRightKey, MoveRightWindow);
            InputController.RegisterAction(moveLeftKey, MoveLeftWindow);
            InputController.RegisterAction(moveDownKey, MoveDownWindow);
            InputController.RegisterAction(moveUpKey, MoveUpWindow);
        }

        public void MoveRightWindow(object sender) => Location = new Point(Location.X + 10, Location.Y);

        public void MoveLeftWindow(object sender) => Location = new Point(Location.X - 10, Location.Y);

        public void MoveDownWindow(object sender) => Location = new Point(Location.X, Location.Y + 10);

        public void MoveUpWindow(object sender) => Location = new Point(Location.X - 10, Location.Y - 10);

        public void VisibleCommand(object sender) => Visible = !Visible;

        public void CloseApp(object sender) => Application.Exit();
    }
}
