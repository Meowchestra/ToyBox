using ToyBox.IPC;

namespace ToyBox.Functions;

public static class Party
{
    public static void InviteToParty()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyInviteAccept, new List<string>());
        foreach (var player in LocalPlayerCollector.localPlayers)
        {
            if (player.LocalContentId == Api.ClientState.LocalContentId)
                continue;
            IPCProvider.PartyInviteAction(player.Name, Convert.ToUInt16(player.HomeWorld));
        }
    }

    public static void GimmePartyLead()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyPromote, new List<string>()
                {
                    Api.ClientState.LocalPlayer.Name.TextValue,
                    Api.ClientState.LocalPlayer.HomeWorld.Id.ToString(),
                });
    }

    public static void Disband()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyLeave, new List<string>() { });
    }

    public static void EnterHouse()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyEnterHouse, new List<string>());
    }

    public static void Teleport()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyTeleport, new List<string>());
        IPCProvider.PartyTeleportAction(true);
    }

    public static void FollowMe()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyFollow, new List<string>()
                {
                    (true).ToString(),
                    Api.ClientState.LocalPlayer.GameObjectId.ToString(),
                    Api.ClientState.LocalPlayer.Name.TextValue,
                    Api.ClientState.LocalPlayer.HomeWorld.Id.ToString(),
                });
    }

    public static void StopFollow()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyFollow, new List<string>()
                {
                    (false).ToString()
                });
    }
}
