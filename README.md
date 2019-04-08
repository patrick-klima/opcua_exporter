# opcua_exporter
Opcua_exporter is a cross platform (linux/win) tool written in C# dotcore 2.1, that analyses traffic of OPC UA TCP packets and expose the latency for prometheus.

## How to get started
1) Change to the NetworkInfo directory and run ``dotnet run``. You should now get a list of available network interfaces. Write down or remember the id of the network interface where opc ua packets go through.
2) Change to the OpcExporter directory and run the following line:

``dotnet run -- --opc-port 4840 --listen-port 9085 --interface {ID} --verbose``  
where {ID} is the network interface id from step 1.

### Parameters
|Parameter|Description|
|--|--|
| --opc-port | The port used for the OPC UA Server |
| --interface | Network interface to listen to |
| --listen-port | The port where metrics are exposed |
| --verbose | Display additional information |
  
  
![Alt text](/Screenshots/opc_exporter.PNG "screenshot")
