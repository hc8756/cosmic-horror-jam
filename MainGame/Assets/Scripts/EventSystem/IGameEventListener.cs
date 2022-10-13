public interface IGameEventListener<T>
{
    public void OnEventRaised(T data);
}

public interface IGameEventListener
{
    public void OnEventRaised();
}