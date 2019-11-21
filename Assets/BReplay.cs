using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class BReplay : MonoBehaviour
{
    public int frameID = 0;
    public PlayMode playMode = PlayMode.RECORD;
    public enum PlayMode { RECORD, REPLAY }

    private List<Frame> frames;
    private Replay replay;
    // Start is called before the first frame update
    void Start()
    {
        frames = new List<Frame>();
        replay = new Replay();
        if (PlayerPrefs.HasKey("save_replay"))
        {
            StartPlaying(PlayerPrefs.GetString("save_replay"), GetComponent<Player>());
            PlayerPrefs.DeleteKey("save_replay");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartRecording(Player player)
    {
        Debug.Log("StartRecording");
        replay = new Replay();
        replay.startX = (int)(player.transform.position.x * 1000);
        replay.startY = (int)(player.transform.position.y * 1000);

        player.transform.position = new Vector3(0.001f * replay.startX, 0.001f * replay.startY, 0);
        frames = new List<Frame>();
        frameID = 0;
        playMode = PlayMode.RECORD;
    }

    public void StartPlaying(string save, Player player)
    {
        replay = FromJson(save);
        frames = replay.frames.ToList();
        Debug.Log("StartPlaying(" + frames.Count + ")");
        frameID = 0;

        player.transform.position = new Vector3(0.001f * replay.startX, 0.001f * replay.startY, 0);
        playMode = PlayMode.REPLAY;
    }

    internal void HandleInput(ref Player.FixedMouseButtons fuMouse)
    {
        switch (playMode)
        {
            case PlayMode.RECORD:
                if (fuMouse.wasDown || fuMouse.wasUp)
                {
                    if (frames.Count <= 0)
                    {
                        StartRecording(GetComponent<Player>());
                    }
                    var frame = new Frame(frameID, false, fuMouse);
                    Debug.Log("Record # " + frame.frameID + " (" + (frame.wasDown ? "Down" : "Up") + ") x=" + frame.x + " y=" + frame.y);
                    frames.Add(frame);
                }
                break;
            case PlayMode.REPLAY:
                if (frames.Count() > 0)
                {
                    var tail = frames.First();
                    if (frameID == tail.frameID)
                    {
                        Debug.Log("Replay # " + tail.frameID + " (" + (tail.wasDown ? "Down" : "Up") + ") x=" + tail.x + " y=" + tail.y);
                        fuMouse.str_x = tail.x;
                        fuMouse.str_y = tail.y;
                        fuMouse.wasDown = tail.wasDown;
                        fuMouse.wasUp = tail.wasUp;
                        frames.RemoveAt(0);
                        if (frames.Count() <= 0)
                        {
                            Debug.Log("Replay Ended.");
                        }
                    }
                }
                break;
        }
        frameID++;
    }

    public string ToJson()
    {
        replay.frames = frames.ToArray();
        return JsonUtility.ToJson(replay);
    }

    public Replay FromJson(string save)
    {
        return JsonUtility.FromJson<Replay>(save);
    }


    [Serializable]
    public struct Replay
    {
        public int startX;
        public int startY;
        public Frame[] frames;
    }

    [Serializable]
    public struct Frame
    {
        public Frame(int frameID, bool isEnd, Player.FixedMouseButtons fuMouse)
        {
            this.frameID = frameID;
            this.isEnd = isEnd;
            x = fuMouse.str_x;
            y = fuMouse.str_y;
            wasDown = fuMouse.wasDown;
            wasUp = fuMouse.wasUp;
        }
        public int frameID;
        public bool isEnd;
        public int x;
        public int y;
        public bool wasDown;
        public bool wasUp;
    }
}
