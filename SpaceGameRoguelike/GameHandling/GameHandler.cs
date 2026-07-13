using System.Collections.Generic;
using SpaceGameRoguelike.GameHandling.CommandMarker;
using SpaceGameRoguelike.GameScene;
using SpaceGameRoguelike.GameScene.AppMenu;
using SpaceGameRoguelike.GameScene.PlayerBaseMenu;
using TimedRunner;

namespace SpaceGameRoguelike.GameHandling;

public class GameHandler : IGameHandler, ITRunnable
{
    private readonly IAppMenuMain appMenuMainScene = new AppMenuMain(
        new[]
        {
            "Start new game",
        });

    private readonly IPlayerBaseMenuMain playerBaseMenuMainScene = new PlayerBaseMenuMain(
        new[]
        {
            "Research",
            "Embark",
            "Go to main menu",
        });

    private readonly IPlayerBaseMenuResearch playerBaseMenuResearchScene = new PlayerBaseMenuResearch(
        new[]
        {
            "Return to player base",
        });

    private readonly IPlayerBaseMenuEmbark playerBaseMenuEmbarkScene = new PlayerBaseMenuEmbark(
        new[]
        {
            "Return to player base",
        });

    private IGameSceneReadOnly currentScene;

    public GameHandler()
    {
        currentScene = appMenuMainScene;
    }

    public bool IsWaitingForCommand { get; private set; } = true;
    public bool NeedsRedraw { get; private set; } = true;
    public Queue<string> OutputQueue { get; } = new();

    public IGameSceneReadOnly GetCurrentSceneReadOnly()
    {
        return currentScene;
    }

    public bool TryHandle(IGameCommand command)
    {
        return command switch
        {
            IMainMenuCommand mainMenuCommand => TryHandleMainMenuCommand(mainMenuCommand),
            IPlayerBaseCommand playerBaseCommand => TryHandlePlayerBaseCommand(playerBaseCommand),
            _ => false,
        };
    }

    public void Process(float deltaTime)
    {
    }

    public void RunOnce(float deltaTime)
    {
        Process(deltaTime);
    }

    public void ClearNeedsRedraw()
    {
        NeedsRedraw = false;
    }

    private bool TryHandleMainMenuCommand(IMainMenuCommand command)
    {
        if (currentScene is not IAppMenuMain mainMenuScene)
        {
            return false;
        }

        if (!IsValidButtonIndex(command.ButtonIndex, mainMenuScene.Buttons.Count))
        {
            return false;
        }

        switch (command.ButtonIndex)
        {
            case 0:
                StartNewGame();
                return true;
            default:
                return false;
        }
    }

    private bool TryHandlePlayerBaseCommand(IPlayerBaseCommand command)
    {
        return currentScene switch
        {
            IPlayerBaseMenuMain playerBaseMenuMain => TryHandlePlayerBaseMainCommand(command, playerBaseMenuMain),
            IPlayerBaseMenuResearch playerBaseMenuResearch => TryHandlePlayerBaseResearchCommand(command, playerBaseMenuResearch),
            IPlayerBaseMenuEmbark playerBaseMenuEmbark => TryHandlePlayerBaseEmbarkCommand(command, playerBaseMenuEmbark),
            _ => false,
        };
    }

    private bool TryHandlePlayerBaseMainCommand(
        IPlayerBaseCommand command,
        IPlayerBaseMenuMain playerBaseMenuMain)
    {
        if (!IsValidButtonIndex(command.ButtonIndex, playerBaseMenuMain.Buttons.Count))
        {
            return false;
        }

        switch (command.ButtonIndex)
        {
            case 0:
                SwitchToPlayerBaseResearch();
                return true;
            case 1:
                SwitchToPlayerBaseEmbark();
                return true;
            case 2:
                SwitchToAppMenuMain();
                return true;
            default:
                return false;
        }
    }

    private bool TryHandlePlayerBaseResearchCommand(
        IPlayerBaseCommand command,
        IPlayerBaseMenuResearch playerBaseMenuResearch)
    {
        if (!IsValidButtonIndex(command.ButtonIndex, playerBaseMenuResearch.Buttons.Count))
        {
            return false;
        }

        switch (command.ButtonIndex)
        {
            case 0:
                SwitchToPlayerBaseMain();
                return true;
            default:
                return false;
        }
    }

    private bool TryHandlePlayerBaseEmbarkCommand(
        IPlayerBaseCommand command,
        IPlayerBaseMenuEmbark playerBaseMenuEmbark)
    {
        if (!IsValidButtonIndex(command.ButtonIndex, playerBaseMenuEmbark.Buttons.Count))
        {
            return false;
        }

        switch (command.ButtonIndex)
        {
            case 0:
                SwitchToPlayerBaseMain();
                return true;
            default:
                return false;
        }
    }

    private void StartNewGame()
    {
        OutputQueue.Enqueue("new game started");
        SwitchToPlayerBaseMain();
    }

    private void SwitchToAppMenuMain()
    {
        currentScene = appMenuMainScene;
        IsWaitingForCommand = true;
        NeedsRedraw = true;
    }

    private void SwitchToPlayerBaseMain()
    {
        currentScene = playerBaseMenuMainScene;
        IsWaitingForCommand = true;
        NeedsRedraw = true;
    }

    private void SwitchToPlayerBaseResearch()
    {
        currentScene = playerBaseMenuResearchScene;
        IsWaitingForCommand = true;
        NeedsRedraw = true;
    }

    private void SwitchToPlayerBaseEmbark()
    {
        currentScene = playerBaseMenuEmbarkScene;
        IsWaitingForCommand = true;
        NeedsRedraw = true;
    }

    private static bool IsValidButtonIndex(int buttonIndex, int buttonCount)
    {
        return buttonIndex >= 0 && buttonIndex < buttonCount;
    }
}
