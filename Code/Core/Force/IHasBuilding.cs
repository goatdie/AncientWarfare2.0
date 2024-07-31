namespace AncientWarfare.Core.Force;

public interface IHasBuilding
{
    public void RemoveBuildingOneside(string building_id);
    public void AddBuildingOneside(Building  building);
}