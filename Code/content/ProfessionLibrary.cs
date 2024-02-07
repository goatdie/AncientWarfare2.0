using Figurebox.abstracts;
using Figurebox.constants;
using Figurebox.utils;

namespace Figurebox.content;

public class ProfessionLibrary : ExtendedLibrary<ProfessionAsset>
{
    protected override void init()
    {
        add(new ProfessionAsset
        {
            id = AWUnitProfession.Slave.ToString().ToLower(),
            can_capture = true,
            profession_id = AWUnitProfession.Slave.C(),
            special_skin_path = "unit_slave"
        });
        add(new ProfessionAsset
        {
            id = AWUnitProfession.Heir.ToString().ToLower(),
            can_capture = true,
            profession_id = AWUnitProfession.Heir.C(),
            special_skin_path = "unit_heir"
        });
    }
}