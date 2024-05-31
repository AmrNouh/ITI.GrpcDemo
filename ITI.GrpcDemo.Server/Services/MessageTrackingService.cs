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
            _logger.LogInformation($"Message received from deviceId: {request.DeviceId} at location : ({request.Location.Latitude}, {request.Location.Longitude}) at {request.DateTime.ToDateTime()}");

            return await Task.FromResult(new TrackingMessageResponse
            {
                Status = true
            });
        }
    }
}
