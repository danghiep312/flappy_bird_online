
using System;
using System.Collections.Generic;
using Riptide;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : Singleton<Spawner>
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] private float delayTime;
    private List<Wall> _listWalls = new List<Wall>();
    private Vector3 _lastPos;

    private float _time;

    private void Start()
    {
        _lastPos = spawnPoint.position;
    }

    private void Update()
    {
        if (!GameLogic.IsPlaying) return;
        if (Player.list.Count <= 0) return;
        _time += Time.deltaTime;

        if (_time >= delayTime)
        {
            Vector3 pos = Vector3.right * (_lastPos.x + 7f) + Vector3.up * Random.Range(-1f, 2.2f);
            _listWalls.Add(Wall.Spawn(Wall.CurId++, pos));

            Star.Spawn(pos + Vector3.right * Random.Range(-2f, 2f) + Vector3.up * Random.Range(-1f, 1f));
            
            _lastPos = _listWalls[^1].transform.position;
            _time -= delayTime;
        }
    }

    public void Restart()
    {
        _lastPos = spawnPoint.position;
    }


    public void DeleteWall(Wall wall)
    {
        _listWalls.Remove(wall);
    }

    public void DeleteAll()
    {
        foreach (Wall wall in _listWalls)
        {
            if (wall != null) {
                DestroyImmediate(wall.gameObject);
            }
        }
    }
}