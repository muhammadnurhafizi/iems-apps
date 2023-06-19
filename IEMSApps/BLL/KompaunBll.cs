using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Inputs;
using IEMSApps.BusinessObject.Responses;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using Java.Lang;
using Newtonsoft.Json;

namespace IEMSApps.BLL
{
    public static class KompaunBll
    {
        public static Dictionary<string, string> GetAllAkta()
        {
            var result = new Dictionary<string, string>();
            result.Add("", "");

            var listData = DataAccessQuery<TbAkta>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var data in list)
                {
                    result.Add(data.KodAkta, data.Prgn);
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetAllAktaKots()
        {
            var result = new Dictionary<string, string>();
            result.Add("", "");

            var listData = DataAccessQuery<TbAkta>.GetAll();
            if (listData.Success)
            {
                var listAkta = listData.Datas.ToList();

                var listKesalahan = DataAccessQuery<TbKesalahan>.GetAll();
                if (listKesalahan.Success)
                {
                    var listDatas = listKesalahan.Datas.Where(c => c.KOTS == Constants.KompaunKots).ToList();
                    foreach (var tbAkta in listAkta)
                    {
                        var kotsAkta = listDatas.FirstOrDefault(c => c.KodAkta == tbAkta.KodAkta);
                        if (kotsAkta != null)
                        {
                            result.Add(tbAkta.KodAkta, tbAkta.Prgn);
                        }
                    }

                }

            }

            return result;
        }

        public static Dictionary<string, string> GetAllKesalahanByAkta(string kodAkta)
        {
            var result = new Dictionary<string, string>();

            var listData = DataAccessQuery<TbKesalahan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.Where(c => c.KodAkta == kodAkta).ToList();
                foreach (var data in list)
                {

                    result.Add(data.KodSalah.ToString(), data.Prgn);
                }
            }

            return result;
        }



        //public static Result<TbKompaun> SaveKompaun(TbKompaun input)
        //{
        //    if (!HandheldBll.CreateOrUpdateHandheldData(Enums.PrefixType.KOTS, input.NoKmp))
        //    {
        //        var data = new Result<TbKompaun>
        //        {
        //            Success = false,
        //            Message = "Ralat Kemaskini No Rujukan"
        //        };
        //        return data;
        //        //throw new Exception("Ralat Kemaskini No Rujukan");
        //    }
        //
        //    var listData = DataAccessQuery<TbKpp>.GetAll();
        //    if (listData.Success)
        //    {
        //        var tbKpp = listData.Datas.FirstOrDefault(c => c.NoRujukanKpp == input.NoRujukanKpp);
        //        if (tbKpp != null)
        //        {
        //            input.KodKatPremis = tbKpp.KodKatPremis;
        //        }
        //
        //    }
        //    var result = DataAccessQuery<TbKompaun>.Insert(input);
        //    if (result.Success)
        //    {
        //        SendOnlineBll.InserDataOnline(input.NoKmp, Enums.TableType.Kompaun);
        //        SendOnlineBll.InserDataOnline(input.NoKmp, Enums.TableType.Kompaun_HH);
        //
        //        SendOnlineBll.InserImagesDataOnline(input.NoKmp);
        //
        //        SaveKompaunDataKes(input);
        //    }
        //    else
        //    {
        //        Log.WriteLogFile("KompaunBll", "SaveKompaun", "Error Insert Kompaun : " + input.NoKmp, Enums.LogType.Debug);
        //        Log.WriteLogFile("KompaunBll", "SaveKompaun", "Message : " + result.Message, Enums.LogType.Debug);
        //        Log.WriteLogFile("KompaunBll", "SaveKompaun", "Data : " + JsonConvert.SerializeObject(input), Enums.LogType.Debug);
        //
        //        HandheldBll.UpdateRollBackNumberHandheldData(Enums.PrefixType.KOTS, input.NoKmp);
        //    }
        //    return result;
        //}

        public static decimal GetAmountKesalahan(string kodAkta, int kodSalah, bool isIndividu)
        {
            var result = new Dictionary<string, string>();

            var listData = DataAccessQuery<TbKesalahan>.GetAll();
            if (listData.Success)
            {
                var data = listData.Datas.FirstOrDefault(c => c.KodAkta == kodAkta && c.KodSalah == kodSalah);
                if (data != null)
                {
                    return isIndividu ? data.AmnKmp_Ind : data.AmnKmp_Sya;
                }

            }

            return 0;
        }

        public static Result<TbKompaun> GetKompaunByRujukan(string noRujukan)
        {
            Result<TbKompaun> data;
            data = DataAccessQuery<TbKompaun>.Get(c => c.NoKmp == noRujukan);
            return data;
        }

        public static TbKompaun GetKompaunByRujukanKpp(string noRujukan)
        {
            Result<TbKompaun> data;
            data = DataAccessQuery<TbKompaun>.Get(c => c.NoRujukanKpp == noRujukan);
            return data.Datas;
        }

        public static TbDataKes GetSiasatByRujukanKpp(string noRujukan)
        {
            Result<TbDataKes> data;
            data = DataAccessQuery<TbDataKes>.Get(c => c.NoKpp == noRujukan);
            return data.Datas;
        }

        public static TbDataKes GetSiasatByNoKes(string noKes)
        {
            Result<TbDataKes> data;
            data = DataAccessQuery<TbDataKes>.Get(c => c.NoKes == noKes);
            return data.Datas;
        }

        public static TbDataKes GetSiasatByNoKmp(string noKmp)
        {
            Result<TbDataKes> data;
            data = DataAccessQuery<TbDataKes>.Get(c => c.NoKmp == noKmp);
            return data.Datas;
        }

        public static Result<TbKompaun> UpdateKompaunAkuan(TbKompaun input)
        {
            return DataAccessQuery<TbKompaun>.Update(input);
        }

        //public static Result<TbDataKes> SaveSiasatLanjutan(TbDataKes input,
        //    TbDataKesKesalahan inputKesalahan, TbDataKesPesalah inputPesalah)
        //{
        //    if (!HandheldBll.CreateOrUpdateHandheldData(Enums.PrefixType.SiasatLanjutan, input.NoKes))
        //    {
        //        var data = new Result<TbDataKes>
        //        {
        //            Success = false,
        //            Message = "Ralat Kemaskini No Rujukan"
        //        };
        //        return data;
        //        //throw new Exception("Ralat Kemaskini No Rujukan");
        //    }
        //
        //    var result = DataAccessQuery<TbDataKes>.Insert(input);
        //    if (result.Success)
        //    {
        //        SendOnlineBll.InserDataOnline(input.NoKes, Enums.TableType.DataKes);
        //        SendOnlineBll.InserDataOnline(input.NoKes, Enums.TableType.DataKes_HH);
        //        SendOnlineBll.InserImagesDataOnline(input.NoKes);
        //
        //        DataAccessQuery<TbDataKesKesalahan>.Insert(inputKesalahan);
        //        DataAccessQuery<TbDataKesPesalah>.Insert(inputPesalah);
        //        SaveDataKesAsasTindakan(input.NoKes, input.PgnDaftar, input.TrkhDaftar, input.NoKpp);
        //    }
        //    else
        //    {
        //        Log.WriteLogFile("KompaunBll", "SaveSiasatLanjutan", "Error Insert SiasatLanjutan : " + input.NoKes, Enums.LogType.Debug);
        //        Log.WriteLogFile("KompaunBll", "SaveSiasatLanjutan", "Message : " + result.Message, Enums.LogType.Debug);
        //        Log.WriteLogFile("KompaunBll", "SaveSiasatLanjutan", "Data : " + JsonConvert.SerializeObject(input), Enums.LogType.Debug);
        //
        //        HandheldBll.UpdateRollBackNumberHandheldData(Enums.PrefixType.SiasatLanjutan, input.NoKes);
        //    }
        //    return result;
        //
        //}





        //public static void KompaunIzinCheckThread(string noRujukan)
        //{
        //    Thread t = new Thread(() =>
        //    {
        //        CheckServiceKompaunIzin(noRujukan);
        //    });
        //    t.Start();
        //}

        //private static void CheckServiceKompaunIzin(string noRujukan)
        //{

        //    for (int i = 1; i <= Constants.MaxKompaunIzinRetry; i++)
        //    {
        //        //call service
        //        var result = Task.Run(async () => await HttpClientService.CheckKompaunIzin(noRujukan)).Result;
        //        if (result.Success)
        //        {
        //            var statusIzin = result.Result.Status;

        //            UpdateTbKompaunIzinStatus(noRujukan, statusIzin);
        //            return;
        //        }
        //        Thread.Sleep(Constants.SleepRetryKompaunIzin);
        //    }

        //    //failed 
        //    UpdateTbKompaunIzinStatus(noRujukan, Enums.StatusIzinKompaun.Denied);
        //}

        public static Response<CheckKompaunIzinResponse> CheckServiceKompaunIzin(string noRujukan, Android.Content.Context context)
        {
            var result = new Response<CheckKompaunIzinResponse>();

            if (!GeneralAndroidClass.IsOnline())
            {
                result.Success = false;
                result.Mesage = Constants.ErrorMessages.NoInternetConnection;
                return result;
            }

            //call service
            result = Task.Run(async () => await HttpClientService.CheckKompaunIzin(noRujukan)).Result;
            if (result.Success)
            {
                var statusIzin = result.Result.Status;
                var catatan = result.Result.Catatan;
                var ip_status_api = result.Result.ip_status_api;

                if (statusIzin == Enums.StatusIzinKompaun.Approved ||
                    statusIzin == Enums.StatusIzinKompaun.Denied)
                {
                    UpdateTbKompaunIzinStatus(noRujukan, statusIzin, catatan, ip_status_api);
                }

            }
            else
            {
                if (result.Mesage == Constants.ErrorMessages.NotFound)
                {
                    SentKompaunIzin(noRujukan, context);
                    result.Success = true;
                    result.Result = new CheckKompaunIzinResponse { Status = Enums.StatusIzinKompaun.Waiting };
                }
            }

            return result;
        }

        public static Result<TbKompaunIzin> SentKompaunIzin(string noRujukan, Android.Content.Context context)
        {
            var result = new Result<TbKompaunIzin>();
            var data = DataAccessQuery<TbKompaunIzin>.Get(c => c.NoRujukanKpp == noRujukan);

            //var data = new TbKompaunIzin
            //{
            //    NoRujukanKpp = noRujukan,
            //    KodCawangan = GeneralBll.GetUserCawangan(),
            //    TrkhMohon = GeneralBll.GetLocalDateTimeForDatabase(),
            //    Status = Enums.StatusIzinKompaun.Waiting,
            //    PgnDaftar = GeneralBll.GetUserStaffId(),
            //    TrkhDaftar = GeneralBll.GetLocalDateTimeForDatabase(),
            //    PgnAkhir = GeneralBll.GetUserStaffId(),
            //    TrkhAkhir = GeneralBll.GetLocalDateTimeForDatabase()
            //};

            if (data.Success && data.Datas != null)
            {
                var sqlQuery = "INSERT INTO tbkompaun_izin(NoRujukanKpp,KodCawangan,TrkhMohon,Status," +
                               "PgnDaftar,TrkhDaftar,PgnAkhir,TrkhAkhir)" +
                               " VALUES('" + data.Datas.NoRujukanKpp + "'" +
                               " ,'" + data.Datas.KodCawangan + "'" +
                               " ,'" + data.Datas.TrkhMohon + "'" +
                               " ," + (int)data.Datas.Status +
                               " ,'" + data.Datas.PgnDaftar + "'" +
                               " ," + GeneralBll.GetUnixDateTimeQuery(data.Datas.TrkhDaftar) +
                               " ,'" + data.Datas.PgnAkhir + "'" +
                               " ," + GeneralBll.GetUnixDateTimeQuery(data.Datas.TrkhAkhir) + ")";



                if (!GeneralAndroidClass.IsOnline())
                {
                    result.Success = false;
                    result.Message = Constants.ErrorMessages.NoInternetConnection;
                }
                else
                {
                    var resultInsert = Task.Run(async () => await HttpClientService.ExecuteQuery(sqlQuery, context)).Result;
                    if (!resultInsert.Success)
                    {
                        result.Success = false;
                        result.Message = resultInsert.Mesage;
                    }
                }

            }




            return result;
        }

        public static void UpdateTbKompaunIzinStatus(string noRujukan, Enums.StatusIzinKompaun statusIzin, string catatan, int ip_status_api)
        {
            var data = DataAccessQuery<TbKompaunIzin>.Get(c => c.NoRujukanKpp == noRujukan);
            if (data.Success && data.Datas != null)
            {
                data.Datas.Status = statusIzin;
                data.Datas.Catatan = catatan;
                data.Datas.ip_status_api = ip_status_api;
                DataAccessQuery<TbKompaunIzin>.Update(data.Datas);
            }
        }

        public static Result<TbKompaunIzin> GetKompaunIzinByRujukanAndStatus(string noRujukan, Enums.StatusIzinKompaun status)
        {
            return DataAccessQuery<TbKompaunIzin>.Get(c => c.NoRujukanKpp == noRujukan && c.Status == status);

        }

        //public static bool SaveKompaunDataKes(TbKompaun kompaun)
        //{
        //    var noKes = GeneralBll.GenerateNoRujukan(Enums.PrefixType.SiasatLanjutan);
        //    var trkhDaftar = GeneralBll.GetLocalDateTimeForDatabase();
        //
        //    var inputDataKes = new TbDataKes
        //    {
        //        NoKes = noKes,
        //        KodCawangan = GeneralBll.GetUserCawangan(),
        //        NoEp = kompaun.NoEp,
        //        NoIp = kompaun.NoIp,
        //        TrkhSalah = kompaun.TrkhKmp,
        //        Tempat = kompaun.TempatSalah,
        //        NoKpp = kompaun.NoRujukanKpp,
        //        NoKmp = kompaun.NoKmp,
        //        KodKatKawasan = "",
        //        KodTujuan = 0,
        //        PegawaiSerbuan = 0,
        //        NamaPremis = kompaun.NamaPremis,
        //        NoDaftarPremis = "",
        //        KodKatPerniagaan = 0,
        //        KodJenama = 0,
        //        NoLaporanPolis = kompaun.NoLaporanPolis,
        //        KodStatusKes = "BS",
        //
        //        Status = Constants.Status.Aktif,
        //        PgnDaftar = kompaun.PgnDaftar,
        //        TrkhDaftar = kompaun.TrkhDaftar,
        //        PgnAkhir = kompaun.PgnAkhir,
        //        TrkhAkhir = kompaun.TrkhAkhir
        //
        //    };
        //
        //    //get kpp
        //    //from tindakan = 1 = KOTS
        //    var tbKpp = PemeriksaanBll.GetPemeriksaanByRujukan(kompaun.NoRujukanKpp);
        //    if (tbKpp != null)
        //    {
        //        if (tbKpp.KodKatPremis == 1)
        //        {
        //            inputDataKes.KelasKes = "A1";
        //        }
        //        else if (tbKpp.KodKatPremis == 2)
        //        {
        //            inputDataKes.KelasKes = "A2";
        //        }
        //        inputDataKes.KodStatusKes_Det = "BS04";
        //
        //        inputDataKes.KodKatKawasan = tbKpp.KodKatKawasan;
        //        inputDataKes.KodTujuan = tbKpp.KodTujuan;
        //
        //
        //        if (tbKpp.Tindakan == 1 && kompaun.AmnByr > 0)
        //        {
        //            inputDataKes.KodStatusKes = "S";
        //            inputDataKes.KodStatusKes_Det = "S02";
        //        }
        //        else
        //        {
        //            inputDataKes.KodStatusKes = "BS";
        //            inputDataKes.KodStatusKes_Det = "BS04";
        //        }
        //
        //        if (tbKpp.Tindakan == 2)
        //        {
        //            inputDataKes.KodStatusKes_Det = "BS01";
        //        }
        //    }
        //    if (!HandheldBll.CreateOrUpdateHandheldData(Enums.PrefixType.SiasatLanjutan, noKes))
        //    {
        //        Log.WriteLogFile("KompaunBll", "SaveKompaunDataKes", "Ralat Kemaskini No Rujukan", Enums.LogType.Error);
        //        Log.WriteLogFile("KompaunBll", "SaveKompaunDataKes", "noKes : " + noKes, Enums.LogType.Error);
        //        return false;
        //        //throw new Exception("Ralat Kemaskini No Rujukan");
        //    }
        //
        //    var result = DataAccessQuery<TbDataKes>.Insert(inputDataKes);
        //    if (result.Success)
        //    {
        //        SendOnlineBll.InserDataOnline(noKes, Enums.TableType.DataKes);
        //        SendOnlineBll.InserDataOnline(noKes, Enums.TableType.DataKes_HH);
        //
        //        SaveDataKesKesalahan(noKes, inputDataKes.PgnDaftar, trkhDaftar, kompaun.KodAkta, kompaun.KodSalah,
        //            kompaun.ButirSalah);
        //
        //        SaveDataKesAsasTindakan(noKes, inputDataKes.PgnDaftar, trkhDaftar, kompaun.NoRujukanKpp);
        //        SaveDataKesPesalah(noKes, inputDataKes.PgnDaftar, trkhDaftar, kompaun.NoKpOkk, kompaun.NamaOkk,
        //            kompaun.AlamatOkk1, kompaun.AlamatOkk2, kompaun.AlamatOkk3);
        //
        //    }
        //    else
        //    {
        //        Log.WriteLogFile("KompaunBll", "SaveKompaunDataKes", "Error Insert SiasatLanjutan : " + noKes, Enums.LogType.Debug);
        //        Log.WriteLogFile("KompaunBll", "SaveKompaunDataKes", "Message : " + result.Message, Enums.LogType.Debug);
        //        Log.WriteLogFile("KompaunBll", "SaveKompaunDataKes", "Data : " + JsonConvert.SerializeObject(inputDataKes), Enums.LogType.Debug);
        //
        //        HandheldBll.UpdateRollBackNumberHandheldData(Enums.PrefixType.SiasatLanjutan, noKes);
        //    }
        //    return true;
        //}

        private static bool SaveDataKesKesalahan(string noKes, int userId, string trkhDaftar,
            string kodAkta, int kodSalah, string butirSalah, DataAccessQueryTrx insAccess = null)
        {

            var data = new TbDataKesKesalahan
            {
                NoKes = noKes,
                KodCawangan = GeneralBll.GetUserCawangan(),
                KodAkta = kodAkta,
                KodSalah = kodSalah,
                ButirSalah = butirSalah,
                IsSendOnline = Enums.StatusOnline.New,
                PgnDaftar = userId,
                TrkhDaftar = trkhDaftar
            };

            if (insAccess != null)
            {
                return insAccess.InsertTrx(data);
            }

            DataAccessQuery<TbDataKesKesalahan>.Insert(data);
            return true;
        }

        private static bool SaveDataKesAsasTindakan(string noKes, int userId, string trkhDaftar,
            string rujukanKpp, DataAccessQueryTrx insAccess = null)
        {
            var listData = DataAccessQuery<TbKppAsasTindakan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.Where(c => c.NoRujukanKpp == rujukanKpp)
                    .ToList();
                foreach (var tbKppAsasTindakan in list)
                {
                    var input = new TbDataKesAsasTindakan
                    {
                        NoKes = noKes,
                        KodTujuan = tbKppAsasTindakan.KodTujuan,
                        KodAsas = tbKppAsasTindakan.KodAsas,
                        IsSendOnline = Enums.StatusOnline.New,
                        PgnDaftar = userId,
                        TrkhDaftar = trkhDaftar
                    };

                    if (insAccess != null)
                    {
                        if (!insAccess.InsertTrx(input))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        DataAccessQuery<TbDataKesAsasTindakan>.Insert(input);
                    }

                }
            }
            return true;
        }

        private static bool SaveDataKesPesalah(string noKes, int userId, string trkhDaftar, string noKpOkk,
            string namaOkk, string alamatOkk1, string alamatOkk2, string alamatOkk3, DataAccessQueryTrx insAccess = null)
        {

            var data = new TbDataKesPesalah()
            {
                NoKes = noKes,
                KodCawangan = GeneralBll.GetUserCawangan(),
                NoKpOks = noKpOkk,
                NamaOks = namaOkk,
                AlamatOks1 = alamatOkk1,
                AlamatOks2 = alamatOkk2,
                AlamatOks3 = alamatOkk3,
                IsSendOnline = Enums.StatusOnline.New,
                PgnDaftar = userId,
                TrkhDaftar = trkhDaftar
            };

            if (insAccess != null)
            {
                return insAccess.InsertTrx(data);
            }

            DataAccessQuery<TbDataKesPesalah>.Insert(data);
            return true;
        }

        public static TbDataKesKesalahan GetDataKesKesalahan(string noKes)
        {
            var data = DataAccessQuery<TbDataKesKesalahan>.Get(c => c.NoKes == noKes);
            return data.Datas;
        }

        public static TbDataKesPesalah GetDataKesPesalah(string noKes)
        {
            var data = DataAccessQuery<TbDataKesPesalah>.Get(c => c.NoKes == noKes);
            return data.Datas;
        }

        public static bool SaveKompaunTrx(TbKompaun input)
        {
            var insAccess = new DataAccessQueryTrx();
            if (insAccess.BeginTrx())
            {
                var noKes = GeneralBll.GenerateNoRujukan(Enums.PrefixType.SiasatLanjutan);

                if (!HandheldBll.CreateOrUpdateHandheldDataTrx(Enums.PrefixType.KOTSAndSiasatLanjutan, input.NoKmp, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                var listData = DataAccessQuery<TbKpp>.GetAll();
                if (listData.Success)
                {
                    var tbKpp = listData.Datas.FirstOrDefault(c => c.NoRujukanKpp == input.NoRujukanKpp);
                    if (tbKpp != null)
                    {
                        input.KodKatPremis = tbKpp.KodKatPremis;
                    }

                }

                if (!insAccess.InsertTrx(input))
                {
                    insAccess.RollBackTrx();
                    return false;
                }


                if (!SendOnlineBll.InserDataOnline(input.NoKmp, Enums.TableType.Kompaun, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SendOnlineBll.InserDataOnline(input.NoKmp, Enums.TableType.Kompaun_HH, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SendOnlineBll.InserImagesDataOnline(input.NoKmp, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }


                if (!SaveKompaunDataKesTrx(input, noKes, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                return insAccess.CommitTrx();

            }

            return false;
        }

        public static bool SaveKompaunDataKesTrx(TbKompaun kompaun, string noKes, DataAccessQueryTrx insAccess)
        {


            var trkhDaftar = GeneralBll.GetLocalDateTimeForDatabase();

            var inputDataKes = new TbDataKes
            {
                NoKes = noKes,
                KodCawangan = GeneralBll.GetUserCawangan(),
                NoEp = kompaun.NoEp,
                NoIp = kompaun.NoIp,
                TrkhSalah = kompaun.TrkhKmp,
                Tempat = kompaun.TempatSalah,
                NoKpp = kompaun.NoRujukanKpp,
                NoKmp = kompaun.NoKmp,
                KodKatKawasan = "",
                KodTujuan = 0,
                //PegawaiSerbuan = null,
                PegawaiSerbuan = kompaun.PegawaiPengeluar, //Zihan set PegawaiSerbuan = kompaun.PegawaiPengeluar
                NamaPremis = kompaun.NamaPremis,
                NoDaftarPremis = kompaun.NoDaftarPremis,
                KodKatPerniagaan = null,
                KodJenama = null,
                NoLaporanPolis = kompaun.NoLaporanPolis,
                KodStatusKes = "BS",

                Status = Constants.Status.Aktif,
                PgnDaftar = kompaun.PgnDaftar,
                TrkhDaftar = kompaun.TrkhDaftar,
                PgnAkhir = kompaun.PgnAkhir,
                TrkhAkhir = kompaun.TrkhAkhir

            };


            //get kpp
            //from tindakan = 1 = KOTS
            var tbKpp = PemeriksaanBll.GetPemeriksaanByRujukan(kompaun.NoRujukanKpp);
            if (tbKpp != null)
            {
                if (tbKpp.KodKatPremis == 1)
                {
                    inputDataKes.KelasKes = "A1";
                }
                else if (tbKpp.KodKatPremis == 2)
                {
                    inputDataKes.KelasKes = "A2";
                }
                inputDataKes.KodStatusKes_Det = "BS04";

                inputDataKes.KodKatKawasan = tbKpp.KodKatKawasan;
                inputDataKes.KodTujuan = tbKpp.KodTujuan;

                if (tbKpp.Tindakan == 1 && kompaun.AmnByr > 0)
                {
                    inputDataKes.KodStatusKes = "S";
                    inputDataKes.KodStatusKes_Det = "S02";
                }
                else
                {
                    inputDataKes.KodStatusKes = "BS";
                    inputDataKes.KodStatusKes_Det = "BS04";
                }

                if (tbKpp.Tindakan == 2)
                {
                    inputDataKes.KodStatusKes_Det = "BS01";
                }
            }
            //if (!HandheldBll.CreateOrUpdateHandheldDataTrx(Enums.PrefixType.SiasatLanjutan, noKes, insAccess))
            //{
            //    insAccess.RollBackTrx();
            //    return false;
            //}

            if (!insAccess.InsertTrx(inputDataKes))
            {
                insAccess.RollBackTrx();
                return false;
            }

            if (!SendOnlineBll.InserDataOnline(noKes, Enums.TableType.DataKes, insAccess))
            {
                insAccess.RollBackTrx();
                return false;
            }

            if (!SendOnlineBll.InserDataOnline(noKes, Enums.TableType.DataKes_HH, insAccess))
            {
                insAccess.RollBackTrx();
                return false;
            }


            if (!SaveDataKesKesalahan(noKes, inputDataKes.PgnDaftar, trkhDaftar, kompaun.KodAkta, kompaun.KodSalah,
                kompaun.ButirSalah, insAccess))
            {
                insAccess.RollBackTrx();
                return false;
            }

            if (!SaveDataKesAsasTindakan(noKes, inputDataKes.PgnDaftar, trkhDaftar, kompaun.NoRujukanKpp, insAccess))
            {
                insAccess.RollBackTrx();
                return false;
            }

            if (!SaveDataKesPesalah(noKes, inputDataKes.PgnDaftar, trkhDaftar, kompaun.NoKpOkk, kompaun.NamaOkk,
                kompaun.AlamatOkk1, kompaun.AlamatOkk2, kompaun.AlamatOkk3, insAccess))
            {
                insAccess.RollBackTrx();
                return false;
            }


            return true;
        }

        public static bool SaveSiasatLanjutanTrx(TbDataKes input,
            TbDataKesKesalahan inputKesalahan, TbDataKesPesalah inputPesalah)
        {
            var insAccess = new DataAccessQueryTrx();
            if (insAccess.BeginTrx())
            {
                if (!HandheldBll.CreateOrUpdateHandheldDataTrx(Enums.PrefixType.SiasatLanjutan, input.NoKes, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!insAccess.InsertTrx(input))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SendOnlineBll.InserDataOnline(input.NoKes, Enums.TableType.DataKes, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SendOnlineBll.InserDataOnline(input.NoKes, Enums.TableType.DataKes_HH, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SendOnlineBll.InserImagesDataOnline(input.NoKes, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!insAccess.InsertTrx(inputKesalahan))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!insAccess.InsertTrx(inputPesalah))
                {
                    insAccess.RollBackTrx();
                    return false;
                }


                if (!SaveDataKesAsasTindakan(input.NoKes, input.PgnDaftar, input.TrkhDaftar, input.NoKpp, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                return insAccess.CommitTrx();
            }


            return false;

        }

        public static bool SaveDataAkuanTrx(SaveAkuanInput input)
        {
            var insAccess = new DataAccessQueryTrx();
            if (insAccess.BeginTrx())
            {
                var data = GetKompaunByRujukan(input.NoRujukan);
                var photoName = SendOnlineBll.InsertImagesReceiptNameOnKPP(data.Datas.NoRujukanKpp);
                if (data.Success && data.Datas != null)
                {
                    data.Datas.NamaPenerima_Akuan = input.NamaPenerima;
                    data.Datas.ip_identiti_pelanggan_id_akuan = input.jeniskad;
                    data.Datas.NoKpPenerima_Akuan = input.NoKpPenerima;
                    data.Datas.notelpenerima_akuan = input.notelpenerima;
                    data.Datas.emelpenerima_akuan = input.emelpenerima;
                    data.Datas.negeripenerima_akuan = input.negeripenerima;
                    data.Datas.negarapenerima_akuan = input.negarapenerima;
                    data.Datas.bandarpenerima_akuan = input.bandarpenerima;
                    data.Datas.poskodpenerima_akuan = input.poskodpenerima;
                    data.Datas.AlamatPenerima1_Akuan = input.AlamatPenerima1;
                    data.Datas.AlamatPenerima2_Akuan = input.AlamatPenerima2;
                    data.Datas.AlamatPenerima3_Akuan = input.AlamatPenerima3;
                    data.Datas.NoResit = input.NoResit;
                    data.Datas.AmnByr = input.AmountByr;
                    data.Datas.TrkhPenerima_Akuan = input.TrkhPenerima;
                    data.Datas.isbayarmanual = input.isbayarmanual;
                    data.Datas.gambarbuktibayaran = photoName;
                    if (!insAccess.UpdateTrx(data.Datas))
                    {
                        insAccess.RollBackTrx();
                        return false;
                    }

                    var dataKes = GetSiasatByNoKmp(data.Datas.NoKmp);
                    var tbKpp = PemeriksaanBll.GetPemeriksaanByRujukan(data.Datas.NoRujukanKpp);
                    if (dataKes != null)
                    {
                        if (tbKpp?.Tindakan == 1 && input.AmountByr > 0)
                        {
                            dataKes.KodStatusKes = "S";
                            dataKes.KodStatusKes_Det = "S02";
                        }
                        else
                        {
                            dataKes.KodStatusKes = "BS";
                            dataKes.KodStatusKes_Det = "BS04";
                        }

                        if (tbKpp?.Tindakan == 2)
                        {
                            dataKes.KodStatusKes_Det = "BS01";
                        }
                            
                        if (!insAccess.UpdateTrx(dataKes))
                        {
                            insAccess.RollBackTrx();
                            return false;
                        }

                        //add function for Update data on Server
                        Log.WriteLogFile("KompaundBll - SaveDataAkuanTrx", "UpdateDataKesAfterPaid", "Updating...", Enums.LogType.Debug);
                        var result = Task.Run(async () => await SendOnlineBll.UpdateDataKesAfterPaid(dataKes.NoKes, dataKes.KodStatusKes, dataKes.KodStatusKes_Det)).Result;
                        if (!result.Success)
                        {
                            Log.WriteLogFile("KompaundBll - SaveDataAkuanTrx", "UpdateDataKesAfterPaid", result.Mesage, Enums.LogType.Error);
                        }

                        result = Task.Run(async () => await SendOnlineBll.UpdateDataKesHHAfterPaid(dataKes.NoKes, dataKes.KodStatusKes, dataKes.KodStatusKes_Det)).Result;
                        if (!result.Success)
                        {
                            Log.WriteLogFile("KompaundBll - SaveDataAkuanTrx", "UpdateDataKesHHAfterPaid", result.Mesage, Enums.LogType.Error);
                        }

                        result = Task.Run(async () => await SendOnlineBll.InsertTbDataKesTindakan(dataKes.NoKes, input.AmountByr)).Result;
                        if (!result.Success)
                        {
                            Log.WriteLogFile("KompaundBll - SaveDataAkuanTrx", "InsertTbDataKesTindakan", result.Mesage, Enums.LogType.Error);
                        }

                        Log.WriteLogFile("KompaundBll - SaveDataAkuanTrx", "UpdateDataKesAfterPaid", "Finish...", Enums.LogType.Debug);
                    }

                    AkuanBll.SavePusatTerimaanTrx(input, insAccess); // check balik

                    if (data.Datas.IsCetakAkuan == Constants.CetakAkuan.Yes)
                    {
                        if (!SendOnlineBll.InserDataOnline(input.NoRujukan, Enums.TableType.KompaunBayaran, insAccess))
                        {
                            insAccess.RollBackTrx();
                            return false;
                        }

                        if (!SendOnlineBll.InserDataOnline(input.NoRujukan, Enums.TableType.Akuan_UpdateKompaun, insAccess))
                        {
                            insAccess.RollBackTrx();
                            return false;
                        }

                        if (!SendOnlineBll.InserDataOnline(input.NoRujukan, Enums.TableType.Akuan_UpdateKompaun_HH, insAccess))
                        {
                            insAccess.RollBackTrx();
                            return false;
                        }

                        //new add.
                        if (data.Datas.isbayarmanual == 1)
                        {
                            if (!SendOnlineBll.InsertImagesDataOnlineReceipt(data.Datas.NoRujukanKpp, insAccess))
                            {
                                insAccess.RollBackTrx();
                                return false;
                            }
                            //if (!SendOnlineBll.InserDataOnline(data.Datas.NoRujukanKpp, Enums.TableType.IpResit_Manual, insAccess))
                            //{
                            //    insAccess.RollBackTrx();
                            //    return false;
                            //}
                        } else
                        {
                            // masuk dlm tbsendonline jika dia bayar cash . Tidak guna iPayment.
                            if (!SendOnlineBll.InserDataOnline(data.Datas.NoKmp, Enums.TableType.MaklumatBayaran, insAccess))
                            {
                                insAccess.RollBackTrx();
                                return false;
                            }
                        }

                    }

                    return insAccess.CommitTrx();

                }
            }

            return false;
        }

    }
}