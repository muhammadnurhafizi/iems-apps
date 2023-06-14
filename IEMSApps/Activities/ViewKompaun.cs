using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Woosim.Printer;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using Plugin.BxlMpXamarinSDK;
using Plugin.BxlMpXamarinSDK.Abstractions;

namespace IEMSApps.Activities
{
    [Activity(Label = "View Kompaun", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ViewKompaun : BaseActivity
    {
        private const string LayoutName = "ViewKompaun";
        LinearLayout tabButiran, tabPesalah, tabPenerima;
        TextView lblTabButiran, lblTabPesalah, lblTabPenerima;
        View viewButiran, viewPesalah, viewPenerima;
        private AlertDialog _dialog;
        ServicetHandler handler;

        private HourGlassClass _hourGlass = new HourGlassClass();

        private MPosControllerPrinter _printer;
        private MposConnectionInformation _connectionInfo;
        private static SemaphoreSlim _printSemaphore = new SemaphoreSlim(1, 1);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ViewKompaun);

            //initialize printer and connection null
            _printer = null;
            _connectionInfo = null;

            //  SetInit();
            _hourGlass?.StartMessage(this, SetInit);
        }

        private string _noRujukanKpp;
        private TextView lblNoKpp;

        private void SetInit()
        {
            try
            {
                var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
                txtHhId.Text = GeneralBll.GetUserHandheld();

                //_jenisKompaun = Intent.GetStringExtra("JenisKmp") ?? "";
                _noRujukanKpp = Intent.GetStringExtra("NoRujukanKpp") ?? "";

                tabButiran = FindViewById<LinearLayout>(Resource.Id.tabButiran);
                tabPesalah = FindViewById<LinearLayout>(Resource.Id.tabPesalah);
                tabPenerima = FindViewById<LinearLayout>(Resource.Id.tabPenerima);

                lblTabButiran = FindViewById<TextView>(Resource.Id.lblTabButiran);
                lblTabPesalah = FindViewById<TextView>(Resource.Id.lblTabPesalah);
                lblTabPenerima = FindViewById<TextView>(Resource.Id.lblTabPenerima);

                viewButiran = FindViewById<View>(Resource.Id.viewButiran);
                viewPesalah = FindViewById<View>(Resource.Id.viewPesalah);
                viewPenerima = FindViewById<View>(Resource.Id.viewPenerima);

                tabButiran.Click += TabButiran_Click;
                tabPesalah.Click += TabPesalah_Click;
                tabPenerima.Click += TabPenerima_Click;

                SetLayoutVisible(viewButiran, lblTabButiran, tabButiran);
                SetLayoutInvisible(viewPesalah, lblTabPesalah, tabPesalah);
                SetLayoutInvisible(viewPenerima, lblTabPenerima, tabPenerima);

                var data = KompaunBll.GetKompaunByRujukanKpp(_noRujukanKpp);

                if (data != null)
                {

                    lblNoKpp = FindViewById<TextView>(Resource.Id.lblNoKpp);
                    lblNoKpp.Text = data.NoKmp;

                    LoadDataButiran(data);

                    LoadDataPesalah(data);

                    LoadDataPenerima(data);

                    LoadButton();
                }


            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }
            _hourGlass?.StopMessage();
        }

        private void LoadDataButiran(TbKompaun data)
        {
            var txtNoLaporPolis = FindViewById<EditText>(Resource.Id.txtNoLaporPolis);
            txtNoLaporPolis.Text = data.NoLaporanPolis;
            SetDisableEditText(txtNoLaporPolis);

            var txtNoLaporCawangan = FindViewById<EditText>(Resource.Id.txtNoLaporCawangan);
            txtNoLaporCawangan.Text = data.NoLaporanCwgn;
            SetDisableEditText(txtNoLaporCawangan);

            var dtData = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhKmp);
            var txtTarikh = FindViewById<TextView>(Resource.Id.txtTarikh);
            txtTarikh.Text = dtData.ToString(Constants.DateFormatDisplay);

            var txtMasa = FindViewById<TextView>(Resource.Id.txtMasa);
            txtMasa.Text = dtData.ToString(Constants.TimeFormatDisplay);

            var txtTempat = FindViewById<EditText>(Resource.Id.txtTempat);
            txtTempat.Text = data.TempatSalah;
            SetDisableEditText(txtTempat);

            var rdIndividu = FindViewById<RadioButton>(Resource.Id.rdIndividu);
            var rdSyarikat = FindViewById<RadioButton>(Resource.Id.rdSyarikat);

            rdIndividu.Checked = true;
            if (data.JenisPesalah == Constants.JenisPesalah.Syarikat)
            {
                rdSyarikat.Checked = true;
            }

            rdIndividu.Enabled = false;
            rdSyarikat.Enabled = false;

            var txtAkta = FindViewById<EditText>(Resource.Id.txtAkta);
            txtAkta.Text = MasterDataBll.GetAktaName(data.KodAkta);
            SetDisableEditText(txtAkta);

            var txtKesalahan = FindViewById<EditText>(Resource.Id.txtKesalahan);
            txtKesalahan.Text = MasterDataBll.GetKesalahanName(data.KodSalah, data.KodAkta);
            SetDisableEditText(txtKesalahan);

            var txtButirKesalahan = FindViewById<EditText>(Resource.Id.txtButirKesalahan);
            txtButirKesalahan.Text = data.ButirSalah;
            SetDisableEditText(txtButirKesalahan);

            var txtAmaunTawaran = FindViewById<EditText>(Resource.Id.txtAmaunTawaran);
            txtAmaunTawaran.Text = data.AmnKmp.ToString(Constants.DecimalFormat);
            SetDisableEditText(txtAmaunTawaran);

            var chkArahan = FindViewById<CheckBox>(Resource.Id.chkArahan);
            if (data.IsArahanSemasa == Constants.ArahanSemasa.Yes)
            {
                chkArahan.Checked = true;
            }
            chkArahan.Enabled = false;

            var txtTempohTawaran = FindViewById<EditText>(Resource.Id.txtTempohTawaran);
            txtTempohTawaran.Text = data.TempohTawaran.ToString();
            SetDisableEditText(txtTempohTawaran);

            var txtNoEP = FindViewById<EditText>(Resource.Id.txtNoEP);
            txtNoEP.Text = data.NoEp;
            SetDisableEditText(txtNoEP);

            var txtNoIP = FindViewById<EditText>(Resource.Id.txtNoIP);
            txtNoIP.Text = data.NoIp;
            SetDisableEditText(txtNoIP);

            txtNoEP.SetFilters(new IInputFilter[] { new FilterChar() });
            txtNoIP.SetFilters(new IInputFilter[] { new FilterChar() });
        }

        private void LoadDataPesalah(TbKompaun data)
        {
            var txtNama = FindViewById<EditText>(Resource.Id.txtNama);
            txtNama.Text = data.NamaOkk;
            SetDisableEditText(txtNama);

            var txtNoKp = FindViewById<EditText>(Resource.Id.txtNoKp);
            txtNoKp.Text = data.NoKpOkk;
            SetDisableEditText(txtNoKp);


            var txtNamaSyarikat = FindViewById<EditText>(Resource.Id.txtNamaSyarikat);
            txtNamaSyarikat.Text = data.NamaPremis;
            SetDisableEditText(txtNamaSyarikat);

            var txtNoDaftarSyarikat = FindViewById<EditText>(Resource.Id.txtNoDaftarSyarikat);
            txtNoDaftarSyarikat.Text = data.NoDaftarPremis;
            SetDisableEditText(txtNoDaftarSyarikat);

            var txtAlamatPesalah1 = FindViewById<EditText>(Resource.Id.txtAlamatPesalah1);
            txtAlamatPesalah1.Text = data.AlamatOkk1;
            SetDisableEditText(txtAlamatPesalah1);

            var txtAlamatPesalah2 = FindViewById<EditText>(Resource.Id.txtAlamatPesalah2);
            txtAlamatPesalah2.Text = data.AlamatOkk2;
            SetDisableEditText(txtAlamatPesalah2);

            var txtAlamatPesalah3 = FindViewById<EditText>(Resource.Id.txtAlamatPesalah3);
            txtAlamatPesalah3.Text = data.AlamatOkk3;
            SetDisableEditText(txtAlamatPesalah3);

            var txtBarangKompaun = FindViewById<EditText>(Resource.Id.txtBarangKompaun);
            txtBarangKompaun.Text = data.BarangKompaun;
            SetDisableEditText(txtBarangKompaun);

        }

        private void LoadDataPenerima(TbKompaun data)
        {
            var txtNamaPenerima = FindViewById<EditText>(Resource.Id.txtNamaPenerima);
            txtNamaPenerima.Text = data.NamaPenerima;
            SetDisableEditText(txtNamaPenerima);

            var txtNoKpPenerima = FindViewById<EditText>(Resource.Id.txtNoKpPenerima);
            txtNoKpPenerima.Text = data.NoKpPenerima;
            SetDisableEditText(txtNoKpPenerima);


            var txtAlamatPenerima1 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima1);
            txtAlamatPenerima1.Text = data.AlamatPenerima1;
            SetDisableEditText(txtAlamatPenerima1);

            var txtAlamatPenerima2 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima2);
            txtAlamatPenerima2.Text = data.AlamatPenerima2;
            SetDisableEditText(txtAlamatPenerima2);

            var txtAlamatPenerima3 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima3);
            txtAlamatPenerima3.Text = data.AlamatPenerima3;
            SetDisableEditText(txtAlamatPenerima3);

            var chkAmaran = FindViewById<CheckBox>(Resource.Id.chkAmaran);
            if (data.IsCetakAkuan == Constants.CetakAkuan.Yes)
            {
                chkAmaran.Checked = true;
            }
            chkAmaran.Enabled = false;

        }

        private void LoadButton()
        {
            var btnOk = FindViewById<Button>(Resource.Id.btnOk);
            btnOk.Click += BtnOk_Click;

            var btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            var btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            var btnAkuan = FindViewById<Button>(Resource.Id.btnNote);
            var lblBtnAkuan = FindViewById<TextView>(Resource.Id.lblBtnNote);
            lblBtnAkuan.Text = "Akuan";

            btnCamera.Click += BtnCamera_Click;
            btnPrint.Click += BtnPrint_Click;
            btnAkuan.Click += BtnAkuan_Click;

            //var linearFooterPemeriksaan = FindViewById<LinearLayout>(Resource.Id.linearFooterPemeriksaan);
            //linearFooterPemeriksaan.WeightSum = 3;
            //var linearOk = FindViewById<LinearLayout>(Resource.Id.linearOk);
            //linearOk.Visibility = ViewStates.Gone;
        }

        private void BtnAkuan_Click(object sender, EventArgs e)
        {
            try
            {
                _hourGlass?.StartMessage(this, ShowAkuan);

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnAkuan_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void ShowAkuan()
        {
            var data = KompaunBll.GetKompaunByRujukan(lblNoKpp.Text);
            var tbKompaun = data.Datas;
            var resit = tbKompaun != null ? tbKompaun.NoResit : "";

            if (data.Success && tbKompaun != null & !string.IsNullOrEmpty(resit))
            {
                var intent = new Intent(this, typeof(ViewAkuan));
                intent.PutExtra("NoRujukan", lblNoKpp.Text);
                StartActivity(intent);
            }
            else
            {
                //GeneralAndroidClass.ShowToast("Tidak ada data Akuan");
                var message = string.Format(Constants.Messages.SambungAkuan);
                var ad = GeneralAndroidClass.GetDialogCustom(this);
                ad.SetMessage(Html.FromHtml(message));
                ad.SetButton(Constants.Messages.No, (s, ev) => { });
                ad.SetButton2(Constants.Messages.Yes, (s, ev) =>
                {
                    var intent = new Intent(this, typeof(Akuan));
                    intent.PutExtra("NoRujukan", lblNoKpp.Text);
                    StartActivity(intent);
                });
                ad.Show();
            }
            _hourGlass?.StopMessage();
        }
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                var printImageBll = new PrintImageBll();
                var bitmap = printImageBll.Kompaun(this, lblNoKpp.Text);
                GeneralAndroidClass.ShowToast("Sedang cetak");
                return;
#endif

                //var printImageBll = new PrintImageBll();
                //var bitmap = printImageBll.Kompaun(this, lblNoKpp.Text);
                //GeneralAndroidClass.ShowToast("Sedang cetak");

                //return;

                var ad = GeneralAndroidClass.GetDialogCustom(this);

                ad.SetMessage(Constants.Messages.DialogRePrint);
                // Positive

                ad.SetButton("Tidak", (s, ev) => { });
                ad.SetButton2("Ya", (s, ev) =>
                {
                    Print(true);
                });
                ad.Show();

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnPrint_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnCamera_Click(object sender, EventArgs e)
        {
            try
            {
                _hourGlass?.StartMessage(this, OnCamera);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnCamera_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void OnCamera()
        {

            var intent = new Intent(this, typeof(Camera));
            intent.PutExtra("filename", lblNoKpp.Text);
            intent.PutExtra("allowtakepicture", false);
            intent.PutExtra("allowreplace", false);

            StartActivity(intent);

            _hourGlass?.StopMessage();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnOk_Click", ex.Message, Enums.LogType.Error);
            }
        }
        private void SetDisableEditText(EditText data)
        {
            data.SetBackgroundResource(Resource.Drawable.textView_bg);
            data.Enabled = false;
        }

        #region Layout 

        private void TabPenerima_Click(object sender, EventArgs e)
        {
            SetLayoutInvisible(viewButiran, lblTabButiran, tabButiran);
            SetLayoutInvisible(viewPesalah, lblTabPesalah, tabPesalah);
            SetLayoutVisible(viewPenerima, lblTabPenerima, tabPenerima);
        }

        private void TabPesalah_Click(object sender, EventArgs e)
        {
            SetLayoutInvisible(viewButiran, lblTabButiran, tabButiran);
            SetLayoutVisible(viewPesalah, lblTabPesalah, tabPesalah);
            SetLayoutInvisible(viewPenerima, lblTabPenerima, tabPenerima);
        }

        private void TabButiran_Click(object sender, EventArgs e)
        {
            SetLayoutVisible(viewButiran, lblTabButiran, tabButiran);
            SetLayoutInvisible(viewPesalah, lblTabPesalah, tabPesalah);
            SetLayoutInvisible(viewPenerima, lblTabPenerima, tabPenerima);
        }

        private void SetLayoutVisible(View view, TextView text, View tab)
        {
            view.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            view.Visibility = ViewStates.Visible;

            text.SetTextColor(Color.White);

            tab.SetBackgroundResource(Resource.Drawable.tab_active_bg);
        }

        private void SetLayoutInvisible(View view, TextView text, View tab)
        {
            view.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 1);
            view.Visibility = ViewStates.Invisible;

            text.SetTextColor(Color.Black);

            tab.SetBackgroundResource(Resource.Drawable.tab_bg);
        }

        #endregion

        private ListView lvResult;
        private AlertDialog _alert;

        #region Printing

        private void PreparePrinterDevice()
        {
            try
            {
                if (!GeneralAndroidClass.IsPrinterExist())
                {
                    GeneralAndroidClass.LogData(LayoutName, "PreparePrinterDevice", Constants.ErrorMessages.PrinterNotFound, Enums.LogType.Info);
                    return;
                }

                if (GlobalClass.BluetoothDevice == null)
                {
                    lvResult = new ListView(this);
                    var adapter = new DeviceListAdapter(this, GlobalClass.BluetoothAndroid._listDevice);
                    lvResult.Adapter = adapter;
                    lvResult.ItemClick += lvResult_ItemClick;

                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    _alert = builder.Create();
                    _alert.SetMessage(Constants.Messages.SelectYourItem);
                    _alert.SetView(lvResult);


                    _alert.Show();
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "PreparePrinterDevice", ex.Message, Enums.LogType.Error);

            }

        }

        async void lvResult_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                if (e.Position > GlobalClass.BluetoothAndroid._listDevice.Count)
                    return;

                _alert.Dismiss();
                GlobalClass.BluetoothDevice = GlobalClass.BluetoothAndroid._listDevice[e.Position];
                Print(false);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "lvResult_ItemClick", ex.Message, Enums.LogType.Error);
            }
        }

        private void Print(bool isNeedCheck)
        {
            if (isNeedCheck)
            {
                PreparePrinterDevice();

                if (GlobalClass.BluetoothDevice == null)
                {
                    //_dialog?.Dismiss();
                    return;
                }
            }

            //GetFWCode();

            new Task(() =>
            {
                try
                {
                    //RunOnUiThread(() => OnPrinting());
                    //OnPrinting();
                    //IsLoading(this, false);

                    string BluetoothName = GlobalClass.BluetoothDevice.Name;
                    //GeneralAndroidClass.ShowToast("Printer Dipilih : " + BluetoothName);
                    GeneralAndroidClass.LogData(LayoutName, "Print using Device : ", BluetoothName, Enums.LogType.Debug);
                    if (BluetoothName == Constants.BixolonBluetoothName)
                    {
                        OnPrintingBixolon();
                    }
                    else
                    {
                        //RunOnUiThread(() => OnPrinting());
                        //RunOnUiThread(() => GetFWCode()) ;
                        GetFWCode();
                        OnPrinting();
                        IsLoading(this, false);
                    }
                }
                catch (Exception ex)
                {
                    IsLoading(this, false);
                    GeneralAndroidClass.LogData(LayoutName, "Print", ex.Message, Enums.LogType.Error);
                }
            }).RunSynchronously();
        }


        public const int MESSAGE_DEVICE_NAME = 1;
        public const int MESSAGE_TOAST = 2;
        public const int MESSAGE_READ = 3;
        public const string DEVICE_NAME = "device_name";

        class ServicetHandler : Handler
        {
            readonly ViewKompaun activity;

            public ServicetHandler(ViewKompaun activity)
            {
                this.activity = activity;
            }

            public override void HandleMessage(Message msg)
            {
                activity?.HandleMessage(msg);
            }
        }

        void HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case MESSAGE_DEVICE_NAME:
                    var deviceName = msg.Data.GetString(DEVICE_NAME);
                    Toast.MakeText(this, $"Sambungan ke{deviceName}.", ToastLength.Short).Show();
                    this.InvalidateOptionsMenu();
                    break;
                case MESSAGE_TOAST:
                    Toast.MakeText(this, msg.Arg1, ToastLength.Short).Show();
                    break;
                case MESSAGE_READ:
                    string rcvMsg = GeneralBll.ProcessRcvData((byte[])msg.Obj);
                    GlobalClass.FwCode = GlobalClass.FwCode + rcvMsg;
                    break;
            }
        }

        void SendData(byte[] data)
        {
            if (GlobalClass.printService.GetState() != BluetoothPrintService.STATE_CONNECTED)
            {
                Log.WriteLogFile(LayoutName, "OnPrinting", "Not Connected print", Enums.LogType.Info);
                GeneralAndroidClass.ShowToast("Tiada Sambungan Pencetak");
            }
            else if (data.Length > 0)
            {
                GlobalClass.printService.Write(data);
            }
        }

        private async Task OnPrinting()
        {
            Log.WriteLogFile("Printer Firmware : " + GlobalClass.FwCode);
            string strAddress = GlobalClass.BluetoothDevice.Address;
            if (!GeneralAndroidClass.IsRegisterPrinter(strAddress))
            {
                if (GlobalClass.FwCode != Constants.FWCODE)
                {
                    Toast.MakeText(this, "Sila cuba sekali lagi", ToastLength.Short).Show();
                    await ShowMessageNew(false, "");
                    return;
                }
                else
                    GeneralAndroidClass.RegisterPrinter(strAddress);
            }

            await ShowMessageNew(true, Constants.Messages.GenerateBitmap);

            var printImageBll = new PrintImageBll();
            var bitmap = printImageBll.Kompaun(this, lblNoKpp.Text);

            IsLoading(this, true, Constants.Messages.ConnectionToBluetooth);


            if (GlobalClass.printService == null)
            {
                if (handler == null)
                    handler = new ServicetHandler(this);

                GlobalClass.printService = new BluetoothPrintService(handler);
                GlobalClass.printService?.Connect(GlobalClass.BluetoothDevice);
                Thread.Sleep(Constants.DefaultWaitingConnectionToBluetooth);
            }
            else
            {
                if (GlobalClass.printService.GetState() != BluetoothPrintService.STATE_CONNECTED)
                {
                    GlobalClass.printService?.Connect(GlobalClass.BluetoothDevice);
                    Thread.Sleep(Constants.DefaultWaitingConnectionToBluetooth);
                }
            }

            await ShowMessageNew(true, Constants.Messages.PrintWaitMessage);

            SendData(WoosimCmd.InitPrinter());
            SendData(WoosimCmd.SetPageMode());
            SendData(WoosimImage.PrintColorBitmap(0, 0, 0, 0, bitmap));
            SendData(WoosimCmd.PM_setStdMode());

            bitmap.Dispose();
            await ShowMessageNew(true, Constants.Messages.SuccessPrint);
            Thread.Sleep(Constants.DefaultWaitingMilisecond);
            await ShowMessageNew(false, "");
        }

        private void GetFWCode()
        {
            String strAddress = "";
            try
            {
                strAddress = GlobalClass.BluetoothDevice.Address;
                if (GeneralAndroidClass.IsRegisterPrinter(strAddress))
                    return;

                if (!string.IsNullOrEmpty(GlobalClass.FwCode))
                    return;


                if (GlobalClass.printService == null)
                {
                    if (handler == null)
                        handler = new ServicetHandler(this);

                    GlobalClass.printService = new BluetoothPrintService(handler);
                    GlobalClass.printService?.Connect(GlobalClass.BluetoothDevice);
                    Thread.Sleep(Constants.DefaultWaitingConnectionToBluetooth);
                }
                else
                {
                    if (GlobalClass.printService.GetState() != BluetoothPrintService.STATE_CONNECTED)
                    {
                        GlobalClass.printService?.Connect(GlobalClass.BluetoothDevice);
                        Thread.Sleep(Constants.DefaultWaitingConnectionToBluetooth);
                    }
                }
                byte[] cmd = { 0x1B, 0x00, 0x02, 0x02 };

                SendData(cmd);
                Thread.Sleep(Constants.DefaultWaitingConnectionToBluetooth);
                Thread.Sleep(Constants.DefaultWaitingConnectionToBluetooth);
                int iTry = 0;
                while (string.IsNullOrEmpty(GlobalClass.FwCode) && iTry < 50000)
                {
                    iTry++;
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "GetFWCode", ex.Message, Enums.LogType.Error);
            }
        }

        public async Task ShowMessageNew(bool value, string message)
        {
            IsLoading(this, value, message);
            await Task.Delay(Constants.DefaultWaitingMilisecond);
        }
        #endregion

        #region PrintingBixolon

        private async Task OnPrintingBixolon()
        {
            uint stats = 0;
            int check = 1;
            PrinterBixolonClass bixolonClass = new PrinterBixolonClass();
            try
            {
                _connectionInfo = new MposConnectionInformation();

                _connectionInfo.IntefaceType = MPosInterfaceType.MPOS_INTERFACE_BLUETOOTH;
                _connectionInfo.Name = GlobalClass.BluetoothDevice.Name;
                _connectionInfo.MacAddress = GlobalClass.BluetoothDevice.Address;

                if (!GeneralAndroidClass.IsRegisterPrinter(_connectionInfo.MacAddress))
                {
                    GeneralAndroidClass.RegisterPrinter(_connectionInfo.MacAddress);
                }
                Log.WriteLogFile("CheckPrinter", "connection Info : " + _connectionInfo, Enums.LogType.Debug);

                // convert kpp to bitmapkpp to get ready to print
                await ShowMessageNew(true, Constants.Messages.GenerateBitmap);
                var printImageBll = new PrintImageBll();
                var bitmap = printImageBll.Kompaun(this, lblNoKpp.Text);

                // Prepares to communicate with the printer
                _printer = await bixolonClass.OpenPrinterService(_connectionInfo) as MPosControllerPrinter;
                await ShowMessageNew(true, Constants.Messages.ConnectionToBluetooth + " Printer Bixolon");

                check = await bixolonClass.CheckPrinter(_printer);
                if (check == 2)
                {
                    Thread.Sleep(Constants.DefaultWaitingConnectionToBluetooth);
                    await ShowMessageNew(false, "");
                    return;
                }
                else
                {
                    await ShowMessageNew(true, "Printer Avalaible");
                    Thread.Sleep(Constants.DefaultWaitingConnectionToBluetooth);
                }

                await ShowMessageNew(true, "Menyemak Status Printer Bixolon");
                stats = await bixolonClass.CheckPrinterBixolonStatus(_printer);
                if (stats > 0)
                {
                    //reset _printer = null, if failed to connect after turn off and on the printer.
                    bixolonClass.ResetPrinterConnection();
                    GeneralAndroidClass.ShowToast("Sila Cuba Sekali Lagi");
                    await ShowMessageNew(false, "");
                    return;
                }

                await ShowMessageNew(true, Constants.Messages.PrintWaitMessage);

                await _printSemaphore.WaitAsync();

                await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_IN);
                await _printer.directIO(new byte[] { 0x1b, 0x40 });
                await _printer.printBitmap(bitmap, -2, 1, Constants.Brightness, true, true);
                await _printer.directIO(new byte[] { 0x1b, 0x4a, 0xaf });
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "OnPrintingBixolon : ", ex.Message, Enums.LogType.Error);
            }
            finally
            {
                if (check == 1)
                {
                    // Printer starts printing by calling "setTransaction" function with "MPOS_PRINTER_TRANSACTION_OUT"
                    await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_OUT);
                    // If there's nothing to do with the printer, call "closeService" method to disconnect the communication between Host and Printer.
                    await _printer.closeService();
                    _printSemaphore.Release();

                    await ShowMessageNew(true, Constants.Messages.SuccessPrint);
                }

                Thread.Sleep(Constants.DefaultWaitingMilisecond);
                await ShowMessageNew(false, "");
            }
        }

        #endregion
    }
}