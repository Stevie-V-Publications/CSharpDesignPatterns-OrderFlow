using Microsoft.EntityFrameworkCore;
using OrderFlow.Components;
using OrderFlow.Data;
using OrderFlow.Patterns.Builder;
using OrderFlow.Patterns.FactoryMethod;
using OrderFlow.Patterns.Singleton;
using OrderFlow.Patterns.Adapter;
using OrderFlow.Patterns.Adapter.ThirdParty;
using OrderFlow.Patterns.Facade;
using OrderFlow.Patterns.Observer;
using OrderFlow.Patterns.Strategy;
using OrderFlow.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server (Razor Components with interactive server render mode).
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- Persistence -----------------------------------------------------
// SQLite via EF Core. Kept intentionally simple: this app's job is to
// teach design patterns, not showcase advanced EF Core usage.
builder.Services.AddDbContextFactory<OrderFlowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("OrderFlowDb")
        ?? "Data Source=orderflow.db"));

// The only service that reads/writes the database: records an order
// history row after each successful checkout and reads it back for the
// Admin Dashboard.
builder.Services.AddScoped<OrderHistoryService>();

// UI-only role switcher (not real auth). Scoped: one active persona per
// connected circuit, shared by the navbar and every page.
builder.Services.AddScoped<PersonaContext>();

// --- Pattern services --------------------------------------------------
// Each pattern's services are registered here, grouped by pattern number
// so a reader can see at a glance which DI lifetime the pattern needs and
// why. Builder is intentionally NOT registered as a singleton/scoped
// service — a new OrderBuilder is created per order, which is the whole
// point of the pattern (see Patterns/01-Builder/OrderBuilder.cs).
builder.Services.AddTransient<OrderBuilder>();
builder.Services.AddTransient<IOrderBuilder, OrderBuilder>();
// The Director depends on IOrderBuilder and is itself Transient so each
// "reorder my usual" gets a fresh builder (see OrderDirector.cs).
builder.Services.AddTransient<OrderDirector>();

// Factory Method (Pattern 2). The factory itself is stateless, so one shared
// instance is fine; the INotifier instances it produces are cheap and
// created per call.
builder.Services.AddSingleton<NotificationFactory>();

// Singleton (Pattern 3). The DI container can hold one instance for us, but
// the pattern still owns the guarantee: we hand it the same object its
// static Instance property exposes, so code that reaches it either way
// (injected, or KitchenDisplayService.Instance) shares one queue set.
builder.Services.AddSingleton(_ => KitchenDisplayService.Instance);

// Adapter (Pattern 4). The three "SDK" clients are third-party stand-ins;
// each adapter wraps one and exposes the common IPaymentProcessor. All
// three are registered against the interface, so a consumer can inject
// IEnumerable<IPaymentProcessor> and pick by ProviderName.
builder.Services.AddSingleton<StripeLikeClient>();
builder.Services.AddSingleton<SquareLikeClient>();
builder.Services.AddSingleton<PayPalLikeClient>();
builder.Services.AddSingleton<IPaymentProcessor, StripePaymentAdapter>();
builder.Services.AddSingleton<IPaymentProcessor, SquarePaymentAdapter>();
builder.Services.AddSingleton<IPaymentProcessor, PayPalPaymentAdapter>();

// Facade (Pattern 6). CheckoutFacade coordinates inventory + payment +
// kitchen + notifications behind one PlaceOrder call. It owns no state,
// so its lifetime is only about the services it pulls in — Scoped is
// fine here.
builder.Services.AddSingleton<IInventoryService, InMemoryInventoryService>();
builder.Services.AddScoped<CheckoutFacade>();

// Observer (Pattern 8). One shared publisher for the whole process so a
// status change raised anywhere reaches every subscribed screen. The
// subject must outlive any individual observer (a Blazor circuit
// subscribes on init and disposes its subscription on teardown), so it
// is a Singleton.
builder.Services.AddSingleton<OrderStatusPublisher>();

// Strategy (Pattern 10). Each delivery-pricing rule is stateless, so one
// shared instance each is fine. Registering all three against the
// interface lets a consumer inject IEnumerable<IDeliveryPricingStrategy>
// and pick by Name — or a settings screen could bind the active one.
builder.Services.AddSingleton<IDeliveryPricingStrategy, FlatRateStrategy>();
builder.Services.AddSingleton<IDeliveryPricingStrategy, DistanceBasedStrategy>();
builder.Services.AddSingleton<IDeliveryPricingStrategy, SurgeStrategy>();

var app = builder.Build();

// Create the SQLite schema and seed reference data before serving.
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<OrderFlowDbContext>>();
    using var db = contextFactory.CreateDbContext();
    OrderFlowDbInitializer.Initialize(db);
}

// --- HTTP pipeline -----------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
