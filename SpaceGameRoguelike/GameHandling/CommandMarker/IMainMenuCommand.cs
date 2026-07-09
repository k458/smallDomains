namespace SpaceGameRoguelike.GameHandling.CommandMarker;

public interface IMainMenuCommand : IGameCommand
{
    int ButtonIndex { get; }
}
