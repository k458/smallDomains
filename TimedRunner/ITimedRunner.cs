using System.Collections.Generic;

namespace TimedRunner;

public interface ITimedRunner : ITRunnable
{
    IReadOnlyList<ITRunnable> Runnables { get; }
    float TickInterval { get; set; }
    float TickProgress { get; }

    bool Subscribe(ITRunnable runnable);
    bool Unsubscribe(ITRunnable runnable);
}
