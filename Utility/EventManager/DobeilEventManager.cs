using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobeilEventManager : Singleton<DobeilEventManager>
{
    private static readonly Dictionary<string, List<Action<object>>> allEvents = new Dictionary<string, List<Action<object>>>();
    public static void RegisterGlobalEvent(string eventName, Action<object> _event)
    {
        if (!allEvents.ContainsKey(eventName))
            allEvents.Add(eventName, new List<Action<object>>());

        allEvents[eventName].Add(_event);
    }

    public static void SendGlobalEvent(string eventName, object eventData = null)
    {
        if (allEvents.ContainsKey(eventName))
            for (int i = 0; i < allEvents[eventName].Count; i++)
                allEvents[eventName][i]?.Invoke(eventData);
    }

    public static void RemoveGlobalEvent(string eventName, Action<object> _action)
    {
        if (allEvents.ContainsKey(eventName))
            allEvents[eventName].Remove(_action);
        else
            Debug.LogError(eventName + " Doesn't Exist!!!");
    }
}
