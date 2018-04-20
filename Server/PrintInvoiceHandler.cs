using System.Threading.Tasks;
using NServiceBus;

namespace Server
{
    public class PrintInvoiceHandler:IHandleMessages<PrintInvoice>
    {
        public async Task Handle(PrintInvoice message, IMessageHandlerContext context)
        {
            var session = context.SynchronizedStorageSession.RavenSession();

            //Upps!!!!
            var item = await session.LoadAsync<PrintInvoiceDataTransfomer.Result>(message.OrderId, typeof(PrintInvoiceDataTransfomer));

        }
    }
}