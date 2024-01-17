using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using Figurebox.attributes;
using Figurebox.Utils;
using JetBrains.Annotations;

namespace Figurebox.core;

public class EventsManager
{
    private string OperatingDataBasePath;
    public bool InitializeSuccessful { get; private set; } = true;
    [CanBeNull] public SQLiteConnection OperatingDB { get; private set; }
    public static EventsManager Instance { get; } = new();

    public void CreateDataBase()
    {
        try
        {
            CleanTempDataBase();
            SQLiteConnection.CreateFile(OperatingDataBasePath);
            OperatingDB = new SQLiteConnection("data source=" + OperatingDataBasePath);
            OperatingDB.Open();
            InitializeDB();
        }
        catch (Exception e)
        {
            InitializeSuccessful = false;
            Main.LogWarning("Failed to create database, events will not be stored");
            Main.LogWarning(e.Message);
            Main.LogWarning(e.StackTrace);
        }
    }

    private void InitializeDB()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var table_def = type.GetCustomAttribute<TableDefAttribute>();
            if (table_def == null) continue;

            var tableName = table_def.Name;
            var fields = type.GetFields();
            var tableItems = (
                from field in fields
                let attribute = field.GetCustomAttribute<TableItemDefAttribute>() ?? new TableItemDefAttribute()
                let col_type = field.FieldType.Name.ToLower() switch
                {
                    "string" => SQLiteHelper.ColumnType.TEXT,
                    "boolean" => SQLiteHelper.ColumnType.INTEGER,
                    "byte" => SQLiteHelper.ColumnType.INTEGER,
                    "sbyte" => SQLiteHelper.ColumnType.INTEGER,
                    "int16" => SQLiteHelper.ColumnType.INTEGER,
                    "uint16" => SQLiteHelper.ColumnType.INTEGER,
                    "int32" => SQLiteHelper.ColumnType.INTEGER,
                    "uint32" => SQLiteHelper.ColumnType.INTEGER,
                    "int64" => SQLiteHelper.ColumnType.INTEGER,
                    "uint64" => SQLiteHelper.ColumnType.INTEGER,
                    "single" => SQLiteHelper.ColumnType.REAL,
                    "double" => SQLiteHelper.ColumnType.REAL,
                    _ => SQLiteHelper.ColumnType.BLOB
                }
                let name = string.IsNullOrEmpty(attribute.Name) ? field.Name.ToUpper() : attribute.Name
                select new SQLiteHelper.ColumnDef(name, col_type, attribute.IsPrimary, attribute.IsUnique,
                    attribute.IsNotNull, attribute.DefaultValue, attribute.Check)
            ).ToList();

            OperatingDB.CreateTable(tableName, tableItems);
        }
    }

    public void CleanTempDataBase()
    {
        OperatingDB?.Close();
        OperatingDB = null;
        OperatingDataBasePath = Path.Combine(Main.mainPath, ".tmp.db");
        if (File.Exists(OperatingDataBasePath)) File.Delete(OperatingDataBasePath);
    }
}