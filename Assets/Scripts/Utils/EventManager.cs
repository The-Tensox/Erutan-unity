using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Erutan.Scripts.Utils;
using System;


/// I guess the usage of this class should be restricted to simple events : UI
public class EventManager : Singleton<EventManager> {
    private Dictionary <string, Action<string>> eventDictionary;

    void Start ()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action<string>>();
        }
    }

    public static void StartListening (string eventName, Action<string> listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventName))
        {
            Instance.eventDictionary[eventName] += listener;
        }
        else
        {
            Instance.eventDictionary.Add(eventName, listener);
        }
    }

    public static void StopListening (string eventName, Action<string> listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventName)) 
            Instance.eventDictionary[eventName] -= listener;
    }

    public static void TriggerEvent (string eventName, string param)
    {
        if (Instance.eventDictionary.ContainsKey(eventName)) 
            Instance.eventDictionary[eventName](param);
    }
}
