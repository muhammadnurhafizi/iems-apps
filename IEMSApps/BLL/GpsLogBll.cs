using System;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public static class GpsLogBll
    {
        public static void Save(string longitude , string latitude)
        {
            try
            {
                var gpsData = new TbGpsLog
                {
                    KodCawangan = GeneralBll.GetUserCawangan(),
                    IdHh = GeneralBll.GetUserHandheld(),
                    IdStaf = GeneralBll.GetUserStaffId(),
                    TrkhLog = GeneralBll.GetLocalDateTimeForDatabase(),
                    Longitud = longitude,
                    Latitud = latitude
                };

                gpsData.PgnDaftar = gpsData.IdStaf;
                gpsData.TrkhDaftar = gpsData.TrkhLog;

                var result = DataAccessQuery<TbGpsLog>.Insert(gpsData);
                if (!result.Success)
                {
                    Log.WriteLogFile("GpsLogBll", "Save", result.Message, Enums.LogType.Error);
                }
            }
            catch (Exception e)
            {
                Log.WriteLogFile("GpsLogBll", "Save", e.Message, Enums.LogType.Error);
            }
           

        }
    }
}