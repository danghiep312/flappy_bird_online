using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEvent : MonoBehaviour
{
    public EventID eventId;
    public void Post() {
        this.PostEvent(eventId);
    }
}
