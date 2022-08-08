
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Responses;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IEMSApps.Fragments
{
    public class CleanData : Fragment
    {
        private const string LayoutName = "CleanData";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CleanDataLayout, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetInit();
        }

        LinearLayout linearSnack;
        ProgressBar progressBar1, progressBar2;
        TextView lblInfo, lblTotal, lblPercentage;
        Button btnCheckout;

        private void SetInit()
        {
            try
            {
                linearSnack = View.FindViewById<LinearLayout>(Resource.Id.linearSnack);
                progressBar1 = View.FindViewById<ProgressBar>(Resource.Id.progressBar1);
                progressBar2 = View.FindViewById<ProgressBar>(Resource.Id.progressBar2);
                lblInfo = View.FindViewById<TextView>(Resource.Id.lblInfo);
                lblTotal = View.FindViewById<TextView>(Resource.Id.lblTotal);
                lblPercentage = View.FindViewById<TextView>(Resource.Id.lblPercentage);
                btnCheckout = View.FindViewById<Button>(Resource.Id.btnCheckout);
                lblInfo.Text = string.Empty;
                ShowCheckutProcess(false);
                UpdateInfo(string.Empty, 0, 0);

                btnCheckout.Click += BtnCheckout_Click;
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            new Task(async () => { await OnCleanDataAsync(); }).Start();
        }

        private void ShowCheckutProcess(bool isRunning)
        {
            Activity.RunOnUiThread(() =>
            {
                if (isRunning)
                {
                    linearSnack.Visibility = ViewStates.Visible;
                    progressBar2.Visibility = ViewStates.Visible;

                    btnCheckout.Visibility = ViewStates.Gone;
                }
                else
                {
                    linearSnack.Visibility = ViewStates.Invisible;
                    progressBar2.Visibility = ViewStates.Gone;

                    btnCheckout.Visibility = ViewStates.Visible;
                }
            });
        }

        private void UpdateInfo(string message, int index = 0, int total = 0)
        {
            Activity.RunOnUiThread(() =>
            {
                SleepProcess();
                lblInfo.Text = message;

                if (index != 0 && total != 0)
                {
                    lblTotal.Text = $"Data {index}/{total}";
                    lblPercentage.Text = $"{(int)Math.Round((double)(100 * index) / total)}%";
                    progressBar1.Progress = index;
                }
                else
                {
                    lblTotal.Text = string.Empty;
                    lblPercentage.Text = string.Empty;
                }

                Log.WriteLogFile(message + ((index != 0 && total != 0) ? $" Data {index}/{total}" : string.Empty));
            });
        }

        private async Task OnCleanDataAsync()
        {
            try
            {
                if (!GeneralAndroidClass.IsOnline())
                {
                    var localDate = GeneralBll.GetLocalDateTime();

                    UpdateInfo(
                        localDate.ToString(Constants.DateFormatDisplay) + " " +
                        localDate.ToString(Constants.TimeFormatDisplay) + " " +
                        Constants.ErrorMessages.NoInternetConnection, 0, 0);

                    UpdateInfo(Constants.ErrorMessages.NoInternetConnection, 0, 0);
                    ShowCheckutProcess(false);
                    return;
                }
                else
                {
                    ShowCheckutProcess(true);
                    //Send Online
                    UpdateInfo("Sila Tunggu...");

                    UpdateInfo("Servis Dihentikan...");
                    GeneralAndroidClass.StopBackgroundService(this.Context);
                    Thread.Sleep(2000);

                    Log.WriteLogFile("CleanData", "OnCleanDataAsync", "Start Clean Data Async", Enums.LogType.Info);

                    UpdateInfo("Hantar Data...");
                    SleepProcess();

                    UpdateInfo("Hantar Data TBSendOnlineData...");
                    var allDataAlreadySent = await SendAndCleanDatas();
                    UpdateInfo("Finish Hantar Data TBSendOnlineData...");
                    UpdateInfo("Hantar Images TBSendOnlineImages...");
                    var allImagesAlreadaySent = await SendAndCleanImagesFromTbSendOnlineGambar();
                    UpdateInfo("Finish Hantar Images TBSendOnlineImages...");
                    UpdateInfo("Hantar Images From Backup Folders...");
                    var allImagesFrombackup = await SendAndCleanImagesFromBackupPath();
                    UpdateInfo("Finish Hantar Images From Backup Folders...");
                    //- the file Errorrecordstmt_<hhid>_<yyyymmddh>.txt need to send to server in checkout process (last step) and backup to backup folder before delete
                    UpdateInfo($"Check Error File...Data Error : {allDataAlreadySent} - Images : {allImagesAlreadaySent} - Images From Backup : {allImagesFrombackup}");
                    //if (!allDataAlreadySent || !allImagesAlreadaySent || !allImagesFrombackup)
                    //{
                    //var logFile = Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.LogPath + Constants.ErrorRecordsPath;
                    //var fileName = $"Errorrecordstmt_{GeneralBll.GetUserHandheld()}_{DateTime.Now.ToString("yyyymmddh")}.txt";
                    //var logFileErrorStmt = Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.LogPath + fileName;
                    //if (File.Exists(logFile))
                    //{
                    //    File.Copy(logFile, logFileErrorStmt, true);
                    //}

                    //if (File.Exists(logFileErrorStmt))
                    //{
                    //    UpdateInfo($"Check Server...");
                    //    var versionDto = await HttpClientService.GetVersionAsync();
                    //    UpdateInfo($"Uploading {fileName} to {versionDto.Result[0]?.Url}/logs/");
                    //    if (versionDto.Success && !string.IsNullOrEmpty(versionDto.Result[0]?.Url))
                    //    {
                    //        UpdateInfo($"Uploading {fileName}...");
                    //        SleepProcess();

                    //        var client = new WebClient();
                    //        client.UploadProgressChanged += Client_UploadProgressChanged;
                    //        client.UploadFileCompleted += Client_UploadFileCompleted;

                    //        var data = System.IO.File.ReadAllBytes(logFileErrorStmt);
                    //        client.UploadData(new Uri($"{versionDto.Result[0]?.Url}/logs/"), data);

                    //        UpdateInfo("Proses Pembersihan Data telah selesai...");
                    //    }
                    //}
                    //else
                    //    UpdateInfo("Proses Pembersihan Data telah selesai...");

                    var errorDatas = DataAccessQuery<TbError>.GetAll();
                    UpdateInfo($"Check error data...");
                    if (errorDatas.Success && errorDatas.Datas != null)
                    {
                        UpdateInfo($"Check error data..." + errorDatas.Datas.Count());
                        var totalDatas = errorDatas.Datas.Count();

                        foreach (var item in errorDatas.Datas.Select((value, index) => new { index, value }))
                        {
                            if (item.value.Status == ((int)Enums.StatusOnline.Sent).ToString()) continue;
                            UpdateInfo($"Hantar error data...", item.index, totalDatas);

                            var query = $" Insert into tberrorhh (idhh, kodcawangan, sqlstmt, status, pgndaftar, trkhdaftar, pgnakhir, trkhakhir) " +
                                        $" values('{item.value.IdHH}', '{item.value.KodCawangan}', '{item.value.SqlStmt}', '{item.value.Status}', '{item.value.PgnDaftar}', UNIX_TIMESTAMP('{item.value.TrkhDaftar}'), '{item.value.PgnAkhir}', UNIX_TIMESTAMP('{item.value.TrkhAkhir}')) ";
                            var response = await HttpClientService.ExecuteQuery(query, null);

                            UpdateInfo($"Hantar error data..." + (response.Success ? "Success" : "Gagal-" + response.Mesage), item.index, totalDatas);

                            if (response.Success)
                            {
                                UpdateInfo($"Delete error data..." + JsonConvert.SerializeObject(item.value));
                                DataAccessQuery<TbError>.ExecuteSql($" Delete from tberrorhh where id = '{item.value.Id}' ");
                            }
                            else
                                DataAccessQuery<TbError>.ExecuteSql($" Update tberrorhh set status = '{((int)Enums.StatusOnline.Error).ToString()}' where id = '{item.value.Id}' ");
                        }
                        UpdateInfo("Proses Pembersihan Data telah selesai...");
                    }
                    else
                        UpdateInfo("Proses Pembersihan Data telah selesai...");
                    //}
                    //else
                    //{
                    //    UpdateInfo("Proses Pembersihan Data telah selesai...");
                    //}
                    ShowCheckutProcess(false);
                }

            }
            catch (Exception ex)
            {
                UpdateInfo("Error Exception - CleanData - OnCleanDataAsync" + ex.Message);
                UpdateInfo(ex.Message, 0, 0);
                //GeneralBll.LogDataWithException("CleanData", "OnCleanDataAsync", ex);                
                ShowCheckutProcess(false);
            }
        }

        public static string Error;
        public static string Result;
        public static bool Completed;

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            Console.WriteLine(new string('-', 12));
            if (e.Error != null)
            {
                Error = e.Error.ToString();
                UpdateInfo($"Error : {Error}");
            }
            else
            {
                PercentUploaded = 100;
                Completed = true;
                Result = Encoding.UTF8.GetString(e.Result);

                UpdateInfo($"File upload Completed. Server Response: {Result}");
            }
        }

        public static double PercentUploaded;

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            PercentUploaded = e.ProgressPercentage;
            UpdateInfo($"Uploaded {e.BytesSent} of {e.TotalBytesToSend} bytes. {e.ProgressPercentage} % complete...");
        }

        private async Task<bool> SendAndCleanDatas()
        {
            var allDataAlreadySent = true;
            var query = string.Empty;
            var responseCheckData = new Response<CountDataResponse>();
            var responseResendData = new Response<string>();

            UpdateInfo("Get Datas...");
            var result = DataAccessQuery<TbSendOnlineData>.GetAll(m => m.Status != Enums.StatusOnline.Sent);
            if (result.Success && result.Datas != null)
            {
                var totalDatas = result.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in result.Datas.Select((value, index) => new { index, value }))
                {
                    //type = 1,5
                    //select count(*) as count from tbkpp where norujukankpp = <>
                    #region KPP and KPP_HH
                    if (item.value.Type == Enums.TableType.KPP || item.value.Type == Enums.TableType.KPP_HH)
                    {
                        UpdateInfo($"Check data {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                        query = $"select count(*) as count from tbkpp where norujukankpp = '{item.value.NoRujukan}'";
                        if (item.value.Type == Enums.TableType.KPP_HH)
                            query = $"select count(*) as count from tbkpp_hh where norujukankpp = '{item.value.NoRujukan}'";

                        responseCheckData = await HttpClientService.CountAync(query);
                        UpdateInfo($"Check data {item.value.Type.ToString()} {item.value.NoRujukan}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", item.index, totalDatas);
                        if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                        {
                            UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} to Sent", item.index, totalDatas);

                            SleepProcess();

                            item.value.Status = Enums.StatusOnline.Sent;
                            DataAccessQuery<TbSendOnlineData>.Update(item.value);
                        }
                        else
                        {
                            UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                            responseResendData = await SendOnlineBll.SendKppOnlineAsyncV2(item.value.NoRujukan, item.value.Type, this.Context, true);
                            UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan} " + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, item.index, totalDatas);
                            if (!responseResendData.Success)
                            {
                                allDataAlreadySent = false;

                                UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan} Gagal...", item.index, totalDatas);
                                SleepProcess();
                                UpdateInfo($"Writing to log {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                                SleepProcess();

                                UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} to Sent", item.index, totalDatas);
                                UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - {item.value.Type} to Sent", item.index, totalDatas);

                                item.value.Status = Enums.StatusOnline.Sent;
                                DataAccessQuery<TbSendOnlineData>.Update(item.value);

                                var pasukanTrans = DataAccessQuery<TbPasukanTrans>.GetAll(m =>
                                                                m.JenisTrans == Constants.JenisTrans.Kpp &&
                                                                m.IsSendOnline != Enums.StatusOnline.Sent && m.NoRujukan == item.value.NoRujukan);
                                if (pasukanTrans.Success && pasukanTrans.Datas != null)
                                {
                                    foreach (var pasukan in pasukanTrans.Datas)
                                    {
                                        UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - Pasukan Trans (ID : {pasukan.Id}) to Sent", item.index, totalDatas);
                                        pasukan.IsSendOnline = Enums.StatusOnline.Sent;
                                        DataAccessQuery<TbPasukanTrans>.Update(pasukan);
                                    }

                                }

                                var asasTindakan = DataAccessQuery<TbKppAsasTindakan>.GetAll(m => m.NoRujukanKpp == item.value.NoRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
                                if (asasTindakan.Success && asasTindakan.Datas != null)
                                {
                                    foreach (var asas in asasTindakan.Datas)
                                    {
                                        UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - Asas Tindakan (Kod Tujuan : {asas.KodTujuan}, Kod Asas : {asas.KodAsas}) to Sent", item.index, totalDatas);

                                        asas.IsSendOnline = Enums.StatusOnline.Sent;
                                        DataAccessQuery<TbKppAsasTindakan>.Update(asas);
                                    }

                                }
                            }
                        }
                    }
                    #endregion

                    //type = 2,6
                    //select count(*) as count from tbkompaun where nokmp = <>	
                    #region KOMPAUN or KOMPAUN_HH
                    if (item.value.Type == Enums.TableType.Kompaun || item.value.Type == Enums.TableType.Kompaun_HH)
                    {
                        UpdateInfo($"Check data {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                        query = $"select count(*) as count from tbkompaun where nokmp = '{item.value.NoRujukan}'";
                        if (item.value.Type == Enums.TableType.Kompaun_HH)
                            query = $"select count(*) as count from tbkompaun_hh where nokmp = '{item.value.NoRujukan}'";
                        responseCheckData = await HttpClientService.CountAync(query);
                        UpdateInfo($"Check data {item.value.Type.ToString()} {item.value.NoRujukan}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", item.index, totalDatas);
                        if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                        {
                            UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} to Sent", item.index, totalDatas);

                            SleepProcess();

                            item.value.Status = Enums.StatusOnline.Sent;
                            DataAccessQuery<TbSendOnlineData>.Update(item.value);
                        }
                        else
                        {
                            UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                            responseResendData = await SendOnlineBll.SendKompaunOnlineAsyncV2(item.value.NoRujukan, item.value.Type, this.Context, true);
                            UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan} " + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, item.index, totalDatas);
                            if (!responseResendData.Success)
                            {
                                allDataAlreadySent = false;

                                UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan} Gagal...", item.index, totalDatas);
                                SleepProcess();
                                UpdateInfo($"Writing to log {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                                SleepProcess();

                                UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} to Sent", item.index, totalDatas);
                                UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - {item.value.Type} to Sent", item.index, totalDatas);

                                item.value.Status = Enums.StatusOnline.Sent;
                                DataAccessQuery<TbSendOnlineData>.Update(item.value);
                            }
                        }
                    }
                    #endregion

                    //type = 3,7
                    //select count(*) as count from tbdatakes where nokes = <>
                    #region DATAKES or DATAKES_HH
                    if (item.value.Type == Enums.TableType.DataKes || item.value.Type == Enums.TableType.DataKes_HH)
                    {
                        UpdateInfo($"Check data {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                        query = $"select count(*) as count from tbdatakes where nokes = '{item.value.NoRujukan}'";

                        responseCheckData = await HttpClientService.CountAync(query);
                        UpdateInfo($"Check data {item.value.Type.ToString()} {item.value.NoRujukan}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", item.index, totalDatas);

                        if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                        {
                            UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} to Sent", item.index, totalDatas);

                            SleepProcess();

                            item.value.Status = Enums.StatusOnline.Sent;
                            DataAccessQuery<TbSendOnlineData>.Update(item.value);
                        }
                        else
                        {
                            UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                            responseResendData = await SendOnlineBll.SendDataKesOnlineAsyncV2(item.value.NoRujukan, item.value.Type, this.Context, true);
                            UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan} " + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, item.index, totalDatas);
                            if (!responseResendData.Success)
                            {
                                allDataAlreadySent = false;

                                UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan} Gagal...", item.index, totalDatas);
                                SleepProcess();
                                UpdateInfo($"Writing to log {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                                SleepProcess();

                                UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} to Sent", item.index, totalDatas);
                                UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - {item.value.Type} to Sent", item.index, totalDatas);

                                item.value.Status = Enums.StatusOnline.Sent;
                                DataAccessQuery<TbSendOnlineData>.Update(item.value);

                                var dataKesPesalah = DataAccessQuery<TbDataKesPesalah>.GetAll(m => m.NoKes == item.value.NoRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
                                if (dataKesPesalah.Success && dataKesPesalah.Datas != null)
                                {
                                    foreach (var kesPesalah in dataKesPesalah.Datas)
                                    {
                                        UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - Data Kes Pesalah (ID : {kesPesalah.Id}) to Sent", item.index, totalDatas);

                                        kesPesalah.IsSendOnline = Enums.StatusOnline.Sent;
                                        DataAccessQuery<TbDataKesPesalah>.Update(kesPesalah);
                                    }
                                }

                                var dataKesAsasTindakan = DataAccessQuery<TbDataKesAsasTindakan>.GetAll(m => m.NoKes == item.value.NoRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
                                if (dataKesAsasTindakan.Success && dataKesAsasTindakan.Datas != null)
                                {
                                    foreach (var kesAsasTindakan in dataKesAsasTindakan.Datas)
                                    {
                                        UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - Data Kes Asas Tindakan (ID : {kesAsasTindakan.Id}) to Sent", item.index, totalDatas);

                                        kesAsasTindakan.IsSendOnline = Enums.StatusOnline.Sent;
                                        DataAccessQuery<TbDataKesAsasTindakan>.Update(kesAsasTindakan);
                                    }
                                }

                                var dataKesKesalahan = DataAccessQuery<TbDataKesKesalahan>.GetAll(m => m.NoKes == item.value.NoRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
                                if (dataKesKesalahan.Success && dataKesKesalahan.Datas != null)
                                {
                                    foreach (var kesalahan in dataKesKesalahan.Datas)
                                    {
                                        UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - Data Kes Kesalahan (ID : {kesalahan.Id}) to Sent", item.index, totalDatas);

                                        kesalahan.IsSendOnline = Enums.StatusOnline.Sent;
                                        DataAccessQuery<TbDataKesKesalahan>.Update(kesalahan);
                                    }
                                }

                                var dataPesalah = DataAccessQuery<TbDataKesPesalah>.GetAll(m => m.NoKes == item.value.NoRujukan && m.IsSendOnline != Enums.StatusOnline.Sent);
                                if (dataPesalah.Success && dataPesalah.Datas != null)
                                {
                                    foreach (var pesalah in dataPesalah.Datas)
                                    {
                                        UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - Data Pesalah (ID : {pesalah.Id}) to Sent", item.index, totalDatas);

                                        pesalah.IsSendOnline = Enums.StatusOnline.Sent;
                                        DataAccessQuery<TbDataKesPesalah>.Update(pesalah);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    //type = 4,8,9
                    //select count(*) as count from tbkompaun_bayaran where nokmp = <>
                    #region KOMPAUN BAYARAN or UPDATE KOMPAUN or UPDATE KOMPAUN HH
                    if (item.value.Type == Enums.TableType.KompaunBayaran || item.value.Type == Enums.TableType.Akuan_UpdateKompaun || item.value.Type == Enums.TableType.Akuan_UpdateKompaun_HH)
                    {
                        UpdateInfo($"Check data {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                        query = $"select count(*) as count from tbkompaun_bayaran where nokmp = '{item.value.NoRujukan}'";

                        responseCheckData = await HttpClientService.CountAync(query);
                        UpdateInfo($"Check data {item.value.Type.ToString()} {item.value.NoRujukan}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", item.index, totalDatas);
                        if (responseCheckData.Success && responseCheckData.Result?.Count > 0)
                        {
                            UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} to Sent", item.index, totalDatas);

                            SleepProcess();

                            item.value.Status = Enums.StatusOnline.Sent;
                            DataAccessQuery<TbSendOnlineData>.Update(item.value);
                        }
                        else
                        {
                            UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                            responseResendData = await SendOnlineBll.SendAkuanAsync(item.value.NoRujukan, item.value.Type, this.Context, true);
                            UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan} " + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, item.index, totalDatas);
                            if (!responseResendData.Success)
                            {
                                allDataAlreadySent = false;

                                UpdateInfo($"Hantar data {item.value.Type.ToString()} {item.value.NoRujukan} Gagal...", item.index, totalDatas);
                                SleepProcess();
                                UpdateInfo($"Writing to log {item.value.Type.ToString()} {item.value.NoRujukan}...", item.index, totalDatas);
                                SleepProcess();

                                UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} to Sent", item.index, totalDatas);
                                UpdateInfo($"Update data {item.value.Type.ToString()} {item.value.NoRujukan} - {item.value.Type} to Sent", item.index, totalDatas);

                                item.value.Status = Enums.StatusOnline.Sent;
                                DataAccessQuery<TbSendOnlineData>.Update(item.value);
                            }
                        }
                    }
                    #endregion
                }
            }

            //tbdatakes_asastindakan
            //select count(*) as count from tbdatakes_asastindakan where nokes=<> and kodtujuan=<> and kodasas = <>
            var tbDatakesAsasTindakans = DataAccessQuery<TbDataKesAsasTindakan>.GetAll(m => m.IsSendOnline != Enums.StatusOnline.Sent);
            if (tbDatakesAsasTindakans.Success && tbDatakesAsasTindakans.Datas != null)
            {
                var totalDatas = tbDatakesAsasTindakans.Datas.Count();
                progressBar1.Max = totalDatas;

                foreach (var datakesAsasTindakan in tbDatakesAsasTindakans.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Check data Data Kes Asas Tindakan, No Kes {datakesAsasTindakan.value.NoKes} {datakesAsasTindakan.value.KodTujuan} {datakesAsasTindakan.value.KodAsas}...", datakesAsasTindakan.index, totalDatas);
                    query = $"select count(*) as count from tbdatakes_asastindakan where nokes='{datakesAsasTindakan.value.NoKes}' and kodtujuan='{datakesAsasTindakan.value.KodTujuan}' and kodasas = '{datakesAsasTindakan.value.KodAsas}'";

                    responseCheckData = await HttpClientService.CountAync(query);
                    UpdateInfo($"Check data Data Kes Asas Tindakan, No Kes {datakesAsasTindakan.value.NoKes} {datakesAsasTindakan.value.KodTujuan} {datakesAsasTindakan.value.KodAsas}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", datakesAsasTindakan.index, totalDatas);
                    if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                    {
                        UpdateInfo($"Hantar data Data Kes Asas Tindakan...", datakesAsasTindakan.index, totalDatas);
                        responseResendData = await SendOnlineBll.SendDataKesAsasTindakan(datakesAsasTindakan.value.NoKes, this.Context, true);
                        UpdateInfo($"Hantar data Data Kes Asas Tindakan... " + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, datakesAsasTindakan.index, totalDatas);
                        if (!responseResendData.Success)
                        {
                            allDataAlreadySent = false;

                            UpdateInfo($"Hantar data Gagal...", datakesAsasTindakan.index, totalDatas);
                            SleepProcess();
                            UpdateInfo($"Writing to log...", datakesAsasTindakan.index, totalDatas);
                            SleepProcess();
                        }
                        else
                        {
                            UpdateInfo($"Update data to Sent", datakesAsasTindakan.index, totalDatas);
                            SleepProcess();
                            DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSql($"UPDATE tbdatakes_asastindakan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' WHERE Id = '{datakesAsasTindakan.value.Id}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}' ");
                        }
                    }
                    else
                    {
                        UpdateInfo($"Update data to Sent", datakesAsasTindakan.index, totalDatas);
                        SleepProcess();
                        DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSql($"UPDATE tbdatakes_asastindakan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' WHERE Id = '{datakesAsasTindakan.value.Id}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}' ");
                    }
                }
            }

            //tbdatakes_kesalahan
            //select count(*) as count from tbdatakes_kesalahan where nokes=<> and kodakta=<> and kodsalah = <>
            var tbDataKesKesalahans = DataAccessQuery<TbDataKesKesalahan>.GetAll(m => m.IsSendOnline != Enums.StatusOnline.Sent);
            if (tbDataKesKesalahans.Success && tbDataKesKesalahans.Datas != null)
            {
                var totalDatas = tbDataKesKesalahans.Datas.Count();
                progressBar1.Max = totalDatas;

                foreach (var datakesKesalahan in tbDataKesKesalahans.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Check data Data Kes Kesalahan, ID {datakesKesalahan.value.Id} - No Kes {datakesKesalahan.value.NoKes}...", datakesKesalahan.index, totalDatas);
                    query = $"select count(*) as count from tbdatakes_kesalahan where nokes='{datakesKesalahan.value.NoKes}' and kodakta='{datakesKesalahan.value.KodAkta}' and kodsalah = '{datakesKesalahan.value.KodSalah}'";

                    responseCheckData = await HttpClientService.CountAync(query);
                    UpdateInfo($"Check data Data Kes Kesalahan, ID {datakesKesalahan.value.Id} - No Kes {datakesKesalahan.value.NoKes}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", datakesKesalahan.index, totalDatas);
                    if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                    {
                        UpdateInfo($"Hantar data Data Kes Kesalahan...", datakesKesalahan.index, totalDatas);
                        responseResendData = await SendOnlineBll.SendDataKesKesalahan(datakesKesalahan.value.NoKes, this.Context, true);
                        UpdateInfo($"Hantar data Data Kes Kesalahan..." + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, datakesKesalahan.index, totalDatas);
                        if (!responseResendData.Success)
                        {
                            allDataAlreadySent = false;

                            UpdateInfo($"Hantar data Gagal...", datakesKesalahan.index, totalDatas);
                            SleepProcess();
                            UpdateInfo($"Writing to log...", datakesKesalahan.index, totalDatas);
                            SleepProcess();
                        }
                        else
                        {
                            UpdateInfo($"Update data to Sent", datakesKesalahan.index, totalDatas);

                            SleepProcess();
                            DataAccessQuery<TbDataKesKesalahan>.ExecuteSql($"UPDATE tbdatakes_kesalahan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' WHERE NoKes = '{datakesKesalahan.value.NoKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                        }
                    }
                    else
                    {
                        UpdateInfo($"Update data to Sent", datakesKesalahan.index, totalDatas);

                        SleepProcess();
                        DataAccessQuery<TbDataKesKesalahan>.ExecuteSql($"UPDATE tbdatakes_kesalahan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' WHERE NoKes = '{datakesKesalahan.value.NoKes}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                    }
                }
            }


            //tbdatakes_pesalah
            //select count(*) as count from tbdatakes_pesalah where nokes=<>
            var tbDataKesPesalahs = DataAccessQuery<TbDataKesPesalah>.GetAll(m => m.IsSendOnline != Enums.StatusOnline.Sent);
            if (tbDataKesPesalahs.Success && tbDataKesPesalahs.Datas != null)
            {
                var totalDatas = tbDataKesPesalahs.Datas.Count();
                progressBar1.Max = totalDatas;

                foreach (var datakespesalah in tbDataKesPesalahs.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Check data Data Kes Pesalah, ID {datakespesalah.value.Id} - No Kes {datakespesalah.value.NoKes}...", datakespesalah.index, totalDatas);
                    query = $"select count(*) as count from tbdatakes_pesalah where nokes='{datakespesalah.value.NoKes}' ";

                    responseCheckData = await HttpClientService.CountAync(query);
                    UpdateInfo($"Check data Data Kes Pesalah, ID {datakespesalah.value.Id} - No Kes {datakespesalah.value.NoKes}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", datakespesalah.index, totalDatas);
                    if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                    {
                        UpdateInfo($"Hantar data Data Kes Kesalahan...", datakespesalah.index, totalDatas);
                        responseResendData = await SendOnlineBll.SendDataKesPesalah(datakespesalah.value.NoKes, this.Context, true);
                        UpdateInfo($"Hantar data Data Kes Kesalahan..." + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, datakespesalah.index, totalDatas);
                        if (!responseResendData.Success)
                        {
                            allDataAlreadySent = false;

                            UpdateInfo($"Hantar data Gagal...", datakespesalah.index, totalDatas);
                            SleepProcess();
                            UpdateInfo($"Writing to log...", datakespesalah.index, totalDatas);
                            SleepProcess();
                        }
                        else
                        {
                            UpdateInfo($"Update data to Sent", datakespesalah.index, totalDatas);

                            SleepProcess();
                            datakespesalah.value.IsSendOnline = Enums.StatusOnline.Sent;
                            DataAccessQuery<TbDataKesPesalah>.Update(datakespesalah.value);

                            UpdateInfo($"Delete Data...", datakespesalah.index, totalDatas);

                            SleepProcess();
                            DataAccessQuery<TbDataKesPesalah>.Delete(datakespesalah.value);
                        }
                    }
                    else
                    {
                        UpdateInfo($"Update data to Sent", datakespesalah.index, totalDatas);

                        SleepProcess();
                        datakespesalah.value.IsSendOnline = Enums.StatusOnline.Sent;
                        DataAccessQuery<TbDataKesPesalah>.Update(datakespesalah.value);

                        UpdateInfo($"Delete Data...", datakespesalah.index, totalDatas);

                        SleepProcess();
                        DataAccessQuery<TbDataKesPesalah>.Delete(datakespesalah.value);
                    }
                }
            }

            //
            //** if response for both tbdatakes_kesalahan and tbdatakes_pesalah count = 0, write sql statement in error file for tbdatakes_pesalah_kesalahan
            //
            UpdateInfo($"Check TbDataKesPesalah...");
            var tbDataKesPesalahAlreadySent = DataAccessQuery<TbDataKesPesalah>.GetAll(m => m.IsSendOnline == Enums.StatusOnline.Sent);
            if (tbDataKesPesalahAlreadySent.Success && tbDataKesPesalahAlreadySent.Datas != null)
            {
                var totalDatas = tbDataKesPesalahAlreadySent.Datas.Count();
                progressBar1.Max = totalDatas;
                UpdateInfo($"Check TbDataKesPesalah...");

                foreach (var item in tbDataKesPesalahAlreadySent.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Check TbDataKesPesalah No Kes {item.value.NoKes}");

                    var dataKesKesalahan = DataAccessQuery<TbDataKesKesalahan>.Get(m => m.NoKes == item.value.NoKes && m.IsSendOnline == Enums.StatusOnline.Sent);

                    UpdateInfo($"Check TbDataKesPesalah and TbDataKesKesalahan({dataKesKesalahan.Success}) No Kes {item.value.NoKes}");

                    if (dataKesKesalahan.Success && dataKesKesalahan != null)
                    {
                        allDataAlreadySent = false;

                        UpdateInfo($"Check TbDataKesPesalah and TbDataKesKesalahan({dataKesKesalahan.Success}) No Kes {item.value.NoKes}, Write into Log...");

                        var queryDataKes = $" INSERT INTO tbdatakes_pesalah_kesalahan " +
                           $" (idpesalah, idkesalahan, kodcawangan, pgndaftar, trkhdaftar, pgnakhir, trkhakhir, nokes) " +
                           $" SELECT a.idpesalah, b.idkesalahan, a.kodcawangan, a.pgndaftar, a.trkhdaftar, a.pgnakhir, a.trkhakhir,  a.nokes " +
                           $" FROM tbdatakes_pesalah a, tbdatakes_kesalahan b " +
                           $" WHERE a.nokes = b.nokes AND a.nokes = '{item.value.NoKes}' AND a.STATUS = 1; ";
                        Log.WriteErrorRecords(queryDataKes);
                    }

                }
            }

            //var tbDataKesPesalahAlreadySent = DataAccessQuery<TbDataKesPesalah>.GetAll(m => m.NoKes == item.value.NoRujukan && m.IsSendOnline == Enums.StatusOnline.Sent);
            //var tbDataKesKesalahanAlreadySent = DataAccessQuery<TbDataKesKesalahan>.GetAll(m => m.NoKes == item.value.NoRujukan && m.IsSendOnline == Enums.StatusOnline.Sent);
            //if (tbDataKesPesalahAlreadySent.Datas.Any() && tbDataKesKesalahanAlreadySent.Datas.Any())
            //{
            //    var queryDataKes = $" INSERT INTO tbdatakes_pesalah_kesalahan " +
            //               $" (idpesalah, idkesalahan, kodcawangan, pgndaftar, trkhdaftar, pgnakhir, trkhakhir, nokes) " +
            //               $" SELECT a.idpesalah, b.idkesalahan, a.kodcawangan, a.pgndaftar, a.trkhdaftar, a.pgnakhir, a.trkhakhir,  a.nokes " +
            //               $" FROM tbdatakes_pesalah a, tbdatakes_kesalahan b " +
            //               $" WHERE a.nokes = b.nokes AND a.nokes = '{item.value.NoRujukan}' AND a.STATUS = 1; ";
            //    Log.WriteErrorRecords(queryDataKes);
            //}


            //tbkpp_asastindakan
            //select count(*) as count from tbkpp_asastindakan where norujukankpp=<> and kodtujuan=<> and kodasas = <>
            var tbKppAsasTindakans = DataAccessQuery<TbKppAsasTindakan>.GetAll(m => m.IsSendOnline != Enums.StatusOnline.Sent);
            if (tbKppAsasTindakans.Success && tbKppAsasTindakans.Datas != null)
            {
                var totalDatas = tbKppAsasTindakans.Datas.Count();
                progressBar1.Max = totalDatas;

                foreach (var kppAsasTindakan in tbKppAsasTindakans.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Check data KPP Asas Tindakan, No Rujukan {kppAsasTindakan.value.NoRujukanKpp} - No Tujuan {kppAsasTindakan.value.KodTujuan} - Kod Asas {kppAsasTindakan.value.KodAsas}...", kppAsasTindakan.index, totalDatas);
                    query = $"select count(*) as count from tbkpp_asastindakan where norujukankpp='{kppAsasTindakan.value.NoRujukanKpp}' and kodtujuan='{kppAsasTindakan.value.KodTujuan}' and kodasas = '{kppAsasTindakan.value.KodAsas}' ";

                    responseCheckData = await HttpClientService.CountAync(query);
                    UpdateInfo($"Check data KPP Asas Tindakan, No Rujukan {kppAsasTindakan.value.NoRujukanKpp} - No Tujuan {kppAsasTindakan.value.KodTujuan} - Kod Asas {kppAsasTindakan.value.KodAsas}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", kppAsasTindakan.index, totalDatas);
                    if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                    {
                        UpdateInfo($"Hantar data Data Kes Kesalahan...", kppAsasTindakan.index, totalDatas);
                        responseResendData = await SendOnlineBll.SendKppAsasTindakan(kppAsasTindakan.value.NoRujukanKpp, this.Context, true);
                        UpdateInfo($"Hantar data Data Kes Kesalahan..." + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, kppAsasTindakan.index, totalDatas);
                        if (!responseResendData.Success)
                        {
                            allDataAlreadySent = false;

                            UpdateInfo($"Hantar data Gagal...", kppAsasTindakan.index, totalDatas);
                            SleepProcess();
                            UpdateInfo($"Writing to log...", kppAsasTindakan.index, totalDatas);
                            SleepProcess();
                        }
                        else
                        {
                            UpdateInfo($"Update data to Sent", kppAsasTindakan.index, totalDatas);

                            SleepProcess();
                            DataAccessQuery<TbKppAsasTindakan>.ExecuteSql($"UPDATE tbkpp_asastindakan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' " +
                                              $"WHERE NoRujukanKpp = '{kppAsasTindakan.value.NoRujukanKpp}' AND KodTujuan = '{kppAsasTindakan.value.KodTujuan}' AND KodAsas = '{kppAsasTindakan.value.KodAsas}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");


                            //UpdateInfo($"Delete data...", kppAsasTindakan.index, totalDatas);
                            //
                            //SleepProcess();
                            //DataAccessQuery<TbKppAsasTindakan>.Delete(kppAsasTindakan.value);
                        }
                    }
                    else
                    {
                        UpdateInfo($"Update data to Sent", kppAsasTindakan.index, totalDatas);

                        SleepProcess();
                        DataAccessQuery<TbKppAsasTindakan>.ExecuteSql($"UPDATE tbkpp_asastindakan SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' " +
                                              $"WHERE NoRujukanKpp = '{kppAsasTindakan.value.NoRujukanKpp}' AND KodTujuan = '{kppAsasTindakan.value.KodTujuan}' AND KodAsas = '{kppAsasTindakan.value.KodAsas}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");

                        //UpdateInfo($"Delete data...", kppAsasTindakan.index, totalDatas);
                        //
                        //SleepProcess();
                        //DataAccessQuery<TbKppAsasTindakan>.Delete(kppAsasTindakan.value);
                    }
                }
            }

            //tbpasukan_trans
            //select count(*) as count from tbpasukan_trans where jenistrans=<> and norujukan=<> and kodpasukan=<> and idpengguna = <id>
            var tbPasukanTrans = DataAccessQuery<TbPasukanTrans>.GetAll(m => m.IsSendOnline != Enums.StatusOnline.Sent);
            if (tbPasukanTrans.Success && tbPasukanTrans.Datas != null)
            {
                var totalDatas = tbPasukanTrans.Datas.Count();
                progressBar1.Max = totalDatas;

                foreach (var pasukanTrans in tbPasukanTrans.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Check data Pasukan Trans, No Rujukan {pasukanTrans.value.NoRujukan} - Id {pasukanTrans.value.Id}...", pasukanTrans.index, totalDatas);
                    query = $"select count(*) as count from tbpasukan_trans where jenistrans='{pasukanTrans.value.JenisTrans}' and norujukan ='{pasukanTrans.value.NoRujukan}' and kodpasukan='{pasukanTrans.value.KodPasukan}' and idpengguna = '{pasukanTrans.value.Id}' ";

                    responseCheckData = await HttpClientService.CountAync(query);
                    UpdateInfo($"Check data Pasukan Trans, No Rujukan {pasukanTrans.value.NoRujukan} - Id {pasukanTrans.value.Id}...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseResendData.Mesage}", pasukanTrans.index, totalDatas);
                    if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                    {
                        UpdateInfo($"Hantar data Data Kes Kesalahan...", pasukanTrans.index, totalDatas);
                        responseResendData = await SendOnlineBll.SendPasukanTrans(pasukanTrans.value.NoRujukan, this.Context, true);
                        UpdateInfo($"Hantar data Data Kes Kesalahan..." + (responseResendData.Success ? "Sent " : "Gagal ") + responseResendData.Mesage, pasukanTrans.index, totalDatas);
                        if (!responseResendData.Success)
                        {
                            allDataAlreadySent = false;

                            UpdateInfo($"Hantar data Gagal...", pasukanTrans.index, totalDatas);
                            SleepProcess();
                            UpdateInfo($"Writing to log...", pasukanTrans.index, totalDatas);
                            SleepProcess();
                        }
                        else
                        {
                            UpdateInfo($"Update data to Sent", pasukanTrans.index, totalDatas);

                            SleepProcess();
                            DataAccessQuery<TbPasukanTrans>.ExecuteSql($"UPDATE tbpasukan_trans SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' WHERE id = '{pasukanTrans.value.Id}' AND NoRujukan = '{pasukanTrans.value.NoRujukan}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                        }
                    }
                    else
                    {
                        UpdateInfo($"Update data to Sent", pasukanTrans.index, totalDatas);

                        SleepProcess();
                        DataAccessQuery<TbPasukanTrans>.ExecuteSql($"UPDATE tbpasukan_trans SET IsSendOnline = '{(int)Enums.StatusOnline.Sent}' WHERE id = '{pasukanTrans.value.Id}' AND NoRujukan = '{pasukanTrans.value.NoRujukan}' AND IsSendOnline <> '{(int)Enums.StatusOnline.Sent}'");
                    }
                }
            }


            //-  response from API
            //- if count > 0 then just update data status in sqlite to sent
            //- if count = 0, write the sql statment into Errorrecordstmt_<hhid>_<yyyymmddh>.txt
            //

            //select from tbkpp(loop)
            //- call api to check existense : select count(*) as count from tbkpp where norujukankpp = <>
            //- if count > 0 then just delete that nokpp in tbkpp
            //- if count = 0, call API to resend. 
            //    if success = just delete that nokpp in tbkpp
            //    if failed = write the sql statment into Errorrecordstmt_<hhid> _<yyyymmddh>.txt
            var listOfKpp = DataAccessQuery<TbKpp>.GetAll();
            if (listOfKpp.Success && listOfKpp.Datas != null)
            {
                var totalDatas = listOfKpp.Datas.Count();
                query = string.Empty;

                foreach (var kpp in listOfKpp.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Check data KPP {kpp.value.NoRujukanKpp}", kpp.index, totalDatas);
                    query = $"select count(*) as count from tbkpp where norujukankpp = '{kpp.value.NoRujukanKpp}' ";
                    responseCheckData = await HttpClientService.CountAync(query);
                    UpdateInfo($"select count(*) as count from tbkpp where norujukankpp = '{kpp.value.NoRujukanKpp}'...{responseCheckData.Success}-{responseCheckData.Result?.Count}-{responseCheckData.Mesage}", kpp.index, totalDatas);
                    if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                    {
                        query = SendOnlineBll.GenerateSQLScriptForTableKpp(kpp.value);
                        UpdateInfo($"Resent KPP {kpp.value.NoRujukanKpp}", kpp.index, totalDatas);
                        var response = await HttpClientService.ExecuteQuery(query);
                        UpdateInfo($"Resent KPP {kpp.value.NoRujukanKpp} {response.Success}-{response.Mesage}", kpp.index, totalDatas);
                        if (!response.Success)
                        {
                            UpdateInfo($"Write KPP {kpp.value.NoRujukanKpp} to File", kpp.index, totalDatas);
                            Log.WriteErrorRecords(query);
                            allDataAlreadySent = false;
                        }
                        else
                        {
                            UpdateInfo($"Delete {kpp.value.NoRujukanKpp}", kpp.index, totalDatas);
                            DataAccessQuery<TbKpp>.Delete(kpp.value);
                        }
                    }
                    else
                    {
                        UpdateInfo($"Delete {kpp.value.NoRujukanKpp}", kpp.index, totalDatas);
                        DataAccessQuery<TbKpp>.Delete(kpp.value);
                    }
                }
            }



            //Delete Data 20210824
            var TbPasukanTransData = DataAccessQuery<TbPasukanTrans>.GetAll(m => m.IsSendOnline == Enums.StatusOnline.Sent);
            if (TbPasukanTransData.Success && TbPasukanTransData.Datas != null)
            {
                UpdateInfo($"Delete TbPasukanTrans");
                var totalDatas = TbPasukanTransData.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in TbPasukanTransData.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Delete TbPasukanTrans - {item.value.Id}", item.index, totalDatas);
                    DataAccessQuery<TbPasukanTrans>.Delete(item.value);
                }
            }
            SleepProcess();

            var TbKppAsasTindakanData = DataAccessQuery<TbKppAsasTindakan>.GetAll(m => m.IsSendOnline == Enums.StatusOnline.Sent);
            if (TbKppAsasTindakanData.Success && TbKppAsasTindakanData.Datas != null)
            {
                UpdateInfo($"Delete TbKppAsasTindakanData");
                var totalDatas = TbKppAsasTindakanData.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in TbKppAsasTindakanData.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Delete TbKppAsasTindakanData - {item.value.NoRujukanKpp}", item.index, totalDatas);
                    DataAccessQuery<TbKppAsasTindakan>.ExecuteSql($" Delete FROM tbkpp_asastindakan where NoRujukanKpp = '{item.value.NoRujukanKpp}' and KodTujuan = '{item.value.KodTujuan}' and KodAsas = '{item.value.KodAsas}' ");
                }
            }
            SleepProcess();

            var TbDataKesPesalahData = DataAccessQuery<TbDataKesPesalah>.GetAll(m => m.IsSendOnline == Enums.StatusOnline.Sent);
            if (TbDataKesPesalahData.Success && TbDataKesPesalahData.Datas != null)
            {
                UpdateInfo($"Delete TbDataKesPesalahData");
                var totalDatas = TbDataKesPesalahData.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in TbDataKesPesalahData.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Delete TbDataKesPesalahData - {item.value.Id}", item.index, totalDatas);
                    DataAccessQuery<TbDataKesPesalah>.Delete(item.value);
                }
            }
            SleepProcess();

            var TbDataKesAsasTindakanData = DataAccessQuery<TbDataKesAsasTindakan>.GetAll(m => m.IsSendOnline == Enums.StatusOnline.Sent);
            if (TbDataKesAsasTindakanData.Success && TbDataKesAsasTindakanData.Datas != null)
            {
                UpdateInfo($"Delete TbDataKesAsasTindakan");
                var totalDatas = TbDataKesAsasTindakanData.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in TbDataKesAsasTindakanData.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Delete TbDataKesAsasTindakan - {item.value.Id}", item.index, totalDatas);
                    DataAccessQuery<TbDataKesAsasTindakan>.Delete(item.value);
                }
            }
            SleepProcess();

            var TbDataKesKesalahanData = DataAccessQuery<TbDataKesKesalahan>.GetAll(m => m.IsSendOnline == Enums.StatusOnline.Sent);
            if (TbDataKesKesalahanData.Success && TbDataKesKesalahanData.Datas != null)
            {
                UpdateInfo($"Delete TbDataKesKesalahan");
                var totalDatas = TbDataKesKesalahanData.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in TbDataKesKesalahanData.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Delete TbDataKesKesalahan - {item.value.Id}", item.index, totalDatas);
                    DataAccessQuery<TbDataKesKesalahan>.Delete(item.value);
                }
            }
            SleepProcess();

            var TbDataKesData = DataAccessQuery<TbDataKes>.GetAll();
            if (TbDataKesData.Success && TbDataKesData.Datas != null)
            {
                UpdateInfo($"Delete TbDataKes");
                var totalDatas = TbDataKesData.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in TbDataKesData.Datas.Select((value, index) => new { index, value }))
                {
                    var count = DataAccessQuery<TbSendOnlineData>.Count(m =>
                                m.Status == Enums.StatusOnline.Sent && (m.Type == Enums.TableType.DataKes || m.Type == Enums.TableType.DataKes_HH));
                    UpdateInfo($"Delete TbDataKes - when DataKes and DataKes_HH already Sent : Count == 2 : ({count == 2})");
                    if (count == 2)
                    {
                        UpdateInfo($"Delete TbDataKes - {item.value.NoKes}", item.index, totalDatas);
                        DataAccessQuery<TbDataKes>.Delete(item.value);
                    }
                }
            }
            SleepProcess();

            var tbkompaun = DataAccessQuery<TbKompaun>.GetAll();
            if (tbkompaun.Success)
            {
                UpdateInfo($"Delete TbKompaun");

                var totalDatas = tbkompaun.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in tbkompaun.Datas.Select((value, index) => new { index, value }))
                {
                    var count = DataAccessQuery<TbSendOnlineData>.Count(m => m.NoRujukan == item.value.NoKmp && m.Status == Enums.StatusOnline.Sent
                        && (m.Type == Enums.TableType.Kompaun ||
                            m.Type == Enums.TableType.Kompaun_HH ||
                            m.Type == Enums.TableType.KompaunBayaran ||
                            m.Type == Enums.TableType.Akuan_UpdateKompaun ||
                            m.Type == Enums.TableType.Akuan_UpdateKompaun_HH));

                    UpdateInfo($"Delete TbDataKes - when Kompaun, Kompaun_HH, KompaunBayaran, Akuan_UpdateKompaun and Akuan_UpdateKompaun_HH - {count == 5}");
                    if (count == 5)
                    {
                        UpdateInfo($"Bersihkan data Kompaun {item.value.NoKmp}...", item.index, totalDatas);
                        DataAccessQuery<TbKompaun>.Delete(item.value);
                    }
                    else
                    {
                        var dataSendOnline = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == item.value.NoKmp);
                        if (dataSendOnline.Success && dataSendOnline.Datas == null)
                        {
                            UpdateInfo($"Bersihkan data Kompaun {item.value.NoKmp}...", item.index, totalDatas);
                            DataAccessQuery<TbKompaun>.Delete(item.value);
                        }
                    }
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            UpdateInfo($"Bersihkan datas Online Data");
            var TbSendOnlineDataData = DataAccessQuery<TbSendOnlineData>.GetAll();
            var tbSendData = DataAccessQuery<TbSendOnlineData>.GetAll();
            if (tbSendData.Success)
            {
                var datas = tbSendData.Datas.Where(m => m.Status == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan datas Online Data {item.value.NoRujukan}...", item.index, totalDatas);
                    DataAccessQuery<TbSendOnlineData>.Delete(item.value);
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            SleepProcess();

            return allDataAlreadySent;
        }

        private async Task<bool> SendAndCleanImagesFromTbSendOnlineGambar()
        {
            var allImagesAlreadySent = true;
            var imagesInsideBackupFolder = GeneralBll.GetAllImages(Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.BackupPath);

            var result = DataAccessQuery<TbSendOnlineGambar>.GetAll(m => m.Status != Enums.StatusOnline.Sent);
            if (result.Success && result.Datas != null)
            {
                var totalDatas = result.Datas.Count();
                progressBar1.Max = totalDatas;

                foreach (var item in result.Datas.Select((value, index) => new { index, value }))
                {
                    //Check the data, it's KPP, Kompaun or DataKes for get NoRujukan, KodCawangan, Status, PgnDaftar
                    var noRujukan = item.value.NoRujukan;
                    var kodCawangan = GeneralBll.GetUserCawangan();
                    var status = "1";
                    var pgnDaftar = GeneralBll.GetUserStaffId().ToString() == "0" ? "1" : GeneralBll.GetUserStaffId().ToString();
                    var trkhDaftar = GeneralBll.GetLocalDateTimeForDatabase();
                    var pgnAkhir = GeneralBll.GetUserStaffId().ToString();
                    var trkhAkhir = GeneralBll.GetLocalDateTimeForDatabase();
                    int kategory = noRujukan.StartsWith("KPP") ? 2 : (noRujukan.StartsWith("KTS") ? 1 : 3); //=> KPP

                    var kpp = PemeriksaanBll.GetPemeriksaanByRujukan(item.value.NoRujukan);
                    if (kpp != null)
                    {
                        noRujukan = item.value.NoRujukan;
                        status = kpp.Status;
                        pgnDaftar = kpp.PgnDaftar.ToString();
                        trkhDaftar = kpp.TrkhDaftar.ToString();
                        pgnAkhir = kpp.PgnAkhir.ToString();
                        trkhAkhir = kpp.TrkhAkhir.ToString();
                        kategory = 2;
                        kodCawangan = kpp.KodCawangan;
                    }
                    else
                    {
                        var kompaun = KompaunBll.GetKompaunByRujukan(item.value.NoRujukan);
                        if (kompaun.Success && kompaun.Datas != null)
                        {
                            noRujukan = item.value.NoRujukan;
                            status = kompaun.Datas.Status;
                            pgnDaftar = kompaun.Datas.PgnDaftar.ToString();
                            trkhDaftar = kompaun.Datas.TrkhDaftar.ToString();
                            pgnAkhir = kompaun.Datas.PgnAkhir.ToString();
                            trkhAkhir = kompaun.Datas.TrkhAkhir.ToString();
                            kategory = 3;
                            kodCawangan = kompaun.Datas.KodCawangan;
                        }
                        else
                        {
                            var dataKes = KompaunBll.GetSiasatByNoKes(item.value.NoRujukan);
                            if (dataKes != null)
                            {
                                noRujukan = item.value.NoRujukan;
                                status = dataKes.Status;
                                pgnDaftar = dataKes.PgnDaftar.ToString();
                                trkhDaftar = dataKes.TrkhDaftar.ToString();
                                pgnAkhir = dataKes.PgnAkhir.ToString();
                                trkhAkhir = dataKes.TrkhAkhir.ToString();
                                kategory = 1;
                                kodCawangan = dataKes.KodCawangan;
                            }
                        }
                    }
                    UpdateInfo($"----");
                    UpdateInfo($"Check physical Image for {item.value.NoRujukan} - {item.value.Name}...", item.index, totalDatas);
                    //Get Image
                    var imageLocation = Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.ImgsPath + item.value.Name;
                    if (!File.Exists(imageLocation))
                    {
                        UpdateInfo($"Image for {item.value.Name} - Not Exist in - {imageLocation}", item.index, totalDatas);

                        imageLocation = imagesInsideBackupFolder.FirstOrDefault(m => m.Contains(item.value.Name));
                        UpdateInfo($"Check physical Image for {item.value.Name} in Backup Folder", item.index, totalDatas);
                        if (!File.Exists(imageLocation))
                        {
                            UpdateInfo($"Check physical Image for {item.value.Name} in Backup Folder - Not Exist in Backup Folder", item.index, totalDatas);
                            imageLocation = string.Empty; //=> Image is not exists on IMGS and Backup Folder
                        }
                    }

                    if (string.IsNullOrEmpty(imageLocation))
                    {
                        UpdateInfo($"Image for {item.value.NoRujukan} - {item.value.Name} not exists", item.index, totalDatas);
                        SleepProcess();
                        UpdateAndDeleteImage(item.value, totalDatas, imageLocation, item.index);
                    }
                    else
                    {
                        UpdateInfo($"Image for {item.value.Name} Exists in Backup Folder - {imageLocation}", item.index, totalDatas);

                        UpdateInfo($"Check Image for {item.value.NoRujukan} - {item.value.Name}...", item.index, totalDatas);
                        //Check Image in Server
                        var query = $"SELECT  count(*) as count FROM tbkompaun_gambar where nokmp = '{item.value.NoRujukan}' and namagambar = '{item.value.Name}'";
                        if (kategory == 1)
                        {
                            query = $"SELECT  count(*) as count FROM tbdatakes_gambar where nokes = '{item.value.NoRujukan}' and namagambar = '{item.value.Name}'";
                        }
                        else if (kategory == 2)
                        {
                            query = $"SELECT  count(*) as count FROM tbkpp_gambar where nokpp = '{item.value.NoRujukan}' and namagambar = '{item.value.Name}'";
                        }

                        var responseCheckData = await HttpClientService.CountAync(query);
                        UpdateInfo($"Check Image for {item.value.NoRujukan} - {item.value.Name}...{responseCheckData.Success}-{responseCheckData.Mesage}", item.index, totalDatas);
                        if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                        {
                            UpdateInfo($"Resent Image for {item.value.NoRujukan} - {item.value.Name}...", item.index, totalDatas);
                            //ReSent Image and write the image name in LOG when error
                            var response = await SendOnlineBll.SendImageOnlinePath(imageLocation, noRujukan, kodCawangan, status, pgnDaftar, trkhDaftar, pgnAkhir, trkhAkhir, kategory, true);
                            UpdateInfo($"Resent Image for {item.value.NoRujukan} - {item.value.Name}...{response.Success}-{response.Mesage}", item.index, totalDatas);
                            if (!response.Success)
                            {
                                allImagesAlreadySent = false;
                            }
                            else
                            {
                                UpdateAndDeleteImage(item.value, totalDatas, imageLocation, item.index);
                            }
                        }
                        else
                        {
                            UpdateAndDeleteImage(item.value, totalDatas, imageLocation, item.index);
                        }
                    }
                }
            }

            return allImagesAlreadySent;
        }

        private async Task<bool> SendAndCleanImagesFromBackupPath()
        {
            var allImagesAlreadySent = true;

            var imagesInsideBackupFolder = GeneralBll.GetAllImages(Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.BackupPath);
            var imagesFiltered = imagesInsideBackupFolder.Where(m => m.Contains("KPP") || m.Contains("KTS") || m.Contains("DTK"));
            if (imagesFiltered.Any())
            {
                var totalDatas = imagesFiltered.Count();
                progressBar1.Max = totalDatas;

                foreach (var item in imagesFiltered.Select((value, index) => new { index, value }))
                {
                    var fullFileName = Path.GetFileName(item.value);
                    var fileName = fullFileName.Split('_')[1];

                    var type = fullFileName.Split('_')[1]?.Substring(0, 3);

                    //Check the data, it's KPP, Kompaun or DataKes for get NoRujukan, KodCawangan, Status, PgnDaftar
                    var noRujukan = fullFileName.Split('_')[1].Substring(0, 16);
                    var kodCawangan = fullFileName.Split('_')[1].Substring(3, 3);
                    var status = "1";
                    var pgnDaftar = GeneralBll.GetUserStaffId().ToString() == "0" ? "1" : GeneralBll.GetUserStaffId().ToString();
                    var trkhDaftar = GeneralBll.GetLocalDateTimeForDatabase();
                    var pgnAkhir = GeneralBll.GetUserStaffId().ToString();
                    var trkhAkhir = GeneralBll.GetLocalDateTimeForDatabase();
                    int kategory = type == "KPP" ? 2 : (type == "KTS" ? 1 : 3); //=> KPP

                    UpdateInfo($"----");
                    UpdateInfo($"Found Image in Backup Folder {item.value}", item.index, totalDatas);
                    UpdateInfo($"Send Image {fileName}", item.index, totalDatas);
                    var result = await CheckAndResendImageAsync(noRujukan, fullFileName, item.value, fileName, kategory, kodCawangan, status, pgnDaftar, trkhDaftar, pgnAkhir, trkhAkhir, item.index, totalDatas);
                    if (!result)
                        allImagesAlreadySent = false;

                    #region Moved to Object
                    ////Get Image
                    //var imageLocation = item.value;
                    //if (!File.Exists(imageLocation))
                    //{
                    //    continue;
                    //}

                    //if (string.IsNullOrEmpty(imageLocation))
                    //{
                    //    UpdateInfo($"Image for {noRujukan} - {item.value} not exists", item.index, totalDatas);
                    //    SleepProcess();
                    //    UpdateAndDeleteImage(totalDatas, imageLocation, item.index);
                    //}
                    //else
                    //{
                    //    UpdateInfo($"Check Image for {noRujukan} - {fullFileName}...", item.index, totalDatas);
                    //    //Check Image in Server
                    //    var query = $"SELECT  count(*) as count FROM tbkompaun_gambar where nokmp = '{noRujukan}' and namagambar = '{fileName}'";
                    //    if (kategory == 1)
                    //    {
                    //        query = $"SELECT  count(*) as count FROM tbdatakes_gambar where nokes = '{noRujukan}' and namagambar = '{fileName}'";
                    //    }
                    //    else if (kategory == 2)
                    //    {
                    //        query = $"SELECT  count(*) as count FROM tbkpp_gambar where nokpp = '{noRujukan}' and namagambar = '{fileName}'";
                    //    }

                    //    var responseCheckData = await HttpClientService.CountAync(query);
                    //    UpdateInfo($"Check Image for {noRujukan} - {fullFileName}...{responseCheckData.Success}-{responseCheckData.Mesage}", item.index, totalDatas);
                    //    if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                    //    {
                    //        UpdateInfo($"Resent Image for {noRujukan} - {fullFileName}...", item.index, totalDatas);
                    //        //ReSent Image and write the image name in LOG when error
                    //        var response = await SendOnlineBll.SendImageOnlinePath(imageLocation, noRujukan, kodCawangan, status, pgnDaftar, trkhDaftar, pgnAkhir, trkhAkhir, kategory, true);
                    //        UpdateInfo($"Resent Image for {noRujukan} - {fullFileName}...{response.Success}-{response.Mesage}", item.index, totalDatas);
                    //        if (!response.Success)
                    //        {
                    //            allImagesAlreadySent = false;
                    //        }
                    //        else
                    //        {
                    //            UpdateAndDeleteImage(totalDatas, imageLocation, item.index);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        UpdateAndDeleteImage(totalDatas, imageLocation, item.index);
                    //    }
                    //}
                    #endregion
                }
            }

            var imagesInsideIMGSFolder = GeneralBll.GetAllImages(Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.ImgsPath);
            imagesFiltered = imagesInsideIMGSFolder.Where(m => m.Contains("KPP") || m.Contains("KTS") || m.Contains("DTK"));
            if (imagesFiltered.Any())
            {
                var totalDatas = imagesFiltered.Count();
                progressBar1.Max = totalDatas;

                foreach (var item in imagesFiltered.Select((value, index) => new { index, value }))
                {
                    var fullFileName = Path.GetFileName(item.value);
                    var fileName = fullFileName;

                    var type = fullFileName.Substring(0, 3);

                    var noRujukan = fullFileName.Substring(0, 16);
                    var kodCawangan = fullFileName.Substring(3, 3);
                    var status = "1";
                    var pgnDaftar = GeneralBll.GetUserStaffId().ToString() == "0" ? "1" : GeneralBll.GetUserStaffId().ToString();
                    var trkhDaftar = GeneralBll.GetLocalDateTimeForDatabase();
                    var pgnAkhir = GeneralBll.GetUserStaffId().ToString();
                    var trkhAkhir = GeneralBll.GetLocalDateTimeForDatabase();
                    int kategory = type == "KPP" ? 2 : (type == "KTS" ? 1 : 3); //=> KPP

                    UpdateInfo($"Found Image in IMGS {item.value}", item.index, totalDatas);
                    UpdateInfo($"Send Image {fileName}", item.index, totalDatas);

                    var result = await CheckAndResendImageAsync(noRujukan, fullFileName, item.value, fileName, kategory, kodCawangan, status, pgnDaftar, trkhDaftar, pgnAkhir, trkhAkhir, item.index, totalDatas);
                    if (!result)
                        allImagesAlreadySent = false;
                }
            }


            return allImagesAlreadySent;
        }

        private async Task<bool> CheckAndResendImageAsync(string noRujukan, string fullFileName, string value, string fileName, int kategory, string kodCawangan, string status,
            string pgnDaftar, string trkhDaftar, string pgnAkhir, string trkhAkhir, int index, int totalDatas)
        {
            UpdateInfo($"Check physical Image for {noRujukan} - {fullFileName}... {value}", index, totalDatas);

            //Get Image
            var imageLocation = value;
            if (!File.Exists(imageLocation))
            {
                return true;
            }

            if (string.IsNullOrEmpty(imageLocation))
            {
                UpdateInfo($"Image for {noRujukan} - {value} not exists", index, totalDatas);
                SleepProcess();
                UpdateAndDeleteImage(totalDatas, imageLocation, index);
            }
            else
            {
                UpdateInfo($"Check Image for {noRujukan} - {fullFileName}...", index, totalDatas);
                //Check Image in Server
                var query = $"SELECT  count(*) as count FROM tbkompaun_gambar where nokmp = '{noRujukan}' and namagambar = '{fileName}'";
                if (kategory == 1)
                {
                    query = $"SELECT  count(*) as count FROM tbdatakes_gambar where nokes = '{noRujukan}' and namagambar = '{fileName}'";
                }
                else if (kategory == 2)
                {
                    query = $"SELECT  count(*) as count FROM tbkpp_gambar where nokpp = '{noRujukan}' and namagambar = '{fileName}'";
                }

                var responseCheckData = await HttpClientService.CountAync(query);
                UpdateInfo($"Check Image for {noRujukan} - {fullFileName}...{responseCheckData.Success}-{responseCheckData.Mesage}", index, totalDatas);
                if (!responseCheckData.Success || responseCheckData.Result?.Count == 0)
                {
                    UpdateInfo($"Resent Image for {noRujukan} - {fullFileName}...", index, totalDatas);
                    //ReSent Image and write the image name in LOG when error
                    var response = await SendOnlineBll.SendImageOnlinePath(imageLocation, noRujukan, kodCawangan, status, pgnDaftar, trkhDaftar, pgnAkhir, trkhAkhir, kategory, true);
                    UpdateInfo($"Resent Image for {noRujukan} - {fullFileName}...{response.Success}-{response.Mesage}", index, totalDatas);
                    if (!response.Success)
                    {
                        return false;
                    }
                    else
                    {
                        UpdateAndDeleteImage(totalDatas, imageLocation, index);
                    }
                }
                else
                {
                    UpdateAndDeleteImage(totalDatas, imageLocation, index);
                }
            }

            return true;
        }

        private void UpdateAndDeleteImage(TbSendOnlineGambar item, int totalData, string imageLocation, int index)
        {
            UpdateInfo($"Update data Image for {item.NoRujukan} - {item.Name} to Sent", index, totalData);
            SleepProcess();

            item.Status = Enums.StatusOnline.Sent;
            DataAccessQuery<TbSendOnlineGambar>.Update(item);

            UpdateInfo($"Delete Image {imageLocation} ", index, totalData);
            if (File.Exists(Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.ImgsPath + item.Name))
                File.Delete(Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.ImgsPath + item.Name);
            if (File.Exists(imageLocation))
                File.Delete(imageLocation);
        }

        private void UpdateAndDeleteImage(int totalData, string imageLocation, int index)
        {
            UpdateInfo($"Delete Image {imageLocation}", index, totalData);
            if (File.Exists(imageLocation))
                File.Delete(imageLocation);
        }

        private void SleepProcess()
        {
            Thread.Sleep(500);
        }
    }
}