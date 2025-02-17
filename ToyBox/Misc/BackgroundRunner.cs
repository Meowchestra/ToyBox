﻿using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace ToyBox.Misc;

public class BackgroundRunner : IDisposable
{
    private static readonly Lazy<BackgroundRunner> LazyInstance = new(static () => new BackgroundRunner());
    public static BackgroundRunner Instance => LazyInstance.Value;

    private CancellationTokenSource cancelToken = new();

    private static ToyBox? plugin { get; set; }

    private BackgroundRunner(){}

    public void Initialize(ToyBox? pluginmain)
    {
        plugin = pluginmain;

        Api.Framework?.RunOnTick(delegate
        {
            cancelToken = new CancellationTokenSource();
            Task.Factory.StartNew(() => SlowRunner(cancelToken.Token), TaskCreationOptions.LongRunning);
        });
    }

    public void Dispose()
    {
        cancelToken.Cancel();
    }


    private async Task SlowRunner(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (token.IsCancellationRequested)
                break;

            PerformanceShowHideMainUi();

            await Task.Delay(500, token).ContinueWith(static tsk => { }, token);
        }
    }

    private unsafe void PerformanceShowHideMainUi()
    {
        if (AgentModule.Instance()->GetAgentByInternalId(AgentId.PerformanceMode)->IsAgentActive())
        {
            if (plugin != null) plugin.SuspendMainUi = true;
        }
        else if (plugin != null) plugin.SuspendMainUi = false;
    }
}