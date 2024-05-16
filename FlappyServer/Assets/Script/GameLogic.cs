using System;
using Riptide;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class GameLogic : Singleton<GameLogic>
{
    public static bool IsPlaying;
    public static int StartFrame;
    public static float StartTime;

    public GameObject PlayerPrefab => playerPrefab;
    public GameObject WallPrefab => wallPrefab;

    public GameObject GroundPrefab => groundPrefab;

    public GameObject StarPrefab => starPrefab;

    [Header("Prefabs")] [Header("Prefabs")] [SerializeField]
    private GameObject playerPrefab;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject starPrefab;

    public bool tempPause;
    public bool isPausing;

    public float _countdown = 5;

    public override void Awake()
    {
        base.Awake();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 100;
    }

    private void Start()
    {
        this.RegisterListener(EventID.EndGame, OnEndGame);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.EndGame, OnEndGame);
    }

    private void OnEndGame(object obj)
    {
        SendEndGame();
    }


    public static void CheckAllPlayerReady()
    {
        foreach (var key in Player.list)
        {
            if (!Player.list[key.Key].IsReady) return;
        }

        IsPlaying = true;
        StartFrame = Time.frameCount;
        Spawner.Instance.Restart();

        Instance.SendPlaySignal(IsPlaying);
        StartTime = Time.time;
    }

    private void Update()
    {
        Application.targetFrameRate = 100;
        Time.timeScale = 0;
        if (tempPause)
        {
            _countdown -= Time.unscaledDeltaTime;

            if (CheckVoteAccept())
            {
                SendPauseAccepted(true);
                isPausing = true;
                tempPause = false;
                _countdown = 5f;
            }

            if (_countdown < 0)
            {
                SendPauseAccepted(false);
                _countdown = 5f;
                tempPause = false;
            }
        }


        if (isPausing)
        {
            if (CheckVoteAccept())
            {
                SendPauseAccepted(false);
                isPausing = false;
            }
        }

        if (tempPause || isPausing)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private bool CheckVoteAccept()
    {
        foreach (var key in Player.list.Keys)
        {
            if (Player.list.TryGetValue(key, out Player player))
            {
                if (!player.Vote) return false;
            }
        }

        return true;
    }


    public void SendEndGame()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.EndGame);
        message.AddFloat(Time.time - GameLogic.StartTime);
        message.AddInt(Time.frameCount - GameLogic.StartFrame);
        NetworkManager.Instance.Server.SendToAll(message);
    }

    private void SendPauseAccepted(bool isAccepted)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.PauseAccepted);
        message.AddBool(isAccepted);

        Player.ResetVote();

        NetworkManager.Instance.Server.SendToAll(message);
    }

    private void SendPlaySignal(bool status)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.Play);
        message.Add(status);

        NetworkManager.Instance.Server.SendToAll(message);
    }


    [Button]
    public void PlayTest()
    {
        Player player = Instantiate(playerPrefab, Vector3.up, quaternion.identity).GetComponent<Player>();
        Time.timeScale = 1;
        player.IsReady = true;
        IsPlaying = true;
    }

    [MessageHandler((ushort)ClientToServerId.Pause)]
    private static void Pause(ushort fromClientId, Message message)
    {
        if (IsPlaying)
        {
            if (Player.list.TryGetValue(fromClientId, out Player player))
            {
                bool requestPause = message.GetBool();
                Message response = Message.Create(MessageSendMode.Reliable, ServerToClientId.Vote);
                response.AddBool(requestPause);

                if (requestPause)
                {
                    if (!Instance.tempPause && !Instance.isPausing)
                    {
                        Instance.tempPause = true;
                        NetworkManager.Instance.Server.SendToAll(response);
                    }
                }
                else if (Instance.isPausing)
                {
                    NetworkManager.Instance.Server.SendToAll(response);
                }
            }
        }
    }
}