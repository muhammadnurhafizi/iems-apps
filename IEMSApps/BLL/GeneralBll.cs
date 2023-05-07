using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Media;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public static class GeneralBll
    {
        private const string ClassName = "GeneralBll";

        public static void InitFolder()
        {
            string strFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath;
            CreateFolder(strFolder);

            var listFolder = new List<string>
            {
                Constants.LogPath,
                Constants.DatabasePath,
                Constants.BackupPath,
                Constants.ImgsPath,
                Constants.ConfigPath,
            };

            foreach (var folder in listFolder)
            {
                CreateFolder(strFolder + folder);
            }
        }

        public static void CreateFolder(string path)
        {
            var tmpFile = new Java.IO.File(path);
            if (!tmpFile.Exists())
                tmpFile.Mkdirs();
        }

        /// <summary>
        /// Determines whether [is file exist] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool IsFileExist(string path)
        {
            return File.Exists(path);
        }

        public static bool IsFileExist(string path, bool fullPath)
        {
            if (fullPath)
                return File.Exists(path);

            return File.Exists(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + path);
        }

        public static bool IsDatabaseExist()
        {
            string strFileDb = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                               Constants.DatabasePath + Constants.DatabaseName;
            return IsFileExist(strFileDb);
        }

        public static DateTime GetLocalDateTime()
        {
            return DateTime.Now;
        }

        public static int ConvertStringToInt(string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        public static decimal ConvertStringToDecimal(string value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return 0;
            }
        }

        public static void ClearUserData()
        {
            SharedPreferences.SaveString(SharedPreferencesKeys.UserId, "");
            SharedPreferences.SaveString(SharedPreferencesKeys.UserName, "");
            SharedPreferences.SaveString(SharedPreferencesKeys.UserNoKp, "");
            SharedPreferences.SaveString(SharedPreferencesKeys.UserKodPasukan, "");
            //SharedPreferences.SaveString(SharedPreferencesKeys.UserKodCawangan, "");
            SharedPreferences.SaveString(SharedPreferencesKeys.UserNegeri, "");
            SharedPreferences.SaveString(SharedPreferencesKeys.IsUserLogin, "");
        }

        public static void LogDataWithException(string className, string functionName, Exception ex)
        {
            Log.WriteLogFile(className, functionName, ex.Message, Enums.LogType.Error);
            Log.WriteLogFile(ex.StackTrace, Enums.LogType.Error);
        }

        public static string GetInternalImagePath()
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                          Constants.ImgsPath;
            return path;
        }

        public static string GetInternalLogsPath()
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                          Constants.LogPath;
            return path;
        }

        public static string GetInternalBackupPath()
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                          Constants.BackupPath;
            return path;
        }

        public static string GetAPKFolderPath()
        {
            var path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                          Constants.APKFolder;

            var tmpFile = new Java.IO.File(path);
            if (!tmpFile.Exists())
                tmpFile.Mkdirs();

            return path;
        }

        //public static bool IsConfigExist()
        //{
        //    var resut = DataAccessQuery<TbKonfigurasi>.GetAll();
        //    if (resut.Success)
        //    {
        //        var config = resut.Datas.FirstOrDefault();
        //        if (config != null)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public static bool IsHandheldExists()
        {
            var result = DataAccessQuery<TbHandheld>.GetAll();
            if (result.Success)
            {
                return result.Datas.Any();
            }
            return false;
        }

        public static int ClearHandle()
        {
            return DataAccessQuery<TbHandheld>.ExecuteSql("DELETE FROM tbhandheld");
        }

        public static string GetKodPasukanUser()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.UserKodPasukan);
        }

        public static string GetKetuaPasukanName()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.KetuaPasukanName);
        }

        public static string GetUserId()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.UserId);
        }

        public static string GetUserName()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.UserName);
        }

        public static string GetUserHandheld()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.UserHandheld);
        }

        public static string GetUserCawangan()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.UserKodCawangan);
        }

        public static int GetUserStaffId()
        {
            return GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.UserStaffld));
        }

        public static string GetStatePemeriksaan()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.SaveStatePemeriksaan);
        }

        public static string GetKeySelected(Dictionary<string, string> listData, string sValue)
        {
            return listData.FirstOrDefault(c => c.Value == sValue).Key;
        }

        public static string GetLastSaveLongitude()
        {
            try
            {
                return SharedPreferences.GetString(SharedPreferencesKeys.LocationLongitude);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetLastSaveLatitude()
        {
            try
            {
                return SharedPreferences.GetString(SharedPreferencesKeys.LocationLatitude);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetStatusGps()
        {
            try
            {
                return SharedPreferences.GetString(SharedPreferencesKeys.StatusGps);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GenerateNoRujukan(Enums.PrefixType prefixType)
        {
            //to check running number is need reset or not
            //HandheldBll.CheckRunningNumber();

            // <prefix><tbhandheld.idhh><year in YY format><runningNO in 5digit>
            var userHandheld = SharedPreferences.GetString(SharedPreferencesKeys.UserHandheld);

            var handheld = DataAccessQuery<TbHandheld>.Get(c => c.IdHh == userHandheld);

            if (!handheld.Success)
            {
                throw new Java.Lang.Exception("error get data handheld");
            }
            TbHandheld tbHanheld = handheld.Datas;
            int runningNumber = 0;
            string prefixRujukan = "";


            if (tbHanheld != null)
            {
                //to check running number is need reset or not
                tbHanheld = HandheldBll.CheckRunningNumber(tbHanheld);

                if (prefixType == Enums.PrefixType.KPP)
                {
                    runningNumber = tbHanheld.NotUrutan_Kpp;
                    prefixRujukan = prefixType.ToString();
                }
                else if (prefixType == Enums.PrefixType.KOTS)
                {
                    runningNumber = tbHanheld.NotUrutan_Kots;
                    prefixRujukan = "KTS";
                }
                else if (prefixType == Enums.PrefixType.SiasatLanjutan)
                {
                    runningNumber = tbHanheld.NotUrutan_DataKes;
                    prefixRujukan = "DTK";
                }
            }

            runningNumber += 1;


            var localYear = GetLocalDateTime().ToString("yy");
            var noRujukan =
                $"{prefixRujukan}{userHandheld}{localYear}{runningNumber.ToString(Constants.FormatRunningNumber)}";


            return noRujukan;
        }

        public static int GetCountPhotoByRujukan(string noRujukan)
        {
            var dirImagePath = GetInternalImagePath();
            DirectoryInfo di = new DirectoryInfo(dirImagePath);
            FileInfo[] rgFiles = di.GetFiles(noRujukan + "*.*");
            return rgFiles.Count();
        }

        public static void SetFinishPage(bool isSet = false)
        {
            SharedPreferences.SaveString(SharedPreferencesKeys.FinishPage, isSet ? Constants.FinishPage : "");
        }

        public static string GetFinishPage()
        {
            try
            {
                return SharedPreferences.GetString(SharedPreferencesKeys.FinishPage);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ConvertTimeDisplayToDatabase(string sValue)
        {
            //if (sValue.Length == 11) //hh:mm:ss tt
            //{
            //    var sHour = ConvertStringToInt(sValue.Substring(0, 2));

            //    if (sValue.ToLower().Contains("pm"))
            //    {


            //        if (sHour != 12)
            //        {
            //            sHour += 12;
            //        }

            //        var result = sValue.Substring(2, sValue.Length - 5);//:mm:ss

            //        result = sHour + result;

            //        return result;
            //    }
            //    else
            //    {
            //        if (sHour == 12)
            //        {
            //            sHour = 0;
            //        }
            //        var result = sValue.Substring(2, sValue.Length - 5);//:mm:ss
            //        result = sHour.ToString("00") + result;
            //        return result;
            //        //return sValue.Substring(0, sValue.Length - 3);

            //    }

            //}

            return sValue;
        }

        public static string ConvertDateDisplayToDatabase(string sValue)
        {
            if (sValue.Length == 10) //dd/MM/yyyy
            {

                var sTemp = sValue.Split('/');
                if (sTemp.Length == 3)
                {
                    return $"{sTemp[2]}-{sTemp[1]}-{sTemp[0]}";
                }

            }
            return sValue;
        }


        public static ConfirmDto CreateConfirmDto(string label, string value, bool isTitle = false)
        {
            var data = new ConfirmDto
            {
                Label = label,
                Value = value,
                IsTitle = isTitle
            };
            return data;
        }

        public static CardInfoDto ReadMyKadInfo(string stringJsonFormat)
        {
            var cardInfo = new CardInfoDto();
            try
            {
                Log.WriteLogFile("GeneralBll", "ReadMyKadInfo", "stringJsonFormat : " + stringJsonFormat, Enums.LogType.Info);

                cardInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<CardInfoDto>(stringJsonFormat);
                cardInfo.IsSuccessRead = true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("ReadMyKadInfo Error : " + ex.Message);
                cardInfo.IsSuccessRead = false;
                cardInfo.Message = "Error Read MyKad, Error : " + ex.Message;
            }

            return cardInfo;
        }

        public static CardInfoDto2 ReadMyKadInfo2(string stringJsonFormat)
        {
            var cardInfo = new CardInfoDto2();
            try
            {
                Log.WriteLogFile("GeneralBll", "ReadMyKadInfo2", "stringJsonFormat : " + stringJsonFormat, Enums.LogType.Info);

                cardInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<CardInfoDto2>(stringJsonFormat);
                cardInfo.IsSuccessRead = true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("ReadMyKadInfo2 Error : " + ex.Message);
                cardInfo.IsSuccessRead = false;
                cardInfo.Message = "Error Read MyKad2, Error : " + ex.Message;
            }

            return cardInfo;
        }

        public static List<string> SeparateText(string sValueText, int lenList, int MaxLength)
        {
            List<string> listString = new List<string>();
            //int MaxLengthPaper = 80;
            int nCurrPos = 0;
            int nPos = 0;

            for (int i = 0; i < lenList; i++)
            {
                var result = "";
                if (i == 0)
                {
                    nPos = FormatOffDesc(ref result, sValueText, MaxLength);

                }
                else
                {
                    nPos = FormatOffDesc(ref result, sValueText.Substring(nCurrPos, sValueText.Length - nCurrPos), MaxLength);
                }

                listString.Add(result);

                if (nPos <= 0)
                    break;
                nCurrPos = nCurrPos + nPos + 1;
                if (nCurrPos > sValueText.Length)
                    break;
            }

            for (int j = listString.Count; j < lenList; j++)
            {
                listString.Add("");
            }

            return listString;
        }

        public static List<string> SeparateText(string sValueText, int MaxLength)
        {
            var result = new List<string>();
            if (sValueText.Length < MaxLength)
                MaxLength = sValueText.Length;

            var totalData = (int)Math.Ceiling(Convert.ToDecimal(sValueText.Length) / Convert.ToDecimal(MaxLength));
            for (int i = 0; i < totalData; i++)
            {
                result.Add(i == totalData - 1 ? sValueText.Substring(i * MaxLength) : sValueText.Substring(i * MaxLength, MaxLength));
            }
            return result;
        }

        private static int FormatOffDesc(ref string sdest, string sSource, int maxLen)
        {
            int nPos, nLen, nPrePos;

            nLen = sSource.Length;
            if (nLen > maxLen)
                nLen = maxLen;
            nPos = FindString(sSource, " ", nLen);
            if (nPos > 0)
                sdest = sSource.Substring(0, nPos);
            else
                sdest = sSource;

            nLen = sdest.Trim().Length;

            if (nLen > maxLen)
            {
                nPrePos = nLen;
                nLen = maxLen;
                //nPos = GetNextChar(sSource, nLen, nPrePos);
                nPos = GetPrevChar(sSource, nPrePos);
                if (nPos <= 0)
                    nPos = nLen;
                sdest = sSource.Substring(0, nPos);
            }

            return nPos;
        }

        private static int FindString(string source, string target, int startPos)
        {
            int srcLen = source.Length;

            int iResult = -1;
            if (startPos < srcLen)
            {
                for (int i = startPos; i < srcLen; i++)
                {
                    if (source.Substring(i, 1) == target)
                    {
                        iResult = i;
                        break;
                    }
                }
            }

            return iResult;
        }

        private static int FindReverseString(string source, string target, int startPos)
        {
            int srcLen = source.Length;

            int iResult = -1;
            if (startPos <= srcLen)
            {
                //for (int i = startPos; i < 0; i--)
                for (int i = startPos - 1; i > 0; i--)
                {
                    if (source.Substring(i, 1) == target)
                    {
                        iResult = i;
                        break;
                    }
                }
            }

            return iResult;
        }

        private static int GetPrevChar(string sSource, int nPrePos)
        {
            int nPos = 0;

            nPos = FindReverseString(sSource, " ", nPrePos);
            if (nPos <= 0 || nPos > nPrePos)
            {
                nPos = FindReverseString(sSource, ",", nPrePos);
                if (nPos <= 0 || nPos > nPrePos)
                {
                    nPos = FindReverseString(sSource, ".", nPrePos);
                    if (nPos <= 0 || nPos > nPrePos)
                    {
                        nPos = FindReverseString(sSource, "\\", nPrePos);
                        if (nPos <= 0 || nPos > nPrePos)
                        {
                            nPos = FindReverseString(sSource, "|", nPrePos);
                            if (nPos <= 0 || nPos > nPrePos)
                            {
                                nPos = FindReverseString(sSource, "-", nPrePos);
                                if (nPos <= 0 || nPos > nPrePos)
                                {
                                    nPos = FindReverseString(sSource, "/", nPrePos);
                                    if (nPos <= 0 || nPos > nPrePos)
                                        nPos = nPrePos;
                                }
                            }
                        }
                    }
                }
            }

            return nPos;
        }

        public static string GettOneAlamat(string alamat1, string alamat2, string alamat3)
        {
            if (!string.IsNullOrEmpty(alamat2)) alamat1 += " " + alamat2;
            if (!string.IsNullOrEmpty(alamat3)) alamat1 += " " + alamat3;

            return alamat1;
        }

        public static List<string> GetPhotoNameByRujukan(string noRujukan)
        {
            var dirImagePath = GetInternalImagePath();
            DirectoryInfo di = new DirectoryInfo(dirImagePath);
            FileInfo[] rgFiles = di.GetFiles(noRujukan + "*.*");

            var result = new List<string>();
            foreach (var fileInfo in rgFiles)
            {
                result.Add(fileInfo.Name);
            }

            return result;
        }

        public static List<string> GetReceiptPhotoNameByRujukan(string noRujukan)
        {
            var dirImagePath = GetInternalImagePath();
            DirectoryInfo di = new DirectoryInfo(dirImagePath);
            FileInfo[] rgFiles = di.GetFiles(Constants.Receipt + noRujukan + "*.*");

            var result = new List<string>();
            foreach (var fileInfo in rgFiles)
            {
                result.Add(fileInfo.Name);
            }

            return result;
        }

        public static int GetPositionSelected(List<string> listData, string data)
        {
            int index = 0;
            foreach (var sValue in listData)
            {
                if (sValue == data) return index;
                index++;
            }

            return 0;
        }
        public static int GetPositionSelected(Dictionary<string, string> listData, string sValue)
        {
            int index = 0;
            foreach (var data in listData)
            {
                if (data.Key == sValue) return index;
                index++;
            }

            return 0;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText.Trim());
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static DateTime ConvertDatabaseFormatStringToDateTime(string sValue)
        {
            try
            {
                return DateTime.ParseExact(sValue, Constants.DatabaseDateFormat, null);
            }
            catch (Exception e)
            {
                return GetLocalDateTime();
            }
        }

        public static string GetLocalDateTimeForDatabase()
        {
            return GetLocalDateTime().ToString(Constants.DatabaseDateFormat);
        }

        public static string GetUnixDateTimeQuery(string sValue)
        {
            return "unix_timestamp('" + sValue + "')";
        }

        public static List<string> GetListPatPhotoNameByRujukan(string noRujukan)
        {
            var dirImagePath = GetInternalImagePath();
            DirectoryInfo di = new DirectoryInfo(dirImagePath);
            FileInfo[] rgFiles = di.GetFiles(noRujukan + "*.*", SearchOption.AllDirectories);

            return rgFiles.Select(file => file.FullName).ToList();
        }

        public static List<string> GetListPathPhotoReceiptNameByRujukan(string noRujukan)
        {
            var dirImagePath = GetInternalImagePath();
            DirectoryInfo di = new DirectoryInfo(dirImagePath);
            FileInfo[] rgFiles = di.GetFiles(Constants.Receipt + noRujukan + "*.*", SearchOption.AllDirectories);

            return rgFiles.Select(file => file.FullName).ToList();
        }

        public static string GetBase64FromImagePath(string path)
        {
            var result = string.Empty;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var imageData = new byte[fs.Length];
                fs.Read(imageData, 0, (int)fs.Length);
                result = Convert.ToBase64String(imageData);
            }
            return result;
        }

        public static int StringDatetimeToInt(string datetime)
        {
            try
            {
                var tarikhAkhir = DateTime.ParseExact(datetime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                return (Int32)(tarikhAkhir.Subtract(new DateTime(1970, 1, 2))).TotalSeconds;
                //var tarikhAkhir = DateTime.ParseExact(datetime, Constants.DatabaseDateFormat, System.Globalization.CultureInfo.InvariantCulture);
                //return (Int32)(DateTime.UtcNow.Subtract(tarikhAkhir)).TotalMilliseconds;
            }
            catch (Exception e)
            {
                return (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 2))).TotalSeconds;
            }
        }

        public static string GetAlphabetByNumber(int number)
        {
            if (number > 26) return number.ToString();
            var strAlphabet = "abcdefghijklmnopqrstuvwxyz";
            return strAlphabet.Substring(number - 1, 1);
        }

        public static string GetStringOrEmpty(string sValue)
        {
            return !string.IsNullOrEmpty(sValue) ? sValue : "";
        }

        public static List<string> SeparateTextTitle(string sValueText, int maxLength)
        {
            List<string> listString = new List<string>();

            var listData = sValueText.Split(' ').ToList();

            string sTemp = "";

            foreach (var s in listData)
            {
                int lenghtTemp = sTemp.Length;
                int lengthS = s.Length;

                if (lenghtTemp + lengthS + 1 > maxLength)
                {
                    listString.Add(sTemp.Trim());
                    sTemp = s + " ";
                }
                else
                {
                    sTemp += s + " ";
                }
            }


            return listString;
        }

        //public static string SetString(string sValue)
        //{
        //    return sValue.Replace(")(", ") (");

        //    // tajuk = "PERATURAN-PERATURAN PERIHAL DAGANGAN (PENGKOMPAUNAN KESALAHAN-KESALAHAN )(PINDAAN) 2010";
        //    var index = sValue.IndexOf(")", StringComparison.Ordinal);
        //    if (index == sValue.Length)
        //    {
        //        return sValue;
        //    }
        //    if (sValue.Substring(index, 1) == ")")
        //    {
        //        string result = sValue.Substring(0, index);
        //        result += " " + sValue.Substring(index, sValue.Length - index);
        //        return result;
        //    }

        //    return sValue;
        //}

        public static string DecimalToWord(decimal decValue)
        {
            string[] satuan = new string[10] { "nol", "satu", "dua", "tiga", "empat", "lima", "enam", "tujuh", "delapan", "sembilan" };
            string[] belasan = new string[10] { "sepuluh", "sebelas", "dua belas", "tiga belas", "empat belas", "lima belas", "enam belas", "tujuh belas", "delapan belas", "sembilan belas" };
            string[] puluhan = new string[10] { "", "", "dua puluh", "tiga puluh", "empat puluh", "lima puluh", "enam puluh", "tujuh puluh", "delapan puluh", "sembilan puluh" };
            string[] ribuan = new string[5] { "", "ribu", "juta", "milyar", "triliyun" };

            string strHasil = "";
            //Decimal frac = decValue - Decimal.Truncate(decValue);

            //if (Decimal.Compare(frac, 0.0m) != 0)
            //    strHasil = DecimalToWord(decimal.Round(frac * 100)) + " sen";
            //else
            //    strHasil = "rupiah";
            int xDigit = 0;
            int xPosisi = 0;

            string strTemp = Decimal.Truncate(decValue).ToString();
            for (int i = strTemp.Length; i > 0; i--)
            {
                string tmpx = "";
                xDigit = Convert.ToInt32(strTemp.Substring(i - 1, 1));
                xPosisi = (strTemp.Length - i) + 1;
                switch (xPosisi % 3)
                {
                    case 1:
                        bool allNull = false;
                        if (i == 1)
                            tmpx = satuan[xDigit] + " ";
                        else if (strTemp.Substring(i - 2, 1) == "1")
                            tmpx = belasan[xDigit] + " ";
                        else if (xDigit > 0)
                            tmpx = satuan[xDigit] + " ";
                        else
                        {
                            allNull = true;
                            if (i > 1)
                                if (strTemp.Substring(i - 2, 1) != "0")
                                    allNull = false;
                            if (i > 2)
                                if (strTemp.Substring(i - 3, 1) != "0")
                                    allNull = false;
                            tmpx = "";
                        }

                        if ((!allNull) && (xPosisi > 1))
                            if ((strTemp.Length == 4) && (strTemp.Substring(0, 1) == "1"))
                                tmpx = "satu " + ribuan[(int)decimal.Round(xPosisi / 3m)] + " ";
                            else
                                tmpx = tmpx + ribuan[(int)decimal.Round(xPosisi / 3)] + " ";
                        strHasil = tmpx + strHasil;
                        break;
                    case 2:
                        if (xDigit > 0)
                            strHasil = puluhan[xDigit] + " " + strHasil;
                        break;
                    case 0:
                        if (xDigit > 0)
                            if (xDigit == 1)
                                strHasil = "satu ratus " + strHasil;
                            else
                                strHasil = satuan[xDigit] + " ratus " + strHasil;
                        break;
                }
            }
            strHasil = strHasil.Trim().ToLower();
            if (strHasil.Length > 0)
            {
                strHasil = strHasil.Substring(0, 1).ToUpper() +
                  strHasil.Substring(1, strHasil.Length - 1);
            }
            return strHasil.ToUpper();
        }
        public static string CreateFolderReturnPath(string folderName)
        {
            var backupFolder = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + folderName);
            if (!backupFolder.Exists())
                backupFolder.Mkdirs();

            return backupFolder.AbsolutePath;
        }

        public static string GetFolderPath(string folderName)
        {
            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + folderName;
        }

        public static void InitConfig()
        {
            var configPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                                   Constants.ConfigPath + Constants.ConfigName;
            if (!File.Exists(configPath))
            {
                using (BinaryReader br = new BinaryReader(Application.Context.Assets.Open(Constants.ConfigName)))
                {
                    using (BinaryWriter bw = new BinaryWriter(new FileStream(configPath, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int len = 0;
                        while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, len);
                        }
                    }
                }
            }

            SetConfigPreferences();
        }

        private static void SetConfigPreferences()
        {
            var configDto = ConfigAccess.GetConfigAccess();
            SharedPreferences.RemoveKey(SharedPreferencesKeys.WebServiceUrl);
            SharedPreferences.SaveString(SharedPreferencesKeys.WebServiceUrl, configDto.WebServiceUrl);
            SharedPreferences.SaveString(SharedPreferencesKeys.IntervalBackgroundServiceInSecond, configDto.IntervalBackgroundServiceInSecond.ToString());
        }

        public static string GetWebServicUrl()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.WebServiceUrl, Constants.DefaultWebServiceUrl);
        }

        public static int GetIntervalBackgroundService()
        {
            return GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.IntervalBackgroundServiceInSecond, Constants.DefaultIntervalBackgroundServiceInSecond.ToString()));
        }

        public static bool IsBackgroundServiceRunning()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.IsBackgroundServiceRunning, "1") == "1";
        }

        public static string ProcessRcvData(byte[] buffer)
        {
            string s1 = "";
            char c;
            int i = 0, asciival = 0;

            if (buffer.Length <= 1)
                return s1;

            for (i = 0; i < buffer.Length; i++)
            {
                c = (char)buffer[i];
                asciival = (int)c;
                if ((asciival >= 48 && asciival <= 57) || (asciival >= 65 && asciival <= 90) || (asciival >= 97 && asciival <= 122) || asciival == 32)
                    s1 = s1 + c.ToString();
            }

            //            string s = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            return s1;
        }

        public static bool IsSkipControl()
        {
#if DEBUG
            return true;
#endif
            var data = DataAccessQuery<TbSkipControl>.GetAll();
            if (data.Success && data.Datas.Count > 0)
            {
                var result = data.Datas[0].IsSkip;
                if (result == Constants.SkipIzin.Yes) return true;
            }
            return false;
        }

        public static bool IsUserLogin()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.IsUserLogin) == "1";
        }


        public static List<List<T>> Split<T>(this List<T> source)
        {
            return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / Constants.MaxLengthSentData)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
        }

        public static List<string> GetAllImages(string path)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(m => m.ToLower().EndsWith("jpg") ||
                            m.ToLower().EndsWith("jpeg") ||
                            m.ToLower().EndsWith("png")).ToList();
        }

    }
}