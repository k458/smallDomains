using SpaceGameRoguelike.GameHandling.CommandMarker;

namespace SpaceGameRoguelike.GameHandling.GameCommand.PlayerBaseMenuCommand;

public class PlayerBaseMenuCommand : IPlayerBaseCommand
{
    public PlayerBaseMenuCommand(int buttonIndex)
    {
        ButtonIndex = buttonIndex;
    }

    public int ButtonIndex { get; }
}
