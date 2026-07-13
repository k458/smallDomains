using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene.PlayerBaseMenu;

public interface IPlayerBaseMenuResearch : IPlayerBaseMenu
{
    IReadOnlyList<string> Buttons { get; }
}
