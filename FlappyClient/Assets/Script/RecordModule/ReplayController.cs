using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ReplayController : Singleton<ReplayController>
{
    [ShowInInspector] public Dictionary<int, PlayerMovement> recordPlayer = new();
    [ShowInInspector] public Dictionary<int, PlayerScore> scores;
    public string[] recordPath;
    public Recorder record;

    [Header("Record")] public GameObject recordLocalPlayerPrefab;
    public GameObject recordPlayerPrefab;

    [Space] public GameObject recordPrefab;
    public Transform content;

    private int _startFrame;
    private float _startTime;

    private float _time;

    [Button]
    public void LoadTest(string path)
    {
        record = Util.LoadRecord(path);
    }

    public override void Awake()
    {
        base.Awake();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 100;
    }

    public void Start()
    {
        recordPath = PlayerPrefs.GetString("record", "").Split(",");

        foreach (string path in recordPath)
        {
            if (string.IsNullOrEmpty(path)) continue;
            RecordUI rec = Instantiate(recordPrefab, content).GetComponent<RecordUI>();
            rec.SetPath(path);
        }

        this.RegisterListener(EventID.GoToRecord, GoToRecord);
    }

    private void GoToRecord(object obj)
    {
        scores = new Dictionary<int, PlayerScore>();
        
        GameLogic.Instance.IsReplaying = true;
        UIManager.Instance.ShowReplay(true);

        foreach (var playerId in record.clickTime.Keys)
        {
            PlayerMovement player =
                Instantiate(record.CheckLocal(playerId) ? recordLocalPlayerPrefab : recordPlayerPrefab,
                        Vector3.up + Vector3.right * -4f, Quaternion.identity)
                    .GetComponent<PlayerMovement>();

            player.IsLocal = record.CheckLocal(playerId);
            player.Id = playerId;
            recordPlayer.Add(playerId, player);
        }

        foreach (var userId in record.user.Keys)
        {
            scores.Add(userId, new PlayerScore(record.user[userId], 0));
            Scoreboard.Instance.AddRecord(userId, scores[userId]);
        }
        
        _startFrame = Time.frameCount;
        _startTime = Time.time;
        _time = 0;
    }

    private void Update()
    {
        if (GameLogic.Instance.IsReplaying)
        {
            _time += Time.deltaTime;
            Scoreboard.Scores = scores;
            foreach (var playerId in recordPlayer.Keys)
            {
                if (record.clickTime.TryGetValue(playerId, out List<float> clickValue))
                {
                    if (clickValue.Count > 0 && clickValue[0] <= _time)
                    {
                        recordPlayer[playerId].Jump();
                        clickValue.RemoveAt(0);
                    }
                }


                if (record.scaleTime.TryGetValue(playerId, out List<float> val))
                {
                    // if (val.Contains(frame)) recordPlayer[playerId].Acceleration();
                    if (val.Count > 0 && val[0] <= _time)
                    {
                        recordPlayer[playerId].Acceleration();
                        val.RemoveAt(0);
                    }
                }
            }

            // if (record.wallTime.Contains(frame))
            // {
            //     SpawnWall(record.wallTime.IndexOf(frame));
            // }
            //
            // if (record.starTime.Contains(frame))
            // {
            //     SpawnStar(record.starTime.IndexOf(frame));
            // }
            //
            if (record.wallTime.Count > 0 && record.wallTime[0] <= _time)
            {
                SpawnWall(0);
                record.wallPos.RemoveAt(0);
                record.wallTime.RemoveAt(0);
            }
            
            if (record.starTime.Count > 0 && record.starTime[0] <= _time)
            {
                SpawnStar(0);
                record.starPos.RemoveAt(0);
                record.starTime.RemoveAt(0);
            }
            
            if (record.CheckEndRecord(_time))
            {
                Time.timeScale = 0;
                GameLogic.Instance.IsReplaying = false;
                UIManager.Instance.ShowEndRecord(true);
            }

        }
    }

    public void EndRecord()
    {
        this.PostEvent(EventID.EndRecord, -1);
    }


    private void SpawnWall(int index)
    {
        Spawner.Instance.Spawn(ParseVector3(record.wallPos[index]));
    }

    private void SpawnStar(int index)
    {
        Spawner.Instance.SpawnStar(ParseVector3(record.starPos[index]));
    }

    public Vector3 ParseVector3(Position pos)
    {
        return new Vector3(pos.x, pos.y, pos.z);
    }
}