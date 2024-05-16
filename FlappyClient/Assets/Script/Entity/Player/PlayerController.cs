using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Riptide;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    private Player _player;
    
    private bool input;
    private bool pause;
    private Camera _cam;
    private bool _isAcceleration;

    private void Start()
    {
        _player = GetComponent<Player>();
        _cam = Camera.main;
        this.RegisterListener(EventID.Vote, OnVote);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.Vote, OnVote);
    }

    private void OnVote(object obj)
    {
        bool status = (bool)obj;
        Debug.Log("Vote " + status);
        SendVote(status);
    }

    private void Update()
    {
        if (!_player.IsAlive) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            input = true;
            SendInput();
            // GameLogic.Instance.PlayerClick(_player.Id);
            // _player.PlayerClick();
            input = false;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            SendPauseSignal(true);
        }

        if (Input.GetKeyDown(KeyCode.X) && GameLogic.Instance.IsPlaying)
        {
            _isAcceleration = !_isAcceleration;
            SendAccelerationSignal(_isAcceleration);
            //GameLogic.Instance.AddAcceleration(_player.Id);
        }

        if (Input.GetKeyDown(KeyCode.U) && GameLogic.Instance.IsPausing)
        {
            SendPauseSignal(false);
        }
       
        if (_player.IsLocal) _cam.transform.position = Vector3.right * (transform.position.x + 4f) + Vector3.forward * -10f;
        
    }
    

    #region Messages

    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.Input);

        message.AddBool(input);
        NetworkManager.Instance.Client.Send(message);
    }
    
    private void SendVote(bool vote)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.Vote);
        message.Add(vote);
        NetworkManager.Instance.Client.Send(message);
    }

    private void SendPauseSignal(bool signal)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.Pause);
        message.Add(signal);
        NetworkManager.Instance.Client.Send(message);
    }

    private void SendAccelerationSignal(bool signal)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.Acceleration);
        message.Add(signal);
        NetworkManager.Instance.Client.Send(message);
    }
    

    #endregion
    
    
}
