using System;
using System.Collections.Generic;
using Riptide;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }
    
    public int Score { get; set; }
    public bool IsAlive { get; set; }
    
    public bool IsReady { get; set; }
    
    public bool Vote { get; set; }
    
    public bool Acceleration { get; set; }

    public Ground follower;
    
    public PlayerMovement Movement => movement;

    [SerializeField] private PlayerMovement movement;

    private void Start()
    {
        IsAlive = true;
    }

    private void OnDestroy()
    {
        if (follower) DestroyImmediate(follower.gameObject);
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username)
    {
        // send other data player to player id
        foreach (Player otherPlayer in list.Values)
        {
            otherPlayer.SendSpawned(id);
        }
        // spawn player
        Player player = Instantiate(GameLogic.Instance.PlayerPrefab, Vector3.up + Vector3.right * -4f, Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(String.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;

        player.SendSpawned();
        list.Add(id, player);
        
        Ground ground = Instantiate(GameLogic.Instance.GroundPrefab, Vector3.up * -5f, Quaternion.identity).GetComponent<Ground>();
        ground.FollowObject = player.transform;

        player.follower = ground;
        
        ScoreServer.Scores.Add(id, new PlayerScore(username, 0));
        ScoreServer.Instance.SendScore();
    }

    public static bool CheckPlayerRemain()
    {
        foreach (ushort key in list.Keys)
        {
            if (list[key].IsAlive) return true;
        }

        return false;
    }
    
    
    public static void ResetVote()
    {
        foreach (var key in list.Keys)
        {
            if (list.TryGetValue(key, out Player player))
            {
                player.Vote = false;
            }
        }
    }

    

    #region Message Handler

    [MessageHandler((ushort)ClientToServerId.Name)]
    private static void GetUsernameFromClient(ushort clientId, Message message)
    {
        string username = message.GetString();
        if (string.IsNullOrEmpty(username)) username = $"Guest {clientId}";
        Spawn(clientId, username);
    }

    
    [MessageHandler((ushort)ClientToServerId.Input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            if (player.IsReady)
            {
                if (!player.IsAlive) return;
                player.Movement.SetInput(message.GetBool());
                player.ResponseInput();
            }
        }
    }


    [MessageHandler((ushort)ClientToServerId.Ready)]
    private static void Ready(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.IsReady = message.GetBool();
            GameLogic.CheckAllPlayerReady();
        }
    }


    [MessageHandler((ushort)ClientToServerId.Vote)]
    private static void GetVote(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            Debug.Log("Get vote");
            player.Vote = message.GetBool();
        }
    }


    [MessageHandler((ushort)ClientToServerId.Acceleration)]
    private static void GetAcceleration(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            Debug.Log("Get Acceleration");
            player.Acceleration = message.GetBool();
            player.ResponseAcceleration();
        }
    }
    #endregion
    
    #region Messages

    /// <summary>
    /// Send event to all client is connecting
    /// </summary>
    public void SendSpawned()
    {
        NetworkManager.Instance.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.PlayerSpawned)));
    }
    
    
    /// <summary>
    /// Send to client Id
    /// </summary>
    /// <param name="toClientId">Id of player will receive message</param>
    public void SendSpawned(ushort toClientId)
    {
        NetworkManager.Instance.Server.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.PlayerSpawned)), toClientId);
    }


    /// <summary>
    /// Add id, username, position to Message
    /// </summary>
    /// <param name="message">object need add data</param>
    /// <returns></returns>
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }
    
    
    public void ResponseInput()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.ResponseInput);
        message.AddUShort(Id);
        message.AddFloat(Time.time - GameLogic.StartTime);
        message.AddInt(Time.frameCount - GameLogic.StartFrame);
        Debug.Log(Time.time - GameLogic.StartTime);
        NetworkManager.Instance.Server.SendToAll(message);
    }

    public void ResponseAcceleration()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.ResponseAcceleration);
        message.AddUShort(Id);
        message.AddFloat(Time.time - GameLogic.StartTime);
        message.AddBool(Acceleration);
        message.AddInt(Time.frameCount - GameLogic.StartFrame);
        NetworkManager.Instance.Server.SendToAll(message);
    }
    
    #endregion

}