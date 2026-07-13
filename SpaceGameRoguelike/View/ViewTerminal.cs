using System;
using System.Diagnostics;
using System.Threading;
using SpaceGameRoguelike.GameHandling;
using SpaceGameRoguelike.GameHandling.GameCommand.MainMenuCommand;
using SpaceGameRoguelike.GameHandling.GameCommand.PlayerBaseMenuCommand;
using SpaceGameRoguelike.GameScene;
using SpaceGameRoguelike.GameScene.AppMenu;
using SpaceGameRoguelike.GameScene.PlayerBaseMenu;
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
                IGameSceneReadOnly scene = gameHandler.GetCurrentSceneReadOnly();
                currentScene = scene;
                PrintQueue(gameHandler);
                RedrawScene(scene);
                gameHandler.ClearNeedsRedraw();
            }

            if (gameHandler.IsWaitingForCommand)
            {
                Console.Write("input: ");
                string? input = Console.ReadLine();
                Console.WriteLine(input);

                currentScene ??= gameHandler.GetCurrentSceneReadOnly();

                if (TryBuildCommand(input, currentScene, out IGameCommand? command) && command is not null)
                {
                    gameHandler.TryHandle(command);
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

    private static bool TryBuildCommand(
        string? input,
        IGameSceneReadOnly scene,
        out IGameCommand? command)
    {
        command = null;

        if (!TryParseInput(input, out int buttonIndex))
        {
            return false;
        }

        return scene switch
        {
            IAppMenu appMenu => TryBuildAppMenuCommand(buttonIndex, appMenu, out command),
            IPlayerBaseMenu playerBaseMenu => TryBuildPlayerBaseMenuCommand(buttonIndex, playerBaseMenu, out command),
            _ => false,
        };
    }

    private static bool TryBuildAppMenuCommand(
        int buttonIndex,
        IAppMenu appMenu,
        out IGameCommand? command)
    {
        command = appMenu switch
        {
            IAppMenuMain => new MainMenuCommand(buttonIndex),
            _ => null,
        };

        return command is not null;
    }

    private static bool TryBuildPlayerBaseMenuCommand(
        int buttonIndex,
        IPlayerBaseMenu playerBaseMenu,
        out IGameCommand? command)
    {
        command = playerBaseMenu switch
        {
            IPlayerBaseMenuMain => new PlayerBaseMenuCommand(buttonIndex),
            IPlayerBaseMenuResearch => new PlayerBaseMenuCommand(buttonIndex),
            IPlayerBaseMenuEmbark => new PlayerBaseMenuCommand(buttonIndex),
            _ => null,
        };

        return command is not null;
    }

    private static void PrintQueue(IGameHandler gameHandler)
    {
        while (gameHandler.OutputQueue.TryDequeue(out string? output))
        {
            Console.WriteLine(output);
        }
    }

    private void RedrawScene(IGameSceneReadOnly scene)
    {
        switch (scene)
        {
            case IAppMenu appMenu:
                RedrawAppMenu(appMenu);
                break;
            case IPlayerBaseMenu playerBaseMenu:
                RedrawPlayerBaseMenu(playerBaseMenu);
                break;
        }
    }

    private void RedrawAppMenu(IAppMenu appMenu)
    {
        switch (appMenu)
        {
            case IAppMenuMain appMenuMain:
                RedrawAppMenuMain(appMenuMain);
                break;
        }
    }

    private void RedrawPlayerBaseMenu(IPlayerBaseMenu playerBaseMenu)
    {
        switch (playerBaseMenu)
        {
            case IPlayerBaseMenuMain playerBaseMenuMain:
                RedrawPlayerBaseMenuMain(playerBaseMenuMain);
                break;
            case IPlayerBaseMenuResearch playerBaseMenuResearch:
                RedrawPlayerBaseMenuResearch(playerBaseMenuResearch);
                break;
            case IPlayerBaseMenuEmbark playerBaseMenuEmbark:
                RedrawPlayerBaseMenuEmbark(playerBaseMenuEmbark);
                break;
        }
    }

    private void RedrawAppMenuMain(IAppMenuMain appMenuMain)
    {
        Console.WriteLine();
        Console.WriteLine("MAIN MENU");

        for (int i = 0; i < appMenuMain.Buttons.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {appMenuMain.Buttons[i]}");
        }
    }

    private void RedrawPlayerBaseMenuMain(IPlayerBaseMenuMain playerBaseMenuMain)
    {
        Console.WriteLine();
        Console.WriteLine("PLAYER BASE");

        for (int i = 0; i < playerBaseMenuMain.Buttons.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {playerBaseMenuMain.Buttons[i]}");
        }
    }

    private void RedrawPlayerBaseMenuResearch(IPlayerBaseMenuResearch playerBaseMenuResearch)
    {
        Console.WriteLine();
        Console.WriteLine("RESEARCH");

        for (int i = 0; i < playerBaseMenuResearch.Buttons.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {playerBaseMenuResearch.Buttons[i]}");
        }
    }

    private void RedrawPlayerBaseMenuEmbark(IPlayerBaseMenuEmbark playerBaseMenuEmbark)
    {
        Console.WriteLine();
        Console.WriteLine("EMBARK");

        for (int i = 0; i < playerBaseMenuEmbark.Buttons.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {playerBaseMenuEmbark.Buttons[i]}");
        }
    }
}
