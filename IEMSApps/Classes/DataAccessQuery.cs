using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IEMSApps.BLL;
using IEMSApps.BusinessObject;
using IEMSApps.Utils;
using Newtonsoft.Json;
using SQLite.Net;
using SQLite.Net.Platform.SQLCipher.XamarinAndroid;
using Environment = Android.OS.Environment;

namespace IEMSApps.Classes
{
    public static class DataAccessQuery<T> where T : class
    {
        private const string ClassName = "DataAccessQuery";

        private static readonly string PathDb = Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                                               Constants.DatabasePath + Constants.DatabaseName;

        private static SQLiteConnection _sQLiteConnection;
        private static SQLiteConnection GetConnection()
        {
            if (_sQLiteConnection == null)
                _sQLiteConnection = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb);
            return _sQLiteConnection;
        }

        public static BaseResult CreateTable()
        {
            var result = new BaseResult();
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                conn.CreateTable<T>();
                //}
            }
            catch (SQLiteException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                GeneralBll.LogDataWithException(ClassName, "CreateTable", ex);
            }

            return result;
        }


        /// <summary>
        /// this function will execute Script of SQL, and will return int
        /// 1 if the query is successfully executed
        /// -1 if the query has error
        /// this function will use like Insert, Udpate and Delete
        /// </summary>
        /// <param name="sqlScript"></param>
        /// <returns></returns>
        public static int ExecuteSql(string sqlScript)
        {
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                var sqlCommand = conn.CreateCommand(sqlScript);
                return sqlCommand.ExecuteNonQuery();
                //}
            }
            catch (Exception ex)
            {
                GeneralBll.LogDataWithException(ClassName, "ExecuteSql", ex);
                Log.WriteLogFile(ClassName, "ExecuteSql", "Script : " + sqlScript, Enums.LogType.Error);
                return -1;
            }
        }

        /// <summary>
        /// this function will execute Script of SQL, and will return the datas
        /// this function will use for select
        /// </summary>
        /// <param name="sqlScript"></param>
        /// <returns></returns>
        public static List<T> ExecuteSelectSql(string sqlScript)
        {
            var result = new List<T>();
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                result = conn.Query<T>(sqlScript);
                //}
            }
            catch (Exception ex)
            {
                GeneralBll.LogDataWithException(ClassName, "ExecuteSelectSql", ex);
                Log.WriteLogFile(ClassName, "ExecuteSelectSql", "script : " + sqlScript, Enums.LogType.Error);
            }

            return result;
        }

        public static Result<T> Update(T entity)
        {
            var result = new Result<T>();
            try
            {
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                var conn = GetConnection();
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                conn.Update(entity);
                //}
            }
            catch (SQLiteException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                Log.WriteLogFile(ClassName, "Update", "DataType : " + entity.GetType().Name, Enums.LogType.Error);
                Log.WriteLogFile(ClassName, "Update", "Data : " + JsonConvert.SerializeObject(entity), Enums.LogType.Error);
                GeneralBll.LogDataWithException(ClassName, "Update", ex);
            }
            return result;
        }

        public static Result<T> Delete(T entity)
        {
            var result = new Result<T>();
            try
            {
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                var conn = GetConnection();
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                conn.Delete(entity);
                //}
            }
            catch (SQLiteException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                Log.WriteLogFile(ClassName, "Delete", "DataType : " + entity.GetType().Name, Enums.LogType.Error);
                Log.WriteLogFile(ClassName, "Delete", "Data : " + JsonConvert.SerializeObject(entity), Enums.LogType.Error);
                GeneralBll.LogDataWithException(ClassName, "Delete", ex);
            }
            return result;
        }

        public static Result<T> Insert(T entity)
        {
            var result = new Result<T>();


            try
            {
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                var conn = GetConnection();
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                conn.Insert(entity);
                //}
            }
            catch (SQLiteException ex)
            {
                result.Success = false;
                result.Message = ex.Message;

                Log.WriteLogFile(ClassName, "Insert", "DataType : " + entity.GetType().Name, Enums.LogType.Error);
                Log.WriteLogFile(ClassName, "Insert", "Data : " + JsonConvert.SerializeObject(entity), Enums.LogType.Error);
                GeneralBll.LogDataWithException(ClassName, "Insert", ex);
            }
            return result;
        }

        public static Result<List<T>> GetAll()
        {
            var result = new Result<List<T>>();
            try
            {
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                var conn = GetConnection();
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                var data = conn.Table<T>().ToList();
                result.Datas = data;
                //}
            }
            catch (SQLiteException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                GeneralBll.LogDataWithException(ClassName, "GetAll", ex);
            }
            return result;
        }

        public static Result<T> Get(Expression<Func<T, bool>> predicate)
        {
            var result = new Result<T>() { Datas = null };
            try
            {
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                var conn = GetConnection();
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                var data = conn.Table<T>().Where(predicate).FirstOrDefault();
                result.Datas = data;
                //}
            }
            catch (SQLiteException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                GeneralBll.LogDataWithException(ClassName, "Get", ex);
            }
            return result;
        }

        public static Result<List<T>> GetAll(Expression<Func<T, bool>> predicate)
        {
            var result = new Result<List<T>>();
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                var data = conn.Table<T>().Where(predicate).ToList();
                result.Datas = data;
                //}
            }
            catch (SQLiteException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                GeneralBll.LogDataWithException(ClassName, "Get", ex);
            }
            return result;
        }

        public static bool DeleteAll()
        {
            var result = true;
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                conn.DeleteAll<T>();
                //}
            }
            catch (SQLiteException ex)
            {
                GeneralBll.LogDataWithException(ClassName, "DeleteAll", ex);
                result = false;
            }
            return result;
        }

        public static bool ExecuteCheckSelectSql(string sqlScript, bool log = false)
        {
            var result = new List<T>();
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                result = conn.Query<T>(sqlScript);
                return true;
                //}
            }
            catch (Exception ex)
            {
                if (log)
                {
                    GeneralBll.LogDataWithException(ClassName, "ExecuteSelectSql", ex);
                }
                return false;
            }
        }

        public static int Count(Expression<Func<T, bool>> predicate)
        {
            var result = -1;
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                result = conn.Table<T>().Where(predicate).Count();
                //}
            }
            catch (SQLiteException ex)
            {
                result = -1;
                GeneralBll.LogDataWithException(ClassName, "Count", ex);
            }
            return result;
        }

        public static int Count()
        {
            var result = 0;
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                result = conn.Table<T>().Count();
                //}
            }
            catch (SQLiteException ex)
            {
                result = 0;
                GeneralBll.LogDataWithException(ClassName, "Any", ex);
            }
            return result;
        }

        public static bool Any(string sqlScript)
        {
            var result = false;
            try
            {
                var conn = GetConnection();
                //using (var conn = new SQLiteConnection(new SQLitePlatformAndroid(Constants.Password), PathDb))
                //{
                conn.ExecuteScalar<int>($"PRAGMA cipher_license = '{Constants.LicenseKey}';");
                result = conn.Query<T>(sqlScript).Any();
                //}
            }
            catch (Exception ex)
            {
                GeneralBll.LogDataWithException(ClassName, "ExecuteSelectSql", ex);
                Log.WriteLogFile(ClassName, "ExecuteSelectSql", "script : " + sqlScript, Enums.LogType.Error);
            }

            return result;
        }
    }
}