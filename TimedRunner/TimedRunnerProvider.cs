namespace TimedRunner;

public static class TimedRunnerProvider
{
    public static ITimedRunner Singleton { get; } = new TimedRunner();
}
