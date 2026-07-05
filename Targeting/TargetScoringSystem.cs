namespace Targeting;

public class TargetScoringSystem
{
    public IReadOnlyList<float?> ScoreTargets(
        IReadOnlyList<float?> hitChancesByPosition,
        IReadOnlyList<ITargetState?> targetStatesByPosition,
        ITargetingBiases targetingBiases)
    {
        if (hitChancesByPosition.Count != targetStatesByPosition.Count)
        {
            throw new ArgumentException("Hit chances and target states must have the same count.");
        }

        float?[] scores = new float?[hitChancesByPosition.Count];

        for (int i = 0; i < hitChancesByPosition.Count; i++)
        {
            float? hitChance = hitChancesByPosition[i];
            ITargetState? targetState = targetStatesByPosition[i];

            if (!hitChance.HasValue || hitChance.Value <= 0 || targetState is null)
            {
                scores[i] = null;
                continue;
            }

            scores[i] = targetingBiases.ApplyTo(hitChance.Value, targetState);
        }

        return scores;
    }
}
