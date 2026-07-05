# Damage

Owns damage type, resistance, calculation, and the boundary for applying already-resolved damage.

## Main Types

- `DamageType` lists `Physical`, `Cold`, `Fire`, and `Lightning`.
- `DamageResistances` stores resistance values for each damage type. Defaults are `100`.
- `DamageRequest` describes a damage attempt.
- `DamageResult` reports the calculation result.
- `IDamageable` exposes resistances and receives resolved damage.
- `DamageSystem` calculates and applies damage.

## Resistance Formula

Resistance is scaled around `100`:

```csharp
finalDamage = amount * 100 / resistance;
```

Examples:

- `100` resistance means full damage.
- `200` resistance means half damage.
- `50` resistance means double damage.

`DamageSystem` calculates final damage first, then calls `ApplyResolvedDamage` on the target. The target should treat that amount as already post-resistance.

## Boundary

This system should not play hit animations, spawn damage numbers, remove scene nodes, or advance turns. Those belong to scene/gameplay coordination code.
