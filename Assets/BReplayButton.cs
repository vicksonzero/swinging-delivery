using UnityEngine;
using UnityEngine.UI;

public class BReplayButton : MonoBehaviour
{
    public Text playerNameLabel;
    public Text levelLabel;
    public Text clearTimeLabel;
    public BContentful.ScoreData save;

    public delegate void OnButtonClickedDelegate();
    public OnButtonClickedDelegate onButtonClicked;

    // Start is called before the first frame update

    public void SetInfo(string playerName, string level, string clearTime, BContentful.ScoreData save)
    {
        playerNameLabel.text = playerName;
        levelLabel.text = level;
        clearTimeLabel.text = clearTime;
        this.save = save;
    }

    public void OnButtonClicked()
    {
        FindObjectOfType<BChooseReplayPanel>().gameObject.SetActive(false);
        FindObjectOfType<BReplay>().StartPlaying(save, FindObjectOfType<Player>());
    }
}
