using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ITI.GrpcDemo.Server.Protos;
using static ITI.GrpcDemo.Server.Protos.TrackingSerivce;

namespace ITI.GrpcDemo.Server.Services
{
    public class MessageTrackingService : TrackingSerivceBase
    {
        private readonly ILogger<MessageTrackingService> _logger;

        public MessageTrackingService(ILogger<MessageTrackingService> logger)
        {
            _logger = logger;
        }

        public override async Task<TrackingMessageResponse> ReviceMessage(TrackingMessage request, ServerCallContext context)
        {
            if (request.DateTime is null)
            {
                Metadata entries = new Metadata
                {
                    { "DateTimeValue", "DateTime is required" }
                };
                throw new RpcException(new Status(StatusCode.InvalidArgument, "DateTime is required"), entries);
                
            }
            _logger.LogInformation($"Message received from deviceId: {request.DeviceId} at location : ({request.Location.Latitude}, {request.Location.Longitude}) at {request.DateTime.ToDateTime()}");

            return await Task.FromResult(new TrackingMessageResponse
            {
                Status = true
            });
        }

        public override async Task<Empty> KeepAlive(IAsyncStreamReader<PulseMessage> requestStream, ServerCallContext context)
        {
            //while (await requestStream.MoveNext())
            //{
            //    var request = requestStream.Current;
            //    _logger.LogInformation($"Keep alive received from deviceId: {request.DeviceId} with status: {request.Status} at {request.SentTime.ToDateTime()} Additional Details: {request.Details.Title}");
            //}
            //while(!context.CancellationToken.IsCancellationRequested)
            //{
            //}

            await foreach (var request in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"Keep alive received from deviceId: {request.DeviceId} with status: {request.Status} at {request.SentTime.ToDateTime()} Additional Details: {request.Details.Title}");
            }

            return await Task.FromResult(new Empty());
        }

        public override Task Subscribe(SubscribeMessage request, IServerStreamWriter<Nofication> responseStream, ServerCallContext context)
        {
            while(!context.CancellationToken.IsCancellationRequested)
            {
                Task.Run(async () =>
                {
                    var notification = new Nofication
                    {
                        Message = "Hello from server",
                        DateTime = Timestamp.FromDateTime(DateTime.UtcNow)
                    };

                    await responseStream.WriteAsync(notification);
                    await Task.Delay(1000);
                });
            }

            return Task.CompletedTask;
        }
    }
}
