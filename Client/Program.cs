using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.RavenDB.Client";

        var endpointConfiguration = new EndpointConfiguration("Samples.RavenDB.Client");
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        var transport = endpointConfiguration.UseTransport<MsmqTransport>();
        transport.Transactions(TransportTransactionMode.None);

        var routing = transport.Routing();
        routing.RegisterPublisher(
            eventType: typeof(OrderCompleted),
            publisherEndpoint: "Samples.RavenDB.Server");

        endpointConfiguration.SendFailedMessagesTo("error");

        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

        Console.WriteLine("Press 'enter' to send a StartOrder messages");
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }

            var orderId = Guid.NewGuid();
            var startOrder = new StartOrder
            {
                OrderId = orderId.ToString()
            };
            await endpointInstance.Send("Samples.RavenDB.Server", startOrder) .ConfigureAwait(false);

            Console.WriteLine($"StartOrder Message sent with OrderId {orderId}");
        }

        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}