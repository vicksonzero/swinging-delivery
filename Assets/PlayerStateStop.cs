﻿using UnityEngine;

public class PlayerStateStop : IPlayerState
{
    public override string GetName() => "Stop";

    public bool prepHop = false;

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

        if (fixedMouse.wasUp && prepHop)
        {
            var dir = new Vector3(0.5f, 1);
            dir.x = player.runningDir * Mathf.Abs(dir.x);
            player.Hop(dir);
            return new PlayerStateHop(player);
        }
        if (fixedMouse.wasUp && player.grapple && player.grapple.isShooting())
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
