using System;
using System.Collections.Generic;

namespace SimpleMessageQueue
{
    public class MessageQueue : IMessageQueue
    {
        readonly Dictionary<string, List<Action<object>>> listeners;

        public MessageQueue()
        {
            listeners = new Dictionary<string, List<Action<object>>>();
        }
        
        public void Subscribe<T>(Action<T> callback)
        {
            if (callback == null)
                return;

            var key = MakeKey<T>();
            lock(listeners)
            {
                var action = new Action<object>(o => callback((T)o));
                if (listeners.ContainsKey(key))
                {
                    AddToListeners(key, action);
                }
                else
                {
                    CreateListeners(key, action);
                }
            }
        }

        public void Publish<T>(T message)
        {
            if (message == null)
                return;

            var key = MakeKey<T>();
            lock(listeners)
            {
                if (listeners.ContainsKey(key))
                {
                    var actions = listeners[key];
                    Publish(message, actions);
                }
            }
        }

        void Publish<T>(T message, List<Action<object>> actions)
        {
            foreach(var action in actions)
            {
                action(message);
            }
        }

        void AddToListeners(string key, Action<object> action)
        {
            var actions = listeners[key];
            actions.Add(action);
        }

        void CreateListeners(string key, Action<object> action)
        {
            var actions = new List<Action<object>>
            {
                action
            };
            listeners[key] = actions;
        }

        string MakeKey<T>() => typeof(T).ToString();
    }
}
