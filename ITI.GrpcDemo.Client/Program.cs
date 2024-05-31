using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using ITI.GrpcDemo.Client.Protos;
using static ITI.GrpcDemo.Client.Protos.TrackingSerivce;

var channel = GrpcChannel.ForAddress("https://localhost:7053");
var client = new TrackingSerivceClient(channel);

var request = new TrackingMessage
{
    DeviceId = 1,
    DateTime = Timestamp.FromDateTime(DateTime.UtcNow),
    Message = "Hello from client",
    Location = new Location
    {
        Latitude = 30.0444f,
        Longitude = 31.2357f
    },
};
request.Sensors.Add(new Sensor { Name = "Sensor1", Value = 1000 });

var response = await client.ReviceMessageAsync(request);

Console.WriteLine($"Response: {response.Status}");