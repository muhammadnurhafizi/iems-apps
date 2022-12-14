using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Woosim.Printer;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Fragments;
using IEMSApps.Services;
using IEMSApps.Utils;

namespace IEMSApps.Activities
{
    [Activity(Label = "View Pemeriksaan", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ViewPemeriksaan : BaseActivity
    {
        private const string LayoutName = "ViewPemeriksaan";
        LinearLayout tabPremis, tabLawatan, tabPenerima;
        TextView lblTabPremis, lblTabLawatan, lblTabPenerima;
        View viewPremis, viewLawatan, viewPenerima;

        ServicetHandler handler;

        private HourGlassClass _hourGlass = new HourGlassClass();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ViewPemeriksaan);

            _hourGlass?.StartMessage(this, SetInit);

            //SetInit();
        }

        private string _noRujukan;
        private int _tindakan;
        private TextView lblNoKpp;

        private void SetInit()
        {
            try
            {
                var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
                txtHhId.Text = GeneralBll.GetUserHandheld();

                _noRujukan = Intent.GetStringExtra("norujukan") ?? "";
                // _tindakan = Intent.GetStringExtra("tindakan") ?? "";

                tabPremis = FindViewById<LinearLayout>(Resource.Id.tabPremis);
                tabLawatan = FindViewById<LinearLayout>(Resource.Id.tabLawatan);
                tabPenerima = FindViewById<LinearLayout>(Resource.Id.tabPenerima);

                lblTabPremis = FindViewById<TextView>(Resource.Id.lblTabPremis);
                lblTabLawatan = FindViewById<TextView>(Resource.Id.lblTabLawatan);
                lblTabPenerima = FindViewById<TextView>(Resource.Id.lblTabPenerima);

                viewPremis = FindViewById<View>(Resource.Id.viewPremis);
                viewLawatan = FindViewById<View>(Resource.Id.viewLawatan);
                viewPenerima = FindViewById<View>(Resource.Id.viewPenerima);

                tabPremis.Click += TabPremis_Click;
                tabLawatan.Click += TabLawatan_Click;
                tabPenerima.Click += TabPenerima_Click;

                SetLayoutVisible(viewLawatan, lblTabLawatan, tabLawatan);
                SetLayoutInvisible(viewPremis, lblTabPremis, tabPremis);
                SetLayoutInvisible(viewPenerima, lblTabPenerima, tabPenerima);

                //var dtStart = DateTime.Now;

                var data = PemeriksaanBll.GetPemeriksaanByRujukan(_noRujukan);

                if (data != null)
                {

                    lblNoKpp = FindViewById<TextView>(Resource.Id.lblNoKpp);
                    lblNoKpp.Text = _noRujukan;
                    _tindakan = data.Tindakan;

                    LoadDataLawatan(data);

                    LoadDataPremis(data);

                    LoadDataPenerima(data);

                    LoadButton();
                }

                //var dtEnd = DateTime.Now;
                //var result = dtEnd - dtStart;
                //GeneralAndroidClass.ShowToast("Time : " + result.Seconds);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }

            _hourGlass?.StopMessage();
        }

        private void LoadDataLawatan(TbKpp data)
        {
            var dtData = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhMulaLawatankpp);
            var txtTarikh = FindViewById<TextView>(Resource.Id.txtTarikh);
            txtTarikh.Text = dtData.ToString(Constants.DateFormatDisplay);

            var txtMasa = FindViewById<TextView>(Resource.Id.txtMasa);
            txtMasa.Text = dtData.ToString(Constants.TimeFormatDisplay);

            var txtKategoriKawasan = FindViewById<EditText>(Resource.Id.txtKategoriKawasan);
            txtKategoriKawasan.Text = MasterDataBll.GetKatKawasanName(data.KodKatKawasan);
            SetDisableEditText(txtKategoriKawasan);

            var txtLokasi = FindViewById<EditText>(Resource.Id.txtLokasi);
            txtLokasi.Text = data.LokasiLawatan;
            SetDisableEditText(txtLokasi);

            //var txtTujuanLawatan = FindViewById<EditText>(Resource.Id.txtTujuanLawatan);
            //txtTujuanLawatan.Text = MasterDataBll.GetTujuanLawatanName(data.KodTujuan);
            //SetDisableEditText(txtTujuanLawatan);

            var txtAsasTindakan = FindViewById<EditText>(Resource.Id.txtAsasTindakan);
            txtAsasTindakan.Text = MasterDataBll.GetKppAsasTindakan(data.NoRujukanKpp);
            if (string.IsNullOrEmpty(txtAsasTindakan.Text))
            {
                txtAsasTindakan.Text = MasterDataBll.GetAsasTindakanName(data.KodTujuan, data.KodAsas);
            }

            SetDisableEditText(txtAsasTindakan);

            var txtNoAduan = FindViewById<EditText>(Resource.Id.txtNoAduan);
            txtNoAduan.Text = data.NoAduan;
            SetDisableEditText(txtNoAduan);

            var txtNoRujukanAtr = FindViewById<EditText>(Resource.Id.txtNoRujukanAtr);
            txtNoRujukanAtr.Text = data.NoRujukanAtr;
            SetDisableEditText(txtNoRujukanAtr);

            var txtCatatanLawatan = FindViewById<EditText>(Resource.Id.txtCatatanLawatan);
            txtCatatanLawatan.Text = data.CatatanLawatan;
            SetDisableEditText(txtCatatanLawatan);

            var txtHasilLawatan = FindViewById<EditText>(Resource.Id.txtHasilLawatan);
            txtHasilLawatan.Text = data.HasilLawatan;
            SetDisableEditText(txtHasilLawatan);


        }

        private void LoadDataPremis(TbKpp data)
        {

            var txtKategoriPremis = FindViewById<EditText>(Resource.Id.txtKategoriPremis);
            txtKategoriPremis.Text = MasterDataBll.GetKatPremisName(data.KodKatPremis);
            SetDisableEditText(txtKategoriPremis);

            var txtJenisPerniagaan = FindViewById<EditText>(Resource.Id.txtJenisPerniagaan);
            txtJenisPerniagaan.Text = MasterDataBll.GetJenisPerniagaanName(data.KodJenis);
            SetDisableEditText(txtJenisPerniagaan);

            var txtNamaPremis = FindViewById<EditText>(Resource.Id.txtNamaPremis);
            txtNamaPremis.Text = data.NamaPremis;
            SetDisableEditText(txtNamaPremis);

            var txtAlamat1 = FindViewById<EditText>(Resource.Id.txtAlamat1);
            txtAlamat1.Text = data.AlamatPremis1;
            SetDisableEditText(txtAlamat1);

            var txtAlamat2 = FindViewById<EditText>(Resource.Id.txtAlamat2);
            txtAlamat2.Text = data.AlamatPremis2;
            SetDisableEditText(txtAlamat2);

            var txtAlamat3 = FindViewById<EditText>(Resource.Id.txtAlamat3);
            txtAlamat3.Text = data.AlamatPremis3;
            SetDisableEditText(txtAlamat3);

            var txtNoDaftarSyarikat = FindViewById<EditText>(Resource.Id.txtNoDaftarSyarikat);
            txtNoDaftarSyarikat.Text = data.NoDaftarPremis;
            SetDisableEditText(txtNoDaftarSyarikat);

            var txtNoLesenBkPda = FindViewById<EditText>(Resource.Id.txtNoLesenBkPda);
            txtNoLesenBkPda.Text = data.NoLesenBKP_PDA;
            SetDisableEditText(txtNoLesenBkPda);

            var txtNoLesenMajelisPremis = FindViewById<EditText>(Resource.Id.txtNoLesenMajelisPremis);
            txtNoLesenMajelisPremis.Text = data.NoLesenMajlis_Permit;
            SetDisableEditText(txtNoLesenMajelisPremis);

            var txtNoTelefon = FindViewById<EditText>(Resource.Id.txtNoTelefon);
            txtNoTelefon.Text = data.NoTelefonPremis;
            SetDisableEditText(txtNoTelefon);

            var txtLainLain = FindViewById<EditText>(Resource.Id.txtLainLain);
            txtLainLain.Text = data.CatatanPremis;
            SetDisableEditText(txtLainLain);

            var chkAmaran = FindViewById<CheckBox>(Resource.Id.chkAmaran);
            if (data.Amaran == Constants.Amaran.Yes)
            {
                chkAmaran.Checked = true;
            }
            chkAmaran.Enabled = false;
        }

        private void LoadDataPenerima(TbKpp data)
        {

            var txtNamaPenerima = FindViewById<EditText>(Resource.Id.txtNamaPenerima);
            txtNamaPenerima.Text = data.NamaPenerima;
            SetDisableEditText(txtNamaPenerima);

            var txtNoKpPenerima = FindViewById<EditText>(Resource.Id.txtNoKpPenerima);
            txtNoKpPenerima.Text = data.NoKpPenerima;
            SetDisableEditText(txtNoKpPenerima);

            var txtJawatanPenerima = FindViewById<EditText>(Resource.Id.txtJawatanPenerima);
            txtJawatanPenerima.Text = data.Jawatanpenerima;
            SetDisableEditText(txtJawatanPenerima);


            var txtAlamatPenerima1 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima1);
            txtAlamatPenerima1.Text = data.AlamatPenerima1;
            SetDisableEditText(txtAlamatPenerima1);

            var txtAlamatPenerima2 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima2);
            txtAlamatPenerima2.Text = data.AlamatPenerima2;
            SetDisableEditText(txtAlamatPenerima2);

            var txtAlamatPenerima3 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima3);
            txtAlamatPenerima3.Text = data.AlamatPenerima3;
            SetDisableEditText(txtAlamatPenerima3);

            //var rdTiadaKes = FindViewById<RadioButton>(Resource.Id.rdTiadaKes);
            //rdTiadaKes.Enabled = false;

            //var rdKots = FindViewById<RadioButton>(Resource.Id.rdKots);
            //rdKots.Enabled = false;

            //var rdSiasatanLanjut = FindViewById<RadioButton>(Resource.Id.rdSiasatanLanjut);
            //rdSiasatanLanjut.Enabled = false;

            //if (data.Tindakan == Constants.Tindakan.TiadaKes)
            //{
            //    rdTiadaKes.Checked = true;
            //}
            //else if (data.Tindakan == Constants.Tindakan.Kots)
            //{
            //    rdKots.Checked = true;
            //}
            //else if (data.Tindakan == Constants.Tindakan.SiasatLanjutan)
            //{
            //    rdSiasatanLanjut.Checked = true;
            //}
            var linearSiasatUlangan = FindViewById<LinearLayout>(Resource.Id.linearSiasatUlangan);
            linearSiasatUlangan.Visibility = ViewStates.Invisible;

            var txtTindakan = FindViewById<EditText>(Resource.Id.txtTindakan);
            if (data.Tindakan == Constants.Tindakan.TiadaKes)
            {
                txtTindakan.Text = Constants.TindakanName.Pemeriksaan;
            }
            else if (data.Tindakan == Constants.Tindakan.Kots)
            {
                txtTindakan.Text = Constants.TindakanName.KOTS;
            }
            else if (data.Tindakan == Constants.Tindakan.SiasatLanjutan)
            {
                txtTindakan.Text = Constants.TindakanName.SiasatLanjut;
            }
            else if (data.Tindakan == Constants.Tindakan.SiasatUlangan)
            {
                txtTindakan.Text = Constants.TindakanName.SiasatUlangan;
                linearSiasatUlangan.Visibility = ViewStates.Visible;
                var txtNoEP = FindViewById<EditText>(Resource.Id.txtNoEP);
                txtNoEP.Text = data.NoEp;
                SetDisableEditText(txtNoEP);

                var txtNoIP = FindViewById<EditText>(Resource.Id.txtNoIP);
                txtNoIP.Text = data.NoIp;
                SetDisableEditText(txtNoIP);

                txtNoEP.SetFilters(new IInputFilter[] { new FilterChar() });
                txtNoIP.SetFilters(new IInputFilter[] { new FilterChar() });
            }

            SetDisableEditText(txtTindakan);


            var chkBayar = FindViewById<CheckBox>(Resource.Id.chkBayar);
            if (data.SetujuByr == Constants.SetujuBayar.Yes)
            {
                chkBayar.Checked = true;
            }
            chkBayar.Enabled = false;

            var dtData = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhTamatLawatanKpp);
            var txtTarikhTamat = FindViewById<TextView>(Resource.Id.txtTarikhTamat);
            txtTarikhTamat.Text = dtData.ToString(Constants.DateFormatDisplay);

            var txtMasaTamat = FindViewById<TextView>(Resource.Id.txtMasaTamat);
            txtMasaTamat.Text = dtData.ToString(Constants.TimeFormatDisplay);
        }

        private void LoadButton()
        {
            var btnOk = FindViewById<Button>(Resource.Id.btnOk);
            btnOk.Click += BtnOk_Click;

            var btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            btnCamera.Click += BtnCamera_Click;

            var btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            btnPrint.Click += BtnPrint_Click;

            var btnNote = FindViewById<Button>(Resource.Id.btnNote);
            btnNote.Click += BtnNote_Click;
        }

        private void BtnNote_Click(object sender, EventArgs e)
        {

            try
            {
                //var data = PemeriksaanBll.GetPemeriksaanByRujukan(_noRujukan);
                //if (data != null)
                //{
                //    if (_tindakan == Constants.Tindakan.Kots)
                //    {
                //        ShowKompaunPage();
                //    }
                //    else if (_tindakan == Constants.Tindakan.SiasatLanjutan)
                //    {
                //        ShowSiasatPage();
                //    }
                //}
                _hourGlass?.StartMessage(this, OnTindakan);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnNote_Click", ex.Message, Enums.LogType.Error);
            }

        }

        private void OnTindakan()
        {
            var data = PemeriksaanBll.GetPemeriksaanByRujukan(_noRujukan);
            if (data != null)
            {
                if (_tindakan == Constants.Tindakan.Kots)
                {
                    ShowKompaunPage();
                }
                else if (_tindakan == Constants.Tindakan.SiasatLanjutan)
                {
                    ShowSiasatPage();
                }
            }
            _hourGlass?.StopMessage();
        }

        private void ShowKompaunPage()
        {

            var data = KompaunBll.GetKompaunByRujukanKpp(lblNoKpp.Text);
            if (data != null)
            {
                var intent = new Intent(this, typeof(ViewKompaun));
                intent.PutExtra("JenisKmp", ((int)Enums.JenisKompaun.KOTS).ToString());
                intent.PutExtra("NoRujukanKpp", lblNoKpp.Text);
                StartActivity(intent);
            }
            else
            {
                GeneralAndroidClass.ShowToast("Tidak ada data KOTS");
            }
        }

        private void ShowSiasatPage()
        {
            var data = KompaunBll.GetSiasatByRujukanKpp(lblNoKpp.Text);
            if (data != null)
            {
                var intent = new Intent(this, typeof(ViewSiasatLanjut));
                intent.PutExtra("NoRujukanKpp", lblNoKpp.Text);
                StartActivity(intent);
            }
            else
            {
                GeneralAndroidClass.ShowToast("Tidak ada data Siasatan Lanjut");
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // //NoRujukan = lblNoKpp.Text;
                // //Print(true);
                //// GeneralAndroidClass.ShowToast("Print");
                //var printImageBll = new PrintImageBll();
                //var bitmap = printImageBll.Pemeriksaan(this, lblNoKpp.Text);
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
            intent.PutExtra("filename", _noRujukan);
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
            SetLayoutInvisible(viewPremis, lblTabPremis, tabPremis);
            SetLayoutInvisible(viewLawatan, lblTabLawatan, tabLawatan);
            SetLayoutVisible(viewPenerima, lblTabPenerima, tabPenerima);
        }

        private void TabLawatan_Click(object sender, EventArgs e)
        {
            SetLayoutInvisible(viewPremis, lblTabPremis, tabPremis);
            SetLayoutVisible(viewLawatan, lblTabLawatan, tabLawatan);
            SetLayoutInvisible(viewPenerima, lblTabPenerima, tabPenerima);
        }

        private void TabPremis_Click(object sender, EventArgs e)
        {
            SetLayoutVisible(viewPremis, lblTabPremis, tabPremis);
            SetLayoutInvisible(viewLawatan, lblTabLawatan, tabLawatan);
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
                    GeneralAndroidClass.ShowToast(Constants.ErrorMessages.PrinterNotFound);
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

        void lvResult_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
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
#if !DEBUG
            if (isNeedCheck)
            {
                PreparePrinterDevice();

                if (GlobalClass.BluetoothDevice == null)
                {
                    //_dialog?.Dismiss();
                    return;
                }
            }

            //RunOnUiThread(() => GetFWCode()) ;
            GetFWCode();

#endif
            new Task(() =>
            {
                try
                {
                    //RunOnUiThread(() => OnPrinting());
                    OnPrinting();
                    IsLoading(this, false);
                }
                catch (Exception ex)
                {
                    IsLoading(this, false);
                    GeneralAndroidClass.LogData(LayoutName, "Print", ex.Message, Enums.LogType.Error);
                }
            }).RunSynchronously();
        }

        //BluetoothPrintService printService = null;
        //ServicetHandler handler;

        public const int MESSAGE_DEVICE_NAME = 1;
        public const int MESSAGE_TOAST = 2;
        public const int MESSAGE_READ = 3;
        public const string DEVICE_NAME = "device_name";

        class ServicetHandler : Handler
        {
            readonly ViewPemeriksaan activity;

            public ServicetHandler(ViewPemeriksaan activity)
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
                Log.WriteLogFile("Pemeriksaan", "OnPrinting", "Not Connected print", Enums.LogType.Info);
                GeneralAndroidClass.ShowToast("Tiada Sambungan Pencetak");
            }
            else if (data.Length > 0)
            {
                GlobalClass.printService.Write(data);
            }
        }

        public async Task ShowMessageNew(bool value, string message)
        {
            IsLoading(this, value, message);
            await Task.Delay(Constants.DefaultWaitingMilisecond);
        }

        private async Task OnPrinting()
        {
            Log.WriteLogFile("Printer Firmware : " + GlobalClass.FwCode);
#if DEBUG
            //await ShowMessageNew(true, "Loading...");
            //Thread.Sleep(1000);
            //await ShowMessageNew(true, "Message 1");
            //Thread.Sleep(2000);
            //await ShowMessageNew(true, "Message 2");
            //Thread.Sleep(2000);
            //await ShowMessageNew(true, "Message 3");
            //Thread.Sleep(2000);
            //RunOnUiThread(() => GeneralAndroidClass.ShowToast(Constants.Messages.GenerateBitmap));
            //Thread.Sleep(2000);
            var bitmapDebug = new PrintImageBll().Pemeriksaan(this, lblNoKpp.Text);
            return;
#endif
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

            //IsLoading(this, true, Constants.Messages.GenerateBitmap);
            //RunOnUiThread(() => GeneralAndroidClass.ShowToast(Constants.Messages.GenerateBitmap));
            await ShowMessageNew(true, Constants.Messages.GenerateBitmap);

            var printImageBll = new PrintImageBll();
            var bitmap = printImageBll.Pemeriksaan(this, lblNoKpp.Text);

            await ShowMessageNew(true, Constants.Messages.ConnectionToBluetooth);

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

            //IsLoading(this, true, Constants.Messages.PrintWaitMessage);
            await ShowMessageNew(true, Constants.Messages.PrintWaitMessage);

            //#if !DEBUG
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
                Thread.Sleep(6000);
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


        #endregion
    }
}