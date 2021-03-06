﻿using Library.V1.Common;
using Library.V1.SQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Entity
{
    public class Database : IDisposable
    {
        #region Constructor
        public Database(AppSetting appConfig)
        {
            this.Method = "get";
            this.GetUrl = string.Empty;
            this.SaveUrl = string.Empty;

            this.Tables = new Dictionary<string, Table>();
            this.Other = new Dictionary<string, object>();
            this.Collections = new Dictionary<string, Collection>();
            this.User = new AdminUser();
            this.Error = new Error();

            this.DSQL = new SqlHelper(appConfig.DataAccount);
            this.FSQL = new SqlHelper(appConfig.FileAccount);
            this.IsDebug = this.DSQL.IsDebug;
        }
        public Database(AppSetting appConfig, AdminUser user):this(appConfig)
        {
            this.User = user;
        }
        public void Dispose()
        {
            this.DSQL.Close();
            this.DSQL.Dispose();
            this.FSQL.Close();
            this.FSQL.Dispose();
        }
        #endregion

        #region Public Fields
        public string Method { get; set; }
        public string GetUrl { get; set; }
        public string SaveUrl { get; set; }
        public bool IsDebug { get; set; }
        [JsonIgnore]
        public SqlHelper DSQL { get; set; }
        [JsonIgnore]
        public SqlHelper FSQL { get; set; }
        public Dictionary<string, Table> Tables { get; set; }
        public Dictionary<string, object> Other { get; set; }
        public Dictionary<string, Collection> Collections { get; set; }
        public AdminUser User { get; set; }
        public Error Error { get; set; }
        #endregion

        #region Database Methods
        public Database AddTable(Table table)
        {
            table.SetConfig(this.DSQL, this.FSQL, this.User);
            this.Tables.Add(table.Name, table);
            return this;
        }
        public Database AddTables(params Table[] tables)
        {
            foreach (Table tb in tables) this.AddTable(tb);
            return this;
        }
        public Database AddCollection(Collection collect)
        {
            collect.SetConfig(this.DSQL);
            this.Collections.Add(collect.Name, collect);
            return this;
        }
        public Database AddCollections(params Collection[] collects)
        {
            foreach (Collection cl in collects) this.AddCollection(cl);
            return this;
        }
        #endregion

        #region Database FillData Method
        public Database FillCollection(string name)
        {
            if (this.Collections.ContainsKey(name))
            {
                this.Collections[name].FillData();
            }
            return this;
        }
        public Database FillTable(string name)
        {
            if (this.Tables.ContainsKey(name))
            {
                this.Tables[name].FillData();
            }
            return this;
        }
        public Database FillAll()
        {
            foreach (string tableName in this.Tables.Keys)
            {
                this.FillTable(tableName);
            }
            foreach (string collectionName in this.Collections.Keys)
            {
                this.FillCollection(collectionName);
            }
            return this;
        }
        public Table ReloadTable(JSTable jsTable)
        {
            if (this.Tables.ContainsKey(jsTable.Name))
                return this.Tables[jsTable.Name].ReloadData(jsTable);
            else
                return new Table();
        }
        #endregion

        #region Database SaveData Method
        public Table ValidateTable(JSTable jsTable)
        {
            if (this.Tables.ContainsKey(jsTable.Name))
            {
                return this.Tables[jsTable.Name].ValidateTable(jsTable);
            }
            else
            {
                return new Table();
            }
        }
        public Table SaveTable(JSTable jstable)
        {
            if (this.Tables.ContainsKey(jstable.Name))
            {
                return this.Tables[jstable.Name].SaveData(jstable);
            }
            else
            {
                return new Table();
            }
        }

        #endregion
    }
}
