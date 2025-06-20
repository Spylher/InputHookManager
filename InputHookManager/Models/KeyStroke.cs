using System.Runtime.InteropServices;
using InputHookManager.Enums;

namespace InputHookManager.Models;

[StructLayout(LayoutKind.Explicit, Size = 0x0C)]
internal struct KeyStroke
{
    [FieldOffset(0x02)] public ushort Code;
    [FieldOffset(0x04)] public KeyState State;
    [FieldOffset(0x08)] public uint Information;
}