using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using RabbitMQ.IntegrationMessages;

namespace RequestReply.ApiGateway
{
    public class ReserveTicketConsumer :
        IConsumer<ReserveTicketResponse>
    {
        private BusEventsCommunicationChannel<ReserveTicketResponse> _commChannel;
        public ReserveTicketConsumer(
            ILogger<ReserveTicketConsumer> logger,
            BusEventsCommunicationChannel<ReserveTicketResponse> commChannel)
        {
            Logger = logger;
            _commChannel = commChannel;
        }

        public ILogger<ReserveTicketConsumer> Logger { get; }

        public async Task Consume(ConsumeContext<ReserveTicketResponse> context)
        {
            await _commChannel.PushEvent(context.Message);
        }
    }

    public class SubmitOrderConsumerDefinition :
    ConsumerDefinition<ReserveTicketConsumer>
    {
        public SubmitOrderConsumerDefinition()
        {
            // override the default endpoint name, for whatever reason
            EndpointName = "ticket-reservation-responses";

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            ConcurrentMessageLimit = 4;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReserveTicketConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        }
    }


}
