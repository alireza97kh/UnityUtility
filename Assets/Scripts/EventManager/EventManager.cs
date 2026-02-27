using System.Collections.Generic;
using UnityEngine.Events;

public static class EventManager
{
	private static Dictionary<string, UnityEvent<object>> eventDictionary = new();
	public static void RegisterGlobalEvent(string eventName, UnityAction<object> listener)
	{
		if (eventDictionary.TryGetValue(eventName, out UnityEvent<object> currentEvent))
		{
			currentEvent.AddListener(listener);
		}
		else
		{
			currentEvent = new UnityEvent<object>();
			currentEvent.AddListener(listener);
			eventDictionary.Add(eventName, currentEvent);
		}
	}

	public static void RemoveGlobalEvent(string eventName, UnityAction<object> listener)
	{
		if (eventDictionary.TryGetValue(eventName, out UnityEvent<object> currentEvent))
		{
			currentEvent.RemoveListener(listener);
		}
	}

	public static void SendGlobalEvent(string eventName, object value)
	{
		if (eventDictionary.TryGetValue(eventName, out UnityEvent<object> currentEvent))
		{
			currentEvent.Invoke(value);
		}
	}

}
