using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public float jumpHeight = 3;
    public float timeToJumpApex = .5f;
    public float dropGravityMultiplier = 0.5f;
    float accelerationTimeAirborne = .4f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    internal Vector3 velocity;
    float velocityXSmoothing;

    public Grapple grapple;

    internal Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        //Debug.Log(Input.GetKeyDown(KeyCode.Space) + " " + controller.collisions.below);
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            Debug.Log("Jump");
            Jump();
            controller.collisions.below = false;
        }
        if (grapple)
        {
            if (grapple.isShooting())
            {
                grapple.lineRenderer.SetPosition(0, transform.position - grapple.transform.position);
                grapple.lineRenderer.SetPosition(1, grapple.startingPosition * (grapple.shootingInterval - (Time.time - grapple.startTime)) / grapple.shootingInterval);
            }
            else
            {
                grapple.lineRenderer.SetPosition(0, transform.position - grapple.transform.position);
                grapple.lineRenderer.SetPosition(1, Vector3.zero);
            }
        }
    }

    void FixedUpdate()
    {
        Debug.Log(
            controller.collisions.above + " " +
            controller.collisions.below + " " +
            controller.collisions.left + " " +
            controller.collisions.right + " " +
            controller.collisions.climbingSlope
            );
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        var projectedPos = transform.position + velocity * Time.deltaTime;
        var distToGrapple = (grapple == null) ? 0 : Vector3.Distance(projectedPos, grapple.transform.position);
        //Debug.Log("grapple dist" + distToGrapple);
        if (grapple == null || distToGrapple < grapple.grappleLength)
        {
            //Debug.Log("Normal dist=" + distToGrapple);
            float targetVelocityX = input.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);


            float augmentedGravity = gravity;
            if (velocity.y <= 0)
            {
                augmentedGravity *= dropGravityMultiplier;
            }
            if (grapple != null)
            {
                if (grapple.isShooting())
                {
                    augmentedGravity = 0;
                }
                else
                {
                    augmentedGravity /= dropGravityMultiplier;
                }
            }
            velocity.y += augmentedGravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, -8);

            var displacement = velocity;
            controller.Move(displacement * Time.deltaTime);
            if (grapple != null)
            {
                grapple.wasSwinging = false;
            }
        }
        else if (!controller.collisions.below)
        {
            if (grapple.wasSwinging == false)
            {
                grapple.InitSwing(this);
            }
            grapple.time += Time.deltaTime;
            var angle = -Mathf.Sign(grapple.startingPosition.x) * grapple.totalAngle * Mathf.Pow(Mathf.Sin(grapple.time / grapple.totalTIme * Mathf.PI), 2);
            //Debug.Log("time:" + (grapple.time / grapple.totalTIme) + " angle:" + angle);
            //Debug.DrawLine(grapple.transform.position, grapple.transform.position + grapple.startingPosition, Color.red);
            //Debug.DrawLine(grapple.transform.position, grapple.transform.position + Quaternion.AngleAxis(-Mathf.Sign(grapple.startingPosition.x) * grapple.totalAngle, Vector3.forward) * grapple.startingPosition, Color.green);
            //Debug.DrawLine(grapple.transform.position, grapple.transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * grapple.startingPosition);


            var swingPos = Quaternion.AngleAxis(angle, Vector3.forward) * grapple.startingPosition;
            var displacement = grapple.transform.position + swingPos - transform.position;
            velocity = displacement / Time.deltaTime;

            controller.Move(displacement);
            //velocity = velocity.normalized * originalSpeed;

            grapple.wasSwinging = true;
        }
    }

    public void SetGrapple(Grapple g)
    {
        if (grapple != null)
        {
            Destroy(grapple.gameObject);
        }
        grapple = g;
        if (grapple)
        {
            grapple.InitSwing(this);
        }
    }

    public void Jump()
    {
        velocity.y = jumpVelocity;
    }

    public void JumpDiagonal(Vector3 direction)
    {
        direction = direction.normalized * jumpVelocity;
        velocity = direction;
    }
}