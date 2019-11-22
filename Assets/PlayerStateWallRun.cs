using UnityEngine;

public class PlayerStateWallRun : IPlayerState
{
    public override string GetName() => "WallRun";

    public bool prepHop = false;
    public float startPrepHop = 0;
    public float stopRequirement = 0.8f;

    public PlayerStateWallRun(Player p) : base(p)
    {
    }

    public override void OnAttach()
    {
    }

    public override IPlayerState HandleInput()
    {
        var fuMouse = player.fuMouse;
        var collisions = player.controller.collisions;
        var grapple = player.grapple;
        if (!(collisions.left || collisions.right))
        {
            // player.runningDir = player.runningDir; // keep runningDir
            var dir = new Vector3(player.runningDir, 1);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (fuMouse.wasUp && prepHop)
        {
            var dir = new Vector3(1, 1);
            dir.x = -player.runningDir * Mathf.Abs(dir.x);
            player.runningDir = (int)Mathf.Sign(dir.x);
            player.JumpDiagonal(dir);
            return new PlayerStateHop(player);
        }
        if (grapple && !grapple.IsShooting())
        {
            var dir = new Vector3(1, 1);
            dir.x = -player.runningDir * Mathf.Abs(dir.x);
            player.runningDir = (int)Mathf.Sign(dir.x);
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
                startPrepHop = Time.fixedTime;
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

        if (prepHop && BReplay.FixedTime() - startPrepHop >= stopRequirement)
        {
            player.velocity = Vector3.zero;
            return new PlayerStateWallStop(player);
        }
        return null;
    }

    public override void HandleMovement()
    {
        var grapple = player.grapple;
        Vector2 input = new Vector2(1, 1);
        float targetVelocityY = input.normalized.y * player.moveSpeed;
        input.x = input.x * player.runningDir;
        player.velocity.y = Mathf.SmoothDamp(player.velocity.y, targetVelocityY, ref player.velocityYSmoothing, player.accelerationTimeGrounded);
        player.velocity.x = input.x;

        var displacement = player.velocity;
        if (grapple != null && grapple.IsShooting() || prepHop)
        {
            displacement *= 1f / displacement.magnitude;
        }
        player.controller.Move(displacement * BReplay.FixedDeltaTime());
    }

    public override void OnDetach()
    {
    }
}
