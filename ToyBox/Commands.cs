using System.Text.RegularExpressions;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using ToyBox.IPC;

namespace ToyBox;

public class Commands : IDisposable
{
    private const string CommandName = "/ht";
    private const string CommandBrName = "/hbr";
    private static ICommandManager CommandManager { get; set; }
    private static ToyBox plugin { get; set; }

    public Commands(ToyBox pluginmain, ICommandManager manager)
    {
        plugin         = pluginmain;
        CommandManager = manager;
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });
        CommandManager.AddHandler(CommandBrName, new CommandInfo(OnCommandBr)
        {
            HelpMessage = "Broadcast your message to all."
        });
        CommandManager.AddHandler("/hbrn", new CommandInfo(OnCommandBr)
        {
            HelpMessage = "Broadcast your message to all except yourself."
        });
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler(CommandBrName);
        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommandBr(string command, string args)
    {
        if (args.Length <= 0)
            return;

        if (Api.ClientState.LocalPlayer == null)
            return;

        if (command.Equals(CommandBrName))
            Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.Chat, [true.ToString(), args]);
        else
            Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.Chat, [false.ToString(), args]);
    }

    private void OnCommand(string command, string args)
    {
        var regex = Regex.Match(args, "^(\\w+) ?(.*)");
        var subcommand = regex.Success && regex.Groups.Count > 1 ? regex.Groups[1].Value : string.Empty;

        if (subcommand == "")
        {
            plugin.ToggleDrawMainUI();
            return;
        }

        switch (subcommand.ToLower())
        {
            case "br":
            case "broadcast":
            {
                if (regex.Groups.Count < 2 || string.IsNullOrEmpty(regex.Groups[2].Value))
                {
                    Api.ChatGui?.Print("[Broadcast] missing parameter");
                    return;
                }
                var arg = regex.Groups[2].Value;
                if (Api.ClientState?.LocalPlayer == null)
                    return;

                Broadcaster.SendMessage(Api.ClientState.LocalContentId, MessageType.Chat, [arg]);
            }
                break;
        }
    }

}