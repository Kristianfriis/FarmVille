using System;
using FarmVille.Client.Models;

namespace FarmVille.Client.Services;

public class ToolbarService
{    
    public event Action<FarmTool>? OnToolChange;

    public void ChooseTool(FarmTool tool)
    {
        OnToolChange?.Invoke(tool);
    }

}
