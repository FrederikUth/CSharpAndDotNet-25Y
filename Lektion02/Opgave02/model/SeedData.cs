namespace Opgave02.model;

public static class SeedData
{
    public static readonly Product Laptop = new(1, "Laptop", Category.Elektronik, 7999m, 5);
    public static readonly Product Mus = new(2, "Mus", Category.Elektronik, 299m, 25);
    public static readonly Product Kaffemaskine = new(3, "Kaffemaskine", Category.Husholdning, 499m, 0);
    public static readonly Product Skrivebord = new(4, "Skrivebord", Category.Mobler, 1299m, 8);
    public static readonly Product Kontorstol = new(5, "Kontorstol", Category.Mobler, 899m, 3);
    public static readonly Product Skaerm = new(6, "Skærm 27\"", Category.Elektronik, 1999m, 12);
    public static readonly Product Vandkoger = new(7, "Vandkoger", Category.Husholdning, 199m, 15);

    public static List<Product> Products = new()
    {
        Laptop,
        Mus,
        Kaffemaskine,
        Skrivebord,
        Kontorstol,
        Skaerm,
        Vandkoger
    };

    public static List<Customer> Customers = new()
    {
        new(1, "Anna Hansen", "Aarhus", new() {
            new(101, DateTime.Now.AddDays(-10), new() { new(Mus, 2), new(Laptop, 1) })
        }),
        new(2, "Bo Jensen", "København", new() {
            new(102, DateTime.Now.AddDays(-30), new() { new(Kontorstol, 1) }),
            new(103, DateTime.Now.AddDays(-2), new() { new(Skaerm, 2) })
        }),
        new(3, "Clara Møller", "Aarhus", new()
        {
            new Order(105, DateTime.Now.AddDays(-7), new List<OrderItem>()
            {
                new(Kontorstol, 1), new(Mus, 2)
            })
        }),
        new(4, "David Olsen", "Odense", new() {
            new(104, DateTime.Now.AddDays(-5), new() { new(Vandkoger, 1) })
        })
    };
}
