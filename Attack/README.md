# Attack

This folder contains stat contracts and services for resolving weapon attacks.

## Stats

`IAttackerStats` supplies the attacker's `Accuracy`.

`IDefenderStats` supplies the target's `Dodge` and current `Hp`. The properties
are read-only because these services calculate results; the owning gameplay
object is responsible for applying them.

## Hit rolls

`HitRollCalculator.RollHit` calculates the base hit chance as:

```text
max(Accuracy - Dodge, 10)%
```

It multiplies that chance by the range multiplier interpolated from
`WeaponRangeConfiguration`, then compares it with a random float from `0` to
`1`. A target beyond `maxRange` is always missed.

The method returns `HitRollResult`, whose `IsHit` field reports the outcome.

## Damage

`WeaponDamageConfiguration` stores base `damage` and `penetration` values.

`WeaponDamageCalculator.CalculateDamage` reports the calculated damage through
`DamageCalculationResult.damageDealt`. Damage is currently limited to the
defender's remaining HP and does not mutate the defender.

`distance` and `penetration` are accepted by the damage API but reserved for
the distance and armor rules that still need to be defined.
