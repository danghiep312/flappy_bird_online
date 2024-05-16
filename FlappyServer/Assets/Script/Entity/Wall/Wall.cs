
using System;
using Riptide;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public static ushort CurId = 1;
    public ushort ID { get; set; }

    public static Wall Spawn(ushort id, Vector3 pos)
    {
        Wall wall = Instantiate(GameLogic.Instance.WallPrefab, pos, Quaternion.identity).GetComponent<Wall>();
        wall.ID = id;
        wall.name = $"Wall id: {id}";
        wall.SendSpawned(pos);
        return wall;    
    }

    #region Messages

    private void SendSpawned(Vector3 pos)
    {
        Debug.Log("Send spawn");
        NetworkManager.Instance.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.WallSpawned), pos));
    }
    
    private Message AddSpawnData(Message message, Vector3 pos)
    {
        message.AddUShort(ID);
        message.AddVector3(pos);
        message.AddFloat(Time.time - GameLogic.StartTime);
        message.AddInt(Time.frameCount - GameLogic.StartFrame);
        return message;
    }
    

    #endregion
} 