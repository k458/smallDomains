using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene.PlayerBaseMenu;

public class PlayerBaseMenuMain : IPlayerBaseMenuMain
{
    public PlayerBaseMenuMain(IReadOnlyList<string> buttons)
    {
        Buttons = buttons;
    }

    public IReadOnlyList<string> Buttons { get; }
}
