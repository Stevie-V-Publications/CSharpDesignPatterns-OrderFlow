# OrderFlow

A multi-restaurant food delivery & kitchen ops platform, built to showcase
classic C# design patterns inside one real Blazor Server application —
companion codebase to the book of the same name.

The goal: no toy demos. Clone the repo, run the app, click through real
features (customer ordering, a live kitchen display, a dispatch board, an
admin dashboard), then open the matching `Patterns/NN-*` folder to see
exactly how that feature is built.

## Status

**~99.9% complete.** All 10 primary patterns plus both bench patterns are
implemented, each visible in two places: an isolated demo in the Pattern
Playground **and** wired into a real app flow. Every pattern has a full
book chapter, and `book/build-book.sh --check` passes clean.

| # | Pattern | Category | Powers | Real-app surface |
|---|---------|----------|--------|------------------|
| 1 | Builder | Creational | Step-by-step order assembly | `/order` |
| 2 | Factory Method | Creational | Per-customer notification channel (SMS/Email/Push) | checkout |
| 3 | Singleton | Creational | One shared in-memory kitchen queue per restaurant | `/kitchen` |
| 4 | Adapter | Structural | Unified payment interface over Stripe/Square/PayPal-style SDKs | `/admin` |
| 5 | Decorator | Structural | Stackable toppings/add-ons that adjust price & description | `/order` |
| 6 | Facade | Structural | `PlaceOrder()` hiding inventory + payment + kitchen routing + notifications | `/order`, `/admin` |
| 7 | Composite | Structural | Menu categories → items → combo meals | `/order` |
| 8 | Observer | Behavioral | Live order-status push to Kitchen, Customer Tracker, Dispatch | `/kitchen`, `/track`, `/dispatch` |
| 9 | State | Behavioral | Order lifecycle: Placed → Confirmed → Preparing → Ready → OutForDelivery → Completed | `/kitchen`, `/track` |
| 10 | Strategy | Behavioral | Swappable delivery pricing (flat / distance-based / surge) | `/admin` |
| 11 | Command *(bonus)* | Behavioral | Order adjust/cancel with undo + audit trail | `/admin` |
| 12 | Chain of Responsibility *(bonus)* | Behavioral | Fraud → inventory → payment-auth checkout pipeline | `/admin` |

Remaining 0.1%: cover art, final proofread pass on the manuscript, and
KDP export polish.

## Running locally

Requires the **.NET 10 SDK**.

```bash
cd src/OrderFlow
dotnet restore
dotnet run
```

Then open the URL printed in the console. Run the pattern test suite with:

```bash
dotnet test
```

`test/OrderFlow.Tests` has one file per pattern; each chapter's *Now It's
Testable* section is quoted straight from it.

- **Pattern Playground** (`/patterns`) — each pattern demoed in isolation, matching its book chapter.
- **Order** (`/order`) — 3-step customer flow (menu → review → confirm): Composite, Decorator, Builder, Facade.
- **Track** (`/track`) — customer-facing live order tracker: Observer + State.
- **Kitchen** (`/kitchen`) — kitchen display: Singleton, State, Observer. Orders placed at `/order` appear here live.
- **Dispatch** (`/dispatch`, Admin) — driver assignment board: the third Observer surface.
- **Admin** (`/admin`) — Strategy, Adapter, Facade, Chain of Responsibility, Command, plus order history.

A **persona switcher** (Customer / Kitchen Staff / Admin) in the navbar is
real — it gates routes and nav links. It's not authentication; it exists
only where it makes a pattern's UI clearer.

## Architecture notes

- `/Domain` — plain POCOs, zero pattern logic (reads like code written before learning patterns).
- `/Patterns/NN-*` — one folder per pattern, numbered 1:1 with book chapters. Namespaces drop the number (`OrderFlow.Patterns.Builder`).
- `/Components` — Blazor UI; patterns get composed here.
- EF Core + SQLite persistence is real but deliberately minimal — `OrderHistoryService` writes a record after each checkout; no pattern class touches EF. `*.db` is gitignored.
- Cross-pattern wiring: `CheckoutFacade` orchestrates Builder, Adapter, Singleton, Factory Method, and drives the Placed → Confirmed transition through the State machine.

Build verification while the app is running (the running exe locks `bin/`):

```bash
dotnet build -p:UseAppHost=false -o <scratch-dir>
```



