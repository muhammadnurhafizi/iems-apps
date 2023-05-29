using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using Newtonsoft.Json;
using static Android.Content.ClipData;

namespace IEMSApps.BLL
{
    public static class SendOnlineBll
    {
        public static bool InserDataOnline(string noRujukan, Enums.TableType type, DataAccessQueryTrx insAccess = null)
        {
            var data = new TbSendOnlineData
            {
                Type = type,
                NoRujukan = noRujukan,
                Status = Enums.StatusOnline.New,
                CreatedDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat)
            };

            if (insAccess != null)
            {
                return insAccess.InsertTrx(data);
            }

            var result = DataAccessQuery<TbSendOnlineData>.Insert(data);

            return result.Success;
        }

        public static bool InserImagesDataOnline(string noRujukan, DataAccessQueryTrx insAccess = null)
        {
            var listPhotos = GeneralBll.GetPhotoNameByRujukan(noRujukan);
            foreach (var photo in listPhotos)
            {
                var data = new TbSendOnlineGambar
                {
                    Name = photo,
                    NoRujukan = noRujukan,
                    Status = Enums.StatusOnline.New,
                    CreatedDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat)
                };

                if (insAccess != null)
                {
                    if (!insAccess.InsertTrx(data))
                    {
                        return false;
                    }
                }
                else
                {
                    DataAccessQuery<TbSendOnlineGambar>.Insert(data);
                }

            }

            return true;
        }

        public static bool InsertImagesDataOnlineReceipt(string noRujukan, DataAccessQueryTrx insAccess = null)
        {
            var listPhotos = GeneralBll.GetReceiptPhotoNameByRujukan(noRujukan);
            foreach (var photo in listPhotos)
            {
                var data = new TbSendOnlineGambar
                {
                    Name = photo,
                    NoRujukan = noRujukan,
                    Status = Enums.StatusOnline.New,
                    CreatedDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat)
                };

                if (insAccess != null)
                {
                    if (!insAccess.InsertTrx(data))
                    {
                        return false;
                    }
                }
                else
                {
                    DataAccessQuery<TbSendOnlineGambar>.Insert(data);
                }

            }

            return true;
        }

        public static string InsertImagesReceiptNameOnKPP(string noRujukan)
        {
            var listPhotos = GeneralBll.GetReceiptPhotoNameByRujukan(noRujukan);
            foreach (var photo in listPhotos)
            {
                return photo;
            }

            return null;
        }

        public static void SetStatusDataOnline(string noRujukan, Enums.TableType type, Enums.StatusOnline status)
        {
            var data = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == type);
            if (data.Success)
            {
                if (data.Datas != null)
                {
                    var entity = data.Datas;
                    entity.Status = status;
                    entity.UpdateDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat);

                    DataAccessQuery<TbSendOnlineData>.Update(entity);
                }
            }
        }

        public static void SetStatusImagesDataOnline(string noRujukan, Enums.StatusOnline status, string fileName)
        {
            var data = DataAccessQuery<TbSendOnlineGambar>.Get(m => m.NoRujukan == noRujukan && m.Name == fileName && m.Status != Enums.StatusOnline.Sent);
            if (data.Success)
            {
                if (data.Datas != null)
                {
                    var entity = data.Datas;
                    entity.Status = status;
                    entity.UpdateDate = GeneralBll.GetLocalDateTimeForDatabase();

                    DataAccessQuery<TbSendOnlineGambar>.Update(entity);
                }
            }
        }

        public static void SetStatusOnlinePasukanTrans(int id, Enums.StatusOnline statusOnline, string noRujukan)
        {
            DataAccessQuery<TbPasukanTrans>.ExecuteSql($"UPDATE tbpasukan_trans SET IsSendOnline = '{(int)statusOnline}' WHERE id = '{id}' AND NoRujukan = '{noRujukan}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noRujukan"></param>
        /// <param name="type"></param>
        public static async Task<Response<string>> SendDataOnlineAsync(string noRujukan, Enums.TableType type, Android.Content.Context context)
        {
#if !DEBUG
            if (!GeneralAndroidClass.IsOnline())
                return new Response<string> { Success = false, Mesage = "Tiada Sambungan Internet" };
#endif
            var allDatasIsSended = true;
            var errorMessage = "";

            var datas = DataAccessQuery<TbSendOnlineData>.GetAll();
            if (datas.Success)
            {
                datas.Datas = datas.Datas.Where(m => m.NoRujukan == noRujukan).ToList();
                if (type == Enums.TableType.KPP)
                {
                    foreach (var item in datas.Datas.Where(m => m.Type == Enums.TableType.KPP || m.Type == Enums.TableType.KPP_HH))
                    {
                        //if (item.Status == Enums.StatusOnline.Sent) continue;
                        var respose = await SendKppOnlineAsyncV2(noRujukan, item.Type, context);
                        if (!respose.Success)
                        {
                            allDatasIsSended = false;
                            errorMessage = respose.Mesage;
                        }

                    }
                    return new Response<string>() { Success = allDatasIsSended, Mesage = errorMessage };
                }
                else if (type == Enums.TableType.Kompaun)
                {
                    foreach (var item in datas.Datas.Where(m => m.Type == Enums.TableType.Kompaun || m.Type == Enums.TableType.Kompaun_HH))
                    {
                        //if (item.Status == Enums.StatusOnline.Sent) continue;
                        var respose = await SendKompaunOnlineAsyncV2(noRujukan, item.Type, context);
                        if (!respose.Success)
                        {
                            allDatasIsSended = false;
                            errorMessage = respose.Mesage;
                        }
                    }

                    var response = await CheckDataKesBasedNoRujukan(noRujukan, context);
                    if (!response.Success)
                    {
                        allDatasIsSended = false;
                        errorMessage = response.Mesage;
                    }

                    return new Response<string>() { Success = allDatasIsSended, Mesage = errorMessage };
                }
                else if (type == Enums.TableType.DataKes)
                {
                    foreach (var item in datas.Datas.Where(m => m.Type == Enums.TableType.DataKes || m.Type == Enums.TableType.DataKes_HH))
                    {
                        //if (item.Status == Enums.StatusOnline.Sent) continue;
                        var respose = await SendDataKesOnlineAsyncV2(noRujukan, item.Type, context);
                        if (!respose.Success)
                        {
                            allDatasIsSended = false;
                            errorMessage = respose.Mesage;
                        }
                    }
                    return new Response<string>() { Success = allDatasIsSended, Mesage = errorMessage };
                }
                else if (type == Enums.TableType.Akuan_UpdateKompaun || type == Enums.TableType.Akuan_UpdateKompaun_HH)
                {
                    foreach (var item in datas.Datas.Where(m => m.Type == Enums.TableType.Akuan_UpdateKompaun || m.Type == Enums.TableType.Akuan_UpdateKompaun_HH || m.Type == Enums.TableType.KompaunBayaran))
                    {
                        //if (item.Status == Enums.StatusOnline.Sent) continue;
                        var respose = await SendAkuanAsync(noRujukan, item.Type, context);
                        if (!respose.Success)
                        {
                            allDatasIsSended = false;
                            errorMessage = respose.Mesage;
                        }
                    }
                    return new Response<string>() { Success = allDatasIsSended, Mesage = errorMessage };
                }

            }
            return new Response<string>() { Success = false, Mesage = "Ralat" };
        }

        #region SendKompaun        

        private static string GenerateSQLScriptForTableKompaun_Hh(TbKompaun tbKompaun)
        {
            var nokmp = tbKompaun.NoKmp;
            var tahun = DateTime.Now.ToString("yy");
            if (!string.IsNullOrEmpty(tbKompaun.TrkhDaftar))
                tahun = tbKompaun.TrkhDaftar.Substring(2, 2);

            var trkhKmp = GeneralBll.ConvertDatabaseFormatStringToDateTime(tbKompaun.TrkhKmp).ToString(Constants.DatabaseDateFormat);
            var trkhSalah = GeneralBll.ConvertDatabaseFormatStringToDateTime(tbKompaun.TrkhSalah).ToString(Constants.DatabaseDateFormat);
            var trkhPenerima = GeneralBll.ConvertDatabaseFormatStringToDateTime(tbKompaun.TrkhPenerima).ToString(Constants.DatabaseDateFormat);

            return " INSERT INTO tbkompaun_hh " +
                " (nokmp, norujukankmp, idhh, jeniskmp, kodcawangan, kodkatpremis, jenispesalah, namaokk, nokpokk, noep, noip, " +
                " namapremis, nodaftarpremis, alamatokk1, alamatokk2, alamatokk3, trkhkmp, nolaporanpolis, nolaporancwgn, trkhsalah, " +
                " tempatsalah, kodakta, kodsalah, butirsalah, isarahansemasa, tempohtawaran, amnkmp, amnbyr, noresit, pegawaipengeluar, " +
                " norujukankpp, namapenerima, nokppenerima, alamatpenerima1, alamatpenerima2, alamatpenerima3, trkhpenerima, iscetakakuan, " +
                " namapenerima_akuan, nokppenerima1_akuan, alamatpenerima1_akuan, alamatpenerima2_akuan, alamatpenerima3_akuan, trkhpenerima_akuan, " +
                " ishh, tahun, status, pgndaftar, trkhdaftar, pgnakhir, trkhakhir, barangkompaun, " +
                " ip_identiti_pelanggan_id_penerima, poskodpenerima, bandarpenerima, negeripenerima, negarapenerima, notelpenerima, emelpenerima, " +
                " poskodpenerima_akuan, bandarpenerima_akuan, negeripenerima_akuan, negarapenerima_akuan, notelpenerima_akuan, emelpenerima_akuan, isbayarmanual, gambarbuktibayaran, ip_identiti_pelanggan_id_akuan)" +
                " VALUES " +
                $"('{nokmp}', '', '{tbKompaun.IdHh}', '{tbKompaun.JenisKmp}', '{tbKompaun.KodCawangan}', '{tbKompaun.KodKatPremis}', '{tbKompaun.JenisPesalah}', '{tbKompaun.NamaOkk.ReplaceSingleQuote()}', '{tbKompaun.NoKpOkk.ReplaceSingleQuote()}','{tbKompaun.NoEp}','{tbKompaun.NoIp}', " +
                $" '{tbKompaun.NamaPremis.ReplaceSingleQuote()}', '{tbKompaun.NoDaftarPremis.ReplaceSingleQuote()}', '{tbKompaun.AlamatOkk1.ReplaceSingleQuote()}', '{tbKompaun.AlamatOkk2.ReplaceSingleQuote()}', '{tbKompaun.AlamatOkk3.ReplaceSingleQuote()}', '{trkhKmp}', '{tbKompaun.NoLaporanPolis.ReplaceSingleQuote()}', '{tbKompaun.NoLaporanCwgn.ReplaceSingleQuote()}', '{trkhSalah}', " +
                $" '{tbKompaun.TempatSalah.ReplaceSingleQuote()}', '{tbKompaun.KodAkta}', '{tbKompaun.KodSalah}', '{tbKompaun.ButirSalah.ReplaceSingleQuote()}', '{tbKompaun.IsArahanSemasa}', '{tbKompaun.TempohTawaran}', '{tbKompaun.AmnKmp}', '{tbKompaun.AmnByr}', '{tbKompaun.NoResit}', '{tbKompaun.PegawaiPengeluar}', " +
                $" '{tbKompaun.NoRujukanKpp}', '{tbKompaun.NamaPenerima.ReplaceSingleQuote()}', '{tbKompaun.NoKpPenerima}', '{tbKompaun.AlamatPenerima1.ReplaceSingleQuote()}', '{tbKompaun.AlamatPenerima2.ReplaceSingleQuote()}', '{tbKompaun.AlamatPenerima3.ReplaceSingleQuote()}', '{trkhPenerima}', '{tbKompaun.IsCetakAkuan}', " +
                $" '{tbKompaun.NamaPenerima_Akuan.ReplaceSingleQuote()}', '{tbKompaun.NoKpPenerima_Akuan.ReplaceSingleQuote()}', '{tbKompaun.AlamatPenerima1_Akuan.ReplaceSingleQuote()}', '{tbKompaun.AlamatPenerima2_Akuan.ReplaceSingleQuote()}', '{tbKompaun.AlamatPenerima3_Akuan.ReplaceSingleQuote()}', NULL, " +
                $" '0', '{tahun}', '{tbKompaun.Status}', '{tbKompaun.PgnDaftar}', UNIX_TIMESTAMP('{tbKompaun.TrkhDaftar}'), '{tbKompaun.PgnAkhir}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), '{tbKompaun.BarangKompaun.ReplaceSingleQuote()}', " +
                $" '{tbKompaun.ip_identiti_pelanggan_id}', '{tbKompaun.poskodpenerima}', '{tbKompaun.bandarpenerima}', '{tbKompaun.negeripenerima}', '{tbKompaun.negarapenerima}', '{tbKompaun.notelpenerima}', '{tbKompaun.emelpenerima}', " +
                $" '{tbKompaun.poskodpenerima_akuan}', '{tbKompaun.bandarpenerima_akuan}', '{tbKompaun.negeripenerima_akuan}', '{tbKompaun.negarapenerima_akuan}', '{tbKompaun.notelpenerima_akuan}', '{tbKompaun.emelpenerima_akuan}', '{tbKompaun.isbayarmanual}', '{tbKompaun.gambarbuktibayaran}', '{tbKompaun.ip_identiti_pelanggan_id_akuan}' )";
        }

        private static string GenerateSQLScriptForTableKompaun(TbKompaun tbKompaun)
        {
            var nokmp = tbKompaun.NoKmp;
            var trkhKmp = GeneralBll.ConvertDatabaseFormatStringToDateTime(tbKompaun.TrkhKmp).ToString(Constants.DatabaseDateFormat);
            var trkhSalah = GeneralBll.ConvertDatabaseFormatStringToDateTime(tbKompaun.TrkhSalah).ToString(Constants.DatabaseDateFormat);
            var trkhPenerima = GeneralBll.ConvertDatabaseFormatStringToDateTime(tbKompaun.TrkhPenerima).ToString(Constants.DatabaseDateFormat);

            return " INSERT INTO tbkompaun " +
                " (nokmp, norujukankmp, idhh, jeniskmp, kodcawangan, kodkatpremis, jenispesalah, namaokk, nokpokk, namapremis, noep, noip, " +
                " nodaftarpremis, alamatokk1, alamatokk2, alamatokk3, trkhkmp, nolaporanpolis, nolaporancwgn, trkhsalah, tempatsalah, " +
                " kodakta, kodsalah, butirsalah, isarahansemasa, tempohtawaran, amnkmp, amnbyr, noresit, pegawaipengeluar, norujukankpp, " +
                " namapenerima, nokppenerima, alamatpenerima1, alamatpenerima2, alamatpenerima3, trkhpenerima, iscetakakuan, namapenerima_akuan, " +
                " nokppenerima1_akuan, alamatpenerima1_akuan, alamatpenerima2_akuan, alamatpenerima3_akuan, trkhpenerima_akuan,ishh, " +
                " status, pgndaftar, trkhdaftar, pgnakhir, trkhakhir, barangkompaun, " +
                " ip_identiti_pelanggan_id_penerima, poskodpenerima, bandarpenerima, negeripenerima, negarapenerima, notelpenerima, emelpenerima, " +
                " poskodpenerima_akuan, bandarpenerima_akuan, negeripenerima_akuan, negarapenerima_akuan, notelpenerima_akuan, emelpenerima_akuan, isbayarmanual, gambarbuktibayaran, ip_identiti_pelanggan_id_akuan)" +
                " VALUES " +
                $" ('{nokmp}' ,'' ,'{tbKompaun.IdHh}' ,'{tbKompaun.JenisKmp}' ,'{tbKompaun.KodCawangan}' ,'{tbKompaun.KodKatPremis}' ,'{tbKompaun.JenisPesalah}' ,'{tbKompaun.NamaOkk.ReplaceSingleQuote()}' ,'{tbKompaun.NoKpOkk}' ,'{tbKompaun.NamaPremis.ReplaceSingleQuote()}', '{tbKompaun.NoEp}','{tbKompaun.NoIp}'," +
                $" '{tbKompaun.NoDaftarPremis.ReplaceSingleQuote()}' ,'{tbKompaun.AlamatOkk1.ReplaceSingleQuote()}' ,'{tbKompaun.AlamatOkk2.ReplaceSingleQuote()}' ,'{tbKompaun.AlamatOkk3.ReplaceSingleQuote()}' , '{trkhKmp}' ,'{tbKompaun.NoLaporanPolis.ReplaceSingleQuote()}' ,'{tbKompaun.NoLaporanCwgn}' , '{trkhSalah}' ,'{tbKompaun.TempatSalah.ReplaceSingleQuote()}', " +
                $" '{tbKompaun.KodAkta}' ,'{tbKompaun.KodSalah}' ,'{tbKompaun.ButirSalah.ReplaceSingleQuote()}' ,'{tbKompaun.IsArahanSemasa}' ,'{tbKompaun.TempohTawaran}' ,'{tbKompaun.AmnKmp}' ,'{tbKompaun.AmnByr}' ,'{tbKompaun.NoResit.ReplaceSingleQuote()}' ,'{tbKompaun.PegawaiPengeluar}' ,'{tbKompaun.NoRujukanKpp}', " +
                $" '{tbKompaun.NamaPenerima.ReplaceSingleQuote()}' ,'{tbKompaun.NoKpPenerima}' ,'{tbKompaun.AlamatPenerima1.ReplaceSingleQuote()}' ,'{tbKompaun.AlamatPenerima2.ReplaceSingleQuote()}' ,'{tbKompaun.AlamatPenerima3.ReplaceSingleQuote()}' , '{trkhPenerima}' ,'{tbKompaun.IsCetakAkuan}' ,'{tbKompaun.NamaPenerima_Akuan.ReplaceSingleQuote()}', " +
                $" '{tbKompaun.NoKpPenerima_Akuan.ReplaceSingleQuote()}' ,'{tbKompaun.AlamatPenerima1_Akuan.ReplaceSingleQuote()}' ,'{tbKompaun.AlamatPenerima2_Akuan.ReplaceSingleQuote()}' ,'{tbKompaun.AlamatPenerima3_Akuan.ReplaceSingleQuote()}' , NULL ,'0', " +
                $" '{tbKompaun.Status}' ,'{tbKompaun.PgnDaftar}' ,UNIX_TIMESTAMP('{tbKompaun.TrkhDaftar}') ,'{tbKompaun.PgnAkhir}' ,UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), '{tbKompaun.BarangKompaun.ReplaceSingleQuote()}', " +
                $" '{tbKompaun.ip_identiti_pelanggan_id}', '{tbKompaun.poskodpenerima}', '{tbKompaun.bandarpenerima}', '{tbKompaun.negeripenerima}', '{tbKompaun.negarapenerima}', '{tbKompaun.notelpenerima}', '{tbKompaun.emelpenerima}', " +
                $" '{tbKompaun.poskodpenerima_akuan}', '{tbKompaun.bandarpenerima_akuan}', '{tbKompaun.negeripenerima_akuan}', '{tbKompaun.negarapenerima_akuan}', '{tbKompaun.notelpenerima_akuan}', '{tbKompaun.emelpenerima_akuan}', '{tbKompaun.isbayarmanual}', '{tbKompaun.gambarbuktibayaran}', '{tbKompaun.ip_identiti_pelanggan_id_akuan}' )";
        }

        //private static async Task<Response<string>> SendKompaunOnline(string noRujukan)
        //{
        //    var response = new Response<string>() { Success = false, Mesage = "Ralat" };

        //    var result = KompaunBll.GetKompaunByRujukan(noRujukan);
        //    if (result.Success)
        //    {
        //        if (result.Datas != null)
        //        {
        //            response = await HttpClientService.ExecuteQuery(GenerateSQLScriptForTableKompaun_Hh(result.Datas));
        //            if (response.Success)
        //            {
        //                response = await HttpClientService.ExecuteQuery(GenerateSQLScriptForTableKompaun(result.Datas));
        //                if (response.Success)
        //                {
        //                    SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun, Enums.StatusOnline.Sent);
        //                }
        //            }

        //            await SendImageOnline(noRujukan, result.Datas.KodCawangan, result.Datas.Status, result.Datas.PgnDaftar.ToString(), result.Datas.TrkhDaftar, result.Datas.PgnAkhir.ToString(), result.Datas.TrkhAkhir, 3);

        //            if (result.Datas.IsCetakAkuan == Constants.CetakAkuan.Yes)
        //            {
        //                var trkhBayar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
        //                if (!string.IsNullOrEmpty(result.Datas.TrkhPenerima_Akuan))
        //                    trkhBayar = result.Datas.TrkhPenerima_Akuan;

        //                var sqlQuery = " insert into tbkompaun_bayaran (kodcawangan,nokmp,trkhbyr,amnbyr,noresit,status,pgndaftar, " +
        //                               " trkhdaftar,pgnakhir,trkhakhir) VALUES " +
        //                               $" ('{result.Datas.KodCawangan}', '{result.Datas.NoKmp}', '{trkhBayar}','{result.Datas.AmnByr}','{result.Datas.NoResit}', '{result.Datas.Status}', '{result.Datas.PgnDaftar}'," +
        //                               $" UNIX_TIMESTAMP('{result.Datas.TrkhDaftar}'), '{result.Datas.PgnDaftar}', UNIX_TIMESTAMP('{result.Datas.TrkhAkhir}')) ";
        //                response = await HttpClientService.ExecuteQuery(sqlQuery);
        //                if (response.Success)
        //                {
        //                    SetStatusDataOnline(noRujukan, Enums.TableType.KompaunBayaran, Enums.StatusOnline.Sent);
        //                }
        //            }
        //        }
        //    }

        //    return response;
        //}

        #endregion

        #region SendKPP        

        public static string GenerateSQLScriptForTableKpp(TbKpp rujukan)
        {
            string TrkhTamatLawatanKpp = string.IsNullOrEmpty(rujukan.TrkhTamatLawatanKpp) ? "NULL" : $"'{rujukan.TrkhTamatLawatanKpp}'";
            return " INSERT INTO tbkpp  " +
                   " (norujukankpp, idhh, kodcawangan, kodtujuan, kodasas, " +
                   " kodjenis, catatanlawatan, kodkatkawasan, norujukanatr, nosiriborangkpp," +
                   " trkhmulalawatankpp, trkhtamatlawatankpp, kodkatpremis, namapremis, alamatpremis1," +
                   " alamatpremis2, alamatpremis3, nodaftarpremis, nolesenBKP_PDA, nolesenmajlis_permit," +
                   " notelefonpremis, catatanpremis, pengeluarkpp, longitudpremis, latitudpremis," +
                   " amaran, lokasilawatan, noaduan, hasillawatan, tindakan," +
                   " namapenerima, nokppenerima, jawatanpenerima, alamatpenerima1, alamatpenerima2," +
                   " alamatpenerima3, trkhpenerima, setujubyr, status, pgndaftar," +
                   " trkhdaftar, pgnakhir, trkhakhir, ishh," +
                   " jenispesalah, kodakta, kodsalah, butirsalah, isarahansemasa, tempohtawaran, amnkmp," +
                   " noep, noip," +
                   " ip_identiti_pelanggan_id, poskodpenerima, bandarpenerima, negeripenerima, negarapenerima, notelpenerima, emelpenerima )" +
                   " VALUES" +
                   $" ('{rujukan.NoRujukanKpp}', '{rujukan.IdHh}', '{rujukan.KodCawangan}', '{rujukan.KodTujuan}', '{rujukan.KodAsas}', " +
                   $" '{rujukan.KodJenis}', '{rujukan.CatatanLawatan.ReplaceSingleQuote()}', '{rujukan.KodKatKawasan}', '{rujukan.NoRujukanAtr.ReplaceSingleQuote()}', '{rujukan.NoSiriBorangKpp.ReplaceSingleQuote()}', " +
                   $" '{rujukan.TrkhMulaLawatankpp}', {TrkhTamatLawatanKpp}, '{rujukan.KodKatPremis}', '{rujukan.NamaPremis.ReplaceSingleQuote()}', '{rujukan.AlamatPremis1.ReplaceSingleQuote()}', " +
                   $" '{rujukan.AlamatPremis2.ReplaceSingleQuote()}', '{rujukan.AlamatPremis3.ReplaceSingleQuote()}', '{rujukan.NoDaftarPremis.ReplaceSingleQuote()}', '{rujukan.NoLesenBKP_PDA.ReplaceSingleQuote()}', '{rujukan.NoLesenMajlis_Permit.ReplaceSingleQuote()}', " +
                   $" '{rujukan.NoTelefonPremis.ReplaceSingleQuote()}', '{rujukan.CatatanPremis.ReplaceSingleQuote()}', '{rujukan.PengeluarKpp}', '{rujukan.LongitudPremis}', '{rujukan.LatitudPremis}', " +
                   $" '{rujukan.Amaran}', '{rujukan.LokasiLawatan.ReplaceSingleQuote()}', '{rujukan.NoAduan.ReplaceSingleQuote()}', '{rujukan.HasilLawatan.ReplaceSingleQuote()}', '{rujukan.Tindakan}', " +
                   $" '{rujukan.NamaPenerima.ReplaceSingleQuote()}', '{rujukan.NoKpPenerima.ReplaceSingleQuote()}', '{rujukan.Jawatanpenerima.ReplaceSingleQuote()}', '{rujukan.AlamatPenerima1.ReplaceSingleQuote()}', '{rujukan.AlamatPenerima2.ReplaceSingleQuote()}', " +
                   $" '{rujukan.AlamatPenerima3.ReplaceSingleQuote()}', '{rujukan.TrkhPenerima}', '{rujukan.SetujuByr}', '{rujukan.Status}', '{rujukan.PgnDaftar}', " +
                   $"  UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), {rujukan.PgnAkhir}, UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), 2," +
                   $" '{rujukan.JenisPesalah}', '{rujukan.KodAkta}','{rujukan.KodSalah}', '{rujukan.ButirSalah.ReplaceSingleQuote()}', '{rujukan.IsArahanSemasa}', '{rujukan.TempohTawaran}', '{rujukan.AmnKmp}'," +
                   $" '{rujukan.NoEp}','{rujukan.NoIp}'," +
                   $" '{rujukan.ip_identiti_pelanggan_id}', '{rujukan.poskodpenerima}', '{rujukan.bandarpenerima}', '{rujukan.negeripenerima}', '{rujukan.negarapenerima}', '{rujukan.notelpenerima}', '{rujukan.emelpenerima}'); ";
        }

        public static string GenerateSQLScriptForTablePasukan_Trans(TbPasukanTrans pasukanTrans, string query)
        {
            var pasukanTransPgnAkhir = string.IsNullOrEmpty(pasukanTrans.TrkhDaftar) ? GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat) : pasukanTrans.TrkhDaftar;
            var pasukanTransTrkhAkhir = string.IsNullOrEmpty(pasukanTrans.TrkhAkhir) ? GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat) : pasukanTrans.TrkhAkhir;

            if (string.IsNullOrEmpty(query))
                return " insert into tbpasukan_trans " +
                   " (jenistrans, norujukan, kodpasukan, idpengguna, status, " +
                   " pgndaftar, trkhdaftar, pgnakhir, trkhakhir, catatan) " +
                   " values " +
                   $" ('{pasukanTrans.JenisTrans}','{pasukanTrans.NoRujukan}','{pasukanTrans.KodPasukan}','{pasukanTrans.Id}','{pasukanTrans.Status}', " +
                   $" '{pasukanTrans.PgnDaftar}',UNIX_TIMESTAMP('{pasukanTransPgnAkhir}'),'{pasukanTrans.PgnAkhir}',UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), '{pasukanTrans.Catatan.ReplaceSingleQuote()}'),";
            else
            {
                query += $" ('{pasukanTrans.JenisTrans}','{pasukanTrans.NoRujukan}','{pasukanTrans.KodPasukan}','{pasukanTrans.Id}','{pasukanTrans.Status}', " +
                       $" '{pasukanTrans.PgnDaftar}',UNIX_TIMESTAMP('{pasukanTransPgnAkhir}'),'{pasukanTrans.PgnAkhir}',UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), '{pasukanTrans.Catatan.ReplaceSingleQuote()}'),";
                return query;
            }
        }

        public static string GenerateSQLScriptForTableKpp_Hh(TbKpp rujukan)
        {
            var tahun = DateTime.Now.ToString("yy");
            if (!string.IsNullOrEmpty(rujukan.TrkhMulaLawatankpp))
                tahun = rujukan.TrkhMulaLawatankpp.Substring(2, 2);

            string TrkhTamatLawatanKpp = "NULL";
            if (!string.IsNullOrEmpty(rujukan.TrkhTamatLawatanKpp))
                TrkhTamatLawatanKpp = $"'{GeneralBll.ConvertDatabaseFormatStringToDateTime(rujukan.TrkhTamatLawatanKpp).ToString(Constants.DatabaseDateFormat)}'";


            return " INSERT INTO tbkpp_hh  " +
                   " (norujukankpp, idhh, kodcawangan, kodtujuan, kodasas, " +
                   " kodjenis, catatanlawatan, kodkatkawasan, norujukanatr, nosiriborangkpp," +
                   " trkhmulalawatankpp, trkhtamatlawatankpp, kodkatpremis, namapremis, alamatpremis1," +
                   " alamatpremis2, alamatpremis3, nodaftarpremis, nolesenBKP_PDA, nolesenmajlis_permit," +
                   " notelefonpremis, catatanpremis, pengeluarkpp, longitudpremis, latitudpremis," +
                   " amaran, lokasilawatan, noaduan, hasillawatan, tindakan," +
                   " namapenerima, nokppenerima, jawatanpenerima, alamatpenerima1, alamatpenerima2," +
                   " alamatpenerima3, trkhpenerima, setujubyr, tahun, pgndaftar," +
                   " trkhdaftar, isupdate, " +
                   " jenispesalah, kodakta, kodsalah, butirsalah, isarahansemasa, tempohtawaran, amnkmp," +
                   " noep, noip," +
                   " ip_identiti_pelanggan_id, poskodpenerima, bandarpenerima, negeripenerima, negarapenerima, notelpenerima, emelpenerima )" +
                   " VALUES" +
                   $" ('{rujukan.NoRujukanKpp}', '{rujukan.IdHh}', '{rujukan.KodCawangan}', '{rujukan.KodTujuan}', '{rujukan.KodAsas}', " +
                   $" '{rujukan.KodJenis}', '{rujukan.CatatanLawatan.ReplaceSingleQuote()}', '{rujukan.KodKatKawasan}', '{rujukan.NoRujukanAtr.ReplaceSingleQuote()}', '{rujukan.NoSiriBorangKpp.ReplaceSingleQuote()}', " +
                   $" '{rujukan.TrkhMulaLawatankpp}', {TrkhTamatLawatanKpp}, '{rujukan.KodKatPremis}', '{rujukan.NamaPremis.ReplaceSingleQuote()}', '{rujukan.AlamatPremis1.ReplaceSingleQuote()}', " +
                   $" '{rujukan.AlamatPremis2.ReplaceSingleQuote()}', '{rujukan.AlamatPremis3.ReplaceSingleQuote()}', '{rujukan.NoDaftarPremis.ReplaceSingleQuote()}', '{rujukan.NoLesenBKP_PDA.ReplaceSingleQuote()}', '{rujukan.NoLesenMajlis_Permit.ReplaceSingleQuote()}', " +
                   $" '{rujukan.NoTelefonPremis.ReplaceSingleQuote()}', '{rujukan.CatatanPremis.ReplaceSingleQuote()}', '{rujukan.PengeluarKpp}', '{rujukan.LongitudPremis}', '{rujukan.LatitudPremis}', " +
                   $" '{rujukan.Amaran}', '{rujukan.LokasiLawatan.ReplaceSingleQuote()}', '{rujukan.NoAduan.ReplaceSingleQuote()}', '{rujukan.HasilLawatan.ReplaceSingleQuote()}', '{rujukan.Tindakan}', " +
                   $" '{rujukan.NamaPenerima.ReplaceSingleQuote()}', '{rujukan.NoKpPenerima}', '{rujukan.Jawatanpenerima.ReplaceSingleQuote()}', '{rujukan.AlamatPenerima1.ReplaceSingleQuote()}', '{rujukan.AlamatPenerima2.ReplaceSingleQuote()}', " +
                   $" '{rujukan.AlamatPenerima3.ReplaceSingleQuote()}', '{rujukan.TrkhPenerima}', '{rujukan.SetujuByr}', '{tahun}', '{rujukan.PgnDaftar}', " +
                   $"  UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), 0," +
                   $" '{rujukan.JenisPesalah}', '{rujukan.KodAkta}','{rujukan.KodSalah}', '{rujukan.ButirSalah.ReplaceSingleQuote()}', '{rujukan.IsArahanSemasa}', '{rujukan.TempohTawaran}', '{rujukan.AmnKmp}'," +
                   $" '{rujukan.NoEp}','{rujukan.NoIp}'," +
                   $" '{rujukan.ip_identiti_pelanggan_id}', '{rujukan.poskodpenerima}', '{rujukan.bandarpenerima}', '{rujukan.negeripenerima}', '{rujukan.negarapenerima}', '{rujukan.notelpenerima}', '{rujukan.emelpenerima}'); ";
        }
        #endregion

        #region SendDataKes

        private static string GenerateSQLScriptForTableDataKes_Hh(TbDataKes dataKes)
        {
            var pegawaiSerbuan = dataKes.PegawaiSerbuan == null ? "NULL" : dataKes.PegawaiSerbuan.ToString();
            var kodKatPerniagaan = dataKes.KodKatPerniagaan == null ? "NULL" : dataKes.KodKatPerniagaan.ToString();
            var kodJenama = dataKes.KodJenama == null ? "NULL" : dataKes.KodJenama.ToString();

            var tariksalah = GeneralBll.ConvertDatabaseFormatStringToDateTime(dataKes.TrkhSalah).ToString(Constants.DatabaseDateFormat);
            return " INSERT INTO tbdatakes_hh (nokes,kodcawangan,noep,noip, " +
                   " trkhsalah,tempat," +
                   " nokpp,nokmp,kodkatkawasan, kodtujuan, pegawaiserbuan, " +
                   " namapremis,nodaftarpremis," +
                   " kodkatperniagaan, kodjenama, " +
                   " nolaporanpolis, kelaskes, kodstatuskes, kodstatuskes_det, " +
                   " pgndaftar,trkhdaftar,isupdate) VALUES " +
                   $" ('{dataKes.NoKes}', '{dataKes.KodCawangan}', '{dataKes.NoEp.ReplaceSingleQuote()}', '{dataKes.NoIp.ReplaceSingleQuote()}'," +
                   $"  '{tariksalah}',  '{dataKes.Tempat.ReplaceSingleQuote()}'," +
                   $"  '{dataKes.NoKpp}','{dataKes.NoKmp}','{dataKes.KodKatKawasan}','{dataKes.KodTujuan}', {pegawaiSerbuan}, " +
                   $"  '{dataKes.NamaPremis.ReplaceSingleQuote()}', '{dataKes.NoDaftarPremis.ReplaceSingleQuote()}', " +
                   $"  {kodKatPerniagaan}, {kodJenama}," +
                   $" '{dataKes.NoLaporanPolis.ReplaceSingleQuote()}','{dataKes.KelasKes.ReplaceSingleQuote()}','{dataKes.KodStatusKes}','{dataKes.KodStatusKes_Det.ReplaceSingleQuote()}', " +
                   $"  '{dataKes.PgnDaftar}', UNIX_TIMESTAMP('{dataKes.TrkhDaftar}'), '0'); ";

        }

        private static string GenerateSQLScriptForTableDataKes(TbDataKes dataKes)
        {
            var pegawaiSerbuan = dataKes.PegawaiSerbuan == null ? "NULL" : dataKes.PegawaiSerbuan.ToString();
            var kodKatPerniagaan = dataKes.KodKatPerniagaan == null ? "NULL" : dataKes.KodKatPerniagaan.ToString();
            var kodJenama = dataKes.KodJenama == null ? "NULL" : dataKes.KodJenama.ToString();

            var tariksalah = GeneralBll.ConvertDatabaseFormatStringToDateTime(dataKes.TrkhSalah).ToString(Constants.DatabaseDateFormat);
            return " INSERT INTO tbdatakes (nokes,kodcawangan,noep,noip, " +
                  " trkhsalah,tempat," +
                  " nokpp,nokmp, " +
                  " kodkatkawasan,kodtujuan,pegawaiserbuan, " +
                  " namapremis,nodaftarpremis,kodkatperniagaan,kodjenama,nolaporanpolis,kelaskes,kodstatuskes,kodstatuskes_det," +
                  " pgndaftar,trkhdaftar,pgnakhir,trkhakhir) VALUES " +
                  $" ('{dataKes.NoKes}', '{dataKes.KodCawangan}', '{dataKes.NoEp.ReplaceSingleQuote()}', '{dataKes.NoIp.ReplaceSingleQuote()}'," +
                  $"  '{tariksalah}',  '{dataKes.Tempat.ReplaceSingleQuote()}'," +
                  $"  '{dataKes.NoKpp}', '{dataKes.NoKmp}', " +
                  $"  '{dataKes.KodKatKawasan}', '{dataKes.KodTujuan}', {pegawaiSerbuan}, " +
                  $"  '{dataKes.NamaPremis.ReplaceSingleQuote()}', '{dataKes.NoDaftarPremis.ReplaceSingleQuote()}', {kodKatPerniagaan}, {kodJenama},'{dataKes.NoLaporanPolis.ReplaceSingleQuote()}','{dataKes.KelasKes}','{dataKes.KodStatusKes}','{dataKes.KodStatusKes_Det}'," +
                  $"  '{dataKes.PgnDaftar}', UNIX_TIMESTAMP('{dataKes.TrkhDaftar}'), '{dataKes.PgnAkhir}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')); ";

        }

        private static string GenerateSQLScriptForTableTbDataKesPesalah(TbDataKesPesalah kesPesalah)
        {
            return " INSERT INTO tbdatakes_pesalah (nokes, kodcawangan, nokpoks, namaoks," +
                   " alamatoks1, alamatoks2, alamatoks3, PgnDaftar, TrkhDaftar, pgnakhir,trkhakhir) VALUES " +
                   $" ('{kesPesalah?.NoKes}', '{kesPesalah?.KodCawangan}', '{kesPesalah?.NoKpOks.ReplaceSingleQuote()}', '{kesPesalah?.NamaOks.ReplaceSingleQuote()}', " +
                   $"  '{kesPesalah?.AlamatOks1.ReplaceSingleQuote()}', '{kesPesalah?.AlamatOks2.ReplaceSingleQuote()}', '{kesPesalah?.AlamatOks3.ReplaceSingleQuote()}', '{kesPesalah?.PgnDaftar}', UNIX_TIMESTAMP('{kesPesalah.TrkhDaftar}'), '{kesPesalah?.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')); ";
        }

        private static string GenerateSQLScriptForTableTbDataKesAsasTindakan(TbDataKesAsasTindakan kesAsasTindakan, string query)
        {
            //return " INSERT INTO tbdatakes_asastindakan (nokes, kodtujuan, kodasas, PgnDaftar, TrkhDaftar, pgnakhir,trkhakhir) " +
            //       $" VALUES('{kesAsasTindakan?.NoKes}', '{kesAsasTindakan?.KodTujuan}', '{kesAsasTindakan?.KodAsas}', " +
            //       $"  '{kesAsasTindakan?.PgnDaftar}', UNIX_TIMESTAMP('{kesAsasTindakan.TrkhDaftar}'), '{kesAsasTindakan?.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')); ";
            if (string.IsNullOrEmpty(query))
            {
                return " INSERT INTO tbdatakes_asastindakan (nokes, kodtujuan, kodasas, PgnDaftar, TrkhDaftar, pgnakhir,trkhakhir) " +
                      $" VALUES('{kesAsasTindakan?.NoKes}', '{kesAsasTindakan?.KodTujuan}', '{kesAsasTindakan?.KodAsas}', " +
                      $"  '{kesAsasTindakan?.PgnDaftar}', UNIX_TIMESTAMP('{kesAsasTindakan.TrkhDaftar}'), '{kesAsasTindakan?.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')),";
            }
            else
            {
                query += $" ('{kesAsasTindakan?.NoKes}', '{kesAsasTindakan?.KodTujuan}', '{kesAsasTindakan?.KodAsas}', " +
                      $"  '{kesAsasTindakan?.PgnDaftar}', UNIX_TIMESTAMP('{kesAsasTindakan.TrkhDaftar}'), '{kesAsasTindakan?.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')),";
            }
            return query;

            //return " INSERT INTO tbdatakes_asastindakan (nokes, kodtujuan, kodasas, PgnDaftar, TrkhDaftar, pgnakhir,trkhakhir) " +
            //       $" SELECT '{kesAsasTindakan?.NoKes}', '{kesAsasTindakan?.KodTujuan}', '{kesAsasTindakan?.KodAsas}', " +
            //       $"  '{kesAsasTindakan?.PgnDaftar}', UNIX_TIMESTAMP('{kesAsasTindakan.TrkhDaftar}'), '{kesAsasTindakan?.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}') FROM dual " +
            //       $"WHERE NOT EXISTS (SELECT concat(nokes, kodtujuan, kodasas) FROM tbdatakes_asastindakan " +
            //       $"WHERE concat(nokes, kodtujuan, kodasas) = '{kesAsasTindakan?.NoKes}{kesAsasTindakan?.KodTujuan}{kesAsasTindakan?.KodAsas}') LIMIT 1; ";
        }

        private static string GenerateSQLScriptForTableTbDataKesKesalahan(TbDataKesKesalahan kesKesalahan)
        {
            return
                 " INSERT INTO tbdatakes_kesalahan (nokes, kodcawangan, kodakta, kodsalah, butirsalah, PgnDaftar, TrkhDaftar, pgnakhir,trkhakhir) VALUES " +
                 $"('{kesKesalahan?.NoKes}', '{kesKesalahan?.KodCawangan}', '{kesKesalahan?.KodAkta}', '{kesKesalahan?.KodSalah}', " +
                 $"  '{kesKesalahan?.ButirSalah.ReplaceSingleQuote()}', '{kesKesalahan?.PgnDaftar}', UNIX_TIMESTAMP('{kesKesalahan.TrkhDaftar}'), '{kesKesalahan?.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')) ";

            //$"" +
            //$"INSERT INTO tbdatakes_kesalahan (nokes, kodcawangan, kodakta, kodsalah, butirsalah, " +
            //$"PgnDaftar, TrkhDaftar,pgnakhir,trkhakhir) " +
            //$" " +
            //$"SELECT '{kesKesalahan?.NoKes}', '{kesKesalahan?.KodCawangan}', '{kesKesalahan?.KodAkta}', '{kesKesalahan?.KodSalah}', " +
            //$"'{kesKesalahan?.ButirSalah.ReplaceSingleQuote()}', '{kesKesalahan?.PgnDaftar}', UNIX_TIMESTAMP('{kesKesalahan.TrkhDaftar}'), '{kesKesalahan?.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}') " +
            //$"FROM dual " +
            //$" " +
            //$"WHERE NOT EXISTS(SELECT concat(nokes, kodakta, kodsalah) FROM tbdatakes_kesalahan " +
            //$"WHERE concat(nokes, kodakta, kodsalah) = '{kesKesalahan?.NoKes}{kesKesalahan?.KodAkta}{kesKesalahan?.KodSalah}') LIMIT 1; ";
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noKmp"></param>
        /// <param name="kodCawangan"></param>
        /// <param name="status"></param>
        /// <param name="pgndaftar"></param>
        /// <param name="trkhdaftar"></param>
        /// <param name="pgnakhir"></param>
        /// <param name="trkhakhir"></param>
        /// <param name="kategori"></param>
        /// <param name="saveFileName"></param>
        /// <returns></returns>
        public static async Task<Response<string>> SendImageOnline(string noKmp, string kodCawangan, string status, string pgndaftar, string trkhdaftar, string pgnakhir, string trkhakhir, int kategori, bool saveFileName = false)
        {

            var allDataSent = true;
            var message = "";

            var listOfImage = GeneralBll.GetListPatPhotoNameByRujukan(noKmp);
            var list = DataAccessQuery<TbSendOnlineGambar>.GetAll();

            //if listOfImage is 0 (or the image is not exists on IMGS Folder) will set the status to Sent            
            if (!listOfImage.Any())
            {
                var imagesIsNotExist = list.Datas.Where(m => m.NoRujukan == noKmp && m.Status != Enums.StatusOnline.Sent);
                foreach (var item in imagesIsNotExist)
                {
                    item.Status = Enums.StatusOnline.Sent;
                    item.UpdateDate = GeneralBll.GetLocalDateTimeForDatabase();

                    DataAccessQuery<TbSendOnlineGambar>.Update(item);
                }
            }

            var imagesSended = new List<string>();
            if (list.Success)
            {
                imagesSended = list.Datas.Where(mbox => mbox.NoRujukan == noKmp && mbox.Status == Enums.StatusOnline.Sent).Select(m => m.Name).ToList();
            }

            foreach (var path in listOfImage)
            {
                if (imagesSended.Any(m => m == Path.GetFileName(path)))
                    continue;

                var imageBase64 = GeneralBll.GetBase64FromImagePath(path);

                var tarikhDafterInt = GeneralBll.StringDatetimeToInt(trkhdaftar);
                var tarikhAkhirInt = GeneralBll.StringDatetimeToInt(trkhakhir);

                var options = new
                {
                    nokmp = noKmp,
                    kodcawangan = kodCawangan,
                    namagambar = Path.GetFileName(path),
                    status = status,
                    pgndaftar = pgndaftar,
                    trkhdaftar = tarikhDafterInt,
                    pgnakhir = pgnakhir,
                    trkhakhir = tarikhAkhirInt,
                    images = $"data:image/jpeg;base64,{imageBase64}",
                    kategori = kategori
                };
                var jsonParam = JsonConvert.SerializeObject(options);

                var response = await HttpClientService.UploadImage(jsonParam);
                SetStatusImagesDataOnline(noKmp, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, Path.GetFileName(path));
                if (!response.Success)
                {
                    allDataSent = false;
                    message = response.Mesage;
                    if (saveFileName)
                    {
                        Log.WriteErrorRecords($"Faild to Upload Image.\rNo Rujukan : {noKmp}, \rFile Name: {Path.GetFileName(path)}, \rPath : {path}");
                    }
                }
            }

            return new Response<string> { Success = allDataSent, Mesage = message };
        }

        public static async Task<Response<string>> SendReceiptManualImageOnline(string noKmp, string kodCawangan, string status, string pgndaftar, string trkhdaftar, string pgnakhir, string trkhakhir, int kategori, bool saveFileName = false)
        {

            var allDataSent = true;
            var message = "";

            var listOfImage = GeneralBll.GetListPathPhotoReceiptNameByRujukan(noKmp);
            var list = DataAccessQuery<TbSendOnlineGambar>.GetAll();

            //if listOfImage is 0 (or the image is not exists on IMGS Folder) will set the status to Sent            
            if (!listOfImage.Any())
            {
                var imagesIsNotExist = list.Datas.Where(m => m.NoRujukan == noKmp && m.Status != Enums.StatusOnline.Sent);
                foreach (var item in imagesIsNotExist)
                {
                    item.Status = Enums.StatusOnline.Sent;
                    item.UpdateDate = GeneralBll.GetLocalDateTimeForDatabase();

                    DataAccessQuery<TbSendOnlineGambar>.Update(item);
                }
            }

            var imagesSended = new List<string>();
            if (list.Success)
            {
                imagesSended = list.Datas.Where(mbox => mbox.NoRujukan == noKmp && mbox.Status == Enums.StatusOnline.Sent).Select(m => m.Name).ToList();
            }

            foreach (var path in listOfImage)
            {
                if (imagesSended.Any(m => m == Path.GetFileName(path)))
                    continue;

                var imageBase64 = GeneralBll.GetBase64FromImagePath(path);

                var tarikhDafterInt = GeneralBll.StringDatetimeToInt(trkhdaftar);
                var tarikhAkhirInt = GeneralBll.StringDatetimeToInt(trkhakhir);

                var options = new
                {
                    nokmp = noKmp,
                    kodcawangan = kodCawangan,
                    namagambar = Path.GetFileName(path),
                    status = status,
                    pgndaftar = pgndaftar,
                    trkhdaftar = tarikhDafterInt,
                    pgnakhir = pgnakhir,
                    trkhakhir = tarikhAkhirInt,
                    images = $"data:image/jpeg;base64,{imageBase64}",
                    kategori = kategori
                };
                var jsonParam = JsonConvert.SerializeObject(options);

                var response = await HttpClientService.UploadImage(jsonParam);
                SetStatusImagesDataOnline(noKmp, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, Path.GetFileName(path));
                //Set Data name resit send
                SetStatusDataOnline(noKmp, Enums.TableType.IpResit_Manual, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                if (!response.Success)
                {
                    allDataSent = false;
                    message = response.Mesage;
                    if (saveFileName)
                    {
                        Log.WriteErrorRecords($"Faild to Upload Image.\rNo Rujukan : {noKmp}, \rFile Name: {Path.GetFileName(path)}, \rPath : {path}");
                    }
                }
            }

            return new Response<string> { Success = allDataSent, Mesage = message };
        }

        public static async Task<Response<string>> SendImageOnlinePath(string path, string noKmp, string kodCawangan, string status, string pgndaftar, string trkhdaftar, string pgnakhir, string trkhakhir, int kategori, bool saveFileName = false)
        {
            var result = new Response<string> { Success = true };
#if DEBUG
            return result;
#endif

            var imageBase64 = GeneralBll.GetBase64FromImagePath(path);

            var tarikhDafterInt = GeneralBll.StringDatetimeToInt(trkhdaftar);
            var tarikhAkhirInt = GeneralBll.StringDatetimeToInt(trkhakhir);

            var options = new
            {
                nokmp = noKmp,
                kodcawangan = kodCawangan,
                namagambar = Path.GetFileName(path),
                status = status,
                pgndaftar = pgndaftar,
                trkhdaftar = tarikhDafterInt,
                pgnakhir = pgnakhir,
                trkhakhir = tarikhAkhirInt,
                images = $"data:image/jpeg;base64,{imageBase64}",
                kategori = kategori
            };
            var jsonParam = JsonConvert.SerializeObject(options);

            var response = await HttpClientService.UploadImage(jsonParam);
            SetStatusImagesDataOnline(noKmp, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, Path.GetFileName(path));
            if (!response.Success)
            {
                result.Success = false;

                if (saveFileName)
                {
                    Log.WriteErrorRecords($"Faild to Upload Image.\rNo Rujukan : {noKmp}, \rFile Name: {Path.GetFileName(path)}, \rPath : {path}");
                }
            }

            return result;
        }

        public static async Task<Response<string>> SendDataOnlineFromService(Android.Content.Context context)
        {
            var response = new Response<string> { Success = false, Mesage = "Tiada Sambungan Internet" };

            if (!GeneralAndroidClass.IsOnline())
                return response;

            if (!GeneralBll.IsBackgroundServiceRunning())
            {
                response.Mesage = "Servis Dalaman Terhenti";
                return response;
            }

            var result = DataAccessQuery<TbSendOnlineData>.GetAll();
            if (result.Success)
            {
                foreach (var item in result.Datas.Where(m => m.Type != Enums.TableType.KompaunBayaran
                        && m.Type != Enums.TableType.KPP_HH && m.Type != Enums.TableType.Kompaun_HH && m.Type != Enums.TableType.DataKes_HH && m.Type != Enums.TableType.Akuan_UpdateKompaun_HH))
                {
                    if (!GeneralBll.IsBackgroundServiceRunning())
                    {
                        response.Mesage = "Servis Dalaman Terhenti";
                        return response;
                    }

                    response = await SendDataOnlineAsync(item.NoRujukan, item.Type, context);
                    Log.WriteLogBackgroundService($"SendOnlineBLL \r SendDataOnlineFromService \r Send - {item.NoRujukan}, Success: {response.Success}, Message : {response.Mesage}");
                }
            }

            if (!GeneralBll.IsBackgroundServiceRunning())
            {
                response.Mesage = "Servis Dalaman Terhenti";
                return response;
            }

            await SendPasukanTrans(string.Empty, context);

            var imageResponse = DataAccessQuery<TbSendOnlineGambar>.GetAll();
            if (imageResponse.Success)
            {
                foreach (var item in imageResponse.Datas.Where(m => m.Status != Enums.StatusOnline.Sent))
                {
                    if (!GeneralBll.IsBackgroundServiceRunning())
                    {
                        response.Mesage = "Servis Dalaman Terhenti";
                        return response;
                    }

                    var dataOnline = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == item.NoRujukan);
                    if (dataOnline.Success)
                    {
                        if (dataOnline.Datas.Type == Enums.TableType.KPP)
                        {
                            var tbKpp = PemeriksaanBll.GetPemeriksaanByRujukan(dataOnline.Datas.NoRujukan);
                            if (tbKpp != null)
                            {
                                await SendImageOnline(dataOnline.Datas.NoRujukan, tbKpp.KodCawangan, tbKpp.Status, tbKpp.PgnDaftar.ToString(), tbKpp.TrkhDaftar, tbKpp.PgnAkhir.ToString(), tbKpp.TrkhAkhir, 2);
                            }
                        }
                        else if (dataOnline.Datas.Type == Enums.TableType.Kompaun)
                        {
                            var kompaun = KompaunBll.GetKompaunByRujukan(dataOnline.Datas.NoRujukan);
                            if (kompaun.Success)
                            {
                                if (kompaun.Datas != null)
                                {
                                    await SendImageOnline(dataOnline.Datas.NoRujukan, kompaun.Datas.KodCawangan, kompaun.Datas.Status, kompaun.Datas.PgnDaftar.ToString(), kompaun.Datas.TrkhDaftar, kompaun.Datas.PgnAkhir.ToString(), kompaun.Datas.TrkhAkhir, 3);
                                }
                            }
                        }
                        else if (dataOnline.Datas.Type == Enums.TableType.DataKes)
                        {
                            var dataKes = KompaunBll.GetSiasatByNoKes(dataOnline.Datas.NoRujukan);
                            if (dataKes != null)
                            {
                                await SendImageOnline(dataOnline.Datas.NoRujukan, dataKes.KodCawangan, dataKes.Status, dataKes.PgnDaftar.ToString(), dataKes.TrkhDaftar, dataKes.PgnAkhir.ToString(), dataKes.TrkhAkhir, 1);
                            }
                        }
                    }
                }
            }

            await PrepareGPSLog(null);

            return response;
        }

        private static string GenerateScriptForSendGpsLog(TbGpsLog item)
        {
            return " insert into tbgpslog " +
                   " (kodcawangan, idhh, idstaf, trkhlog, longitud, latitud, pgndaftar, " +
                   " trkhdaftar, pgnakhir, trkhakhir) VALUES " +
                   $" ('{item.KodCawangan}', '{item.IdHh}', '{item.IdStaf}', '{item.TrkhLog}', '{item.Longitud}', '{item.Latitud}', '{item.PgnDaftar}', " +
                   $" UNIX_TIMESTAMP('{item.TrkhDaftar}'), '{item.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'))";
        }

        private static async Task PrepareGPSLog(Android.Content.Context context)
        {
            var result = DataAccessQuery<TbGpsLog>.GetAll();
            if (result.Success)
            {
                foreach (var item in result.Datas.Where(m => m.IsSendOnline != Enums.StatusOnline.Sent))
                {
                    await SendGPSLog(item, context);
                }
            }
        }

        public static async Task SendGPSLog(TbGpsLog tbGpsLog, Android.Content.Context context)
        {
            var response = await HttpClientService.ExecuteQuery(GenerateScriptForSendGpsLog(tbGpsLog), context);
            if (response.Success)
            {
                tbGpsLog.IsSendOnline = Enums.StatusOnline.Sent;
                DataAccessQuery<TbGpsLog>.Update(tbGpsLog);
            }
            Thread.Sleep(100);
        }

        public static async Task UpdateHandheldAsync(Android.Content.Context context)
        {
            var result = DataAccessQuery<TbHandheld>.GetAll();
            if (result.Success)
            {
                foreach (var item in result.Datas)
                {
                    var sqlQuery = $" update tbhandheld set " +
                                   $" appver = '{Constants.AppVersion}', " +
                                   $" noturutan_kpp = '{item.NotUrutan_Kpp}', " +
                                   $" noturutan_kots = '{item.NotUrutan_Kots}', " +
                                   $" noturutan_datakes = '{item.NotUrutan_DataKes}', " +
                                   $" trkhhhcheckout = '{item.TrkhHhCheckout}', " +
                                   $" Jumlah_Kpp = {item.Jumlah_Kpp}," +
                                   $" Jumlah_Gambar_Kpp = {item.Jumlah_Gambar_Kpp}, " +
                                   $" Jumlah_Kots = {item.Jumlah_Kots}," +
                                   $" Jumlah_Ak = {item.Jumlah_Ak}," +
                                   $" Jumlah_Gambar_Kots = {item.Jumlah_Gambar_Kots}, " +
                                   $" Jumlah_DataKes = {item.Jumlah_DataKes},  " +
                                   $" Jumlah_Gambar_DataKes = {item.Jumlah_Gambar_DataKes},  " +
                                   $" Jumlah_Nota = {item.Jumlah_Nota}, " +
                                   $" pgnakhir = '{item.PgnAkhir}', TrkhAkhir = unix_timestamp('{GeneralBll.GetLocalDateTimeForDatabase()}') where idhh = '{item.IdHh}' ";

                    var response = await HttpClientService.ExecuteQuery(sqlQuery, context);
                    Log.WriteLogFile($"SendOnlineBLL \r UpdateHandheldAsync \r Update - {item.IdHh}, Success: {response.Success}, Message : {response.Mesage}");
                }
            }
        }

        #region NewVersion

        public static async Task<Response<string>> SendKppOnlineAsyncV2(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>() { Success = true, Mesage = "Ralat" };
            var query = "";
            //Send TbKPP
            var tbKpp = PemeriksaanBll.GetPemeriksaanByRujukan(noRujukan);
            if (tbKpp != null)
            {
                if (tableType == Enums.TableType.KPP_HH)
                {
                    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KPP_HH && m.Status != Enums.StatusOnline.Sent);
                    if (datas.Success && datas.Datas != null)
                    {
                        //Check data to server
                        query = $"select count(*) as count from tbkpp_hh where norujukankpp = '{noRujukan}'";
                        var responseCheckData = await HttpClientService.CountAync(query);
                        Log.WriteLogFile($"Check data KPP_HH for NoRujukan {noRujukan}, {responseCheckData.Success} - {responseCheckData.Result?.Count}");

                        if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                        {
                            SetStatusDataOnline(noRujukan, Enums.TableType.KPP_HH, Enums.StatusOnline.Sent);
                        }
                        else
                        {
                            //Send KPP_HH
                            query = GenerateSQLScriptForTableKpp_Hh(tbKpp);
                            response = await HttpClientService.ExecuteQuery(query, context);
                            //Set Data online for KPP_HH
                            SetStatusDataOnline(noRujukan, Enums.TableType.KPP_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                            //Save the script if have error when send api
                            if (saveScriptToFile && !response.Success)
                            {
                                Log.WriteErrorRecords(query);
                                response.Success = response.Success;
                            }
                        }
                    }
                }

                if (tableType == Enums.TableType.KPP)
                {
                    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KPP && m.Status != Enums.StatusOnline.Sent);
                    if (datas.Success && datas.Datas != null)
                    {
                        //Check data to server
                        query = $"select count(*) as count from tbkpp where norujukankpp = '{noRujukan}'";
                        var responseCheckData = await HttpClientService.CountAync(query);
                        Log.WriteLogFile($"Check data KPP for NoRujukan {noRujukan}, {responseCheckData.Success} - {responseCheckData.Result?.Count}");

                        if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                        {
                            SetStatusDataOnline(noRujukan, Enums.TableType.KPP, Enums.StatusOnline.Sent);
                        }
                        else
                        {
                            //Send KPP
                            query = GenerateSQLScriptForTableKpp(tbKpp);
                            response = await HttpClientService.ExecuteQuery(query, context);
                            //Set Data online for KPP
                            SetStatusDataOnline(noRujukan, Enums.TableType.KPP, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                            //Save the script if have error when send api
                            if (saveScriptToFile && !response.Success)
                            {
                                Log.WriteErrorRecords(query);
                                response.Success = response.Success;
                            }
                        }
                    }
                }

                //Send Pasukan Trans
                var responsePasukanTrans = await SendPasukanTrans(noRujukan, context, saveScriptToFile);
                if (!responsePasukanTrans.Success)
                    response.Success = false;

                //Send Images
                var responseImage = await SendImageOnline(noRujukan, tbKpp.KodCawangan, tbKpp.Status, tbKpp.PgnDaftar.ToString(), tbKpp.TrkhDaftar, tbKpp.PgnAkhir.ToString(), tbKpp.TrkhAkhir, 2, saveScriptToFile);
                if (!responseImage.Success)
                    response.Success = false;

                //Send Asas Tindakan
                var responseAsasTindakan = await SendKppAsasTindakan(noRujukan, context, saveScriptToFile);
                if (!responseAsasTindakan.Success)
                    response.Success = false;
            }

            return response;
        }

        //public static async Task<Response<string>> SendIPResitOnlineAsyncV1(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        //{
        //    var response = new Response<string>() { Success = true, Mesage = "Ralat" };
        //    var query = "";
        //    //Send TbKPP
        //    var tbKpp = PemeriksaanBll.GetPemeriksaanByRujukan(noRujukan);
        //    if (tbKpp != null)
        //    {
        //        if (tableType == Enums.TableType.IpResit_Manual)
        //        {
        //            //Set Data name resit send
        //            SetStatusDataOnline(noRujukan, Enums.TableType.IpResit_Manual, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
        //            if (response.Success) 
        //            {
        //                return response = true ;
        //            }
        //        }
        //    }

        //    return response;
        //}

        public static string GenerateScriptAsasTindakan(TbKppAsasTindakan tindakan, string query)
        {
            ////old style
            //return " INSERT INTO tbkpp_asastindakan " +
            //" (norujukankpp, kodtujuan, kodasas, pgndaftar, trkhdaftar, pgnakhir, trkhakhir) " +
            //" VALUES " +
            //$" ('{tindakan.NoRujukanKpp}', '{tindakan.KodTujuan}', '{tindakan.KodAsas}', '{tindakan.PgnDaftar}', UNIX_TIMESTAMP('{tindakan.TrkhDaftar}'), '{tindakan.PgnAkhir}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')); ";

            //new style for multiple value
            if (string.IsNullOrEmpty(query))
            {
                query = " INSERT INTO tbkpp_asastindakan " +
                        " (norujukankpp, kodtujuan, kodasas, pgndaftar, trkhdaftar, pgnakhir, trkhakhir) " +
                        " VALUES " +
                        $" ('{tindakan.NoRujukanKpp}', '{tindakan.KodTujuan}', '{tindakan.KodAsas}', '{tindakan.PgnDaftar}', UNIX_TIMESTAMP('{tindakan.TrkhDaftar}'), '{tindakan.PgnAkhir}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')),";
            }
            else
            {
                query += $" ('{tindakan.NoRujukanKpp}', '{tindakan.KodTujuan}', '{tindakan.KodAsas}', '{tindakan.PgnDaftar}', UNIX_TIMESTAMP('{tindakan.TrkhDaftar}'), '{tindakan.PgnAkhir}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')),";
            }
            return query;

            //new style
            //return " INSERT INTO tbkpp_asastindakan " +
            // " (norujukankpp, kodtujuan, kodasas, pgndaftar, trkhdaftar, pgnakhir, trkhakhir) " +
            // $" SELECT '{tindakan.NoRujukanKpp}', '{tindakan.KodTujuan}', '{tindakan.KodAsas}', '{tindakan.PgnDaftar}', UNIX_TIMESTAMP('{tindakan.TrkhDaftar}'), '{tindakan.PgnAkhir}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}') " +
            // $"FROM dual " +
            // $"WHERE NOT EXISTS (SELECT concat(norujukankpp, kodtujuan, kodasas) FROM tbkpp_asastindakan " +
            // $"WHERE concat(norujukankpp, kodtujuan, kodasas) = '{tindakan.NoRujukanKpp}{tindakan.KodTujuan}{tindakan.KodAsas}') LIMIT 1; ";

        }

        public static async Task<Response<string>> SendKppAsasTindakan(string noRujukan, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var allAsasTindakanSend = true;
            var asasTindakan = DataAccessQuery<TbKppAsasTindakan>.GetAll(m => m.NoRujukanKpp == noRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
            foreach (var item in asasTindakan.Datas)
            {
                //Check data to server
                var query = $"select count(*) as count from tbkpp_asastindakan where norujukankpp='{item.NoRujukanKpp}' and kodtujuan='{item.KodTujuan}' and kodasas = '{item.KodAsas}' ";
                var responseCheckData = await HttpClientService.CountAync(query);
                Log.WriteLogFile($"Check data KPP Asas Tindakan, No Rujukan {item.NoRujukanKpp} - No Tujuan {item.KodTujuan} - Kod Asas {item.KodAsas}... , {responseCheckData.Success} - {responseCheckData.Result?.Count}");

                if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                {
                    DataAccessQuery<TbKppAsasTindakan>.ExecuteSql($"UPDATE tbkpp_asastindakan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' " +
                                              $"WHERE NoRujukanKpp = '{item.NoRujukanKpp}' AND KodTujuan = '{item.KodTujuan}' AND KodAsas = '{item.KodAsas}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                }
            }

            asasTindakan = DataAccessQuery<TbKppAsasTindakan>.GetAll(m => m.NoRujukanKpp == noRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
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
                        if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(query);
                        if (!allAsasTindakanSend)
                        {
                            response.Success = false;
                            response.Mesage = "Asas Tindakan Tidak Dihantar";
                            return response;
                        }
                    }
                }
            }

            return new Response<string>();
        }

        public static async Task<Response<string>> SendPasukanTrans(string noRujukan, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var allDataAlreadySent = true;
            var message = "";
            var pasukanTrans = DataAccessQuery<TbPasukanTrans>.GetAll(m => m.JenisTrans == Constants.JenisTrans.Kpp && m.IsSendOnline != Enums.StatusOnline.Sent && m.NoRujukan == noRujukan);
            if (pasukanTrans.Success)
            {
                foreach (var item in pasukanTrans.Datas)
                {
                    //Check data to server
                    var query = $"select count(*) as count from tbpasukan_trans where jenistrans='{item.JenisTrans}' and norujukan ='{item.NoRujukan}' and kodpasukan='{item.KodPasukan}' and idpengguna = '{item.Id}' ";
                    var responseCheckData = await HttpClientService.CountAync(query);
                    Log.WriteLogFile($"Check data tbpasukan_trans for NoRujukan: {noRujukan} - ID: {item.Id} , {responseCheckData.Success} - {responseCheckData.Result?.Count}");

                    if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                    {
                        SetStatusOnlinePasukanTrans(item.Id, Enums.StatusOnline.Sent, item.NoRujukan);
                    }
                }
            }


            pasukanTrans = DataAccessQuery<TbPasukanTrans>.GetAll(m => m.JenisTrans == Constants.JenisTrans.Kpp && m.IsSendOnline != Enums.StatusOnline.Sent && m.NoRujukan == noRujukan);
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
                            SetStatusOnlinePasukanTrans(item.Id, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, item.NoRujukan);
                        }

                        if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(query);
                        if (!response.Success)
                        {
                            message = response.Mesage;
                            allDataAlreadySent = false;
                        }
                    }
                }
            }
            //if (pasukanTrans.Success)
            //{
            //    var datas = pasukanTrans.Datas.Where(m => m.JenisTrans == Constants.JenisTrans.Kpp && m.IsSendOnline != Enums.StatusOnline.Sent);
            //    if (!string.IsNullOrEmpty(noRujukan))
            //        datas = datas.Where(m => m.NoRujukan == noRujukan);
            //
            //    foreach (var item in datas)
            //    {
            //        var query = GenerateSQLScriptForTablePasukan_Trans(item);
            //        var response = await HttpClientService.ExecuteQuery(query, context);
            //        SetStatusOnlinePasukanTrans(item.Id, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, item.NoRujukan);
            //
            //        //Save the script if have error when send api
            //        if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(query);
            //        if (!response.Success)
            //        {
            //            allDataAlreadySent = false;
            //            message = response.Mesage;
            //        }
            //    }
            //}

            return new Response<string> { Success = allDataAlreadySent, Mesage = message };
        }

        public static async Task<Response<string>> SendKompaunOnlineAsyncV2(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>() { Success = true, Mesage = "Ralat" };
            var query = "";
            var result = KompaunBll.GetKompaunByRujukan(noRujukan);
            if (result.Success)
            {
                if (result.Datas != null)
                {
                    if (tableType == Enums.TableType.Kompaun_HH)
                    {
                        var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Kompaun_HH && m.Status != Enums.StatusOnline.Sent);
                        if (dataKompaund.Success && dataKompaund.Datas != null)
                        {
                            //Check data to server
                            query = $"select count(*) as count from tbkompaun_hh where nokmp = '{noRujukan}'";
                            var responseCheckData = await HttpClientService.CountAync(query);
                            Log.WriteLogFile($"Check data tbkompaun_hh for NoRujukan {noRujukan}, {responseCheckData.Success} - {responseCheckData.Result?.Count}");

                            if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                            {
                                SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun_HH, Enums.StatusOnline.Sent);
                            }
                            else
                            {
                                //Send Kompaun_HH
                                query = GenerateSQLScriptForTableKompaun_Hh(result.Datas);
                                response = await HttpClientService.ExecuteQuery(query, context);
                                //Set Data online for Kompaun_HH
                                SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                                //Save the script if have error when send api
                                if (saveScriptToFile && !response.Success)
                                {
                                    Log.WriteErrorRecords(query);
                                    response.Success = response.Success;
                                }
                            }
                        }
                    }
                    if (tableType == Enums.TableType.Kompaun)
                    {
                        var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Kompaun && m.Status != Enums.StatusOnline.Sent);
                        if (dataKompaund.Success && dataKompaund.Datas != null)
                        {
                            //Check data to server
                            query = $"select count(*) as count from tbkompaun where nokmp = '{noRujukan}'";
                            var responseCheckData = await HttpClientService.CountAync(query);
                            Log.WriteLogFile($"Check data tbkompaun for NoRujukan {noRujukan}, {responseCheckData.Success} - {responseCheckData.Result?.Count}");

                            if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                            {
                                SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun, Enums.StatusOnline.Sent);
                            }
                            else
                            {
                                //Send Kompaun
                                query = GenerateSQLScriptForTableKompaun(result.Datas);
                                response = await HttpClientService.ExecuteQuery(query, context);
                                //Set Data online for Kompaun
                                SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                                //Save the script if have error when send api
                                if (saveScriptToFile && !response.Success)
                                {
                                    Log.WriteErrorRecords(query);
                                    response.Success = response.Success;
                                }
                            }
                        }
                    }

                    var responseImage = await SendImageOnline(noRujukan, result.Datas.KodCawangan, result.Datas.Status, result.Datas.PgnDaftar.ToString(), result.Datas.TrkhDaftar, result.Datas.PgnAkhir.ToString(), result.Datas.TrkhAkhir, 3, saveScriptToFile);
                    if (!responseImage.Success)
                        response.Success = responseImage.Success;
                }
            }

            return response;
        }

        public static async Task<Response<string>> CheckDataKesBasedNoRujukan(string noRujukan, Android.Content.Context context)
        {
            var response = new Response<string> { Success = true };

            var tbDateKes = DataAccessQuery<TbDataKes>.Get(c => c.NoKmp == noRujukan);
            if (tbDateKes.Success && tbDateKes.Datas != null)
            {
                var dataKesFromKompaun = DataAccessQuery<TbSendOnlineData>.Get(
                    m => m.NoRujukan == tbDateKes.Datas.NoKes &&
                    m.Type == Enums.TableType.DataKes &&
                    m.Status != Enums.StatusOnline.Sent);
                if (dataKesFromKompaun.Success && dataKesFromKompaun.Datas != null)
                {
                    // NoRujukan = NoKes, ex: DTKKCH0222100007
                    var result = await SendDataKesOnlineAsyncV2(dataKesFromKompaun.Datas.NoRujukan, Enums.TableType.DataKes, context);
                    if (!result.Success)
                        response.Success = false;
                }

                dataKesFromKompaun = DataAccessQuery<TbSendOnlineData>.Get(
                    m => m.NoRujukan == tbDateKes.Datas.NoKes &&
                    m.Type == Enums.TableType.DataKes_HH &&
                    m.Status != Enums.StatusOnline.Sent);
                if (dataKesFromKompaun.Success && dataKesFromKompaun.Datas != null)
                {
                    var result = await SendDataKesOnlineAsyncV2(dataKesFromKompaun.Datas.NoRujukan, Enums.TableType.DataKes_HH, context);
                    if (!result.Success)
                        response.Success = false;
                }
            }

            return response;
        }

        public static async Task<Response<string>> SendAkuanAsync(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>() { Success = true, Mesage = "" };

            var kompaun = KompaunBll.GetKompaunByRujukan(noRujukan);
            if (kompaun.Success)
            {
                if (kompaun.Datas != null)
                {
                    if (kompaun.Datas.IsCetakAkuan == Constants.CetakAkuan.Yes)
                    {
                        var isKompaunHasSent = false;
                        var data = DataAccessQuery<TbSendOnlineData>.GetAll();
                        if (data.Success)
                        {
                            var kompauns = data.Datas.Where(m => m.NoRujukan == noRujukan
                                                              && m.Status == Enums.StatusOnline.Sent
                                                              && (m.Type == Enums.TableType.Kompaun || m.Type == Enums.TableType.Kompaun_HH));
                            if (kompauns.Count() == 2 || kompauns.Count() == 0)  //it will be check data kompaun already sent or data is deleted
                                isKompaunHasSent = true;
                        }

                        // we will send KompaunBayaran, Akuan_UpdateKompaun and Akuan_UpdateKompaun_HH IF KOMPAUN AND KOMPAUN HAS BEEN SENT
                        if (!isKompaunHasSent)
                            return response;

                        if (tableType == Enums.TableType.KompaunBayaran)
                        {
                            var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KompaunBayaran && m.Status != Enums.StatusOnline.Sent);
                            if (dataKompaund.Success && dataKompaund.Datas != null)
                            {
                                var query = $"select count(*) as count from tbkompaun_bayaran where nokmp = '{kompaun.Datas.NoKmp}'";
                                var responseCheckData = await HttpClientService.CountAync(query);
                                Log.WriteLogFile($"Check data tbkompaun_bayaran for nokmp {kompaun.Datas.NoKmp}, {responseCheckData.Success} - {responseCheckData.Result?.Count}");

                                if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                                {
                                    SetStatusDataOnline(noRujukan, Enums.TableType.KompaunBayaran, Enums.StatusOnline.Sent);
                                    return response;
                                }
                                else
                                {
                                    var trkhBayar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
                                    var kompaunBayaran = AkuanBll.GetKompaunBayaranByKompaun(kompaun.Datas.NoKmp);
                                    if (!string.IsNullOrEmpty(kompaun.Datas.TrkhPenerima_Akuan))
                                        trkhBayar = kompaun.Datas.TrkhPenerima_Akuan;

                                    var sqlQuery = " insert into tbkompaun_bayaran (kodcawangan,nokmp,trkhbyr,amnbyr,noresit,status,pgndaftar, " +
                                                   " trkhdaftar,pgnakhir,trkhakhir, pusat_terimaan) VALUES " +
                                                   $" ('{kompaun.Datas.KodCawangan}', '{kompaun.Datas.NoKmp}', '{trkhBayar}','{kompaun.Datas.AmnByr}','{kompaun.Datas.NoResit.ReplaceSingleQuote()}', '2', '{kompaun.Datas.PgnDaftar}'," +
                                                   $" UNIX_TIMESTAMP('{kompaun.Datas.TrkhDaftar}'), '{kompaun.Datas.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}') , " +
                                                   $" '{kompaunBayaran.pusat_terimaan}'); ";
                                    //Send Kompaun Bayaran
                                    response = await HttpClientService.ExecuteQuery(sqlQuery, context);
                                    //Set Data online for Kompaun Bayaran
                                    SetStatusDataOnline(noRujukan, Enums.TableType.KompaunBayaran, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                                    //Save the script if have error when send api
                                    if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(sqlQuery);
                                    return response;
                                }
                            }
                            else
                            {
                                response.Success = true;
                                return response;
                            }
                        }

                        var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Status == Enums.StatusOnline.Sent
                                    && m.Type == Enums.TableType.KompaunBayaran);
                        if (datas.Success)
                        {
                            if (datas.Datas != null && datas.Datas.Status == Enums.StatusOnline.Sent)
                            {
                                var trkhBayar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
                                if (!string.IsNullOrEmpty(kompaun.Datas.TrkhPenerima_Akuan))
                                    trkhBayar = kompaun.Datas.TrkhPenerima_Akuan;

                                if (tableType == Enums.TableType.Akuan_UpdateKompaun)
                                {
                                    var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Akuan_UpdateKompaun && m.Status != Enums.StatusOnline.Sent);
                                    if (dataKompaund.Success && dataKompaund.Datas != null)
                                    {
                                        var sqlQuery = $" update tbkompaun set namapenerima_akuan = '{kompaun.Datas.NamaPenerima_Akuan.ReplaceSingleQuote()}', nokppenerima1_Akuan = '{kompaun.Datas.NoKpPenerima_Akuan.ReplaceSingleQuote()}', alamatpenerima1_akuan = '{kompaun.Datas.AlamatPenerima1_Akuan.ReplaceSingleQuote()}', " +
                                                   $" alamatpenerima2_akuan = '{kompaun.Datas.AlamatPenerima2_Akuan.ReplaceSingleQuote()}', alamatpenerima3_akuan = '{kompaun.Datas.AlamatPenerima3_Akuan.ReplaceSingleQuote()}', trkhpenerima_akuan = '{trkhBayar}', amnbyr = '{kompaun.Datas.AmnByr}', " +
                                                   $" noresit ='{kompaun.Datas.NoResit.ReplaceSingleQuote()}', trkhakhir = unix_timestamp('{GeneralBll.GetLocalDateTimeForDatabase()}'), status = 3, " +
                                                   $" poskodpenerima_akuan ='{kompaun.Datas.poskodpenerima_akuan}', bandarpenerima_akuan='{kompaun.Datas.bandarpenerima_akuan}', negeripenerima_akuan='{kompaun.Datas.negeripenerima_akuan}', negarapenerima_akuan='{kompaun.Datas.negarapenerima_akuan}', notelpenerima_akuan='{kompaun.Datas.notelpenerima_akuan}', emelpenerima_akuan='{kompaun.Datas.emelpenerima_akuan}', " +
                                                   $" isbayarmanual='{kompaun.Datas.isbayarmanual}', gambarbuktibayaran='{kompaun.Datas.gambarbuktibayaran}', ip_identiti_pelanggan_id_akuan='{kompaun.Datas.ip_identiti_pelanggan_id_akuan}' " +
                                                   $" where nokmp = '{noRujukan}' and iscetakakuan = 1 ";
                                        //Send Kompaun Bayaran
                                        response = await HttpClientService.ExecuteQuery(sqlQuery, context);
                                        //Set Data online for Kompaun Bayaran
                                        SetStatusDataOnline(noRujukan, Enums.TableType.Akuan_UpdateKompaun, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                                        //Set Data name resit send
                                        SetStatusDataOnline(noRujukan, Enums.TableType.IpResit_Manual, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                                        //trigger maklumat pembayaran if isBayarManual is 0
                                        //http://localhost:8000/api/ipayment/maklumatpembayaran/KPPMLKMAN2300001/1

                                        //Save the script if have error when send api
                                        if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(sqlQuery);
                                    }
                                }
                                if (tableType == Enums.TableType.Akuan_UpdateKompaun_HH)
                                {
                                    var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Akuan_UpdateKompaun_HH && m.Status != Enums.StatusOnline.Sent);
                                    if (dataKompaund.Success && dataKompaund.Datas != null)
                                    {
                                        var sqlQuery = $" update tbkompaun_hh set namapenerima_akuan = '{kompaun.Datas.NamaPenerima_Akuan.ReplaceSingleQuote()}', nokppenerima1_Akuan = '{kompaun.Datas.NoKpPenerima_Akuan.ReplaceSingleQuote()}', alamatpenerima1_akuan = '{kompaun.Datas.AlamatPenerima1_Akuan.ReplaceSingleQuote()}', " +
                                                   $" alamatpenerima2_akuan = '{kompaun.Datas.AlamatPenerima2_Akuan.ReplaceSingleQuote()}', alamatpenerima3_akuan = '{kompaun.Datas.AlamatPenerima3_Akuan.ReplaceSingleQuote()}', trkhpenerima_akuan = '{trkhBayar}', amnbyr = '{kompaun.Datas.AmnByr}', " +
                                                   $" noresit ='{kompaun.Datas.NoResit.ReplaceSingleQuote()}', trkhakhir = unix_timestamp('{GeneralBll.GetLocalDateTimeForDatabase()}'), status = 3, " +
                                                   $" poskodpenerima_akuan ='{kompaun.Datas.poskodpenerima_akuan}', bandarpenerima_akuan='{kompaun.Datas.bandarpenerima_akuan}', negeripenerima_akuan='{kompaun.Datas.negeripenerima_akuan}', negarapenerima_akuan='{kompaun.Datas.negarapenerima_akuan}', notelpenerima_akuan='{kompaun.Datas.notelpenerima_akuan}', emelpenerima_akuan='{kompaun.Datas.emelpenerima_akuan}', " +
                                                   $" isbayarmanual='{kompaun.Datas.isbayarmanual}', gambarbuktibayaran='{kompaun.Datas.gambarbuktibayaran}', ip_identiti_pelanggan_id_akuan='{kompaun.Datas.ip_identiti_pelanggan_id_akuan}' " +
                                                   $" where nokmp = '{noRujukan}' and iscetakakuan = 1 ";
                                                   
                                        //Send Kompaun Bayaran
                                        response = await HttpClientService.ExecuteQuery(sqlQuery, context);
                                        //Set Data online for Kompaun Bayaran
                                        //Save the script if have error when send api
                                        if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(sqlQuery);
                                        SetStatusDataOnline(noRujukan, Enums.TableType.Akuan_UpdateKompaun_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                                        //Set Data name resit send
                                        SetStatusDataOnline(noRujukan, Enums.TableType.IpResit_Manual, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                                    }
                                }
                            }

                        }
                    }
                }
            }

            return response;

        }

        public static async Task<Response<string>> SendDataKesOnlineAsyncV2(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>() { Success = true, Mesage = "Ralat" };
            var query = "";
            var data = KompaunBll.GetSiasatByNoKes(noRujukan);
            if (data != null)
            {
                if (tableType == Enums.TableType.DataKes_HH)
                {
                    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.DataKes_HH && m.Status != Enums.StatusOnline.Sent);
                    if (datas.Success && datas.Datas != null)
                    {
                        query = $"select count(*) as count from tbdatakes_hh where nokes = '{noRujukan}'";

                        var responseCheckData = await HttpClientService.CountAync(query);
                        Log.WriteLogFile($"Check data tbdatakes_hh for NoRujukan {noRujukan}, {responseCheckData.Success} - {responseCheckData.Result?.Count}");
                        if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                        {
                            SetStatusDataOnline(noRujukan, Enums.TableType.DataKes_HH, Enums.StatusOnline.Sent);
                        }
                        else
                        {
                            //Send DataKes_HH
                            query = GenerateSQLScriptForTableDataKes_Hh(data);
                            response = await HttpClientService.ExecuteQuery(query, context);
                            //Set Data online for DataKes_HH
                            SetStatusDataOnline(noRujukan, Enums.TableType.DataKes_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                            //Save the script if have error when send api
                            if (saveScriptToFile && !response.Success)
                            {
                                Log.WriteErrorRecords(query);
                                response.Success = response.Success;
                            }
                        }
                    }
                }
                if (tableType == Enums.TableType.DataKes)
                {
                    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.DataKes && m.Status != Enums.StatusOnline.Sent);
                    if (datas.Success && datas.Datas != null)
                    {
                        query = $"select count(*) as count from tbdatakes where nokes = '{noRujukan}'";

                        var responseCheckData = await HttpClientService.CountAync(query);
                        Log.WriteLogFile($"Check data tbdatakes for NoRujukan {noRujukan}, {responseCheckData.Success} - {responseCheckData.Result?.Count}");
                        if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                        {
                            SetStatusDataOnline(noRujukan, Enums.TableType.DataKes, Enums.StatusOnline.Sent);
                        }
                        else
                        {
                            //Send DataKes
                            query = GenerateSQLScriptForTableDataKes(data);
                            response = await HttpClientService.ExecuteQuery(query, context);
                            //Set Data online for DataKes
                            SetStatusDataOnline(noRujukan, Enums.TableType.DataKes, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                            //Save the script if have error when send api
                            if (saveScriptToFile && !response.Success)
                            {
                                Log.WriteErrorRecords(query);
                                response.Success = response.Success;
                            }
                        }
                    }
                }

                //Send TbDataKesPesalah
                var responseDataKesPesalah = await SendDataKesPesalah(data.NoKes, context, saveScriptToFile);
                if (!responseDataKesPesalah.Success)
                    response.Success = responseDataKesPesalah.Success;

                //Send TbDataKesAsasTindakan
                var responseDataKesAsasTindakan = await SendDataKesAsasTindakan(data.NoKes, context, saveScriptToFile);
                if (!responseDataKesAsasTindakan.Success)
                    response.Success = responseDataKesPesalah.Success;

                //Send TbDataKesKesalahan
                var responseDataKesKesalahan = await SendDataKesKesalahan(data.NoKes, context, saveScriptToFile);
                if (!responseDataKesKesalahan.Success)
                    response.Success = responseDataKesKesalahan.Success;

                //Send TbDataKesPesalahKesalahan
                var responseDataKesPesalahKesalahan = await SendDataKesPesalahKesalahan(data.NoKes, context, saveScriptToFile);
                if (!responseDataKesPesalahKesalahan.Success)
                    response.Success = responseDataKesPesalahKesalahan.Success;

                var responseDataKesTindakan = await InsertTbDataKesTindakan(data.NoKes);

                var responseImage = await SendImageOnline(noRujukan, data.KodCawangan, data.Status, data.PgnDaftar.ToString(), data.TrkhDaftar, data.PgnAkhir.ToString(), data.TrkhAkhir, 1);
                if (!responseImage.Success)
                    response.Success = false;

                //var responseImage = await SendImageOnline(noRujukan, data.KodCawangan, data.Status, data.PgnDaftar.ToString(), data.TrkhDaftar, data.PgnAkhir.ToString(), data.TrkhAkhir, 1);
                //if (!responseImage.Success)
                //    response.Success = false;

            }
            return response;
        }

        public static async Task<Response<string>> UpdateDataKesAfterPaid(string noKes, string kodStatusKes, string kodStatusKesDet)
        {
            var query = $" UPDATE tbdatakes SET kodstatuskes = '{kodStatusKes}' , kodstatuskes_det = '{kodStatusKesDet}' WHERE nokes = '{noKes}'; ";
            return await HttpClientService.ExecuteQuery(query);
        }

        public static async Task<Response<string>> UpdateDataKesHHAfterPaid(string noKes, string kodStatusKes, string kodStatusKesDet)
        {
            var query = $" UPDATE tbdatakes_hh SET kodstatuskes = '{kodStatusKes}' , kodstatuskes_det = '{kodStatusKesDet}' WHERE nokes = '{noKes}'; ";
            return await HttpClientService.ExecuteQuery(query);
        }

        public static async Task<Response<string>> InsertTbDataKesTindakan(string noKes, decimal amnByr = 0)
        {
            var dataKes = DataAccessQuery<TbDataKes>.Get(m => m.NoKes == noKes);
            if (dataKes.Success & dataKes.Datas != null)
            {
                var kpp = DataAccessQuery<TbKpp>.Get(m => m.NoRujukanKpp == dataKes.Datas.NoKpp);
                var kompaund = DataAccessQuery<TbKompaun>.Get(m => m.NoKmp == dataKes.Datas.NoKmp);
                if (amnByr == 0)
                    amnByr = kompaund.Datas?.AmnByr ?? 0;

                var queryCheckTbDataKesTindakan = "";

                if (kpp.Datas?.Tindakan == Constants.Tindakan.Kots)
                {
                    var kodStatuskes = amnByr > 0 ? "S" : "BS";
                    var kodStatuskes_Det = amnByr > 0 ? "S02" : "BS04";

                    queryCheckTbDataKesTindakan = $" SELECT * FROM tbdatakes_tindakan WHERE nokes = '{dataKes.Datas.NoKes}' and kodstatuskes IN ('BS','S') ";
                    var dataExists = await HttpClientService.CheckData(queryCheckTbDataKesTindakan);
                    if (!dataExists.Success)
                    {
                        var tbDataKesTindakan = $" INSERT INTO tbdatakes_tindakan " +
                                                $" (nokes, kodcawangan, trkhtindakan, pegawaiid, catatan, kodstatuskes, kodstatuskes_det, pgndaftar, trkhdaftar, pgnakhir, trkhakhir) " +
                                                $"        SELECT nokes, kodcawangan, trkhsalah, pegawaiserbuan, '', '{kodStatuskes}', '{kodStatuskes_Det}', '{GeneralBll.GetUserStaffId()}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), '{GeneralBll.GetUserStaffId()}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}') FROM tbdatakes " +
                                                $" WHERE nokes = '{dataKes.Datas.NoKes}' AND STATUS = 1; ";

                        return await HttpClientService.ExecuteQuery(tbDataKesTindakan);
                    }
                    else
                    {
                        var tbDataKesTindakan = $" UPDATE tbdatakes_tindakan SET kodstatuskes = '{kodStatuskes}', kodstatuskes_det = '{kodStatuskes_Det}' WHERE nokes = '{dataKes.Datas.NoKes}'";
                        return await HttpClientService.ExecuteQuery(tbDataKesTindakan);
                    }
                }
                else if (kpp.Datas?.Tindakan == Constants.Tindakan.SiasatLanjutan)
                {
                    queryCheckTbDataKesTindakan = $" SELECT * FROM tbdatakes_tindakan WHERE nokes = '{dataKes.Datas.NoKes}' and kodstatuskes = 'BS' and kodstatuskes_det = 'BS01' ";
                    var dataExists = await HttpClientService.CheckData(queryCheckTbDataKesTindakan);
                    if (!dataExists.Success)
                    {

                        var tbDataKesTindakan = $" INSERT INTO tbdatakes_tindakan " +
                                                $" (nokes, kodcawangan, trkhtindakan, pegawaiid, catatan, kodstatuskes, kodstatuskes_det, pgndaftar, trkhdaftar, pgnakhir, trkhakhir) " +
                                                $"        SELECT nokes, kodcawangan, trkhsalah, pegawaiserbuan, '', 'BS', 'BS01', '{GeneralBll.GetUserStaffId()}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}'), '{GeneralBll.GetUserStaffId()}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}') FROM tbdatakes " +
                                                $" WHERE nokes = '{dataKes.Datas.NoKes}' AND STATUS = 1; ";

                        return await HttpClientService.ExecuteQuery(tbDataKesTindakan);
                    }
                }
            }
            return new Response<string> { Success = true };
        }

        public static async Task<Response<string>> SendDataKesPesalah(string noKes, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string> { Success = true };
            var query = string.Empty;
            var dataKesPesalah = DataAccessQuery<TbDataKesPesalah>.Get(m => m.NoKes == noKes);
            if (dataKesPesalah.Success && dataKesPesalah.Datas != null && dataKesPesalah.Datas.IsSendOnline != Enums.StatusOnline.Sent)
            {
                query = $"select count(*) as count from tbdatakes_pesalah where nokes = '{noKes}'";
                var responseCheckData = await HttpClientService.CountAync(query);
                Log.WriteLogFile($"Check data tbdatakes_pesalah for nokes {noKes}, {responseCheckData.Success} - {responseCheckData.Result?.Count}");
                if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                {
                    DataAccessQuery<TbDataKesPesalah>.ExecuteSql($"UPDATE tbdatakes_pesalah SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' WHERE NoKes = '{dataKesPesalah.Datas.NoKes}' ");
                }
                else
                {
                    query = GenerateSQLScriptForTableTbDataKesPesalah(dataKesPesalah.Datas);
                    var responseKesalahan = await HttpClientService.ExecuteQuery(query, context);
                    var statusOnline = responseKesalahan.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                    DataAccessQuery<TbDataKesPesalah>.ExecuteSql($"UPDATE tbdatakes_pesalah SET IsSendOnline = '{(int)statusOnline}' WHERE NoKes = '{dataKesPesalah.Datas.NoKes}' ");

                    //Save the script if have error when send api
                    if (saveScriptToFile && !responseKesalahan.Success)
                    {
                        Log.WriteErrorRecords(query);
                    }

                    if (!responseKesalahan.Success)
                        response.Success = responseKesalahan.Success;
                }
            }

            return response;
        }

        public static async Task<Response<string>> SendDataKesAsasTindakan(string noKes, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>();
            var dataKesAsasTindakan = DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSelectSql($" SELECT * FROM tbdatakes_asastindakan WHERE NoKES = '{noKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
            if (dataKesAsasTindakan.Any())
            {
                foreach (var item in dataKesAsasTindakan)
                {
                    var query = $"select count(*) as count from tbdatakes_asastindakan where nokes = '{noKes}' and kodtujuan = '{item.KodTujuan}' and kodasas = '{item.KodAsas}'";
                    var responseCheckData = await HttpClientService.CountAync(query);
                    Log.WriteLogFile($"Check data tbdatakes_asastindakan for nokes {noKes} and kodtujuan = '{item.KodTujuan}' and kodasas = '{item.KodAsas}', {responseCheckData.Success} - {responseCheckData.Result?.Count}");
                    if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                    {
                        DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSql($"UPDATE tbdatakes_asastindakan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' WHERE Id = '{item.Id}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}' ");
                    }
                }
            }



            dataKesAsasTindakan = DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSelectSql($" SELECT * FROM tbdatakes_asastindakan WHERE NoKES = '{noKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
            if (dataKesAsasTindakan.Any())
            {
                var query = "";
                var asasTindakanAfterSplit = dataKesAsasTindakan.Split();
                foreach (var dataKesAsasTindakans in asasTindakanAfterSplit)
                {
                    query = string.Empty;
                    foreach (var item in dataKesAsasTindakans)
                    {
                        query = GenerateSQLScriptForTableTbDataKesAsasTindakan(item, query);
                    }

                    if (query.EndsWith(","))
                        query = query.Remove(query.Length - 1) + ";";

                    var responseKesAsasTindakan = await HttpClientService.ExecuteQuery(query, context);
                    var statusOnline = responseKesAsasTindakan.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;

                    foreach (var item in dataKesAsasTindakans)
                    {
                        DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSql($"UPDATE tbdatakes_asastindakan SET IsSendOnline = '{(int)statusOnline}' WHERE Id = '{item.Id}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}' ");
                    }

                    //Save the script if have error when send api
                    if (saveScriptToFile && !responseKesAsasTindakan.Success)
                    {
                        Log.WriteErrorRecords(query);
                    }
                    if (!responseKesAsasTindakan.Success)
                        response.Success = false;
                }


            }
            return response;
        }

        public static async Task<Response<string>> SendDataKesKesalahan(string noKes, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>();

            var dataKesKesalahan = DataAccessQuery<TbDataKesKesalahan>.Get(m => m.NoKes == noKes);
            if (dataKesKesalahan.Success && dataKesKesalahan.Datas != null && dataKesKesalahan.Datas.IsSendOnline != Enums.StatusOnline.Sent)
            {
                var query = $"select count(*) as count from tbdatakes_kesalahan where nokes='{noKes}' and kodcawangan='{dataKesKesalahan.Datas.KodCawangan}' and kodakta = '{dataKesKesalahan.Datas.KodAkta}' and kodsalah ='{dataKesKesalahan.Datas.KodSalah}'";
                var responseCheckData = await HttpClientService.CountAync(query);
                Log.WriteLogFile($"Check data tbdatakes_kesalahan for nokes='{noKes}' and kodcawangan='{dataKesKesalahan.Datas.KodCawangan}' and kodakta = '{dataKesKesalahan.Datas.KodAkta}' and kodsalah ='{dataKesKesalahan.Datas.KodSalah}', {responseCheckData.Success} - {responseCheckData.Result?.Count}");
                if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                {
                    DataAccessQuery<TbDataKesKesalahan>.ExecuteSql($"UPDATE tbdatakes_kesalahan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' " +
                        $" WHERE NoKes = '{noKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                }
                else
                {
                    query = GenerateSQLScriptForTableTbDataKesKesalahan(dataKesKesalahan.Datas);
                    response = await HttpClientService.ExecuteQuery(query, context);
                    var statusOnline = response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                    DataAccessQuery<TbDataKesKesalahan>.ExecuteSql($"UPDATE tbdatakes_kesalahan SET IsSendOnline = '{(int)statusOnline}' " +
                        $" WHERE NoKes = '{noKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");

                    //Save the script if have error when send api
                    if (saveScriptToFile && !response.Success)
                    {
                        Log.WriteErrorRecords(query);
                        response.Success = response.Success;
                    }
                }
            }

            return response;
        }

        public static async Task<Response<string>> SendDataKesPesalahKesalahan(string noKes, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>();

            var tbDataKesPesalahAlreadySent = DataAccessQuery<TbDataKesPesalah>.GetAll(m => m.NoKes == noKes && m.IsSendOnline == Enums.StatusOnline.Sent);
            var tbDataKesKesalahanAlreadySent = DataAccessQuery<TbDataKesKesalahan>.GetAll(m => m.NoKes == noKes && m.IsSendOnline == Enums.StatusOnline.Sent);

            if (tbDataKesKesalahanAlreadySent.Datas.Any() && tbDataKesKesalahanAlreadySent.Datas.Any())
            {
                var query = $"SELECT count(*) as Count FROM tbdatakes_pesalah_kesalahan where nokes = '{noKes}'";
                var responseCheckData = await HttpClientService.CountAync(query);

                if (!responseCheckData.Success || responseCheckData.Result.Count == 0)
                {

                    var queryDataKes = $" INSERT INTO tbdatakes_pesalah_kesalahan " +
                                       $" (idpesalah, idkesalahan, kodcawangan, pgndaftar, trkhdaftar, pgnakhir, trkhakhir, nokes) " +
                                       $" SELECT a.idpesalah, b.idkesalahan, a.kodcawangan, a.pgndaftar, a.trkhdaftar, a.pgnakhir, a.trkhakhir,  a.nokes " +
                                       $" FROM tbdatakes_pesalah a, tbdatakes_kesalahan b " +
                                       $" WHERE a.nokes = b.nokes AND a.nokes = '{noKes}' AND a.STATUS = 1; ";
                    response = await HttpClientService.ExecuteQuery(queryDataKes, context);

                    if (!response.Success && saveScriptToFile)
                        Log.WriteErrorRecords(queryDataKes);
                }
            }

            return response;
        }
        #endregion


        #region Send Error Data when Checkout
        public static async Task<Response<string>> SendErrorDataAsync(string noRujukan, Enums.TableType tableType, Android.Content.Context context)
        {
            //Todo
            //Write SQL Statment to txt when send data error.
            var response = new Response<string>() { Success = false, Mesage = "Ralat" };

            if (tableType == Enums.TableType.KPP_HH || tableType == Enums.TableType.KPP)
            {
                response = await SendKppOnlineAsyncV2(noRujukan, tableType, context, true);
            }

            if (tableType == Enums.TableType.Kompaun_HH || tableType == Enums.TableType.Kompaun)
            {
                response = await SendKompaunOnlineAsyncV2(noRujukan, tableType, context, true);
            }

            if (tableType == Enums.TableType.DataKes_HH || tableType == Enums.TableType.DataKes)
            {
                response = await SendDataKesOnlineAsyncV2(noRujukan, tableType, context, true);
            }

            if (tableType == Enums.TableType.Akuan_UpdateKompaun || tableType == Enums.TableType.Akuan_UpdateKompaun_HH)
            {
                response = await SendAkuanAsync(noRujukan, tableType, context, true);
            }
            Log.WriteLogFile($"SendOnlineBLL \r SendErrorDataAsync {noRujukan}-{tableType.ToString()} \r Success: {response.Success} \r Message : {response.Mesage}");

            return response;

        }

        //public static async Task SendErrorRecordFile(string path)
        //{
        //    var fileBase64 = GeneralBll.GetBase64FromImagePath(path);

        //    var tarikhDafterInt = GeneralBll.StringDatetimeToInt(GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat));
        //    var tarikhAkhirInt = GeneralBll.StringDatetimeToInt(GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat));

        //    var options = new
        //    {
        //        nokmp = Constants.ErrorRecordsPath,
        //        kodcawangan = Constants.ErrorRecordsPath,
        //        namagambar = Path.GetFileName(path),
        //        status = Constants.Status.Aktif,
        //        pgndaftar = 1,
        //        trkhdaftar = tarikhDafterInt,
        //        pgnakhir = 1,
        //        trkhakhir = tarikhAkhirInt,
        //        //images = $"data:image/jpeg;base64,{fileBase64}",
        //        images = $"data:text/plain;base64,{fileBase64}",
        //        kategori = 4
        //    };
        //    var jsonParam = JsonConvert.SerializeObject(options);

        //    var response = await HttpClientService.UploadImage(jsonParam);
        //}
        #endregion
#if !DEBUG
        private static async Task<Response<string>> SendKppOnlineAsyncOneQuery(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>() { Success = true, Mesage = "Ralat" };
            var query = "";
            //Send TbKPP
            var tbKpp = PemeriksaanBll.GetPemeriksaanByRujukan(noRujukan);
            if (tbKpp != null)
            {
                //Move to One Query
                //if (tableType == Enums.TableType.KPP_HH)
                //{
                //    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KPP_HH && m.Status != Enums.StatusOnline.Sent);
                //    if (datas.Success && datas.Datas != null)
                //    {
                //        //Send KPP_HH
                //        query = GenerateSQLScriptForTableKpp_Hh(tbKpp);
                //        response = await HttpClientService.ExecuteQuery(query, context);
                //        //Set Data online for KPP_HH
                //        SetStatusDataOnline(noRujukan, Enums.TableType.KPP_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                //
                //        //Save the script if have error when send api
                //        if (saveScriptToFile && !response.Success)
                //        {
                //            Log.WriteErrorRecords(query);
                //            response.Success = response.Success;
                //        }
                //    }
                //}

                if (tableType == Enums.TableType.KPP)
                {
                    //Change to one query 20210224
                    ////var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KPP && m.Status != Enums.StatusOnline.Sent);
                    ////if (datas.Success && datas.Datas != null)
                    ////{
                    ////    //Send KPP
                    ////    query = GenerateSQLScriptForTableKpp(tbKpp);
                    ////    response = await HttpClientService.ExecuteQuery(query, context);
                    ////    //Set Data online for KPP
                    ////    SetStatusDataOnline(noRujukan, Enums.TableType.KPP, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                    ////    //Save the script if have error when send api
                    ////    if (saveScriptToFile && !response.Success)
                    ////    {
                    ////        Log.WriteErrorRecords(query);
                    ////        response.Success = response.Success;
                    ////    }
                    ////}
                    ///

                    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KPP && m.Status != Enums.StatusOnline.Sent);
                    if (datas.Success && datas.Datas != null)
                    {
                        //Prepare data KPP
                        query = GenerateSQLScriptForTableKpp(tbKpp);

                        //Prepare data KPP_HH
                        query += GenerateSQLScriptForTableKpp_Hh(tbKpp);

                        //Prepare data Pasukan Trans
                        var pasukanTrans = DataAccessQuery<TbPasukanTrans>.GetAll(m => m.JenisTrans == Constants.JenisTrans.Kpp && m.IsSendOnline != Enums.StatusOnline.Sent && m.NoRujukan == noRujukan);
                        if (pasukanTrans.Success)
                        {
                            var queryPasukanTrans = "";
                            foreach (var item in pasukanTrans.Datas)
                            {
                                queryPasukanTrans += GenerateSQLScriptForTablePasukan_Trans(item, queryPasukanTrans);
                            }

                            if (queryPasukanTrans.EndsWith(","))
                                queryPasukanTrans = queryPasukanTrans.Remove(queryPasukanTrans.Length - 1) + ";";

                            query += queryPasukanTrans;
                        }

                        //Prepare data Asas Tindakan
                        var asasTindakan = DataAccessQuery<TbKppAsasTindakan>.GetAll(m => m.NoRujukanKpp == noRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
                        if (asasTindakan.Success)
                        {
                            var queryAsasTindakan = "";
                            foreach (var item in asasTindakan.Datas)
                            {
                                queryAsasTindakan = GenerateScriptAsasTindakan(item, queryAsasTindakan);
                            }

                            if (queryAsasTindakan.EndsWith(","))
                                queryAsasTindakan = queryAsasTindakan.Remove(queryAsasTindakan.Length - 1) + ";";

                            query += queryAsasTindakan;
                        }
                        //Send Query for KPP, Pasukan Trans and Asas Tindakan
                        response = await HttpClientService.ExecuteQuery(query, context);

                        //When Response is Success it will be update Status Data Online to Sent for:
                        if (response.Success)
                        {
                            //KPP
                            SetStatusDataOnline(noRujukan, Enums.TableType.KPP, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                            //KPP_HH
                            SetStatusDataOnline(noRujukan, Enums.TableType.KPP_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                            //Pasukan Trans
                            if (pasukanTrans.Success)
                            {
                                foreach (var item in pasukanTrans.Datas)
                                {
                                    SetStatusOnlinePasukanTrans(item.Id, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error, item.NoRujukan);
                                }
                            }

                            //Asas Tindakan
                            if (asasTindakan.Success)
                            {
                                foreach (var item in asasTindakan.Datas)
                                {
                                    var statusOnline = response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                                    DataAccessQuery<TbKppAsasTindakan>.ExecuteSql($"UPDATE tbkpp_asastindakan SET IsSendOnline = '{(int)statusOnline}' WHERE KodTujuan = '{item.KodTujuan}' AND KodAsas = '{item.KodAsas}' AND NoRujukanKpp = '{noRujukan}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                                }
                            }
                        }

                    }
                }

                //Send Pasukan Trans
                //Move to one Query await SendPasukanTrans(noRujukan, context, saveScriptToFile);

                //Send Images
                await SendImageOnline(noRujukan, tbKpp.KodCawangan, tbKpp.Status, tbKpp.PgnDaftar.ToString(), tbKpp.TrkhDaftar, tbKpp.PgnAkhir.ToString(), tbKpp.TrkhAkhir, 2);

                //Send Asas Tindakan
                //Move to one Query await SendAsasTindakan(noRujukan, saveScriptToFile, context);
            }

            return response;
        }
        private static async Task<Response<string>> SendKompaunOnlineAsyncOneQuery(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>() { Success = true, Mesage = "Ralat" };
            var query = "";
            var result = KompaunBll.GetKompaunByRujukan(noRujukan);
            if (result.Success)
            {
                if (result.Datas != null)
                {
                    //Move to One Query
                    //if (tableType == Enums.TableType.Kompaun_HH)
                    //{
                    //    var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Kompaun_HH && m.Status != Enums.StatusOnline.Sent);
                    //    if (dataKompaund.Success && dataKompaund.Datas != null)
                    //    {
                    //        //Send Kompaun_HH
                    //        query = GenerateSQLScriptForTableKompaun_Hh(result.Datas);
                    //        response = await HttpClientService.ExecuteQuery(query, context);
                    //        //Set Data online for Kompaun_HH
                    //        SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                    //
                    //        //Save the script if have error when send api
                    //        if (saveScriptToFile && !response.Success)
                    //        {
                    //            Log.WriteErrorRecords(query);
                    //            response.Success = response.Success;
                    //        }
                    //    }
                    //}
                    if (tableType == Enums.TableType.Kompaun)
                    {
                        ////var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Kompaun && m.Status != Enums.StatusOnline.Sent);
                        ////if (dataKompaund.Success && dataKompaund.Datas != null)
                        ////{
                        ////    //Send Kompaun
                        ////    query = GenerateSQLScriptForTableKompaun(result.Datas);
                        ////    response = await HttpClientService.ExecuteQuery(query, context);
                        ////    //Set Data online for Kompaun
                        ////    SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                        ////    //Save the script if have error when send api
                        ////    if (saveScriptToFile && !response.Success)
                        ////    {
                        ////        Log.WriteErrorRecords(query);
                        ////        response.Success = response.Success;
                        ////    }
                        ////}
                        ///

                        var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Kompaun && m.Status != Enums.StatusOnline.Sent);
                        if (dataKompaund.Success && dataKompaund.Datas != null)
                        {
                            //Send Kompaun
                            query = GenerateSQLScriptForTableKompaun(result.Datas);
                            //Send Kompaun_HH
                            query += GenerateSQLScriptForTableKompaun_Hh(result.Datas);

                            response = await HttpClientService.ExecuteQuery(query, context);
                            //Set Data online for Kompaun
                            SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                            SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                        }
                    }

                    await SendImageOnline(noRujukan, result.Datas.KodCawangan, result.Datas.Status, result.Datas.PgnDaftar.ToString(), result.Datas.TrkhDaftar, result.Datas.PgnAkhir.ToString(), result.Datas.TrkhAkhir, 3);

                    //When Send Kompaun, it need send TbDataKes, KesKesalahan, KesAsasTindakan, KesAsasPesalah also, 
                    //becuase on the KompaunBLL, After Insert Kompaun it will save data to TbDataKes, KesKesalahan, KesAsasTindakan, KesAsasPesalah also
                    //CODE MOVED TO CheckDataKesBasedNoRujukan

                    //var tbDateKes = DataAccessQuery<TbDataKes>.Get(c => c.NoKmp == noRujukan);
                    //if (tbDateKes.Success && tbDateKes.Datas != null)
                    //{
                    //    var dataKesFromKompaun = DataAccessQuery<TbSendOnlineData>.Get(
                    //        m => m.NoRujukan == tbDateKes.Datas.NoKes &&
                    //        m.Type == Enums.TableType.DataKes &&
                    //        m.Status != Enums.StatusOnline.Sent);
                    //    if (dataKesFromKompaun.Success && dataKesFromKompaun.Datas != null)
                    //        await SendDataKesOnlineAsyncV2(dataKesFromKompaun.Datas.NoRujukan, Enums.TableType.DataKes, context);
                    //
                    //    dataKesFromKompaun = DataAccessQuery<TbSendOnlineData>.Get(
                    //        m => m.NoRujukan == tbDateKes.Datas.NoKes &&
                    //        m.Type == Enums.TableType.DataKes_HH &&
                    //        m.Status != Enums.StatusOnline.Sent);
                    //    if (dataKesFromKompaun.Success && dataKesFromKompaun.Datas != null)
                    //        await SendDataKesOnlineAsyncV2(dataKesFromKompaun.Datas.NoRujukan, Enums.TableType.DataKes_HH, context);
                    //}
                }
            }

            return response;
        }
        private static async Task<Response<string>> SendDataKesOnlineAsyncOneQuery(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>() { Success = true, Mesage = "Ralat" };
            var query = "";
            var data = KompaunBll.GetSiasatByNoKes(noRujukan);
            if (data != null)
            {
                if (tableType == Enums.TableType.DataKes)
                {
                    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.DataKes && m.Status != Enums.StatusOnline.Sent);
                    if (datas.Success && datas.Datas != null)
                    {
                        //Send DataKes
                        query = GenerateSQLScriptForTableDataKes(data);
                        //Send DataKes_HH
                        query += GenerateSQLScriptForTableDataKes_Hh(data);
                        //Send TbDataKesPesalah
                        var dataKesPesalah = DataAccessQuery<TbDataKesPesalah>.Get(m => m.NoKes == data.NoKes && m.IsSendOnline != Enums.StatusOnline.Sent);
                        if (dataKesPesalah.Success && dataKesPesalah.Datas != null)
                        {
                            query += GenerateSQLScriptForTableTbDataKesPesalah(dataKesPesalah.Datas);
                        }
                        //Send TbDataKesAsasTindakan
                        var dataKesAsasTindakan = DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSelectSql($" SELECT * FROM tbdatakes_asastindakan WHERE NoKES = '{data.NoKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                        if (dataKesAsasTindakan.Any())
                        {
                            foreach (var item in dataKesAsasTindakan)
                            {
                                query = GenerateSQLScriptForTableTbDataKesAsasTindakan(item, query);
                            }

                            if (query.EndsWith(","))
                                query = query.Remove(query.Length - 1) + ";";
                        }
                        //Send TbDataKesKesalahan
                        var dataKesKesalahan = DataAccessQuery<TbDataKesKesalahan>.Get(m => m.NoKes == data.NoKes && m.IsSendOnline != Enums.StatusOnline.Sent);
                        if (dataKesKesalahan.Success && dataKesKesalahan.Datas != null)
                        {
                            query += GenerateSQLScriptForTableTbDataKesKesalahan(dataKesKesalahan.Datas);
                        }

                        response = await HttpClientService.ExecuteQuery(query, context);

                        //Set Data online for DataKes
                        SetStatusDataOnline(noRujukan, Enums.TableType.DataKes, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                        //Set Data online for DataKes_HH
                        SetStatusDataOnline(noRujukan, Enums.TableType.DataKes_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                        //Set Data online for TbDataKesPesalah
                        if (dataKesPesalah.Success && dataKesPesalah.Datas != null)
                        {
                            var statusOnline = response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                            DataAccessQuery<TbDataKesPesalah>.ExecuteSql($"UPDATE tbdatakes_pesalah SET IsSendOnline = '{(int)statusOnline}' " +
                                $" WHERE NoKes = '{dataKesPesalah.Datas.NoKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                        }
                        //Set Data online for DataKesAsasTindakan
                        if (dataKesAsasTindakan.Any())
                        {
                            foreach (var item in dataKesAsasTindakan)
                            {
                                var statusOnline = response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                                DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSql($"UPDATE tbdatakes_asastindakan SET IsSendOnline = '{(int)statusOnline}' WHERE Id = '{item.Id}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                            }
                        }
                        //Set Data online for DataKesKesalahan
                        if (dataKesKesalahan.Success && dataKesKesalahan.Datas != null)
                        {
                            var statusOnline = response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                            DataAccessQuery<TbDataKesKesalahan>.ExecuteSql($"UPDATE tbdatakes_kesalahan SET IsSendOnline = '{(int)statusOnline}' " +
                                $" WHERE NoKes = '{dataKesPesalah.Datas.NoKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                        }
                    }
                }

                //if (tableType == Enums.TableType.DataKes_HH)
                //{
                //    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.DataKes_HH && m.Status != Enums.StatusOnline.Sent);
                //    if (datas.Success && datas.Datas != null)
                //    {
                //        //Send DataKes_HH
                //        query = GenerateSQLScriptForTableDataKes_Hh(data);
                //        response = await HttpClientService.ExecuteQuery(query, context);
                //        //Set Data online for DataKes_HH
                //        SetStatusDataOnline(noRujukan, Enums.TableType.DataKes_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                //
                //        //Save the script if have error when send api
                //        if (saveScriptToFile && !response.Success)
                //        {
                //            Log.WriteErrorRecords(query);
                //            response.Success = response.Success;
                //        }
                //    }
                //}
                //if (tableType == Enums.TableType.DataKes)
                //{
                //    var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.DataKes && m.Status != Enums.StatusOnline.Sent);
                //    if (datas.Success && datas.Datas != null)
                //    {
                //        //Send DataKes
                //        query = GenerateSQLScriptForTableDataKes(data);
                //        response = await HttpClientService.ExecuteQuery(query, context);
                //        //Set Data online for DataKes
                //        SetStatusDataOnline(noRujukan, Enums.TableType.DataKes, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                //
                //        //Save the script if have error when send api
                //        if (saveScriptToFile && !response.Success)
                //        {
                //            Log.WriteErrorRecords(query);
                //            response.Success = response.Success;
                //        }
                //    }
                //}
                //
                ////Send TbDataKesPesalah
                //var dataKesPesalah = DataAccessQuery<TbDataKesPesalah>.Get(m => m.NoKes == data.NoKes);
                //if (dataKesPesalah.Success && dataKesPesalah.Datas != null && dataKesPesalah.Datas.IsSendOnline != Enums.StatusOnline.Sent)
                //{
                //    query = GenerateSQLScriptForTableTbDataKesPesalah(dataKesPesalah.Datas);
                //    var responseKesalahan = await HttpClientService.ExecuteQuery(query, context);
                //    var statusOnline = responseKesalahan.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                //    DataAccessQuery<TbDataKesPesalah>.ExecuteSql($"UPDATE tbdatakes_pesalah SET IsSendOnline = '{(int)statusOnline}' " +
                //        $" WHERE NoKes = '{dataKesPesalah.Datas.NoKes}' ");
                //
                //    //Save the script if have error when send api
                //    if (saveScriptToFile && !response.Success)
                //    {
                //        Log.WriteErrorRecords(query);
                //        response.Success = response.Success;
                //    }
                //}

                //Send TbDataKesAsasTindakan
                //var dataKesAsasTindakan = DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSelectSql($" SELECT * FROM tbdatakes_asastindakan WHERE NoKES = '{data.NoKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                //if (dataKesAsasTindakan.Any())
                //{
                //    foreach (var item in dataKesAsasTindakan)
                //    {
                //        query = GenerateSQLScriptForTableTbDataKesAsasTindakan(item);
                //        var responseKesAsasTindakan = await HttpClientService.ExecuteQuery(query, context);
                //        var statusOnline = responseKesAsasTindakan.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                //        DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSql($"UPDATE tbdatakes_asastindakan SET IsSendOnline = '{(int)statusOnline}' WHERE Id = '{item.Id}' ");
                //
                //        //Save the script if have error when send api
                //        if (saveScriptToFile && !response.Success)
                //        {
                //            Log.WriteErrorRecords(query);
                //            response.Success = response.Success;
                //        }
                //    }
                //}

                //Send TbDataKesKesalahan
                //var dataKesKesalahan = DataAccessQuery<TbDataKesKesalahan>.Get(m => m.NoKes == data.NoKes);
                //if (dataKesKesalahan.Success && dataKesKesalahan.Datas != null && dataKesKesalahan.Datas.IsSendOnline != Enums.StatusOnline.Sent)
                //{
                //    query = GenerateSQLScriptForTableTbDataKesKesalahan(dataKesKesalahan.Datas);
                //    var responseKesKesalahan = await HttpClientService.ExecuteQuery(query, context);
                //    var statusOnline = responseKesKesalahan.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                //    DataAccessQuery<TbDataKesKesalahan>.ExecuteSql($"UPDATE tbdatakes_kesalahan SET IsSendOnline = '{(int)statusOnline}' " +
                //        $" WHERE NoKes = '{dataKesPesalah.Datas.NoKes}' ");
                //
                //    //Save the script if have error when send api
                //    if (saveScriptToFile && !response.Success)
                //    {
                //        Log.WriteErrorRecords(query);
                //        response.Success = response.Success;
                //    }
                //}

                //Comment First :
                // //Insert tbdatakes_pesalah_kesalahan
                // //Check existing Data
                // var dataKesPesalahKesalahan = DataAccessQuery<TbDataKesPesalahKesalahan>.Get(m => m.NoKes == data.NoKes);
                // if (dataKesPesalahKesalahan.Success && dataKesPesalahKesalahan.Datas == null)
                // {
                //     //Insert data
                //     DataAccessQuery<TbDataKesPesalahKesalahan>.Insert(new TbDataKesPesalahKesalahan
                //     {
                //         NoKes = data.NoKes,
                //         NoRujukan = noRujukan,
                //         Status = Enums.StatusOnline.New,
                //         CreatedDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat)
                //     });
                // }
                // 
                // var dataKesPesalahKesalahans = DataAccessQuery<TbDataKesPesalahKesalahan>.GetAll(m => m.NoKes == data.NoKes && m.NoRujukan == noRujukan && m.Status != Enums.StatusOnline.Sent);
                // if (dataKesPesalahKesalahans.Success && dataKesPesalahKesalahans.Datas.Any())
                // {
                //     foreach (var item in dataKesPesalahKesalahans.Datas)
                //     {
                //         var pesalah = await HttpClientService.GetDataKesPesalahBasedNoKes(data.NoKes);
                //         if (pesalah.Success && pesalah.Result.Any())
                //         {
                //             var kesalahan = await HttpClientService.GetDataKesKesalahanBasedNoKes(data.NoKes);
                //             if (kesalahan.Success && kesalahan.Result.Any())
                //             {
                //                 var sql = "INSERT INTO tbdatakes_pesalah_kesalahan (nokes, idpesalah, idkesalahan, kodcawangan, " +
                //                           " pgndaftar, trkhdaftar, pgnakhir, trkhakhir)values" +
                //                           $"('{data.NoKes}', {pesalah.Result.FirstOrDefault()?.IdPesalah}, {kesalahan.Result.FirstOrDefault()?.IdKesalahan}, '{data.KodCawangan}'," +
                //                           $"'{data.PgnDaftar}', UNIX_TIMESTAMP('{data.TrkhDaftar}'), '{data.PgnAkhir}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')";
                // 
                //                 var responseKesPesalahKesalahan = await HttpClientService.ExecuteQuery(query, context);
                //                 item.Status = responseKesPesalahKesalahan.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error;
                //                 item.UpdateDate = GeneralBll.GetLocalDateTime().ToString(Constants.DatabaseDateFormat);
                // 
                //                 DataAccessQuery<TbDataKesPesalahKesalahan>.Update(item);
                //             }
                //         }
                //     }
                // }

                await SendImageOnline(noRujukan, data.KodCawangan, data.Status, data.PgnDaftar.ToString(), data.TrkhDaftar, data.PgnAkhir.ToString(), data.TrkhAkhir, 1);

            }
            return response;
        }
        private static async Task<Response<string>> SendAkuanAsyncOneQuery(string noRujukan, Enums.TableType tableType, Android.Content.Context context, bool saveScriptToFile = false)
        {
            var response = new Response<string>() { Success = true, Mesage = "" };

            var kompaun = KompaunBll.GetKompaunByRujukan(noRujukan);
            if (kompaun.Success)
            {
                if (kompaun.Datas != null)
                {
                    if (kompaun.Datas.IsCetakAkuan == Constants.CetakAkuan.Yes)
                    {
                        var isKompaunHasSent = false;
                        var data = DataAccessQuery<TbSendOnlineData>.GetAll();
                        if (data.Success)
                        {
                            var kompauns = data.Datas.Where(m => m.NoRujukan == noRujukan
                                                              && m.Status == Enums.StatusOnline.Sent
                                                              && (m.Type == Enums.TableType.Kompaun || m.Type == Enums.TableType.Kompaun_HH));
                            if (kompauns.Count() == 2)
                                isKompaunHasSent = true;
                        }

                        // we will send KompaunBayaran, Akuan_UpdateKompaun and Akuan_UpdateKompaun_HH IF KOMPAUN AND KOMPAUN HAS BEEN SENT
                        if (!isKompaunHasSent)
                            return response;

                        if (tableType == Enums.TableType.KompaunBayaran)
                        {
                            var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KompaunBayaran && m.Status != Enums.StatusOnline.Sent);
                            if (dataKompaund.Success && dataKompaund.Datas != null)
                            {
                                var trkhBayar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
                                if (!string.IsNullOrEmpty(kompaun.Datas.TrkhPenerima_Akuan))
                                    trkhBayar = kompaun.Datas.TrkhPenerima_Akuan;

                                //Send Kompaun Bayaran
                                var sqlQuery = " insert into tbkompaun_bayaran (kodcawangan,nokmp,trkhbyr,amnbyr,noresit,status,pgndaftar, " +
                                               " trkhdaftar,pgnakhir,trkhakhir) VALUES " +
                                               $" ('{kompaun.Datas.KodCawangan}', '{kompaun.Datas.NoKmp}', '{trkhBayar}','{kompaun.Datas.AmnByr}','{kompaun.Datas.NoResit.ReplaceSingleQuote()}', '2', '{kompaun.Datas.PgnDaftar}'," +
                                               $" UNIX_TIMESTAMP('{kompaun.Datas.TrkhDaftar}'), '{kompaun.Datas.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')); ";


                                //Send Akuan_UpdateKompaun
                                var akuan_UpdateKompaun = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Akuan_UpdateKompaun && m.Status != Enums.StatusOnline.Sent);
                                if (akuan_UpdateKompaun.Success && akuan_UpdateKompaun.Datas != null)
                                {
                                    sqlQuery += $" update tbkompaun set namapenerima_akuan = '{kompaun.Datas.NamaPenerima_Akuan.ReplaceSingleQuote()}', nokppenerima1_Akuan = '{kompaun.Datas.NoKpPenerima_Akuan.ReplaceSingleQuote()}', alamatpenerima1_akuan = '{kompaun.Datas.AlamatPenerima1_Akuan.ReplaceSingleQuote()}', " +
                                               $" alamatpenerima2_akuan = '{kompaun.Datas.AlamatPenerima2_Akuan.ReplaceSingleQuote()}', alamatpenerima3_akuan = '{kompaun.Datas.AlamatPenerima3_Akuan.ReplaceSingleQuote()}', trkhpenerima_akuan = '{trkhBayar}', amnbyr = '{kompaun.Datas.AmnByr}', " +
                                               $" noresit ='{kompaun.Datas.NoResit.ReplaceSingleQuote()}', trkhakhir = unix_timestamp('{GeneralBll.GetLocalDateTimeForDatabase()}'), status = 3 where nokmp = '{noRujukan}' and iscetakakuan = 1; ";
                                }

                                //Send Akuan_UpdateKompaun_HH
                                var akuan_UpdateKompaun_HH = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Akuan_UpdateKompaun_HH && m.Status != Enums.StatusOnline.Sent);
                                if (akuan_UpdateKompaun_HH.Success && akuan_UpdateKompaun_HH.Datas != null)
                                {
                                    sqlQuery += $" update tbkompaun_hh set namapenerima_akuan = '{kompaun.Datas.NamaPenerima_Akuan.ReplaceSingleQuote()}', nokppenerima1_Akuan = '{kompaun.Datas.NoKpPenerima_Akuan.ReplaceSingleQuote()}', alamatpenerima1_akuan = '{kompaun.Datas.AlamatPenerima1_Akuan.ReplaceSingleQuote()}', " +
                                               $" alamatpenerima2_akuan = '{kompaun.Datas.AlamatPenerima2_Akuan.ReplaceSingleQuote()}', alamatpenerima3_akuan = '{kompaun.Datas.AlamatPenerima3_Akuan.ReplaceSingleQuote()}', trkhpenerima_akuan = '{trkhBayar}', amnbyr = '{kompaun.Datas.AmnByr}', " +
                                               $" noresit ='{kompaun.Datas.NoResit.ReplaceSingleQuote()}', trkhakhir = unix_timestamp('{GeneralBll.GetLocalDateTimeForDatabase()}'), status = 3 where nokmp = '{noRujukan}' and iscetakakuan = 1; ";
                                }

                                response = await HttpClientService.ExecuteQuery(sqlQuery, context);

                                //Set Data online for Kompaun Bayaran
                                SetStatusDataOnline(noRujukan, Enums.TableType.KompaunBayaran, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                                //Set Data online for Akuan_UpdateKompaun
                                SetStatusDataOnline(noRujukan, Enums.TableType.Akuan_UpdateKompaun, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                                //Set Data online for Akuan_UpdateKompaun_HH
                                SetStatusDataOnline(noRujukan, Enums.TableType.Akuan_UpdateKompaun_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);

                            }
                            else
                            {
                                response.Success = true;
                                return response;
                            }
                        }

                        //if (tableType == Enums.TableType.KompaunBayaran)
                        //{
                        //    var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.KompaunBayaran && m.Status != Enums.StatusOnline.Sent);
                        //    if (dataKompaund.Success && dataKompaund.Datas != null)
                        //    {
                        //        var trkhBayar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
                        //        if (!string.IsNullOrEmpty(kompaun.Datas.TrkhPenerima_Akuan))
                        //            trkhBayar = kompaun.Datas.TrkhPenerima_Akuan;

                        //        var sqlQuery = " insert into tbkompaun_bayaran (kodcawangan,nokmp,trkhbyr,amnbyr,noresit,status,pgndaftar, " +
                        //                       " trkhdaftar,pgnakhir,trkhakhir) VALUES " +
                        //                       $" ('{kompaun.Datas.KodCawangan}', '{kompaun.Datas.NoKmp}', '{trkhBayar}','{kompaun.Datas.AmnByr}','{kompaun.Datas.NoResit.ReplaceSingleQuote()}', '2', '{kompaun.Datas.PgnDaftar}'," +
                        //                       $" UNIX_TIMESTAMP('{kompaun.Datas.TrkhDaftar}'), '{kompaun.Datas.PgnDaftar}', UNIX_TIMESTAMP('{GeneralBll.GetLocalDateTimeForDatabase()}')); ";
                        //        //Send Kompaun Bayaran
                        //        response = await HttpClientService.ExecuteQuery(sqlQuery, context);
                        //        //Set Data online for Kompaun Bayaran
                        //        SetStatusDataOnline(noRujukan, Enums.TableType.KompaunBayaran, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                        //        //Save the script if have error when send api
                        //        if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(sqlQuery);
                        //        return response;
                        //    }
                        //    else
                        //    {
                        //        response.Success = true;
                        //        return response;
                        //    }
                        //}

                        //var datas = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Status == Enums.StatusOnline.Sent
                        //            && m.Type == Enums.TableType.KompaunBayaran);
                        //if (datas.Success)
                        //{
                        //    if (datas.Datas != null && datas.Datas.Status == Enums.StatusOnline.Sent)
                        //    {
                        //        var trkhBayar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
                        //        if (!string.IsNullOrEmpty(kompaun.Datas.TrkhPenerima_Akuan))
                        //            trkhBayar = kompaun.Datas.TrkhPenerima_Akuan;

                        //        if (tableType == Enums.TableType.Akuan_UpdateKompaun)
                        //        {
                        //            var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Akuan_UpdateKompaun && m.Status != Enums.StatusOnline.Sent);
                        //            if (dataKompaund.Success && dataKompaund.Datas != null)
                        //            {
                        //                var sqlQuery = $" update tbkompaun set namapenerima_akuan = '{kompaun.Datas.NamaPenerima_Akuan.ReplaceSingleQuote()}', nokppenerima1_Akuan = '{kompaun.Datas.NoKpPenerima_Akuan.ReplaceSingleQuote()}', alamatpenerima1_akuan = '{kompaun.Datas.AlamatPenerima1_Akuan.ReplaceSingleQuote()}', " +
                        //                           $" alamatpenerima2_akuan = '{kompaun.Datas.AlamatPenerima2_Akuan.ReplaceSingleQuote()}', alamatpenerima3_akuan = '{kompaun.Datas.AlamatPenerima3_Akuan.ReplaceSingleQuote()}', trkhpenerima_akuan = '{trkhBayar}', amnbyr = '{kompaun.Datas.AmnByr}', " +
                        //                           $" noresit ='{kompaun.Datas.NoResit.ReplaceSingleQuote()}', trkhakhir = unix_timestamp('{GeneralBll.GetLocalDateTimeForDatabase()}'), status = 3 where nokmp = '{noRujukan}' and iscetakakuan = 1 ";
                        //                //Send Kompaun Bayaran
                        //                response = await HttpClientService.ExecuteQuery(sqlQuery, context);
                        //                //Set Data online for Kompaun Bayaran
                        //                SetStatusDataOnline(noRujukan, Enums.TableType.Akuan_UpdateKompaun, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                        //                //Save the script if have error when send api
                        //                if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(sqlQuery);
                        //            }
                        //        }
                        //        if (tableType == Enums.TableType.Akuan_UpdateKompaun_HH)
                        //        {
                        //            var dataKompaund = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == noRujukan && m.Type == Enums.TableType.Akuan_UpdateKompaun_HH && m.Status != Enums.StatusOnline.Sent);
                        //            if (dataKompaund.Success && dataKompaund.Datas != null)
                        //            {
                        //                var sqlQuery = $" update tbkompaun_hh set namapenerima_akuan = '{kompaun.Datas.NamaPenerima_Akuan.ReplaceSingleQuote()}', nokppenerima1_Akuan = '{kompaun.Datas.NoKpPenerima_Akuan.ReplaceSingleQuote()}', alamatpenerima1_akuan = '{kompaun.Datas.AlamatPenerima1_Akuan.ReplaceSingleQuote()}', " +
                        //                           $" alamatpenerima2_akuan = '{kompaun.Datas.AlamatPenerima2_Akuan.ReplaceSingleQuote()}', alamatpenerima3_akuan = '{kompaun.Datas.AlamatPenerima3_Akuan.ReplaceSingleQuote()}', trkhpenerima_akuan = '{trkhBayar}', amnbyr = '{kompaun.Datas.AmnByr}', " +
                        //                           $" noresit ='{kompaun.Datas.NoResit.ReplaceSingleQuote()}', trkhakhir = unix_timestamp('{GeneralBll.GetLocalDateTimeForDatabase()}'), status = 3 where nokmp = '{noRujukan}' and iscetakakuan = 1 ";
                        //                //Send Kompaun Bayaran
                        //                response = await HttpClientService.ExecuteQuery(sqlQuery, context);
                        //                //Set Data online for Kompaun Bayaran
                        //                //Save the script if have error when send api
                        //                if (saveScriptToFile && !response.Success) Log.WriteErrorRecords(sqlQuery);
                        //                SetStatusDataOnline(noRujukan, Enums.TableType.Akuan_UpdateKompaun_HH, response.Success ? Enums.StatusOnline.Sent : Enums.StatusOnline.Error);
                        //            }
                        //        }
                        //    }

                        //}
                    }
                }
            }

            return response;

        }
#endif
    }
}