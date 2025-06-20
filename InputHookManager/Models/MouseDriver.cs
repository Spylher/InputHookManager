using InputHookManager.Utils;

namespace InputHookManager.Models;
internal unsafe struct MouseDriver
{
    public Device Device;

    public void Send(MouseStroke* stroke) => Device.Send(stroke);
    public bool Receive(MouseStroke* stroke) => Device.Receive(stroke);
}
