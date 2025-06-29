public class HumanAnimationComponent : BasePlayerAnimation
{
    // SetRunSpeed는 Human이 사용하지 않으므로 비워둠
    public override void SetRunSpeed(float speed)
    {
        // Human 플레이어는 RunSpeed 파라미터를 사용하지 않음
    }
}
