namespace OrderFlow.Services;

/// <summary>
/// The three personas OrderFlow can be "logged in as". Deliberately not a
/// real identity system (see CLAUDE.md) — just a UI role switcher that
/// decides which surfaces a persona sees and gives patterns like Strategy
/// and Command a natural "acting as Admin" context.
/// </summary>
public enum Persona
{
    Customer,
    KitchenStaff,
    Admin,
}

/// <summary>
/// Holds the active <see cref="Persona"/> for one Blazor circuit. Scoped
/// (one per connected user), so a switch in the navbar is seen by the nav
/// menu and every page on the same circuit. Components subscribe to
/// <see cref="Changed"/> and re-render.
/// </summary>
public sealed class PersonaContext
{
    private Persona _current = Persona.Customer;

    public Persona Current
    {
        get => _current;
        set
        {
            if (_current == value) return;
            _current = value;
            Changed?.Invoke();
        }
    }

    public event Action? Changed;

    public string DisplayName => Current switch
    {
        Persona.KitchenStaff => "Kitchen Staff",
        _ => Current.ToString(),
    };

    /// <summary>Which personas are allowed to see a given route path.</summary>
    public static bool CanAccess(Persona persona, string path) => persona switch
    {
        Persona.Admin => true,
        Persona.Customer => path is "" or "patterns" or "order" or "track",
        Persona.KitchenStaff => path is "" or "patterns" or "kitchen",
        _ => false,
    };
}
