namespace GameLoopProcessing;

public interface IVariableRateProcessable
{
    void ProcessFrame(float deltaTime, float tickProgress);
}
