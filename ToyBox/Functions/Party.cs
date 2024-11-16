using ToyBox.IPC;

namespace ToyBox.Functions;

public static class Party
{
    public static void InviteToParty()
    {
        if (Api.ClientState?.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyInviteAccept, []);
        foreach (var player in LocalPlayerCollector.localPlayers.Where(player => player.LocalContentId != Api.ClientState.LocalContentId))
        {
            IPCProvider.PartyInviteAction(player.Name, Convert.ToUInt16(player.HomeWorld));
        }
    }

    public static void GimmePartyLead()
    {
        if (Api.ClientState?.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyPromote, [
            Api.ClientState.LocalPlayer.Name.TextValue,
            Api.ClientState.LocalPlayer.HomeWorld.ValueNullable?.RowId.ToString()
        ]);
    }

    public static void Disband()
    {
        if (Api.ClientState?.LocalPlayer == null)
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyLeave, []);
    }

    public static void EnterHouse()
    {
        if (Api.ClientState?.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyEnterHouse, []);
    }

    public static void Teleport()
    {
        if (Api.ClientState?.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyTeleport, []);
        IPCProvider.PartyTeleportAction(true);
    }

    public static void FollowMe()
    {
        if (Api.ClientState?.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyFollow, [
            true.ToString(),
            Api.ClientState.LocalPlayer.GameObjectId.ToString(),
            Api.ClientState.LocalPlayer.Name.TextValue,
            Api.ClientState.LocalPlayer.HomeWorld.ValueNullable?.RowId.ToString(),
        ]);
    }

    public static void StopFollow()
    {
        if (Api.ClientState?.LocalPlayer == null)
            return;
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.PartyFollow, [false.ToString()]);
    }
}