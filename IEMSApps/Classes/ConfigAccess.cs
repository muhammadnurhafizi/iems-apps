using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.Utils;

namespace IEMSApps.Classes
{
    public static class ConfigAccess
    {
        public static ConfigAppDto GetConfigAccess()
        {
            var configDto = new ConfigAppDto();
            string strFullFileName = "";

            try
            {
                configDto.IntervalInSecond = Constants.DefaultIntervalInSecond;
                configDto.DistanceInMeter = Constants.DefaultIntervalInSecond;
                configDto.WebServiceUrl = Constants.DefaultWebServiceUrl;
                configDto.IntervalBackgroundServiceInSecond = Constants.DefaultIntervalBackgroundServiceInSecond;

                strFullFileName = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
                                   Constants.ConfigPath + Constants.ConfigName;

                if (File.Exists(strFullFileName))
                {
                    DataSet dsData = new DataSet();
                    dsData.ReadXml(strFullFileName);

                    string settingName = "";
                    foreach (DataRow myRow in dsData.Tables[0].Rows)
                    {
                        foreach (DataColumn myCol in dsData.Tables[0].Columns)
                        {
                            settingName = myCol.ColumnName.ToUpper();
                            if (string.Compare(settingName, "IntervalInSecond".ToUpper(), StringComparison.Ordinal) == 0)
                                configDto.IntervalInSecond = GeneralBll.ConvertStringToInt(myRow[myCol].ToString());
                            else if (string.Compare(settingName, "DistanceInMeter".ToUpper(), StringComparison.Ordinal) == 0)
                                configDto.DistanceInMeter = GeneralBll.ConvertStringToInt(myRow[myCol].ToString());
                            else if (string.Compare(settingName, "WebServiceUrl".ToUpper(), StringComparison.Ordinal) == 0)
                                configDto.WebServiceUrl = myRow[myCol].ToString();
                            else if (string.Compare(settingName, "IntervalBackgroundServiceInSecond".ToUpper(), StringComparison.Ordinal) == 0)
                                configDto.IntervalBackgroundServiceInSecond = GeneralBll.ConvertStringToInt(myRow[myCol].ToString());
                        }
                    }

                    if (configDto.IntervalInSecond == 0)
                    {
                        configDto.IntervalInSecond = Constants.DefaultIntervalInSecond;
                    }

                    if (configDto.DistanceInMeter == 0)
                    {
                        configDto.DistanceInMeter = Constants.DefaultIntervalInSecond;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("File Name " + strFullFileName, ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("GetConfigApp", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);
            }

            return configDto;
        }
    }
}