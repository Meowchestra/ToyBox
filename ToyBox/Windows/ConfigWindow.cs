using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ToyBox.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration;

    public ConfigWindow(ToyBox plugin) : base(
        "Config Window")
    {
        this.Size = new Vector2(350, 300);
        this.SizeCondition = ImGuiCond.Appearing;
        this.configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
    }
}
