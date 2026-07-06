using System.Collections.Generic;

namespace ResearchProgress;

public interface IResearchContainer
{
    List<IResearch> Researches { get; }
}
