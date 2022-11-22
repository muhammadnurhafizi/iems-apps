using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IEMSApps.BusinessObject.Responses;
using IEMSApps.Utils;
using System.Text;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BLL;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using IEMSApps.Classes;

namespace IEMSApps.Services
{
    public class HttpClientService
    {
        private static HttpClient GenerateHttpClient()
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(2)
            };

            return httpClient;
        }

        public static async Task<Response<List<TbHandheld>>> GetRecordHandheldAsync(string IdPeranti)
        {
            var result = new Response<List<TbHandheld>>()
            {
                Success = false,
                Mesage = "Ralat"
            };

            try
            {
                var query = $"SELECT idhh,kodcawangan,kodaset_peranti,kodaset_pencetak,appver,noturutan_kpp,noturutan_kots,noturutan_datakes,status, Year FROM tbhandheld where idhh='{IdPeranti}'";
                var encodedQuery = BLL.GeneralBll.Base64Encode(query);

                using (HttpClient client = GenerateHttpClient())
                {
                    //var url = $"http://1.9.46.170:98/API_IEMS/api/getrecord/" + encodedQuery;
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();
                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        result.Mesage = resultObject.Mesage;
                        result.Success = resultObject.Success;

                        var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result);
                        result.Result = JsonConvert.DeserializeObject<List<TbHandheld>>(jsonData.Substring(jsonData.IndexOf('[')));
                    }
                    else
                    {
                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "GetRecordHandheldAsync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "GetRecordHandheldAsync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<DownloadDataResponse>> PrepareDownloadDatas(string IdPeranti, string trkhhhcheckin)
        {
            var result = new Response<DownloadDataResponse>()
            {
                Success = false,
                Mesage = $"ID Peranti {IdPeranti} belum mempunyai Pasukan. Sila hubungi Pejabat"
            };

            try
            {
                //var encodedIdPiranti = BLL.GeneralBll.Base64Encode($"SAL002|2020-01-27 07:00:00");
                var encodedIdPiranti = BLL.GeneralBll.Base64Encode($"{IdPeranti}|{trkhhhcheckin}");
#if DEBUG
                //encodedIdPiranti = BLL.GeneralBll.Base64Encode($"KCH013|{trkhhhcheckin}");
#endif
                using (HttpClient client = GenerateHttpClient())
                {
                    ///var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.PrepareDownloadData}" + encodedIdPiranti;
                    var url = $"{GeneralBll.GetWebServicUrl()}API_IEMS/api/preparedownloaddata/" + encodedIdPiranti;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();

                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result.Replace("\"", ""));
                        result.Result = JsonConvert.DeserializeObject<DownloadDataResponse>(jsonData.Substring(jsonData.IndexOf('{')));
                        result.Success = true;
                        result.Mesage = string.Empty;
                    }
                    else
                    {
                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "PrepareDownloadDatas", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "PrepareDownloadDatas", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

//        public static async Task<Response<DownloadDataFreshResponse>> PrepareDownloadFreshDatas(string IdPeranti)
//        {
//            var result = new Response<DownloadDataFreshResponse>()
//            {
//                Success = false,
//                Mesage = $"ID Peranti {IdPeranti} belum mempunyai Pasukan. Sila hubungi Pejabat"
//            };

//            try
//            {
//                //var encodedIdPiranti = BLL.GeneralBll.Base64Encode($"SAL002|2020-01-27 07:00:00");
//                var encodedIdPiranti = BLL.GeneralBll.Base64Encode($"{IdPeranti}");
//#if DEBUG
//                encodedIdPiranti = BLL.GeneralBll.Base64Encode($"SAL002");
//#endif
//                using (HttpClient client = GenerateHttpClient())
//                {
//                    ///var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.PrepareDownloadData}" + encodedIdPiranti;
//                    var url = $"{GeneralBll.GetWebServicUrl()}API_IEMS/api/preparedownloaddataFresh/" + encodedIdPiranti;
//                    var req = new HttpRequestMessage(HttpMethod.Get, url);
//                    var response = await client.SendAsync(req);
//                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
//                    {
//                        var stringJson = await response.Content.ReadAsStringAsync();

//                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

//                        var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result.Replace("\"", ""));
//                        result.Result = JsonConvert.DeserializeObject<DownloadDataFreshResponse>(jsonData.Substring(jsonData.IndexOf('{')));
//                        result.Success = true;
//                        result.Mesage = string.Empty;
//                    }
//                    else
//                    {
//                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
//                        result.Success = false;
//                    }
//                }
//            }
//            catch (TimeoutException ex)
//            {
//                Log.WriteLogFile("HttpClientService", "PrepareDownloadDatas", ex.Message, Enums.LogType.Error);
//                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

//                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
//                result.Success = false;
//            }
//            catch (System.Exception ex)
//            {
//                Log.WriteLogFile("HttpClientService", "PrepareDownloadDatas", ex.Message, Enums.LogType.Error);
//                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

//                if (ex.Message.Contains("Unable to resolve host"))
//                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
//                else
//                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
//                result.Success = false;
//            }

//            return result;
//        }

        public static async Task<Response<CheckKompaunIzinResponse>> CheckKompaunIzin(string noRujukan)
        {
            var result = new Response<CheckKompaunIzinResponse>()
            {
                Success = false,
                Mesage = "Ralat"
            };

            try
            {
                var query = $"Select status, catatan from tbkompaun_izin where norujukankpp = '{noRujukan}'";
                var encodedQuery = BLL.GeneralBll.Base64Encode(query);

                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
                    //#if DEBUG
                    //                    url = $"http://iemsstag.kpdnhep.gov.my/{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
                    //#endif
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        var stringJson = await response.Content.ReadAsStringAsync();

                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result.Replace("\"", ""));
                        var jsonDataResult = jsonData.Substring(jsonData.IndexOf('{')).Replace("]", "");

                        result.Result = JsonConvert.DeserializeObject<CheckKompaunIzinResponse>(jsonDataResult);
                        result.Success = true;
                        result.Mesage = string.Empty;


                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        result.Mesage = Constants.ErrorMessages.NotFound;
                        result.Success = false;

                        Log.WriteLogFile("HttpClientService", "CheckKompaunIzin", $"No Rujukan : {noRujukan} Not Found", Enums.LogType.Error);
                        Log.WriteLogFile("HttpClientService", "CheckKompaunIzin", query, Enums.LogType.Error);
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "CheckKompaunIzin", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "CheckKompaunIzin", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<string>> ExecuteQuery(string query, Android.Content.Context context = null)
        {
#if DEBUG
            return new Response<string> { Success = true };
#endif

            if (context != null)
                GeneralAndroidClass.StopLocationService(context);

            Thread.Sleep(500);
            var result = new Response<string>()
            {
                Success = false,
                Mesage = "Ralat"
            };

            var encodedQuery = BLL.GeneralBll.Base64Encode(query);
            var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.ExecQueryPost}str=" + encodedQuery;

            try
            {
                using (HttpClient client = GenerateHttpClient())
                {
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();
                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        result.Success = true;
                        result.Mesage = string.Empty;

                    }
                    else
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();
                        if (stringJson.Contains("Duplicate entry"))
                        {
                            result.Success = true;
                            result.Mesage = string.Empty;

                            Log.WriteLogFile("HttpClientService", "ExecuteQuery, StatusCode", response.StatusCode.ToString(), Enums.LogType.Debug);
                            Log.WriteLogFile("HttpClientService", "query", query, Enums.LogType.Debug);
                            Log.WriteLogFile("HttpClientService", "url", url, Enums.LogType.Debug);
                            Log.WriteLogFile("HttpClientService", "url", stringJson, Enums.LogType.Debug);
                            return result;
                        }

                        Log.WriteLogFile("HttpClientService", "ExecuteQuery, StatusCode", response.StatusCode.ToString(), Enums.LogType.Debug);
                        Log.WriteLogFile("HttpClientService", "query", query, Enums.LogType.Debug);
                        Log.WriteLogFile("HttpClientService", "url", url, Enums.LogType.Debug);

                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }

                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "ExecuteQuery", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);
                Log.WriteLogFile("HttpClientService", "url", url, Enums.LogType.Debug);
                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "ExecuteQuery", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);
                Log.WriteLogFile("HttpClientService", "url", url, Enums.LogType.Debug);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }
            finally
            {
                if (context != null)
                    GeneralAndroidClass.StartLocationService(context);
            }

            //Log.WriteLogFile("HttpClientService", "Status => ", $"{result.Success}", Enums.LogType.Info);


            return result;
        }

        public static async Task<Response<string>> UploadImage(string parameter)
        {

#if DEBUG
            return new Response<string> { Success = true, Mesage = string.Empty };
#endif


            var result = new Response<string>()
            {
                Success = false,
                Mesage = "Ralat"
            };

            try
            {
                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.UploadImage}";
                    //StringContent content = new StringContent(parameter, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var content = new StringContent(parameter, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();

                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        //var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result.Replace("\"", ""));
                        //result.Result = JsonConvert.DeserializeObject<int>(jsonData.Substring(jsonData.IndexOf('{')));
                        result.Success = true;
                        result.Mesage = string.Empty;
                    }
                    else
                    {
                        Log.WriteLogFile("HttpClientService", "UploadImage, StatusCode", response.StatusCode.ToString(), Enums.LogType.Debug);
                        //Log.WriteLogFile("HttpClientService", "Parameters", parameter, Enums.LogType.Debug);
                        Log.WriteLogFile("HttpClientService", "url", url, Enums.LogType.Debug);

                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "UploadImage", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "UploadImage", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<List<VersionDto>>> GetVersionAsync()
        {
            var result = new Response<List<VersionDto>>()
            {
                Success = false,
                Mesage = "Ralat"
            };

            try
            {
                var query = $" select versionid, devicetype, descr, priority, url from tbappcontrol order by devicetype,versionid desc limit 1 ";
                var encodedQuery = BLL.GeneralBll.Base64Encode(query);

                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();
                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        result.Mesage = resultObject.Mesage;
                        result.Success = resultObject.Success;

                        var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result);
                        result.Result = JsonConvert.DeserializeObject<List<VersionDto>>(jsonData.Substring(jsonData.IndexOf('[')));
                    }
                    else
                    {
                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "GetVersionAsync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "GetVersionAsync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<List<KppDto>>> GetKPPAsync(string noRujukan)
        {
            var result = new Response<List<KppDto>>()
            {
                Success = false,
                Mesage = "Ralat"
            };

            try
            {
                var query = $" SELECT * FROM tbkpp where norujukankpp  = '{noRujukan}' ";
                var encodedQuery = BLL.GeneralBll.Base64Encode(query);

                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();
                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        result.Mesage = resultObject.Mesage;
                        result.Success = resultObject.Success;

                        var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result);
                        result.Result = JsonConvert.DeserializeObject<List<KppDto>>(jsonData.Substring(jsonData.IndexOf('[')));
                    }
                    else
                    {
                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "GetKPPAsync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "GetKPPAsync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<string>> CheckData(string query)
        {
            var result = new Response<string>()
            {
                Success = false,
                Mesage = "Ralat"
            };

            try
            {
                var encodedQuery = BLL.GeneralBll.Base64Encode(query);

                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();
                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        result.Mesage = resultObject.Mesage;
                        result.Success = resultObject.Success;
                    }
                    else
                    {
                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "CheckData", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "CheckData", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<List<TableSummaryResponse>>> GetTableSummary(string idPeranti)
        {
            var result = new Response<List<TableSummaryResponse>>();

#if DEBUG
            result = new Response<List<TableSummaryResponse>>
            {
                Success = true,
                Result = new List<TableSummaryResponse>
                {
                    new TableSummaryResponse
                    {
                        IsModified = 1,
                        IsSelected = false,
                        RecordDesc = "Cawangan",
                        TableName = "Cawangan",
                        TotalApp = 10,
                        TotalRec = 10
                    },
                    new TableSummaryResponse
                    {
                        IsModified = 0,
                        IsSelected = true,
                        RecordDesc = "Cawangan",
                        TableName = "Cawangan",
                        TotalApp = 10,
                        TotalRec = 10
                    },
                    new TableSummaryResponse
                    {
                        IsModified = 0,
                        IsSelected = true,
                        RecordDesc = "Cawangan",
                        TableName = "Cawangan",
                        TotalApp = 10,
                        TotalRec = 10
                    },
                    new TableSummaryResponse
                    {
                        IsModified = 1,
                        IsSelected = false,
                        RecordDesc = "Cawangan",
                        TableName = "Cawangan",
                        TotalApp = 10,
                        TotalRec = 10
                    },
                    new TableSummaryResponse
                    {
                        IsModified = 1,
                        IsSelected = false,
                        RecordDesc = "Cawangan",
                        TableName = "Cawangan",
                        TotalApp = 10,
                        TotalRec = 10
                    },
                    new TableSummaryResponse
                    {
                        IsModified = 1,
                        IsSelected = false,
                        RecordDesc = "Cawangan",
                        TableName = "Cawangan",
                        TotalApp = 10,
                        TotalRec = 10
                    },
                    new TableSummaryResponse
                    {
                        IsModified = 1,
                        IsSelected = false,
                        RecordDesc = "Cawangan",
                        TableName = "Cawangan",
                        TotalApp = 10,
                        TotalRec = 10
                    }
                }
            };
            return result;
#endif

            try
            {
                var encodedQuery = BLL.GeneralBll.Base64Encode(idPeranti);

                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetTableSummary}" + encodedQuery;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();
                        var resultObject = JsonConvert.DeserializeObject<List<TableSummaryResponse>>(stringJson);

                        result.Result = resultObject;
                    }
                    else
                    {
                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "GetTableSummary", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "GetTableSummary", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<PrepareDownloadDataSelectedResponse>> PrepareDownloadDataSelected(string param)
        {
            var result = new Response<PrepareDownloadDataSelectedResponse>()
            {
                Success = false,
                Mesage = ""
            };

            try
            {
                var base64Param = BLL.GeneralBll.Base64Encode($"{param}");
                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.PrepareDownloadDataSelected}" + base64Param;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();

                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result.Replace("\"", ""));
                        result.Result = JsonConvert.DeserializeObject<PrepareDownloadDataSelectedResponse>(jsonData.Substring(jsonData.IndexOf('{')));
                        result.Success = true;
                        result.Mesage = string.Empty;
                    }
                    else
                    {
                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "PrepareDownloadDatas", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "PrepareDownloadDatas", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<DataSsmReponse>> GetListSsm(string inputSearch)
        {
            var result = new Response<DataSsmReponse>()
            {
                Success = false,
                Mesage = "Error"
            };

            try
            {

                var encodedQuery = BLL.GeneralBll.Base64Encode(inputSearch);

                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetListSsm}" + encodedQuery;
#if DEBUG
                    url = $"http://iemsstag.kpdnhep.gov.my/{Constants.ApiUrlAction.GetListSsm}" + encodedQuery;
#endif
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        var stringJson = await response.Content.ReadAsStringAsync();

                        var resultObject = JsonConvert.DeserializeObject<List<DataSsmDetails>>(stringJson);

                        //var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result.Replace("\"", ""));
                        //var jsonDataResult = jsonData.Substring(jsonData.IndexOf('{')).Replace("]", "");

                        //result.Result = JsonConvert.DeserializeObject<DataSsmReponse>(jsonDataResult);
                        result.Result = new DataSsmReponse
                        {
                            DataDetails = resultObject
                        };
                        result.Success = true;
                        result.Mesage = string.Empty;


                    }

                    else if (response.StatusCode == HttpStatusCode.BadRequest
                        || response.StatusCode == HttpStatusCode.NotFound)
                    {
                        result.Mesage = Constants.ErrorMessages.NoDataFound;
                        result.Success = false;
                    }
                    else
                    {
                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "GetListSsm", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "GetListSsm", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }

        public static async Task<Response<JpnDetailResponse>> GetListJpnDetail(string inputSearch, string noKpFOrUser)
        {
            var result = new Response<JpnDetailResponse>()
            {
                Success = false,
                Mesage = "Error"
            };

#if DEBUG
            var json = "{\"status\":105,\"messageCode\":\"API name is required\"}";
            result.Result = JsonConvert.DeserializeObject<JpnDetailResponse>(json);
            result.Success = true;
            result.Mesage = string.Empty;
            return result;
#endif

            try
            {

                var encodedQuery = GeneralBll.Base64Encode(inputSearch);
                var encodedNoKpForUSer = GeneralBll.Base64Encode(noKpFOrUser);
                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetJpnDetailByNoIcWithErr}{encodedQuery}/{encodedNoKpForUSer}";
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        var stringJson = await response.Content.ReadAsStringAsync();
                        Log.WriteLogFile("HttpClientService", "GetListJpnDetail", $"Response : {stringJson}", Enums.LogType.Info);

                        if (!string.IsNullOrEmpty(stringJson))
                        {
                            var resultObject = JsonConvert.DeserializeObject<JpnDetailResponse>(stringJson);
                            result.Result = resultObject;
                            //if (string.IsNullOrEmpty(result.Result.status))
                            //{
                            //    Log.WriteLogFile("HttpClientService", "GetListJpnDetail", $"{resultObject.messageCode}", Enums.LogType.Info);

                            //    result.Success = false;
                            //    result.Mesage = resultObject.messageCode;
                            //    return result;
                            //}

                            result.Success = true;
                            result.Mesage = string.Empty;
                        }
                        else
                        {
                            Log.WriteLogFile("HttpClientService", "GetListJpnDetail", $"Data not Found for {inputSearch}", Enums.LogType.Info);

                            result.Success = true;
                            result.Result = null;
                        }
                    }

                    else if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Log.WriteLogFile("HttpClientService", "GetListJpnDetail", "HttpStatusCode.BadRequest | HttpStatusCode.NotFound", Enums.LogType.Error);

                        result.Mesage = Constants.ErrorMessages.NoDataFound;
                        result.Success = false;
                    }
                    else
                    {
                        Log.WriteLogFile("HttpClientService", "GetListJpnDetail", "Error : " + response.StatusCode.ToString(), Enums.LogType.Error);

                        result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
                        result.Success = false;
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "GetListJpnDetail", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "GetListJpnDetail", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);

                result.Success = false;
            }

            return result;
        }

        //public static void UploadFile(string address, List<UploadFileDto> files)
        //{
        //    using (var client = new WebClient())
        //    {
        //        client.UploadFile("http://iemsstag.kpdnhep.gov.my/iems/apk/", WebRequestMethods.Ftp.UploadFile, files[0].Path);
        //    }
        //
        //    //var request = WebRequest.Create(address);
        //    //request.Method = "POST";
        //    //var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
        //    //request.ContentType = "multipart/form-data; boundary=" + boundary;
        //    //boundary = "--" + boundary;
        //    //
        //    //using (var requestStream = request.GetRequestStream())
        //    //{
        //    //    foreach (var file in files)
        //    //    {
        //    //        var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
        //    //        requestStream.Write(buffer, 0, buffer.Length);
        //    //        buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
        //    //        requestStream.Write(buffer, 0, buffer.Length);
        //    //        buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
        //    //        requestStream.Write(buffer, 0, buffer.Length);
        //    //        file.Stream.CopyTo(requestStream);
        //    //        buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
        //    //        requestStream.Write(buffer, 0, buffer.Length);
        //    //    }
        //    //    var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
        //    //    requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
        //    //}
        //    //
        //    //using (var response = request.GetResponse())
        //    //using (var responseStream = response.GetResponseStream())
        //    //using (var stream = new MemoryStream())
        //    //{
        //    //    responseStream.CopyTo(stream);
        //    //    return stream.ToArray();
        //    //}
        //}

        public static async Task<bool> UploadFileAsync(string fileLocation, string fileName)
        {
            try
            {
                if (!File.Exists(fileLocation))
                {
                    Log.WriteLogFile("HttpClientService", "UploadFileAsync", "Upload File Faild, File not exists", Enums.LogType.Info);
                    return false;
                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Constants.FtpAccount.Url + fileName);
                request.Credentials = new NetworkCredential(Constants.FtpAccount.UserName, Constants.FtpAccount.Password);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                using (Stream fileStream = File.OpenRead(fileLocation))
                using (Stream ftpStream = await request.GetRequestStreamAsync())
                {
                    byte[] buffer = new byte[10240];
                    int read;
                    while ((read = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await ftpStream.WriteAsync(buffer, 0, read);
                        Console.WriteLine("Uploaded {0} bytes", fileStream.Position);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "UploadFileAsync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);
                return false;
            }

        }

        //public static async Task<Response<List<Pesalah>>> GetDataKesPesalahBasedNoKes(string noKes)
        //{
        //    var result = new Response<List<Pesalah>>()
        //    {
        //        Success = false,
        //        Mesage = "Ralat"
        //    };

        //    try
        //    {
        //        var query = $"select * from tbdatakes_pesalah where nokes= '{noKes}'";
        //        var encodedQuery = BLL.GeneralBll.Base64Encode(query);

        //        using (HttpClient client = GenerateHttpClient())
        //        {
        //            var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
        //            var req = new HttpRequestMessage(HttpMethod.Get, url);
        //            var response = await client.SendAsync(req);
        //            if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //            {
        //                var stringJson = await response.Content.ReadAsStringAsync();
        //                var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

        //                result.Mesage = resultObject.Mesage;
        //                result.Success = resultObject.Success;

        //                var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result);
        //                result.Result = JsonConvert.DeserializeObject<List<Pesalah>>(jsonData.Substring(jsonData.IndexOf('[')));
        //            }
        //            else
        //            {
        //                result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
        //                result.Success = false;
        //            }
        //        }
        //    }
        //    catch (TimeoutException ex)
        //    {
        //        Log.WriteLogFile("HttpClientService", "GetDataKesPesalahBasedNoKes", ex.Message, Enums.LogType.Error);
        //        Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

        //        result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
        //        result.Success = false;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Log.WriteLogFile("HttpClientService", "GetDataKesPesalahBasedNoKes", ex.Message, Enums.LogType.Error);
        //        Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

        //        if (ex.Message.Contains("Unable to resolve host"))
        //            result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
        //        else
        //            result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
        //        result.Success = false;
        //    }

        //    return result;
        //}

        //public static async Task<Response<List<Kesalahan>>> GetDataKesKesalahanBasedNoKes(string noKes)
        //{
        //    var result = new Response<List<Kesalahan>>()
        //    {
        //        Success = false,
        //        Mesage = "Ralat"
        //    };

        //    try
        //    {
        //        var query = $"select * from tbdatakes_kesalahan where nokes= '{noKes}'";
        //        var encodedQuery = BLL.GeneralBll.Base64Encode(query);

        //        using (HttpClient client = GenerateHttpClient())
        //        {
        //            var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
        //            var req = new HttpRequestMessage(HttpMethod.Get, url);
        //            var response = await client.SendAsync(req);
        //            if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //            {
        //                var stringJson = await response.Content.ReadAsStringAsync();
        //                var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

        //                result.Mesage = resultObject.Mesage;
        //                result.Success = resultObject.Success;

        //                var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result);
        //                result.Result = JsonConvert.DeserializeObject<List<Kesalahan>>(jsonData.Substring(jsonData.IndexOf('[')));
        //            }
        //            else
        //            {
        //                result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi, response.StatusCode);
        //                result.Success = false;
        //            }
        //        }
        //    }
        //    catch (TimeoutException ex)
        //    {
        //        Log.WriteLogFile("HttpClientService", "GetDataKesKesalahanBasedNoKes", ex.Message, Enums.LogType.Error);
        //        Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

        //        result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
        //        result.Success = false;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Log.WriteLogFile("HttpClientService", "GetDataKesKesalahanBasedNoKes", ex.Message, Enums.LogType.Error);
        //        Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

        //        if (ex.Message.Contains("Unable to resolve host"))
        //            result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
        //        else
        //            result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
        //        result.Success = false;
        //    }

        //    return result;
        //}

        public static async Task<Response<CountDataResponse>> CountAync(string query)
        {
#if DEBUG
            return new Response<CountDataResponse>() { Success = true, Result = new CountDataResponse { Count = 0 } };
#endif
            var result = new Response<CountDataResponse>()
            {
                Success = false,
                Mesage = "Ralat"
            };

            try
            {
                var encodedQuery = BLL.GeneralBll.Base64Encode(query);

                using (HttpClient client = GenerateHttpClient())
                {
                    var url = $"{GeneralBll.GetWebServicUrl()}{Constants.ApiUrlAction.GetRecord}" + encodedQuery;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await client.SendAsync(req);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var stringJson = await response.Content.ReadAsStringAsync();

                        var resultObject = JsonConvert.DeserializeObject<Response<string>>(stringJson);

                        var jsonData = BLL.GeneralBll.Base64Decode(resultObject.Result.Replace("\"", ""));
                        var jsonDataResult = jsonData.Substring(jsonData.IndexOf('{')).Replace("]", "");

                        result.Result = JsonConvert.DeserializeObject<CountDataResponse>(jsonDataResult);
                        result.Success = true;
                        result.Mesage = string.Empty;


                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        result.Mesage = Constants.ErrorMessages.NotFound;
                        result.Success = true;
                        result.Result = new CountDataResponse { Count = 0 };

                        Log.WriteLogFile("HttpClientService", "CountAync", $" Data Not Found", Enums.LogType.Error);
                        Log.WriteLogFile("HttpClientService", "CountAync", query, Enums.LogType.Error);
                    }
                }
            }
            catch (TimeoutException ex)
            {
                Log.WriteLogFile("HttpClientService", "CountAync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                result.Success = false;
            }
            catch (System.Exception ex)
            {
                Log.WriteLogFile("HttpClientService", "CountAync", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : ", ex.StackTrace, Enums.LogType.Error);

                if (ex.Message.Contains("Unable to resolve host"))
                    result.Mesage = Constants.ErrorMessages.ErrorApiTimeout;
                else
                    result.Mesage = String.Format(Constants.ErrorMessages.ErrorApi_Exception, ex.Message);
                result.Success = false;
            }

            return result;
        }
    }
}