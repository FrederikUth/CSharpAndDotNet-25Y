namespace SubTrack.Api.Models;

public sealed class PaymentMethod
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
