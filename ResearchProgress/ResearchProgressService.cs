namespace ResearchProgress;

public class ResearchProgressService
{
    public void AddResearch(
        IResearchContainer researchContainer,
        IResearch research,
        bool allowStacking)
    {
        IResearch? existingResearch = FindResearch(
            researchContainer,
            research.ResearchType);

        if (existingResearch is null)
        {
            researchContainer.Researches.Add(research);
            return;
        }

        if (allowStacking)
        {
            existingResearch.Progress += research.Progress;
        }
    }

    public bool TryAddResearchProgress(
        IResearchContainer researchContainer,
        ResearchType researchType,
        float progress)
    {
        IResearch? research = FindResearch(
            researchContainer,
            researchType);

        if (research is null)
        {
            return false;
        }

        research.Progress += progress;
        return true;
    }

    private static IResearch? FindResearch(
        IResearchContainer researchContainer,
        ResearchType researchType)
    {
        foreach (IResearch research in researchContainer.Researches)
        {
            if (research.ResearchType == researchType)
            {
                return research;
            }
        }

        return null;
    }
}
