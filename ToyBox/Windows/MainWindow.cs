using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using OtterGui.Text;
using OtterGuiInternal.Structs;
using ToyBox.Formations;
using ToyBox.Functions;
using ToyBox.IPC;
using ToyBox.Misc;

namespace ToyBox.Windows;

public class MainWindow : Window, IDisposable
{
    private ToyBox plugin;
    private FormationsData? selected_formation;

    public MainWindow(ToyBox plugin) : base(
        "ToyBox", ImGuiWindowFlags.AlwaysUseWindowPadding)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose()
    {
    }


    public override void OnOpen()
    {
        plugin.Configuration.MainWindowVisible = true;
    }

    public override void OnClose()
    {
        plugin.Configuration.MainWindowVisible = false;
    }

    public override void Draw()
    {
        /*********************************************************/
        /***                   CamHack Settings                ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("CamHack", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Enable Locally"))
                CamHack.Instance.Enable();
            ImGui.SameLine();
            if (ImGui.Button("Enable Others"))
                CamHack.Instance.EnableOthers();
            ImGui.SameLine();
            if (ImGui.Button("Disable"))
            {
                if (Api.ClientState != null)
                    Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.CamHack, [false.ToString()]);
                CamHack.Instance.Disable();
            }
        }
            
        /*********************************************************/
        /***                   Graphic Settings                ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("Graphic Settings", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Set Low except me"))
                GameConfig.SetLowExceptMe();
            ImGui.SameLine();
            if (ImGui.Button("Set Low Locally"))
                IPCProvider.SetGfxLowAction(true);
            ImGui.SameLine();
            if (ImGui.Button("Reset"))
            {
                GameConfig.Reset();
                IPCProvider.SetGfxLowAction(false);
            }
        }

        /*********************************************************/
        /***                      Party Menu                   ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("Party", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Invite to party"))
                Party.InviteToParty();
            ImGui.SameLine();
            if (ImGui.Button("Gimme the lead"))
                Party.GimmePartyLead();
            ImGui.SameLine();
            if (ImGui.Button("Disband"))
                Party.Disband();

            if (ImGui.Button("Teleport"))
                Party.Teleport();
            ImGui.SameLine();
            if (ImGui.Button("Enter House"))
                Party.EnterHouse();

            if (ImGui.Button("Follow Me"))
                Party.FollowMe();
            ImGui.SameLine();
            if (ImGui.Button("Stop Follow"))
                Party.StopFollow();
        }

        /*********************************************************/
        /***                      Misc  Menu                   ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("Misc", ImGuiTreeNodeFlags.DefaultOpen))
        {
            var AllowMultiBox = plugin.Configuration.AllowMultiBox;
            if (ImGui.Checkbox("Enable Multiboxing", ref AllowMultiBox))
            {
                Multiboxing.RemoveHandle();
                plugin.Configuration.AllowMultiBox = AllowMultiBox;
                plugin.Configuration.Save();
            }
            ImGui.SameLine();
            
            var setWindowTitle = plugin.Configuration.SetWindowTitle;
            if (ImGui.Checkbox("Set Window Title", ref setWindowTitle))
            {
                if (setWindowTitle)
                    MiscFunctions.SetWindowText(Process.GetCurrentProcess().MainWindowHandle, Api.ClientState?.LocalPlayer?.Name.TextValue + "@" + Api.ClientState?.LocalPlayer?.HomeWorld.ValueNullable?.Name.ToDalamudString().TextValue);
                else
                    MiscFunctions.SetWindowText(Process.GetCurrentProcess().MainWindowHandle, "FINAL FANTASY XIV");
                plugin.Configuration.SetWindowTitle = setWindowTitle;
                plugin.Configuration.Save();
            }

            if (ImGui.Button("Import BtBFormation"))
            {
                plugin.ToggleDrawBtBUI();
            }
        }

        /*********************************************************/
        /***                   Formations  Menu                ***/
        /*********************************************************/
        if (ImGui.CollapsingHeader("Formations", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.BeginCombo("##combo", selected_formation != null? selected_formation.Name : ""))
            {
                var comboData = plugin.Configuration.FormationsList;
                for (var n = 0; n < plugin.Configuration.FormationsList.Count; n++)
                {
                    var is_selected = selected_formation == comboData[n];
                    if (ImGui.Selectable(comboData[n]?.Name, is_selected))
                        selected_formation = comboData[n];
                    if (is_selected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            if (ImGui.Button("Load"))
            {
                if (selected_formation != null)
                    FormationFactory.LoadFormation(selected_formation);
            }
            ImGui.SameLine();
            if (ImGui.Button("STOP"))
                FormationFactory.StopFormation();

            if (ImGui.Button("Open Editor"))
                plugin.ToggleDrawFormationEditUI();
        }

        /*********************************************************/
        /***                  Characters  Menu                 ***/
        /*********************************************************/
        ImGui.Separator();
        if (ImGui.CollapsingHeader("Connected Chars"))
        {
            foreach (var p in LocalPlayerCollector.localPlayers)
            {
                ImGui.Text(p.Name + " " + p.GetWorldName());
                {
                    ImGui.BeginChild(p.LocalContentId.ToString(), new Vector2(0, 90), true, ImGuiWindowFlags.NoScrollbar);
                    var width = ImGui.GetWindowWidth() * 70 / 100;
                    ImGui.Columns(2);
                    ImGui.SetColumnWidth(0, ImGui.GetWindowWidth() - width);
                    ImGui.SetColumnWidth(1, width);

                    ImGui.PushID("##ID" + p.LocalContentId);
                    var bce = p.BroadCastEnabled;
                    if (ImGui.Checkbox("BC Enabled", ref bce))
                    {
                        p.BroadCastEnabled = bce;
                        MiscFunctions.EnableBCForClient(p);
                    }

                    if (ImUtf8.IconButton(FontAwesomeIcon.SignOutAlt))
                        GameConfig.Logout(p.LocalContentId);

                    ImGui.SameLine();
                    if (ImUtf8.IconButton(FontAwesomeIcon.PowerOff))
                        GameConfig.Shutdown(p.LocalContentId);

                    ImGui.NextColumn();
                    //DrawProcFrame(p);
                    ImGui.Text("");
                    ImGui.PopID();
                    ImGui.EndChild();
                }
                ImGui.Separator();
            }
        }
    }

    public void DrawProcFrame(LocalPlayer player)
    {
        var AffinityMask = player.Affinity;
        ImGui.BeginChildFrame(2, new ImVec2(0, 0), ImGuiWindowFlags.HorizontalScrollbar);
        for (var i = 0; i < ProcAffinity.GetCPUCores(); i++)
        {
            ImGui.PushID("##CPU" + i);

            var flag = (AffinityMask & (1 << i)) > 0;

            ImGui.Checkbox((i+1).ToString("D2"), ref flag);
            if ( i != ProcAffinity.GetCPUCores()/2)
                ImGui.SameLine();

            if (flag)
                AffinityMask |= 1 << i;
            else
                AffinityMask = AffinityMask & ~(1 << i);

            if (AffinityMask != player.Affinity)
                player.Affinity = AffinityMask;
            //ProcAffinity.SetAffinity(AffinityMask);
            ImGui.PopID();
        }
        ImGui.EndChildFrame();
    }
}