using AncientWarfare.Abstracts;

namespace AncientWarfare.Core.Content;

public class ActorTraitExtendLibrary : ExtendedLibrary<ActorTrait>, IManager
{
    /// <summary>
    ///     野蛮
    /// </summary>
    public static readonly ActorTrait savage;

    public void Initialize()
    {
    }

    protected override void init()
    {
        init_fields();
    }
}