using System;

namespace NodeMapTrident;

public static class TridentLaneRules
{
    public const int MinY = 0;
    public const int CentralY = 1;
    public const int MaxY = 2;
    public const int MaxNodesPerDepth = 3;
    public const int MaxForwardDeltaY = 1;
    public const int MaxOutgoingConnections = 3;

    public static TridentLane GetLane(int y)
    {
        ValidateY(y);
        return (TridentLane)y;
    }

    public static TridentLaneModifiers GetLaneModifiers(int y)
    {
        return GetLane(y) switch
        {
            TridentLane.Upper => new TridentLaneModifiers(
                rareEncounterMultiplier: 1.2,
                rewardVarianceMultiplier: 1.1,
                dangerMultiplier: 1.1),
            TridentLane.Central => new TridentLaneModifiers(
                rareEncounterMultiplier: 0.6,
                rewardVarianceMultiplier: 0.75,
                dangerMultiplier: 0.9),
            TridentLane.Lower => new TridentLaneModifiers(
                rareEncounterMultiplier: 1.4,
                rewardVarianceMultiplier: 1.25,
                dangerMultiplier: 1.15),
            _ => throw new ArgumentOutOfRangeException(nameof(y))
        };
    }

    public static bool CanConnect(
        int currentY,
        int nextY)
    {
        ValidateY(currentY);
        ValidateY(nextY);
        return Math.Abs(currentY - nextY) <= MaxForwardDeltaY;
    }

    public static void ValidateY(int y)
    {
        if (y < MinY || y > MaxY)
        {
            throw new ArgumentOutOfRangeException(
                nameof(y),
                y,
                "Trident node Y must be 0, 1, or 2.");
        }
    }
}
