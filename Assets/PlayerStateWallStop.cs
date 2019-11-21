using UnityEngine;

public class PlayerStateWallStop : IPlayerState
{
    public override string GetName() => "WallStop";

    public bool prepHop = false;

    public PlayerStateWallStop(Player p) : base(p)
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

        if (fuMouse.wasUp && prepHop)
        {
            var dir = new Vector3(1, 1);
            dir.x = -player.runningDir * Mathf.Abs(dir.x);
            player.runningDir = (int)Mathf.Sign(dir.x);
            player.JumpDiagonal(dir);
            return new PlayerStateHop(player);
        }
        if (grapple != null && !grapple.IsShooting())
        {
            var dir = new Vector3(1, 1);
            dir.x = player.runningDir * Mathf.Abs(dir.x);
            player.JumpDiagonal(dir);
            return new PlayerStateHop(player);
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
                // never reached
                var dir = new Vector3(0.5f, 1);
                dir.x = player.runningDir * Mathf.Abs(dir.x);
                player.Hop(dir);
                return new PlayerStateHop(player);
            }
        }
        if (fuMouse.wasDown)
        {
            var clickingIntoWall = player.runningDir * (fuMouse.x - player.transform.position.x) > 0;
            if (clickingIntoWall)
            {
                prepHop = true;
                player.runningDir = (int)Mathf.Sign(fuMouse.x - player.transform.position.x);
                Debug.Log("prepHop");
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
