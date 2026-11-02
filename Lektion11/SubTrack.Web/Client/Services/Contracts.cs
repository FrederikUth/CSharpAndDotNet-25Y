using System.ComponentModel.DataAnnotations;

namespace Client.Services;

public enum BillingCycle
{
    Monthly,
    Quarterly,
    Yearly
}

public enum SubscriptionStatus
{
    Active,
    Paused,
    Cancelled
}

public sealed class SubscriptionDraft
{
    public int? Id { get; set; }

    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999")]
    public decimal Price { get; set; }

    [Required, RegularExpression("^[A-Z]{3}$")]
    public string Currency { get; set; } = "DKK";

    public BillingCycle BillingCycle { get; set; } = BillingCycle.Monthly;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly NextBillingDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int PaymentMethodId { get; set; }
}

public sealed class CategoryDraft
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")]
    public string ColorCode { get; set; } = "#5b6df5";
}

public sealed class PaymentMethodDraft
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}

public sealed record SubscriptionItem(
    int Id,
    string Name,
    decimal Price,
    string Currency,
    BillingCycle BillingCycle,
    DateOnly StartDate,
    DateOnly NextBillingDate,
    SubscriptionStatus Status,
    int CategoryId,
    string CategoryName,
    string CategoryColorCode,
    int PaymentMethodId,
    string PaymentMethodName);

public sealed record CategoryItem(int Id, string Name, string ColorCode);
public sealed record PaymentMethodItem(int Id, string Name);
public sealed record CategorySpending(string CategoryName, string ColorCode, decimal MonthlyAmount);
public sealed record RenewalItem(int Id, string Name, DateOnly NextBillingDate, decimal Price, string Currency);
public sealed record DashboardSummary(
    decimal MonthlyTotal,
    decimal AnnualTotal,
    IReadOnlyList<CategorySpending> SpendingByCategory,
    IReadOnlyList<RenewalItem> UpcomingRenewals);
