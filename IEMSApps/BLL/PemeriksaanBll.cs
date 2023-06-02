
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Responses;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using Java.Lang;
using Newtonsoft.Json;
using Thread = System.Threading.Thread;

namespace IEMSApps.BLL
{
    public static class PemeriksaanBll
    {

        public static Dictionary<string, string> GetAllKategoriPremis(bool isAddDefault = true)
        {
            var result = new Dictionary<string, string>();
            if (isAddDefault)
            {
                result.Add("0", "");
            }
            var listData = DataAccessQuery<TbKategoriPremis>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var tbKategoriPremis in list)
                {
                    result.Add(tbKategoriPremis.KodKatPremis.ToString(), tbKategoriPremis.Prgn);
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetAllKategoriKawasan(bool isAddDefault = true)
        {
            var result = new Dictionary<string, string>();
            if (isAddDefault)
            {
                result.Add("0", "");
            }
            var listData = DataAccessQuery<TbKategoriKawasan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var tbKategoriKawasan in list)
                {
                    result.Add(tbKategoriKawasan.KodKatKawasan, tbKategoriKawasan.Prgn);
                }
            }

            return result;
        }


        public static Dictionary<string, string> GetAllTujuanLawatan(bool isAddDefault = true)
        {
            var result = new Dictionary<string, string>();
            if (isAddDefault)
            {
                result.Add("0", "");
            }
            var listData = DataAccessQuery<TbTujuanLawatan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var tbTujuanLawatan in list)
                {
                    result.Add(tbTujuanLawatan.KodTujuan.ToString(), tbTujuanLawatan.Prgn);
                }
            }

            return result;
        }


        public static Dictionary<string, string> GetAllAsasTindakanByTujuan(int kodTujuan, bool isAddDefault = true)
        {
            var result = new Dictionary<string, string>();
            if (isAddDefault)
            {
                result.Add("0", "");
            }
            var listData = DataAccessQuery<TbAsasTindakan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.Where(c => c.KodTujuan == kodTujuan)
                    .ToList();
                foreach (var tbAsasTindakan in list)
                {
                    result.Add(tbAsasTindakan.KodAsas.ToString(), tbAsasTindakan.Prgn);
                }
            }

            return result;
        }


        public static Dictionary<string, string> GetAllJenisPerniagaan(bool isAddDefault = true)
        {
            var result = new Dictionary<string, string>();
            if (isAddDefault)
            {
                result.Add("0", "");
            }
            var listData = DataAccessQuery<TbJenisPerniagaan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var tbJenisPerniagaan in list)
                {
                    result.Add(tbJenisPerniagaan.KodJenis.ToString(), tbJenisPerniagaan.Prgn);
                }
            }

            return result;
        }


        //public static Result<TbKpp> SavePemeriksaan(TbKpp input, List<int> listAsas)
        //{
        //    if (!HandheldBll.CreateOrUpdateHandheldData(Enums.PrefixType.KPP, input.NoRujukanKpp))
        //    {
        //        throw new Exception("Ralat Kemaskini No Rujukan");
        //    }
        //
        //    input.LongitudPremis = GeneralBll.GetLastSaveLongitude();
        //    input.LatitudPremis = GeneralBll.GetLastSaveLatitude();
        //
        //
        //    var listPhotos = GeneralBll.GetPhotoNameByRujukan(input.NoRujukanKpp);
        //    if (listPhotos.Count > 0)
        //    {
        //        input.GambarLawatan1 = listPhotos[0];
        //        if (listPhotos.Count > 1)
        //        {
        //            input.GambarLawatan2 = listPhotos[1];
        //        }
        //    }
        //
        //    var result = DataAccessQuery<TbKpp>.Insert(input);
        //    if (result.Success)
        //    {
        //        if (!SavePasukanTrans(Constants.JenisTrans.Kpp, input.NoRujukanKpp))
        //        {
        //            Log.WriteLogFile("PemeriksaanBll", "SavePemeriksaan",
        //                "SavePasukanTrans failed : " + input.NoRujukanKpp, Enums.LogType.Debug);
        //            Log.WriteLogFile("PemeriksaanBll", "SavePemeriksaan", "Data : " + JsonConvert.SerializeObject(input), Enums.LogType.Debug);
        //
        //            result.Success = false;
        //        }
        //
        //        SendOnlineBll.InserDataOnline(input.NoRujukanKpp, Enums.TableType.KPP);
        //        SendOnlineBll.InserDataOnline(input.NoRujukanKpp, Enums.TableType.KPP_HH);
        //        SendOnlineBll.InserImagesDataOnline(input.NoRujukanKpp);
        //
        //        SaveKppAsasTindakan(input.NoRujukanKpp, input.KodTujuan, listAsas);
        //    }
        //    else
        //    {
        //        Log.WriteLogFile("PemeriksaanBll", "SavePemeriksaan", "Error Insert Kpp : " + input.NoRujukanKpp, Enums.LogType.Debug);
        //        Log.WriteLogFile("PemeriksaanBll", "SavePemeriksaan", "Message : " + result.Message, Enums.LogType.Debug);
        //        Log.WriteLogFile("PemeriksaanBll", "SavePemeriksaan", "Data : " + JsonConvert.SerializeObject(input), Enums.LogType.Debug);
        //
        //        HandheldBll.UpdateRollBackNumberHandheldData(Enums.PrefixType.KPP, input.NoRujukanKpp);
        //
        //    }
        //    return result;
        //    //HandheldBll.UpdateRollBackNumberHandheldData(Enums.PrefixType.KPP, input.NoRujukanKpp);
        //    //var result = new Result<TbKpp>();
        //    //result.Success = false;
        //    //result.Message = "Constraint";
        //    //return result;
        //}


        private static bool SavePasukanTrans(int jenisTrans, string noRujukan)
        {
            var listData = DataAccessQuery<TbPasukanHh>.GetAll();
            if (listData.Success)
            {
                var listPasukan = listData.Datas;

                var userIdStaf = GeneralBll.GetUserStaffId();

                foreach (var tbPasukanDetail in listPasukan)
                {
                    var tbPasukanTrans = new TbPasukanTrans
                    {
                        JenisTrans = jenisTrans,
                        NoRujukan = noRujukan,
                        KodPasukan = tbPasukanDetail.KodPasukan,
                        Id = tbPasukanDetail.Id,
                        Status = tbPasukanDetail.Status,
                        PgnDaftar = userIdStaf,
                        PgnAkhir = userIdStaf,
                        IsSendOnline = Enums.StatusOnline.New,
                        Catatan = tbPasukanDetail.Catatan
                    };

                    var data = DataAccessQuery<TbPasukanTrans>.Insert(tbPasukanTrans);
                    if (!data.Success) return false;
                }

                return true;
            }


            return false;

        }

        //public static TbKpp GetPemeriksaanByRujukan(string noRujukan)
        //{
        //    var listData = DataAccessQuery<TbKpp>.GetAll();
        //    if (listData.Success)
        //    {
        //        var data = listData.Datas.FirstOrDefault(c =>
        //            c.Status == Constants.Status.Aktif && c.NoRujukanKpp == noRujukan);

        //        return data;
        //    }

        //    return null;
        //}

        public static TbKpp GetPemeriksaanByRujukan(string noRujukan)
        {
            var pemeriksaan = DataAccessQuery<TbKpp>.Get(c => c.NoRujukanKpp == noRujukan && c.Status == Constants.Status.Aktif);
            if (pemeriksaan.Success) return pemeriksaan.Datas;
            return null;
        }

        public static Dictionary<string, string> GetAllPasukanSerbu()
        {
            var result = new Dictionary<string, string>();

            var listData = DataAccessQuery<TbPasukanHh>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.Where(c => c.Status == Constants.Status.Aktif)
                    .ToList();
                foreach (var tbPasukanDetail in list)
                {
                    result.Add(tbPasukanDetail.Id.ToString(), LoginBll.GetNamaPenggunaByNoKp(tbPasukanDetail.NoKp));
                }
            }

            return result;
        }

        public static bool UpdatePemeriksaanTamatAkhir(string noRujukan)
        {

            var data = DataAccessQuery<TbKpp>.Get(c => c.NoRujukanKpp == noRujukan);
            if (data.Success && data.Datas != null)
            {
                data.Datas.TrkhTamatLawatanKpp = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat);

                var result = DataAccessQuery<TbKpp>.Update(data.Datas);
                if (result.Success) return true;
            }
            return false;
        }

        public static TbKompaun GetKompaunByRujukanKpp(string noRujukan)
        {
            var data = DataAccessQuery<TbKompaun>.Get(c => c.NoRujukanKpp == noRujukan);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }

            return null;
        }

        public static TbDataKes GetSiasatLanjutByRujukanKpp(string noRujukan)
        {
            var data = DataAccessQuery<TbDataKes>.Get(c => c.NoKpp == noRujukan);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }

            return null;
        }

        public static List<string> GetPasukanAhliByRujukan(string noRujukan)
        {
            string sSql = "SELECT b.* FROM tbpasukan_trans a " +
                          "inner join tbpengguna b on a.Id = b.Id " +
                          "inner join tbpasukan_hh c on a.Id = c.Id " +
                          "WHERE a.Status ='" + Constants.Status.Aktif + "' " +
                          "AND a.NoRujukan = '" + noRujukan + "'" +
                          //" AND JenisPengguna = " + (int)Enums.JenisPengguna.Ahli +
                          " ORDER BY C.Turutan";

            var listData = DataAccessQuery<TbPengguna>.ExecuteSelectSql(sSql);

            var result = new List<string>();

            foreach (var tbPengguna in listData)
            {
                result.Add($"{tbPengguna.Singkatan_Jawatan} {tbPengguna.Nama}");
            }

            return result;
        }

        public static string GetPasukanKetuaNameByRujukan(string noRujukan)
        {
            string sSql = "SELECT c.* FROM tbpasukan_trans a " +
                          "inner join tbpasukan_hh b on a.KodPasukan = b.KodPasukan and  a.Id = b.Id " +
                          "AND b.JenisPengguna = " + (int)Enums.JenisPengguna.Ketua +
                          " Inner join TbPengguna c on a.id = c.Id" +
                          " WHERE a.Status ='" + Constants.Status.Aktif + "'" +
                          " AND a.NoRujukan = '" + noRujukan + "'";


            var data = DataAccessQuery<TbPengguna>.ExecuteSelectSql(sSql).FirstOrDefault();

            return data != null ? data.Nama : "";
        }

        public static TbKompaunIzin GetKompaunIzinByRujukanKpp(string noRujukan)
        {
            var data = DataAccessQuery<TbKompaunIzin>.Get(c => c.NoRujukanKpp == noRujukan);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }

            return null;
        }

        private static bool SaveKppAsasTindakan(string noRujukan, int kodTujuan, List<AsasTindakanDto> listAsas, DataAccessQueryTrx insAccess = null)
        {
            var localDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat);
            var userId = GeneralBll.GetUserStaffId();

            foreach (var asas in listAsas)
            {
                var kppAsasTindakan = new TbKppAsasTindakan
                {
                    NoRujukanKpp = noRujukan,
                    KodTujuan = asas.KodTujuan,
                    KodAsas = asas.KodAsas,
                    PgnDaftar = userId,
                    PgnAkhir = userId,
                    TrkhDaftar = localDate,
                    TrkhAkhir = localDate,
                    IsSendOnline = Enums.StatusOnline.New
                };

                if (insAccess != null)
                {
                    if (!insAccess.InsertTrx(kppAsasTindakan))
                    {
                        return false;
                    }
                }

                else
                {
                    var result = DataAccessQuery<TbKppAsasTindakan>.Insert(kppAsasTindakan);
                    if (!result.Success)
                    {
                        return false;
                    }
                }


            }

            return true;

        }

        private static bool SaveKppLokalitiKategoriKhas(string noRujukan, int LokalitiKategoriKhas, List<LokalitiKategoriKhasDto> listLokalitiKategori, DataAccessQueryTrx insAccess = null)
        {
            var localDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat);
            var userId = GeneralBll.GetUserStaffId();

            foreach (var lokaliti in listLokalitiKategori)
            {
                var kppLokalitiKhas = new TbKppLokalitiKategoriKhas
                {
                    norujukankpp = noRujukan,
                    tblokaliti_kategori_khas_id = lokaliti.Id,
                    PgnDaftar = userId,
                    PgnAkhir = userId,
                    TrkhDaftar = localDate,
                    TrkhAkhir = localDate,
                    IsSendOnline = Enums.StatusOnline.New
                };

                if (insAccess != null)
                {
                    if (!insAccess.InsertTrx(kppLokalitiKhas))
                    {
                        return false;
                    }
                }

                else
                {
                    var result = DataAccessQuery<TbKppLokalitiKategoriKhas>.Insert(kppLokalitiKhas);
                    if (!result.Success)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool SaveKppAgensiTerlibat(string noRujukan, string kodAgensiTerlibat, List<AgensiSerahanDto> listAgensiTerlibat, DataAccessQueryTrx insAccess = null)
        {
            var localDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat);
            var userId = GeneralBll.GetUserStaffId();

            foreach (var agensi in listAgensiTerlibat)
            {
                var agensiTerlibat = new TbKppAgensiTerlibat
                {
                    norujukankpp = noRujukan,
                    kodserahagensi = agensi.kodserahagensi,
                    PgnDaftar = userId,
                    PgnAkhir = userId,
                    TrkhDaftar = localDate,
                    TrkhAkhir = localDate,
                    IsSendOnline = Enums.StatusOnline.New
                };

                if (insAccess != null)
                {
                    if (!insAccess.InsertTrx(agensiTerlibat))
                    {
                        return false;
                    }
                }

                else
                {
                    var result = DataAccessQuery<TbKppAgensiTerlibat>.Insert(agensiTerlibat);
                    if (!result.Success)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static Dictionary<string, string> GetAllTindakan(bool isAddDefault = true)
        {
            var result = new Dictionary<string, string>();
            if (isAddDefault)
            {
                result.Add("-", "");
            }
            result.Add("0", Constants.TindakanName.Pemeriksaan);
            result.Add("1", Constants.TindakanName.KOTS);
            result.Add("2", Constants.TindakanName.SiasatLanjut);
            result.Add("3", Constants.TindakanName.SiasatUlangan);
            result.Add("4", Constants.TindakanName.SerahanNotis);

            return result;
        }

        public static bool UpdatePemeriksaanSkipIzin(string noRujukan, int skipIzin)
        {

            var data = DataAccessQuery<TbKpp>.Get(c => c.NoRujukanKpp == noRujukan);
            if (data.Success && data.Datas != null)
            {
                data.Datas.IsSkipIzin = skipIzin;

                var result = DataAccessQuery<TbKpp>.Update(data.Datas);
                if (result.Success) return true;
            }
            return false;
        }

        public static Result<TbKompaunIzin> CheckKompaunIzin(string noRujukan)
        {
            var data = DataAccessQuery<TbKompaunIzin>.Get(c => c.NoRujukanKpp == noRujukan);
            //return data.Success && data.Datas == null ? CreateDefaultKompaunIzin(noRujukan) : data;
            return data;
        }

        public static Result<TbKompaunIzin> CreateDefaultKompaunIzin(string noRujukan, Android.Content.Context context)
        {

            var result = new Result<TbKompaunIzin>();

            if (!GeneralAndroidClass.IsOnline())
            {
                result.Success = false;
                result.Message = Constants.ErrorMessages.NoInternetConnection;
                return result;
            }


            result = Task.Run(async () => await CheckRellatedKompaunIzinDataAsync(noRujukan, context)).Result;

            if (!result.Success)
            {
                return result;
            }

            var data = new TbKompaunIzin
            {
                NoRujukanKpp = noRujukan,
                KodCawangan = GeneralBll.GetUserCawangan(),
                TrkhMohon = GeneralBll.GetLocalDateTimeForDatabase(),
                Status = Enums.StatusIzinKompaun.Waiting,
                PgnDaftar = GeneralBll.GetUserStaffId(),
                TrkhDaftar = GeneralBll.GetLocalDateTimeForDatabase(),
                PgnAkhir = GeneralBll.GetUserStaffId(),
                TrkhAkhir = GeneralBll.GetLocalDateTimeForDatabase()
            };


            var sqlQuery = "INSERT INTO tbkompaun_izin(NoRujukanKpp,KodCawangan,TrkhMohon,Status," +
                           "PgnDaftar,TrkhDaftar,PgnAkhir,TrkhAkhir)" +
                           " VALUES('" + data.NoRujukanKpp + "'" +
                           " ,'" + data.KodCawangan + "'" +
                           " ,'" + data.TrkhMohon + "'" +
                           " ," + (int)data.Status +
                           " ,'" + data.PgnDaftar + "'" +
                           " ," + GeneralBll.GetUnixDateTimeQuery(data.TrkhDaftar) +
                           " ,'" + data.PgnAkhir + "'" +
                           " ," + GeneralBll.GetUnixDateTimeQuery(data.TrkhAkhir) + ")";


            result = DataAccessQuery<TbKompaunIzin>.Insert(data);
            if (result.Success)
            {
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


        public static async Task<Result<TbKompaunIzin>> CheckRellatedKompaunIzinDataAsync(string noRujukan, Android.Content.Context context)
        {
            var result = new Result<TbKompaunIzin>
            {
                Success = true
            };

            var tbKpp = GetPemeriksaanByRujukan(noRujukan);
            if (tbKpp == null)
            {
                result.Success = false;
                result.Message = "KPP tidak dijumpai";
                return result;
            }

            var onlineData = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KPP_HH);
            if (onlineData.Success && onlineData.Datas?.Status != Enums.StatusOnline.Sent)
            {

                //Send KPP_HH
                var query = SendOnlineBll.GenerateSQLScriptForTableKpp_Hh(tbKpp);
                var response = await HttpClientService.ExecuteQuery(query, context);
                SendOnlineBll.SetStatusDataOnline(noRujukan, Enums.TableType.KPP_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                if (!response.Success)
                {
                    result.Success = false;
                    result.Message = string.Format(Constants.ErrorMessages.ErrorSendRellatedData, "KPP_HH");//"KPP_HH tidak dihantar";
                    return result;
                }
            }

            onlineData = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KPP);
            if (onlineData.Success && onlineData.Datas?.Status != Enums.StatusOnline.Sent)
            {

                //This function will check data on server
                var isDataExists = await HttpClientService.GetKPPAsync(noRujukan);
                if (!isDataExists.Success && isDataExists.Result == null)
                {
                    //Send KPP
                    var query = SendOnlineBll.GenerateSQLScriptForTableKpp(tbKpp);
                    var response = await HttpClientService.ExecuteQuery(query, context);
                    SendOnlineBll.SetStatusDataOnline(noRujukan, Enums.TableType.KPP, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                    if (!response.Success)
                    {
                        result.Success = false;
                        result.Message = string.Format(Constants.ErrorMessages.ErrorSendRellatedData, "KPP");//"KPP not send";
                        return result;
                    }
                }
                else
                {
                    SendOnlineBll.SetStatusDataOnline(noRujukan, Enums.TableType.KPP, Enums.StatusOnline.Sent);
                }
            }

            //Pasukan Trans
            var allPasukanSend = true;
            //            var pasukanTrans = DataAccessQuery<TbPasukanTrans>.GetAll();
            //            if (pasukanTrans.Success)
            //            {
            //                var datas = pasukanTrans.Datas.Where(m => m.JenisTrans == Constants.JenisTrans.Kpp &&
            //                   m.IsSendOnline != Enums.StatusOnline.Sent && m.NoRujukan == noRujukan);                
            //                foreach (var item in datas)
            //                {
            //                    var query = SendOnlineBll.GenerateSQLScriptForTablePasukan_Trans(item);
            //                    var response = await HttpClientService.ExecuteQuery(query, context);
            //                    SendOnlineBll.SetStatusOnlinePasukanTrans(item.Id, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, item.NoRujukan);
            //                    if (!response.Success)
            //                    {
            //                        allPasukanSend = false;
            //                    }
            //                }

            //#if !DEBUG
            //                if (!allPasukanSend)
            //                {
            //                    result.Success = false;
            //                    result.Message = string.Format(Constants.ErrorMessages.ErrorSendRellatedData, "Pasukan Trans");// "Pasukan Trans Tidak Dihantar";
            //                    return result;
            //                }
            //#endif
            //            }

            var pasukanTrans = DataAccessQuery<TbPasukanTrans>.GetAll(m => m.JenisTrans == Constants.JenisTrans.Kpp && m.IsSendOnline != Enums.StatusOnline.Sent && m.NoRujukan == noRujukan);
            if (pasukanTrans.Success)
            {
                var query = string.Empty;
                var pasukanTransAfterSplit = pasukanTrans.Datas.Split();
                foreach (var items in pasukanTransAfterSplit)
                {
                    query = string.Empty;
                    foreach (var item in items)
                    {
                        query = SendOnlineBll.GenerateSQLScriptForTablePasukan_Trans(item, query);
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (query.EndsWith(","))
                            query = query.Remove(query.Length - 1) + ";";
                        var response = await HttpClientService.ExecuteQuery(query, context);

                        foreach (var item in items)
                        {
                            SendOnlineBll.SetStatusOnlinePasukanTrans(item.Id, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, item.NoRujukan);
                        }

                        if (!response.Success)
                        {
                            allPasukanSend = false;
                        }
                    }
                }

#if !DEBUG
                if (!allPasukanSend)
                {
                    result.Success = false;
                    result.Message = string.Format(Constants.ErrorMessages.ErrorSendRellatedData, "Pasukan Trans");// "Pasukan Trans Tidak Dihantar";
                    return result;
                }
#endif
            }

            //Images
            var allImagesSend = true;
            var listOfImage = GeneralBll.GetListPatPhotoNameByRujukan(noRujukan);
            var list = DataAccessQuery<TbSendOnlineGambar>.GetAll();
            var imagesSended = new List<string>();
            if (list.Success)
            {
                imagesSended = list.Datas.Where(mbox => mbox.NoRujukan == noRujukan && mbox.Status == Enums.StatusOnline.Sent).Select(m => m.Name).ToList();
            }

            foreach (var path in listOfImage)
            {
                if (imagesSended.Any(m => m == Path.GetFileName(path)))
                    continue;

                var imageBase64 = GeneralBll.GetBase64FromImagePath(path);

                var tarikhDafterInt = GeneralBll.StringDatetimeToInt(tbKpp.TrkhDaftar);
                var tarikhAkhirInt = GeneralBll.StringDatetimeToInt(tbKpp.TrkhAkhir);

                var options = new
                {
                    nokmp = noRujukan,
                    kodcawangan = tbKpp.KodCawangan,
                    namagambar = Path.GetFileName(path),
                    status = tbKpp.Status,
                    pgndaftar = tbKpp.PgnDaftar,
                    trkhdaftar = tarikhDafterInt,
                    pgnakhir = tbKpp.PgnAkhir,
                    trkhakhir = tarikhAkhirInt,
                    images = $"data:image/jpeg;base64,{imageBase64}",
                    kategori = 2
                };
                var jsonParam = JsonConvert.SerializeObject(options);

                var response = await HttpClientService.UploadImage(jsonParam);
                SendOnlineBll.SetStatusImagesDataOnline(noRujukan, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, Path.GetFileName(path));
                if (!response.Success)
                {
                    allImagesSend = false;
                }
            }
            if (!allImagesSend)
            {
                result.Success = false;
                result.Message = string.Format(Constants.ErrorMessages.ErrorSendRellatedData, "Gambar");//"Gambar Tidak Dihantar";
                return result;
            }

            //Asas Tindakan
            var allAsasTindakanSend = true;
            var asasTindakan = DataAccessQuery<TbKppAsasTindakan>.GetAll(m => m.NoRujukanKpp == noRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
            if (asasTindakan.Success)
            {
                var query = "";
                var tbKppAsasTindakanAfterSplit = asasTindakan.Datas.Split();
                foreach (var asasTindakans in tbKppAsasTindakanAfterSplit)
                {
                    query = string.Empty;
                    foreach (var item in asasTindakans)
                    {
                        query = SendOnlineBll.GenerateScriptAsasTindakan(item, query);
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        if (query.EndsWith(","))
                            query = query.Remove(query.Length - 1) + ";";

                        var response = await HttpClientService.ExecuteQuery(query, context);
                        var statusOnline = response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                        foreach (var item in asasTindakans)
                        {
                            DataAccessQuery<TbKppAsasTindakan>.ExecuteSql($"UPDATE tbkpp_asastindakan SET IsSendOnline = '{(int)statusOnline}' " +
                                                                          $"WHERE NoRujukanKpp = '{item.NoRujukanKpp}' AND KodTujuan = '{item.KodTujuan}' AND KodAsas = '{item.KodAsas}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                        }

                        if (!response.Success)
                        {
                            allAsasTindakanSend = false;
                        }

                        if (!allAsasTindakanSend)
                        {
                            result.Success = false;
                            result.Message = "Asas Tindakan Tidak Dihantar";
                            return result;
                        }
                    }
                }
            }

            return result;

        }

        public static bool SavePemeriksaanTrx(TbKpp input, List<AsasTindakanDto> listAsas, List<LokalitiKategoriKhasDto> kategoriKhas, List<AgensiSerahanDto> agensiTerlibat)
        {
            var insAccess = new DataAccessQueryTrx();
            if (insAccess.BeginTrx())
            {
                if (!HandheldBll.CreateOrUpdateHandheldDataTrx(Enums.PrefixType.KPP, input.NoRujukanKpp, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                input.LongitudPremis = GeneralBll.GetLastSaveLongitude();
                input.LatitudPremis = GeneralBll.GetLastSaveLatitude();


                var listPhotos = GeneralBll.GetPhotoNameByRujukan(input.NoRujukanKpp);
                if (listPhotos.Count > 0)
                {
                    input.GambarLawatan1 = listPhotos[0];
                    if (listPhotos.Count > 1)
                    {
                        input.GambarLawatan2 = listPhotos[1];
                    }
                }
                if (!insAccess.InsertTrx(input))
                {
                    insAccess.RollBackTrx();
                    return false;
                }


                if (!SavePasukanTransTrx(Constants.JenisTrans.Kpp, input.NoRujukanKpp, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SendOnlineBll.InserDataOnline(input.NoRujukanKpp, Enums.TableType.KPP, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SendOnlineBll.InserDataOnline(input.NoRujukanKpp, Enums.TableType.KPP_HH, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SendOnlineBll.InserImagesDataOnline(input.NoRujukanKpp, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SaveKppAsasTindakan(input.NoRujukanKpp, input.KodTujuan, listAsas, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SaveKppLokalitiKategoriKhas(input.NoRujukanKpp, input.lokalitikategorikhas, kategoriKhas, insAccess))
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                if (!SaveKppAgensiTerlibat(input.NoRujukanKpp, input.kodagensiterlibat, agensiTerlibat, insAccess)) 
                {
                    insAccess.RollBackTrx();
                    return false;
                }

                return insAccess.CommitTrx();

            }
            return false;
        }



        //private static bool InsertPemeriksaanTrx(TbKpp rujukan, DataAccessQueryTrx insAccess)
        //{
        //    var query = " INSERT INTO tbkpp  " +
        //                " (norujukankpp, idhh, kodcawangan, kodtujuan, kodasas, " +
        //                " kodjenis, catatanlawatan, kodkatkawasan, norujukanatr, nosiriborangkpp," +
        //                " trkhmulalawatankpp, trkhtamatlawatankpp, kodkatpremis, namapremis, alamatpremis1," +
        //                " alamatpremis2, alamatpremis3, nodaftarpremis, nolesenBKP_PDA, nolesenmajlis_permit," +
        //                " notelefonpremis, catatanpremis, pengeluarkpp, longitudpremis, latitudpremis," +
        //                " amaran, lokasilawatan, noaduan, hasillawatan, tindakan," +
        //                " namapenerima, nokppenerima, jawatanpenerima, alamatpenerima1, alamatpenerima2," +
        //                " alamatpenerima3, trkhpenerima, setujubyr, status, pgndaftar," +
        //                " trkhdaftar, pgnakhir, trkhakhir," +
        //                " jenispesalah, kodakta, kodsalah, butirsalah, isarahansemasa, tempohtawaran, amnkmp," +
        //                " noep, noip)";
        //    query += " VALUES" +
        //           $" ('{rujukan.NoRujukanKpp}', '{rujukan.IdHh}', '{rujukan.KodCawangan}', '{rujukan.KodTujuan}', '{rujukan.KodAsas}', " +
        //           $" '{rujukan.KodJenis}', '{rujukan.CatatanLawatan.ReplaceSingleQuote()}', '{rujukan.KodKatKawasan}', " +
        //           $"'{rujukan.NoRujukanAtr.ReplaceSingleQuote()}', '{rujukan.NoSiriBorangKpp.ReplaceSingleQuote()}', " +
        //           $" '{rujukan.TrkhMulaLawatankpp}', {rujukan.TrkhTamatLawatanKpp}, '{rujukan.KodKatPremis}', " +
        //           $"'{rujukan.NamaPremis.ReplaceSingleQuote()}', '{rujukan.AlamatPremis1.ReplaceSingleQuote()}', " +
        //           $" '{rujukan.AlamatPremis2.ReplaceSingleQuote()}', '{rujukan.AlamatPremis3.ReplaceSingleQuote()}'," +
        //           $" '{rujukan.NoDaftarPremis.ReplaceSingleQuote()}', '{rujukan.NoLesenBKP_PDA.ReplaceSingleQuote()}', " +
        //           $"'{rujukan.NoLesenMajlis_Permit.ReplaceSingleQuote()}', " +
        //           $" '{rujukan.NoTelefonPremis}', '{rujukan.CatatanPremis.ReplaceSingleQuote()}', '{rujukan.PengeluarKpp}'," +
        //           $" '{rujukan.LongitudPremis}', '{rujukan.LatitudPremis}', " +
        //           $" '{rujukan.Amaran}', '{rujukan.LokasiLawatan.ReplaceSingleQuote()}'," +
        //           $" '{rujukan.NoAduan.ReplaceSingleQuote()}', '{rujukan.HasilLawatan.ReplaceSingleQuote()}', '{rujukan.Tindakan}', " +
        //           $" '{rujukan.NamaPenerima.ReplaceSingleQuote()}', '{rujukan.NoKpPenerima.ReplaceSingleQuote()}', " +
        //           $"'{rujukan.Jawatanpenerima.ReplaceSingleQuote()}', '{rujukan.AlamatPenerima1.ReplaceSingleQuote()}'," +
        //           $" '{rujukan.AlamatPenerima2.ReplaceSingleQuote()}', " +
        //           $" '{rujukan.AlamatPenerima3.ReplaceSingleQuote()}', '{rujukan.TrkhPenerima}', '{rujukan.SetujuByr}'," +
        //           $" '{rujukan.Status}', '{rujukan.PgnDaftar}', " +
        //           $"  '{rujukan.TrkhDaftar}', {rujukan.PgnAkhir}, '{rujukan.TrkhAkhir}'," +
        //           $" '{rujukan.JenisPesalah}', '{rujukan.KodAkta}','{rujukan.KodSalah}', '{rujukan.ButirSalah}', " +
        //           $"'{rujukan.IsArahanSemasa}', '{rujukan.TempohTawaran}', '{rujukan.AmnKmp}'," +
        //           $" '{rujukan.NoEp}','{rujukan.NoIp}'); ";

        //    return insAccess.ExecuteQueryTrx(query);
        //}

        //private static bool InsertPasukanTranstrx(int jenisTrans, string noRujukan, DataAccessQueryTrx insAccess)
        //{
        //    var listData = DataAccessQuery<TbPasukanHh>.GetAll();
        //    if (listData.Success)
        //    {
        //        var listPasukan = listData.Datas;

        //        var userIdStaf = GeneralBll.GetUserStaffId();

        //        foreach (var tbPasukanDetail in listPasukan)
        //        {
        //            var tbPasukanTrans = new TbPasukanTrans
        //            {
        //                JenisTrans = jenisTrans,
        //                NoRujukan = noRujukan,
        //                KodPasukan = tbPasukanDetail.KodPasukan,
        //                Id = tbPasukanDetail.Id,
        //                Status = tbPasukanDetail.Status,
        //                PgnDaftar = userIdStaf,
        //                PgnAkhir = userIdStaf,
        //                IsSendOnline = Enums.StatusOnline.New,
        //                Catatan = tbPasukanDetail.Catatan
        //            };

        //            var data = DataAccessQuery<TbPasukanTrans>.Insert(tbPasukanTrans);
        //            if (!data.Success) return false;
        //        }

        //        return true;
        //    }


        //    return false;

        //}

        private static bool SavePasukanTransTrx(int jenisTrans, string noRujukan, DataAccessQueryTrx insAccess)
        {
            var listData = DataAccessQuery<TbPasukanHh>.GetAll();
            if (listData.Success)
            {
                var listPasukan = listData.Datas;

                var userIdStaf = GeneralBll.GetUserStaffId();

                foreach (var tbPasukanDetail in listPasukan)
                {
                    var tbPasukanTrans = new TbPasukanTrans
                    {
                        JenisTrans = jenisTrans,
                        NoRujukan = noRujukan,
                        KodPasukan = tbPasukanDetail.KodPasukan,
                        Id = tbPasukanDetail.Id,
                        Status = tbPasukanDetail.Status,
                        PgnDaftar = userIdStaf,
                        PgnAkhir = userIdStaf,
                        IsSendOnline = Enums.StatusOnline.New,
                        Catatan = tbPasukanDetail.Catatan
                    };

                    if (!insAccess.InsertTrx(tbPasukanTrans))
                        return false;

                }

                return true;
            }


            return false;

        }

        public static Response<List<DataSsmDto>> GetListSsm(string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return new Response<List<DataSsmDto>> { Success = true, Result = new List<DataSsmDto>() };
            }
            var result = new Response<List<DataSsmDto>>();
            result.Result = new List<DataSsmDto>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<List<DataSsmDto>> { Success = false, Mesage = Constants.ErrorMessages.NoInternetConnection };
            }

            //for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
            //{
            var resultData = Task.Run(async () => await HttpClientService.GetListSsm(searchValue)).Result;

            if (resultData.Success)
            {
                int number = 1;
                foreach (var resultDataDetail in resultData.Result.DataDetails)
                {
                    var data = new DataSsmDto
                    {
                        Number = number,
                        NoSyarikat = resultDataDetail.NoSyarikat,
                        NamaSyarikat = resultDataDetail.NamaSyarikat,
                        AlamatNiaga1 = resultDataDetail.AlamatNiaga1,
                        AlamatNiaga2 = resultDataDetail.AlamatNiaga2,
                        AlamatNiaga3 = resultDataDetail.AlamatNiaga3
                    };
                    result.Result.Add(data);
                    number++;
                }
                result.Success = true;
                return result;
            }
            else
            {
                result.Success = false;
                result.Mesage = resultData.Mesage;
            }
            //Thread.Sleep(Constants.SleepRetryActiveParking);
            //}

            return result;
        }


        public static Response<JpnDetailResponse> GetListJpnDetail(string searchValue, string noKpFOrUser)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return new Response<JpnDetailResponse> { Success = true, Result = new JpnDetailResponse() };
            }
            var result = new Response<List<DataSsmDto>>();
            result.Result = new List<DataSsmDto>();
            if (!GeneralAndroidClass.IsOnline())
            {
                return new Response<JpnDetailResponse> { Success = false, Mesage = Constants.ErrorMessages.NoInternetConnection };
            }

            //for (int i = 1; i <= Constants.MaxCallAPIRetry; i++)
            //{
            var resultData = Task.Run(async () => await HttpClientService.GetListJpnDetail(searchValue, noKpFOrUser)).Result;

            if (resultData.Success && resultData.Result == null)
            {
                resultData.Success = false;
                resultData.Mesage = string.Format(Constants.ErrorMessages.NoDataFoundJpnDetail, searchValue);
            }
            return resultData;

        }

        public static Dictionary<string, string> GetKewarganegaraan(bool isAddDefault = true)
        {
            var result = new Dictionary<string, string>();
            if (isAddDefault)
            {
                result.Add("1", Constants.KewarganegaraanName.Warganegara);
            }
            result.Add("2", Constants.KewarganegaraanName.BukanWarganegara);

            return result;
        }
    }

}