using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene.PlayerBaseMenu;

public class PlayerBaseMenuResearch : IPlayerBaseMenuResearch
{
    public PlayerBaseMenuResearch(IReadOnlyList<string> buttons)
    {
        Buttons = buttons;
    }

    public IReadOnlyList<string> Buttons { get; }
}
