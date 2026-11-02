using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubTrack.Api.Data;
using SubTrack.Api.Dto;
using SubTrack.Api.Models;

namespace SubTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController(SubscriptionContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<DashboardResponseDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponseDto>> Get(CancellationToken cancellationToken)
    {
        var active = await context.Subscriptions
            .AsNoTracking()
            .Include(subscription => subscription.Category)
            .Where(subscription => subscription.Status == SubscriptionStatus.Active)
            .ToListAsync(cancellationToken);

        var monthly = active.Select(subscription => new
        {
            subscription.Category.Name,
            subscription.Category.ColorCode,
            Amount = subscription.Price / (subscription.BillingCycle switch
            {
                BillingCycle.Monthly => 1m,
                BillingCycle.Quarterly => 3m,
                BillingCycle.Yearly => 12m,
                _ => 1m
            })
        });
        var monthlyTotal = monthly.Sum(item => item.Amount);
        var byCategory = monthly
            .GroupBy(item => new { item.Name, item.ColorCode })
            .Select(group => new CategorySpendingDto(group.Key.Name, group.Key.ColorCode, group.Sum(item => item.Amount)))
            .OrderByDescending(item => item.MonthlyAmount)
            .ToList();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var renewals = await context.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.Status == SubscriptionStatus.Active
                && subscription.NextBillingDate >= today
                && subscription.NextBillingDate <= today.AddDays(7))
            .OrderBy(subscription => subscription.NextBillingDate)
            .Select(subscription => new RenewalDto(
                subscription.Id,
                subscription.Name,
                subscription.NextBillingDate,
                subscription.Price,
                subscription.Currency))
            .ToListAsync(cancellationToken);

        return Ok(new DashboardResponseDto(monthlyTotal, monthlyTotal * 12, byCategory, renewals));
    }
}
