using System.Collections.Generic;

namespace GameLoopProcessing;

public class ProcessingController : IProcessingController
{
    private readonly List<IFixedRateProcessable> fixedRateProcessables = new();
    private readonly List<IVariableRateProcessable> variableRateProcessables = new();
    private float tickTime = 1f;
    private float timeSinceLastTick;

    public IReadOnlyList<IFixedRateProcessable> FixedRateProcessables => fixedRateProcessables;
    public IReadOnlyList<IVariableRateProcessable> VariableRateProcessables => variableRateProcessables;

    public bool AutomaticFixedTick { get; set; } = true;

    public float TickTime
    {
        get => tickTime;
        set
        {
            if (value <= 0f)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(value),
                    "Tick time must be greater than zero.");
            }

            tickTime = value;
            timeSinceLastTick = System.MathF.Min(timeSinceLastTick, tickTime);
        }
    }

    public float TickProgress => System.Math.Clamp(timeSinceLastTick / tickTime, 0f, 1f);

    public bool TryAddFixedRateProcessable(IFixedRateProcessable processable)
    {
        return TryAddProcessable(processable, fixedRateProcessables);
    }

    public bool TryRemoveFixedRateProcessable(IFixedRateProcessable processable)
    {
        return fixedRateProcessables.Remove(processable);
    }

    public bool TryAddVariableRateProcessable(IVariableRateProcessable processable)
    {
        return TryAddProcessable(processable, variableRateProcessables);
    }

    public bool TryRemoveVariableRateProcessable(IVariableRateProcessable processable)
    {
        return variableRateProcessables.Remove(processable);
    }

    public void TriggerFixedTick()
    {
        ProcessFixedRateProcessables();
        timeSinceLastTick = 0f;
    }

    public void ProcessFrame(float deltaTime)
    {
        timeSinceLastTick += deltaTime;

        if (AutomaticFixedTick)
        {
            while (timeSinceLastTick >= tickTime)
            {
                ProcessFixedRateProcessables();
                timeSinceLastTick -= tickTime;
            }
        }

        IVariableRateProcessable[] frameProcessables = variableRateProcessables.ToArray();

        foreach (IVariableRateProcessable processable in frameProcessables)
        {
            processable.ProcessFrame(deltaTime, TickProgress);
        }
    }

    private void ProcessFixedRateProcessables()
    {
        IFixedRateProcessable[] tickProcessables = fixedRateProcessables.ToArray();

        foreach (IFixedRateProcessable processable in tickProcessables)
        {
            processable.ProcessFixedTick();
        }
    }

    private static bool TryAddProcessable<TProcessable>(
        TProcessable processable,
        List<TProcessable> processables)
    {
        if (processables.Contains(processable))
        {
            return false;
        }

        processables.Add(processable);
        return true;
    }
}
