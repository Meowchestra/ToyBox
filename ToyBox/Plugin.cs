using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ToyBox.Functions;
using ToyBox.IPC;
using ToyBox.Misc;
using ToyBox.Windows;

namespace ToyBox;
public class ToyBox : IDalamudPlugin
{
    public string Name => "ToyBox";

    private static IDalamudPluginInterface PluginInterface { get; set; }

    public Configuration Configuration { get; init; }
    public WindowSystem WindowSystem = new("ToyBox");

    public Api? api { get; set; }

    private Commands commands { get; set; }

    private ConfigWindow ConfigWindow { get; init; }
    private BtBFormation BtBFormation { get; init; }
    private FormationEditor FormationEditor { get; init; }
    private MainWindow MainWindow { get; init; }

    public bool SuspendMainUi { get; set; } = false;

    private ulong ContentId { get; set; }
    private string PlayerName { get; set; } = "";

    public ToyBox(IDalamudPluginInterface pluginInterface, IChatGui chatGui, IDataManager data, ICommandManager commandManager, IClientState clientState, IPartyList partyList)
    {
        api = pluginInterface.Create<Api>();

        PluginInterface = pluginInterface;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        //Init the IPC - Receiver
        IPCProvider.Initialize();
        Broadcaster.Initialize(this);
        BackgroundRunner.Instance.Initialize(this);
        CamHack.Instance.Initialize();

        Api.ClientState.Login  += OnLogin;
        Api.ClientState.Logout += OnLogout;

        //Build and register the Windows
        ConfigWindow    = new ConfigWindow(this);
        BtBFormation    = new BtBFormation(this);
        FormationEditor = new FormationEditor(this);
        MainWindow      = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(BtBFormation);
        WindowSystem.AddWindow(FormationEditor);
        WindowSystem.AddWindow(MainWindow);

        commands = new Commands(this, commandManager);

        PluginInterface.UiBuilder.Draw         += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += UiBuilder_OpenConfigUi;
        PluginInterface.UiBuilder.OpenMainUi   += UiBuilder_OpenMainUi;

        MainWindow.IsOpen = Configuration.MainWindowVisible;
        OnLogin();
    }

    private void UiBuilder_OpenMainUi()
    {
        MainWindow.IsOpen = true;
    }

    private void UiBuilder_OpenConfigUi()
    {
        ConfigWindow.IsOpen = true;
    }

    public void Dispose()
    {
        BackgroundRunner.Instance.Dispose();
        Api.ClientState.Login  -= OnLogin;
        Api.ClientState.Logout -= OnLogout;

        OnLogout();

        Broadcaster.Dispose();
        IPCProvider.Dispose();
        CamHack.Instance.Dispose();

        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        if (Configuration != null)
            Configuration.Save();

        commands.Dispose();
    }

    private void OnLogin()
    {
        if (Api.ClientState.LocalPlayer == null)
            return;
        if (!Api.ClientState.LocalPlayer.IsValid())
            return;

        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCAdd, [
            Api.ClientState.LocalPlayer.Name.TextValue,
            Api.ClientState.LocalPlayer.HomeWorld.Id.ToString(),
            "0"
        ]);

        ContentId  = Api.ClientState.LocalContentId;
        PlayerName = Api.ClientState.LocalPlayer.Name.TextValue;
    }

    private void OnLogout()
    {
        if (Api.ClientState.LocalPlayer == null)
        {
            if (ContentId == 0)
                return;
            Broadcaster.SendMessage(ContentId, MessageType.BCRemove, [PlayerName]);
            ContentId  = 0;
            PlayerName = "";
            return;
        }
        Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.BCRemove, [
            Api.ClientState.LocalPlayer.Name.TextValue,
            Api.ClientState.LocalPlayer.HomeWorld.Id.ToString()
        ]);
    }

    private void DrawUI()
    {
        if (!SuspendMainUi)
            WindowSystem.Draw();
    }

    public bool MainWindowState() => MainWindow.IsOpen;

    public void ToggleDrawMainUI()
    {
        MainWindow.IsOpen = !MainWindow.IsOpen;
    }

    public void ToggleDrawConfigUI()
    {
        ConfigWindow.IsOpen = !ConfigWindow.IsOpen;
    }

    public void ToggleDrawBtBUI()
    {
        BtBFormation.IsOpen = !BtBFormation.IsOpen;
    }

    public void ToggleDrawFormationEditUI()
    {
        FormationEditor.IsOpen = !FormationEditor.IsOpen;
    }
}