using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotePanel : MonoBehaviour, IInit
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    

    public void Init()
    {
        acceptButton.onClick.AddListener(Accept);
        rejectButton.onClick.AddListener(Reject);
    }

    private void Reject()
    {
        this.PostEvent(EventID.Vote, false);
    }

    private void Accept()
    {
        this.PostEvent(EventID.Vote, true);
    }

    public void SetTitle(string txt)
    {
        title.text = txt;
    }
}

public interface IInit
{
    void Init();
}