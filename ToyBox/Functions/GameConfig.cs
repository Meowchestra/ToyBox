using ToyBox.IPC;

namespace ToyBox.Functions;

public static class GameConfig
{
    public static void SetLowExceptMe()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.SetGfx, new List<string>() { true.ToString() });
    }

    public static void Reset()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.SetGfx, new List<string>() { false.ToString() });
    }

    public static void Logout(ulong cid)
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        if (Api.ClientState.LocalContentId == cid)
        {
            IPCProvider.CharacterLogoutAction();
            return;
        }
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.ClientLogout, new List<string>() { cid.ToString() });
    }

    public static void Shutdown(ulong cid)
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        if (Api.ClientState.LocalContentId == cid)
        {
            IPCProvider.GameShutdownAction();
            return;
        }
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.GameShutdown, new List<string>() { cid.ToString() });
    }

}
