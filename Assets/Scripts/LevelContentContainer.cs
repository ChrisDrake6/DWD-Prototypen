using System.Collections.Generic;

public class LevelContentContainer
{
    public LevelParameters LevelParameters { get; set; }
    public List<ConsequencePreview> Consequences { get; set; }
    public List<DecisionData> Decisions { get; set; }
}
