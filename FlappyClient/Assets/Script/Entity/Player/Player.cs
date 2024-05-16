using System;
using System.Collections.Generic;
using Riptide;
using UnityEngine;
using Random = System.Random;

public class Player : MonoBehaviour
{
    // List player in server
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    
    //public Recorder recorder;

    public bool IsAlive { get; set; }
    
    public ushort Id { get; private set; }
    public bool IsLocal { get; set; }
    public string Username { get; set; }
    
    
    private void OnDestroy()
    {
        list.Remove(Id);
        // this.RemoveListener(EventID.PlayGame, OnPlayGame);
        // this.RemoveListener(EventID.WallSpawned, OnWallSpawned);
    }

    private void Start()
    {
        // this.RegisterListener(EventID.PlayGame, OnPlayGame);
        // this.RegisterListener(EventID.WallSpawned, OnWallSpawned);
    }

    public static void Spawn(ushort id, string username, Vector3 position)
    {
        Player player;
        if (id == NetworkManager.Instance.Client.Id)
        {
            player = Instantiate(GameLogic.Instance.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Instance.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }
        player.IsAlive = true;
        player.name = $"Player {id} (username)";
        player.Id = id;
        player.Username = username;
        
        list.Add(id, player);

        Scoreboard.Instance.AddRecord(id, new PlayerScore(username, 0));
        GameLogic.Instance.SpawnPlayer(player);
    }
    
    // public void PlayerClick()
    // {
    //     // record.AddClick(Time.time - _startTime);
    //     recorder.AddClick(Time.frameCount - _startFrame);
    // }
    //
    // public void OnWallSpawned(object obj)
    // {
    //     Vector3 pos = (Vector3) obj;
    //     recorder.AddWallInfo(pos.y, Time.frameCount - _startFrame);
    // }

    private void Move(Vector3 newPos)
    {
        transform.position = newPos;
    }

    #region Messages

    [MessageHandler((ushort)ServerToClientId.PlayerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
    
    [MessageHandler((ushort)ServerToClientId.PlayerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.Move(message.GetVector3());
    }

    [MessageHandler((ushort)ServerToClientId.CollideWithWall)]
    private static void PlayerDead(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.IsAlive = message.GetBool();
        }
    }

    [MessageHandler((ushort)ServerToClientId.ResponseInput)]
    private static void ResponseInput(Message message)
    {
        ushort playerId = message.GetUShort();
        if (Player.list.TryGetValue(playerId, out Player player))
        {
            GameLogic.Instance.PlayerClick(playerId, message.GetFloat());
        }
    }


    [MessageHandler((ushort)ServerToClientId.ResponseAcceleration)]
    private static void ResponseAcceleration(Message message)
    {
        ushort playerId = message.GetUShort();
        if (Player.list.TryGetValue(playerId, out Player player))
        {
            GameLogic.Instance.AddAcceleration(playerId, message.GetFloat());
        }
    }

    
    #endregion
}