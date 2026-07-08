using System;
using System.Collections.Generic;

namespace TimedRunner;

public class TimedRunner : ITimedRunner
{
    private readonly List<ITRunnable> runnables = new();
    private float tickInterval = 1f;
    private float timeSinceLastTick;

    public IReadOnlyList<ITRunnable> Runnables => runnables;

    public float TickInterval
    {
        get => tickInterval;
        set
        {
            if (value <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Tick interval must be greater than zero.");
            }

            tickInterval = value;
            timeSinceLastTick = MathF.Min(timeSinceLastTick, tickInterval);
        }
    }

    public float TickProgress => Math.Clamp(timeSinceLastTick / tickInterval, 0f, 1f);

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

        while (timeSinceLastTick >= tickInterval)
        {
            RunSubscribed(tickInterval);
            timeSinceLastTick -= tickInterval;
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
