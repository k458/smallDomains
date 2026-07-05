namespace GameplayMessaging;

public interface IGameplayMessageReceiver
{
    void ReceiveMessage(in GameplayMessageContext context);
}
