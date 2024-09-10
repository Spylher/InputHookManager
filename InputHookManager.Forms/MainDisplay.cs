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
            
            var visibilityKey = new HotKey(InputKey.V);

            InputController.Attach(4252);
            InputController.RegisterAction(visibilityKey, VisibleCommand, true);
            InputController.RegisterAction(new HotKey(InputKey.S), VisibleCommand, true, ActionMode.Global, KeyState.Pressed);
        }
        
        public void VisibleCommand(object sender) => Visible = !Visible;
    }
}
