﻿using System;
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
                try
                {
                    InsertIpResits(noRujukan, result.Result);
                }
                catch (System.Exception ex) 
                {
                    Log.WriteLogFile("AkuanBll", "CheckServiceReceiptIP", ex.Message, Enums.LogType.Error);
                    Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);
                }
            }
            else
            {
                result.Mesage = result.Mesage;
                result.Success = false;
            }

            return result;
        }

        public static void InsertIpResits(string noRujukan, CheckIPResitsResponse result)
        {
            var dataResponse = new ip_resits
            {
                norujukankpp = result.norujukankpp,
                diterima_drpd = result.diterima_drpd,
                no_identiti = result.no_identiti,
                emel = result.emel,
                alamat_1 = result.alamat_1,
                alamat_2 = result.alamat_2,
                alamat_3 = result.alamat_3,
                poskod = result.poskod,
                bandar = result.bandar,
                negeri = result.negeri,
                no_resit = result.no_resit,
                no_rujukan = result.no_rujukan,
                no_transaksi_ipayment = result.no_transaksi_ipayment,
                no_transaksi_rma = result.no_transaksi_rma,
                mod_pembayaran = result.mod_pembayaran,
                rangkaian = result.rangkaian,
                tarikh_bayaran = result.tarikh_bayaran, 
                perihal = result.perihal,
                keterangan = result.keterangan,
                kod_akaun = result.kod_akaun,
                jumlah = result.jumlah,
                jumlah_bayaran = result.jumlah_bayaran,
                jumlah_bayaran_words = result.jumlah_bayaran_words

            };
            var data = DataAccessQuery<ip_resits>.Get(c => c.norujukankpp == noRujukan);
            if (data.Success && data.Datas == null)
            {
                Result<ip_resits> insertResult = DataAccessQuery<ip_resits>.Insert(dataResponse);
                if (!insertResult.Success) 
                {
                    Log.WriteLogFile("AkuanBll", "InsertIpResits", insertResult.Message, Enums.LogType.Error);
                }
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

        public static ip_chargelines GetIdByKodCawangan(string kodcawangan)
        {
            var data = DataAccessQuery<ip_chargelines>.Get(c => c.kodcawangan == kodcawangan);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }

            return null;
        }
    }
}