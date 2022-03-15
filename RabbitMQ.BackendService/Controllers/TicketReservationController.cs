using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.IntegrationMessages;
using System.Threading.Channels;

namespace RabbitMQ.BackendService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketReservationController : ControllerBase
    {
        private ISendEndpointProvider _eventBus { get; }
        private Channel<ReserveTicketResponse> _channel { get; }
        private readonly ILogger<TicketReservationController> _logger;

        public TicketReservationController(
            ISendEndpointProvider eventBus,
            Channel<ReserveTicketResponse> channels, ILogger<TicketReservationController> logger)
        {
            _eventBus = eventBus;
            _channel = channels;
            _logger = logger;
        }

        [HttpPost(Name = "reserve-ticket")]
        public async Task<IActionResult> ReserveFor(Guid showId, Guid customerId, Guid requestId)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            ReserveTicketResponse? reserveTicketResponse = default;

            await _eventBus.Send<ReserveTicketRequest>(
                new { RequestId = requestId, CustomerId = customerId, ShowId = showId });

            try
            {
                await foreach(var result in _channel.Reader.ReadAllAsync(cts.Token))
                {
                    if(result.RequestId == requestId)
                    {
                        reserveTicketResponse = result;
                        break;
                    }
                }
            } 
            catch(OperationCanceledException)
            {
                return new UnprocessableEntityResult();
            }

            return Ok(reserveTicketResponse);
        }
    }
}