using InputHookManager.Enums;
using InputHookManager.Utils;
namespace InputHookManager;

public partial class InputManager : ISimulator
{
    public void KeyDown(InputKey key) => Simulator.KeyDown(key);

    public void KeyDown(InputKey[] key) => Simulator.KeyDown(key);

    public void KeyUp(InputKey key) => Simulator.KeyUp(key);

    public void KeyUp(InputKey[] key) => Simulator.KeyUp(key);

    public void SendKey(InputKey key) => Simulator.SendKey(key);

    public void SendKey(InputKey[] key) => Simulator.SendKey(key);

    public void SendKey(HotKey key) => Simulator.SendKey(key);

    public void SendClick() => Simulator.SendClick();

    public void SendClick(int x, int y, bool relative) => Simulator.SendClick();

    public void SendRightClick() => Simulator.SendRightClick();

    public void SendRightClick(int x, int y, bool relative) => Simulator.SendRightClick();

    public void SendMiddleButton() => Simulator.SendMiddleButton();

    public void SendXButton1() => Simulator.SendXButton1();

    public void SendXButton2() => Simulator.SendXButton2();

    public void MoveMouse(int x, int y, bool relative = false) => Simulator.MoveMouse(x, y, relative);

    public void SetMouseWheel(int delta) => Simulator.SetMouseWheel(delta);

    public void SetMouseHWheel(int delta) => Simulator.SetMouseHWheel(delta);
}
