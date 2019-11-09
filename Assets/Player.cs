using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(GrappleThrower))]
public class Player : MonoBehaviour
{
    public string stateName = "";

    public IPlayerState state;
    public IPlayerState nextState;
    public FixedMouseButtons fixedMouse;

    public float jumpHeight = 2;
    public float timeToJumpApex = .5f;
    public float dropGravityMultiplier = 0.5f;
    public int runningDir = 0;
    internal float accelerationTimeAirborne = 1.1f;
    internal float accelerationTimeGrounded = .7f;
    internal float moveSpeed = 4;
    internal float airMoveSpeed = 2;
    internal float dashSpeed = 2f;

    internal float gravity;
    float jumpVelocity;
    internal Vector3 velocity;
    internal float velocityXSmoothing;
    internal float velocityYSmoothing;

    public Grapple grapple;

    internal Controller2D controller;
    private GrappleThrower grappleThrower;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        grappleThrower = GetComponent<GrappleThrower>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        state = new PlayerStateStop(this);
        state.OnAttach();
        stateName = state.GetName();
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
        fixedMouse = new FixedMouseButtons();
    }

    void Update()
    {
        //Debug.Log(Input.GetKeyDown(KeyCode.Space) + " " + controller.collisions.below);
        //if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        //{
        //    Debug.Log("Jump");
        //    Jump();
        //    controller.collisions.below = false;
        //}
        if (Input.GetMouseButtonUp(0))
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = 0;

            fixedMouse.wasUp = true;
            fixedMouse.x = pos.x;
            fixedMouse.y = pos.y;
        }
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = 0;

            fixedMouse.wasDown = true;
            fixedMouse.x = pos.x;
            fixedMouse.y = pos.y;
        }

        // grapple graphics
        if (grapple)
        {
            if (grapple.IsShooting())
            {
                grapple.lineRenderer.SetPosition(0, transform.position - grapple.transform.position);
                grapple.lineRenderer.SetPosition(1, grapple.startingPosition * (grapple.shootingInterval - (Time.fixedTime - grapple.startTime)) / grapple.shootingInterval);
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
        //Debug.Log(
        //    controller.collisions.above + " " +
        //    controller.collisions.below + " " +
        //    controller.collisions.left + " " +
        //    controller.collisions.right + " " +
        //    controller.collisions.climbingSlope
        //    );

        //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (controller.collisions.below)
        {
            velocity.y = 0;
        }
        nextState = state.HandleInput();
        fixedMouse = new FixedMouseButtons();
        if (nextState != null)
        {
            ChangeState(nextState);
            nextState = null;
        }
        state.HandleMovement();
    }

    private void ChangeState(IPlayerState newState)
    {
        // use newState to tell player to change state. player should choose when to change it
        var oldState = state;
        Debug.Log("ChangeState: " + oldState.GetName() + " -> " + newState.GetName());
        oldState.OnDetach();

        state = newState;
        stateName = state.GetName();

        state.OnAttach();
    }

    internal void DoRun()
    {
        //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 input = new Vector2(runningDir, 0);
        float targetVelocityX = input.normalized.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);


        float augmentedGravity = state.GetGravity();

        velocity.y += augmentedGravity * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, -8);

        var displacement = velocity;
        if (controller.collisions.above && displacement.y > 0)
        {
            displacement.y = 0;
        }
        if (grapple != null && grapple.IsShooting())
        {
            displacement *= 0.01f / displacement.magnitude;
        }
        controller.Move(displacement * Time.deltaTime);
        if (grapple != null)
        {
            grapple.wasSwinging = false;
        }
    }

    internal void DoSwing()
    {
        if (grapple.wasSwinging == false)
        {
            grapple.InitSwing(this, velocity);
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

    public void SetGrapple(Grapple g)
    {
        if (grapple != null)
        {
            Destroy(grapple.gameObject);
        }
        grapple = g;
        if (grapple)
        {
            grapple.InitSwing(this, velocity);
        }
    }

    public void CreateGrapple()
    {
        var grappleInst = grappleThrower.CreateGrapple();
        SetGrapple(grappleInst);
    }

    public void Jump()
    {
        velocity.y = jumpVelocity;
    }

    public void JumpDiagonal(Vector3 direction)
    {
        direction = direction.normalized * jumpVelocity;
        velocity = direction;
        runningDir = (int)Mathf.Sign(direction.x);
    }

    public void Hop(Vector3 direction)
    {
        Debug.Log("Hop!");
        direction = new Vector3(Mathf.Sign(direction.x) * airMoveSpeed, 1 * jumpVelocity);
        velocity = direction;
        runningDir = (int)Mathf.Sign(direction.x);
    }

    public struct FixedMouseButtons
    {
        public float x;
        public float y;
        public bool wasDown;
        public bool wasUp;
    }
}