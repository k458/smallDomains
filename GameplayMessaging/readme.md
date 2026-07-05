# GameplayMessaging

Defines a direct message boundary between gameplay scene elements and a local scene controller.

## Main Types

- `GameplayMessageType` identifies the message.
- `GameplayMessageContext` carries the type plus optional integer and float values.
- `IGameplayMessageSender` exposes its receiver and sends messages.
- `IGameplayMessageReceiver` receives messages.

## Intended Use

A Godot child node can implement `IGameplayMessageSender` and receive an `IGameplayMessageReceiver` reference when it is created. The local scene controller implements the receiver interface.

Messages travel directly from the child to the controller. Intermediate panels do not need to forward messages unless they have real message-handling behavior.
