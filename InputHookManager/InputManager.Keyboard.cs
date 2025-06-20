using InputHookManager.Enums;
using InputHookManager.Utils;
using System.Runtime.InteropServices;
namespace InputHookManager;

public partial class InputManager
{
    internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    internal LowLevelKeyboardProc KeyboardProc = default!;
    internal IntPtr KeyboardHookId = IntPtr.Zero;

    private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return CallNextHookEx(KeyboardHookId, nCode, wParam, lParam);

        var hookStruct = Marshal.PtrToStructure<WinApi.KBDLLHOOKSTRUCT>(lParam);
        var scanCode = hookStruct.scanCode;
        var key = (InputKey)scanCode;
        //var vkCode = hookStruct.vkCode; // Virtual key code

        var isKeyDown = wParam == (IntPtr)WinMessages.WM_KEYDOWN || wParam == (IntPtr)WinMessages.WM_SYSKEYDOWN;
        var isKeyUp = wParam == (IntPtr)WinMessages.WM_KEYUP || wParam == (IntPtr)WinMessages.WM_SYSKEYUP;

        if (key == InputKey.None)
            return CallNextHookEx(MouseHookId, nCode, wParam, lParam);
        if (isKeyDown && !KeyboardDriverCallback_OnKeyDown(key))
            return 1;
        if (isKeyUp && !KeyboardDriverCallback_OnKeyUp(key))
            return 1;

        return CallNextHookEx(KeyboardHookId, nCode, wParam, lParam);
    }

    private bool KeyActionHandler(Dictionary<InputKey[], Action<object>> keyMappings)
    {
        foreach (var (inputKeys, action) in keyMappings.OrderByDescending(pair => pair.Key.Length))
        {
            if (IsKeyDown(inputKeys) && (GlobalKeys.Contains(inputKeys) && IsHookActive || WinApi.IsActiveWindow(Hwnd)))
            {
                // Reset the state of the hotkey to avoid strange behavior
                ChangeKeyState(inputKeys, false);

                // Invoke the action associated with the hotkey
                action.Invoke(inputKeys);

                // Set the state of the hotkey back to pressed
                ChangeKeyState(inputKeys, true);
                return true;
            }
        }

        return false;
    }

    public bool IsKeyDown(InputKey key)
    {
        return IsKeyDown([key]);
    }

    public bool IsKeyDown(InputKey[] keys)
    {
        foreach (var key in keys)
        {
            if (!KeyStates.TryGetValue(key, out var isPressed) || !isPressed)
                return false; // If any key is not pressed, return false
        }

        return true;
    }

    public bool IsKeyDown(HotKey hk)
    {
        return IsKeyDown(hk.ToInputKey());
    }

    public void ChangeKeyState(InputKey[] inputKeys, bool state)
    {
        foreach (var inputKey in inputKeys)
        {
            KeyStates[inputKey] = state;

            if (!state && inputKey.IsCommandKey())
                Simulator.KeyUp(inputKey); // Release command keys like Ctrl, Shift, Alt
        }
    }

    private bool KeyboardDriverCallback_OnKeyDown(InputKey key)
    {
        KeyStates[key] = true; // Update the key state to pressed

        if (KeyActionHandler(KeyMappingsPressed))
            return false; // If the action occurred, we don't want to pass the key press further

        return true;
    }

    private bool KeyboardDriverCallback_OnKeyUp(InputKey key)
    {
        KeyActionHandler(KeyMappingsReleased);

        // Update the key state to released
        KeyStates[key] = false;
        return true;
    }
}

