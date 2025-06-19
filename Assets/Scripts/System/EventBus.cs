using System;

public static class EventBus
{
    public static Action<float> OnMoveInputChanged;
    public static Action OnStateChanged;
    public static Action OnRoll;

    public static Action<int, int> OnHpChanged;
    public static Action<int, int> OnEpChanged;
    public static Action<int, int> OnExpChanged;
    public static Action<int> OnLevelUp;
    public static Action<float> OnMoveSpeedChanged;
}