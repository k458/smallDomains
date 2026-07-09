using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene;

public class MainMenuScene : IMainMenuSceneReadOnly
{
    public MainMenuScene(IReadOnlyList<string> buttons)
    {
        Buttons = buttons;
    }

    public IReadOnlyList<string> Buttons { get; }
}
