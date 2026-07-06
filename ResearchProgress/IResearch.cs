namespace ResearchProgress;

public interface IResearch
{
    float Progress { get; set; }
    float RequiredProgress { get; }
    ResearchType ResearchType { get; }
}
