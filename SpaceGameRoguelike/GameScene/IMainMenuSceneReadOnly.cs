using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene;

public interface IMainMenuSceneReadOnly : IGameSceneReadOnly
{
    IReadOnlyList<string> Buttons { get; }
}
