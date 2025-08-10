/*
 * Copyright(c) 2025 Meowchestra, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/Meowchestra/MeowMusic/blob/main/LICENSE for full license information.
 */

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ToyBox;

public class Api
{
    [PluginService]
    //[RequiredVersion("1.0")]
    public static IDalamudPluginInterface? PluginInterface { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IChatGui? ChatGui { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IClientState? ClientState { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IGameInteropProvider? GameInteropProvider { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IFramework? Framework { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IPartyList? PartyList { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IPluginLog? PluginLog { get; private set; }

    [PluginService]
    public static IObjectTable? Objects { get; private set; }
}
