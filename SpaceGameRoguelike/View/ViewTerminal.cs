using System;
using System.Diagnostics;
using System.Threading;
using SpaceGameRoguelike.GameHandling;
using SpaceGameRoguelike.GameHandling.GameCommand.MainMenuCommand;
using SpaceGameRoguelike.GameScene;
using TimedRunner;

namespace SpaceGameRoguelike.View;

public class ViewTerminal
{
    private IGameSceneReadOnly? currentScene;

    public bool IsRunning { get; private set; }

    public void Run(IGameHandler gameHandler, ITimedRunner timedRunner)
    {
        IsRunning = true;
        Stopwatch stopwatch = Stopwatch.StartNew();

        while (IsRunning)
        {
            float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();

            timedRunner.RunOnce(deltaTime);

            if (gameHandler.NeedsRedraw)
            {
                currentScene = gameHandler.GetCurrentSceneReadOnly();
                Redraw(gameHandler);
                gameHandler.ClearNeedsRedraw();
            }

            if (gameHandler.IsWaitingForCommand)
            {
                Console.Write("input: ");
                string? input = Console.ReadLine();
                Console.WriteLine(input);

                if (TryParseInput(input, out int parsedInput))
                {
                    currentScene ??= gameHandler.GetCurrentSceneReadOnly();
                    IGameCommand? command = BuildCommand(parsedInput);

                    if (command is not null)
                    {
                        gameHandler.TryHandle(command);
                    }
                }
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    public void Stop()
    {
        IsRunning = false;
    }

    private static bool TryParseInput(string? input, out int buttonIndex)
    {
        buttonIndex = -1;

        if (!int.TryParse(input, out int selectedNumber))
        {
            return false;
        }

        if (selectedNumber <= 0)
        {
            return false;
        }

        buttonIndex = selectedNumber - 1;
        return true;
    }

    private IGameCommand? BuildCommand(int buttonIndex)
    {
        return currentScene switch
        {
            IMainMenuSceneReadOnly => new MainMenuCommand(buttonIndex),
            _ => null,
        };
    }

    private void Redraw(IGameHandler gameHandler)
    {
        while (gameHandler.OutputQueue.TryDequeue(out string? output))
        {
            Console.WriteLine(output);
        }

        switch (currentScene)
        {
            case IMainMenuSceneReadOnly mainMenuScene:
                RedrawMainMenu(mainMenuScene);
                break;
        }
    }

    private void RedrawMainMenu(IMainMenuSceneReadOnly mainMenuScene)
    {
        Console.WriteLine("main menu");

        for (int i = 0; i < mainMenuScene.Buttons.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {mainMenuScene.Buttons[i]}");
        }
    }
}


