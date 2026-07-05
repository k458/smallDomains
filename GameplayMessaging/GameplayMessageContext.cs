namespace GameplayMessaging;

public readonly record struct GameplayMessageContext(
    GameplayMessageType Type,
    int IntValue = 0,
    float FloatValue = 0);
