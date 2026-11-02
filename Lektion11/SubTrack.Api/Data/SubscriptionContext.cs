using Microsoft.EntityFrameworkCore;
using SubTrack.Api.Models;

namespace SubTrack.Api.Data;

public sealed class SubscriptionContext(DbContextOptions<SubscriptionContext> options) : DbContext(options)
{
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.Property(subscription => subscription.Name).HasMaxLength(120).IsRequired();
            entity.Property(subscription => subscription.Price).HasPrecision(10, 2);
            entity.Property(subscription => subscription.Currency).HasMaxLength(3);
            entity.Property(subscription => subscription.BillingCycle).HasConversion<string>().HasMaxLength(16);
            entity.Property(subscription => subscription.Status).HasConversion<string>().HasMaxLength(16);
            entity.HasOne(subscription => subscription.Category)
                .WithMany(category => category.Subscriptions)
                .HasForeignKey(subscription => subscription.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(subscription => subscription.PaymentMethod)
                .WithMany(method => method.Subscriptions)
                .HasForeignKey(subscription => subscription.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(category => category.Name).HasMaxLength(80).IsRequired();
            entity.Property(category => category.ColorCode).HasMaxLength(7).IsRequired();
            entity.HasIndex(category => category.Name).IsUnique();
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.Property(method => method.Name).HasMaxLength(80).IsRequired();
            entity.HasIndex(method => method.Name).IsUnique();
        });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Underholdning", ColorCode = "#7868e6" },
            new Category { Id = 2, Name = "Fitness", ColorCode = "#30a46c" },
            new Category { Id = 3, Name = "Produktivitet", ColorCode = "#e59b2f" });
        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod { Id = 1, Name = "Visa/Dankort" },
            new PaymentMethod { Id = 2, Name = "MobilePay" },
            new PaymentMethod { Id = 3, Name = "PayPal" });
        modelBuilder.Entity<Subscription>().HasData(
            new Subscription
            {
                Id = 1,
                Name = "Streamio",
                Price = 129,
                Currency = "DKK",
                BillingCycle = BillingCycle.Monthly,
                StartDate = new DateOnly(2026, 1, 1),
                NextBillingDate = new DateOnly(2026, 11, 6),
                Status = SubscriptionStatus.Active,
                CategoryId = 1,
                PaymentMethodId = 1
            },
            new Subscription
            {
                Id = 2,
                Name = "FitClub",
                Price = 299,
                Currency = "DKK",
                BillingCycle = BillingCycle.Monthly,
                StartDate = new DateOnly(2026, 2, 1),
                NextBillingDate = new DateOnly(2026, 11, 18),
                Status = SubscriptionStatus.Active,
                CategoryId = 2,
                PaymentMethodId = 2
            },
            new Subscription
            {
                Id = 3,
                Name = "CloudNotes",
                Price = 899,
                Currency = "DKK",
                BillingCycle = BillingCycle.Yearly,
                StartDate = new DateOnly(2026, 3, 1),
                NextBillingDate = new DateOnly(2027, 3, 1),
                Status = SubscriptionStatus.Active,
                CategoryId = 3,
                PaymentMethodId = 3
            });
    }
}
