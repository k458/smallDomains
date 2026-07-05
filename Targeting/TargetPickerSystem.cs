namespace Targeting;

public class TargetPickerSystem
{
    private readonly Random random;

    public TargetPickerSystem(Random? random = null)
    {
        this.random = random ?? Random.Shared;
    }

    public int? PickTargetIndex(IReadOnlyList<float?> scores)
    {
        double totalScore = 0;

        foreach (float? score in scores)
        {
            if (score > 0)
            {
                totalScore += score.Value;
            }
        }

        if (totalScore == 0)
        {
            return null;
        }

        double roll = random.NextDouble() * totalScore;
        double cumulativeScore = 0;

        for (int i = 0; i < scores.Count; i++)
        {
            float? score = scores[i];

            if (!score.HasValue || score.Value <= 0)
            {
                continue;
            }

            cumulativeScore += score.Value;

            if (roll < cumulativeScore)
            {
                return i;
            }
        }

        return null;
    }
}
