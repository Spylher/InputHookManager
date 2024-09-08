namespace InputHookManager
{
    public enum WinMessages : uint
    {
        //process
        PROCESS_ALL_ACCESS = 0x1F0FFF,
        PROCESS_VM_READ = 0x0010,

        //Window Styles
        //GWL_EXSTYLE = -20,
        WS_EX_TOOLWINDOW = 0x80,
        WS_EX_APPWINDOW = 0x40000,
        WS_CAPTION = 0xC00000,
        WS_POPUP = 0x80000000,
        WS_THICKFRAME = 0x00040000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,
        WS_SYSMENU = 0x00080000,

        //window
        WM_NCHITTEST = 0x84,
        HT_CLIENT = 0x1,
        HT_CAPTION = 0x2,
        CS_DROPSHADOW = 0x00020000,
        WM_PAINT = 0x000F,
        WM_NCPAINT = 0x0085,
        WM_ACTIVATEAPP = 0x001C,
        WM_CLOSEAPP = 0x001D,


        //keyboard
        WH_KEYBOARD_LL = 13,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_NCLBUTTONDOWN = 0xA1,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,

        //mouse
        WH_MOUSE_LL = 14,
        MOUSEEVENTF_LEFTDOWN = 0x02,
        MOUSEEVENTF_LEFTUP = 0x04,
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C,
        XBUTTON1 = 5,
        XBUTTON2 = 6,
    }

    public enum WinCommand
    {
        //window
        Hide = 0,
        ShowNormal = 1,
        ShowMinimized = 2,
        ShowMaximized = 3,
        Restore = 9
    }

    public enum WindowStateType
    {
        Borderless,
        Fullscreen,
        Borders
    }

    public enum KeyState
    {
        Pressed,
        Released
    }

}