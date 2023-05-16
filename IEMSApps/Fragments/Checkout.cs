using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.Activities;
using IEMSApps.BLL;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using static IEMSApps.Utils.Enums;

namespace IEMSApps.Fragments
{
    public class Checkout : Fragment
    {
        private const string LayoutName = "Checkout";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.CheckOutLayout, container, false);
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
            new Task(async () => { await OnCheckoutAsync(); }).Start();
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

        private async Task OnCheckoutAsync()
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
                }
                else
                {
#if true

                    var response = HandheldBll.GetHandheldData();
                    if (response.Success)
                    {
                        response.Datas.TrkhHhCheckout = GeneralBll.GetLocalDateTimeForDatabase();
                        DataAccessQuery<TbHandheld>.Update(response.Datas);
                    }

                    ShowCheckutProcess(true);
                    //Send Online
                    UpdateInfo("Sila Tunggu...");

                    UpdateInfo("Servis Dihentikan...");
                    GeneralAndroidClass.StopLocationService(this.Context);
                    GeneralAndroidClass.StopBackgroundService(this.Context);
                    Thread.Sleep(2000);

                    Log.WriteLogFile("Checkout", "OnCheckoutAsync", "Start Checkout", Enums.LogType.Info);

                    UpdateInfo("Hantar Data...");
                    var respon = await SendDatasAsync();
                    if (!string.IsNullOrEmpty(respon.Mesage))
                    {
                        UpdateInfo($"Gagal berhubung ke server IEMS. Sila cuba lagi atau hubungi admin sistem", 0, 0);
                        ShowCheckutProcess(false);
                        return;
                    }
                    UpdateInfo("Hantar Gambar...");
                    await SendImagesAsync();
                    UpdateInfo("Hantar Log GPS...");
                    await SendGPSLogAsync();
                    UpdateInfo("Kemaskini Handheld...");
                    await SendHandheldAsync();

                    UpdateInfo("Semak Data Ralat...");
                    //New Function Send _HH table when error

                    var dataNotSent = SearchBll.GetSearchData(new BusinessObject.Inputs.SearchDataInput());
                    var totalDataNotSent = dataNotSent.Count(m => !m.IsSent);

                    if (totalDataNotSent > 0)
                    {
                        UpdateInfo($"Terdapat {totalDataNotSent} gagal dikemaskini ke Sistem Utama IEMS. Sila semak talian internet anda dan lakukan semula Proses Tamat Tugas", 0, 0);
                        ShowCheckutProcess(false);
                        return;
                    }
#endif
                    await SendErrorDatasAsync();
                    UpdateInfo("Membuat Salinan...");
                    Backup();
#if true
                    UpdateInfo("Reset Transaksi...");
                    ResetTransactions();

                    UpdateInfo("Padam Gambar...");
                    DeleteImages();
#endif
                    UpdateInfo("Padam Log...");
                    DeleteLogs();
                    UpdateInfo("Padam Backup...");
                    DeleteBackupFolder();
                    UpdateInfo("Sila Tunggu...");
                    Thread.Sleep(1000);

                    UpdateInfo("Kembali Ke Skrin Log Masuk...");
                    Thread.Sleep(1000);

                    var intent = new Intent(this.Context, typeof(Login));
                    intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(intent);
                    this.Activity.Finish();
                }

            }
            catch (Exception ex)
            {
                UpdateInfo(ex.Message, 0, 0);

                GeneralBll.LogDataWithException("Checkout", "OnCheckoutAsync", ex);
            }
            finally
            {
                ShowCheckutProcess(false);
            }
        }

        private void DeleteImages()
        {
            var listOfImages = Directory.GetFiles(GeneralBll.GetInternalImagePath());
            var totalDatas = listOfImages.Count();
            progressBar1.Max = totalDatas;
            foreach (var item in listOfImages.Select((value, index) => new { index, value }))
            {
                //1. get data from TbSendOnlineGambar based on FileName and status not sent,
                //2. if the status not sent it will copy to backup folder
                var fileName = Path.GetFileName(item.value);
                var imageIsSent = DataAccessQuery<TbSendOnlineGambar>.Get(m => m.Status == Enums.StatusOnline.Sent && m.Name == fileName);
                if (imageIsSent.Success && imageIsSent.Datas != null)
                {
                    UpdateInfo($"Pemadaman...", item.index, totalDatas);
                    File.Delete(item.value);
                }
            }
            UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);


            //UpdateInfo($"Get all data from TbSendOnlineGambar...");
            ////Get all data from TbSendOnlineGambar
            //var imageDatas = DataAccessQuery<TbSendOnlineGambar>.GetAll();

            //UpdateInfo($"Get all images from IMGS Folder...");
            ////Get all images from IMGS Folder
            //var imagesInsideIMGSFolder = GeneralBll.GetAllImages(Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.ImgsPath);

            //var totalDatas = imagesInsideIMGSFolder.Count();
            //progressBar1.Max = totalDatas;
            //foreach (var image in imagesInsideIMGSFolder.Select((value, index) => new { index, value }))
            //{
            //    UpdateInfo($"Check {image}");
            //    var data = imageDatas.Datas.FirstOrDefault(m => m.Name.Contains(Path.GetFileNameWithoutExtension(image.value)));
            //    if (data != null)
            //    {
            //        UpdateInfo($"Image {Path.GetFileName(image.value)} exist in TbSendOnlineGambar and Status {data.Status}");
            //        if (data.Status == StatusOnline.Sent)
            //        {
            //            UpdateInfo($"Pemadaman...{Path.GetFileName(image.value)}");
            //            File.Delete(image.value);
            //        }
            //    }
            //    else
            //    {
            //        UpdateInfo($"Pemadaman...{Path.GetFileName(image.value)}");
            //        File.Delete(image.value);
            //    }
            //}

            //UpdateInfo($"Get all images from Backup Folder...");
            ////Get all images from Backup Folder
            //var imagesInsideBackupFolder = GeneralBll.GetAllImages(Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.BackupPath);
            //totalDatas = imagesInsideBackupFolder.Count();
            //progressBar1.Max = totalDatas;
            //foreach (var image in imagesInsideBackupFolder.Select((value, index) => new { index, value }))
            //{
            //    UpdateInfo($"Check {image}");
            //    var fullFileName = Path.GetFileNameWithoutExtension(image.value);
            //    var fileName = fullFileName.Split('_')[1];

            //    var data = imageDatas.Datas.FirstOrDefault(m => m.Name.Contains(fileName));
            //    if (data != null)
            //    {
            //        UpdateInfo($"Image {fileName} exist in TbSendOnlineGambar and Status {data.Status}");
            //        if (data.Status == StatusOnline.Sent)
            //        {
            //            UpdateInfo($"Pemadaman...{Path.GetFileName(image.value)}");
            //            File.Delete(image.value);
            //        }
            //    }
            //    else
            //    {
            //        UpdateInfo($"Pemadaman...{Path.GetFileName(image.value)}");
            //        File.Delete(image.value);
            //    }
            //}

            //UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
        }

        private void DeleteLogs()
        {
            var ListOfLogs = Directory.GetFiles(GeneralBll.GetInternalLogsPath());
            var totalDatas = ListOfLogs.Count();
            progressBar1.Max = totalDatas;
            foreach (var item in ListOfLogs.Select((value, index) => new { index, value }))
            {
                if (File.GetCreationTime(item.value) < DateTime.Now.AddMonths(-2))
                {
                    UpdateInfo($"Pemadaman...", item.index, totalDatas);
                    File.Delete(item.value);
                }
            }
            UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
        }

        private void DeleteBackupFolder()
        {
            //var ListOfLogs = Directory.GetDirectories(GeneralBll.GetInternalBackupPath());
            //var totalDatas = ListOfLogs.Count();
            //progressBar1.Max = totalDatas;
            //foreach (var item in ListOfLogs.Select((value, index) => new { index, value }))
            //{
            //    if (DateTime.ParseExact(Path.GetFileName(item.value), Constants.DateBackupFormat, null) < DateTime.Now.AddMonths(-2))
            //    {
            //        UpdateInfo($"Pemadaman...", item.index, totalDatas);
            //        Directory.Delete(item.value, true);
            //    }
            //}
            //UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);


            var ListOfLogs = Directory.GetDirectories(GeneralBll.GetInternalBackupPath());
            var totalDatas = ListOfLogs.Count();
            progressBar1.Max = totalDatas;
            foreach (var item in ListOfLogs.Select((value, index) => new { index, value }))
            {
                if (DateTime.ParseExact(Path.GetFileName(item.value), Constants.DateBackupFormat, null) < DateTime.Now.AddMonths(-2))
                {
                    var files = Directory.GetFiles(item.value, "*.*", SearchOption.AllDirectories).Where(m => m.ToLower().EndsWith("jpg") || m.ToLower().EndsWith("jpeg") || m.ToLower().EndsWith("png")).ToList();
                    var imagesFiles = files.Where(m => m.Contains("KPP") || m.Contains("KTS") || m.Contains("DTK"));
                    if (!imagesFiles.Any())
                    {
                        UpdateInfo($"Pemadaman...", item.index, totalDatas);
                        Directory.Delete(item.value, true);
                    }
                    else
                        UpdateInfo($"Skip Pemadaman for Folder {Path.GetFileName(item.value)}", item.index, totalDatas);
                }
            }
            UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
        }

        private void ResetTransactions()
        {
            var allDataAlreadySent = -1;

            var dataKes = DataAccessQuery<TbDataKes>.GetAll();
            if (dataKes.Success)
            {
                var totalDatas = dataKes.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in dataKes.Datas.Select((value, index) => new { index, value }))
                {
                    //Get total data from TbSendOnlineData based on noRujukan and Status = Sent,
                    //If total = 2 (DataKes && DataKes_HH already sent), it will delete data TbDataKes
                    allDataAlreadySent = DataAccessQuery<TbSendOnlineData>.Count(m => m.NoRujukan == item.value.NoKes && m.Status == Enums.StatusOnline.Sent
                        && (m.Type == Enums.TableType.DataKes || m.Type == Enums.TableType.DataKes_HH));
                    if (allDataAlreadySent == 2)
                    {
                        UpdateInfo($"Bersihkan data KES {item.value.NoKes}...", item.index, totalDatas);
                        DataAccessQuery<TbDataKes>.Delete(item.value);
                    }
                    else
                    {
                        var dataSendOnline = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == item.value.NoKmp);
                        if (dataSendOnline.Success && dataSendOnline.Datas == null)
                        {
                            UpdateInfo($"Bersihkan data KES {item.value.NoKmp}...", item.index, totalDatas);
                            DataAccessQuery<TbDataKes>.Delete(item.value);
                        }
                    }

                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbkompaun = DataAccessQuery<TbKompaun>.GetAll();
            if (tbkompaun.Success)
            {
                var totalDatas = tbkompaun.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in tbkompaun.Datas.Select((value, index) => new { index, value }))
                {
                    //Get total data from TbSendOnlineData based on noRujukan and Status = Sent,
                    //If total = 2 (Kompaun and Kompaun_HH already sent), it will delete data TbKompaun
                    allDataAlreadySent = DataAccessQuery<TbSendOnlineData>.Count(m => m.NoRujukan == item.value.NoKmp && m.Status == Enums.StatusOnline.Sent
                        && (m.Type == Enums.TableType.Kompaun ||
                            m.Type == Enums.TableType.Kompaun_HH ||
                            m.Type == TableType.KompaunBayaran ||
                            m.Type == TableType.Akuan_UpdateKompaun ||
                            m.Type == TableType.Akuan_UpdateKompaun_HH));
                    if (allDataAlreadySent == 5)
                    {
                        UpdateInfo($"Bersihkan data Kompaun {item.value.NoKmp}...", item.index, totalDatas);
                        DataAccessQuery<TbKompaun>.Delete(item.value);
                    }
                    if (allDataAlreadySent == 0)
                    {
                        UpdateInfo($"Bersihkan data Kompaun {item.value.NoKmp}...", item.index, totalDatas);
                        DataAccessQuery<TbKompaun>.Delete(item.value);
                    }
                    else
                    {
                        var dataSendOnline = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == item.value.NoKmp && m.Status == StatusOnline.Sent);
                        if (dataSendOnline.Success && dataSendOnline.Datas == null)
                        {
                            UpdateInfo($"Bersihkan data Kompaun {item.value.NoKmp}...", item.index, totalDatas);
                            DataAccessQuery<TbKompaun>.Delete(item.value);
                        }


                    }
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbKppAsasTindakan = DataAccessQuery<TbKppAsasTindakan>.GetAll();
            if (tbKppAsasTindakan.Success)
            {
                var datas = tbKppAsasTindakan.Datas.Where(m => m.IsSendOnline == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Kes Asas Tindakan {item.value.NoRujukanKpp}...", item.index, totalDatas);
                    DataAccessQuery<TbKppAsasTindakan>.ExecuteSql($" Delete FROM tbkpp_asastindakan where NoRujukanKpp = '{item.value.NoRujukanKpp}' and KodTujuan = '{item.value.KodTujuan}' and KodAsas = '{item.value.KodAsas}' ");
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbKompaunIzin = DataAccessQuery<TbKompaunIzin>.GetAll();
            if (tbKompaunIzin.Success)
            {
                var datas = tbKompaunIzin.Datas.Where(m => m.Status != Enums.StatusIzinKompaun.Waiting);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Kompaun Izin {item.value.NoRujukanKpp}...", item.index, totalDatas);
                    DataAccessQuery<TbKompaunIzin>.Delete(item.value);
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbkpp = DataAccessQuery<TbKpp>.GetAll();
            if (tbkpp.Success)
            {
                var totalDatas = tbkpp.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in tbkpp.Datas.Select((value, index) => new { index, value }))
                {
                    var total = DataAccessQuery<TbSendOnlineData>.Count(m => m.NoRujukan == item.value.NoRujukanKpp);
                    if (total == 2)
                    {
                        allDataAlreadySent = DataAccessQuery<TbSendOnlineData>.Count(m => m.NoRujukan == item.value.NoRujukanKpp && m.Status == Enums.StatusOnline.Sent && (m.Type == Enums.TableType.KPP || m.Type == Enums.TableType.KPP_HH));
                        if (allDataAlreadySent == 2)
                        {
                            UpdateInfo($"Bersihkan data KPP {item.value.NoRujukanKpp}...", item.index, totalDatas);
                            DataAccessQuery<TbKpp>.Delete(item.value);
                        }
                    }
                    else
                    {
                        var dataSendOnline = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == item.value.NoRujukanKpp && m.Status == StatusOnline.Sent);
                        if (dataSendOnline.Success && dataSendOnline.Datas != null)
                        {
                            UpdateInfo($"Bersihkan data KPP {item.value.NoRujukanKpp}...", item.index, totalDatas);
                            DataAccessQuery<TbKpp>.Delete(item.value);
                        }
                    }

                    ////Get total data from TbSendOnlineData based on noRujukan and Status = Sent,
                    ////If total = 2 (TbKpp and TbKpp_HH already sent), it will delete data TbKpp
                    //allDataAlreadySent = DataAccessQuery<TbSendOnlineData>.Count(m => m.NoRujukan == item.value.NoRujukanKpp && m.Status == Enums.StatusOnline.Sent && (m.Type == Enums.TableType.KPP || m.Type == Enums.TableType.KPP_HH));
                    //if (allDataAlreadySent == 2)
                    //{
                    //    UpdateInfo($"Bersihkan data KPP {item.value.NoRujukanKpp}...", item.index, totalDatas);
                    //    DataAccessQuery<TbKpp>.Delete(item.value);
                    //}
                    //else
                    //{
                    //    var dataSendOnline = DataAccessQuery<TbSendOnlineData>.Get(m => m.NoRujukan == item.value.NoRujukanKpp);
                    //    if (dataSendOnline.Success && dataSendOnline.Datas == null)
                    //    {
                    //        UpdateInfo($"Bersihkan data KPP {item.value.NoRujukanKpp}...", item.index, totalDatas);
                    //        DataAccessQuery<TbKpp>.Delete(item.value);
                    //    }
                    //}
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbPasukanHh = DataAccessQuery<TbPasukanHh>.GetAll();
            if (tbPasukanHh.Success)
            {
                var totalDatas = tbPasukanHh.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in tbPasukanHh.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Pasukan {item.value.NoKp}...", item.index, totalDatas);
                    DataAccessQuery<TbPasukanHh>.ExecuteSql($" Delete FROM tbpasukan_hh where id = '{item.value.Id}'");
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var kesPesalah = DataAccessQuery<TbDataKesPesalah>.GetAll();
            if (kesPesalah.Success)
            {
                var datas = kesPesalah.Datas.Where(m => m.IsSendOnline == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Kes Pesalah {item.value.NoKes}...", item.index, totalDatas);
                    DataAccessQuery<TbDataKesPesalah>.ExecuteSql($"DELETE FROM tbdatakes_pesalah WHERE NoKes = '{item.value.NoKes}' and Id = '{item.value.Id}' ");
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var kesKesalahan = DataAccessQuery<TbDataKesKesalahan>.GetAll();
            if (kesKesalahan.Success)
            {
                var datas = kesKesalahan.Datas.Where(m => m.IsSendOnline == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Kes Kesalahan {item.value.NoKes}...", item.index, totalDatas);
                    DataAccessQuery<TbDataKesKesalahan>.ExecuteSql($"DELETE FROM tbdatakes_kesalahan WHERE NoKes = '{item.value.NoKes}' and Id = '{item.value.Id}' ");
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var kesKesAsasTindakan = DataAccessQuery<TbDataKesAsasTindakan>.GetAll();
            if (kesKesAsasTindakan.Success)
            {
                var datas = kesKesAsasTindakan.Datas.Where(m => m.IsSendOnline == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Kes Asas Tindakan {item.value.NoKes}...", item.index, totalDatas);
                    DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSql($"DELETE FROM tbdatakes_asastindakan WHERE NoKes = '{item.value.NoKes}' and Id = '{item.value.Id}' ");
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var gpsLogs = DataAccessQuery<TbGpsLog>.GetAll();
            if (gpsLogs.Success)
            {
                var datas = gpsLogs.Datas.Where(m => m.IsSendOnline == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data GPS {item.value.Latitud}-{item.value.Longitud}...", item.index, totalDatas);
                    DataAccessQuery<TbGpsLog>.Delete(item.value);
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbPasukanTrans = DataAccessQuery<TbPasukanTrans>.GetAll();
            if (tbPasukanTrans.Success)
            {
                var datas = tbPasukanTrans.Datas.Where(m => m.IsSendOnline == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Pasukan Trans {item.value.NoRujukan}...", item.index, totalDatas);
                    DataAccessQuery<TbPasukanTrans>.ExecuteSql($"DELETE FROM tbpasukan_trans WHERE id = '{item.value.Id}'");
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbSendData = DataAccessQuery<TbSendOnlineData>.GetAll();
            if (tbSendData.Success)
            {
                var datas = tbSendData.Datas.Where(m => m.Status == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan datas {item.value.NoRujukan}...", item.index, totalDatas);
                    DataAccessQuery<TbSendOnlineData>.Delete(item.value);
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbSendDataGambar = DataAccessQuery<TbSendOnlineGambar>.GetAll();
            if (tbSendDataGambar.Success)
            {
                var datas = tbSendDataGambar.Datas.Where(m => m.Status == Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Image {item.value.NoRujukan}...", item.index, totalDatas);
                    DataAccessQuery<TbSendOnlineGambar>.Delete(item.value);
                    var imageLocation = Android.OS.Environment.ExternalStorageDirectory.AbsoluteFile + Constants.ProgramPath + Constants.ImgsPath + item.value.Name;
                    if (File.Exists(imageLocation))
                        File.Delete(imageLocation);
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var tbKompaunBayaran = DataAccessQuery<TbKompaunBayaran>.GetAll();
            if (tbKompaunBayaran.Success)
            {
                var kompaunBayaran = tbKompaunBayaran.Datas;
                var totalDatas = kompaunBayaran.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in kompaunBayaran.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Kompaun Bayran {item.value.nokmp}...", item.index, totalDatas);
                    DataAccessQuery<TbKompaunBayaran>.Delete(item.value);
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            var ipResits = DataAccessQuery<ip_resits>.GetAll();
            if (ipResits.Success)
            {
                var resit = ipResits.Datas;
                var totalDatas = resit.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in resit.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Bersihkan data Resit {item.value.no_resit}...", item.index, totalDatas);
                    DataAccessQuery<ip_resits>.Delete(item.value);
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            UpdateInfo($"Update Handheld...");
            var response = HandheldBll.GetHandheldData();
            if (response.Success)
            {
                response.Datas.TrkhHhCheckout = GeneralBll.GetLocalDateTimeForDatabase();
                response.Datas.Jumlah_Kpp = 0;
                response.Datas.Jumlah_Gambar_Kpp = 0;
                response.Datas.Jumlah_Kots = 0;
                response.Datas.Jumlah_Gambar_Kots = 0;
                response.Datas.Jumlah_Ak = 0;
                response.Datas.Jumlah_Gambar_Kots = 0;
                response.Datas.Jumlah_DataKes = 0;
                response.Datas.Jumlah_Nota = 0;
                response.Datas.PgnAkhir = GeneralBll.GetUserStaffId();
                response.Datas.TrkhAkhir = GeneralBll.GetLocalDateTimeForDatabase();

                DataAccessQuery<TbHandheld>.Update(response.Datas);
            }

            DatabaseBll.AlterDatabase();
        }

        private async Task SendHandheldAsync()
        {
            progressBar1.Max = 2;
            UpdateInfo("Send Handheld...", 1, 2);
            await SendOnlineBll.UpdateHandheldAsync(this.Context);
        }

        private async Task SendGPSLogAsync()
        {
            var result = DataAccessQuery<TbGpsLog>.GetAll();
            if (result.Success)
            {
                var datas = result.Datas.Where(m => m.IsSendOnline != Enums.StatusOnline.Sent);
                var totalDatas = datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Send {item.value.Latitud}-{item.value.Longitud}...", item.index, totalDatas);
                    await SendOnlineBll.SendGPSLog(item.value, this.Context);
                }
                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }
        }

        private async Task<Response<string>> SendDatasAsync()
        {
            var result = DataAccessQuery<TbSendOnlineData>.GetAll();
            if (result.Success)
            {
                //var datas = result.Datas.Where(m => m.Type != Enums.TableType.KompaunBayaran
                //        && m.Type != Enums.TableType.KPP_HH && m.Type != Enums.TableType.Kompaun_HH && m.Type != Enums.TableType.DataKes_HH && m.Type != Enums.TableType.Akuan_UpdateKompaun_HH);
                var totalDatas = result.Datas.Count();
                progressBar1.Max = totalDatas;
                foreach (var item in result.Datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Send {item.value.NoRujukan}...", item.index, totalDatas);

                    var type = item.value.Type;
                    if (item.value.Type == Enums.TableType.KPP || item.value.Type == Enums.TableType.KPP_HH)
                    {
                        type = TableType.KPP;
                    }
                    else if (item.value.Type == Enums.TableType.Kompaun || item.value.Type == Enums.TableType.Kompaun_HH)
                    {
                        type = TableType.Kompaun;
                    }
                    else if (item.value.Type == Enums.TableType.DataKes || item.value.Type == Enums.TableType.DataKes_HH)
                    {
                        type = TableType.DataKes;
                    }
                    else if (item.value.Type == Enums.TableType.Akuan_UpdateKompaun || item.value.Type == Enums.TableType.Akuan_UpdateKompaun_HH || item.value.Type == TableType.KompaunBayaran)
                    {
                        type = TableType.Akuan_UpdateKompaun;
                    }
                    //else if (item.value.Type == Enums.TableType.IpResit_Manual)
                    //{
                    //    type = TableType.IpResit_Manual;
                    //}

                    var response = await SendOnlineBll.SendDataOnlineAsync(item.value.NoRujukan, type, this.Context);
                    if (response.Mesage.Contains("Socket closed") || response.Mesage.Contains("Connection reset") || response.Mesage.Contains("Tiada Sambungan Internet"))
                    {
                        return response;
                    }
                }

                UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
            }

            return new Response<string>();
        }

        private async Task SendImagesAsync()
        {
            var result = DataAccessQuery<TbSendOnlineData>.GetAll();
            if (result.Success)
            {
                var datas = result.Datas.Where(m => m.Type != Enums.TableType.KompaunBayaran && m.Type != Enums.TableType.KPP_HH
                        && m.Type != Enums.TableType.Kompaun_HH && m.Type != Enums.TableType.DataKes_HH).OrderBy(m => m.Type);
                var totalDatas = datas.Count();

                progressBar1.Max = totalDatas;
                foreach (var item in datas.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Send Image {item.value.Type} {item.value.NoRujukan}...", item.index, totalDatas);
                    if (item.value.Type == Enums.TableType.KPP)
                    {
                        var rujukan = PemeriksaanBll.GetPemeriksaanByRujukan(item.value.NoRujukan);
                        if (rujukan != null)
                        {
                            await SendOnlineBll.SendImageOnline(item.value.NoRujukan, rujukan.KodCawangan, rujukan.Status, rujukan.PgnDaftar.ToString(), rujukan.TrkhDaftar,
                                rujukan.PgnAkhir.ToString(), rujukan.TrkhAkhir, 2);
                        }
                    }
                    else if (item.value.Type == Enums.TableType.Kompaun)
                    {
                        var resultKompaun = KompaunBll.GetKompaunByRujukan(item.value.NoRujukan);
                        if (resultKompaun.Success)
                        {
                            if (resultKompaun.Datas != null)
                            {
                                await SendOnlineBll.SendImageOnline(item.value.NoRujukan, resultKompaun.Datas.KodCawangan, resultKompaun.Datas.Status,
                                    resultKompaun.Datas.PgnDaftar.ToString(), resultKompaun.Datas.TrkhDaftar, resultKompaun.Datas.PgnAkhir.ToString(),
                                    resultKompaun.Datas.TrkhAkhir, 3);
                            }
                        }
                    }
                    else if (item.value.Type == Enums.TableType.DataKes)
                    {
                        var data = KompaunBll.GetSiasatByNoKes(item.value.NoRujukan);
                        if (data != null)
                        {
                            await SendOnlineBll.SendImageOnline(item.value.NoRujukan, data.KodCawangan, data.Status,
                                data.PgnDaftar.ToString(), data.TrkhDaftar, data.PgnAkhir.ToString(),
                                data.TrkhAkhir, 3);
                        }
                    } 
                    else if (item.value.Type == Enums.TableType.IpResit_Manual) 
                    {
                        //kategori 4 = ipresit diambil manual di gajet.
                        var data = PemeriksaanBll.GetPemeriksaanByRujukan(item.value.NoRujukan);
                        if (data != null)
                        {
                            await SendOnlineBll.SendReceiptManualImageOnline(item.value.NoRujukan, data.KodCawangan, data.Status, data.PgnDaftar.ToString(), data.TrkhDaftar,
                                data.PgnAkhir.ToString(), data.TrkhAkhir, 4);
                        }
                    }

                    UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
                }
            }
        }

        private void UpdateInfo(string message, int index = 0, int total = 0)
        {
            Activity.RunOnUiThread(() =>
            {
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
            });
        }

        private void Backup()
        {
            var checkoutTime = DateTime.Now.ToString(Constants.DateBackupFormat);
            // Check BackupFolder
            var backupFolder = GeneralBll.CreateFolderReturnPath(Constants.BackupPath);
            var checkOutFolder = GeneralBll.CreateFolderReturnPath(Constants.BackupPath + checkoutTime + "/");

            //MoveDB
            MoveFiles(backupFolder, checkOutFolder, Constants.DatabasePath, checkoutTime);

            //MoveImage
            MoveImagesFiles(backupFolder, checkOutFolder, Constants.ImgsPath, checkoutTime);

            //Move Log
            MoveFiles(backupFolder, checkOutFolder, Constants.LogPath, checkoutTime);
        }

        private void MoveFiles(string backupFolder, string checkoutTimeFolder, string filePath, string checkoutTime)
        {
            var message = "Database";
            if (filePath == Constants.ImgsPath) message = "Image";
            if (filePath == Constants.LogPath) message = "Log";

            var totalFiles = 0;

            if (filePath == Constants.LogPath)
            {
                UpdateInfo($"Pindah {message}...", 1, 1);

                var filesInfo = new DirectoryInfo(GeneralBll.GetFolderPath(filePath));

                var latestBackgroundService = new DirectoryInfo(GeneralBll.GetFolderPath(filePath))
                                                                .GetFiles().OrderByDescending(m => m.LastWriteTime)
                                                                .FirstOrDefault(m => m.FullName.Contains("BackgroundService_"));
                if (latestBackgroundService != null)
                {
                    File.Copy(latestBackgroundService.ToString(), checkoutTimeFolder + $"/{checkoutTime}_" + latestBackgroundService.Name);
                }

                var latestLogService = new DirectoryInfo(GeneralBll.GetFolderPath(filePath))
                                                               .GetFiles().OrderByDescending(m => m.LastWriteTime)
                                                               .FirstOrDefault(m => Path.GetFileName(m.FullName).Contains("LogService_"));
                if (latestLogService != null)
                {
                    File.Copy(latestLogService.ToString(), checkoutTimeFolder + $"/{checkoutTime}_" + latestLogService.Name);
                }

                var latestlog = new DirectoryInfo(GeneralBll.GetFolderPath(filePath))
                                                               .GetFiles().OrderByDescending(m => m.LastWriteTime)
                                                               .FirstOrDefault(m => Path.GetFileName(m.FullName).Contains("Log_"));
                if (latestlog != null)
                {
                    File.Copy(latestlog.ToString(), checkoutTimeFolder + $"/{checkoutTime}_" + latestlog.Name);
                }

            }
            else
            {
                var files = Directory.GetFiles(GeneralBll.GetFolderPath(filePath));
                totalFiles = files.Count();
                progressBar1.Max = totalFiles;

                foreach (var item in files.Select((value, index) => new { index, value }))
                {
                    UpdateInfo($"Pindah {message}...", item.index, totalFiles);
                    File.Copy(item.value, checkoutTimeFolder + $"/{checkoutTime}_" + Path.GetFileName(item.value));
                    Thread.Sleep(250);
                }
            }
            UpdateInfo($"Pindah {message}...", totalFiles, totalFiles);
            Thread.Sleep(1000);
        }

        private void MoveImagesFiles(string backupFolder, string checkoutTimeFolder, string filePath, string checkoutTime)
        {
            var message = "Image";

            var files = Directory.GetFiles(GeneralBll.GetFolderPath(filePath));
            var totalFiles = files.Count();
            progressBar1.Max = totalFiles;

            foreach (var item in files.Select((value, index) => new { index, value }))
            {
                UpdateInfo($"Pindah {message}...", item.index, totalFiles);

                //1. get data from TbSendOnlineGambar based on FileName and status not sent,
                //2. if the status not sent it will copy to backup folder
                var fileName = Path.GetFileName(item.value);
                var imageIsSent = DataAccessQuery<TbSendOnlineGambar>.Get(m => m.Status != Enums.StatusOnline.Sent && m.Name == fileName);
                if (imageIsSent.Success && imageIsSent.Datas != null)
                {
                    File.Copy(item.value, checkoutTimeFolder + $"/{checkoutTime}_" + Path.GetFileName(item.value));
                    Thread.Sleep(250);
                }
            }
            UpdateInfo($"Pindah {message}...", totalFiles, totalFiles);
            Thread.Sleep(1000);
        }

        private async Task SendErrorDatasAsync()
        {
            try
            {
                var errorRecordFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + Constants.LogPath + Constants.ErrorRecordsPath;
                if (File.Exists(errorRecordFile))
                    File.Delete(errorRecordFile);

                // Send Data
                var result = DataAccessQuery<TbSendOnlineData>.GetAll();
                if (result.Success && result.Datas.Any())
                {
                    var sendDatas = result.Datas.Where(m => m.Status != Enums.StatusOnline.Sent);

                    var totalDatas = sendDatas.Count();
                    progressBar1.Max = totalDatas;

                    foreach (var item in sendDatas.Select((value, index) => new { index, value }))
                    {
                        UpdateInfo($"Send {item.value.NoRujukan}...", item.index, totalDatas);
                        if (!await IsDataExistInServerAsync(item.value.NoRujukan, item.value.Type))
                            await SendOnlineBll.SendErrorDataAsync(item.value.NoRujukan, item.value.Type, this.Context);
                    }

                    UpdateInfo($"Sila Tunggu...", totalDatas, totalDatas);
                }

                //Uploading SQL Script
                //UpdateInfo($"Uploading Error Script...");
                //string strFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + Constants.LogPath + Constants.ErrorRecordsPath;
                //await HttpClientService.UploadFileAsync(strFile, GeneralBll.GetUserHandheld() + DateTime.Now.ToString("yyyymmddhhmmss") + "unsend.txt");

                #region Comment

                //UpdateInfo("Bersihkan data Error Datas...");
                //// Delete data after sent
                //result = DataAccessQuery<TbSendOnlineData>.GetAll();
                //if (result.Success && result.Datas.Any())
                //{
                //    var dataHasBeenDeleted = false;
                //
                //    var sendDatas = result.Datas.Where(m => m.Type == Enums.TableType.KPP_HH || m.Type == Enums.TableType.Kompaun_HH || m.Type == Enums.TableType.DataKes_HH);
                //    foreach (var item in sendDatas)
                //    {
                //        if (item.Status == Enums.StatusOnline.Sent)
                //        {
                //            if (item.Type == Enums.TableType.KPP_HH)
                //            {
                //                var tbKpp = DataAccessQuery<TbKpp>.Get(m => m.NoRujukanKpp == item.NoRujukan);
                //                if (tbKpp.Success && tbKpp.Datas != null)
                //                {
                //                    dataHasBeenDeleted = DataAccessQuery<TbKpp>.Delete(tbKpp.Datas).Success;
                //                }
                //            }
                //
                //            if (item.Type == Enums.TableType.Kompaun_HH)
                //            {
                //                var kompaun = DataAccessQuery<TbKompaun>.Get(m => m.NoKmp == item.NoRujukan);
                //                if (kompaun.Success && kompaun.Datas != null)
                //                {
                //                    dataHasBeenDeleted = DataAccessQuery<TbKompaun>.Delete(kompaun.Datas).Success;
                //                }
                //            }
                //
                //            if (item.Type == Enums.TableType.DataKes_HH)
                //            {
                //
                //                var tbDataKes = DataAccessQuery<TbDataKes>.Get(m => m.NoKes == item.NoRujukan);
                //                if (tbDataKes.Success && tbDataKes.Datas != null)
                //                {
                //                    dataHasBeenDeleted = DataAccessQuery<TbDataKes>.Delete(tbDataKes.Datas).Success;
                //                }
                //            }
                //
                //            if (dataHasBeenDeleted)
                //                DataAccessQuery<TbSendOnlineData>.Delete(item);
                //        }
                //    }
                //}


                //if (File.Exists(errorRecordFile))
                //{
                //    //Send Error Record File
                //    SendOnlineBll.SendErrorRecordFile(errorRecordFile);
                //
                //}
                #endregion
            }
            catch (Exception ex)
            {
                Log.WriteLogFile($"Checkout - Send Error Data \r Message : {ex.Message}");
            }

            UpdateInfo("Sila Tunggu...");
        }

        private async Task<bool> IsDataExistInServerAsync(string noRujukan, Enums.TableType type)
        {
            //, tbkpp_asastindakan, tbpasukan_trans, , , , , 

            var isDataExistsInserver = false;
            if (type == Enums.TableType.KPP) //tbkpp
            {
                var kppExists = await HttpClientService.GetKPPAsync(noRujukan);
                if (kppExists.Success)
                {
                    SendOnlineBll.SetStatusDataOnline(noRujukan, Enums.TableType.KPP, Enums.StatusOnline.Sent);
                    isDataExistsInserver = kppExists.Success;
                }

                await CheckKppAsasTindakan(noRujukan); //tbkpp_asastindakan
                await CheckKppPasukanTrans(noRujukan);
            }
            else if (type == Enums.TableType.Kompaun) //tbkompaun
            {
                var kompaunExists = await HttpClientService.CheckData($"Select * from tbkompaun where nokmp = '{noRujukan}'");
                if (kompaunExists.Success)
                {
                    SendOnlineBll.SetStatusDataOnline(noRujukan, Enums.TableType.Kompaun, Enums.StatusOnline.Sent);
                    isDataExistsInserver = kompaunExists.Success;
                }
            }
            else if (type == Enums.TableType.DataKes) //tbdatakes
            {
                var dataKesExists = await HttpClientService.CheckData($"Select * from tbdatakes where nokes = '{noRujukan}'");
                if (dataKesExists.Success)
                {
                    SendOnlineBll.SetStatusDataOnline(noRujukan, Enums.TableType.DataKes, Enums.StatusOnline.Sent);
                    isDataExistsInserver = dataKesExists.Success;
                }

                var data = KompaunBll.GetSiasatByNoKes(noRujukan);
                if (data != null)
                {
                    await CheckDataKesPesalah(data.NoKes); //tbdatakes_pesalah
                    await CheckDataKesAsasTindakan(data.NoKes); //tbdatakes_asastindakan
                    await CheckDataKesKesalahan(data.NoKes); //tbdatakes_kesalahan
                }
            }
            return isDataExistsInserver;
        }

        private async Task CheckKppAsasTindakan(string noRujukan)
        {
            var asasTindakan = DataAccessQuery<TbKppAsasTindakan>.GetAll();
            if (asasTindakan.Success)
            {
                foreach (var item in asasTindakan.Datas.Where(m => m.NoRujukanKpp == noRujukan && m.IsSendOnline != StatusOnline.Sent))
                {
                    var dataDataKesPesalah = await HttpClientService.CheckData($"Select * from tbkpp_asastindakan where norujukankpp = '{item?.NoRujukanKpp}'");
                    if (dataDataKesPesalah.Success)
                    {
                        DataAccessQuery<TbKppAsasTindakan>.ExecuteSql($"UPDATE tbkpp_asastindakan SET IsSendOnline = '{(int)StatusOnline.Sent}' WHERE KodTujuan = '{item.KodTujuan}' AND KodAsas = '{item.KodAsas}' AND NoRujukanKpp = '{noRujukan}' ");
                    }

                }
            }
        }

        private async Task CheckKppPasukanTrans(string noRujukan)
        {
            var asasTindakan = DataAccessQuery<TbPasukanTrans>.GetAll();
            if (asasTindakan.Success)
            {
                foreach (var item in asasTindakan.Datas.Where(m => m.NoRujukan == noRujukan && m.JenisTrans == Constants.JenisTrans.Kpp && m.IsSendOnline != StatusOnline.Sent))
                {
                    var dataDataKesPesalah = await HttpClientService.CheckData($"Select * from tbpasukan_trans where jenistrans = '{item?.JenisTrans}' AND norujukan = '{item?.NoRujukan}'");
                    if (dataDataKesPesalah.Success)
                    {
                        DataAccessQuery<TbPasukanTrans>.ExecuteSql($"UPDATE tbpasukan_trans SET IsSendOnline = '{(int)StatusOnline.Sent}' WHERE id = '{item.Id}' AND NoRujukan = '{item.NoRujukan}' ");
                    }

                }
            }
        }

        private async Task CheckDataKesPesalah(string noKes)
        {
            var dataKesPesalah = DataAccessQuery<TbDataKesPesalah>.Get(m => m.NoKes == noKes);
            if (dataKesPesalah.Success && dataKesPesalah.Datas != null && dataKesPesalah.Datas.IsSendOnline != Enums.StatusOnline.Sent)
            {
                var dataDataKesPesalah = await HttpClientService.CheckData($"Select * from tbdatakes_pesalah where nokes = '{dataKesPesalah?.Datas?.NoKes}'");
                if (dataDataKesPesalah.Success)
                {
                    DataAccessQuery<TbDataKesPesalah>.ExecuteSql($"UPDATE tbdatakes_pesalah SET IsSendOnline = '{(int)StatusOnline.Sent}' WHERE NoKes = '{dataKesPesalah?.Datas?.NoKes}' ");
                }
            }
        }

        private async Task CheckDataKesAsasTindakan(string noKes)
        {
            var dataKesAsasTindakan = DataAccessQuery<TbDataKesAsasTindakan>.Get(m => m.NoKes == noKes);
            if (dataKesAsasTindakan.Success && dataKesAsasTindakan.Datas != null && dataKesAsasTindakan.Datas.IsSendOnline != Enums.StatusOnline.Sent)
            {
                var kesAsasTindakan = await HttpClientService.CheckData($"Select * from tbdatakes_asastindakan where nokes = '{dataKesAsasTindakan?.Datas?.NoKes}'");
                if (kesAsasTindakan.Success)
                {
                    DataAccessQuery<TbDataKesAsasTindakan>.ExecuteSql($"UPDATE tbdatakes_asastindakan SET IsSendOnline = '{(int)StatusOnline.Sent}' WHERE NoKes = '{dataKesAsasTindakan?.Datas?.NoKes}' ");
                }
            }
        }

        private async Task CheckDataKesKesalahan(string noKes)
        {
            var dataKesKesalahan = DataAccessQuery<TbDataKesKesalahan>.Get(m => m.NoKes == noKes);
            if (dataKesKesalahan.Success && dataKesKesalahan.Datas != null && dataKesKesalahan.Datas.IsSendOnline != Enums.StatusOnline.Sent)
            {
                var kesAsasTindakan = await HttpClientService.CheckData($"Select * from tbdatakes_kesalahan where nokes = '{dataKesKesalahan?.Datas?.NoKes}'");
                if (kesAsasTindakan.Success)
                {
                    DataAccessQuery<TbDataKesKesalahan>.ExecuteSql($"UPDATE tbdatakes_kesalahan SET IsSendOnline = '{(int)StatusOnline.Sent}' WHERE NoKes = '{dataKesKesalahan?.Datas?.NoKes}' ");
                }
            }
        }
    }
}