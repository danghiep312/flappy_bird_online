using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private Player _player;

    private void OnValidate()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        _player = GetComponent<Player>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _player.IsAlive = false;
            SendCollideWithWall(_player.Id);
            if (!Player.CheckPlayerRemain())
            {
                GameLogic.IsPlaying = false;
                this.PostEvent(EventID.EndGame);
                this.PostEvent(EventID.DestroyStar, -1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Score"))
        {
            _player.Score++;
            ScoreServer.Scores[_player.Id].score = _player.Score;
            ScoreServer.Instance.SendScore();
        }

        if (other.CompareTag("Star"))
        {
            if (other.TryGetComponent(out Star star))
            {
                this.PostEvent(EventID.DestroyStar, star.ID);
            }
            _player.Score += 2;
            ScoreServer.Scores[_player.Id].score = _player.Score;
            ScoreServer.Instance.SendScore();
        }
    }

    #region Messages

    private void SendCollideWithWall(ushort toClientId)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.CollideWithWall);
        message.AddUShort(_player.Id);
        message.AddBool(_player.IsAlive);
        NetworkManager.Instance.Server.Send(message, toClientId);
    }

    #endregion
}
