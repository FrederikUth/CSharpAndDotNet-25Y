namespace Opgave02.model;

public record Order(int Id, DateTime OrderDate, List<OrderItem> Items);