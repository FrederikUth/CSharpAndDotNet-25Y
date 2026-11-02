namespace SubTrack.Api.Models;

public sealed class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorCode { get; set; } = "#5b6df5";
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
