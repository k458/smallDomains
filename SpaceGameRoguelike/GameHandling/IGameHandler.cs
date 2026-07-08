using SpaceGameRoguelike.GameScene;

namespace SpaceGameRoguelike.GameHandling;

public interface IGameHandler
{
    bool IsWaitingForCommand { get; }
    bool NeedsRedraw { get; }

    IGameSceneReadOnly GetCurrentSceneReadOnly();

    bool TryHandle(IGameCommand command);

    void Process(float deltaTime);
}
