using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BAppleCounter : MonoBehaviour
{
    public Text label;
    public int appleCount = 0;
    public int appleTotal;
    public float startTime = -1;
    public float endTime = -1;
    public GameObject titleScreen;

    public Button retryButton;

    public RectTransform resultPanel;
    public InputField replayBox;

    private BReplay replay;

    // Start is called before the first frame update
    void Start()
    {
        appleTotal = FindObjectsOfType<BApple>().Length;
        //retryButton.gameObject.SetActive(false);
        replay = FindObjectOfType<BReplay>();
    }

    public void StartTImer()
    {
        if (startTime == -1)
        {
            startTime = BReplay.FixedTime();
            titleScreen.SetActive(false);
        }
    }

    public void EndTImer()
    {
        if (endTime == -1)
        {
            endTime = BReplay.FixedTime();
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
        var time = startTime == -1 ? 0 : endTime == -1 ? BReplay.FixedTime() - startTime : endTime - startTime;
        label.text = "Apples found: " + appleCount + " / " + appleTotal + "\nTime: " + time.ToString("0.00");

    }


    public void ShowResult()
    {
        var save = replay.ToJson();
        replayBox.SetTextWithoutNotify(save);
        PlayerPrefs.SetString("save_replay", save);
        resultPanel.gameObject.SetActive(true);
    }

    public void HideResult()
    {
        resultPanel.gameObject.SetActive(false);
    }
}
