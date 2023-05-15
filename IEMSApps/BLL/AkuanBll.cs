using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Inputs;
using IEMSApps.BusinessObject.Responses;
using IEMSApps.Classes;
using IEMSApps.Fragments;
using IEMSApps.Services;
using IEMSApps.Utils;
using Java.Lang;
using Newtonsoft.Json;

namespace IEMSApps.BLL
{
    public static class AkuanBll
    {
        public static Response<CheckIPResitsResponse> CheckServiceReceiptIP(string noRujukan, Android.Content.Context context)
        {
            var result = new Response<CheckIPResitsResponse>();

            if (!GeneralAndroidClass.IsOnline())
            {
                result.Success = false;
                result.Mesage = Constants.ErrorMessages.NoInternetConnection;
                return result;
            }

            //call service
            result = Task.Run(async () => await HttpClientService.CheckReceiptOnServer(noRujukan)).Result;
            if (result.Success)
            {
                InsertIpResits(noRujukan, result.Result);
            }
            else
            {
                if (result.Mesage == Constants.ErrorMessages.NotFound)
                {
                    result.Mesage = Constants.Messages.NoReceiptOnServer;
                    result.Success = false;
                }
            }

            return result;
        }

        public static void InsertIpResits(string noRujukan, CheckIPResitsResponse result)
        {
            var dataResponse = new ip_resits 
            {
                norujukankpp = result.norujukankpp,
                diterima_drpd = result.diterima_drpd,
                byrn_bg_pihak = result.byrn_bg_pihak,
                no_identiti = result.no_identiti,
                alamat_1 = result.alamat_1,
                alamat_2 = result.alamat_2,
                alamat_3 = result.alamat_3,
                poskod = result.poskod,
                bandar = result.bandar,
                negeri = result.negeri,     
                emel   = result.emel,
                no_rujukan_ipayment = result.emel,
                perihal = result.perihal, 
                no_resit = result.no_resit,
                //tarikh_bayaran = result.t
                mod_pembayaran = result.mod_pembayaran,
                rangkaian = result.rangkaian,
                no_transaksi_ipayment = result.no_transaksi_ipayment,
                no_transaksi_rma = result.no_transaksi_rma,
                amaun = result.amaun,
                diskaun = result.diskaun,
                amaun_dgn_diskaun = result.amaun_dgn_diskaun,
                amaun_cukai = result.amaun_cukai,
                amaun_dgn_cukai = result.amaun_dgn_cukai,
                pelarasan_penggenapan = result.pelarasan_penggenapan,
                jumlah_bayaran = result.jumlah_bayaran,
                keterangan = result.keterangan,
                no_rujukan = result.no_rujukan,
                kod_penjenisan = result.kod_penjenisan,
                kod_akaun  = result.kod_akaun,
                jumlah = result.jumlah,
                pusat_terimaan = result.pusat_terimaan,
                petugas = result.petugas

            };

            var data = DataAccessQuery<ip_resits>.Get(c => c.norujukankpp == noRujukan);
            if (data.Success && data.Datas == null)
            {
                DataAccessQuery<ip_resits>.Insert(dataResponse);
            }
        }

        public static Result<ip_resits> CheckIpResitsData(string noRujukan)
        {
            var data = DataAccessQuery<ip_resits>.Get(c => c.norujukankpp == noRujukan);
            return data;
        }

        public static ip_resits GetIpResitsByKPP (string noRujukan)
        {
            Result<ip_resits> data;
            data = DataAccessQuery<ip_resits>.Get(c => c.norujukankpp == noRujukan);
            return data.Datas;
        }

        public static bool SavePusatTerimaanTrx(SaveAkuanInput input, DataAccessQueryTrx insAccess = null)
        {
            var dataKompaun = GetKompaunByRujukan(input.NoRujukan);
            {
                var data = new TbKompaunBayaran
                {
                    kodcawangan = dataKompaun.KodCawangan,
                    nokmp = dataKompaun.NoKmp,
                    amnbyr = dataKompaun.AmnKmp.ToString(),
                    pusat_terimaan = input.pusat_terimaan.ToString(),
                };
                if (insAccess != null)
                {
                    return insAccess.InsertTrx(data);
                }
                var result = DataAccessQuery<TbKompaunBayaran>.Insert(data);
            }
            return false;
        }

        public static TbKompaun GetKompaunByRujukan(string noRujukan)
        {
            var data = DataAccessQuery<TbKompaun>.Get(c => c.NoKmp == noRujukan);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }

            return null;
        }

        public static TbKompaunBayaran GetKompaunBayaranByKompaun(string noKmp)
        {
            var data = DataAccessQuery<TbKompaunBayaran>.Get(c => c.nokmp == noKmp);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }

            return null;
        }

        public static ip_chargelines GetPejabatByKompaun(int idPusatTerimaan)
        {
            var data = DataAccessQuery<ip_chargelines>.Get(c => c.id == idPusatTerimaan);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }

            return null;
        }
    }
}