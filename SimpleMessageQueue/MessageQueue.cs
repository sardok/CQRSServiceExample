using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimpleMessageQueue
{
    public class MessageQueue : IMessageQueue
    {
        readonly Dictionary<string, List<Action<object>>> listeners;
        readonly bool waitForSink;

        public MessageQueue(bool waitForSink = false)
        {
            listeners = new Dictionary<string, List<Action<object>>>();
            this.waitForSink = waitForSink;
        }
        
        public void Subscribe<T>(Action<T> callback) where T : IMessage<T>
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

        public void Publish<T>(IMessage<T> message)
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

        void Publish<T>(IMessage<T> message, List<Action<object>> actions) 
        {
            var tasks = new List<Task>();
            foreach(var action in actions)
            {
                var task = Task.Run(() => action(message.Clone()));
                tasks.Add(task);
            }

            if (waitForSink)
            {
                Task.WaitAll(tasks.ToArray());
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
