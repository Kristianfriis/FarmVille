using System;
using FarmVille.Client.Models;

namespace FarmVille.Client.Services;

public class MarketService
{
    public event Action? OnToggle;
    public event Action<Crop>? OnChooseCrop;
    public event Action? OnMarketClosed;

    public void ToggleMarket()
    {
        OnToggle?.Invoke();
    }

    public void CloseMarket()
    {
        OnMarketClosed?.Invoke();
    }

    public void ChooseCrop(Crop crop)
    {
        OnChooseCrop?.Invoke(crop);
    }
}
