using Microsoft.EntityFrameworkCore;
using Npgsql;
using SubTrack.Api.Data;
using SubTrack.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("appdb")
    ?? throw new InvalidOperationException("The 'appdb' PostgreSQL connection string is required.");

builder.AddServiceDefaults();
builder.Services.AddDbContext<SubscriptionContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
    for (var attempt = 1; ; attempt++)
    {
        try
        {
            await context.Database.MigrateAsync();
            break;
        }
        catch (NpgsqlException exception) when (attempt < 30)
        {
            app.Logger.LogWarning(exception, "PostgreSQL is not ready yet; retrying migration ({Attempt}/30).", attempt);
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

app.Run();
