using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Responses;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exception = Java.Lang.Exception;

namespace IEMSApps.BLL
{
    public static class HandheldBll
    {
        //public static bool CreateOrUpdateHandheldData(Enums.PrefixType prefixType, string noRujukan)
        //{

        //    var userHandheld = GeneralBll.GetUserHandheld();

        //    var handheld = DataAccessQuery<TbHandheld>.Get(c => c.IdHh == userHandheld);

        //    if (!handheld.Success)
        //    {
        //        throw new Java.Lang.Exception("error get data handheld");
        //    }
        //    TbHandheld tbHanheld = handheld.Datas;
        //    bool isNew = false;
        //    if (tbHanheld == null)
        //    {
        //        isNew = true;
        //        tbHanheld = new TbHandheld
        //        {
        //            IdHh = userHandheld,
        //            KodCawangan = GeneralBll.GetUserCawangan(),
        //            PgnDaftar = 0,
        //            TrkhDaftar = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat),
        //            Year = GeneralBll.GetLocalDateTime().Year
        //        };

        //    }

        //    tbHanheld.Status = Constants.Status.Aktif;
        //    var countImage = GeneralBll.GetCountPhotoByRujukan(noRujukan);

        //    if (prefixType == Enums.PrefixType.KPP)
        //    {
        //        tbHanheld.NotUrutan_Kpp += 1;
        //        tbHanheld.Jumlah_Kpp += 1;
        //        tbHanheld.Jumlah_Gambar_Kpp += countImage;
        //    }
        //    else if (prefixType == Enums.PrefixType.KOTS)
        //    {
        //        tbHanheld.NotUrutan_Kots += 1;
        //        tbHanheld.Jumlah_Kots += 1;
        //        tbHanheld.Jumlah_Gambar_Kots += countImage;
        //    }
        //    else if (prefixType == Enums.PrefixType.SiasatLanjutan)
        //    {
        //        tbHanheld.NotUrutan_DataKes += 1;
        //        tbHanheld.Jumlah_DataKes += 1;
        //        tbHanheld.Jumlah_Gambar_DataKes += countImage;
        //    }

        //    var localTime = GeneralBll.GetLocalDateTime();
        //    tbHanheld.TrkhAkhir = localTime.ToString(Constants.DatabaseDateFormat);
        //    tbHanheld.PgnAkhir = GeneralBll.GetUserStaffId();
        //    tbHanheld.AppVer = Constants.AppVersion;

        //    handheld = isNew
        //    ? DataAccessQuery<TbHandheld>.Insert(tbHanheld)
        //    : DataAccessQuery<TbHandheld>.Update(tbHanheld);

        //    if (!handheld.Success)
        //    {
        //        throw new Exception("error insert/update handheld");
        //    }
        //    return true;
        //}

        public static bool UpdateRollBackNumberHandheldData(Enums.PrefixType prefixType, string noRujukan)
        {

            var userHandheld = GeneralBll.GetUserHandheld();

            var handheld = DataAccessQuery<TbHandheld>.Get(c => c.IdHh == userHandheld);

            if (!handheld.Success)
            {
                return false;
                throw new Java.Lang.Exception("error get data handheld");
            }
            TbHandheld tbHanheld = handheld.Datas;

            if (tbHanheld == null)
            {
                Log.WriteLogFile("HandheldBll", "UpdateRollBackNumberHandheldData", "TbHandheld not found : " + userHandheld, Enums.LogType.Error);
                return false;
            }

            var countImage = GeneralBll.GetCountPhotoByRujukan(noRujukan);

            if (prefixType == Enums.PrefixType.KPP)
            {
                tbHanheld.NotUrutan_Kpp -= 1;
                tbHanheld.Jumlah_Kpp -= 1;
                tbHanheld.Jumlah_Gambar_Kpp -= countImage;
            }
            else if (prefixType == Enums.PrefixType.KOTS)
            {
                tbHanheld.NotUrutan_Kots -= 1;
                tbHanheld.Jumlah_Kots -= 1;
                tbHanheld.Jumlah_Gambar_Kots -= countImage;
            }
            else if (prefixType == Enums.PrefixType.SiasatLanjutan)
            {
                tbHanheld.NotUrutan_DataKes -= 1;
                tbHanheld.Jumlah_DataKes -= 1;
                tbHanheld.Jumlah_Gambar_DataKes -= countImage;
            }

            var localTime = GeneralBll.GetLocalDateTime();
            tbHanheld.TrkhAkhir = localTime.ToString(Constants.DatabaseDateFormat);
            tbHanheld.PgnAkhir = GeneralBll.GetUserStaffId();

            var resultUpdate = DataAccessQuery<TbHandheld>.Update(tbHanheld);

            if (!resultUpdate.Success)
            {
                Log.WriteLogFile("HandheldBll", "UpdateRollBackNumberHandheldData", "Error Update TbHandheld ", Enums.LogType.Error);
                Log.WriteLogFile("HandheldBll", "UpdateRollBackNumberHandheldData", "Message : " + resultUpdate.Message, Enums.LogType.Error);
                return false;
                throw new Exception("error insert/update handheld");
            }
            return true;
        }

        public static Result<TbHandheld> GetHandheldData()
        {

            var userHandheld = GeneralBll.GetUserHandheld();

            return DataAccessQuery<TbHandheld>.Get(c => c.IdHh == userHandheld);
        }

        public static string GenerateNoRujukanForSummary(TbHandheld tbHanheld, Enums.PrefixType prefixType)
        {
            int runningNumber = 0;
            string prefixRujukan = "";
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


            //string prefixRujukan = prefixType == Enums.PrefixType.SiasatLanjutan ? "DTK" : prefixType.ToString();

            var localYear = GeneralBll.GetLocalDateTime().ToString("yy");

            if (!string.IsNullOrEmpty(tbHanheld.TrkhAkhir) && tbHanheld.TrkhAkhir.Length > 4)
            {
                localYear = tbHanheld.TrkhAkhir.Substring(2, 2);
            }

            var noRujukan =
                $"{prefixRujukan}{tbHanheld.IdHh}{localYear}{runningNumber.ToString(Constants.FormatRunningNumber)}";


            return noRujukan;
        }

        public static Response<List<TbHandheld>> GetRecordHandheldAsync(string IdPeranti)
        {
            var result = new Response<List<TbHandheld>>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<List<TbHandheld>> { Success = false, Mesage = "Tiada Sambungan Internet" };
            }
            else
            {
                for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
                {
                    result = Task.Run(async () => await HttpClientService.GetRecordHandheldAsync(IdPeranti)).Result;

                    if (result.Success)
                    {
                        return result;
                    }

                    Thread.Sleep(Constants.SleepRetryActiveParking);
                }
            }

            return result;
        }

        public static void CreateFromApi(TbHandheld tbHandheld)
        {
            DataAccessQuery<TbHandheld>.DeleteAll();

            DataAccessQuery<TbHandheld>.Insert(tbHandheld);

            //var existingData = DataAccessQuery<TbHandheld>.Get(c => c.IdHh == tbHandheld.IdHh);
            //if (existingData.Success && existingData.Datas == null)
            //{
            //    DataAccessQuery<TbHandheld>.Insert(tbHandheld);
            //}
        }

        public static Response<DownloadDataResponse> PrepareDownloadDatas(string IdPeranti)
        {
            var userHandheld = GeneralBll.GetUserHandheld();
            var tbHandheld = DataAccessQuery<TbHandheld>.Get(m => m.IdHh == userHandheld);

            var result = new Response<DownloadDataResponse>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<DownloadDataResponse> { Success = false, Mesage = "Tiada Sambungan Internet" };
            }
            else
            {
                for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
                {
                    result = Task.Run(async () => await HttpClientService.PrepareDownloadDatas(IdPeranti, tbHandheld.Datas.TrkhHhCheckin)).Result;

                    if (result.Success)
                    {
                        return result;
                    }

                    Thread.Sleep(Constants.SleepRetryActiveParking);
                }
            }

            return result;
        }

        //public static Response<DownloadDataFreshResponse> PrepareDownloadFreshDatas(string IdPeranti)
        //{
        //    var userHandheld = GeneralBll.GetUserHandheld();
        //    var tbHandheld = DataAccessQuery<TbHandheld>.Get(m => m.IdHh == userHandheld);

        //    var result = new Response<DownloadDataFreshResponse>();
        //    if (!GeneralAndroidClass.IsOnline())
        //    {
        //        return new Response<DownloadDataFreshResponse> { Success = false, Mesage = "Tiada Sambungan Internet" };
        //    }
        //    else
        //    {
        //        for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
        //        {
        //            result = Task.Run(async () => await HttpClientService.PrepareDownloadFreshDatas(IdPeranti)).Result;

        //            if (result.Success)
        //            {
        //                return result;
        //            }

        //            Thread.Sleep(Constants.SleepRetryActiveParking);
        //        }
        //    }

        //    return result;
        //}

        public static void InitUpdateStatusPasukan(string idHh)
        {
            var sql = $"UPDATE tbpasukan_hh SET STATUS = '1' where idHH = '{idHh}'";
            DataAccessQuery<TbPasukanHh>.ExecuteSelectSql(sql);
        }

        public static Response<string> UpdateRecordHandheldAsync(string IdPeranti, string userId, Android.Content.Context context)
        {
            var result = new Response<string>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<string>() { Success = false, Mesage = "Tiada Sambungan Internet" };
            }
            else
            {
                var query = $"update tbhandheld set status = '3', pgnakhir = '319', trkhakhir = UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}') where idhh = '{IdPeranti}'";
                for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
                {
                    result = Task.Run(async () => await HttpClientService.ExecuteQuery(query, context)).Result;

                    if (result.Success)
                    {
                        return result;
                    }

                    Thread.Sleep(Constants.SleepRetryActiveParking);
                }

            }

            return result;
        }

        public static Response<string> UpdatePasukanAsync(string IdPeranti, Android.Content.Context context)
        {
#if DEBUG
            return new Response<string>() { Success = false, Mesage = "Tiada Sambungan Internet" };
#endif
            var result = new Response<string>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<string>() { Success = false, Mesage = "Tiada Sambungan Internet" };
            }
            else
            {
                var query = $"update tbpasukan_hh set status = '2'  where idhh = '{IdPeranti}'";
                for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
                {
                    result = Task.Run(async () => await HttpClientService.ExecuteQuery(query, context)).Result;

                    if (result.Success)
                    {
                        return result;
                    }

                    Thread.Sleep(Constants.SleepRetryActiveParking);
                }

            }

            return result;
        }

        public static Response<string> UpdateHandheldSetTrkhHhCheckin(string IdPeranti, Android.Content.Context context)
        {

            //update tbhandheld 
            var sql = $"UPDATE tbhandheld SET TrkhHhCheckin = '{GeneralBll.GetLocalDateTimeForDatabase()}', TrkhUpdateDate = '{GeneralBll.GetLocalDateTimeForDatabase()}' WHERE IdHh = '{IdPeranti}' ";
            DataAccessQuery<TbHandheld>.ExecuteSelectSql(sql);

            var result = new Response<string>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<string>() { Success = false, Mesage = "Tiada Sambungan Internet" };
            }
            else
            {
                var query = $"update tbhandheld SET trkhhhcheckin = '{GeneralBll.GetLocalDateTimeForDatabase()}' where idhh = '{IdPeranti}'";
                for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
                {
                    result = Task.Run(async () => await HttpClientService.ExecuteQuery(query, context)).Result;

                    if (result.Success)
                    {
                        return result;
                    }

                    Thread.Sleep(Constants.SleepRetryActiveParking);
                }

            }

            return result;
        }

        public static string GetHandheldIdHh()
        {

            var data = DataAccessQuery<TbHandheld>.Get(c => c.Status == Constants.Status.Aktif);
            if (data.Success && data.Datas != null)
            {
                return data.Datas.IdHh;
            }
            return "";
        }

        public static string GetHandheldCawangan()
        {

            var data = DataAccessQuery<TbHandheld>.Get(c => c.Status == Constants.Status.Aktif);
            if (data.Success && data.Datas != null)
            {
                return data.Datas.KodCawangan;
            }
            return "";
        }

        public static TbHandheld GetActiveHandheld()
        {

            var data = DataAccessQuery<TbHandheld>.Get(c => c.Status == Constants.Status.Aktif);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }
            return null;
        }

        //public static void CheckRunningNumber()
        //{
        //    var userHandheld = SharedPreferences.GetString(SharedPreferencesKeys.UserHandheld);

        //    var handheld = DataAccessQuery<TbHandheld>.Get(c => c.IdHh == userHandheld);
        //    if (handheld.Success & handheld.Datas != null)
        //    {
        //        var currentYear = GeneralBll.GetLocalDateTime().Year;
        //        if (currentYear != handheld.Datas.Year)
        //        {
        //            //update running number
        //            handheld.Datas.Year = currentYear;
        //            handheld.Datas.NotUrutan_Kpp = 0;
        //            handheld.Datas.NotUrutan_Kots = 0;
        //            handheld.Datas.NotUrutan_DataKes = 0;

        //            DataAccessQuery<TbHandheld>.Update(handheld.Datas);
        //        }
        //    }
        //}
        public static TbHandheld CheckRunningNumber(TbHandheld handheld)
        {

            var currentYear = GeneralBll.GetLocalDateTime().Year;

            if (currentYear != handheld.Year)
            {
                //update running number
                handheld.Year = currentYear;
                handheld.NotUrutan_Kpp = 0;
                handheld.NotUrutan_Kots = 0;
                handheld.NotUrutan_DataKes = 0;

                DataAccessQuery<TbHandheld>.Update(handheld);
            }
            return handheld;
        }

        public static void CheckRunningZeroYear()
        {
            var handheld = DataAccessQuery<TbHandheld>.Get(c => c.Year == 0);

            if (!handheld.Success)
            {
                return;
            }
            TbHandheld tbHanheld = handheld.Datas;

            if (tbHanheld != null)
            {
                var currentYear = GeneralBll.GetLocalDateTime().Year;

                tbHanheld.Year = currentYear;

                DataAccessQuery<TbHandheld>.Update(tbHanheld);
            }


        }

        public static void UpdateHandheldFirst()
        {
            var handheld = DataAccessQuery<TbHandheld>.GetAll();
            if (handheld.Success)
            {
                var data = handheld.Datas.FirstOrDefault();
                if (data?.Year == 0 && data.NotUrutan_Kpp > 0)
                {
                    data.Year = GeneralBll.GetLocalDateTime().Year;

                    DataAccessQuery<TbHandheld>.Update(data);
                }
            }
        }



        public static bool CreateOrUpdateHandheldDataTrx(Enums.PrefixType prefixType, string noRujukan, DataAccessQueryTrx insAccess)
        {
            try
            {
                var userHandheld = GeneralBll.GetUserHandheld();

                var handheld = DataAccessQuery<TbHandheld>.Get(c => c.IdHh == userHandheld);

                if (!handheld.Success)
                {
                    return false;
                }
                TbHandheld tbHanheld = handheld.Datas;
                bool isNew = false;
                if (tbHanheld == null)
                {
                    isNew = true;
                    tbHanheld = new TbHandheld
                    {
                        IdHh = userHandheld,
                        KodCawangan = GeneralBll.GetUserCawangan(),
                        PgnDaftar = 0,
                        TrkhDaftar = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat),
                        Year = GeneralBll.GetLocalDateTime().Year
                    };

                }

                tbHanheld.Status = Constants.Status.Aktif;
                var countImage = GeneralBll.GetCountPhotoByRujukan(noRujukan);

                if (prefixType == Enums.PrefixType.KPP)
                {
                    tbHanheld.NotUrutan_Kpp += 1;
                    tbHanheld.Jumlah_Kpp += 1;
                    tbHanheld.Jumlah_Gambar_Kpp += countImage;
                }
                else if (prefixType == Enums.PrefixType.KOTS)
                {
                    tbHanheld.NotUrutan_Kots += 1;
                    tbHanheld.Jumlah_Kots += 1;
                    tbHanheld.Jumlah_Gambar_Kots += countImage;
                }
                else if (prefixType == Enums.PrefixType.SiasatLanjutan)
                {
                    tbHanheld.NotUrutan_DataKes += 1;
                    tbHanheld.Jumlah_DataKes += 1;
                    tbHanheld.Jumlah_Gambar_DataKes += countImage;
                }
                else if (prefixType == Enums.PrefixType.KOTSAndSiasatLanjutan)
                {
                    tbHanheld.NotUrutan_Kots += 1;
                    tbHanheld.Jumlah_Kots += 1;
                    tbHanheld.Jumlah_Gambar_Kots += countImage;

                    tbHanheld.NotUrutan_DataKes += 1;
                    tbHanheld.Jumlah_DataKes += 1;
                }

                var localTime = GeneralBll.GetLocalDateTime();
                tbHanheld.TrkhAkhir = localTime.ToString(Constants.DatabaseDateFormat);
                tbHanheld.PgnAkhir = GeneralBll.GetUserStaffId();
                tbHanheld.AppVer = Constants.AppVersion;

                return isNew ? insAccess.InsertTrx(tbHanheld) : insAccess.UpdateTrx(tbHanheld);


            }
            catch (Exception ex)
            {
                Log.WriteLogFile("HandheldBll", "CreateOrUpdateHandheldDataTrx", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("HandheldBll", "StackTrace", ex.StackTrace, Enums.LogType.Error);
                return false;
            }

        }

        //private static bool InsertTbHandheldTrx(TbHandheld tbHandheld, DataAccessQueryTrx insAccess)
        //{
        //    var query = "INSERT INTO tbhandheld(IdHh,KodCawangan,PgnDaftar,TrkhDaftar,Status," +
        //                "NotUrutan_Kpp,Jumlah_Kpp,Jumlah_Gambar_Kpp,NotUrutan_Kots,Jumlah_Kots,Jumlah_Gambar_Kots," +
        //                "NotUrutan_DataKes,Jumlah_DataKes,Jumlah_Gambar_DataKes,TrkhAkhir,PgnAkhir,AppVer)";
        //    query += "Values(";
        //    query +=
        //        $"'{tbHandheld.IdHh}', '{tbHandheld.KodCawangan}', {tbHandheld.PgnDaftar},'{tbHandheld.TrkhDaftar}', '{tbHandheld.Status}',";
        //    query += $"{tbHandheld.NotUrutan_Kpp}, {tbHandheld.Jumlah_Kpp},{tbHandheld.Jumlah_Gambar_Kpp},";
        //    query += $"{tbHandheld.NotUrutan_Kots}, {tbHandheld.Jumlah_Kots},{tbHandheld.Jumlah_Gambar_Kots},";
        //    query += $"{tbHandheld.NotUrutan_DataKes}, {tbHandheld.Jumlah_DataKes},{tbHandheld.Jumlah_Gambar_DataKes},";
        //    query += $"'{tbHandheld.TrkhAkhir}', {tbHandheld.PgnAkhir}, '{tbHandheld.AppVer}')";

        //    return insAccess.ExecuteQueryTrx(query);

        //}

        //private static bool UpdateTbHandheldTrx(TbHandheld tbHandheld, DataAccessQueryTrx insAccess)
        //{

        //    var query = $"UPDATE tbhandheld set KodCawangan = '{tbHandheld.KodCawangan}'," +
        //            $"PgnDaftar = {tbHandheld.PgnDaftar}, TrkhDaftar = '{tbHandheld.TrkhDaftar}'," +
        //            $"Status = '{tbHandheld.Status}'," +
        //            $"NotUrutan_Kpp = {tbHandheld.NotUrutan_Kpp},Jumlah_Kpp = {tbHandheld.Jumlah_Kpp}, " +
        //            $"Jumlah_Gambar_Kpp = {tbHandheld.Jumlah_Gambar_Kpp}," +
        //            $"NotUrutan_Kots = {tbHandheld.NotUrutan_Kots},Jumlah_Kots = {tbHandheld.Jumlah_Kots}, " +
        //            $"Jumlah_Gambar_Kots = {tbHandheld.Jumlah_Gambar_Kots}," +
        //            $"NotUrutan_DataKes = {tbHandheld.NotUrutan_DataKes},Jumlah_DataKes = {tbHandheld.Jumlah_DataKes}, " +
        //            $"Jumlah_Gambar_DataKes = {tbHandheld.Jumlah_Gambar_DataKes}," +
        //            $"TrkhAkhir = '{tbHandheld.TrkhAkhir}', PgnAkhir = {tbHandheld.PgnAkhir}, AppVer = '{tbHandheld.AppVer}' " +
        //            $"Where IdHh = '{tbHandheld.IdHh}'";

        //    return insAccess.ExecuteQueryTrx(query);

        //}

        public static Response<List<TableSummaryResponse>> GetTableSummary(string IdPeranti)
        {
            var result = new Response<List<TableSummaryResponse>>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<List<TableSummaryResponse>> { Success = false, Mesage = "Tiada Sambungan Internet" };
            }
            else
            {
                for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
                {
                    result = Task.Run(async () => await HttpClientService.GetTableSummary(IdPeranti)).Result;

                    if (result.Success)
                    {
                        result.Result = CompareWithExistingData(result.Result);
                        return result;
                    }

                    Thread.Sleep(Constants.SleepRetryActiveParking);
                }
            }

            return result;
        }

        public static List<TableSummaryResponse> CompareWithExistingData(List<TableSummaryResponse> datas)
        {
            foreach (var item in datas)
            {
                item.IsSelected = item.IsModified == 1;
                if (item.TableName == "tbcawangan")
                    item.TotalApp = DataAccessQuery<TbCawangan>.Count();
                if (item.TableName == "tbpengguna")
                    item.TotalApp = DataAccessQuery<TbPengguna>.Count();
                if (item.TableName == "tbkategorikawasan")
                    item.TotalApp = DataAccessQuery<TbKategoriKawasan>.Count();
                if (item.TableName == "tbpremis")
                    item.TotalApp = DataAccessQuery<TbPremis>.Count();
                if (item.TableName == "tbtujuanlawatan")
                    item.TotalApp = DataAccessQuery<TbTujuanLawatan>.Count();
                if (item.TableName == "tbasastindakan")
                    item.TotalApp = DataAccessQuery<TbAsasTindakan>.Count();
                if (item.TableName == "tbkategoripremis")
                    item.TotalApp = DataAccessQuery<TbKategoriPremis>.Count();
                if (item.TableName == "tbjenisperniagaan")
                    item.TotalApp = DataAccessQuery<TbJenisPerniagaan>.Count();
                if (item.TableName == "tbnegeri")
                    item.TotalApp = DataAccessQuery<TbNegeri>.Count();
                if (item.TableName == "tbbandar")
                    item.TotalApp = DataAccessQuery<TbBandar>.Count();
                if (item.TableName == "tbakta")
                    item.TotalApp = DataAccessQuery<TbAkta>.Count();
                if (item.TableName == "tbkesalahan")
                    item.TotalApp = DataAccessQuery<TbKesalahan>.Count();
                if (item.TableName == "tbkategoriperniagaan")
                    item.TotalApp = DataAccessQuery<TbKategoriPerniagaan>.Count();
                if (item.TableName == "tbbarang_jenama")
                    item.TotalApp = DataAccessQuery<TbJenama>.Count();
            }

            return datas;
        }

        public static Response<PrepareDownloadDataSelectedResponse> PrepareDownloadDataSelected(string param)
        {
            var result = new Response<PrepareDownloadDataSelectedResponse>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<PrepareDownloadDataSelectedResponse> { Success = false, Mesage = "Tiada Sambungan Internet" };
            }
            else
            {
                for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
                {
                    result = Task.Run(async () => await HttpClientService.PrepareDownloadDataSelected(param)).Result;
                    if (result.Success)
                    {
                        return result;
                    }

                    Thread.Sleep(Constants.SleepRetryActiveParking);
                }
            }

            return result;
        }


        public static void UpdateHandheldSetTrkhHhMuatTurunData(string IdPeranti, Android.Content.Context context)
        {

            //update tbhandheld 
            var sql = $"UPDATE tbhandheld SET TrkhUpdateDate = '{GeneralBll.GetLocalDateTimeForDatabase()}' WHERE IdHh = '{IdPeranti}' ";
            DataAccessQuery<TbHandheld>.ExecuteSelectSql(sql);

            //var result = new Response<string>();
            //if (!GeneralAndroidClass.IsOnline())
            //{
            //    return new Response<string>() { Success = false, Mesage = "Tiada Sambungan Internet" };
            //}
            //else
            //{
            //    var query = $"update tbhandheld SET TrkhUpdateDate = '{GeneralBll.GetLocalDateTimeForDatabase()}' where idhh = '{IdPeranti}'";
            //    for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
            //    {
            //        result = Task.Run(async () => await HttpClientService.ExecuteQuery(query, context)).Result;
            //
            //        if (result.Success)
            //        {
            //            return result;
            //        }
            //
            //        Thread.Sleep(Constants.SleepRetryActiveParking);
            //    }
            //
            //}

            //return result;
        }
    }
}