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
        var fixedMouse = player.fixedMouse;
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

        if (fixedMouse.wasUp && prepHop)
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
        if (fixedMouse.wasUp && grapple)
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
        if (fixedMouse.wasDown)
        {
            if (fixedMouse.y < player.transform.position.y)
            {
                startPrepHop = Time.fixedTime;
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

        if (prepHop && Time.fixedTime - startPrepHop >= stopRequirement)
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

        player.velocity.y += augmentedGravity * Time.deltaTime;
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
        player.controller.Move(displacement * Time.deltaTime);
        if (grapple != null)
        {
            grapple.wasSwinging = false;
        }
    }

    public override void OnDetach()
    {
    }
}
