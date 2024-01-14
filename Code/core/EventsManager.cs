using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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
        #region 基础对象

        OperatingDB.CreateTable("Kingdom", new List<SQLiteHelper.ColumnDef>
        {
            new("ID", SQLiteHelper.ColumnType.TEXT, true),
            new("CURR_NAME"),
            new("BANNER_ICON", SQLiteHelper.ColumnType.INTEGER),
            new("BANNER_BG", SQLiteHelper.ColumnType.INTEGER),
            new("START_TIME", SQLiteHelper.ColumnType.REAL),
            new("END_TIME", SQLiteHelper.ColumnType.REAL, pDefault: "-1")
        });
        OperatingDB.CreateTable("City", new List<SQLiteHelper.ColumnDef>
        {
            new("ID", SQLiteHelper.ColumnType.TEXT, true),
            new("CURR_NAME", pDefault: "NO_NAME"),
            new("CURR_KINGDOM"),
            new("START_TIME", SQLiteHelper.ColumnType.REAL),
            new("END_TIME", SQLiteHelper.ColumnType.REAL, pDefault: "-1")
        });
        OperatingDB.CreateTable("War", new List<SQLiteHelper.ColumnDef>
        {
            new("ID", SQLiteHelper.ColumnType.TEXT, true),
            new("CURR_NAME"),
            new("START_TIME", SQLiteHelper.ColumnType.REAL),
            new("END_TIME", SQLiteHelper.ColumnType.REAL, pDefault: "-1")
        });
        OperatingDB.CreateTable("Alliance", new List<SQLiteHelper.ColumnDef>
        {
            new("ID", SQLiteHelper.ColumnType.TEXT, true),
            new("CURR_NAME"),
            new("BANNER_ICON", SQLiteHelper.ColumnType.INTEGER),
            new("BANNER_BG", SQLiteHelper.ColumnType.INTEGER),
            new("START_TIME", SQLiteHelper.ColumnType.REAL),
            new("END_TIME", SQLiteHelper.ColumnType.REAL, pDefault: "-1")
        });

        #endregion

        #region 事件

        OperatingDB.CreateTable("KingdomChangeName", new List<SQLiteHelper.ColumnDef>
        {
            new("ID"),
            new("TIMESTAMP", SQLiteHelper.ColumnType.REAL),
            new("OLD_NAME")
        });
        OperatingDB.CreateTable("KingdomChangeYear", new List<SQLiteHelper.ColumnDef>
        {
            new("ID"),
            new("TIMESTAMP", SQLiteHelper.ColumnType.REAL),
            new("OLD_NAME")
        });
        OperatingDB.CreateTable("CityChangeName", new List<SQLiteHelper.ColumnDef>
        {
            new("ID"),
            new("TIMESTAMP", SQLiteHelper.ColumnType.REAL),
            new("OLD_NAME")
        });
        OperatingDB.CreateTable("KingSet", new List<SQLiteHelper.ColumnDef>
        {
            new("KID"),
            new("TIMESTAMP", SQLiteHelper.ColumnType.REAL),
            new("AID")
        });
        OperatingDB.CreateTable("KingdomWar", new List<SQLiteHelper.ColumnDef>
        {
            new("KID"),
            new("TIMESTAMP", SQLiteHelper.ColumnType.REAL),
            new("WID"),
            new("DEAD", SQLiteHelper.ColumnType.INTEGER, pDefault: "0"),
            new("KILL", SQLiteHelper.ColumnType.INTEGER, pDefault: "0"),
            new("EVENT_NAME")
        });

        #endregion
    }

    public void CleanTempDataBase()
    {
        OperatingDB?.Close();
        OperatingDB = null;
        OperatingDataBasePath = Path.Combine(Main.mainPath, ".tmp.db");
        if (File.Exists(OperatingDataBasePath)) File.Delete(OperatingDataBasePath);
    }
}