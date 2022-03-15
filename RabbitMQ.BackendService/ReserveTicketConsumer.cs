using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using RabbitMQ.IntegrationMessages;
using System.Threading.Channels;

namespace RequestReply.ApiGateway
{
    public class ReserveTicketConsumer :
        IConsumer<ReserveTicketResponse>
    {
        private ILogger<ReserveTicketConsumer> _logger;
        private Channel<ReserveTicketResponse> _commChannel;
        public ReserveTicketConsumer(
            ILogger<ReserveTicketConsumer> logger,
            Channel<ReserveTicketResponse> commChannel)
        {
            _logger = logger;
            _commChannel = commChannel;
        }


        public async Task Consume(ConsumeContext<ReserveTicketResponse> context)
        {
            await _commChannel.Writer.WriteAsync(context.Message);
        }
    }

    public class SubmitOrderConsumerDefinition :
    ConsumerDefinition<ReserveTicketConsumer>
    {
        public SubmitOrderConsumerDefinition()
        {
            // override the default endpoint name, for whatever reason
            EndpointName = "ticket-reservation-responses";
            ConcurrentMessageLimit = 4;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReserveTicketConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        }
    }


}
