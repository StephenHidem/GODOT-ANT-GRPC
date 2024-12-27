# Godot ANT gRPC Demo
This project demonstrates how to use the ANT+ gRPC service in a Godot project.
## New Project Setup
1. Create a new Godot project
1. Add a top-level node to the scene and name it `Main`
1. Add a new C# script to the `Main` node
1. Open the solution in Visual Studio
1. Add the following NuGet packages to the project:
    - `SmallEarthTech.AntPlus.Extensions.Hosting`
    - `Grpc.Net.Client`
    - `Google.Protobuf`
1. Add a new folder to the project named `Services`
1. Add the following files to the `Services` folder from the cloned demo project:
    - `AntChannelService.cs`
    - `AntRadioService.cs`
    - `GrpcAntResponse.cs`