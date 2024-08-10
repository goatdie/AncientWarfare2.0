namespace AncientWarfare.Core.Profession;

public class NewProfessionData
{
    public int exp_until_now { get; private set; }
    public int exp_left      { get; private set; }

    public void AddExp(int count)
    {
        exp_until_now += count;
        exp_left += count;
    }

    public void TakeExp(int count)
    {
        exp_left -= count;
    }
}