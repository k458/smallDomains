using System.Collections.Generic;

namespace NodeMapTridentHandmade;

public interface INodeMapService
{
    IReadOnlyList<INode> GenerateNextDepth(INodeMap nodeMap);

    IReadOnlyList<INode> Grow(INodeMap nodeMap);

    IReadOnlyList<INode> GrowLooped(INodeMap nodeMap);

    IReadOnlyList<INode> GrowRandom(INodeMap nodeMap);

    IReadOnlyList<INode> GrowRandomNoCross(INodeMap nodeMap);

    IReadOnlyList<INode> GrowRandomNoCrossV2(INodeMap nodeMap);

    IReadOnlyList<INode> GrowByBranches(INodeMap nodeMap);

    void MoveToDepth(
        INodeMap nodeMap,
        IReadOnlyList<INode> nextDepth);

    void SetNodeSlot(
        INodeMap nodeMap,
        int depth,
        int y,
        INode node);
}
