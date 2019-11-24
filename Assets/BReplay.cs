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
    private List<Position> positions;
    private Replay replay;
    // Start is called before the first frame update
    void Start()
    {
        frames = new List<Frame>();
        positions = new List<Position>();
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
        positions = replay.positions.ToList();
        Debug.Log("StartPlaying(" + frames.Count + ")");
        frameID = 0;

        player.transform.position = new Vector3(0.001f * replay.startX, 0.001f * replay.startY, 0);
        playMode = PlayMode.REPLAY;
    }

    internal void HandleInput(ref Player.FixedMouseButtons fuMouse)
    {
        var player = GetComponent<Player>();
        switch (playMode)
        {
            case PlayMode.RECORD:
                if (fuMouse.wasDown || fuMouse.wasUp)
                {
                    if (frames.Count <= 0)
                    {
                        StartRecording(player);
                    }
                    var frame = new Frame(frameID, false, fuMouse);
                    Debug.Log("Record # " + frame.frameID + " (" + (frame.wasDown ? "Down" : "Up") + ") x=" + frame.x + " y=" + frame.y);
                    frames.Add(frame);

                    var pos = player.transform.position;
                    var velocity = player.velocity;
                    positions.Add(new Position(frameID, (int)(pos.x * 1000), (int)(pos.y * 1000), velocity.x.ToString(), velocity.y.ToString()));
                }
                break;
            case PlayMode.REPLAY:
                if (frames.Count() <= 0)
                {
                    fuMouse = new Player.FixedMouseButtons();
                }
                else
                {
                    var tail = frames.First();
                    var posTail = positions.First();
                    if (frameID != tail.frameID)
                    {
                        fuMouse = new Player.FixedMouseButtons();
                    }
                    else
                    {
                        Debug.Log("Replay # " + tail.frameID + " (" + (tail.wasDown ? "Down" : "Up") + ") x=" + tail.x + " y=" + tail.y);
                        fuMouse.str_x = tail.x;
                        fuMouse.str_y = tail.y;
                        fuMouse.wasDown = tail.wasDown;
                        fuMouse.wasUp = tail.wasUp;


                        var pos = player.transform.position;
                        var velocity = player.velocity;
                        var _pos = new Position(frameID, (int)(pos.x * 1000), (int)(pos.y * 1000), velocity.x.ToString(), velocity.y.ToString());

                        Debug.Log("Replay # " + posTail.frameID + " " +
                            "Position: x=" + _pos.x + " y=" + _pos.y + " vx=" + _pos.vx + " vy=" + _pos.vy + " | " +
                            "REC Position: x=" + posTail.x + " y=" + posTail.y + " vx=" + posTail.vx + " vy=" + posTail.vy +
                            "");

                        bool outSync = (
                            _pos.x != posTail.x ||
                            _pos.y != posTail.y ||
                            !String.Equals(_pos.vx, posTail.vx) ||
                            !String.Equals(_pos.vy, posTail.vy)
                        );

                        if (outSync)
                        {
                            Debug.Log("Out sync!");
                        }

                        frames.RemoveAt(0);
                        positions.RemoveAt(0);
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

    public Replay GetReplay()
    {
        replay.frames = frames.ToArray();
        replay.positions = positions.ToArray();
        replay.clearTime = frameID;
        return replay;
    }

    public ReplayInputs GetReplayInputs()
    {
        var replayInputs = new ReplayInputs();
        replayInputs.startX = replay.startX;
        replayInputs.startY = replay.startY;
        replayInputs.clearTime = frameID;
        replayInputs.frames = frames.ToArray();
        return replayInputs;
    }

    public ReplayPositions GetReplayPositions()
    {
        var replayPositions = new ReplayPositions();
        replayPositions.positions = positions.ToArray();
        return replayPositions;
    }

    public string ToJson()
    {
        var _replay = GetReplay();
        return JsonUtility.ToJson(_replay);
    }

    public Replay FromJson(string save)
    {
        return JsonUtility.FromJson<Replay>(save);
    }

    private static BReplay _inst;
    public static float FixedTime()
    {
        if (!_inst)
        {
            _inst = FindObjectOfType<BReplay>();
        }

        return FixedDeltaTime() * _inst.frameID;
    }
    public static float FixedDeltaTime()
    {
        return 0.02f;
    }


    [Serializable]
    public struct Replay
    {
        public int startX;
        public int startY;
        public int clearTime;
        public Frame[] frames;
        public Position[] positions;
    }
    [Serializable]
    public struct ReplayInputs
    {
        public int startX;
        public int startY;
        public int clearTime;
        public Frame[] frames;
    }
    [Serializable]
    public struct ReplayPositions
    {
        public Position[] positions;
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

    [Serializable]
    public struct Position
    {
        public Position(int frameID, int x, int y, string vx, string vy)
        {
            this.frameID = frameID;
            this.x = x;
            this.y = y;
            this.vx = vx;
            this.vy = vy;
        }
        public int frameID;
        public int x;
        public int y;
        public string vx;
        public string vy;
    }

}
