using UnityEngine;
using UnityEngine.UI;

public class EndRecordPopup : MonoBehaviour, IInit
{
    public Button goHome;


    public void Init()
    {
        goHome.onClick.AddListener(GoHome);
    }

    private void GoHome()
    {
        UIManager.Instance.GoHome();
        this.PostEvent(EventID.EndRecord, -1);
    }
}