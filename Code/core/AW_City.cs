using System;
using Figurebox.constants;

namespace Figurebox.core;

public class AW_City : City
{
    /// <summary>
    ///     拓展后的单位身份
    /// </summary>
    private static readonly AWUnitProfession[] ExtendUnitProfessions =
        (AWUnitProfession[])Enum.GetValues(typeof(AWUnitProfession));
}