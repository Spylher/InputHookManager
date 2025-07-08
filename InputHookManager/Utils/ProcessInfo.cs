namespace InputHookManager.Utils;

public class ProcessInfo
{
    public string ClassName { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public IntPtr MainWindowHandle { get; set; }

    public override string ToString()
    {
        return ClassName;
    }
}
