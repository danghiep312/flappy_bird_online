using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordScore : MonoBehaviour
{
    public TextMeshProUGUI id;
    public TextMeshProUGUI username;
    public TextMeshProUGUI scoreText;


    private void Start()
    {
        this.RegisterListener(EventID.EndRecord, OnEndRecord);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.EndRecord, OnEndRecord);
    }

    private void OnEndRecord(object obj)
    {
        Destroy(gameObject);
    }

    public void Setup(int clientId, PlayerScore score)
    {
        id.text = clientId.ToString();
        username.text = score.username;
        scoreText.text = score.score.ToString();
    }
}
