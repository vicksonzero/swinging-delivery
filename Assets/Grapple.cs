using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Grapple : MonoBehaviour
{
    public float topSpeed;
    public float grappleLength;
    public Vector3 startingPosition;

    public float startTime;
    public float time;
    public float totalTIme;
    public float startingAngle;
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
    }

    public bool isShooting()
    {
        return Time.time - startTime < shootingInterval;
    }

    public void InitSwing(Player player, Vector3 currentVelocity)
    {
        //var height = transform.position.y - grapple.transform.position.y + grapple.grappleLength;
        //grapple.pe = height * -gravity;// PE = mgh, m=1

        startingPosition = player.transform.position - transform.position;
        grappleLength = startingPosition.magnitude;

        startingAngle = Vector3.Angle(Vector3.down, startingPosition);
        totalAngle = startingAngle * 2;
        time = 0;
        totalTIme = 3f;// totalAngle / 180 * Mathf.PI * grappleLength * shootingInterval;
        wasSwinging = false;

        var swingPos = startingPosition;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = new Color(1, 1, 1, 0.5f);
        lineRenderer.endColor = new Color(1, 1, 1, 0.5f);
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, swingPos);

        Debug.Log("Init: grappleLength=" + grappleLength + " startingAngle=" + startingAngle + " totalAngle=" + totalAngle);
    }
}
