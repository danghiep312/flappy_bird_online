
using UnityEngine;
using UnityEngine.UI;

public class SaveRecord : MonoBehaviour, IInit
{
    [SerializeField] private Button accept;
    [SerializeField] private Button reject;
    
    public void Init()
    {
        accept.onClick.AddListener(OnAccept);
        reject.onClick.AddListener(OnReject);
    }

    private void OnReject()
    {
        UIManager.Instance.GoHome();
        gameObject.SetActive(false);
        NetworkManager.Instance.Disconnect();
    }

    private void OnAccept()
    {
        UIManager.Instance.GoHome();
        Util.SaveRecord(GameLogic.Instance.recorder);
        // Util.SaveRecord();
        gameObject.SetActive(false);
        NetworkManager.Instance.Disconnect();
    }
}