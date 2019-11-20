using UnityEngine;

public class BApple : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("GameObject2 collided with " + col.name);
        if (col.gameObject.tag == "Player")
        {
            FindObjectOfType<BAppleCounter>().AddApple();


            Destroy(gameObject);
        }
    }
}
