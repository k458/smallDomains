using System.Collections.Generic;
using SpaceGameRoguelike.GameHandling.CommandMarker;
using SpaceGameRoguelike.GameScene;
using TimedRunner;

namespace SpaceGameRoguelike.GameHandling;

public class GameHandler : IGameHandler, ITRunnable
{
    private IGameSceneReadOnly currentScene = new MainMenuScene(
        new[]
        {
            "Start new game",
        });

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
        if (currentScene is not IMainMenuSceneReadOnly mainMenuScene)
        {
            return false;
        }

        if (command.ButtonIndex < 0 || command.ButtonIndex >= mainMenuScene.Buttons.Count)
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

    private void StartNewGame()
    {
        OutputQueue.Enqueue("new game started");
        IsWaitingForCommand = true;
        NeedsRedraw = true;
    }
}
