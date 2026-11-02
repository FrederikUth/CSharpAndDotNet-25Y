using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubTrack.Api.Data;
using SubTrack.Api.Dto;
using SubTrack.Api.Models;

namespace SubTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentMethodsController(SubscriptionContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentMethodResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var methods = await context.PaymentMethods
            .AsNoTracking()
            .OrderBy(method => method.Name)
            .Select(method => new PaymentMethodResponseDto(method.Id, method.Name))
            .ToListAsync(cancellationToken);
        return Ok(methods);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentMethodResponseDto>> Create(
        PaymentMethodRequestDto request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await context.PaymentMethods.AnyAsync(method => method.Name.ToLower() == name.ToLower(), cancellationToken))
        {
            return Conflict(new { message = "Betalingsmetoden findes allerede." });
        }

        var method = new PaymentMethod { Name = name };
        context.PaymentMethods.Add(method);
        await context.SaveChangesAsync(cancellationToken);
        var response = new PaymentMethodResponseDto(method.Id, method.Name);
        return CreatedAtAction(nameof(GetAll), routeValues: null, response);
    }
}
