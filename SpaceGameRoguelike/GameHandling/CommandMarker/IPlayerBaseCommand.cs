namespace SpaceGameRoguelike.GameHandling.CommandMarker;

public interface IPlayerBaseCommand : IGameCommand
{
    int ButtonIndex { get; }
}
