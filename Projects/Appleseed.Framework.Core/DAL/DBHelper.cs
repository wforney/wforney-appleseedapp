using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Appleseed.Framework.Exceptions;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Data
{
    /// <summary>
    /// Summary description for DBHelper
    /// </summary>
    public class DBHelper
    {
        /// <summary>
        /// Execute script using transaction
        /// </summary>
        /// <param name="scriptPath">The script path.</param>
        /// <param name="useTransaction">if set to <c>true</c> [use transaction].</param>
        /// <returns></returns>
        public static ArrayList ExecuteScript(string scriptPath, bool useTransaction)
        {
            return ExecuteScript(scriptPath, Config.SqlConnectionString, useTransaction);
        }

        /// <summary>
        /// Execute script (no transaction)
        /// </summary>
        /// <param name="scriptPath">The script path.</param>
        /// <returns></returns>
        public static ArrayList ExecuteScript(string scriptPath)
        {
            return ExecuteScript(scriptPath, Config.SqlConnectionString);
        }

        /// <summary>
        /// Execute script using transaction
        /// </summary>
        /// <param name="scriptPath">The script path.</param>
        /// <param name="myConnection">My connection.</param>
        /// <param name="useTransaction">if set to <c>true</c> [use transaction].</param>
        /// <returns></returns>
        public static ArrayList ExecuteScript(string scriptPath, SqlConnection myConnection, bool useTransaction)
        {
            if (!useTransaction)
                return ExecuteScript(scriptPath, myConnection); //FIX: Must pass connection as well
            string strScript = GetScript(scriptPath);
            ErrorHandler.Publish(LogLevel.Info, "Executing Script '" + scriptPath + "'");
            ArrayList errors = new ArrayList();
            // Subdivide script based on GO keyword
            string[] sqlCommands = Regex.Split(strScript, "\\sGO\\s", RegexOptions.IgnoreCase);
            //Open connection
            myConnection.Open();
            //Wraps execution on a transaction 
            //so we know that the script runs or fails
            SqlTransaction myTrans;
            string transactionName = "Appleseed";
            myTrans = myConnection.BeginTransaction(IsolationLevel.RepeatableRead, transactionName);
            ErrorHandler.Publish(LogLevel.Debug, "Start Script Transaction ");

            try
            {
                //Cycles command and execute them
                for (int s = 0; s <= sqlCommands.GetUpperBound(0); s++)
                {
                    string mySqlText = sqlCommands[s].Trim();

                    try
                    {
                        if (mySqlText.Length > 0)
                        {
                            //Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Configuration.LogLevel.Debug, "Executing: " + mySqlText.Replace("\n", " "));
                            // Must assign both transaction object and connection
                            // to Command object for a pending local transaction
                            using (SqlCommand sqldbCommand = new SqlCommand())
                            {
                                sqldbCommand.Connection = myConnection;
                                sqldbCommand.CommandType = CommandType.Text;
                                sqldbCommand.Transaction = myTrans;
                                sqldbCommand.CommandText = mySqlText;
                                sqldbCommand.CommandTimeout = 150;
                                sqldbCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        myTrans.Rollback();
                        errors.Add("<P>"
                                   + ex.Message + "<br>"
                                   + mySqlText
                                   + "</P>");
                        ErrorHandler.Publish(LogLevel.Warn, "ExecuteScript Failed: " + mySqlText, ex);
                        throw new DatabaseUnreachableException("ExecuteScript Failed: " + mySqlText, ex);
                    }
                }
                // Succesfully applied this script
                myTrans.Commit();
                ErrorHandler.Publish(LogLevel.Debug, "Commit Script Transaction.");
            }

            catch (Exception ex)
            {
                errors.Add(ex.Message);
                int count = 0;

                while (ex.InnerException != null && count < 100)
                {
                    ex = ex.InnerException;
                    errors.Add(ex.Message);
                    count++;
                }
            }

            finally
            {
                if (myConnection.State == ConnectionState.Open)
                    myConnection.Close();
            }
            return errors;
        }

        /// <summary>
        /// Execute script (no transaction)
        /// </summary>
        /// <param name="scriptPath">The script path.</param>
        /// <param name="myConnection">My connection.</param>
        /// <returns></returns>
        public static ArrayList ExecuteScript(string scriptPath, SqlConnection myConnection)
        {
            string strScript = GetScript(scriptPath);
            ErrorHandler.Publish(LogLevel.Info, "Executing Script '" + scriptPath + "'");
            ArrayList errors = new ArrayList();
            // Subdivide script based on GO keyword
            string[] sqlCommands = Regex.Split(strScript, "\\sGO\\s", RegexOptions.IgnoreCase);

            try
            {
                //Cycles command and execute them
                for (int s = 0; s <= sqlCommands.GetUpperBound(0); s++)
                {
                    string mySqlText = sqlCommands[s].Trim();

                    try
                    {
                        if (mySqlText.Length > 0)
                        {
                            //Open connection
                            myConnection.Open();

                            ErrorHandler.Publish(LogLevel.Debug, "Executing: " + mySqlText.Replace("\n", " "));
                            using (SqlCommand sqldbCommand = new SqlCommand())
                            {
                                sqldbCommand.Connection = myConnection;
                                sqldbCommand.CommandType = CommandType.Text;
                                sqldbCommand.CommandText = mySqlText;
                                sqldbCommand.CommandTimeout = 150;
                                sqldbCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        errors.Add("<P>"
                                   + ex.Message + "<br>"
                                   + mySqlText
                                   + "</P>");
                        ErrorHandler.Publish(LogLevel.Warn, "ExecuteScript Failed: " + mySqlText, ex);
                        // Rethrow exception
                        throw new AppleseedException(LogLevel.Fatal, HttpStatusCode.ServiceUnavailable,
                                                   "Script failed, please correct the error and retry: " + mySqlText, ex);
                        //throw new Exception("Script failed, please correct the error and retry", ex);
                    }

                    finally
                    {
                        if (myConnection.State == ConnectionState.Open)
                            myConnection.Close();
                    }
                }
            }

            catch (Exception ex)
            {
                errors.Add(ex.Message);
                int count = 0;

                while (ex.InnerException != null && count < 100)
                {
                    ex = ex.InnerException;
                    errors.Add(ex.Message);
                    count++;
                }
            }
            return errors;
        }

        /// <summary>
        /// Executes the SQL scalar.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>A object value...</returns>
        public static object ExecuteSQLScalar(string sql)
        {
            object returnValue;

            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand sqlCommand = new SqlCommand(sql, myConnection))
                {
                    try
                    {
                        sqlCommand.Connection.Open();
                        returnValue = sqlCommand.ExecuteScalar();
                    }

                    catch (Exception e)
                    {
                        throw new DatabaseUnreachableException("Error in DBHelper - ExecuteSQLScalar", e);
                    }

                    finally
                    {
                        sqlCommand.Dispose();
                        myConnection.Close();
                        myConnection.Dispose();
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Exes the SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>A int value...</returns>
        public static Int32 ExeSQL(string sql)
        {
            int returnValue = -1;

            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand sqlCommand = new SqlCommand(sql, myConnection))
                {
                    try
                    {
                        sqlCommand.Connection.Open();
                        returnValue = sqlCommand.ExecuteNonQuery();
                    }

                    catch (Exception e)
                    {
                        ErrorHandler.Publish(LogLevel.Error, "Error in DBHelper - ExeSQL - SQL: '" + sql + "'", e);
                        throw new DatabaseUnreachableException("Error in DBHelper - ExeSQL", e);
                        //throw new Exception("Error in DBHelper:ExeSQL()-> " + e.ToString());
                    }

                    finally
                    {
                        sqlCommand.Dispose();
                        myConnection.Close();
                        myConnection.Dispose();
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the data reader.
        /// </summary>
        /// <param name="selectCmd">The select CMD.</param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
        // TODO --> [Obsolete("Replace me")]
        public static SqlDataReader GetDataReader(string selectCmd)
        {
            SqlConnection myConnection = Config.SqlConnectionString;

            using (SqlCommand sqlCommand = new SqlCommand(selectCmd, myConnection))
            {
                try
                {
                    sqlCommand.Connection.Open();
                }

                catch (Exception e)
                {
                    throw new DatabaseUnreachableException("Error in DBHelper - GetDataReader", e);
                    //throw new Exception("Error in DBHelper::GetDataReader()-> " + e.ToString());
                }
                return sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="selectCmd">The select CMD.</param>
        /// <returns>A System.Data.DataSet value...</returns>
        public static DataSet GetDataSet(string selectCmd)
        {
            DataSet ds;

            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlDataAdapter m_SqlDataAdapter = new SqlDataAdapter(selectCmd, myConnection))
                {
                    try
                    {
                        ds = new DataSet();
                        m_SqlDataAdapter.Fill(ds, "Table0");
                    }

                    catch (Exception e)
                    {
                        throw new DatabaseUnreachableException("Error in DBHelper - GetDataSet", e);
                        //throw new Exception("Error in ItemBase:GetDataSet()-> " + e.ToString());
                    }

                    finally
                    {
                        m_SqlDataAdapter.Dispose();
                        myConnection.Close();
                        myConnection.Dispose();
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// Get the script from a file
        /// </summary>
        /// <param name="scriptPath">The script path.</param>
        /// <returns></returns>
        private static string GetScript(string scriptPath)
        {
            string strScript;

            // Load script file 
            //using (System.IO.StreamReader objStreamReader = System.IO.File.OpenText(scriptPath)) 
            //http://support.Appleseedportal.net/jira/browse/RBP-693
            //to make it possible to have german umlauts or other special characters in the install_scripts
            using (StreamReader objStreamReader = new StreamReader(scriptPath, Encoding.Default))
            {
                strScript = objStreamReader.ReadToEnd();
                objStreamReader.Close();
            }
            return strScript + Environment.NewLine; //Append carriage for execute last command 
        }
    }
}