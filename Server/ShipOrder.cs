using NServiceBus;

public class ShipOrder : IMessage
{
    public string OrderId { get; set; }
}