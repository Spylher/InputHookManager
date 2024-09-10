using InputHookManager.Enums;
using InputHookManager.Utils;

namespace InputHookManager.Forms
{
    public partial class MainDisplay : Form
    {
        internal readonly InputController inputController = new();
        
        public MainDisplay()
        {
            InitializeComponent();

            //var visibilityKey = new HotKey(InputKey.V);

            //InputController.Attach(4252);
            //InputController.RegisterAction(visibilityKey, VisibleCommand, true);
            //InputController.RegisterAction(new HotKey(InputKey.S), VisibleCommand, true, ActionMode.Global, KeyState.Pressed);
            var lcontroKey = new HotKey(InputKey.LControlKey);
            inputController.RegisterAction(lcontroKey, (_) => MessageBox.Show(lcontroKey));
            //inputController.RegisterAction(new HotKey(InputKey.LControlKey), (_) => {});
            //inputController.RegisterAction(new HotKey(InputKey.B), (_) => Console.WriteLine(InputKey.LControlKey), true);
            //inputController.RegisterAction(new HotKey(InputKey.LShiftKey), (_) => Console.WriteLine(InputKey.LShiftKey), true);

        }

        public void VisibleCommand(object sender) => Visible = !Visible;
    }
}
