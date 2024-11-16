using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace ToyBox.IPC;

public class LocalPlayer
{
    public ulong LocalContentId { get; init; }
    public string Name { get; init; } = "";
    public uint HomeWorld { get; init; }
    public bool BroadCastEnabled { get; set; }  = true;
    public int Affinity { get; set; }

    public unsafe string GetWorldName()
    {
        return WorldHelper.Instance()->AllWorlds[(ushort)HomeWorld].NameString;
    }
}

public static class LocalPlayerCollector
{
    public static List<LocalPlayer> localPlayers = [];

    public static void Add(ulong localContentId, string name, uint HomeWorld, int affinity)
    {
        var t = localPlayers.FirstOrDefault(n => n.LocalContentId == localContentId && n.Name == name);
        if (t is null)
        {
            localPlayers.Add(new LocalPlayer { LocalContentId = localContentId, Name = name, HomeWorld = HomeWorld });
            if (Api.ClientState?.LocalPlayer?.Name.TextValue != null)
            {
                Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCAdd, [
                    Api.ClientState.LocalPlayer.Name.TextValue,
                    Api.ClientState.LocalPlayer.HomeWorld.ValueNullable?.RowId.ToString(),
                    "0"
                ]);
            }
        }
    }

    public static void BroadCastEnabled(bool enabled)
    {
        var t = localPlayers.Where(n => Api.ClientState != null && n.LocalContentId == Api.ClientState.LocalContentId);
        t.First().BroadCastEnabled = enabled;
    }

    public static void Remove(ulong localContentId, string name)
    {
        var t = localPlayers.FirstOrDefault(n => n.LocalContentId == localContentId && n.Name == name);
        if (t is null)
            return;
        localPlayers.Remove(t);
    }
}