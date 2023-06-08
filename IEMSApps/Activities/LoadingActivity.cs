using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.Activities
{
    [Activity(NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoadingActivity : BaseActivity
    {
        private TextView _lblInfo, _lblTotal, _lblPercentage;
        private ProgressBar _progressBar1;
        private string _pirantiId, _loadingType;
        private AlertDialog _alertDialog;
        private string _errorMesage;
        private string _urlNewAPK;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Loading);

            _lblInfo = FindViewById<TextView>(Resource.Id.lblInfo);
            _progressBar1 = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            _lblTotal = FindViewById<TextView>(Resource.Id.lblTotal);
            _lblPercentage = FindViewById<TextView>(Resource.Id.lblPercentage);

            _lblTotal.Text = string.Empty;
            _lblPercentage.Text = string.Empty;

            _pirantiId = Intent.GetStringExtra(Constants.PIRANTIID);
            _loadingType = Intent.GetStringExtra(Constants.LOADING_TYPE);
            _urlNewAPK = Intent.GetStringExtra(Constants.URL_NEW_APK);

            _alertDialog = GeneralAndroidClass.GetDialogCustom(this);
            _errorMesage = "";

            if (_loadingType == Constants.LOADING_PREPARE_DOWNLOAD_DATA)
            {
                new Task(() => { DownloadAndInsertRecord(); }).Start();
                //_hourGlass?.StartMessage(this, DownloadAndInsertRecord);
            }
            else if (_loadingType == Constants.LOADING_DOWNLOAD)
            {
                new Task(async () => { await DownloadAsync(); }).Start();
            }
            else
            {
                new Task(() => { GetRecordHandheldAsync(); }).Start();
            }
        }
        #region DownloadAnInsertDatas

        private void DownloadAndInsertRecord()
        {
            IsLoading(this, true);
            try
            {
                var cachePath = Path.GetTempPath();
                if (Directory.Exists(cachePath))
                    Directory.Delete(cachePath);

                if (!Directory.Exists(cachePath))
                    Directory.CreateDirectory(cachePath);

                //var cacheDirs = this.GetExternalCacheDirs();
                //if (cacheDirs != null)
                //{
                //    foreach (var item in cacheDirs)
                //    {
                //        File.Delete(item.AbsolutePath);
                //    }
                //}
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("LoadingActivity", "DownloadAndInsertRecord", ex.Message, Enums.LogType.Error);
            }


            GeneralAndroidClass.StopLocationService(this);
            GeneralAndroidClass.StopBackgroundService(this);
            Thread.Sleep(500);

            UpdateInfo("Memanggil API...");
            _progressBar1.Progress = 5;
            var totalData = 0;

            var result = HandheldBll.PrepareDownloadDatas(_pirantiId);
            _progressBar1.Progress = 50;
            IsLoading(this, false);

            if (result.Success)
            {
                _progressBar1.Progress = 100;
                UpdateCountAndPercentage(0, 0);

                for (int i = 1; i <= 18; i++)
                {
                    switch (i)
                    {
                        case 1:
                            deleteTbGpsLogs();
                            break;
                        case 2:

                            totalData = InsertNegeri(result);

                            //DataAccessQuery<TbNegeri>.DeleteAll();
                            //UpdateInfo(Constants.Messages.InsertData + " Negeri");
                            //
                            //totalData = result.Result.Tbnegeri.Count;
                            //_progressBar1.Max = totalData;
                            //
                            //foreach (var item in result.Result.Tbnegeri.Select((value, index) => new { index, value }))
                            //{
                            //    UpdateCountAndPercentage(item.index, totalData);
                            //    DataAccessQuery<TbNegeri>.ExecuteSql(item.value.Value);
                            //}
                            //UpdateCountAndPercentage(totalData, totalData);
                            break;
                        case 3:
                            InsertCawangan(result.Result.TbcawanganTemp);
                            break;
                        case 4:
                            totalData = InsertLokaliti(result);
                            break;
                        case 5:
                            totalData = InsertTujuanLawatan(result);
                            break;
                        case 6:
                            InsertAsasTindakan(result.Result.TbasastindakanTemp);
                            break;
                        case 7:
                            totalData = InsertKateegoryKawasan(result);
                            break;
                        case 8:
                            totalData = InsertKategoryPremis(result);
                            break;
                        case 9:
                            totalData = InsertKategoryPerniagaan(result);
                            break;
                        case 10:
                            InsertJenisPerniagaan(result.Result.TbjenisperniagaanTemp);
                            break;
                        case 11:
                            InsertPremis(result.Result.TbpremisTemp);
                            break;
                        case 12:
                            InsertAkta(result.Result.TbaktaTemp);
                            break;
                        case 13:
                            InsertKesalahan(result.Result.TbkesalahanTemp);
                            break;
                        case 14:
                            InsertBandar(result.Result.TbbandarTemp);
                            break;
                        case 15:
                            DataAccessQuery<TbSkipControl>.DeleteAll();
                            UpdateInfo(Constants.Messages.InsertData + " Skip Control");

                            totalData = result.Result.Tbskipcontrol.Count;
                            _progressBar1.Max = totalData;

                            foreach (var item in result.Result.Tbskipcontrol.Select((value, index) => new { index, value }))
                            {
                                UpdateCountAndPercentage(item.index, totalData);
                                DataAccessQuery<TbSkipControl>.ExecuteSql(item.value.Value);
                            }
                            UpdateCountAndPercentage(totalData, totalData);
                            break;
                        case 16:
                            totalData = InsertJenama(result);
                            break;
                        case 17:
                            totalData = InsertAgensiSerahan(result);
                            break;
                        case 18:
                            totalData =  InsertJenamaStesenMinyak(result);
                            break;

                    }
                    Thread.Sleep(500);
                }

                //Insert into Table TbPengguna
                InsertPengguna(result.Result.TbpenggunaTemp);

                //Insert into Table TbPasukanHh
                InsertTbPasukan(result);

                HandheldBll.UpdatePasukanAsync(_pirantiId, this);

                UpdateCountAndPercentage(0, 0);
                _progressBar1.Progress = 100;

#if !DEBUG
                HandheldBll.UpdateHandheldSetTrkhHhCheckin(_pirantiId, this);
#endif

                UpdateInfo("Selesai, Akan Kembali Ke Log Masuk");
                Thread.Sleep(2000);
                GoToLogin();
            }
            else
            {
                UpdateInfo("Ralat, Akan Kembali Ke Log Masuk");

                if (result.Mesage.Contains("Sambungan"))
                    _alertDialog.SetMessage("Tiada sambungan ke internet. Data tidak dapat diterima/dihantar");
                else
                    _alertDialog.SetMessage(result.Mesage);
                _alertDialog.SetButton2("OK", (s, e) => { });
                _alertDialog.DismissEvent += (s, e) =>
                {
                    GoToLogin();
                };

                RunOnUiThread(() =>
                    _alertDialog.Show()
                );
            }
        }

        private int InsertJenamaStesenMinyak(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Jenama Stesen Minyak Temp");
            DataAccessQuery<TbJenamaStesenMinyakTemp>.ExecuteSql("DELETE FROM tbjenama_stesen_minyak_temp");

            UpdateInfo(Constants.Messages.InsertData + " Jenama Stesen Minyak Temp");
            totalData = result.Result.TbJenama_Stesen_Minyak_Temp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.TbJenama_Stesen_Minyak_Temp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbJenamaStesenMinyakTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Jenama Stesen Minyak");
            DataAccessQuery<TbJenamaStesenMinyak>.ExecuteSql("DELETE FROM tbjenama_stesen_minyak WHERE kodjenama IN (SELECT kodjenama FROM tbjenama_stesen_minyak_temp)");

            UpdateInfo(Constants.Messages.Move + " Jenama Stesen Minyak");
            DataAccessQuery<TbJenamaStesenMinyak>.ExecuteSql("INSERT INTO tbjenama_stesen_minyak(kodjenama, prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT kodjenama, prgn, PgnDaftar, TrkhDaftar FROM tbjenama_stesen_minyak_temp  WHERE Status = 1");
            return totalData;
        }

        private int InsertAgensiSerahan(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Jenis Agensi Serahan Temp");
            DataAccessQuery<TbAgensiSerahanTemp>.ExecuteSql("DELETE FROM tbagensiserahantemp");

            UpdateInfo(Constants.Messages.InsertData + " Jenis Agensi Serahan Temp");
            totalData = result.Result.TbAgensiSerahanTemp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.TbAgensiSerahanTemp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbAgensiSerahanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Jenis Agensi Serahan");
            DataAccessQuery<TbAgensiSerahan>.ExecuteSql("DELETE FROM tbagensiserahan WHERE kodserahagensi IN (SELECT kodserahagensi FROM tbagensiserahantemp)");

            UpdateInfo(Constants.Messages.Move + " Jenis Agensi Serahan");
            DataAccessQuery<TbAgensiSerahan>.ExecuteSql("INSERT INTO tbagensiserahan(kodserahagensi, prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT kodserahagensi, prgn, PgnDaftar, TrkhDaftar FROM tbagensiserahantemp  WHERE Status = 1");
            return totalData;
        }

        private int InsertJenama(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Jenis Jenama Temp");
            DataAccessQuery<TbJenamaTemp>.ExecuteSql("DELETE FROM tbbarang_jenamaTemp");

            UpdateInfo(Constants.Messages.InsertData + " Jenis Jenama Temp");
            totalData = result.Result.Tbbarang_jenamaTemp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.Tbbarang_jenamaTemp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbJenamaTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Jenis Jenama");
            DataAccessQuery<TbJenama>.ExecuteSql("DELETE FROM tbbarang_jenama WHERE KodJenama IN (SELECT KodJenama FROM tbbarang_jenamaTemp)");

            UpdateInfo(Constants.Messages.Move + " Kategori Perniagaan");
            DataAccessQuery<TbJenama>.ExecuteSql("INSERT INTO tbbarang_jenama(KodJenama, Prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT KodJenama, Prgn, PgnDaftar, TrkhDaftar FROM tbbarang_jenamaTemp  WHERE Status = 1");
            return totalData;
        }

        private int InsertKategoryPerniagaan(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Kategori Perniagaan Temp");
            DataAccessQuery<TbKategoriPerniagaanTemp>.ExecuteSql("DELETE FROM tbkategoriperniagaanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Kategori Perniagaan Temp");
            totalData = result.Result.TbkategoriperniagaanTemp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.TbkategoriperniagaanTemp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbKategoriPerniagaanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Kategori Perniagaan");
            DataAccessQuery<TbKategoriPerniagaan>.ExecuteSql("DELETE FROM tbkategoriperniagaan WHERE KodKatPerniagaan IN (SELECT KodKatPerniagaan FROM tbkategoriperniagaanTemp)");

            UpdateInfo(Constants.Messages.Move + " Kategori Perniagaan");
            DataAccessQuery<TbKategoriPerniagaan>.ExecuteSql("INSERT INTO tbkategoriperniagaan(KodKatPerniagaan, Prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT KodKatPerniagaan, Prgn, PgnDaftar, TrkhDaftar FROM tbkategoriperniagaanTemp  WHERE Status = 1");
            return totalData;
        }

        private int InsertKategoryPremis(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Kategori Premis Temp");
            DataAccessQuery<TbKategoriPremisTemp>.ExecuteSql("DELETE FROM tbkategoripremisTemp");

            UpdateInfo(Constants.Messages.InsertData + " Kategori Premis Temp");
            totalData = result.Result.TbkategoripremisTemp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.TbkategoripremisTemp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbKategoriPremisTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Kategori Premis");
            DataAccessQuery<TbKategoriPremis>.ExecuteSql("DELETE FROM tbkategoripremis WHERE KodKatPremis IN (SELECT KodKatPremis FROM tbkategoripremisTemp)");

            UpdateInfo(Constants.Messages.Move + " Kategori Premis");
            DataAccessQuery<TbKategoriPremis>.ExecuteSql("INSERT INTO tbkategoripremis(KodKatPremis, Prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT KodKatPremis, Prgn, PgnDaftar, TrkhDaftar FROM tbkategoripremisTemp  WHERE Status = 1");
            return totalData;
        }

        private int InsertKateegoryKawasan(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Kategori Kawasan Temp");
            DataAccessQuery<TbKategoriKawasanTemp>.ExecuteSql("DELETE FROM tbkategorikawasanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Kategori Kawasan Temp");
            totalData = result.Result.TbkategorikawasanTemp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.TbkategorikawasanTemp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbKategoriKawasanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Kategori Kawasan");
            DataAccessQuery<TbKategoriKawasan>.ExecuteSql("DELETE FROM tbkategorikawasan WHERE KodKatKawasan IN (SELECT KodKatKawasan FROM tbkategorikawasanTemp)");

            UpdateInfo(Constants.Messages.Move + " Kategori Kawasan");
            DataAccessQuery<TbKategoriKawasan>.ExecuteSql("INSERT INTO tbkategorikawasan(KodKatKawasan, Prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT KodKatKawasan, Prgn, PgnDaftar, TrkhDaftar FROM tbkategorikawasanTemp  WHERE Status = 1");
            return totalData;
        }

        private int InsertTujuanLawatan(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Tujuan Lawatan Temp");
            DataAccessQuery<TbTujuanLawatanTemp>.ExecuteSql("DELETE FROM tbtujuanlawatanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Tujuan Lawatan Temp");
            totalData = result.Result.TbtujuanlawatanTemp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.TbtujuanlawatanTemp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbTujuanLawatanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Tujuan Lawatan");
            DataAccessQuery<TbTujuanLawatan>.ExecuteSql("DELETE FROM tbtujuanlawatan WHERE KodTujuan IN (SELECT KodTujuan FROM tbtujuanlawatanTemp)");

            UpdateInfo(Constants.Messages.Move + " Tujuan Lawatan");
            DataAccessQuery<TbTujuanLawatan>.ExecuteSql("INSERT INTO tbtujuanlawatan(KodTujuan, Prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT KodTujuan, Prgn, PgnDaftar, TrkhDaftar FROM tbtujuanlawatanTemp  WHERE Status = 1");
            return totalData;
        }

        private int InsertNegeri(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Negeri Temp");
            DataAccessQuery<TbNegeriTemp>.ExecuteSql("DELETE FROM tbnegeriTemp");

            UpdateInfo(Constants.Messages.InsertData + " Negeri Temp");
            totalData = result.Result.TbnegeriTemp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.TbnegeriTemp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbNegeriTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Negeri");
            DataAccessQuery<TbNegeri>.ExecuteSql("DELETE FROM tbnegeri WHERE KodNegeri IN (SELECT KodNegeri FROM tbnegeriTemp)");

            UpdateInfo(Constants.Messages.Move + " Negeri");
            DataAccessQuery<TbNegeri>.ExecuteSql("INSERT INTO tbnegeri(KodNegeri, Prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT KodNegeri, Prgn, PgnDaftar, TrkhDaftar FROM tbnegeriTemp  WHERE Status = 1");
            return totalData;
        }

        private int InsertLokaliti(Response<DownloadDataResponse> result)
        {
            int totalData;
            UpdateInfo(Constants.Messages.HapusData + " Lokaliti/ Kategori Khas Temp");
            DataAccessQuery<TbLokalitiKategoriKhasTemp>.ExecuteSql("DELETE FROM TbLokalitiKategoriKhas Temp");

            UpdateInfo(Constants.Messages.InsertData + " Lokaliti/ Kategori Khas Temp");
            totalData = result.Result.TbLokaliti_Kategori_Khas_Temp.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.TbLokaliti_Kategori_Khas_Temp.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbLokalitiKategoriKhasTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Lokaliti/ Kategori Khas");
            DataAccessQuery<TbLokalitiKategoriKhas>.ExecuteSql("DELETE FROM tblokaliti_kategori_khas WHERE id IN (SELECT id FROM tblokaliti_kategori_khas_temp)");

            UpdateInfo(Constants.Messages.Move + " Lokaliti/ Kategori Khas");
            DataAccessQuery<TbLokalitiKategoriKhas>.ExecuteSql("INSERT INTO tblokaliti_kategori_khas(id, prgn, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT id, prgn, PgnDaftar, TrkhDaftar FROM tblokaliti_kategori_khas_temp WHERE Status = 1");
            return totalData;
        }

        private void InsertTbPasukan(BusinessObject.Response<BusinessObject.DownloadDataResponse> result)
        {
            int totalData;
            DataAccessQuery<TbPasukanHh>.DeleteAll();
            UpdateInfo(Constants.Messages.InsertData + " Pasukan");

            totalData = result.Result.Tbpasukan_hh.Count;
            _progressBar1.Max = totalData;

            foreach (var item in result.Result.Tbpasukan_hh.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbPasukanHh>.ExecuteSql(item.value.Value);
            }

            UpdateCountAndPercentage(totalData, totalData);
        }

        #endregion

        #region GetRecordHandled
        private void GetRecordHandheldAsync()
        {
            _errorMesage = "";
            try
            {
                Thread.Sleep(2000);

                UpdateInfo("Mendapatkan Data ...");
                _progressBar1.Progress = 10;

                var result = HandheldBll.GetRecordHandheldAsync(_pirantiId);

                UpdateInfo("Penyediaan Data ...");
                _progressBar1.Progress = 20;

                if (result.Success)
                {
                    var data = result.Result.SingleOrDefault();
                    if (data.Status == "1") //Todo Need update data on server
                    {
                        UpdateInfo("Simpan data ...");
                        _progressBar1.Progress = 30;

                        HandheldBll.CreateFromApi(data);

                        _progressBar1.Progress = 70;
#if !DEBUG
                        HandheldBll.UpdateRecordHandheldAsync(_pirantiId, GeneralBll.GetUserStaffId().ToString(), this);
#endif

                        UpdateInfo("Sila Tunggu. Kembali Ke Log Masuk ...");
                        _progressBar1.Progress = 95;
                        Thread.Sleep(2000);
                        GoToLogin();

                        return;
                    }
                    else if (data.Status == "2")
                    {
                        _errorMesage = $"ID Peranti {_pirantiId} sudah tidak aktif. Sila masukan ID Peranti lain.";
                    }
                    else
                    {
                        _errorMesage = $"ID Peranti {_pirantiId} telah digunakan. Sila masukan ID Peranti lain.";
                    }
                }
                else
                    if (result.Mesage.Contains("Sambungan"))
                    _errorMesage = "Tiada sambungan ke internet. Data tidak dapat diterima/dihantar";
                else
                    _errorMesage = $"ID Peranti {_pirantiId} tidak wujud. Sila hubungi Administrator.";

                _alertDialog.SetMessage(_errorMesage);
                _alertDialog.SetButton2("OK", (s, e) => { });
                _alertDialog.DismissEvent += (s, e) =>
                {
                    GoToSplashScreen();
                };

                //UpdateInfo(message);
                //
                //Thread.Sleep(2000);
                //GoToSplashScreen();

                if (!string.IsNullOrEmpty(_errorMesage))
                {
                    UpdateInfo(_errorMesage);
                    RunOnUiThread(() =>
                        //_ErrorButton.Visibility = Android.Views.ViewStates.Visible
                        _alertDialog.Show()
                    );

                }
            }
            catch (Exception ex)
            {
                _lblInfo.Text = ex.Message;
                Thread.Sleep(2000);
                GoToSplashScreen();
            }
        }

        #endregion

        #region Download

        private async Task DownloadAsync()
        {
            try
            {
                string path = GeneralBll.GetAPKFolderPath();
                string filePath = Path.Combine(path, Constants.NEW_APK_NAME);

                if (File.Exists(filePath))
                    File.Delete(filePath);

                var totalBytes = 0;
                var receivedBytes = 0;
                var received = 0;
                var total = 0;
                UpdateInfo(Constants.Messages.Downloading);

                WebClient client = new WebClient();
                using (var stream = await client.OpenReadTaskAsync($"{_urlNewAPK}/{Constants.NEW_APK_NAME}"))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var buffer = new byte[8 * 1024];
                        int read = 0;
                        totalBytes = Int32.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);
                        _progressBar1.Max = unchecked((int)totalBytes);

                        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);//important : receive every buffer
                            receivedBytes += read;
                            received = unchecked((int)receivedBytes);
                            total = unchecked((int)totalBytes);

                            var percentage = ((float)received) / total;
                            UpdateCountAndPercentage(receivedBytes, total, true);
                            Console.WriteLine("{0}    Memuat turun {1} daripada {2} bait. {3} % selesai...",
                                            "-",
                                            receivedBytes,
                                            total,
                                            percentage);

                        }//END while
                        Stream ALLstream = new MemoryStream(ms.ToArray());//important change Stream


                        var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                        ALLstream.CopyTo(fileStream);
                        fileStream.Dispose();

                    }//END using (MemoryStream
                    stream.Close();
                }//END using (var stream

                UpdateInfo("Pemasangan...");

                Java.IO.File file = new Java.IO.File(filePath);

                Android.Net.Uri apkURI = Android.Net.Uri.FromFile(file); //for Build.VERSION.SDK_INT <= 24
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    apkURI = FileProvider.GetUriForFile(this, this.PackageName + ".fileprovider", file);
                }

                Intent intent = new Intent(Intent.ActionView);
                intent.PutExtra(Intent.ExtraNotUnknownSource, true);
                intent.SetDataAndType(apkURI, "application/vnd.android" + ".package-archive");
                intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                UpdateInfo("Ralat " + ex.Message);
                Thread.Sleep(2000);
                GoToSplashScreen();
            }

        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //UpdateInfo("Download Complete, Will redirect to Login");
            //Thread.Sleep(2000);
            //GoToSplashScreen();
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("{0} Memuat turun {1} daripada {2} bait. {3} % selesai...",
                 "-",
                 e.BytesReceived,
                 e.TotalBytesToReceive,
                 e.ProgressPercentage);

            UpdateCountAndPercentage(e.ProgressPercentage, 100);
        }


        #endregion

        private void UpdateInfo(string message)
        {
            RunOnUiThread(() => _lblInfo.Text = message);
        }

        private void UpdateCountAndPercentage(int index, int total, bool isDownloading = false)
        {
            RunOnUiThread(() =>
            {
                _progressBar1.Progress = index;

                if (index != 0 && total != 0)
                {
                    if (isDownloading)
                    {
                        _lblTotal.Text = $"Memuat turun {String.Format("{0:0.0}", (double)index / 1024 / 1024)}/{String.Format("{0:0.0 Mb}", (double)total / 1024 / 1024)}";
                        _lblPercentage.Text = $"{(int)Math.Round((double)(100 * index) / total)}%";
                    }
                    else
                    {
                        _lblTotal.Text = $"Data {index}/{total}";
                        _lblPercentage.Text = $"{(int)Math.Round((double)(100 * index) / total)}%";
                    }

                }
                else
                {
                    _lblTotal.Text = string.Empty;
                    _lblPercentage.Text = string.Empty;
                }

            });
        }

        private void GoToLogin()
        {
            var intent = new Intent(this, typeof(Login));
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
            Finish();
        }

        private void GoToSplashScreen()
        {
            var intent = new Intent(this, typeof(SplashScreen));
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
            Finish();
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
                return true;
            return base.OnKeyDown(keyCode, e);
        }

        #region TbGpsLogs        

        private void deleteTbGpsLogs ()
        {
            var items = DataAccessQuery<TbGpsLog>.GetAll();

            UpdateInfo(Constants.Messages.HapusData + " TbGpsLogs");

            var totalData = items.Datas.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Datas.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbGpsLog>.Delete(item.value);
            }
            UpdateCountAndPercentage(totalData, totalData);

        }

        #endregion

        #region Penggguna        

        private void InsertPengguna(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Pengguna Temp");
            DataAccessQuery<TbPenggunaTemp>.ExecuteSql("DELETE FROM tbpenggunatemp");

            UpdateInfo(Constants.Messages.InsertData + " Pengguna Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbPenggunaTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Pengguna");
            DataAccessQuery<TbPengguna>.ExecuteSql("DELETE FROM tbpengguna WHERE ID IN (SELECT ID FROM tbpenggunatemp)");

            UpdateInfo(Constants.Messages.Move + " Pengguna");
            DataAccessQuery<TbPengguna>.ExecuteSql("INSERT INTO tbpengguna(ID, NoKp, Kata_Laluan, KodCawangan, Nama, Nama_Bahagian, Nama_Gelaran, Nama_Gelaran_Jawatan, Nama_Jawatan, Gred, Singkatan_Jawatan, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT ID, NoKp, Kata_Laluan, KodCawangan, Nama, Nama_Bahagian, Nama_Gelaran, Nama_Gelaran_Jawatan, Nama_Jawatan, Gred, Singkatan_Jawatan, PgnDaftar, TrkhDaftar FROM tbpenggunatemp WHERE Status = 1");
        }

        #endregion

        #region Cawangan
        private void InsertCawangan(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Cawangan Temp");
            DataAccessQuery<TbCawanganTemp>.ExecuteSql("DELETE FROM tbcawanganTemp");

            UpdateInfo(Constants.Messages.InsertData + " Cawangan Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbCawanganTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Cawangan");
            DataAccessQuery<TbCawangan>.ExecuteSql("DELETE FROM tbcawangan WHERE KodCawangan IN (SELECT KodCawangan FROM tbcawanganTemp)");

            UpdateInfo(Constants.Messages.Move + " Cawangan");
            DataAccessQuery<TbCawangan>.ExecuteSql("INSERT INTO tbcawangan(KodCawangan, Prgn, Nama_Cawangan, Alamat1, Alamat2, Poskod, KodNegeri, Bandar, Emel, No_Faks, No_Telefon, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT KodCawangan, Prgn, Nama_Cawangan, Alamat1, Alamat2, Poskod, KodNegeri, Bandar, Emel, No_Faks, No_Telefon, PgnDaftar, TrkhDaftar FROM tbcawanganTemp  WHERE Status = 1");

        }
        #endregion

        #region tbasastindakantemp
        private void InsertAsasTindakan(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Asas Tindakan Temp");
            DataAccessQuery<TbAsasTindakanTemp>.ExecuteSql("DELETE FROM tbasastindakanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Asas Tindakan Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbAsasTindakanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Asas Tindakan");
            DataAccessQuery<TbAsasTindakan>.ExecuteSql("DELETE FROM tbasastindakan WHERE KodTujuan||'-'||KodAsas IN (SELECT KodTujuan||'-'||KodAsas FROM tbasastindakanTemp)");

            UpdateInfo(Constants.Messages.Move + " Asas Tindakan");
            DataAccessQuery<TbAsasTindakan>.ExecuteSql("INSERT INTO tbasastindakan(KodTujuan, KodAsas, Prgn, PgnDaftar, TrkhDaftar) " +
                                                       "SELECT KodTujuan, KodAsas, Prgn, PgnDaftar, TrkhDaftar FROM tbasastindakanTemp WHERE Status = 1");
        }
        #endregion

        #region tbjenisperniagaantemp
        private void InsertJenisPerniagaan(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Jenis Perniagaan Temp");
            DataAccessQuery<TbJenisPerniagaanTemp>.ExecuteSql("DELETE FROM tbjenisperniagaanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Jenis Perniagaan Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbJenisPerniagaanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Jenis Perniagaan");
            DataAccessQuery<TbJenisPerniagaan>.ExecuteSql("DELETE FROM tbjenisperniagaan WHERE KodJenis IN (SELECT KodJenis FROM tbjenisperniagaanTemp)");

            UpdateInfo(Constants.Messages.Move + " Jenis Perniagaan");
            DataAccessQuery<TbJenisPerniagaan>.ExecuteSql("INSERT INTO tbjenisperniagaan(KodJenis, Prgn, PgnDaftar, TrkhDaftar) " +
                                                       "SELECT KodJenis, Prgn, PgnDaftar, TrkhDaftar FROM tbjenisperniagaanTemp WHERE Status = 1");
        }
        #endregion

        #region TbPremistemp
        private void InsertPremis(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Premis Temp");
            DataAccessQuery<TbPremisTemp>.ExecuteSql("DELETE FROM tbpremisTemp");

            UpdateInfo(Constants.Messages.InsertData + " Premis Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbPremisTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Premis");
            DataAccessQuery<TbPremis>.ExecuteSql("DELETE FROM tbpremis WHERE IdPremis IN (SELECT IdPremis FROM tbpremisTemp)");

            UpdateInfo(Constants.Messages.Move + " Premis");
            DataAccessQuery<TbPremis>.ExecuteSql("INSERT INTO tbpremis(IdPremis, KodCawangan, NamaPremis, AlamatPremis1, AlamatPremis2, AlamatPremis3, NoDaftarPremis, PgnDaftar, TrkhDaftar) " +
                                                 "SELECT IdPremis, KodCawangan, NamaPremis, AlamatPremis1, AlamatPremis2, AlamatPremis3, NoDaftarPremis, PgnDaftar, TrkhDaftar FROM tbpremisTemp WHERE Status = 1");
        }
        #endregion

        #region TbAktatemp
        private void InsertAkta(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Akta Temp");
            DataAccessQuery<TbAktaTemp>.ExecuteSql("DELETE FROM tbaktaTemp");

            UpdateInfo(Constants.Messages.InsertData + " Akta Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbAktaTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Akta");
            DataAccessQuery<TbAkta>.ExecuteSql("DELETE FROM tbakta WHERE KodAkta IN (SELECT KodAkta FROM tbaktaTemp)");

            UpdateInfo(Constants.Messages.Move + " Akta");
            DataAccessQuery<TbAkta>.ExecuteSql("INSERT INTO tbakta(KodAkta, KodAktaKump, Prgn, PerintahPeraturan, Tajuk1, Tajuk2, Tajuk3, Tajuk4, Tajuk5, Daripada1, Daripada2, Perenggan1, Perenggan2, Perenggan3, Perenggan4, Pengeluar1, Pengeluar2, PgnDaftar, TrkhDaftar) " +
                                               "SELECT KodAkta, KodAktaKump, Prgn, PerintahPeraturan, Tajuk1, Tajuk2, Tajuk3, Tajuk4, Tajuk5, Daripada1, Daripada2, Perenggan1, Perenggan2, Perenggan3, Perenggan4, Pengeluar1, Pengeluar2, PgnDaftar, TrkhDaftar FROM tbaktaTemp WHERE Status = 1");
        }
        #endregion

        #region TbKesalahantemp
        private void InsertKesalahan(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Kesalahan Temp");
            DataAccessQuery<TbKesalahanTemp>.ExecuteSql("DELETE FROM tbkesalahanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Kesalahan Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbKesalahanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Kesalahan");
            DataAccessQuery<TbKesalahan>.ExecuteSql("DELETE FROM tbkesalahan WHERE KodAkta||'-'||KodSalah IN (SELECT KodAkta||'-'||KodSalah FROM tbkesalahanTemp)");

            UpdateInfo(Constants.Messages.Move + " Kesalahan");
            DataAccessQuery<TbKesalahan>.ExecuteSql("INSERT INTO tbkesalahan (KodAkta, KodSalah, Seksyen, Prgn, AmnKmp_Ind, AmnKmp_Ind_Word, AmnKmp_Sya, AmnKmp_Sya_Word, TempohHari_Tetap, TempohHari_Terbuka, KOTS, PgnDaftar, TrkhDaftar) " +
                                               "SELECT KodAkta, KodSalah, Seksyen, Prgn, AmnKmp_Ind, AmnKmp_Ind_Word, AmnKmp_Sya, AmnKmp_Sya_Word, TempohHari_Tetap, TempohHari_Terbuka, KOTS, PgnDaftar, TrkhDaftar FROM tbkesalahanTemp WHERE Status = 1");
        }
        #endregion

        #region TbBandartemp
        private void InsertBandar(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Bandar Temp");
            DataAccessQuery<TbBandarTemp>.ExecuteSql("DELETE FROM tbbandarTemp");

            UpdateInfo(Constants.Messages.InsertData + " Bandar Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbBandarTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Bandar");
            DataAccessQuery<TbBandar>.ExecuteSql("DELETE FROM tbbandar WHERE KodNegeri||'-'||KodBandar IN (SELECT KodNegeri||'-'||KodBandar FROM tbbandarTemp)");

            UpdateInfo(Constants.Messages.Move + " Bandar");
            DataAccessQuery<TbBandar>.ExecuteSql("INSERT INTO tbbandar(KodNegeri, KodBandar, Prgn, PgnDaftar, TrkhDaftar) " +
                                               "SELECT KodNegeri, KodBandar, Prgn, PgnDaftar, TrkhDaftar FROM tbbandarTemp WHERE Status = 1");

        }
        #endregion        
    }
}

//private void Dynamic<T>(string tableName, string tableTempName, List<ValueResponse> items) where T : class
//{
//    UpdateInfo(Constants.Messages.HapusData + $"{tableName}Temp");
//    DataAccessQuery<T>.ExecuteSql($"DELETE FROM {tableTempName}");

//    UpdateInfo(Constants.Messages.InsertData + " Pengguna Temp");
//    var totalData = items.Count;
//    _progressBar1.Max = totalData;

//    foreach (var item in items.Select((value, index) => new { index, value }))
//    {
//        UpdateCountAndPercentage(item.index, totalData);
//        DataAccessQuery<T>.ExecuteSql(item.value.Value);
//    }
//    UpdateCountAndPercentage(totalData, totalData);

//    UpdateInfo(Constants.Messages.HapusData + $" {tableName}");
//    DataAccessQuery<TbPengguna>.ExecuteSql($"DELETE FROM {tableName} WHERE ID IN (SELECT ID FROM {tableTempName})");

//    UpdateInfo(Constants.Messages.Move + $" {tableName}");
//    DataAccessQuery<TbPengguna>.ExecuteSql($"INSERT INTO {tableName} " +
//                                           "SELECT ID, NoKp, Kata_Laluan, KodCawangan, Nama, Nama_Bahagian, Nama_Gelaran, Nama_Gelaran_Jawatan, Nama_Jawatan, Gred, Singkatan_Jawatan, PgnDaftar, TrkhDaftar FROM tbpenggunatemp");
//}