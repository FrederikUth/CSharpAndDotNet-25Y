namespace Client.Services;

public interface ISubTrackService
{
    Task<IReadOnlyList<SubscriptionItem>> GetSubscriptionsAsync(CancellationToken cancellationToken = default);
    Task<DashboardSummary> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryItem>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentMethodItem>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default);
    Task CreateSubscriptionAsync(SubscriptionDraft subscription, CancellationToken cancellationToken = default);
    Task UpdateSubscriptionAsync(SubscriptionDraft subscription, CancellationToken cancellationToken = default);
    Task DeleteSubscriptionAsync(int id, CancellationToken cancellationToken = default);
    Task CreateCategoryAsync(CategoryDraft category, CancellationToken cancellationToken = default);
    Task CreatePaymentMethodAsync(PaymentMethodDraft paymentMethod, CancellationToken cancellationToken = default);
}
