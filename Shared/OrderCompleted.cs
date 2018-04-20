using NServiceBus;

public class OrderCompleted : IEvent
{
    public string OrderId { get; set; }
}

public class PrintInvoice : ICommand
{
    public string OrderId { get; set; }
}