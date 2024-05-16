using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Transports.Tcp;
using Riptide.Utils;

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
    public Client Client;
    
    public void Start()
    {
        Application.targetFrameRate = 90;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft; // other player left server
        Client.Disconnected += DidDisconnect; // this player left server 
        
    }

    public void Connect(string ip, ushort port)
    {
        Client.Connect($"{ip}:{port}");
    }

    private void Update()
    {
        Client.Update();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        UIManager.Instance.BackToMain();
        List<Player> curplayer = new List<Player>();
        foreach (var key in Player.list.Keys)
        {
            if (Player.list.TryGetValue(key, out Player player))
            {
                curplayer.Add(player);
            }
        }

        foreach (var player in curplayer)
        {
            DestroyImmediate(player.gameObject);
        }
        Wall.DestroyAllWall();
        this.PostEvent(EventID.StarDestroyed, -1);
    }
    
    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Id, out Player player))
        {
            DestroyImmediate(player.gameObject);
        }
    }

    private void FailedToConnect(object sender, ConnectionFailedEventArgs e)
    {
        UIManager.Instance.BackToMain();
    }

    private void DidConnect(object sender, EventArgs e)
    {
        UIManager.Instance.SendName();
    }

    public void Disconnect()
    {
        Client.Disconnect();
    }
}
