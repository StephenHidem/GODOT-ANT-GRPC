using Godot;
using GodotAntGrpc.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Main : Node2D
{
    private readonly string[] _options = {
        "--Logging:LogLevel:Default=Debug",
        "--TimeoutOptions:MissedMessages=10"
    };
    private IHost _host;
    private ILogger<Main> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private AntRadioService _antRadioService;
    private AntCollection _antDevices;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _host = Host.CreateDefaultBuilder(_options).
            UseAntPlus().   // add ANT libraries and hosting extensions to services
            ConfigureServices(services =>
            {
                // add the ANT radio service and cancellation token to signal app termination
                services.AddSingleton<IAntRadio, AntRadioService>();
                services.AddSingleton(_cancellationTokenSource);
            }).
            Build();

        // get the logger
        _logger = _host.Services.GetRequiredService<ILogger<Main>>();

        // search for an ANT radio server on the local network
        _antRadioService = _host.Services.GetRequiredService<IAntRadio>() as AntRadioService;
        _ = Task.Run(async () =>
        {
            try
            {
                await _antRadioService.FindAntRadioServerAsync();

                _antDevices = _host.Services.GetRequiredService<AntCollection>();
                _antDevices.CollectionChanged += AntDevices_CollectionChanged;

                // IMPORTANT: Initiate scanning on a background thread.
                await _antDevices.StartScanning();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("OperationCanceledException: Exiting game.");
            }
        });
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void AntDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        _logger.LogInformation($"{nameof(AntDevices_CollectionChanged)}: Action = {e.Action}");
    }
}
