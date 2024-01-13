using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Figurebox.Utils;

namespace Figurebox.core;

public class EventsManager
{
    private bool InitializeSuccessful = true;

    private string OperatingDataBasePath;
    private SQLiteConnection OperatingDB;
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
        OperatingDB.CreateTable("Kingdom", new List<SQLiteHelper.ColumnDef>
        {
            new()
            {
                Name = "ID",
                IsPrimary = true,
                ValueType = SQLiteHelper.ColumnType.TEXT
            },
            new()
            {
                Name = "CURR_NAME",
                ValueType = SQLiteHelper.ColumnType.TEXT
            },
            new()
            {
                Name = "BANNER_ICON",
                ValueType = SQLiteHelper.ColumnType.INTEGER
            },
            new()
            {
                Name = "BANNER_BG",
                ValueType = SQLiteHelper.ColumnType.INTEGER
            }
        });
    }

    public void CleanTempDataBase()
    {
        OperatingDB?.Close();
        OperatingDB = null;
        OperatingDataBasePath = Path.Combine(Main.mainPath, ".tmp.db");
        if (File.Exists(OperatingDataBasePath)) File.Delete(OperatingDataBasePath);
    }
}