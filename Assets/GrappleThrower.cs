using UnityEngine;
using UnityEngine.EventSystems;

public class GrappleThrower : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Grapple grapplePrefab;
    public float shootSpeed = 3.6f;
    public float shootingInterval = 0.25f;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void xUpdate()
    {
        if (player.grapple != null && Input.GetMouseButtonUp(0))
        {
            var grapple = player.grapple;
            var playerToGrapple = grapple.transform.position - player.transform.position;
            Debug.Log("GetMouseButtonUp: " +
                "Time= " + (Time.fixedTime - grapple.startTime) + " " +
                "grapple= " + grapple.extremePosition.x + ",  " + grapple.extremePosition.y + " " +
                "grapple= " + playerToGrapple.x + ",  " + playerToGrapple.y + " " +
                "");
            if (grapple.IsShooting())
            {
                // Debug.Log("grapple.startingPosition.y "+ grapple.startingPosition.y);
                // Debug.Log("player.controller.collisions.below " + player.controller.collisions.below);
                if (grapple.extremePosition.y > 0 && player.controller.collisions.below == true)
                {
                    //var dir = (-grapple.startingPosition + Vector3.up * 1f).normalized;
                    //dir.y = -dir.y;
                    var dir = Vector3.up;
                    dir += Vector3.right * Mathf.Sign(-grapple.extremePosition.x);
                    Debug.Log("JumpDiagonal");
                    player.JumpDiagonal(dir);
                    player.controller.collisions.below = false;
                }
                else
                {
                    player.controller.collisions.above = false;
                    player.controller.collisions.below = false;
                    player.velocity = -grapple.extremePosition * shootSpeed;
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
        //if (Input.GetMouseButtonDown(0))
        //{
        //    CreateGrapple();
        //}
    }

    public Grapple CreateGrapple()
    {
        var pos = Input.mousePosition;
        pos.z = 10.0f;
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = -9;
        var grappleInst = Instantiate(grapplePrefab, pos, Quaternion.identity);
        grappleInst.startTime = Time.fixedTime;
        grappleInst.shootingInterval = shootingInterval;

        return grappleInst;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
    }
}
