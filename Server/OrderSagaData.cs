using NServiceBus;

#region sagadata

public class OrderSagaData :
    ContainSagaData
{
    public string OrderId { get; set; }
    public string OrderDescription { get; set; }
}
#endregion