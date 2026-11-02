using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubTrack.Api.Data;
using SubTrack.Api.Dto;
using SubTrack.Api.Models;

namespace SubTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubscriptionsController(SubscriptionContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SubscriptionResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SubscriptionResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var subscriptions = await context.Subscriptions
            .AsNoTracking()
            .Include(subscription => subscription.Category)
            .Include(subscription => subscription.PaymentMethod)
            .OrderBy(subscription => subscription.Name)
            .ToListAsync(cancellationToken);

        return Ok(subscriptions.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<SubscriptionResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var subscription = await context.Subscriptions
            .AsNoTracking()
            .Include(item => item.Category)
            .Include(item => item.PaymentMethod)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return subscription is null ? NotFound() : Ok(ToResponse(subscription));
    }

    [HttpPost]
    [ProducesResponseType<SubscriptionResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionResponseDto>> Create(
        SubscriptionRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!await ReferencesExist(request, cancellationToken))
        {
            return BadRequest(new { message = "Kategorien eller betalingsmetoden findes ikke." });
        }

        var subscription = ToEntity(request);
        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync(cancellationToken);
        await LoadReferences(subscription, cancellationToken);

        var response = ToResponse(subscription);
        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        SubscriptionRequestDto request,
        CancellationToken cancellationToken)
    {
        var subscription = await context.Subscriptions.FindAsync([id], cancellationToken);
        if (subscription is null)
        {
            return NotFound();
        }

        if (!await ReferencesExist(request, cancellationToken))
        {
            return BadRequest(new { message = "Kategorien eller betalingsmetoden findes ikke." });
        }

        Apply(request, subscription);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var subscription = await context.Subscriptions.FindAsync([id], cancellationToken);
        if (subscription is null)
        {
            return NotFound();
        }

        context.Subscriptions.Remove(subscription);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<bool> ReferencesExist(SubscriptionRequestDto request, CancellationToken cancellationToken) =>
        await context.Categories.AnyAsync(category => category.Id == request.CategoryId, cancellationToken)
        && await context.PaymentMethods.AnyAsync(method => method.Id == request.PaymentMethodId, cancellationToken);

    private async Task LoadReferences(Subscription subscription, CancellationToken cancellationToken)
    {
        await context.Entry(subscription).Reference(item => item.Category).LoadAsync(cancellationToken);
        await context.Entry(subscription).Reference(item => item.PaymentMethod).LoadAsync(cancellationToken);
    }

    private static Subscription ToEntity(SubscriptionRequestDto request)
    {
        var subscription = new Subscription();
        Apply(request, subscription);
        return subscription;
    }

    private static void Apply(SubscriptionRequestDto request, Subscription subscription)
    {
        subscription.Name = request.Name.Trim();
        subscription.Price = request.Price;
        subscription.Currency = request.Currency.ToUpperInvariant();
        subscription.BillingCycle = request.BillingCycle;
        subscription.StartDate = request.StartDate;
        subscription.NextBillingDate = request.NextBillingDate;
        subscription.Status = request.Status;
        subscription.CategoryId = request.CategoryId;
        subscription.PaymentMethodId = request.PaymentMethodId;
    }

    private static SubscriptionResponseDto ToResponse(Subscription subscription) => new(
        subscription.Id,
        subscription.Name,
        subscription.Price,
        subscription.Currency,
        subscription.BillingCycle,
        subscription.StartDate,
        subscription.NextBillingDate,
        subscription.Status,
        subscription.CategoryId,
        subscription.Category.Name,
        subscription.Category.ColorCode,
        subscription.PaymentMethodId,
        subscription.PaymentMethod.Name);
}
