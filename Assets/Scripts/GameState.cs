using System;

public static class GameState
{
    public const int FrontLane = 0;
    public const int MiddleLane = 1;
    public const int BackLane = 2;

    private static int currentLane = MiddleLane;

    public static event Action<int> CurrentLaneChanged;

    public static int CurrentLane
    {
        get => currentLane;
        set
        {
            int newLane = UnityEngine.Mathf.Clamp(value, FrontLane, BackLane);

            if (currentLane == newLane)
            {
                return;
            }

            currentLane = newLane;
            CurrentLaneChanged?.Invoke(currentLane);
        }
    }
}
