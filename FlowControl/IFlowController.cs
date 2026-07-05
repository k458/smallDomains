namespace FlowControl;

public interface IFlowController
{
    void AddFlowBlocker(IFlowBlocker blocker);

    void RemoveFlowBlocker(IFlowBlocker blocker);
}
