using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ToyBox.IPC;

namespace ToyBox.Misc;

public class BackgroundRunner : IDisposable
{
    private static readonly Lazy<BackgroundRunner> LazyInstance = new(static () => new BackgroundRunner());
    public static BackgroundRunner Instance => LazyInstance.Value;

    private CancellationTokenSource cancelToken = new CancellationTokenSource();

    private static ToyBox plugin { get; set; }

    private BackgroundRunner(){}

    public void Initialize(ToyBox pluginmain)
    {
        plugin = pluginmain;

        Api.Framework.RunOnTick(delegate
        {
            cancelToken = new CancellationTokenSource();
            Task.Factory.StartNew(() => SlowRunner(cancelToken.Token), TaskCreationOptions.LongRunning);
        }, default(TimeSpan), 0, default(CancellationToken));
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
            plugin.SuspendMainUi = true;
        else
            plugin.SuspendMainUi = false;
    }
}

