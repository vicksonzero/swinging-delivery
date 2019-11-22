using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Grapple : MonoBehaviour
{
    public float topSpeed;
    public float grappleLength;
    public Vector3 startingPosition;
    public Vector3 extremePosition;
    public Vector3 tangentVelocity;

    public float startTime;
    public float time;
    public float totalTIme;

    public float startingAngle;
    public float extremeAngle;
    public float totalAngle;

    public bool wasSwinging = false;

    public LineRenderer lineRenderer;

    public float shootingInterval;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.startColor = new Color(1, 1, 1, 0.5f);
        lineRenderer.endColor = new Color(1, 1, 1, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + extremePosition, Color.red);
        Debug.DrawLine(transform.position, transform.position + startingPosition, Color.blue);
        Debug.DrawLine(transform.position + startingPosition, transform.position + startingPosition + tangentVelocity, Color.blue);
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(totalAngle, Vector3.forward) * extremePosition, Color.green);
        //Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * extremePosition);

    }

    public bool IsShooting()
    {
        //Debug.Log("IsShooting " + (BReplay.FixedTime() - startTime) + " " + BReplay.FixedDeltaTime() + " " + BReplay.FixedTime() + " " + startTime);
        return BReplay.FixedTime() - startTime < shootingInterval;
    }

    public void InitSwing(Player player, Vector3 currentVelocity)
    {
        //var height = transform.position.y - grapple.transform.position.y + grapple.grappleLength;
        //grapple.pe = height * -gravity;// PE = mgh, m=1

        startingPosition = player.transform.position - transform.position;
        startingPosition.z = 0;
        grappleLength = startingPosition.magnitude;

        tangentVelocity = Vector3.ProjectOnPlane(player.velocity, startingPosition).normalized * player.velocity.magnitude;// * Time.fixedDeltaTime;
        startingAngle = -Mathf.Sign(startingPosition.x) * Vector3.Angle(Vector3.down, startingPosition);

        var pe = grappleLength - Mathf.Cos(startingAngle / 180 * Mathf.PI) * grappleLength; // let m, g be 1
        var ke = tangentVelocity.magnitude * tangentVelocity.magnitude / 2 / 20;
        var totalE = pe + ke;
        print("pe:" + pe + " ke:" + ke + " totalE:" + totalE);

        totalTIme = 1; // period is set to 3
        if (Mathf.Abs(startingAngle) > 90)
        {
            totalTIme = 1.5f;
        }

        extremeAngle = -Mathf.Sign(startingPosition.x) * Mathf.Acos(Mathf.Clamp((grappleLength - totalE) / grappleLength, -1, 1)) * 180 / Mathf.PI;
        totalAngle = extremeAngle * 2;

        //var projectedProgress = Mathf.Asin(Mathf.Sqrt(tangentVelocity.magnitude * Time.fixedDeltaTime / grappleLength));
        //print("projectedProgress:" + projectedProgress);
        //extremeAngle = (-(startingAngle / 180 * Mathf.PI) / projectedProgress) / Mathf.PI * 180;
        //totalAngle = extremeAngle * 2;

        //totalTIme = 3f;// totalAngle / 180 * Mathf.PI * grappleLength * shootingInterval;
        //startingAngle = Vector3.Angle(Vector3.down, startingPosition);
        //extremeAngle = Mathf.Asin(Mathf.Sqrt(tangentVelocity / grappleLength)) * totalTIme / Mathf.PI * 180; // /2 and *2 cancelled out
        //totalAngle = extremeAngle * 2;

        extremePosition = Quaternion.AngleAxis(extremeAngle, Vector3.back) * Vector3.down * grappleLength;



        time = Mathf.Asin(Mathf.Sqrt((extremeAngle - startingAngle) / (totalAngle))) * totalTIme;
        wasSwinging = false;

        var swingPos = startingPosition;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = new Color(1, 1, 1, 0.5f);
        lineRenderer.endColor = new Color(1, 1, 1, 0.5f);
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, swingPos);

        Debug.Log("Init: grappleLength=" + grappleLength + " startingAngle=" + startingAngle + " extremeAngle=" + extremeAngle + " totalAngle=" + totalAngle + " time=" + time);
    }
}
