using System.Net.Http.Json;

namespace Client.Services;

public sealed class SubTrackService(HttpClient httpClient) : ISubTrackService
{
    public async Task<IReadOnlyList<SubscriptionItem>> GetSubscriptionsAsync(CancellationToken cancellationToken = default) =>
        await GetRequiredAsync<List<SubscriptionItem>>("api/subscriptions", cancellationToken);

    public async Task<DashboardSummary> GetDashboardAsync(CancellationToken cancellationToken = default) =>
        await GetRequiredAsync<DashboardSummary>("api/dashboard", cancellationToken);

    public async Task<IReadOnlyList<CategoryItem>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        await GetRequiredAsync<List<CategoryItem>>("api/categories", cancellationToken);

    public async Task<IReadOnlyList<PaymentMethodItem>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default) =>
        await GetRequiredAsync<List<PaymentMethodItem>>("api/paymentmethods", cancellationToken);

    public async Task CreateSubscriptionAsync(SubscriptionDraft subscription, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/subscriptions", subscription, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task UpdateSubscriptionAsync(SubscriptionDraft subscription, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"api/subscriptions/{subscription.Id}",
            subscription,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task DeleteSubscriptionAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/subscriptions/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task CreateCategoryAsync(CategoryDraft category, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/categories", category, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task CreatePaymentMethodAsync(PaymentMethodDraft paymentMethod, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/paymentmethods", paymentMethod, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private async Task<T> GetRequiredAsync<T>(string uri, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(uri, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"API'et returnerede ikke data for {uri}.");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var detail = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException(
            string.IsNullOrWhiteSpace(detail) ? $"API-fejl: {(int)response.StatusCode} {response.ReasonPhrase}" : detail,
            inner: null,
            response.StatusCode);
    }
}
