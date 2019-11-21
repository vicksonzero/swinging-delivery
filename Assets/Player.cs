using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(GrappleThrower))]
public class Player : MonoBehaviour
{
    public string stateName = "";

    public IPlayerState state;
    public IPlayerState nextState;
    public FixedMouseButtons fuMouse;

    public Text stateLabel;

    public float jumpHeight = 2;
    public float timeToJumpApex = .5f;
    public float dropGravityMultiplier = 0.5f;
    public int runningDir = 0;
    internal float accelerationTimeAirborne = 1.1f;
    internal float accelerationTimeGrounded = .7f;
    internal float moveSpeed = 5;
    internal float airMoveSpeed = 3;
    internal float dashSpeed = 2f;

    internal float gravity;
    float jumpVelocity;
    internal Vector3 velocity;
    internal float velocityXSmoothing;
    internal float velocityYSmoothing;

    public Grapple grapple;

    internal Controller2D controller;
    private GrappleThrower grappleThrower;
    internal BReplay replay;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        grappleThrower = GetComponent<GrappleThrower>();
        replay = FindObjectOfType<BReplay>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        state = new PlayerStateStop(this);
        state.OnAttach();
        stateName = state.GetName();
        if (stateLabel)
        {
            stateLabel.text = stateName;
        }
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
        fuMouse = new FixedMouseButtons();
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

            fuMouse.wasUp = true;
            fuMouse.str_x = (int)(pos.x * 1000);
            fuMouse.str_y = (int)(pos.y * 1000);
        }
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = 0;

            fuMouse.wasDown = true;
            fuMouse.str_x = (int)(pos.x * 1000);
            fuMouse.str_y = (int)(pos.y * 1000);
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
        replay.HandleInput(ref fuMouse);
        if (fuMouse.wasDown || fuMouse.wasUp)
        {
            fuMouse.x = 0.001f * fuMouse.str_x;
            fuMouse.y = 0.001f * fuMouse.str_y;
        }
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
        fuMouse = new FixedMouseButtons();
        if (nextState != null)
        {
            ChangeState(nextState);
            nextState = null;
        }
        state.HandleMovement();
        stateLabel.text = stateName + " " + velocity.magnitude.ToString("00.00");
    }

    private void ChangeState(IPlayerState newState)
    {
        // use newState to tell player to change state. player should choose when to change it
        var oldState = state;
        Debug.Log("ChangeState: " + oldState.GetName() + " -> " + newState.GetName());
        oldState.OnDetach();

        state = newState;
        stateName = state.GetName();
        if (stateLabel)
        {
            stateLabel.text = stateName;
        }

        state.OnAttach();
    }

    internal void DoRun()
    {
        //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 input = new Vector2(runningDir, 0);
        float targetVelocityX = input.normalized.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);


        float augmentedGravity = state.GetGravity();

        velocity.y += augmentedGravity * Time.fixedDeltaTime;
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
        controller.Move(displacement * Time.fixedDeltaTime);
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
        grapple.time += Time.fixedDeltaTime;
        var angle = grapple.totalAngle * Mathf.Pow(Mathf.Sin(grapple.time / grapple.totalTIme), 2);
        //Debug.Log("time:" + (grapple.time / grapple.totalTIme) + " angle:" + angle);
        //Debug.DrawLine(grapple.transform.position, grapple.transform.position + grapple.extremePosition, Color.red);
        //Debug.DrawLine(grapple.transform.position, grapple.transform.position + grapple.startingPosition, Color.blue);
        //Debug.DrawLine(grapple.transform.position, grapple.transform.position + Quaternion.AngleAxis(-Mathf.Sign(grapple.extremePosition.x) * grapple.totalAngle, Vector3.forward) * grapple.extremePosition, Color.green);
        //Debug.DrawLine(grapple.transform.position, grapple.transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * grapple.extremePosition);


        var swingPos = Quaternion.AngleAxis(angle, Vector3.forward) * grapple.extremePosition;
        swingPos.z = 0;
        var displacement = grapple.transform.position + swingPos - transform.position;
        displacement.z = 0;
        velocity = displacement / Time.fixedDeltaTime;

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

    public void CreateGrapple(float x, float y)
    {
        var grappleInst = grappleThrower.CreateGrapple(x, y);
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
        public int str_x;
        public float x;
        public int str_y;
        public float y;
        public bool wasDown;
        public bool wasUp;
    }
}