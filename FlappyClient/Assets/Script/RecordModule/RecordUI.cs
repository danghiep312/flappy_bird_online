using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Button button;

    private string path;
    
    private void Start()
    {
        button.onClick.AddListener(GoToReplay);
    }

    private void GoToReplay()
    {
        ReplayController.Instance.record = Util.LoadRecord(path);
        this.PostEvent(EventID.GoToRecord);
    }

    public void SetPath(string path)
    {
        this.path = path;

        var splitPath = path.Split('/');
        text.text = splitPath[^1];
    }
}
