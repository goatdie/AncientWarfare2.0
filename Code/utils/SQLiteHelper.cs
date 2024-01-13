using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Figurebox.Utils;

public static class SQLiteHelper
{
    public enum ColumnType
    {
        NULL,
        INTEGER,
        REAL,
        TEXT,
        BLOB
    }

    public static void CreateTable(this SQLiteConnection pThis, string pTableName, List<ColumnDef> pCols)
    {
        StringBuilder cmd_builder = new();

        cmd_builder.Append("CREATE TABLE ");
        cmd_builder.Append(pTableName);
        cmd_builder.Append('(');

        var primary_found = false;
        var need_comma = false;
        foreach (var col in pCols)
        {
            if (need_comma)
                cmd_builder.Append(", ");
            else
                need_comma = true;
            cmd_builder.Append(col.Name);
            cmd_builder.Append(' ');
            cmd_builder.Append(col.ValueType.ToString());

            if (col.IsPrimary)
            {
                if (primary_found) throw new ArgumentException($"Repeat Primary Key {col.Name}");

                primary_found = true;
                cmd_builder.Append(" PRIMARY KEY");
            }

            if (col.IsUnique) cmd_builder.Append(" UNIQUE");

            if (col.IsNotNull) cmd_builder.Append(" NOT NULL");
        }

        cmd_builder.Append(')');

        using var cmd = new SQLiteCommand(pThis);
        cmd.CommandText = cmd_builder.ToString();
        cmd.ExecuteNonQuery();
    }

    public struct ColumnDef
    {
        public string Name;
        public ColumnType ValueType;
        public bool IsPrimary;
        public bool IsUnique;
        public bool IsNotNull;
    }
}