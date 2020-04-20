namespace SimpleMessageQueue
{
    public interface IMessage<T>
    {
        T Clone();
    }
}
