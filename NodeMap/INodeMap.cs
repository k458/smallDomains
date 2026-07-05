namespace NodeMap;

public interface INodeMap
{
    bool TryAddNode(
        INode node,
        int x,
        int y);

    bool TryAddDependencyNodesToColumn(int x);
    bool TryAddConnectionsToPreviousNodesForColumn(int x);
    bool Expand(int x);
}
