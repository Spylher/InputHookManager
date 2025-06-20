using InputHookManager.Enums;
using InputHookManager.Utils;

namespace InputHookManager
{
    public interface ISimulator
    {
        void KeyDown(InputKey key);
        void KeyDown(InputKey[] key);
        void KeyUp(InputKey key);
        void KeyUp(InputKey[] key);
        void SendKey(InputKey key);
        void SendKey(InputKey[] key);
        void SendKey(HotKey key);

        void SendClick();
        void SendClick(int x, int y, bool relative);
        void SendRightClick();
        void SendRightClick(int x, int y, bool relative);
        void SendMiddleButton();
        void SendXButton1();
        void SendXButton2();
        void MoveMouse(int x, int y, bool relative = false);
        void SetMouseWheel(int delta);
        void SetMouseHWheel(int delta);
    }
}
