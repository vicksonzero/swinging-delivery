using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Player))]
public class GrappleThrower : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Grapple grapplePrefab;
    public float shootSpeed = 10;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - player.grapple.startTime < 0.25f)
            {
                if (player.grapple.startingPosition.y > 0 && player.controller.collisions.below == true)
                {
                    //var dir = (-player.grapple.startingPosition + Vector3.up * 1f).normalized;
                    //dir.y = -dir.y;
                    var dir = Vector3.up;
                    dir += Vector3.right * Mathf.Sign(-player.grapple.startingPosition.x);
                    player.JumpDiagonal(dir);
                    player.controller.collisions.below = false;
                }
                else
                {
                    player.controller.collisions.above = false;
                    player.controller.collisions.below = false;
                    player.velocity = -player.grapple.startingPosition * shootSpeed;
                }
            }
            else
            {
                if (player.velocity.x < 2f)
                {
                    var dirX = Mathf.Sign(-player.grapple.startingPosition.x) * Mathf.Sin(player.grapple.time / player.grapple.totalTIme * Mathf.PI);
                    if (dirX == 0)
                    {
                        dirX = 1;
                    }

                    var dir = Vector3.up;
                    dir += Vector3.right * dirX;
                    player.JumpDiagonal(dir.normalized);
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
