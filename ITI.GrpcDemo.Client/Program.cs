using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ITI.GrpcDemo.Client.Protos;

var channel = GrpcChannel.ForAddress("https://localhost:7053");
var client = new TrackingSerivce.TrackingSerivceClient(channel);
var rand = new Random();

await SendMessage(client);

//await KeepAlive(client);

//var call = client.Subscribe(new SubscribeMessage { DeviceId = 1 });

//while (await call.ResponseStream.MoveNext(CancellationToken.None))
//{
//    var message = call.ResponseStream.Current;
//    Console.WriteLine($"Message: {message.Message} at {message.DateTime}");
//}


static async Task SendMessage(TrackingSerivce.TrackingSerivceClient client)
{
    try
    {
        var request = new TrackingMessage
        {
            DeviceId = 1,
            //DateTime = Timestamp.FromDateTime(DateTime.UtcNow),
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
    }
    catch (RpcException ex)
    {
        Console.WriteLine(ex.Message);
    }
    
}

static async Task KeepAlive(TrackingSerivce.TrackingSerivceClient client)
{
    var call = client.KeepAlive();

    foreach (var i in Enumerable.Range(1, 10))
    {
        await call.RequestStream.WriteAsync(new PulseMessage
        {
            DeviceId = i,
            SentTime = Timestamp.FromDateTime(DateTime.UtcNow),
            Status = MachineStatus.Start,
            Details = new PulseMessage.Types.AdditionalDetails()
            {
                Title = "Title",
                Description = "Description",
            }
        });
        await Task.Delay(1000);
    }
    await call.RequestStream.CompleteAsync();
}