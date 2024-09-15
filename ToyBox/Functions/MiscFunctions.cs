using System.Runtime.InteropServices;
using ToyBox.IPC;

namespace ToyBox.Functions;

public static class MiscFunctions
{
    [DllImport("user32.dll")]
    public static extern int SetWindowText(IntPtr hWnd, string text);

    public static void EnableBCForClient(LocalPlayer player)
    {
        if (Api.ClientState != null)
        {
            Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCEnabled, [
                player.LocalContentId.ToString(),
                player.BroadCastEnabled.ToString()
            ]);
        }
    }
}