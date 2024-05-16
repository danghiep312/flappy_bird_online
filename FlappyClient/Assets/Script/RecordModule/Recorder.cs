using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
public class Recorder
{
    [ShowInInspector] 
    public Dictionary<int, String> user = new();
    [ShowInInspector]
    public Dictionary<int, List<float>> clickTime = new();
    [ShowInInspector]
    public Dictionary<int, List<float>> scaleTime = new();
    [ShowInInspector]
    public Dictionary<int, bool> localCheck = new();
    // public List<int> clickTime = new();
    public List<Position> wallPos = new();
    // public List<float> wallTime;
    public List<float> wallTime = new();

    public List<Position> starPos = new();

    public List<float> starTime = new();


    public int EndSignal { get; set; }
    public float EndTime { get; set; }

    public void AddStarInfo(Vector3 pos, float time)
    {
        starPos.Add(new Position(pos.x, pos.y, pos.z));
        starTime.Add(time);
    }

    public void AddClick(int playerId, float time)
    {
        // clickTime.Add(time);
        if (clickTime.TryGetValue(playerId, out List<float> click))
        {
            click.Add(time);
        }
    }
    
    public void AddWallInfo(Position position, float time)
    {
        wallTime.Add(time);
        wallPos.Add(position);
    }

    public void AddPlayer(int playerId, String username)
    {
        clickTime.Add(playerId, new List<float>());
        scaleTime.Add(playerId, new List<float>());
        localCheck.Add(playerId, Player.list[(ushort)playerId].IsLocal);
        user.Add(playerId, username);
    }

    public void AddScaleTime(int playerId, float time)
    {
        if (scaleTime.TryGetValue(playerId, out List<float> value))
        {
            if (!value.Contains(time)) value.Add(time);
        }
        else
        {
            scaleTime.Add(playerId, new List<float> {time});
        }
    }

    public bool CheckLocal(int playerId)
    {
        return localCheck[playerId];
    }

    public bool CheckEndRecord(float time)
    {
        return time >= EndTime;
    }

    public bool CheckEndRecord(int frame)
    {
        return frame >= EndSignal;
    }
}

[Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;

    public Position()
    {
    }

    public Position(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

[Serializable]
public class RecordPlayerScore
{
    public int id;
    public PlayerScore playerScore;
}