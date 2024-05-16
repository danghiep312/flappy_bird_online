
using System;
using Riptide;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    public float Speed => speed;
    [SerializeField] private float speed;
    [SerializeField] private float boundary;

    [SerializeField] private Wall wall;
#if UNITY_EDITOR
    private void OnValidate()
    {
        wall = GetComponent<Wall>();
    }
#endif

    private void Start()
    {
        wall = GetComponent<Wall>();
    }

    // private void Update()
    // {
    //     if (!GameLogic.IsPlaying) return;
    //     // transform.position -= Vector3.right * (speed * Time.deltaTime);
    //     // if (transform.position.x < boundary)
    //     // {
    //     //     Spawner.Instance.DeleteWall(wall);
    //     //     Destroy(gameObject);
    //     // }
    // }

    private void FixedUpdate()
    {
        SendMovement();
    }

    private void OnDestroy()
    {
        SendDestroyed();
    }

    
    #region Messages
    private void SendDestroyed()
    {
        NetworkManager.Instance.Server.SendToAll(
            AddDestroyedData(Message.Create(MessageSendMode.Reliable, ServerToClientId.WallDestroyed)));
    }

    private void SendMovement()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.WallMovement);
        message.AddUShort(wall.ID);
        message.AddVector3(transform.position);
        NetworkManager.Instance.Server.SendToAll(message);
    }

    private Message AddDestroyedData(Message message)
    {
        message.AddUShort(wall.ID);
        return message;
    }
    
    #endregion
}