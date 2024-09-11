using InputHookManager.Enums;
using System.Runtime.InteropServices;
using InputHookManager.Utils;
using static InputHookManager.Utils.WinUtils;
using static InputHookManager.Enums.WinMessages;

namespace InputHookManager
{
    public partial class InputController
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc MouseProc;
        private IntPtr MouseHookId = IntPtr.Zero;

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(MouseHookId, nCode, wParam, lParam);

            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))!;
            int xButtonValue = (int)((hookStruct.mouseData >> 16) & 0xFFFF);
            UpdateMousePressed(wParam, xButtonValue);

            bool isKeyDown = wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN || wParam == (IntPtr)WM_XBUTTONDOWN;
            bool isKeyUp = wParam == (IntPtr)WM_LBUTTONUP || wParam == (IntPtr)WM_RBUTTONUP || wParam == (IntPtr)WM_XBUTTONUP;
            bool actionResult = false;

            if (isKeyDown)
            {
                KeyState[KeyPressed.MainKey] = true;
                actionResult = HandleKeyAction(KeyMappingsPressed);

                //suppress_invoke_action_release
                if (KeyMappingsReleased.ContainsKey(KeyPressed) && (WinUtils.IsActiveWindow(Hwnd) || Hwnd == 0 || AllowedKeys.Contains(KeyPressed)))
                    actionResult = true;
            }
            else if (isKeyUp)
            {
                KeyState[KeyPressed.MainKey] = false;
                actionResult = HandleKeyAction(KeyMappingsReleased);
                KeyPressed.Clear();
            }

            if (actionResult && SuppressedKeys.Contains(KeyPressed) && !HotKey.IsCommandKey(KeyPressed.MainKey))
                return 1;

            return CallNextHookEx(MouseHookId, nCode, wParam, lParam);
        }


        private void UpdateMousePressed(IntPtr wParam, int xButtonValue = 1)
        {
            if (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_LBUTTONUP)
                KeyPressed.MainKey = InputKey.LButton;
            else if (wParam == (IntPtr)WM_RBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONUP)
                KeyPressed.MainKey = InputKey.RButton;
            else if (wParam == (IntPtr)WM_XBUTTONDOWN || wParam == (IntPtr)WM_XBUTTONUP)
                KeyPressed.MainKey = (xButtonValue == 1) ? InputKey.XButton1 : InputKey.XButton2;
        }

    }
}
