namespace Targeting;

public interface ITargetingBiases
{
    float ApplyTo(float score, ITargetState targetState);
}
