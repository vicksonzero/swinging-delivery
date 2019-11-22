using UnityEngine;

public class PlayerStateRun : IPlayerState
{
    public override string GetName() => "Run";

    public bool prepHop = false;
    public float startPrepHop = 0;
    public float stopRequirement = 0.8f;

    public PlayerStateRun(Player p) : base(p)
    {
    }

    public override void OnAttach()
    {
    }

    public override IPlayerState HandleInput()
    {
        var fuMouse = player.fuMouse;
        var grapple = player.grapple;
        var collisions = player.controller.collisions;
        if (collisions.left || collisions.right)
        {
            // player.runningDir = player.runningDir; // keep runningDir
            player.velocity.y = player.moveSpeed;
            return new PlayerStateWallRun(player);
        }
        if (!collisions.below)
        {
            // player.runningDir = player.runningDir; // keep runningDir
            var dir = new Vector3(player.runningDir, 1);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }

        if (fuMouse.wasUp && prepHop)
        {
            var dir = new Vector3(player.runningDir, 1);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (grapple && !grapple.IsShooting())
        {
            var dir = new Vector3(player.runningDir, 1);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (fuMouse.wasUp && grapple)
        {
            if (grapple.IsShooting())
            {
                player.velocity = -grapple.startingPosition * player.dashSpeed;
                player.runningDir = (int)Mathf.Sign(player.velocity.x);
                player.SetGrapple(null);
                return new PlayerStateHop(player);
            }
            else
            {
                // never reached. if reached, release grapple and resume
                player.SetGrapple(null);
            }
        }
        if (fuMouse.wasDown)
        {
            if (fuMouse.y < player.transform.position.y)
            {
                startPrepHop = BReplay.FixedTime();
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
            return new PlayerStateStop(player);
        }

        return null;
    }

    public override void HandleMovement()
    {
        var grapple = player.grapple;
        //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 input = new Vector2(player.runningDir, 0);
        float targetVelocityX = input.normalized.x * player.moveSpeed;
        player.velocity.x = Mathf.SmoothDamp(player.velocity.x, targetVelocityX, ref player.velocityXSmoothing, player.accelerationTimeGrounded);


        float augmentedGravity = GetGravity();

        player.velocity.y += augmentedGravity * BReplay.FixedDeltaTime();
        player.velocity.y = Mathf.Max(player.velocity.y, -8);

        var displacement = player.velocity;
        if (player.controller.collisions.above && displacement.y > 0)
        {
            displacement.y = 0;
        }
        if (grapple != null && grapple.IsShooting() || prepHop)
        {
            displacement *= 1f / displacement.magnitude;
        }
        player.controller.Move(displacement * BReplay.FixedDeltaTime());
        if (grapple != null)
        {
            grapple.wasSwinging = false;
        }
    }

    public override void OnDetach()
    {
    }
}
