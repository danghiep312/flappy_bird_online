using System;
using Riptide;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameLogic : Singleton<GameLogic>
{
    public GameObject LocalPlayerPrefab => localPlayerPrefab;
    public GameObject PlayerPrefab => playerPrefab;
    public GameObject WallPrefab => wallPrefab;
    
    public GameObject StarPrefab => starPrefab;

    public bool IsPausing { get; set; }

    public bool IsPlaying { get; set; }
    
    public bool IsReplaying { get; set; }
    
    // private float _startTime;
    private int _startFrame;

    public Recorder recorder;


    [Header("Prefabs")]
    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject starPrefab;

    public void PlayGame()
    {
        IsPlaying = true;
        _startFrame = Time.frameCount;
        this.PostEvent(EventID.PlayGame);
    }

    public void PlayerClick(int playerId, int frameTime)
    {
        // record.AddClick(Time.time - _startTime);
        recorder.AddClick(playerId, frameTime);
    }
    
    public void PlayerClick(int playerId, float frameTime)
    {
        // record.AddClick(Time.time - _startTime);
        recorder.AddClick(playerId, frameTime);
    }
    
    public void OnWallSpawned(Vector3 pos, int frame)
    {
        recorder.AddWallInfo(new Position(pos.x, pos.y, pos.z), frame);
    }

    public void OnWallSpawned(Vector3 pos, float frame)
    {
        recorder.AddWallInfo(new Position(pos.x, pos.y, pos.z), frame);
    }
    
    public void OnStarSpawned(Vector3 pos, int frame)
    {
        recorder.AddStarInfo(pos, frame);
    }
    
    public void OnStarSpawned(Vector3 pos, float frame)
    {
        recorder.AddStarInfo(pos, frame);
    }
    
    public void SpawnPlayer(Player player)
    {
        recorder.AddPlayer(player.Id, player.Username);
    }
    
    public void AddAcceleration(ushort playerId, int frameTime)
    {
        recorder.AddScaleTime(playerId, frameTime);
    }
    
    public void AddAcceleration(ushort playerId, float frameTime)
    {
        recorder.AddScaleTime(playerId, frameTime);
    }

    public void OnEndGame(int frameTime)
    {
        recorder.EndSignal = frameTime;
    }

    public void OnEndGame(float endTime)
    {
        recorder.EndTime = endTime;
    }

    [Button]
    public string test(string pattern)
    {
        return DateTime.Now.ToString(pattern);
    }

    [Button]
    public void OpenFolder()
    {
        Application.OpenURL(Application.persistentDataPath);
    }


    #region Message handler

    [MessageHandler((ushort)ServerToClientId.Vote)]
    public static void ShowVote(Message message)
    {
        Time.timeScale = 0f;
        bool pause = message.GetBool();
        
        UIManager.Instance.ShowVote(pause ? "Pause" : "Unpause");
    }

    [MessageHandler((ushort)ServerToClientId.PauseAccepted)]
    public static void GetPauseSignal(Message message)
    {
        bool pause = message.GetBool();
        Debug.Log("Pause " + pause);
        Instance.IsPausing = pause;
        Time.timeScale = pause ? 0 : 1;
        UIManager.Instance.ShowPause(pause);
    }

    [MessageHandler((ushort)ServerToClientId.Play)]
    public static void AllPlayerReady(Message message)
    {
        bool status = message.GetBool();
        if (status) Instance.PlayGame();
    }


    [MessageHandler((ushort)ServerToClientId.EndGame)]
    public static void EndGame(Message message)
    {
        GameLogic.Instance.OnEndGame(message.GetFloat());
        Instance.PostEvent(EventID.EndGame);
    }

    #endregion

}