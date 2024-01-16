using System;

namespace Figurebox.attributes;

public class TableDefAttribute : Attribute
{
    public TableDefAttribute(string pName)
    {
        Name = pName;
    }

    public string Name { get; private set; }
}