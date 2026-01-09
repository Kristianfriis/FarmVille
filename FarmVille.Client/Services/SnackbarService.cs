using System;

namespace FarmVille.Client.Services;

public enum SnackbarType { Success, Error, Info }

public class SnackbarService
{
    // The UI component will subscribe to this event
    public event Action<string, SnackbarType>? OnShow;

    public void Show(string message, SnackbarType type = SnackbarType.Success)
    {
        OnShow?.Invoke(message, type);
    }
}