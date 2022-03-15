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
        private Channel<ReserveTicketResponse> _channel { get; }
        private readonly ILogger<TicketReservationController> _logger;

        public TicketReservationController(
            ISendEndpointProvider sendEndpoint,
            Channel<ReserveTicketResponse> channels)
        {
            _sendEndpoint = sendEndpoint;
            _channel = channels;
        }

        [HttpPost(Name = "reserve-ticket")]
        public async Task<IActionResult> ReserveFor(Guid showId, Guid customerId, Guid requestId)
        {            
            ReserveTicketResponse reserveTicketResponse = default;

            await _sendEndpoint.Send<ReserveTicketRequest>(
                new { RequestId = requestId, CustomerId = customerId, ShowId = showId });

            await foreach(var result in _channel.Reader.ReadAllAsync())
            {
                if(result.RequestId == requestId)
                {
                    reserveTicketResponse = result;
                    break;
                }
            }

            return Ok(reserveTicketResponse);
        }
    }
}