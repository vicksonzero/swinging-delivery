using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BContentful : MonoBehaviour
{
    public string baseUrl = "";
    public string spaceId = "";
    public string envId = "";

    public InputField nameField;
    private static BContentful _inst;
    public static BContentful GetInstance()
    {
        if (!_inst)
        {
            _inst = FindObjectOfType<BContentful>();
        }
        return _inst;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CreateScoreEntry()
    {
        Debug.Log("CreateScoreEntry0");
        var replayJsonString = FindObjectOfType<BReplay>().ToJson();
        //StartCoroutine(CreateScoreEntry(replayJsonString));
        StartCoroutine(CreateScoreEntry(FindObjectOfType<BReplay>().GetReplayInputs()));
        //StartCoroutine(GetAllContentTypes());
        //StartCoroutine(CreateAllScoreEntry(replayJsonString));
    }
    private static IEnumerator<UnityWebRequestAsyncOperation> CreateScoreEntry(BReplay.ReplayInputs replayInputs)
    {
        if (_ContentfulKey.accessToken == "")
        {
            Debug.Log("no _ContentfulKey.accessToken found");
            yield break;
        }

        Debug.Log("CreateScoreEntry1");
        var _inst = GetInstance();

        ScoreData data = new ScoreData();
        data.playerName = new StringData(_inst.nameField.text);
        data.gameVersion = new StringData("0.3.0");
        data.clearTime = new IntData(replayInputs.clearTime);
        //data.replay = new ReplayData(replay);
        data.replayInputs = new ReplayData(replayInputs);

        Debug.Log(JsonConvert.SerializeObject(data));

        var url = string.Format("{0}/spaces/{1}/environments/{2}/entries", _inst.baseUrl, _inst.spaceId, _inst.envId);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        UploadHandler uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{\"fields\": " + JsonConvert.SerializeObject(data) + "}"));
        request.uploadHandler = uploadHandler;
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/vnd.contentful.management.v1+json");
        request.SetRequestHeader("X-Contentful-Content-Type", "score");
        //request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + _ContentfulKey.accessToken);

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log("Error: " + request.error + " Content: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode + " Content: " + request.downloadHandler.text);
        }

    }
    private static IEnumerator<UnityWebRequestAsyncOperation> CreateAllScoreEntry(string replayJsonString)
    {

        if (_ContentfulKey.accessToken == "")
        {
            Debug.Log("no _ContentfulKey.accessToken found");
            yield break;
        }
        Debug.Log("CreateAllScoreEntry");
        if (!_inst)
        {
            _inst = FindObjectOfType<BContentful>();
        }

        //PostData data = new PostData();
        //data.Add("playerName", "Chan Tai Man");
        //data.Add("replay", replayJsonString);
        //data.Add("gameVersion", "0.3.0"); // TODO: remove before launch

        //Debug.Log(data.toJsonString());

        var url = String.Format("{0}/spaces/{1}/environments/{2}/entries", _inst.baseUrl, _inst.spaceId, _inst.envId);
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        //UploadHandler uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data.toJsonString()));
        //request.uploadHandler = uploadHandler;
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/vnd.contentful.management.v1+json");
        request.SetRequestHeader("X-Contentful-Content-Type", "score");
        //request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + _ContentfulKey.accessToken);

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode + " Content: " + request.downloadHandler.text);
        }

    }
    private static IEnumerator<UnityWebRequestAsyncOperation> GetAllContentTypes()
    {
        if (_ContentfulKey.accessToken == "")
        {
            Debug.Log("no _ContentfulKey.accessToken found");
            yield break;
        }
        Debug.Log("CreateAllScoreEntry");
        if (!_inst)
        {
            _inst = FindObjectOfType<BContentful>();
        }

        //PostData data = new PostData();
        //data.Add("playerName", "Chan Tai Man");
        //data.Add("replay", replayJsonString);
        //data.Add("gameVersion", "0.3.0"); // TODO: remove before launch

        //Debug.Log(data.toJsonString());

        var url = String.Format("{0}/spaces/{1}/environments/{2}/content_types", _inst.baseUrl, _inst.spaceId, _inst.envId);
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        //UploadHandler uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data.toJsonString()));
        //request.uploadHandler = uploadHandler;
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/vnd.contentful.management.v1+json");
        //request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + _ContentfulKey.accessToken);

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode + " Content: " + request.downloadHandler.text);
        }

    }


    [Serializable]
    struct ScoreData
    {
        public StringData playerName;
        public StringData gameVersion;
        public IntData clearTime;
        public ReplayData replayInputs;
        //public ReplayData replay;
    }
    [Serializable]
    struct IntData
    {
        public IntData(int val)
        {
            enUS = val;
        }
        [JsonProperty("en-US")]
        public int enUS;
    }
    [Serializable]
    struct StringData
    {
        public StringData(string val)
        {
            enUS = val;
        }
        [JsonProperty("en-US")]
        public string enUS;
    }
    [Serializable]
    struct ReplayData
    {
        public ReplayData(BReplay.ReplayInputs val)
        {
            enUS = val;
        }
        [JsonProperty("en-US")]
        public BReplay.ReplayInputs enUS;
    }
    [Serializable]
    struct FrameData
    {
        public FrameData(BReplay.Frame[] val)
        {
            enUS = val;
        }
        [JsonProperty("en-US")]
        public BReplay.Frame[] enUS;
    }
    [Serializable]
    struct PositionData
    {
        public PositionData(BReplay.Position[] val)
        {
            enUS = val;
        }
        [JsonProperty("en-US")]
        public BReplay.Position[] enUS;
    }
}
