public class PlayerStateDash : IPlayerState
{
    public override string GetName() => "Dash";
    public PlayerStateDash(Player p) : base(p)
    {
    }

    public override void OnAttach()
    {
    }

    public override IPlayerState HandleInput()
    {
        return null;
    }

    public override void HandleMovement()
    {
    }

    public override void OnDetach()
    {
    }
}
