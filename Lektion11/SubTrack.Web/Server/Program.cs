using Microsoft.AspNetCore.Components.WebAssembly.Server;
using SubTrack.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.MapWhen(context => context.Request.Path.StartsWithSegments("/api"), api =>
{
    api.Run(async context =>
    {
        var apiAddress = context.RequestServices.GetRequiredService<IConfiguration>()["services:apiservice:http:0"]
            ?? context.RequestServices.GetRequiredService<IConfiguration>()["Api:BaseUrl"];
        if (!Uri.TryCreate(apiAddress, UriKind.Absolute, out var baseAddress))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "API-adressen er ikke konfigureret. Start appen via Aspire AppHost."
            }, context.RequestAborted);
            return;
        }

        var target = new Uri(baseAddress, context.Request.Path + context.Request.QueryString);
        using var request = new HttpRequestMessage(new HttpMethod(context.Request.Method), target);
        if (context.Request.ContentLength is > 0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))
        {
            request.Content = new StreamContent(context.Request.Body);
        }

        foreach (var header in context.Request.Headers)
        {
            if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase)
                || header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)
                || header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            {
                request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            context.RequestAborted);
        context.Response.StatusCode = (int)response.StatusCode;

        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in response.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        context.Response.Headers.Remove("transfer-encoding");
        await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    });
});

app.MapFallbackToFile("index.html");
app.Run();
