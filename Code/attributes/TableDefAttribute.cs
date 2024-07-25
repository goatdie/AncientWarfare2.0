using System;

namespace AncientWarfare.Attributes;

public class TableDefAttribute : Attribute
{
    public TableDefAttribute(string pName)
    {
        Name = pName;
    }

    public string Name { get; private set; }
}