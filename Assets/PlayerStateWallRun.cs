using UnityEngine;

public class PlayerStateWallRun : IPlayerState
{
    public override string GetName() => "WallRun";

    public bool prepHop = false;

    public PlayerStateWallRun(Player p) : base(p)
    {
    }

    public override void OnAttach()
    {
    }

    public override IPlayerState HandleInput()
    {
        var fixedMouse = player.fixedMouse;
        var collisions = player.controller.collisions;
        var grapple = player.grapple;
        if (!(collisions.left || collisions.right))
        {
            // player.runningDir = player.runningDir; // keep runningDir
            var dir = new Vector3(0.5f, 1);
            dir.x = player.runningDir * Mathf.Abs(dir.x);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (fixedMouse.wasUp && prepHop)
        {
            var dir = new Vector3(1, 1);
            dir.x = -player.runningDir * Mathf.Abs(dir.x);
            player.runningDir = (int)Mathf.Sign(dir.x);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (fixedMouse.wasUp && grapple && grapple.isShooting())
        {
            var dir = new Vector3(1, 1);
            dir.x = -player.runningDir * Mathf.Abs(dir.x);
            player.runningDir = (int)Mathf.Sign(dir.x);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (grapple != null && !grapple.isShooting())
        {
            var dir = new Vector3(1, 1);
            dir.x = -player.runningDir * Mathf.Abs(dir.x);
            player.runningDir = (int)Mathf.Sign(dir.x);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (fixedMouse.wasDown)
        {
            var clickingIntoWall = player.runningDir * (fixedMouse.x - player.transform.position.x) > 0;
            if (clickingIntoWall)
            {
                prepHop = true;
                player.runningDir = (int)Mathf.Sign(fixedMouse.x - player.transform.position.x);
                Debug.Log("prepHop");
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
        Vector2 input = new Vector2(1, 1);
        float targetVelocityY = input.normalized.y * player.moveSpeed;
        input.x = input.x * player.runningDir;
        player.velocity.y = Mathf.SmoothDamp(player.velocity.y, targetVelocityY, ref player.velocityYSmoothing, player.accelerationTimeGrounded);
        player.velocity.x = input.x;

        var displacement = player.velocity;
        player.controller.Move(displacement * Time.deltaTime);
    }

    public override void OnDetach()
    {
    }
}
