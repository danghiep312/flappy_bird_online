
using System;
using Riptide;
using UnityEngine;

public class Star : MonoBehaviour
{
    public int ID;

    private void Start()
    {
        this.RegisterListener(EventID.StarDestroyed, OnStarDestroyed);
        this.RegisterListener(EventID.EndRecord, OnStarDestroyed);
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.StarDestroyed, OnStarDestroyed);
        this.RemoveListener(EventID.EndRecord, OnStarDestroyed);
    }

    private void OnStarDestroyed(object obj)
    { 
        if (ID == (int) obj || (int) obj == -1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameLogic.Instance.IsReplaying)
        {
            gameObject.SetActive(false);
        }
    }

    public static void Spawn(Vector3 pos, int id)
    {
        Star star = Instantiate(GameLogic.Instance.StarPrefab, pos, Quaternion.identity).GetComponent<Star>();
        star.ID = id;
    }
    
    [MessageHandler((ushort) ServerToClientId.StarSpawned)]
    public static void HandleStarSpawned(Message message)
    {
        int id = message.GetInt();
        Vector3 pos = message.GetVector3();
        Spawn(pos, id);
        GameLogic.Instance.OnStarSpawned(pos, message.GetFloat());
    }
    
    [MessageHandler((ushort) ServerToClientId.StarDestroyed)]
    public static void HandleStarDestroyed(Message message)
    {
        int id = message.GetInt();
        EventDispatcher.Instance.PostEvent(EventID.StarDestroyed, id);
    }
}