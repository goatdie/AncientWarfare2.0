using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Figurebox.Utils;

namespace Figurebox.core.dbs;

public abstract class AMultiTableDBManager<T> where T : AMultiTableDBManager<T>
{
    public static T                            Instance;
    protected     List<SQLiteHelper.ColumnDef> ColumnDefs;
    private       bool                         m_InitializeSuccessful = true;

    protected     SQLiteConnection OperatingDB;
    public static bool             InitializeSuccessful => Instance.m_InitializeSuccessful;

    protected abstract void PrepareTableDef();

    protected void CheckTable(string pTableName)
    {
        if (!InitializeSuccessful) return;
        if (TableExists(pTableName)) return;
        try
        {
            OperatingDB?.CreateTable(pTableName, ColumnDefs);
            OperatingDB?.Insert("TABLE_NAME", ColumnVal.Create("NAME", pTableName));
        }
        catch (Exception e)
        {
            m_InitializeSuccessful = false;
            Main.LogWarning($"Failed to create table {pTableName} in {typeof(T).Name}, events will not be stored");
            Main.LogWarning(e.Message);
            Main.LogWarning(e.StackTrace);
        }
    }

    private bool TableExists(string pTableName)
    {
        return OperatingDB.CheckKeyExist("TABLE_NAME", SimpleColumnConstraint.CreateEq("NAME", pTableName));
    }

    public void CreateDataBase()
    {
        if (ColumnDefs == null) PrepareTableDef();
        try
        {
            CleanTempDataBase();
            SQLiteConnection.CreateFile(GetDBFilePath());
            OperatingDB = new SQLiteConnection("data source=" + GetDBFilePath());
            OperatingDB.Open();

            OperatingDB.CreateTable("TABLE_NAME",
                                    new List<SQLiteHelper.ColumnDef>(new[]
                                                                     {
                                                                         new SQLiteHelper.ColumnDef("NAME",
                                                                                  pIsPrimary: true)
                                                                     }));
        }
        catch (Exception e)
        {
            m_InitializeSuccessful = false;
            Main.LogWarning("Failed to create database, events will not be stored");
            Main.LogWarning(e.Message);
            Main.LogWarning(e.StackTrace);
        }
    }

    protected void CleanTempDataBase()
    {
        OperatingDB?.Close();
        OperatingDB = null;

        if (File.Exists(GetDBFilePath())) File.Delete(GetDBFilePath());
    }

    protected abstract string GetDBFilePath();
}