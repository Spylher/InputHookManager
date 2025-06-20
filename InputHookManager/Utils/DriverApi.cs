using InputHookManager.Enums;

namespace InputHookManager.Utils
{
    public class DriverApi : ISimulator
    {
        public void KeyDown(InputKey key)
        {
            if (key < InputKey.None)
            {
                Interception.MouseDown(key);
                return;
            }

            Interception.KeyDown(key);
        }

        public void KeyDown(params InputKey[] keys)
        {
            foreach (var inputKey in keys)
                KeyDown(inputKey);
        }

        public void KeyUp(InputKey key)
        {
            if (key < InputKey.None)
            {
                Interception.MouseUp(key);
                return;
            }

            Interception.KeyUp(key);
        }

        public void KeyUp(params InputKey[] keys)
        {
            foreach (var inputKey in keys)
                KeyUp(inputKey);
        }

        public void SendKey(InputKey key)
        {
            if (key < InputKey.None)
            {
                Interception.MouseHandler(key);
                return;
            }

            Interception.KeyDown(key);
            Thread.Sleep(10);
            Interception.KeyUp(key);
        }

        public void SendKey(params InputKey[] keys)
        {
            var commandKeys = keys.Where(HotKey.IsCommandKey).ToArray();
            var keyboardKeys = keys.Where(k => k >= InputKey.None).ToArray();
            var mouseKeys = keys.Where(k => k < InputKey.None).ToArray();

            if (commandKeys.Length > 0)
            {
                Interception.KeyDown(commandKeys);
                Thread.Sleep(1);
            }

            Interception.KeyDown(keyboardKeys);

            foreach (var inputKey in mouseKeys)
                Interception.MouseHandler(inputKey);

            Interception.KeyUp(keyboardKeys);
            Interception.KeyUp(commandKeys);
        }

        public void SendClick() => Interception.LeftClick();

        public void SendClick(int x, int y, bool relative) => Interception.LeftClick(x, y, relative);

        public void SendRightClick() => Interception.RightClick();

        public void SendRightClick(int x, int y, bool relative) => Interception.RightClick(x, y, relative);

        public void SendMiddleButton() => Interception.MiddleButton();

        public void SendXButton1() => Interception.XButton1();

        public void SendXButton2() => Interception.XButton2();

        public void MoveMouse(int x, int y, bool relative)
        {
            if (relative)
                Interception.MoveMouse(x, y);
            else
                Interception.SetMouse(x, y);
        }

        public void SetMouseWheel(int delta) => Interception.SetMouseWheel((short)delta);

        public void SetMouseHWheel(int delta) => Interception.SetMouseHWheel((short)delta);
    }
}
