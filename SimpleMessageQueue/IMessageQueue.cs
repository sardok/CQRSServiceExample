using System;

namespace SimpleMessageQueue
{
    public interface IMessageQueue
    {
        void Subscribe<T>(Action<T> callback);
        void Publish<T>(T message);
    }
}