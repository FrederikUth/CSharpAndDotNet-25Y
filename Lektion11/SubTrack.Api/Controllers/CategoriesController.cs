using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubTrack.Api.Data;
using SubTrack.Api.Dto;
using SubTrack.Api.Models;

namespace SubTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController(SubscriptionContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponseDto(category.Id, category.Name, category.ColorCode))
            .ToListAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Create(
        CategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await context.Categories.AnyAsync(category => category.Name.ToLower() == name.ToLower(), cancellationToken))
        {
            return Conflict(new { message = "Kategorien findes allerede." });
        }

        var category = new Category { Name = name, ColorCode = request.ColorCode };
        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);
        var response = new CategoryResponseDto(category.Id, category.Name, category.ColorCode);
        return CreatedAtAction(nameof(GetAll), routeValues: null, response);
    }
}
