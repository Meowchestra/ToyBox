using Dalamud.Configuration;
using Dalamud.Plugin;
using ToyBox.Formations;
using ToyBox.Misc;

namespace ToyBox;

[Serializable]

public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public List<FormationsData?> FormationsList { get; set; } = [];

    public Dictionary<long, KeyValuePair<string, string>> ContentIDLookup { get; set; } = new();


    public bool AllowMultiBox { get; set; }

    public bool SetWindowTitle { get; set; }

    public bool MainWindowVisible { get; set; } = true;

    // the below exist just to make saving less cumbersome
    [NonSerialized]
    private IDalamudPluginInterface? pluginInterface;

    public void Initialize(IDalamudPluginInterface? pluginInterface)
    {
        this.pluginInterface = pluginInterface;

        //if mb is enabled
        if (AllowMultiBox)
            Multiboxing.RemoveHandle();
    }

    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);
    }
}