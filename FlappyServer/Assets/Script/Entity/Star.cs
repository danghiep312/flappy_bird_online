using System;
using Riptide;
using Unity.Mathematics;
using UnityEngine;

public class Star : MonoBehaviour
{
    public static int Id = 0;

    public int ID { get; set; }

    private void Start()
    {
        this.RegisterListener(EventID.DestroyStar, OnDestroyStar);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.DestroyStar, OnDestroyStar);
    }

    private void OnDestroyStar(object obj)
    {
        if ((int)obj == -1)
        {
            Destroy(gameObject);
        }
        else if (ID == (int) obj)
        {
            SendDestroyed();
            Destroy(gameObject);
        }
    }

    public static void Spawn(Vector3 pos)
    {
        Star star = Instantiate(GameLogic.Instance.StarPrefab, pos, quaternion.identity).GetComponent<Star>();
        star.ID = Id++;
        SendSpawnStar(star);
    }
    
    private static void SendSpawnStar(Star star)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.StarSpawned);
        message.AddInt(star.ID);
        message.AddVector3(star.transform.position);
        message.AddFloat(Time.time - GameLogic.StartTime);
        message.AddInt(Time.frameCount - GameLogic.StartFrame);
        NetworkManager.Instance.Server.SendToAll(message);
    }
    
    public void SendDestroyed()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.StarDestroyed);
        message.AddInt(ID);
        NetworkManager.Instance.Server.SendToAll(message);
    }
}