using System.Collections.Generic;
using SpaceGameRoguelike.GameScene;

namespace SpaceGameRoguelike.GameHandling;

public interface IGameHandler
{
    bool IsWaitingForCommand { get; }
    bool NeedsRedraw { get; }
    Queue<string> OutputQueue { get; }

    IGameSceneReadOnly GetCurrentSceneReadOnly();

    bool TryHandle(IGameCommand command);

    void Process(float deltaTime);

    void ClearNeedsRedraw();
}
