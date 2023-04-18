using System;
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
                    result.Mesage = Constants.ErrorMessages.NotFound;
                    result.Success = false;
                }
            }

            return result;
        }

        public static void InsertIpResits(string noRujukan, CheckIPResitsResponse result)
        {
            var data = DataAccessQuery<ip_resits>.Get(c => c.no_rujukan_ipayment == noRujukan);
            if (data.Success && data.Datas == null)
            {
                DataAccessQuery<ip_resits>.Insert(data.Datas);
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
    }
}