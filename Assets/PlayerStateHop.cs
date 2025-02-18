﻿using UnityEngine;

public class PlayerStateHop : IPlayerState
{
    public override string GetName() => "Hop";

    public PlayerStateHop(Player p) : base(p)
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
        if (collisions.below)
        {
            // player.runningDir = player.runningDir; // keep runningDir
            player.SetGrapple(null);
            return new PlayerStateRun(player);
        }
        if (collisions.left || collisions.right)
        {
            // player.runningDir = player.runningDir; // keep runningDir
            player.velocity.y = player.moveSpeed;
            return new PlayerStateWallRun(player);
        }
        if (grapple != null && !grapple.IsShooting())
        {
            var projectedPos = player.transform.position + player.velocity * BReplay.FixedDeltaTime();
            var distToGrapple = (grapple == null) ? 0 : Vector3.Distance(projectedPos, grapple.transform.position);

            var isFalling = player.velocity.y < 0;
            if (isFalling || distToGrapple >= grapple.grappleLength)
            {
                grapple.InitSwing(player, player.velocity);
                return new PlayerStateSwing(player);
            }
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
            player.CreateGrapple(fuMouse.x, fuMouse.y);
        }
        return null;
    }

    public override void HandleMovement()
    {
        float augmentedGravity = GetGravity();
        var grapple = player.grapple;

        player.velocity.y += augmentedGravity * BReplay.FixedDeltaTime();
        player.velocity.y = Mathf.Max(player.velocity.y, -8);

        var displacement = player.velocity;
        if (player.controller.collisions.above && displacement.y > 0)
        {
            displacement.y = 0;
        }
        if (grapple != null && grapple.IsShooting())
        {
            displacement *= 4f / displacement.magnitude;
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
