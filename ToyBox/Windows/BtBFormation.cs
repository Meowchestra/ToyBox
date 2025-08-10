using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using ToyBox.Formations;

namespace ToyBox.Windows;

public class BtBFormation : Window, IDisposable
{
    private Configuration configuration;

    public BtBFormation(ToyBox plugin) : base(
        "BtB formation importer",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size          = new Vector2(350, 300);
        SizeCondition = ImGuiCond.Always;
        configuration = plugin.Configuration;
    }

    public void Dispose() { }


    private FileDialogManager? fileDialogManager;
    string loadedFilePath = "";

    static List<string> comboData = [];
    static string current_item = "";
    static string error_msg = "";
    public override void Draw()
    {
        if (ImGui.Button("Open BtB Config"))
        {
            if (fileDialogManager == null)
            {
                fileDialogManager                  =  new FileDialogManager();
                if (Api.PluginInterface != null) 
                    Api.PluginInterface.UiBuilder.Draw += fileDialogManager.Draw;
                fileDialogManager.OpenFileDialog("Select File...", "json File{.json}", (b, files) =>
                {
                    if (files.Count != 1) return;
                    loadedFilePath = files[0];
                    comboData      = FormationFactory.ReadBtBFormationNames(loadedFilePath);
                }, 1, loadedFilePath, true);
                fileDialogManager = null;
            }
        }

        if (ImGui.BeginCombo("##combo", current_item))
        {
            for (var n = 0; n < comboData.Count; n++)
            {
                var is_selected = current_item == comboData[n];
                if (ImGui.Selectable(comboData[n], is_selected))
                    current_item = comboData[n];
                if (is_selected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        if (ImGui.Button("Read Formation"))
        {
            if (loadedFilePath != "" && current_item != "")
            {
                if (configuration.FormationsList.Exists(n => n?.Name != null && n.Name.Equals(current_item)))
                {
                    error_msg = "Formation already exists.";
                    ImGui.OpenPopup("ErrorPopUp");
                }
                else
                {
                    var fData = FormationFactory.ConvertBtBFormation(loadedFilePath, current_item);
                    if (fData != null)
                    {
                        configuration.FormationsList.Add(fData);
                        FormationFactory.CheckMissingCIDs(loadedFilePath, current_item, configuration);
                        configuration.Save();
                    }
                }
            }
        }



        if (ImGui.BeginPopupModal("ErrorPopUp"))
        {
            ImGui.Text(error_msg);
            if (ImGui.Button("Okay"))
                ImGui.CloseCurrentPopup();
            ImGui.EndPopup();
        }

    }
}