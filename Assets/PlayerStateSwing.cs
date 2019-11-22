using UnityEngine;

public class PlayerStateSwing : IPlayerState
{
    public override string GetName() => "Swing";

    public float additionalHeightThreshold = 6f;

    public PlayerStateSwing(Player p) : base(p)
    {
    }

    public override void OnAttach()
    {
    }

    public override IPlayerState HandleInput()
    {
        var fuMouse = player.fuMouse;
        var grapple = player.grapple;
        var projectedPos = player.transform.position + player.velocity * BReplay.FixedDeltaTime();
        var distToGrapple = (player.grapple == null) ? 0 : Vector3.Distance(projectedPos, player.grapple.transform.position);

        var collisions = player.controller.collisions;
        if (collisions.left || collisions.right)
        {
            player.runningDir = (int)Mathf.Sign(player.velocity.x); // set runningDir
            player.SetGrapple(null);
            player.velocity.y = player.moveSpeed;
            return new PlayerStateWallRun(player);
        }
        if (player.controller.collisions.below)
        {
            player.SetGrapple(null);
            player.runningDir = (int)Mathf.Sign(player.velocity.x); // set runningDir
            return new PlayerStateRun(player);
        }
        //Debug.Log("PlayerStateStop.HandleInput " + fuMouse.wasDown + " " + player.transform.position.y + " " + fixedMouse.y);

        if (fuMouse.wasUp)
        {
            if (grapple.time / grapple.totalTIme > 0.5f && player.velocity.magnitude < additionalHeightThreshold)
            {
                var playerToGrapple = grapple.transform.position - player.transform.position;

                var dirX = Mathf.Sign(playerToGrapple.x) * Mathf.Sin(grapple.time / grapple.totalTIme * Mathf.PI);
                if (dirX == 0)
                {
                    dirX = 1;
                }

                var dir = new Vector3(0.5f, 1);
                dir.x = player.runningDir * Mathf.Abs(dir.x);
                player.JumpDiagonal(dir);
            }
            else
            {
                // just release
                player.velocity = player.velocity.normalized * Mathf.Min(player.velocity.magnitude, 20f);
            }
            player.runningDir = (int)Mathf.Sign(player.velocity.x); // set runningDir
            player.SetGrapple(null);
            return new PlayerStateHop(player);
        }
        return null;
    }

    public override void HandleMovement()
    {
        player.DoSwing();
    }

    public override void OnDetach()
    {
    }
}
