using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BAppleCounter : MonoBehaviour
{
    public Text label;
    public int appleCount = 0;
    public int appleTotal;
    public float startTime = 0;
    public float endTime = 0;
    public GameObject titleScreen;

    public Button retryButton;

    // Start is called before the first frame update
    void Start()
    {
        appleTotal = FindObjectsOfType<BApple>().Length;
        //retryButton.gameObject.SetActive(false);
    }

    public void StartTImer()
    {
        if (startTime == 0)
        {
            startTime = Time.time;
            titleScreen.SetActive(false);
        }
    }

    public void EndTImer()
    {
        if (endTime == 0)
        {
            endTime = Time.time;
        }
    }

    public void AddApple()
    {

        appleCount++;
        if (appleCount >= appleTotal)
        {
            EndTImer();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void Update()
    {
        var time = startTime == 0 ? 0 : endTime == 0 ? Time.time - startTime : endTime - startTime;
        label.text = "Apples found: " + appleCount + " / " + appleTotal + "\nTime: " + time.ToString("0.00");
    }
}
