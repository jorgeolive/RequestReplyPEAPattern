namespace RabbitMQ.IntegrationMessages
{
    public interface ReserveTicketResponse : IBusMessage
    {
        public bool Status { get; }
        public Guid CustomerId { get; }
        public Guid ShowId { get; }
        public Guid RequestId { get; }
        public Guid? Token { get; }
    }
}
