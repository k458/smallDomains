using System;
using System.Diagnostics;
using System.Threading;
using SpaceGameRoguelike.GameHandling;
using TimedRunner;

namespace SpaceGameRoguelike.View;

public class ViewTerminal
{
    public bool IsRunning { get; private set; }

    public void Run(IGameHandler gameHandler)
    {
        IsRunning = true;
        Stopwatch stopwatch = Stopwatch.StartNew();

        while (IsRunning)
        {
            float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();

            TimedRunnerProvider.Singleton.RunOnce(deltaTime);

            if (gameHandler.IsWaitingForCommand)
            {
                Console.Write("input: ");
                _ = Console.ReadLine();
            }
            else
            {
                Thread.Sleep(1);
            }

            if (gameHandler.NeedsRedraw)
            {
            }
        }
    }

    public void Stop()
    {
        IsRunning = false;
    }
}
