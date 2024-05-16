    using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public float speed;
    private Vector3 _startPos;
    public Transform FollowObject { get; set; }

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        if (!GameLogic.IsPlaying) return;
        transform.position -= speed * Time.deltaTime * Vector3.right;
        if (transform.position.x < 0) transform.position = _startPos;
        transform.position = Vector3.right * FollowObject.position.x + Vector3.up * -5f;
    }

    private void FixedUpdate()
    {
        if (!GameLogic.IsPlaying) return;
        //SendGroundPosition();
    }

    private void SendGroundPosition()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.GroundMove);
        message.AddVector3(transform.position);
        NetworkManager.Instance.Server.SendToAll(message);
    }
}
