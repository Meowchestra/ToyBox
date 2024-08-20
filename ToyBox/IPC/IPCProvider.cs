using Dalamud.Plugin.Ipc;

namespace ToyBox.IPC;

public static class IPCProvider
{ 
    /// <summary>
    /// Gfx Low Action
    /// </summary>

    public static ICallGateSubscriber<bool, object>? SetGfxLow;
    public static void SetGfxLowAction(bool low) => SetGfxLow?.InvokeAction(low);

    /// <summary>
    /// Send Chat
    /// </summary>

    public static ICallGateSubscriber<string, object>? SendChat;
    public static void SendChatAction(string msg) => SendChat?.InvokeAction(msg);

    /// <summary>
    /// PartyInvite (string)"Name;HomeWorldId"
    /// </summary>

    public static ICallGateSubscriber<string, ushort, object>? PartyInvite;
    public static void PartyInviteAction(string charater, ushort homeWorldId) => PartyInvite?.InvokeAction(charater, homeWorldId);

    /// <summary>
    /// PartyInvite Accept
    /// </summary>

    public static ICallGateSubscriber<object>? PartyInviteAccept;
    public static void PartyInviteAcceptAction() => PartyInviteAccept?.InvokeAction();

    /// <summary>
    /// Set Party Lead
    /// </summary>

    public static ICallGateSubscriber<string, object>? PartySetLead;
    public static void PartySetLeadAction(string charname) => PartySetLead?.InvokeAction(charname);

    /// <summary>
    /// Set Party Lead
    /// </summary>

    public static ICallGateSubscriber<object>? PartyLeave;
    public static void PartyLeaveAction() => PartyLeave?.InvokeAction();
        

    /// <summary>
    /// Party Enter House
    /// </summary>

    public static ICallGateSubscriber<object>? PartyEnterHouse;
    public static void PartyEnterHouseAction() => PartyEnterHouse?.InvokeAction();

    /// <summary>
    /// Party Enter House
    /// </summary>

    public static ICallGateSubscriber<bool, object>? PartyTeleport;
    public static void PartyTeleportAction(bool showMenu) => PartyTeleport?.InvokeAction(showMenu);

    /// <summary>
    /// Party Follow
    /// </summary>

    public static ICallGateSubscriber<ulong, string, ushort, object>? PartyFollow;
    public static void PartyFollowAction(ulong goId, string name, ushort worldId) => PartyFollow?.InvokeAction(goId, name, worldId);

    /// <summary>
    /// Party UnFollow
    /// </summary>

    public static ICallGateSubscriber<object>? PartyUnFollow;
    public static void PartyUnFollowAction() => PartyUnFollow?.InvokeAction();


    /// <summary>
    /// MoveTo
    /// </summary>

    public static ICallGateSubscriber<float, float, float, float, object>? MoveTo;
    public static void MoveToAction(float X, float Y, float Z, float W) => MoveTo?.InvokeAction(X,Y,Z,W);

    /// <summary>
    /// MoveStop
    /// </summary>

    public static ICallGateSubscriber<object>? MoveStop;
    public static void MoveStopAction() => MoveStop?.InvokeAction();

    /// <summary>
    /// CharacterLogout
    /// </summary>

    public static ICallGateSubscriber<object>? CharacterLogout;
    public static void CharacterLogoutAction() => CharacterLogout?.InvokeAction();

    /// <summary>
    /// GameShutdown
    /// </summary>

    public static ICallGateSubscriber<object>? GameShutdown;
    public static void GameShutdownAction() => GameShutdown?.InvokeAction();

    public static void Initialize()
    {
        SetGfxLow = Api.PluginInterface?.GetIpcSubscriber<bool, object>("Whiskers.SetGfxLow");
        SetGfxLow?.Subscribe(SetGfxLowAction);

        SendChat = Api.PluginInterface?.GetIpcSubscriber<string, object>("Whiskers.SendChat");
        SendChat?.Subscribe(SendChatAction);

        PartyInvite = Api.PluginInterface?.GetIpcSubscriber<string, ushort, object>("Whiskers.PartyInvite");
        PartyInvite?.Subscribe(PartyInviteAction);

        PartyInviteAccept = Api.PluginInterface?.GetIpcSubscriber<object>("Whiskers.PartyInviteAccept");
        PartyInviteAccept?.Subscribe(PartyInviteAcceptAction);

        PartySetLead = Api.PluginInterface?.GetIpcSubscriber<string, object>("Whiskers.PartySetLead");
        PartySetLead?.Subscribe(PartySetLeadAction);

        PartyLeave = Api.PluginInterface?.GetIpcSubscriber<object>("Whiskers.PartyLeave");
        PartyLeave?.Subscribe(PartyLeaveAction);

        PartyEnterHouse = Api.PluginInterface?.GetIpcSubscriber<object>("Whiskers.PartyEnterHouse");
        PartyEnterHouse?.Subscribe(PartyEnterHouseAction);

        PartyTeleport = Api.PluginInterface?.GetIpcSubscriber<bool, object>("Whiskers.PartyTeleport");
        PartyTeleport?.Subscribe(PartyTeleportAction);

        PartyFollow = Api.PluginInterface?.GetIpcSubscriber<ulong, string, ushort, object>("Whiskers.PartyFollow");
        PartyFollow?.Subscribe(PartyFollowAction);

        PartyUnFollow = Api.PluginInterface?.GetIpcSubscriber<object>("Whiskers.PartyUnFollow");
        PartyUnFollow?.Subscribe(PartyUnFollowAction);

        MoveTo = Api.PluginInterface?.GetIpcSubscriber<float, float, float, float, object>("Whiskers.MoveTo");
        MoveTo?.Subscribe(MoveToAction);

        MoveStop = Api.PluginInterface?.GetIpcSubscriber<object>("Whiskers.MoveStop");
        MoveStop?.Subscribe(MoveStopAction);

        CharacterLogout = Api.PluginInterface?.GetIpcSubscriber<object>("Whiskers.CharacterLogout");
        CharacterLogout?.Subscribe(CharacterLogoutAction);

        GameShutdown = Api.PluginInterface?.GetIpcSubscriber<object>("Whiskers.GameShutdown");
        GameShutdown?.Subscribe(GameShutdownAction);
    }

    public static void Dispose()
    {
        SetGfxLow?.Unsubscribe(SetGfxLowAction);
        SendChat?.Unsubscribe(SendChatAction);
        PartyInvite?.Unsubscribe(PartyInviteAction);
        PartyInviteAccept?.Unsubscribe(PartyInviteAcceptAction);
        PartySetLead?.Unsubscribe(PartySetLeadAction);
        PartyLeave?.Unsubscribe(PartyLeaveAction);
        PartyEnterHouse?.Unsubscribe(PartyEnterHouseAction);
        PartyTeleport?.Unsubscribe(PartyTeleportAction);
        PartyFollow?.Unsubscribe(PartyFollowAction);
        PartyUnFollow?.Unsubscribe(PartyUnFollowAction);
        MoveTo?.Unsubscribe(MoveToAction);
        MoveStop?.Unsubscribe(MoveStopAction);
        CharacterLogout?.Unsubscribe(CharacterLogoutAction);
        GameShutdown?.Unsubscribe(GameShutdownAction);
    }
}