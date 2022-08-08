using IEMSApps.BLL;
using IEMSApps.BusinessObject.Entities;
using System;
using System.IO;

namespace IEMSApps.Utils
{
    public static class Log
    {
        public static void WriteLogFile(string log)
        {
            var dtLocalTime = DateTime.Now;

            string strFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                             Constants.LogPath + string.Format("Log_{0}.txt", dtLocalTime.ToString("yyyyMMdd"));

            var objWrite =
                new StreamWriter(strFile, true);
            string sLine = log;

            sLine = dtLocalTime.ToString("yyyy-MM-dd HH:mm:ss : ") + sLine + "\r";//Char(13);
            objWrite.WriteLine(sLine);
            objWrite.Close();
            objWrite.Dispose();
        }

        public static void WriteLogFile(string log, Enums.LogType logType)
        {
            log = logType + " : " + log;
            WriteLogFile(log);
        }

        public static void WriteLogFile(string className, string message, Enums.LogType logType)
        {
            string log = logType + " : " + className;
            log += "\r";
            log += logType + " : " + message;
            WriteLogFile(log);
        }

        public static void WriteLogFile(string className, string functionName, string message, Enums.LogType logType)
        {
            string log = logType + " : " + className;
            log += "\r";
            log += "Function Name : " + functionName;
            log += "\r";
            log += "message : " + message;
            WriteLogFile(log);
        }

        public static void WriteLogFile(string log, Enums.LogType logType, Enums.FileLogType fileLogType)
        {
            log = logType + " : " + log;
            WriteLogFile(log, fileLogType);
        }

        public static void WriteLogFile(string log, Enums.FileLogType fileLogType)
        {
            var dtLocalTime = DateTime.Now;

            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + Constants.LogPath;

            string strFile = string.Format("{0}Log{1}_{2}.txt", path, dtLocalTime.ToString("yyyyMMdd"), fileLogType.ToString());

            var objWrite =
                new StreamWriter(strFile, true);
            string sLine = log;

            sLine = dtLocalTime.ToString("yyyy-MM-dd HH:mm:ss : ") + sLine + "\r";//Char(13);
            objWrite.WriteLine(sLine);
            objWrite.Close();
            objWrite.Dispose();
        }

        public static void WritePrintIntoFile(string data)
        {
            var dtLocalTime = DateTime.Now;
            string strFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                             Constants.LogPath + string.Format("PrintToFile_{0}.txt", dtLocalTime.ToString("yyyyMMdd"));
            StreamWriter objWrite = null;
            try
            {

                objWrite = new StreamWriter(strFile, true);
                objWrite.WriteLine(data);
            }
            catch { }
            finally
            {
                if (objWrite != null)
                {
                    objWrite.Close();
                    objWrite.Dispose();
                }
            }
        }

        public static void WriteLogFileServices(string log)
        {
            var dtLocalTime = DateTime.Now;

            string strFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                             Constants.LogPath + string.Format("LogService_{0}.txt", dtLocalTime.ToString("yyyyMMdd"));

            var objWrite =
                new StreamWriter(strFile, true);
            string sLine = log;

            sLine = dtLocalTime.ToString("yyyy-MM-dd HH:mm:ss : ") + sLine + "\r";//Char(13);
            objWrite.WriteLine(sLine);
            objWrite.Close();
            objWrite.Dispose();
        }

        public static void WriteLogFileBackupServices(string className, string log)
        {
            var dtLocalTime = DateTime.Now;

            string strFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                             Constants.LogPath + string.Format("LogBackupService_{0}.txt", dtLocalTime.ToString("yyyyMMdd"));

            var objWrite =
                new StreamWriter(strFile, true);
            string sLine = className + " : " + log;

            sLine = dtLocalTime.ToString("yyyy-MM-dd HH:mm:ss : ") + sLine + "\r";//Char(13);
            objWrite.WriteLine(sLine);
            objWrite.Close();
            objWrite.Dispose();
        }

        public static void WriteLogBackgroundService(string log)
        {
            var dtLocalTime = DateTime.Now;

            string strFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                             Constants.LogPath + string.Format("BackgroundService_{0}.txt", dtLocalTime.ToString("yyyyMMdd"));

            var objWrite =
                new StreamWriter(strFile, true);
            string sLine = log;

            sLine = dtLocalTime.ToString("yyyy-MM-dd HH:mm:ss : ") + sLine + "\r";//Char(13);
            objWrite.WriteLine(sLine);
            objWrite.Close();
            objWrite.Dispose();
        }


        public static void WriteErrorRecords(string errorRecord)
        {
            //var dtLocalTime = DateTime.Now;

            //string strFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + Constants.LogPath + Constants.ErrorRecordsPath;
            //var objWrite = new StreamWriter(strFile, true);
            //string sLine = errorRecord;

            //sLine = dtLocalTime.ToString("yyyy-MM-dd HH:mm:ss : ") + sLine + "\r";//Char(13);
            //objWrite.WriteLine(sLine);
            //objWrite.Close();
            //objWrite.Dispose();

            //if (errorRecord.Contains("Faild to Upload Image")) return;
            try
            {
                var tbError = new TbError
                {
                    IdHH = GeneralBll.GetUserHandheld(),
                    KodCawangan = GeneralBll.GetUserCawangan(),
                    Status = ((int)Enums.StatusOnline.New).ToString(),
                    SqlStmt = BLL.GeneralBll.Base64Encode(errorRecord),
                    PgnDaftar = GeneralBll.GetUserStaffId(),
                    TrkhDaftar = GeneralBll.GetLocalDateTimeForDatabase(),
                    PgnAkhir = GeneralBll.GetUserStaffId(),
                    TrkhAkhir = GeneralBll.GetLocalDateTimeForDatabase(),
                };
                Classes.DataAccessQuery<TbError>.Insert(tbError);
            }
            catch (Exception ex)
            {
                WriteLogFile("Error when insert data to TbError Table " + ex.Message, Enums.LogType.Error);
            }

        }
    }
}