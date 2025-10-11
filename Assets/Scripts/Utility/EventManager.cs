using System;
using System.Collections.Generic;
using UnityEngine;

namespace BusJamDemo.Utility
{
    public static class EventManager
    {
        private static Dictionary<Enum, Action> events = new Dictionary<Enum, Action>();
    
        public static void Subscribe(Enum eventType, Action listener)
        {
            if (events.ContainsKey(eventType))
            {
                events[eventType] += listener;
            }
            else
            {
                events[eventType] = listener;
            }
        }
    
        public static void Unsubscribe(Enum eventType, Action listener)
        {
            if (events.ContainsKey(eventType))
            {
                events[eventType] -= listener;
            }
        }
    
        public static void Execute(Enum eventType)
        {
            if (events.ContainsKey(eventType))
            {
                try
                {
                    events[eventType]?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while invoking event type : {eventType}! Error : {e}");
                }
            }
        }
    }
    
    public static class EventManager<T>
    {
        private static Dictionary<Enum, Action<T>> events = new Dictionary<Enum, Action<T>>();
    
        public static void Subscribe(Enum eventType, Action<T> listener)
        {
            if (events.ContainsKey(eventType))
            {
                events[eventType] += listener;
            }
            else
            {
                events[eventType] = listener;
            }
        }
    
        public static void Unsubscribe(Enum eventType, Action<T> listener)
        {
            if (events.ContainsKey(eventType))
            {
                events[eventType] -= listener;
            }
        }
    
        public static void Execute(Enum eventType, T eventData)
        {
            if (events.ContainsKey(eventType))
            {
                try
                {
                    events[eventType]?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while invoking event type : {eventType} with event data : {eventData}! Error : {e}");
                }
            }
        }
    }   
}