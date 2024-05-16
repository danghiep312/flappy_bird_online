
using System;
using System.Collections.Generic;
using Riptide;
using Unity.Mathematics;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    [SerializeField] Transform spawnPoint;

    private List<Wall> _listWalls = new List<Wall>();

    private float _time;
    private Vector3 _lastPos;

    private void Start()
    {
        _lastPos = spawnPoint.position;
    }

    public void Restart()
    {
        Start();
    }
    

    public void DeleteWall(Wall wall)
    {
        _listWalls.Remove(wall);
    }
    
    public void Spawn(Vector3 pos)
    {
        Wall wall = Instantiate(GameLogic.Instance.WallPrefab, pos, Quaternion.identity).GetComponent<Wall>();
        _listWalls.Add(wall);
        _lastPos = _listWalls[^1].transform.position;
    }

    public void SpawnStar(Vector3 pos)
    {
        Star star = Instantiate(GameLogic.Instance.StarPrefab, pos, quaternion.identity).GetComponent<Star>();
    }
}