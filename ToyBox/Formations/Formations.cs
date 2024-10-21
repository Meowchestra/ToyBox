using System.Globalization;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Newtonsoft.Json;
using ToyBox.IPC;

namespace ToyBox.Formations;

public class FormationsData
{
    public string? Name;
    public Dictionary<long, FormationEntry> formationEntry = new();
}

public class FormationEntry
{
    public int Index { get; set; }
    public long CID { get; set; }
    public Vector3 RelativePosition;
    public float RelativeRotation;
}

public static class FormationFactory
{
    public static FormationsData? CreateNewFormation()
    {
        var formation = new FormationsData();
        var index = 0;
        if (Api.PartyList != null)
        {
            foreach (var f in Api.PartyList)
            {
                //not the same world? Nope.
                if (Api.ClientState != null && Api.ClientState.LocalPlayer != null && f.World.Id != Api.ClientState.LocalPlayer.CurrentWorld.Id)
                    continue;

                //get the gob
                if (Api.Objects?.SearchById(f.ObjectId) is not IPlayerCharacter player)
                    continue;

                var lPlayer = LocalPlayerCollector.localPlayers.First(n =>
                    n.Name.Equals(player.Name.TextValue) && n.HomeWorld == player.HomeWorld.Id);
                if (lPlayer == null)
                    continue;

                var rot = player.Rotation;
                var pos = player.Position;

                if (Api.ClientState != null)
                {
                    if (Api.ClientState.LocalPlayer != null)
                    {
                        var o = FormationCalculation.AbsoluteToRelative(new KeyValuePair<Vector3, float>(pos, rot),
                            new KeyValuePair<Vector3, float>(Api.ClientState.LocalPlayer.Position,
                                Api.ClientState.LocalPlayer.Rotation));

                        var entry = new FormationEntry
                        {
                            Index            = index,
                            CID              = (long)lPlayer.LocalContentId,
                            RelativePosition = o.Key,
                            RelativeRotation = o.Value
                        };
                        index++;

                        formation.formationEntry.Add(entry.CID, entry);
                    }
                }
            }
        }

        formation.Name = "TempName";
        return formation;
    }

    public static void LoadFormation(FormationsData? formation)
    {
        if (Api.ClientState?.LocalPlayer == null)
            return;

        if (formation?.formationEntry.Values != null)
        {
            foreach (var entry in formation.formationEntry.Values)
            {
                Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.FormationData, [
                    entry.CID.ToString(),
                    FormationCalculation.RelativeToAbsolute(entry, Api.ClientState.LocalPlayer).Key
                        .ToString("G", CultureInfo.InvariantCulture),
                    (FormationCalculation.RelativeToAbsolute(entry, Api.ClientState.LocalPlayer).Value + 3.1415927f)
                    .ToString("G", CultureInfo.InvariantCulture)
                ]);
            }
        }
    }

    public static void StopFormation()
    {
        IPCProvider.MoveStopAction();
        if (Api.ClientState != null)
            Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.FormationStop, []);
    }

    public static List<string> ReadBtBFormationNames(string? filename)
    {
        List<string> outdata = [];
        if (filename != null)
        {
            dynamic jData = JsonConvert.DeserializeObject(File.ReadAllText(filename)) ?? throw new InvalidOperationException();
            var f = jData?["SavedFormationList"];
            if (f != null)
            {
                foreach (var name in f)
                    outdata.Add(Convert.ToString(name["11"]));
            }
        }

        return outdata;
    }

    public static FormationsData? ConvertBtBFormation(string? filename, string item)
    {
        if (filename != null)
        {
            var data = File.ReadAllText(filename);
            dynamic jData = JsonConvert.DeserializeObject(data) ?? throw new InvalidOperationException();
            var f = jData?["SavedFormationList"];
            if (f != null)
            {
                foreach (var name in f)
                {
                    if (Convert.ToString(name["11"]) == item)
                    {
                        var da = JsonConvert.SerializeObject((object?)name);
                        da = da.Replace("\"11\":", "\"Name\":");
                        da = da.Replace("\"22\":", "\"FormationEntry\":");
                        da = da.Replace("\"i\":", "\"Index\":");
                        da = da.Replace("\"Pepsi1\":", "\"CID\":");
                        da = da.Replace("\"Pepsi2\":", "\"RelativePosition\":");
                        da = da.Replace("\"Pepsi3\":", "\"RelativeRotation\":");
                        return JsonConvert.DeserializeObject<FormationsData>(da);
                    }
                }
            }
        }

        return null;
    }

    public static void CheckMissingCIDs(string? filename, string item, Configuration config)
    {
        if (filename != null)
        {
            dynamic jData = JsonConvert.DeserializeObject(File.ReadAllText(filename)) ?? throw new InvalidOperationException();
            string txt = JsonConvert.SerializeObject(jData?["CidToNameWorld"]);
            txt = txt.Replace("\"Item1\":", "\"Key\":");
            txt = txt.Replace("\"Item2\":", "\"Value\":");
            var output = JsonConvert.DeserializeObject<Dictionary<long, KeyValuePair<string, string>>>(txt);

            if (output != null)
            {
                foreach (var cid in output)
                    config.ContentIDLookup.TryAdd(cid.Key, cid.Value);
            }
        }
    }
}
public static class FormationCalculation
{
    internal static KeyValuePair<Vector3, float> RelativeToAbsolute(FormationEntry relativePosition, IPlayerCharacter absolutTarget)
    {
        var absolutePosition = RelativeToAbsolute(new KeyValuePair<Vector3, float>(relativePosition.RelativePosition, relativePosition.RelativeRotation), 
            new KeyValuePair<Vector3, float>(absolutTarget.Position,            absolutTarget.Rotation));
        return new KeyValuePair<Vector3, float>(absolutePosition.Key, absolutePosition.Value);
    }

    internal static KeyValuePair<Vector3, float> RelativeToAbsolute(KeyValuePair<Vector3, float> relativePosition, KeyValuePair<Vector3, float> absolutPosition)
    {
        //Create the roatation matrix
        var rotationMatrix = Matrix4x4.CreateRotationY(absolutPosition.Value + MathF.PI);
        //Transform relative pos to our absolute and add the absolute position
        //add the rel rotation to our absolute
        //Will turned later on by +pi to correct the viewing direction
        return new KeyValuePair<Vector3, float>(Vector3.Transform(relativePosition.Key, rotationMatrix) + absolutPosition.Key, relativePosition.Value + absolutPosition.Value);
    }

    internal static KeyValuePair<Vector3, float> AbsoluteToRelative(KeyValuePair<Vector3, float> target, KeyValuePair<Vector3, float> center)
    {
        var rotationMatrix = Matrix4x4.CreateRotationY(-center.Value - MathF.PI);
        return new KeyValuePair<Vector3, float>(Vector3.Transform(target.Key - center.Key, rotationMatrix), target.Value - center.Value);
    }
}