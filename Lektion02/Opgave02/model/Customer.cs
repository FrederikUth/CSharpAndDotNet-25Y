namespace Opgave02.model;

public record Customer(int Id, string Name, string City, List<Order> Orders);