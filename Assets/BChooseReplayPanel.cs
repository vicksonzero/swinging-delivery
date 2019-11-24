using System.Linq;
using UnityEngine;

public class BChooseReplayPanel : MonoBehaviour
{
    public BReplayButton replayButtonPrefab;
    private BContentful.EntryData<BContentful.LevelData>[] links;
    private BContentful.EntryData<BContentful.ScoreData>[] items;
    public RectTransform contentGroup;

    private float padding = 7;

    void Start()
    {
    }

    public void OnShowPanel()
    {
        StartCoroutine(BContentful.CdnGetAllScores());
    }

    // Start is called before the first frame update

    public void PopulateReplayList(BContentful.GetScoreEntriesResult response)
    {
        Debug.Log("PopulateReplayList");
        links = response.includes.Entry;

        items = response.items;

        foreach (Transform child in contentGroup.transform)
        {
            Destroy(child.gameObject);
        }

        var contentRt = contentGroup.GetComponent<RectTransform>();
        contentRt.sizeDelta = new Vector2(contentRt.sizeDelta.x, 40 * items.Length + padding + padding);

        for (int i = 0; i < items.Length; i++)
        {
            var entry = items[i];
            var replayButton = Instantiate(replayButtonPrefab);

            replayButton.transform.SetParent(contentGroup, true);
            replayButton.transform.localScale = Vector3.one;

            var levelLink = entry.fields.level;
            var level = links.ToList().Find((link) => link.sys.id == levelLink.sys.id);

            replayButton.SetInfo(
                entry.fields.playerName,
                level.fields.name,
                "" + (0.02f * entry.fields.clearTime) + "s",
                entry.fields
            );

            var rectTransform = replayButton.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(
                padding,
                -padding - i * 40
            );
            //rectTransform.offsetMin = new Vector2(-7, -padding - i * 40);
            rectTransform.offsetMax = new Vector2(-7, -padding - i * 40);
        }
    }
}
