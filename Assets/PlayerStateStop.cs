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
        var fuMouse = player.fuMouse;
        var grapple = player.grapple;
        //Debug.Log("PlayerStateStop.HandleInput " + fuMouse.wasDown + " " + player.transform.position.y + " " + fixedMouse.y);

        if (fuMouse.wasUp && prepRun)
        {
            GameObject.FindObjectOfType<BAppleCounter>().StartTImer();

            return new PlayerStateRun(player);
        }
        if (fuMouse.wasUp && player.grapple)
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
                var dir = new Vector3(player.runningDir, 1);
                player.Hop(dir);
                return new PlayerStateHop(player);
            }
        }
        if (fuMouse.wasDown)
        {
            if (fuMouse.y < player.transform.position.y)
            {
                prepRun = true;
                player.runningDir = (int)Mathf.Sign(fuMouse.x - player.transform.position.x);
                Debug.Log("prepRun");
                // TODO: set player sprite to PREP_HOP here
            }
            else
            {
                player.CreateGrapple(fuMouse.x, fuMouse.y);
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
