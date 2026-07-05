using System.Collections.Generic;

namespace ItemContainers;

public interface IItemContainer
{
    List<IItem> Items { get; }
    bool LimitedByVolume { get; }
    bool LimitedByWeight { get; }
    int VolumeLimit { get; }
    int WeightLimit { get; }

    bool TryAddItem(IItem item);
    bool TryRemoveItem(IItem item);
}
