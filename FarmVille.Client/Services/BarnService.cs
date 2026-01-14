using System;

namespace FarmVille.Client.Services;

public class BarnService
{
    public event Action? OnToggle;
    public event Action? OnCropSell;

    public void ToggleBarn()
    {
        OnToggle?.Invoke();
    }

    public void CropSold()
    {
        OnCropSell?.Invoke();
    }
}
