using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene.AppMenu;

public class AppMenuMain : IAppMenuMain
{
    public AppMenuMain(IReadOnlyList<string> buttons)
    {
        Buttons = buttons;
    }

    public IReadOnlyList<string> Buttons { get; }
}
