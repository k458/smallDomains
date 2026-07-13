using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene.PlayerBaseMenu;

public class PlayerBaseMenuEmbark : IPlayerBaseMenuEmbark
{
    public PlayerBaseMenuEmbark(IReadOnlyList<string> buttons)
    {
        Buttons = buttons;
    }

    public IReadOnlyList<string> Buttons { get; }
}
