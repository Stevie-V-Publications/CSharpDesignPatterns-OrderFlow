namespace OrderFlow.Domain;

public class Restaurant
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required Address Address { get; init; }
}
