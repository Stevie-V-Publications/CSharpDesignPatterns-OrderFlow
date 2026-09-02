namespace OrderFlow.Domain;

/// <summary>
/// Plain value type. No pattern logic lives here on purpose — Domain
/// types should read like something a junior dev would write before
/// learning any patterns at all.
/// </summary>
public record Address(
    string Street,
    string City,
    string State,
    string PostalCode);
