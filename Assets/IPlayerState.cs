public abstract class IPlayerState
{
    public Player player;
    public abstract string GetName();
    public IPlayerState(Player p)
    {
        player = p;
    }
    public virtual void OnAttach()
    {

    }

    /**
     * handles interactions, and can set next state here
     */
    public virtual IPlayerState HandleInput()
    {
        return null;
    }
    public virtual void OnDetach()
    {
    }
    public virtual void HandleMovement()
    {
    }
    public virtual float GetGravity()
    {
        var velocity = player.velocity;
        var grapple = player.grapple;
        var augmentedGravity = player.gravity;

        if (velocity.y <= 0)
        {
            augmentedGravity *= player.dropGravityMultiplier;
        }
        if (grapple != null)
        {
            augmentedGravity /= player.dropGravityMultiplier;
        }

        return augmentedGravity;
    }
}
