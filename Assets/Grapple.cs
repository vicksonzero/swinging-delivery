using UnityEngine;

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

    public Vector3 startingNormal;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitSwing(Player player)
    {
        //var height = transform.position.y - grapple.transform.position.y + grapple.grappleLength;
        //grapple.pe = height * -gravity;// PE = mgh, m=1

        startingPosition = player.transform.position - transform.position;
        grappleLength = startingPosition.magnitude;

        startingAngle = Vector3.Angle(Vector3.down, startingPosition);
        totalAngle = startingAngle * 2;
        time = 0;
        totalTIme = 3f;// totalAngle / 180 * Mathf.PI * grappleLength * 0.25f;
        wasSwinging = false;
        Debug.Log("Init: grappleLength=" + grappleLength + " startingAngle=" + startingAngle + " totalAngle=" + totalAngle);
    }
}
