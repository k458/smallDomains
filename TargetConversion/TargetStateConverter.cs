using ArenaPositioning;
using Targeting;

namespace TargetConversion;

public static class TargetStateConverter
{
    public static IReadOnlyList<ITargetState?> ConvertOccupantsToTargetStates(
        IReadOnlyList<IArenaPositionOccupant?> occupants)
    {
        ITargetState?[] targetStates = new ITargetState?[occupants.Count];

        for (int i = 0; i < occupants.Count; i++)
        {
            IArenaPositionOccupant? occupant = occupants[i];
            targetStates[i] = occupant is null ? null : (ITargetState)occupant;
        }

        return targetStates;
    }
}
