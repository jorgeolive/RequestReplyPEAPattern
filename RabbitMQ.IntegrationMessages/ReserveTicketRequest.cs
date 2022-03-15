namespace RabbitMQ.IntegrationMessages
{
    public interface ReserveTicketRequest : IBusMessage
    {
        Guid RequestId { get; }
        Guid CustomerId { get; }
        Guid ReserveId { get; }
    }
}