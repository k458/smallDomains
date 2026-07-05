using System.Collections.Generic;

namespace GameLoopProcessing;

public interface IProcessingController
{
    IReadOnlyList<IFixedRateProcessable> FixedRateProcessables { get; }
    IReadOnlyList<IVariableRateProcessable> VariableRateProcessables { get; }

    bool AutomaticFixedTick { get; set; }
    float TickTime { get; set; }
    float TickProgress { get; }

    bool TryAddFixedRateProcessable(IFixedRateProcessable processable);
    bool TryRemoveFixedRateProcessable(IFixedRateProcessable processable);
    bool TryAddVariableRateProcessable(IVariableRateProcessable processable);
    bool TryRemoveVariableRateProcessable(IVariableRateProcessable processable);
    void TriggerFixedTick();
    void ProcessFrame(float deltaTime);
}
