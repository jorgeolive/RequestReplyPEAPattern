using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.IntegrationMessages;
using RequestReply.ApiGateway;
using System.Threading.Channels;

namespace RabbitMQ.BackendService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketReservationController : ControllerBase
    {
        private ISendEndpointProvider _sendEndpoint { get; }
        private BusEventsCommunicationChannel<ReserveTicketResponse> _channels { get; }
        private readonly ILogger<TicketReservationController> _logger;

        public TicketReservationController(
            ISendEndpointProvider sendEndpoint,
            BusEventsCommunicationChannel<ReserveTicketResponse> channels)
        {
            _sendEndpoint = sendEndpoint;
            _channels = channels;
        }

        [HttpPost(Name = "reserve-ticket")]
        public async Task<IActionResult> ReserveFor(Guid showId, Guid customerId, Guid requestId)
        {
            var channel = Channel.CreateUnbounded<ReserveTicketResponse>();

            ReserveTicketResponse reserveTicketResponse = default;

            _channels.AddChannel(channel);

            await _sendEndpoint.Send<ReserveTicketRequest>(
                new { RequestId = requestId, CustomerId = customerId, ShowId = showId });

            await channel.Reader.WaitToReadAsync();

            await foreach(var result in channel.Reader.ReadAllAsync())
            {
                if(result.RequestId == requestId)
                {
                    reserveTicketResponse = result;
                    channel.Writer.Complete();
                    await channel.Reader.Completion;
                }
            }

            _channels.RemoveChannel(channel);

            return Ok(reserveTicketResponse);
        }
    }
}