using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using RabbitMQ.IntegrationMessages;

namespace RequestReply.Payments
{
    public class ReserveTicketConsumer :
        IConsumer<ReserveTicketRequest>
    {
        public ReserveTicketConsumer(ILogger<ReserveTicketConsumer> logger)
        {
            _logger = logger;
        }

        public ILogger<ReserveTicketConsumer> _logger { get; }

        public async Task Consume(ConsumeContext<ReserveTicketRequest> context)
        {
            _logger.LogInformation($"Received message with Correlation Id {context.CorrelationId}");

            await Task.Delay(Random.Shared.Next(1000, 5000));

            await context.Send<ReserveTicketResponse>(new
            {
                CustomerId = context.Message.CustomerId,
                Token = Guid.NewGuid(),
                ShowId = Guid.NewGuid(),
                Status = true,
                RequestId = context.Message.RequestId
            }) ;
        }
    }

    public class SubmitOrderConsumerDefinition :
    ConsumerDefinition<ReserveTicketConsumer>
    {
        public SubmitOrderConsumerDefinition()
        {
            EndpointName = "ticket-reservation-requests";
            ConcurrentMessageLimit = 4;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReserveTicketConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        }
    }
}
