using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene.PlayerBaseMenu;

public interface IPlayerBaseMenuMain : IPlayerBaseMenu
{
    IReadOnlyList<string> Buttons { get; }
}
