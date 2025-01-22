# Godot ANT gRPC Demo
The demo project uses the ANT+ gRPC service to connect to ANT devices and
display the data in the Godot project. Logging is output to the Godot editor
console for debugging purposes.
## Prerequisites
- Godot v4.3.stable.mono.official
- Visual Studio Community 2022
- Clone this demo project
- Clone the [ANT+ repo](https://github.com/StephenHidem/AntPlus)
### Optional for Simulating ANT Devices
- 2 ANT+ USB Sticks. The second stick is used to simulate ANT devices.
- [SimulANT+](https://www.thisisant.com/developer/resources/downloads/) for simulating ANT devices
## Setup
1. Install the ANT+ USB sticks and drivers if needed
1. Build the ANT+ solution in Visual Studio
1. Run the ANT+ project AntGrpcService
    -  Select the HTTPS option
1. Run SimulANT+ and select one of the supported ANT devices, e.g., Heart Rate Monitor or Bike Power
1. Run the demo project in Godot
# New Project Setup
1. Create a new Godot project
1. Add a top-level node to the scene and name it `Main`
1. Add a new C# script to the `Main` node
1. Open the solution in Visual Studio
1. Add the following NuGet packages to the project:
	- `SmallEarthTech.AntPlus.Extensions.Hosting`
	- `Grpc.Net.Client`
	- `Google.Protobuf`
	- `Godot.DependencyInjection` - This package adds support for logging and dependency injection
1. Right-click Dependencies in the Solution Explorer and select Add Reference.
1. Click browse and select AntGrpcShared.dll (netstandard2.1\debug or netstandard2.1\release build) from the cloned AntPlus solution.
1. Add the following code to the Main.cs script constructor:
```csharp
public Main()
{
    // create the host
    _host = Host.CreateDefaultBuilder(_options).
        UseAntPlus().   // add ANT libraries and hosting extensions to services
        ConfigureServices(services =>
        {
            // add the ANT radio service and cancellation token to signal app termination
            services.AddSingleton<IAntRadio, AntRadioService>();
            services.AddSingleton(_cancellationTokenSource);

            // add Godot services
            services.AddGodotServices();
        }).
        Build();

    // get the ANT radio service
    _antRadioService = _host.Services.GetRequiredService<IAntRadio>() as AntRadioService;

    // get the ANT device collection
    _antDevices = _host.Services.GetRequiredService<AntCollection>();

    // get the _logger
    _logger = _host.Services.GetRequiredService<ILogger<Main>>();
}
```
9. Add the following code to the Main.cs script `_Ready` method:
```csharp
public override void _Ready()
{
    // SETUP ANY GODOT NODES HERE

    // search for an ANT radio server on the local network
    _ = Task.Run(async () =>
    {
        try
        {
            await _antRadioService.FindAntRadioServerAsync();

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
```
10. Add a CollectionChanged event handler to the Main.cs script:
```csharp
private void AntDevices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
{
    // handle the ANT device collection changes
    switch (e.Action)
    {
        case NotifyCollectionChangedAction.Add:
            foreach (var device in e.NewItems)
            {
                // handle the new device
                _logger.LogInformation($"New device: {device}");
            }
            break;
        case NotifyCollectionChangedAction.Remove:
            foreach (var device in e.OldItems)
            {
                // handle the removed device
                _logger.LogInformation($"Removed device: {device}");
            }
            break;
        case NotifyCollectionChangedAction.Reset:
            // handle the reset
            _logger.LogInformation("Device collection reset.");
            break;
    }
}
```
Provide methods to manage collection changes, such as adding or removing devices.
