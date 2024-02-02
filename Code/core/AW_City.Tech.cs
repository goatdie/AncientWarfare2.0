namespace Figurebox.core;

public partial class AW_City
{
    public bool HasTech(string pTechId)
    {
        return addition_data.tech_rank.ContainsKey(pTechId);
    }

    public bool HasTech(string pTechId, int pRank)
    {
        return addition_data.tech_rank.TryGetValue(pTechId, out var rank) && rank >= pRank;
    }
}