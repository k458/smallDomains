using SpaceGameRoguelike.GameScene;

namespace SpaceGameRoguelike.GameHandling;

public interface IGameHandler
{
    IGameSceneReadOnly GetCurrentSceneReadOnly();

    bool TryHandle(IGameCommand command);

    void Process(float deltaTime);
}
