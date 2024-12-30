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

    private Label _ipAddress;
    private Label _description;
    private Label _serialNumber;
    private Label _hostVersion;

    public Main()
    {
        Debug.WriteLine("Main()");
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

        // get the ANT radio service
        _antRadioService = _host.Services.GetRequiredService<IAntRadio>() as AntRadioService;

        // get the ANT device collection
        _antDevices = _host.Services.GetRequiredService<AntCollection>();

        // get the logger
        _logger = _host.Services.GetRequiredService<ILogger<Main>>();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // get the labels associated with the ANT radio server
        _ipAddress = GetNode<Label>("%IPAddress");
        _description = GetNode<Label>("%Description");
        _serialNumber = GetNode<Label>("%SerialNumber");
        _hostVersion = GetNode<Label>("%HostVersion");

        // get the list of ANT devices
        _antCollectionList = GetNode<AntCollectionList>("%AntCollectionList");

        //// search for an ANT radio server on the local network
        _ = Task.Run(async () =>
        {
            try
            {
                await _antRadioService.FindAntRadioServerAsync();
                PopulateAntRadioServerInfo(_antRadioService);

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
                    _antCollectionList.AddDevice(item);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (AntDevice item in e.OldItems)
                {
                    _antCollectionList.RemoveDevice(item);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                _antCollectionList.ClearDevices();
                break;
            case NotifyCollectionChangedAction.Replace:
                foreach (AntDevice item in e.NewItems)
                {
                    _antCollectionList.UpdateDevice(item);
                }
                break;
        }
    }

    private void PopulateAntRadioServerInfo(AntRadioService antRadio)
    {
        _ipAddress.SetDeferred("text", antRadio.ServerIPAddress.ToString());
        _description.SetDeferred("text", antRadio.ProductDescription);
        _serialNumber.SetDeferred("text", antRadio.SerialNumber.ToString());
        _hostVersion.SetDeferred("text", antRadio.Version);
    }
}
