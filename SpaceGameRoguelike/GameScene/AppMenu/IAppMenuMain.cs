using System.Collections.Generic;

namespace SpaceGameRoguelike.GameScene.AppMenu;

public interface IAppMenuMain : IAppMenu
{
    IReadOnlyList<string> Buttons { get; }
}
