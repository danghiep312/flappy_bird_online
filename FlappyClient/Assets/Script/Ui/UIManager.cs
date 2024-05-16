using System;
using Riptide;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Connect")] [SerializeField] private GameObject connectUI;
    [SerializeField] private InputField usernameField;
    [SerializeField] private InputField hostField;
    [SerializeField] private InputField port;

    [Header("Object")] [SerializeField] private Button readyButton;
    [SerializeField] private GameObject votePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject saveRecordPanel;
    [SerializeField] private GameObject recordPanel;
    [SerializeField] private GameObject replayPanel;
    [SerializeField] private GameObject endRecordPanel;

    private void Start()
    {
        hostField.text = PlayerPrefs.GetString("host", "127.0.0.1");
        port.text = PlayerPrefs.GetString("port", "7777");

        readyButton.onClick.AddListener(OnClickReady);

        this.RegisterListener(EventID.PlayGame, OnPlayGame);
        this.RegisterListener(EventID.EndGame, OnEndGame);
        this.RegisterListener(EventID.GoToRecord, OnGoToRecord);
    }

    private void OnGoToRecord(object obj)
    {
        recordPanel.SetActive(false);
        connectUI.SetActive(false);
    }

    private void OnEndGame(object obj)
    {
        saveRecordPanel.SetActive(true);
    }

    private void OnPlayGame(object obj)
    {
        readyButton.gameObject.SetActive(false);
    }

    public void ShowRecord(bool status)
    {
        recordPanel.gameObject.SetActive(status);
    }

    private void OnClickReady()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.Ready);
        message.AddBool(true);
        NetworkManager.Instance.Client.Send(message);
    }


    public void Connect()
    {
        usernameField.interactable = false;
        connectUI.SetActive(false);

        NetworkManager.Instance.Connect(hostField.text, ushort.Parse(port.text));
        ScoreClient.Instance.Connect();
        //ScoreNetwork.Instance.Connect(hostField.text, 8888);
    }


    public void BackToMain()
    {
        usernameField.interactable = true;
        connectUI.SetActive(true);
    }


    public void SendName()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.Name);
        message.AddString(usernameField.text);
        NetworkManager.Instance.Client.Send(message);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("host", hostField.text);
        PlayerPrefs.SetString("port", port.text);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetString("host", hostField.text);
            PlayerPrefs.SetString("port", port.text);
        }
    }

    public void ShowVote(string title)
    {
        votePanel.GetComponent<VotePanel>().SetTitle(title);
        votePanel.SetActive(true);
    }

    public void ShowPause(bool status)
    {
        pausePanel.SetActive(status);
    }

    public void ShowReplay(bool status)
    {
        replayPanel.SetActive(status);
    }

    public void ShowEndRecord(bool status)
    {
        endRecordPanel.SetActive(status);
    }

    public void GoHome()
    {
        this.PostEvent(EventID.GoHome);
        replayPanel.SetActive(false);
        endRecordPanel.SetActive(false);
        saveRecordPanel.SetActive(false);
        
        readyButton.gameObject.SetActive(true);
        connectUI.SetActive(true);
    }
}