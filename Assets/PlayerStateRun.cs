using UnityEngine;

public class PlayerStateRun : IPlayerState
{
    public override string GetName() => "Run";

    public bool prepHop = false;

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
            var dir = new Vector3(0.5f, 1);
            dir.x = player.runningDir * Mathf.Abs(dir.x);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }

        if (fixedMouse.wasUp && prepHop)
        {
            var dir = new Vector3(0.5f, 1);
            dir.x = player.runningDir * Mathf.Abs(dir.x);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (grapple != null && !grapple.isShooting())
        {
            var dir = new Vector3(0.5f, 1);
            dir.x = player.runningDir * Mathf.Abs(dir.x);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (fixedMouse.wasDown)
        {
            if (fixedMouse.y < player.transform.position.y)
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
        player.DoRun();
    }

    public override void OnDetach()
    {
    }
}
