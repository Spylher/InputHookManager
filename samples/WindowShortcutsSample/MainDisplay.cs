using InputHookManager;
using InputHookManager.Enums;
using InputHookManager.Utils;

namespace WindowShortcutsSample;

public partial class MainDisplay : Form
{
    internal readonly InputManager InputManager = new();

    public MainDisplay()
    {
        InitializeComponent();

        // Set keys
        var closeFormKey = new HotKey(InputKey.W, true); //"Ctrl+W"
        var visibilityKey = new HotKey(InputKey.V);
        var moveUpKey = new HotKey(InputKey.W);
        var moveLeftKey = new HotKey(InputKey.A); 
        var moveDownKey = new HotKey(InputKey.S);
        var moveRightKey = new HotKey(InputKey.D);

        // Set global actions
        InputManager.RegisterAction(visibilityKey, VisibleCommand, KeyMode.Released, actionMode: ActionMode.Global);
        InputManager.RegisterAction(closeFormKey, CloseApp, actionMode: ActionMode.Global);

        // Set windowed actions - just works on form
        InputManager.Attach(Handle);
        InputManager.RegisterAction(moveRightKey, MoveRightWindow);
        InputManager.RegisterAction(moveLeftKey, MoveLeftWindow);
        InputManager.RegisterAction(moveDownKey, MoveDownWindow);
        InputManager.RegisterAction(moveUpKey, MoveUpWindow);
    }

    public void MoveRightWindow(object sender) => Location = new Point(Location.X + 10, Location.Y);

    public void MoveLeftWindow(object sender) => Location = new Point(Location.X - 10, Location.Y);

    public void MoveDownWindow(object sender) => Location = new Point(Location.X, Location.Y + 10);

    public void MoveUpWindow(object sender) => Location = new Point(Location.X, Location.Y - 10);

    public void VisibleCommand(object sender) => Visible = !Visible;

    public void CloseApp(object sender) => Application.Exit();
}
