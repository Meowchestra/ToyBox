using ToyBox.IPC;

namespace ToyBox.Functions;

public static class MiscFunctions
{
    public static void EnableBCForClient(LocalPlayer player)
    {
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCEnabled, [
            player.LocalContentId.ToString(),
            player.BroadCastEnabled.ToString()
        ]);
    }
}