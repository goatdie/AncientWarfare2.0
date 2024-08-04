using System.Collections.Generic;

namespace AncientWarfare.Core.AI.Abstract;

public interface IProfessionalBeh
{
    public HashSet<string>         ProfessionRequired           { get; }
    public Dictionary<string, int> ProfessionExpGivenPerExecute { get; }

    public void GiveExp(Actor actor);
}