using System;

namespace NodeMapTrident;

public readonly struct TridentConnectionWeights
{
    public TridentConnectionWeights(
        int oneConnectionWeight,
        int twoConnectionWeight,
        int threeConnectionWeight)
    {
        if (oneConnectionWeight < 0
            || twoConnectionWeight < 0
            || threeConnectionWeight < 0
            || oneConnectionWeight + twoConnectionWeight + threeConnectionWeight <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(oneConnectionWeight),
                "At least one connection weight must be positive, and weights cannot be negative.");
        }

        OneConnectionWeight = oneConnectionWeight;
        TwoConnectionWeight = twoConnectionWeight;
        ThreeConnectionWeight = threeConnectionWeight;
    }

    public int OneConnectionWeight { get; }
    public int TwoConnectionWeight { get; }
    public int ThreeConnectionWeight { get; }

    public static TridentConnectionWeights Default { get; } = new(
        oneConnectionWeight: 70,
        twoConnectionWeight: 25,
        threeConnectionWeight: 5);
}
