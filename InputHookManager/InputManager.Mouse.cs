using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Runtime.InteropServices;
using static InputHookManager.Enums.WinMessages;
using static InputHookManager.Utils.WinApi;

namespace InputHookManager
{
    public partial class InputManager
    {
        internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        internal LowLevelMouseProc MouseProc = default!;
        internal IntPtr MouseHookId = IntPtr.Zero;

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(MouseHookId, nCode, wParam, lParam);

            var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))!;
            var xButtonValue = (int)((hookStruct.mouseData >> 16) & 0xFFFF);
            var key = GetPressedKey(wParam, xButtonValue);

            var isKeyDown = wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN || wParam == (IntPtr)WM_XBUTTONDOWN;
            var isKeyUp = wParam == (IntPtr)WM_LBUTTONUP || wParam == (IntPtr)WM_RBUTTONUP || wParam == (IntPtr)WM_XBUTTONUP;

            if (key == InputKey.None)
                return CallNextHookEx(MouseHookId, nCode, wParam, lParam);
            if (isKeyDown && !KeyboardDriverCallback_OnKeyDown(key))
                return 1;
            if (isKeyUp && !KeyboardDriverCallback_OnKeyUp(key))
                return 1;

            return CallNextHookEx(MouseHookId, nCode, wParam, lParam);
        }

        //private bool MouseDriverCallback_OnMouseMove(int x, int y)
        //{
            //KeysState[key] = true; // Update the key state to pressed
            //KeyActionHandler(KeyMappingsPressed);

            //if (SuppressedKeys.Contains(new HotKey(key)))
            //    return false; // If the key is suppressed, we don't want to pass the key press further

        //    return true;
        //}

        public static void SetMouse(int x, int y) => Interception.SetMouse(x, y);
        public static void MoveMouse(int x, int y) => Interception.MoveMouse(x, y);
        public static void LeftClick() => Interception.LeftClick();
        public static void LeftClick(int x, int y, bool relative) => Interception.LeftClick(x, y, relative);
        public static void RightClick() => Interception.RightClick();
        public static void RightClick(int x, int y, bool relative) => Interception.RightClick(x, y, relative);

        private InputKey GetPressedKey(IntPtr wParam, int xButtonValue = 1)
        {
            if (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_LBUTTONUP)
                return InputKey.MouseLeft;
            if (wParam == (IntPtr)WM_RBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONUP)
                return InputKey.MouseRight;
            if (wParam == (IntPtr)WM_XBUTTONDOWN || wParam == (IntPtr)WM_XBUTTONUP)
                return (xButtonValue == 1) ? InputKey.Button1 : InputKey.Button2;

            return InputKey.None;
        }

    }
}
