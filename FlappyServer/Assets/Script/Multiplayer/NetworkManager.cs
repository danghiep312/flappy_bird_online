using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public enum ServerToClientId : ushort
{
    PlayerSpawned = 0,
    PlayerMovement = 1,
    WallMovement = 2,
    WallDestroyed = 3,
    WallSpawned = 4,
    GroundMove = 5,
    CollideWithWall = 6,
    Vote = 7,
    PauseAccepted = 8,
    Play = 9,
    EndGame = 10,
    StarSpawned = 11,
    StarDestroyed = 12,
    ResponseInput = 13,
    ResponseAcceleration = 14,
}

public enum ClientToServerId : ushort
{
    Name = 0,
    Input = 1,
    Ready = 2,
    Pause = 3,
    Vote = 4,
    Acceleration = 5,
}

public class NetworkManager : Singleton<NetworkManager>
{
    public Server Server { get; private set; }

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server("Flappy Server");
        Server.Start(port, maxClientCount);

        Server.ClientConnected += OnClientConnect;
        Server.ClientDisconnected += OnClientDisconnect;
    }

    private void OnClientDisconnect(object sender, ServerDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Client.Id, out Player player))
        {
            Debug.Log("Player " + e.Client.Id + " disconnected");
            DestroyImmediate(player.gameObject);
            if (Player.list.Count == 0)
            {
                Debug.Log("Destroy wall");
                Spawner.Instance.DeleteAll();
            }
        }
    }

    private void OnClientConnect(object sender, ServerConnectedEventArgs e)
    {
        ScoreServer.Instance.SendScore();
    }

    private void Update()
    {
        Server.Update();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    
}
