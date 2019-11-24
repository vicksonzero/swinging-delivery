using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BContentful : MonoBehaviour
{
    public string baseUrl = "";
    public string cdnBaseUrl = "";
    public string spaceId = "";
    public string envId = "";

    public InputField nameField;
    public Button submitButton;
    public Text submitButtonText;

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
        //StartCoroutine(CreateAllScoreEntry());
    }
    private static IEnumerator<UnityWebRequestAsyncOperation> CreateScoreEntry(BReplay.ReplayInputs replayInputs)
    {
        var _inst = GetInstance();
        _inst.submitButton.interactable = false;
        if (_ContentfulKey.accessToken == "")
        {
            Debug.Log("no _ContentfulKey.accessToken found");
            _inst.submitButtonText.text = "Error: no Contentful accessToken";
            yield break;
        }

        Debug.Log("CreateScoreEntry1");

        string levelId = "5pYNHiPFQTDPo8dm1xQ2fY";
        ScoreData data = new ScoreData();
        data.playerName = new EnUSData<string>(_inst.nameField.text);
        data.gameVersion = new EnUSData<string>("0.4.0");
        data.bundleVersion = new EnUSData<int>(4);
        data.clearTime = new EnUSData<int>(replayInputs.clearTime);
        data.level = new EnUSData<SysData>(new SysData(levelId, "Link"));
        //data.replay = new ReplayData(replay);
        data.replayInputs = new EnUSData<BReplay.ReplayInputs>(replayInputs);

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

        string entryId;
        int oldVersion;
        if (request.error != null)
        {
            Debug.Log("Error: " + request.error + " Content: " + request.downloadHandler.text);
            _inst.submitButtonText.text = "Error: " + request.error;
            yield break;
        }
        else
        {

            var responseObj = JsonConvert.DeserializeObject<CreateScoreEntryResult>(request.downloadHandler.text);
            entryId = responseObj.sys.id;
            oldVersion = responseObj.sys.version;

            Debug.Log("Status Code: " + request.responseCode + "entryId:" + entryId + "oldVersion:" + oldVersion + " Content: " + request.downloadHandler.text);
        }

        var url2 = string.Format("{0}/spaces/{1}/environments/{2}/entries/{3}/published", _inst.baseUrl, _inst.spaceId, _inst.envId, entryId);
        UnityWebRequest request2 = new UnityWebRequest(url2, "PUT");
        //UploadHandler uploadHandler2 = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{\"fields\": " + JsonConvert.SerializeObject(data) + "}"));
        //request2.uploadHandler = uploadHandler2;
        request2.downloadHandler = new DownloadHandlerBuffer();

        //request.SetRequestHeader("Content-Type", "application/json");
        request2.SetRequestHeader("X-Contentful-Version", "" + (oldVersion));
        request2.SetRequestHeader("Authorization", "Bearer " + _ContentfulKey.accessToken);

        yield return request2.SendWebRequest();

        if (request2.error != null)
        {
            Debug.Log("Error: " + request2.error + " Content: " + request2.downloadHandler.text);
            _inst.submitButtonText.text = "Error2: " + request.error;
        }
        else
        {
            Debug.Log("request2 Status Code: " + request2.responseCode + " Content: " + request2.downloadHandler.text);

            _inst.submitButtonText.text = "Published";
        }
    }
    private static IEnumerator<UnityWebRequestAsyncOperation> CreateAllScoreEntry()
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
        public EnUSData<string> playerName;
        public EnUSData<SysData> level;
        public EnUSData<string> gameVersion;
        public EnUSData<int> bundleVersion;
        public EnUSData<int> clearTime;
        public EnUSData<BReplay.ReplayInputs> replayInputs;
        //public ReplayData replay;
    }
    [Serializable]
    struct EnUSData<T>
    {
        public EnUSData(T val)
        {
            enUS = val;
        }
        [JsonProperty("en-US")]
        public T enUS;
    }
    [Serializable]
    struct SysData
    {
        public SysData(string id, string linkType)
        {
            sys = new SysContentData(id, linkType);
        }
        public SysContentData sys;
    }
    [Serializable]
    struct SysContentData
    {
        public SysContentData(string id, string type)
        {
            this.type = type;
            this.id = id;
            linkType = "Entry";

        }
        public string type;
        public string linkType;
        public string id;
    }

    [Serializable]
    struct CreateScoreEntryResult
    {
        public CreateScoreEntryResultSys sys;
        public object fields;
    }
    [Serializable]
    struct CreateScoreEntryResultSys
    {
        public string id;
        public string type;
        public int version;
    }


}
