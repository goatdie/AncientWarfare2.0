using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using Figurebox.attributes;
using NeoModLoader.General;

namespace Figurebox.core.table_items;

public abstract class BaseTableItem
{
}

public abstract class AbstractTableItem<T> : BaseTableItem where T : AbstractTableItem<T>, new()
{
    private static readonly Dictionary<string, string> field_name_to_column_name = new();

    private static readonly Dictionary<Type, string> type_to_table_name = new();

    public virtual void ReadFromReader(SQLiteDataReader reader)
    {
        if (field_name_to_column_name.Count == 0)
        {
            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<TableItemDefAttribute>() ?? new TableItemDefAttribute();
                field_name_to_column_name[field.Name] =
                    string.IsNullOrEmpty(attr.Name) ? field.Name.ToUpper() : attr.Name;
            }
        }

        foreach (var pair in field_name_to_column_name)
        {
            var ordinal = reader.GetOrdinal(pair.Value);
            if (ordinal == -1) continue;
            var type = reader.GetFieldType(ordinal);
            if (type == typeof(string))
            {
                ((T)this).SetField(pair.Key, reader.GetString(ordinal));
                continue;
            }

            if (type == typeof(int))
            {
                ((T)this).SetField(pair.Key, reader.GetInt32(ordinal));
                continue;
            }

            if (type == typeof(long))
            {
                ((T)this).SetField(pair.Key, reader.GetInt64(ordinal));
                continue;
            }

            if (type == typeof(float))
            {
                ((T)this).SetField(pair.Key, reader.GetFloat(ordinal));
                continue;
            }

            if (type == typeof(double))
            {
                ((T)this).SetField(pair.Key, reader.GetDouble(ordinal));
                continue;
            }

            if (type == typeof(bool))
            {
                ((T)this).SetField(pair.Key, reader.GetBoolean(ordinal));
                continue;
            }

            ((T)this).SetField(pair.Key, reader[ordinal]);
        }
    }

    public static string GetTableName()
    {
        if (type_to_table_name.ContainsKey(typeof(T))) return type_to_table_name[typeof(T)];
        type_to_table_name[typeof(T)] = typeof(T).GetCustomAttribute<TableDefAttribute>().Name;
        return type_to_table_name[typeof(T)];
    }
}