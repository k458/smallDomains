namespace TurnSequence;

public class TurnSequenceSystem
{
    private readonly List<ITurnSequenceAgent> actedAgents = [];
    private readonly List<ITurnSequenceAgent> notActedAgents = [];

    public ITurnSequenceAgent? CurrentAgent { get; private set; }

    public bool SequenceStarted { get; private set; }

    public IReadOnlyCollection<ITurnSequenceAgent> ActedAgents => actedAgents;

    public IReadOnlyCollection<ITurnSequenceAgent> NotActedAgents => notActedAgents;

    public void AddAgent(ITurnSequenceAgent agent)
    {
        notActedAgents.Add(agent);
    }

    public void StartCombat()
    {
        if (CurrentAgent is not null)
        {
            notActedAgents.Add(CurrentAgent);
            CurrentAgent = null;
        }

        notActedAgents.AddRange(actedAgents);
        actedAgents.Clear();

        StartRound();
    }

    public void StartSequence()
    {
        SequenceStarted = true;
    }

    public bool RemoveAgent(ITurnSequenceAgent agent)
    {
        if (ReferenceEquals(CurrentAgent, agent))
        {
            CurrentAgent = null;
        }

        return actedAgents.Remove(agent) || notActedAgents.Remove(agent);
    }

    public void Update()
    {
        if (!SequenceStarted)
        {
            return;
        }

        if (CurrentAgent is not null && !CurrentAgent.HasActionFinished)
        {
            return;
        }

        if (CurrentAgent is not null)
        {
            actedAgents.Add(CurrentAgent);
            CurrentAgent = null;
        }

        if (notActedAgents.Count == 0)
        {
            notActedAgents.AddRange(actedAgents);
            actedAgents.Clear();

            StartRound();
        }

        CurrentAgent = PickNextAgent();
        CurrentAgent?.TurnStarted();
    }

    private void StartRound()
    {
        SequenceStarted = false;

        foreach (ITurnSequenceAgent agent in notActedAgents)
        {
            agent.RollInitiative();
            agent.RoundStarted();
        }
    }

    private ITurnSequenceAgent? PickNextAgent()
    {
        if (notActedAgents.Count == 0)
        {
            return null;
        }

        ITurnSequenceAgent nextAgent = notActedAgents[0];

        for (int i = 1; i < notActedAgents.Count; i++)
        {
            ITurnSequenceAgent candidate = notActedAgents[i];

            if (candidate.RolledInitiative > nextAgent.RolledInitiative)
            {
                nextAgent = candidate;
            }
        }

        notActedAgents.Remove(nextAgent);
        return nextAgent;
    }
}
