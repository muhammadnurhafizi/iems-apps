using System;
using System.Data;
using System.IO;
using IEMSApps.BLL;
using IEMSApps.BusinessObject;
using IEMSApps.Utils;
using Newtonsoft.Json;
using SQLite.Net;
using SQLite.Net.Platform.SQLCipher.XamarinAndroid;
using Environment = Android.OS.Environment;

namespace IEMSApps.Classes
{
    public class DataAccessQueryTrx
    {
        private const string ClassName = "DataAccessQueryTrx";

        private static readonly string PathDb = Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                                                Constants.DatabasePath + Constants.DatabaseName;

        public static readonly string BackupFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                                                   Constants.BackupPath + Constants.BackupName;

        // private SQLiteCommand _mSqlCmd;
        private SQLiteConnection _mCon;

       
        public bool BeginTrx()
        {
            return BeginTrx(true);
        }

        public bool BeginTrx(bool isLog)
        {
            try
            {
                _mCon = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb);
                _mCon.BeginTransaction();

                if (isLog)
                    AppendSqlStmt("BEGINTRANS");

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile(ClassName, "BeginTrx", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
                return false;
            }
        }

        public bool ExecuteQueryTrx(string sQuery)
        {
            return ExecuteQueryTrx(sQuery, true);
        }

        public bool ExecuteQueryTrx(string sQuery, bool isLog)
        {
            try
            {
                if (!_mCon.IsInTransaction)
                    return false;//"No Connection Open. Please BeginTrx First";

                _mCon.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                var sqlCommand = _mCon.CreateCommand(sQuery);
                sqlCommand.ExecuteNonQuery();

                if (isLog)
                    AppendSqlStmt(sQuery);

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile(ClassName, "ExecuteQueryTrx", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile(ClassName, "Value ", sQuery, Enums.LogType.Error);
                Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
                return false;
            }
        }

        public bool InsertTrx<T>(T entity)
        {
            try
            {
                if (!_mCon.IsInTransaction)
                    return false;//"No Connection Open. Please BeginTrx First";

                _mCon.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                _mCon.Insert(entity);
                //if (isLog)
                //    AppendSqlStmt(sQuery);

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile(ClassName, "InsertTrx", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile(ClassName, "Value ", JsonConvert.SerializeObject(entity), Enums.LogType.Error);
                Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
                return false;
            }
        }

        public bool UpdateTrx<T>(T entity)
        {
            try
            {
                if (!_mCon.IsInTransaction)
                    return false;//"No Connection Open. Please BeginTrx First";

                _mCon.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                _mCon.Update(entity);
               
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile(ClassName, "UpdateTrx", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile(ClassName, "Value ", JsonConvert.SerializeObject(entity), Enums.LogType.Error);
                Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
                return false;
            }
        }

        public bool DeleteTrx<T>(T entity)
        {
            try
            {
                if (!_mCon.IsInTransaction)
                    return false;//"No Connection Open. Please BeginTrx First";

                _mCon.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                _mCon.Delete(entity);

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile(ClassName, "DeleteTrx", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile(ClassName, "Value ", JsonConvert.SerializeObject(entity), Enums.LogType.Error);
                Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
                return false;
            }
        }

        public bool CommitTrx()
        {
            return CommitTrx(true);
        }

        public bool CommitTrx(bool isLog)
        {
            try
            {
                if (!_mCon.IsInTransaction)
                    return false;

                _mCon.Commit();

                _mCon.Close();
                _mCon.Dispose();

                if (isLog)
                    AppendSqlStmt("COMMITTRANS");

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile(ClassName, "CommitTrx", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
                return false;
            }

        }

        public bool RollBackTrx()
        {
            return RollBackTrx(true);
        }

        public bool RollBackTrx(bool isLog)
        {
            try
            {
                if (!_mCon.IsInTransaction)
                    return false;

                _mCon.Rollback();

                _mCon.Close();
                _mCon.Dispose();

                if (isLog)
                    AppendSqlStmt("ROLLBACKTRANS");

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("DataAccess", "RollBackTrx", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
                return false;
            }

        }

        public static bool ExecuteQuery(string sQuery)
        {
            return ExecuteQuery(sQuery, true);
        }

        public static bool ExecuteQuery(string sQuery, bool isLogged)
        {

            try
            {
                using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                {
                    conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                    var sqlCommand = conn.CreateCommand(sQuery);
                    sqlCommand.ExecuteNonQuery();
                }

                //using (var connection = new SqliteConnection(ConnectionString))
                //{
                //    if (connection.State != ConnectionState.Open)
                //        connection.Open();

                //    var cmd = new SqliteCommand();
                //    cmd.Connection = connection;
                //    cmd.CommandText = sQuery;
                //    cmd.ExecuteNonQuery();
                //    connection.Close();
                //}
                return true;

            }
            catch (Exception ex)
            {
                Log.WriteLogFile("DataAccess", "ExecuteQuery", ex.Message, Enums.LogType.Error);
                if (isLogged)
                {
                    Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
                }
                return false;
            }

        }

        private void AppendSqlStmt(string sSql)
        {
            try
            {
             
                StreamWriter sFileLog = new StreamWriter(BackupFile, true);
              
                sSql = sSql.Encrypt();

                sFileLog.WriteLine(sSql);
                sFileLog.Close();
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("DataAccess", "AppendSqlStmt", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
            }
        }
    }
}