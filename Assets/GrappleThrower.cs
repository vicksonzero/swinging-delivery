﻿using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Player))]
public class GrappleThrower : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Grapple grapplePrefab;
    public float shootSpeed = 3.6f;
    public float shootingInterval = 0.3f;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.grapple != null && Input.GetMouseButtonUp(0))
        {
            var grapple = player.grapple;
            var playerToGrapple = grapple.transform.position - player.transform.position;
            Debug.Log("GetMouseButtonUp: " +
                "Time= " + (Time.time - grapple.startTime) + " " +
                "grapple= " + grapple.startingPosition.x + ",  " + grapple.startingPosition.y + " " +
                "grapple= " + playerToGrapple.x + ",  " + playerToGrapple.y + " " +
                "");
            if (Time.time - grapple.startTime < grapple.shootingInterval)
            {
                // Debug.Log("grapple.startingPosition.y "+ grapple.startingPosition.y);
                // Debug.Log("player.controller.collisions.below " + player.controller.collisions.below);
                if (grapple.startingPosition.y > 0 && player.controller.collisions.below == true)
                {
                    //var dir = (-grapple.startingPosition + Vector3.up * 1f).normalized;
                    //dir.y = -dir.y;
                    var dir = Vector3.up;
                    dir += Vector3.right * Mathf.Sign(-grapple.startingPosition.x);
                    Debug.Log("JumpDiagonal");
                    player.JumpDiagonal(dir);
                    player.controller.collisions.below = false;
                }
                else
                {
                    player.controller.collisions.above = false;
                    player.controller.collisions.below = false;
                    player.velocity = -grapple.startingPosition * shootSpeed;
                }
            }
            else
            {
                if (!player.controller.collisions.below && player.velocity.magnitude < 6f || (player.controller.collisions.below && playerToGrapple.y < 0))
                {
                    var dirX = Mathf.Sign(-playerToGrapple.x) * Mathf.Sin(grapple.time / grapple.totalTIme * Mathf.PI);
                    if (dirX == 0)
                    {
                        dirX = 1;
                    }

                    var dir = Vector3.up;
                    dir += Vector3.right * dirX / 4;
                    Debug.Log("JumpDiagonal");
                    player.JumpDiagonal(dir.normalized);
                    player.controller.collisions.below = false;
                }
            }
            player.SetGrapple(null);
        }
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = 0;
            var grappleInst = Instantiate(grapplePrefab, pos, Quaternion.identity);
            grappleInst.startTime = Time.time;
            grappleInst.shootingInterval = shootingInterval;
            player.SetGrapple(grappleInst);
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
    }
}
