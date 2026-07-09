using System;
using System.Collections.Generic;
using SpaceGameRoguelike.ResourceData;

namespace SpaceGameRoguelike.Campaign;

public class Campaign : ICampaign
{
    private readonly Dictionary<CurrencyType, int> currencies = new();

    public IReadOnlyDictionary<CurrencyType, int> Currencies => currencies;

    public void AddCurrency(CurrencyType currencyType, int amount)
    {
        ValidateCurrency(currencyType);
        ValidatePositiveAmount(amount);

        if (currencies.TryGetValue(currencyType, out int currentAmount))
        {
            currencies[currencyType] = currentAmount + amount;
            return;
        }

        currencies.Add(currencyType, amount);
    }

    public bool TryRemoveCurrency(CurrencyType currencyType, int amount)
    {
        ValidateCurrency(currencyType);
        ValidatePositiveAmount(amount);

        if (!currencies.TryGetValue(currencyType, out int currentAmount))
        {
            return false;
        }

        if (currentAmount < amount)
        {
            return false;
        }

        int remainingAmount = currentAmount - amount;

        if (remainingAmount == 0)
        {
            currencies.Remove(currencyType);
            return true;
        }

        currencies[currencyType] = remainingAmount;
        return true;
    }

    private static void ValidateCurrency(CurrencyType currencyType)
    {
        if (currencyType == CurrencyType.Undefined)
        {
            throw new ArgumentException(
                "Currency type must be defined.",
                nameof(currencyType));
        }
    }

    private static void ValidatePositiveAmount(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Currency amount must be greater than zero.");
        }
    }
}
