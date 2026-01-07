namespace FarmVille.Client.Models;

public enum TileAction { None, Plowing, Sowing, Harvesting }

public class FarmTile
{
    public int X { get; set; }
    public int Y { get; set; }

    public TileAction CurrentAction { get; set; } = TileAction.None;
    public TileAction QueuedAction { get; set; }
    public double ActionProgress { get; set; } = 0;
    public double ActionDuration { get; set; } = 3;
    public DateTime? ActionStartedAt { get; set; }
    public TimeSpan? ActionDurationTime { get; set; }
    public int CostWhenCompleted { get; set; } = 0;
    public int ExperienceWhenCompleted { get; set; } = 0;

    public TileStatus Status { get; set; } = TileStatus.Grass;
    public Crop? CurrentCrop { get; set; }
    public DateTime? PlantedAt { get; set; }

    public void UpdateState()
    {
        if (Status == TileStatus.Planted && PlantedAt.HasValue && CurrentCrop != null)
        {
            var elapsed = DateTime.Now - PlantedAt.Value;
            if (elapsed > CurrentCrop.GrowthTime.Add(TimeSpan.FromDays(CurrentCrop.DaysToWither)))
                Status = TileStatus.Withered;
            else if (elapsed >= CurrentCrop.GrowthTime)
                Status = TileStatus.Ready;
        }
    }

    public void StartAction(TileAction action, int duration, int cost = 0, int experience = 0)
    {
        CurrentAction = action;
        // Force the conversion to be explicit
        ActionDurationTime = TimeSpan.FromSeconds(duration);

        ActionProgress = 0;
        ActionStartedAt = DateTime.Now;
        CostWhenCompleted = cost;
        ExperienceWhenCompleted = experience;
    }

    public bool UpdateActionProgress()
    {
        // 1. Safety check: Ensure we have both a start time and a duration set
        if (CurrentAction == TileAction.None || !ActionStartedAt.HasValue || !ActionDurationTime.HasValue)
            return false;

        // 2. Calculate based on the TimeSpan you set in StartAction
        var elapsed = DateTime.Now - ActionStartedAt.Value;
        var totalSeconds = ActionDurationTime.Value.TotalSeconds;

        // 3. Prevent division by zero just in case
        if (totalSeconds <= 0)
        {
            ActionProgress = 100;
        }
        else
        {
            ActionProgress = (elapsed.TotalSeconds / totalSeconds) * 100;
        }

        // 4. Check completion
        if (ActionProgress >= 100)
        {
            ActionProgress = 100; // Cap it for the UI
            return CompleteAction();
        }

        return false;
    }

    public string GetTimeRemaining()
    {
        if (Status != TileStatus.Planted || !PlantedAt.HasValue || CurrentCrop == null) return "";
        var remaining = PlantedAt.Value.Add(CurrentCrop.GrowthTime) - DateTime.Now;
        return remaining.TotalSeconds > 0 ? $"{(int)remaining.TotalMinutes:D2}:{remaining.Seconds:D2}" : "Ready!";
    }

    private bool CompleteAction()
    {
        switch (CurrentAction)
        {
            case TileAction.Plowing:
                Status = TileStatus.Plowed;
                break;
            case TileAction.Sowing:
                Status = TileStatus.Planted;
                PlantedAt = DateTime.Now;
                break;
        }
        CurrentAction = TileAction.None;
        ActionProgress = 0;

        return true;
    }

    public string GetActionRemainingTime()
    {
        if (CurrentAction == TileAction.None || !ActionStartedAt.HasValue || !ActionDurationTime.HasValue)
            return "";

        // 1. Calculate how much time has passed
        var elapsed = DateTime.Now - ActionStartedAt.Value;

        // 2. Subtract elapsed from the total duration
        var remaining = ActionDurationTime.Value - elapsed;

        // 3. Handle the finished state
        if (remaining.TotalSeconds <= 0)
        {
            return "DONE!";
        }

        // 4. Format the output
        // "m\:ss" results in "4:59". If it's over an hour, use "h\:mm\:ss"
        return remaining.ToString(@"m\:ss");
    }
}