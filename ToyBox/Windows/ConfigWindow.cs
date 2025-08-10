using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace ToyBox.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration;

    public ConfigWindow(ToyBox plugin) : base(
        "Config Window")
    {
        Size          = new Vector2(350, 300);
        SizeCondition = ImGuiCond.Appearing;
        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
    }
}