using System;
using System.Collections.Generic;

namespace TimedRunner;

public class TimedRunner : ITimedRunner
{
    private readonly List<ITRunnable> runnables = new();
    private float tickRate = 1f;
    private float timeSinceLastTick;

    public IReadOnlyList<ITRunnable> Runnables => runnables;

    public float TickRate
    {
        get => tickRate;
        set
        {
            if (value <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Tick rate must be greater than zero.");
            }

            tickRate = value;
            timeSinceLastTick = MathF.Min(timeSinceLastTick, tickRate);
        }
    }

    public float TickProgress => Math.Clamp(timeSinceLastTick / tickRate, 0f, 1f);

    public bool Subscribe(ITRunnable runnable)
    {
        if (runnables.Contains(runnable))
        {
            return false;
        }

        runnables.Add(runnable);
        return true;
    }

    public bool Unsubscribe(ITRunnable runnable)
    {
        return runnables.Remove(runnable);
    }

    public void RunOnce(float deltaTime)
    {
        timeSinceLastTick += deltaTime;

        while (timeSinceLastTick >= tickRate)
        {
            RunSubscribed(tickRate);
            timeSinceLastTick -= tickRate;
        }
    }

    private void RunSubscribed(float deltaTime)
    {
        ITRunnable[] currentRunnables = runnables.ToArray();

        foreach (ITRunnable runnable in currentRunnables)
        {
            runnable.RunOnce(deltaTime);
        }
    }
}
