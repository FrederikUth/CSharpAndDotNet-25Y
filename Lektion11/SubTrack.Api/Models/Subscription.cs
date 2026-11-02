namespace SubTrack.Api.Models;

public sealed class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "DKK";
    public BillingCycle BillingCycle { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly NextBillingDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;
}
