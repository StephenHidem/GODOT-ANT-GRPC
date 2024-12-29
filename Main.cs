using Godot;
using GodotAntGrpc.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
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
    private AntCollectionList _antCollectionList;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Debug.WriteLine("Main._Ready()");
        _antCollectionList = GetNode<AntCollectionList>("AntCollectionList");

        // create the host
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
                _logger.LogInformation("ANT Radio Server found.");
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

    private void AntDevices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (AntDevice item in e.NewItems)
                {
                    _antCollectionList.AddDevice(item.ToString(), CreateDeviceTexture(item));
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    _antCollectionList.RemoveDevice(item.ToString());
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                _antCollectionList.ClearDevices();
                break;
            case NotifyCollectionChangedAction.Replace:
                foreach (AntDevice item in e.NewItems)
                {
                    _antCollectionList.UpdateDevice(item.ToString(), CreateDeviceTexture(item));
                }
                break;
        }
    }

    private static Texture2D CreateDeviceTexture(AntDevice item)
    {
        using MemoryStream ms = new();
        item.DeviceImageStream.CopyTo(ms);
        ms.Position = 0;
        Image image = new();
        image.LoadPngFromBuffer(ms.ToArray());
        image.Resize(64, 64);
        return ImageTexture.CreateFromImage(image);
    }
}
