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
1. Add a new folder to the project named `Services`
1. Add the following files to the `Services` folder from the cloned demo project:
	- `AntChannelService.cs`
	- `AntRadioService.cs`
	- `GrpcAntResponse.cs`
