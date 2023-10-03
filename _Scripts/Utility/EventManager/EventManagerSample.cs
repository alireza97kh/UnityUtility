using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManagerSample : MonoBehaviour
{
    void Start()
    {
        DobeilEventManager.RegisterGlobalEvent("FIRST_EVENT", (obj) =>
        {
            if (obj is string)
                Debug.Log($"Data Sended In Event is string : {obj}");
            else if (obj is int)
                Debug.Log($"Data Sended In Event is Int : {obj}");
        });
        DobeilEventManager.SendGlobalEvent("FIRST_EVENT", "Hi");
        DobeilEventManager.SendGlobalEvent("FIRST_EVENT", 123);
    }
}
