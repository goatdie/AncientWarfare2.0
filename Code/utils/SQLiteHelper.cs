using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Figurebox.constants;

namespace Figurebox.Utils;

public class ColumnVal : IReusable
{
    public string Name;
    public object Value;

    public void Setup()
    {
    }

    public void Recycle()
    {
    }

    public static ColumnVal Create(string pName, object pValue)
    {
        var obj = ObjectPool<ColumnVal>.GlobalGetNext();
        obj.Name = pName;
        obj.Value = pValue;
        return obj;
    }
}

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

    private static readonly Dictionary<string, TableInfo> _tableInfos = new();

    public static void Insert(this SQLiteConnection pThis, string pTableName, params ColumnVal[] pValues)
    {
        if (pThis == null)
        {
            if (DebugConst.LOG_ALL_EXCEPTION) Main.LogWarning("Null SQLite Connection", true);
            return;
        }

        var table = _tableInfos[pTableName];
        using var cmd = new SQLiteCommand(pThis);
        cmd.CommandText = table.InsertPrepareCMD;
        cmd.Prepare();
        foreach (var value in pValues)
        {
            cmd.Parameters.AddWithValue(table.ColumnNameToParamName[value.Name], value.Value);
            ObjectPool<ColumnVal>.GlobalRecycle(value);
        }

        cmd.ExecuteNonQuery();
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

            if (!string.IsNullOrEmpty(col.Default))
            {
                cmd_builder.Append(" DEFAULT ");
                if (col.ValueType == ColumnType.TEXT)
                {
                    cmd_builder.Append('\'');
                    cmd_builder.Append(col.Default);
                    cmd_builder.Append('\'');
                }
                else
                {
                    cmd_builder.Append(col.Default);
                }
            }

            if (!string.IsNullOrEmpty(col.Check))
            {
                cmd_builder.Append(" CHECK(");
                cmd_builder.Append(col.Check);
                cmd_builder.Append(')');
            }
        }

        cmd_builder.Append(')');

        using var cmd = new SQLiteCommand(pThis);
        cmd.CommandText = cmd_builder.ToString();
        cmd.ExecuteNonQuery();
        _tableInfos[pTableName] = new TableInfo(pTableName, pCols);
    }

    private class TableInfo
    {
        public readonly Dictionary<string, string> ColumnNameToParamName = new();
        public readonly string InsertPrepareCMD;
        public readonly string Name;
        public List<ColumnDef> ColumnDefs;

        public TableInfo(string pName, List<ColumnDef> pColumnDefs)
        {
            Name = pName;
            ColumnDefs = pColumnDefs;
            var prepare_cmd_builder = new StringBuilder();
            prepare_cmd_builder.Append("INSERT INTO ");
            prepare_cmd_builder.Append(Name);

            prepare_cmd_builder.Append('(');
            var need_comma = false;
            foreach (var col in pColumnDefs)
            {
                if (need_comma)
                    prepare_cmd_builder.Append(", ");
                else
                    need_comma = true;
                prepare_cmd_builder.Append(col.Name);
            }

            prepare_cmd_builder.Append(')');

            prepare_cmd_builder.Append(" VALUES(");
            need_comma = false;
            foreach (var col in pColumnDefs)
            {
                if (need_comma)
                    prepare_cmd_builder.Append(", ");
                else
                    need_comma = true;

                var param_name = "@" + col.Name;
                prepare_cmd_builder.Append(param_name);
                ColumnNameToParamName[col.Name] = param_name;
            }

            prepare_cmd_builder.Append(')');
            InsertPrepareCMD = prepare_cmd_builder.ToString();
        }
    }

    public struct ColumnDef
    {
        public string Name;
        public ColumnType ValueType;
        public bool IsPrimary;
        public bool IsUnique;
        public bool IsNotNull;
        public string Default;
        public string Check;

        public ColumnDef(string pName, ColumnType pValueType = ColumnType.TEXT, bool pIsPrimary = false,
            bool pIsUnique = false, bool pIsNotNull = true, string pDefault = "", string pCheck = "")
        {
            Name = pName;
            ValueType = pValueType;
            IsPrimary = pIsPrimary;
            IsUnique = pIsUnique;
            IsNotNull = pIsNotNull;
            Default = pDefault;
            Check = pCheck;
        }
    }
}