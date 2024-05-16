using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private static Ground Instance;
    private Camera _cam;
    private float offsetY = -5f;

    private void Awake()
    {
        Instance = this;
        _cam = Camera.main;
    }

    private void Start()
    {
        this.RegisterListener(EventID.GoHome, OnGoHome);
        this.RegisterListener(EventID.GoToRecord, OnGoHome);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.GoHome, OnGoHome);
        this.RemoveListener(EventID.GoToRecord, OnGoHome);
    }

    private void OnGoHome(object obj)
    {
        transform.position = Vector3.right * 14f + Vector3.up * -5f;
    }

    private void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Update()
    {
        if (_cam.transform.position.x - transform.position.x > 15f)
        {
            transform.position = Vector3.right * (_cam.transform.position.x + 15f) + Vector3.up * offsetY;
        }
    }

    [MessageHandler((ushort)ServerToClientId.GroundMove)]
    private static void GroundMove(Message message)
    {
        Instance.SetPosition(message.GetVector3());
    }
    
    
}
