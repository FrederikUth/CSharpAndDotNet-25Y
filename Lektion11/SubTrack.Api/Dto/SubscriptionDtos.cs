using System.ComponentModel.DataAnnotations;
using SubTrack.Api.Models;

namespace SubTrack.Api.Dto;

public sealed class SubscriptionRequestDto
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999")]
    public decimal Price { get; set; }

    [Required, RegularExpression("^[A-Z]{3}$")]
    public string Currency { get; set; } = "DKK";

    [EnumDataType(typeof(BillingCycle))]
    public BillingCycle BillingCycle { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly NextBillingDate { get; set; }

    [EnumDataType(typeof(SubscriptionStatus))]
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int PaymentMethodId { get; set; }
}

public sealed record SubscriptionResponseDto(
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

public sealed record CategoryResponseDto(int Id, string Name, string ColorCode);

public sealed class CategoryRequestDto
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")]
    public string ColorCode { get; set; } = "#5b6df5";
}

public sealed record PaymentMethodResponseDto(int Id, string Name);

public sealed class PaymentMethodRequestDto
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}

public sealed record CategorySpendingDto(string CategoryName, string ColorCode, decimal MonthlyAmount);

public sealed record RenewalDto(int Id, string Name, DateOnly NextBillingDate, decimal Price, string Currency);

public sealed record DashboardResponseDto(
    decimal MonthlyTotal,
    decimal AnnualTotal,
    IReadOnlyList<CategorySpendingDto> SpendingByCategory,
    IReadOnlyList<RenewalDto> UpcomingRenewals);
