using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene.PlayerBaseMenu;

public interface IPlayerBaseMenuEmbark : IPlayerBaseMenu
{
    IReadOnlyList<string> Buttons { get; }
}
