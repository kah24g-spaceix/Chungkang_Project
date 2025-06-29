public class HumanPlayerStats : BasePlayerStats
{
    protected override void Start()
    {
        moveSpeed = 4f;  // Human 특화 이동속도
        base.Start();
    }
}
