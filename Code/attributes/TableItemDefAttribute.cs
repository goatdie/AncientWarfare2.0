using System;

namespace Figurebox.attributes;

public class TableItemDefAttribute : Attribute
{
    public TableItemDefAttribute(string pName = "", bool pIsPrimary = false, bool pIsUnique = false,
        bool pIsNotNull = false, string pDefaultValue = "", string pCheck = "")
    {
        Name = pName;
        IsPrimary = pIsPrimary;
        IsUnique = pIsUnique;
        IsNotNull = pIsNotNull;
        DefaultValue = pDefaultValue;
        Check = pCheck;
    }

    public string Name { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsUnique { get; private set; }
    public bool IsNotNull { get; private set; }
    public string DefaultValue { get; private set; }
    public string Check { get; private set; }
}