using SpaceGameRoguelike.GameHandling;
using SpaceGameRoguelike.View;
using TimedRunner;

namespace SpaceGameRoguelike;

public class App
{
    private readonly GameHandler gameHandler = new();
    private readonly ViewTerminal viewTerminal = new();
    private readonly ITimedRunner timedRunner = new TimedRunner.TimedRunner();

    public void Run()
    {
        timedRunner.Subscribe(gameHandler);
        viewTerminal.Run(gameHandler, timedRunner);
    }

    public void Stop()
    {
        viewTerminal.Stop();
        timedRunner.Unsubscribe(gameHandler);
    }
}
