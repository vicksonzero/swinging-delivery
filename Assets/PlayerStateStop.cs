using UnityEngine;

public class PlayerStateStop : IPlayerState
{
    public override string GetName() => "Stop";

    public bool prepRun = false;

    public PlayerStateStop(Player p) : base(p)
    {
    }

    public override void OnAttach()
    {
    }

    public override IPlayerState HandleInput()
    {
        var fixedMouse = player.fixedMouse;
        var grapple = player.grapple;
        //Debug.Log("PlayerStateStop.HandleInput " + fixedMouse.wasDown + " " + player.transform.position.y + " " + fixedMouse.y);

        if (fixedMouse.wasUp && prepRun)
        {
            return new PlayerStateRun(player);
        }
        if (fixedMouse.wasUp && player.grapple)
        {
            if (player.grapple.IsShooting())
            {
                player.velocity = -grapple.startingPosition * player.dashSpeed;
                player.runningDir = (int)Mathf.Sign(player.velocity.x);
                player.SetGrapple(null);
                return new PlayerStateHop(player);
            }
            else
            {
                var dir = new Vector3(0.5f, 1);
                dir.x = player.runningDir * Mathf.Abs(dir.x);
                player.Hop(dir);
                return new PlayerStateHop(player);
            }
        }
        if (fixedMouse.wasDown)
        {
            if (fixedMouse.y < player.transform.position.y)
            {
                prepRun = true;
                player.runningDir = (int)Mathf.Sign(fixedMouse.x - player.transform.position.x);
                Debug.Log("prepRun");
                // TODO: set player sprite to PREP_HOP here
            }
            else
            {
                player.CreateGrapple();
            }
        }

        return null;
    }

    public override void HandleMovement()
    {
        // nope, just stay put
    }

    public override void OnDetach()
    {
    }

    public override float GetGravity()
    {
        return 0;
    }
}
