using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public float jumpHeight = 2;
    public float timeToJumpApex = .3f;
    public float dropGravityMultiplier = 0.9f;
    float accelerationTimeAirborne = .2f;
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

    private void Update()
    {
        Debug.Log(Input.GetKeyDown(KeyCode.Space) + " " + controller.collisions.below);
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            Jump();
            controller.collisions.below = false;
        }
    }

    void FixedUpdate()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        var projectedPos = transform.position + velocity * Time.deltaTime;
        var distToGrapple = (grapple == null) ? 0 : Vector3.Distance(projectedPos, grapple.transform.position);
        //Debug.Log("grapple dist" + distToGrapple);
        if (grapple == null || distToGrapple < grapple.grappleLength)
        {
            //Debug.Log("Normal dist=" + distToGrapple);
            float targetVelocityX = input.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);


            var displacement = velocity;
            float augmentedGravity = velocity.y > 0 ? gravity
                : gravity * dropGravityMultiplier;
            velocity.y += augmentedGravity * Time.deltaTime;

            controller.Move(displacement * Time.deltaTime);
            if (grapple != null)
            {
                grapple.wasSwinging = false;
            }
        }
        else
        {
            if (grapple.wasSwinging == false)
            {
                grapple.InitSwing(this);
            }
            //var displacement = velocity;
            ////var ke = grapple.pe;
            ////var speed = Mathf.Sqrt(2 * ke);

            ////var sign = Mathf.Sign(grapple.transform.position.x - transform.position.x);
            ////var a = Quaternion.AngleAxis(sign * 90, Vector3.back) * (grapple.transform.position - transform.position);

            ////a = a.normalized * speed;
            //var rotatedGravity = Vector3.ProjectOnPlane(Vector3.up * gravity, grapple.transform.position - transform.position) * 1.2f;
            //Debug.DrawLine(transform.position, transform.position + rotatedGravity, Color.yellow);

            ////augmentedGravity = 0;
            ////velocity.y = 0;
            ////velocity += (grapple.transform.position - transform.position).normalized * grappleLength * Time.deltaTime;
            //velocity += rotatedGravity;
            //velocity.y = rotatedGravity.y;

            //Debug.DrawLine(transform.position, transform.position + velocity, Color.red);
            ////Debug.Log(velocity);
            //projectedPos = transform.position + velocity * Time.deltaTime;
            //Debug.DrawLine(transform.position, projectedPos);

            //var swingedPos = (projectedPos - grapple.transform.position).normalized * grapple.grappleLength + grapple.transform.position;
            //var swingVelo = swingedPos - transform.position;
            ////swingedPos = swingedPos.normalized * originalSpeed;
            //Debug.DrawLine(transform.position, transform.position + swingVelo, Color.green);
            //displacement.x = swingVelo.x;
            //displacement.y = swingVelo.y + 0.001f;
            //displacement.z = swingVelo.z;
            //Debug.DrawLine(transform.position, transform.position + velocity);
            //Debug.DrawLine(transform.position, transform.position + velocity);
            grapple.time += Time.deltaTime;
            var angle = -Mathf.Sign(grapple.startingPosition.x) * grapple.totalAngle * Mathf.Pow(Mathf.Sin(grapple.time / grapple.totalTIme * Mathf.PI), 2);
            //Debug.Log("time:" + (grapple.time / grapple.totalTIme) + " angle:" + angle);
            Debug.DrawLine(grapple.transform.position, grapple.transform.position + grapple.startingPosition, Color.red);
            Debug.DrawLine(grapple.transform.position, grapple.transform.position + Quaternion.AngleAxis(-Mathf.Sign(grapple.startingPosition.x) * grapple.totalAngle, Vector3.forward) * grapple.startingPosition, Color.green);
            Debug.DrawLine(grapple.transform.position, grapple.transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * grapple.startingPosition);

            var swingPos = grapple.transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * grapple.startingPosition;
            var displacement = swingPos - transform.position;
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
    }

    public void Jump()
    {
        Debug.Log("Jump");
        velocity.y = jumpVelocity;
    }

    public void JumpDiagonal(Vector3 direction)
    {
        Debug.Log("Jump");
        direction = direction.normalized * jumpVelocity;
        velocity = direction;
    }
}