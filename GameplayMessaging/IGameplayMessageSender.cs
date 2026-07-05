namespace GameplayMessaging;

public interface IGameplayMessageSender
{
    IGameplayMessageReceiver MessageReceiver { get; }

    void SendMessage(in GameplayMessageContext context);
}
