using SpaceGameRoguelike.GameScene;

namespace SpaceGameRoguelike.GameHandling;

public interface IGameHandler
{
    IROGameScene GetCurrentSceneReadOnly();

    bool TryHandle(IGameCommand command);
}
