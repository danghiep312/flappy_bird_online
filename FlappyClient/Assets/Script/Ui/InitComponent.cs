

using System;
using UnityEngine;

public class InitComponent : MonoBehaviour
{
    private void Start()
    {
        foreach (var comp in GetComponentsInChildren<IInit>(true))
        {
            comp.Init();
        }
    }
}