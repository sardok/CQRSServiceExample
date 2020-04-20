using System;

namespace SimpleMessageQueue
{
    public interface IMessageQueue
    {
        void Subscribe<T>(Action<T> callback) where T : IMessage<T>;
        void Publish<T>(IMessage<T> message);
    }
}