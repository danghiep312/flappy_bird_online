using System;
using System.Collections.Generic;
using Riptide;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    
    public float Speed => speed;
    [SerializeField] private float speed;
    [SerializeField] private float boundary;

    [SerializeField] private Wall wall;
    
    private void Start()
    {
        wall = GetComponent<Wall>();
    }

    private void Update()
    {
        if (!GameLogic.Instance.IsReplaying) return;
        // transform.position -= Vector3.right * (speed * Time.deltaTime);
        // if (transform.position.x < boundary)
        // {
        //     Spawner.Instance.DeleteWall(wall);
        //     Destroy(gameObject);
        // }
    }

    
}