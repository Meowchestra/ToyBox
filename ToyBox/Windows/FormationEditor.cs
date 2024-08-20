using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImPlotNET;
using Newtonsoft.Json;
using ToyBox.Formations;
using ToyBox.IPC;

namespace ToyBox.Windows;

public class FormationEditor : Window, IDisposable
{
    private Configuration configuration;

    public FormationEditor(ToyBox plugin) : base(
        "FormationEditor")
    {
        Size          = new Vector2(1280, 800);
        SizeCondition = ImGuiCond.Appearing;
        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    string selected_formation = "";
    FormationsData formation;

    public override void Draw()
    {
        float width = ImGui.GetWindowWidth() * 40 / 100;
        ImGui.Columns(2);
        ImGui.SetColumnWidth(0, ImGui.GetWindowWidth() - width);
        ImGui.SetColumnWidth(1, width);

        if (ImGui.BeginCombo("Saved Formations", selected_formation))
        {
            foreach (var formations in configuration.FormationsList)
            {
                bool isSelected = false;
                if (ImGui.Selectable(formations.Name, isSelected))
                {
                    selected_formation = formations.Name;
                    var tformation = configuration.FormationsList.FirstOrDefault(n => n.Name.Equals(selected_formation));
                    if (tformation != null)
                        formation = JsonConvert.DeserializeObject<FormationsData>(JsonConvert.SerializeObject(tformation)); //q&d copy formation
                }
            }
            ImGui.EndCombo();
        }

        if (formation != null)
            DrawEntry(formation.formationEntry);

        //Side panel
        ImGui.NextColumn();
        if (ImGui.Button("New"))
        {
            var formations = FormationFactory.CreateNewFormation();
            selected_formation = formations.Name;
            formation          = formations;
        }
        ImGui.SameLine();
        if (ImGui.Button("Test"))
        {
            if (formation != null)
                FormationFactory.LoadFormation(formation);
        }
        ImGui.SameLine();
        if (ImGui.Button("Save"))
        {
            if (formation != null)
            {
                int idx = -1;
                for (int i = 0; i != configuration.FormationsList.Count; i++)
                {
                    if (configuration.FormationsList[i].Name.Equals(selected_formation))
                    {
                        idx = i;
                        break;
                    }
                }
                Api.PluginLog.Debug(idx.ToString());
                if (idx != -1)
                {
                    configuration.FormationsList.RemoveAt(idx);
                    configuration.FormationsList.Insert(idx, JsonConvert.DeserializeObject<FormationsData>(JsonConvert.SerializeObject(formation)));
                }
                else
                    configuration.FormationsList.Add(JsonConvert.DeserializeObject<FormationsData>(JsonConvert.SerializeObject(formation)));
            }
        }
        ImGui.SameLine();
        if (ImGui.Button("Delete"))
        {
            if (formation != null)
                configuration.FormationsList.RemoveAll(n => n.Name.Equals(formation.Name));
        }

        if (formation != null)
        {
            ImGui.BeginChild("##DATA");
            ImGui.Separator();
            ImGui.InputText("Formation Name: ", ref formation.Name, 50);

            foreach (var p in formation.formationEntry)
            {
                ImGui.Separator();
                ImGui.Text("Name:");
                ImGui.SameLine();
                ImGui.Text(configuration.ContentIDLookup[p.Key].Key + " " + configuration.ContentIDLookup[p.Key].Value);
                ImGui.Text("Position:");
                ImGui.PushItemWidth(width / 3);
                ImGui.InputFloat("##" + p.Key + " X", ref p.Value.RelativePosition.X);
                ImGui.SameLine();
                ImGui.InputFloat("##" + p.Key + " Z", ref p.Value.RelativePosition.Z);
                ImGui.PopItemWidth();

                ImGui.Text("Rotation:");
                SliderFloatDefault("##" + p.Key, ref p.Value.RelativeRotation, MathF.PI, -MathF.PI, 0.0f);
            }
            ImGui.EndChild();
        }
    }

    static bool SliderFloatDefault(string label, ref float v, float v_min, float v_max, float v_default)
    {
        bool ret = ImGui.SliderFloat(label, ref v, v_min, v_max);
        if (ImGui.BeginPopupContextItem(label))
        {
            if (ImGui.MenuItem("Reset to: " + v_default))
                v = v_default;
            ImGui.MenuItem("Close");
            ImGui.EndPopup();
        }
        return ret;
    }

    /// <summary>
    /// Draw the tiangle
    /// </summary>
    /// <param name="inputRotation"></param>
    /// <param name="ord"></param>
    /// <param name="offsettemplate"></param>
    private static void DrawTri(float inputRotation, Vector2 position, uint color)
    {
        Vector2[] triangle =
        [
            new(0.0f, 1f),
            new(1f, -1f),
            new(0.0f, -1.0f),
            new(-1f, -1f)
        ];
        Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(inputRotation);
        for (int index = 0; index < triangle.Length; index++)
        {
            Vector2 absPos = Vector2.Transform(triangle[index] / 2f, rotationMatrix) + position;
            triangle[index] = ImPlot.PlotToPixels(absPos.X, absPos.Y);
        }
        ImDrawListPtr plotDrawList = ImPlot.GetPlotDrawList();
        plotDrawList.AddPolyline(ref triangle[0], triangle.Length, color, ImDrawFlags.Closed, 0.8f);
    }

    /// <summary>
    /// Draw the entries
    /// </summary>
    /// <param name="formation"></param>
    private void DrawEntry(Dictionary<long, FormationEntry> formation)
    {
        if (formation.Count == 0)
            return;

        uint colorOnline = ImGui.GetColorU32(ImGuiColors.DalamudViolet);
        uint colorOffline = ImGui.GetColorU32(ImGuiColors.DalamudRed);

        //get max axis scale
        float limit = formation.Values.Max(n => Math.Abs(n.RelativePosition.X) >= Math.Abs(n.RelativePosition.Z) ? Math.Abs(n.RelativePosition.X) : Math.Abs(n.RelativePosition.Z)) + 1f;
        ImPlot.SetNextAxisLimits(ImAxis.X1, -limit, limit, ImPlotCond.Always);
        ImPlot.SetNextAxisLimits(ImAxis.Y1, -limit, limit, ImPlotCond.Always);
        if (ImPlot.BeginPlot("Formation", new Vector2(-1f), ImPlotFlags.NoTitle | ImPlotFlags.NoLegend | ImPlotFlags.NoMouseText | ImPlotFlags.NoMenus | ImPlotFlags.NoBoxSelect | ImPlotFlags.NoChild | ImPlotFlags.Equal))
        {
            ImPlot.PushPlotClipRect();
            int pointId = 0;
            foreach (FormationEntry formationEntry in formation.Values)
            {
                //The 2D position (Z is reverse)
                Vector2 posVector2D = new Vector2(formationEntry.RelativePosition.X, -formationEntry.RelativePosition.Z);
                double x = posVector2D.X;
                double y = posVector2D.Y;
                //set color for on/offline
                uint color = LocalPlayerCollector.localPlayers.Exists(n=> n.LocalContentId == (ulong)formationEntry.CID) ? colorOnline : colorOffline;
                //Draw triangle
                DrawTri(formationEntry.RelativeRotation, posVector2D, color);
                //Draw midpoint
                ImPlot.DragPoint(pointId++, ref x, ref y, ImGui.ColorConvertU32ToFloat4(color));
                //Draw the text
                ImPlot.PlotText(configuration.ContentIDLookup[formationEntry.CID].Key, x, y);
                //store new positions
                formationEntry.RelativePosition.X = (float)x;
                formationEntry.RelativePosition.Z = -(float)y;
            }
            ImPlot.PopPlotClipRect();
            ImPlot.EndPlot();
        }
    }
}