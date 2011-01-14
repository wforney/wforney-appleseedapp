using System;
using System.Data.SqlClient;
using System.Web;
using Appleseed.Framework.Data;
using Appleseed.Framework.Exceptions;

namespace Appleseed.Framework.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Gets the db key.
        /// </summary>
        /// <value>The db key.</value>
        public static string dbKey
        {
            get
            {
                string dbKey = "CurrentDatabase";
                if (Config.EnableMultiDbSupport)
                    dbKey = "DatabaseVersion_" + Config.SqlConnectionString.DataSource + "_" +
                            Config.SqlConnectionString.Database; // For multidb support
                return dbKey;
            }
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <value>The database version.</value>
        public static int DatabaseVersion
        {
            //by Manu 16/10/2003
            //Added 2 mods:
            //1) Rbversion is created if it is missed.
            //   This is expecially good for empty databases.
            //   Be aware that this can break compatibility with 1613 version
            //2) Connection problems are thown immediately as errors.
            get
            {
                //Caches dbversion
                int curVersion = 0;

                if (HttpContext.Current.Application[dbKey] == null)
                {
                    try
                    {
                        //Create rbversion if it is missing
                        string createRbVersions =
                            "IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rb_Versions]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)" +
                            "CREATE TABLE [rb_Versions] (" +
                            "[Release] [int] NOT NULL , " +
                            "[Version] [nvarchar] (50) NULL , " +
                            "[ReleaseDate] [datetime] NULL " +
                            ") ON [PRIMARY]"
                            ;
                        DBHelper.ExeSQL(createRbVersions);
                    }

                    catch (SqlException ex)
                    {
                        throw new DatabaseUnreachableException(
                            "Failed to get Database Version - most likely cannot connect to db or no permission.", ex);
                        // Jes1111
                        //Appleseed.Framework.Configuration.ErrorHandler.HandleException("If this fails most likely cannot connect to db or no permission", ex);
                        //If this fails most likely cannot connect to db or no permission
                        //throw;
                    }
                    object version =
                        DBHelper.ExecuteSQLScalar("SELECT TOP 1 Release FROM rb_Versions ORDER BY Release DESC");

                    if (version != null)
                        curVersion = Int32.Parse(version.ToString());

                    else
                    {
                        curVersion = 1110;
                        // TODO: This should be the best place
                        // where run the codefor empty db
                    }
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[dbKey] = curVersion;
                    HttpContext.Current.Application.UnLock();
                }
                return (int) HttpContext.Current.Application[dbKey];
            }
        }
    }
}