using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using Raven.Client.Document;
using Raven.Client.Indexes;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.RavenDB.Server";
        
        var endpointConfiguration = new EndpointConfiguration("Samples.RavenDB.Server");
        using (var documentStore = new DocumentStore
        {
            Url = "http://localhost:8181",
            DefaultDatabase = "RavenSimpleSample",
            EnlistInDistributedTransactions = false
        })
        {
            documentStore.Initialize();

            await new PrintInvoiceDataTransfomer().ExecuteAsync(documentStore).ConfigureAwait(false);

            var persistence = endpointConfiguration.UsePersistence<RavenDBPersistence>();
            persistence.DoNotSetupDatabasePermissions();
            persistence.SetDefaultDocumentStore(documentStore);
            
            endpointConfiguration.SetTimeToKeepDeduplicationData(TimeSpan.FromDays(7));
            endpointConfiguration.SetFrequencyToRunDeduplicationDataCleanup(TimeSpan.FromMinutes(1));

            var transport = endpointConfiguration.UseTransport<MsmqTransport>();
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

            endpointConfiguration.EnableOutbox();
            //endpointConfiguration.EnableFeature<TimeoutManager>();
            //endpointConfiguration.EnableFeature<Sagas>();
            //endpointConfiguration.EnableDurableMessages();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.EnableInstallers();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}

public class PrintInvoiceDataTransfomer : AbstractTransformerCreationTask<OrderShipped>
{
    public class Result
    {
        public Guid Id { get; set; }
        public DateTime DoneAt { get; set; }
    }

    public PrintInvoiceDataTransfomer()
    {
        TransformResults = results => from item in results
            select new
            {
                OrderId = item.Id,
                DoneAt = DateTimeOffset.UtcNow
            };
    }
}