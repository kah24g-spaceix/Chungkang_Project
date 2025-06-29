public class AndroidPlayerStats : BasePlayerStats
{
    protected override void Start()
    {
        moveSpeed = 5f;  // Android 특화 이동속도
        base.Start();
    }
}
