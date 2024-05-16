using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Scoreboard : Singleton<Scoreboard>
{
    [ShowInInspector] public static Dictionary<int, PlayerScore> Scores = new Dictionary<int, PlayerScore>();
    public List<RecordScore> records;
    public GameObject recordPrefab;
    public Transform content;

    private void Start()
    {
        records = new List<RecordScore>();
        this.RegisterListener(EventID.EndRecord, OnEndRecord);
        this.RegisterListener(EventID.GoHome, GoHome);
    }

    private void GoHome(object obj)
    {
        foreach (var rec in records) Destroy(rec.gameObject);
        records.Clear();
    }

    private void OnEndRecord(object obj)
    {
        records.Clear();
    }


    public void AddRecord(int id, PlayerScore score)
    {
        Debug.Log("Add");
        RecordScore newRecord = Instantiate(recordPrefab, content).GetComponent<RecordScore>();
        newRecord.Setup(id, score);
        records.Add(newRecord);
        Scores.TryAdd(id, score);
    }

    private void Update()
    {
        foreach (RecordScore rec in records)
        {
            int id = int.Parse(rec.id.text);
            if (Scores.TryGetValue(id, out PlayerScore score))
            {
                rec.Setup(id, score);
            }
        }
    }

    public void UpdateRecord(Dictionary<int, PlayerScore> scores)
    {
        // foreach (var id in scores.Keys)
        // {
        //     if (Scores.TryGetValue(id, out PlayerScore score))
        //     {
        //         score = scores[id];
        //         foreach (var rec in records)
        //         {
        //             rec.Setup(id, score);
        //         }
        //     }
        //     else
        //     {
        //         AddRecord(id, new PlayerScore(scores[id].username, 0));
        //     }
        // }
        Scores = scores;
    }
}
