using RabbitMQ.IntegrationMessages;
using System.Threading.Channels;

namespace RequestReply.ApiGateway
{
    public class BusEventsCommunicationChannel<T> where T : IBusMessage
    {
        private readonly HashSet<Channel<T>> channels;

        public BusEventsCommunicationChannel()
        {
            channels = new HashSet<Channel<T>>();
        }
        
        public void AddChannel(Channel<T> channel)
        {
            channels.Add(channel);
        }

        public void RemoveChannel(Channel<T> channel)
        {
            channels.Remove(channel);
        }

        public async Task PushEvent(T message)
        {
            foreach(var channel in channels)
            {
                await channel.Writer.WriteAsync(message);
            }
        }
    }
}
