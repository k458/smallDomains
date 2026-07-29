# Armor & Armor Penetration Formula

## Formula

```csharp
float effectiveArmor = MathF.Max(0f, armor - armorPen);
float damageMultiplier = 1f / (1f + effectiveArmor * k);
```

`k` is a tuning constant. For example, `0.01f` means 100 effective armor results in 50% damage taken.

## Design Principles

Armor and Armor Penetration are flat additive stats.

Armor Penetration directly subtracts Armor.

Effective Armor cannot go below 0.

Damage reduction uses a smooth diminishing-returns curve.

No percentage modifiers to Armor or Armor Penetration.

## Scaling Philosophy

The formula works best when Armor and Armor Penetration remain in a small, controlled numerical range.

Instead of allowing thousands of armor points and large multipliers:

- Item level upgrades add a small fixed amount, such as +10 Armor or +10 Penetration.
- Perks provide small flat bonuses, such as +1, +2, or +3.
- Temporary effects also use flat values.

This avoids runaway stat inflation and keeps breakpoints meaningful.

## Advantages

- Very easy for players to understand.
- Easy to display in UI: Armor 85, Penetration 72, Effective Armor 13.
- Simple to balance.
- Armor always has diminishing returns.
- Small bonuses remain meaningful.
- Equipment roles are clear: light, medium, anti-armor.

## Tradeoff

The system contains a natural breakpoint:

```text
Armor Penetration >= Armor
```

At that point, all armor is bypassed.

Therefore it is important to keep Armor and Penetration values relatively compact and avoid large percentage scaling or huge level-to-level stat jumps.

With controlled stat growth, this breakpoint becomes a deliberate gameplay feature rather than a balancing problem.
