using InputHookManager.Utils;

namespace InputHookManager.Models;
public unsafe struct KeyboardDriver
{
    public Device Device;

    public void Send(KeyStroke* stroke) => Device.Send(stroke);
    public bool Receive(KeyStroke* stroke) => Device.Receive(stroke);
}