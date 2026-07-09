using System.Collections.Generic;
using SpaceGameRoguelike.ResourceData;

namespace SpaceGameRoguelike.Campaign;

public interface ICampaign
{
    IReadOnlyDictionary<CurrencyType, int> Currencies { get; }

    void AddCurrency(CurrencyType currencyType, int amount);
    bool TryRemoveCurrency(CurrencyType currencyType, int amount);
}
