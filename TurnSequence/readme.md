# TurnSequence

Owns turn order, round setup, and current-turn progression.

## Main Types

- `TurnSequenceSystem` tracks agents that have acted and agents that have not acted this round.
- `ITurnSequenceAgent` is implemented by anything that can take turns.

## Flow

1. Agents are added with `AddAgent`.
2. `StartCombat` resets combat sequencing into a fresh round.
3. Starting a round calls `RollInitiative` and `RoundStarted` on all not-acted agents.
4. `StartSequence` allows `Update` to advance.
5. `Update` keeps the current agent active until `HasActionFinished` is true.
6. When ready, the system moves the finished agent to acted agents and picks the not-acted agent with the highest `RolledInitiative`.
7. The picked agent becomes `CurrentAgent` and receives `TurnStarted`.

## Boundary

The sequencer should not execute abilities, play animations, or know about scene flow blockers. Agents or scene controllers should adapt those concerns outside this system.
