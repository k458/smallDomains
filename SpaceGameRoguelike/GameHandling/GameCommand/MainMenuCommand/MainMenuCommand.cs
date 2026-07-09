using SpaceGameRoguelike.GameHandling.CommandMarker;

namespace SpaceGameRoguelike.GameHandling.GameCommand.MainMenuCommand;

public class MainMenuCommand : IMainMenuCommand
{
    public MainMenuCommand(int buttonIndex)
    {
        ButtonIndex = buttonIndex;
    }

    public int ButtonIndex { get; }
}
