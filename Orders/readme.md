# Orders

Owns a small queue/orchestration model for orders.

## Main Types

- `Order` stores an `Id`, cost values, compound counter, and whether it can compound.
- `OrderOrchestrator` keeps the active order list and handles pushes/removal/clearing.

## Compounding

When `PushOrder` receives a new order, the orchestrator looks for an existing order with the same `Id` that can compound.

If found, the existing order is modified instead of adding the new one:

- `CompoundCounter` increases.
- `BasicCost` increases by `CompoundCost`.

If no compatible order exists, the new order is added to the list.

`Order.Reset` restores the original basic cost and clears the compound counter so lists can be rebuilt from clean order instances.

## Boundary

This system does not decide which orders are legal or execute them. It only stores and compounds order data.
