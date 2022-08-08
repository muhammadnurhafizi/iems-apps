using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Com.Woosim.Printer;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;

namespace IEMSApps.Activities
{
    [Activity(Label = "View Akuan", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ViewAkuan : BaseActivity
    {
        private const string LayoutName = "ViewAkuan";

        private string _noRujukan;
        private AlertDialog _dialog;
        private HourGlassClass _hourGlass = new HourGlassClass();
        ServicetHandler handler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ViewAkuan);

            //SetInit();
            _hourGlass?.StartMessage(this, SetInit);
        }

        private void SetInit()
        {
            try
            {
                var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
                txtHhId.Text = GeneralBll.GetUserHandheld();

                _noRujukan = Intent.GetStringExtra("NoRujukan") ?? "";

                var kompaun = KompaunBll.GetKompaunByRujukan(_noRujukan);
                if (kompaun.Success)
                {

                    LoadData(kompaun.Datas);
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }
            _hourGlass?.StopMessage();
        }

        private void LoadData(TbKompaun data)
        {

            var txtNamaPenerima = FindViewById<EditText>(Resource.Id.txtNamaPenerima);
            txtNamaPenerima.Text = data.NamaPenerima_Akuan;
            SetDisableEditText(txtNamaPenerima);

            var txtNoKpPenerima = FindViewById<EditText>(Resource.Id.txtNoKpPenerima);
            txtNoKpPenerima.Text = data.NoKpPenerima_Akuan;
            SetDisableEditText(txtNoKpPenerima);

            var txtAlamatPenerima1 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima1);
            txtAlamatPenerima1.Text = data.AlamatPenerima1_Akuan;
            SetDisableEditText(txtAlamatPenerima1);

            var txtAlamatPenerima2 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima2);
            txtAlamatPenerima2.Text = data.AlamatPenerima2_Akuan;
            SetDisableEditText(txtAlamatPenerima2);

            var txtAlamatPenerima3 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima3);
            txtAlamatPenerima3.Text = data.AlamatPenerima3_Akuan;
            SetDisableEditText(txtAlamatPenerima3);

            var txtNoResit = FindViewById<EditText>(Resource.Id.txtNoResit);
            txtNoResit.Text = data.NoResit;
            SetDisableEditText(txtNoResit);

            var txtAmounBayaran = FindViewById<EditText>(Resource.Id.txtAmounBayaran);
            txtAmounBayaran.Text = data.AmnByr.ToString(Constants.DecimalFormat);
            SetDisableEditText(txtAmounBayaran);

            var btnOk = FindViewById<Button>(Resource.Id.btnOk);
            btnOk.Click += BtnOk_Click;

            var btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            btnPrint.Click += BtnPrint_Click;



        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                var printImageBll = new PrintImageBll();
                var bitmap = printImageBll.Akuan(this, _noRujukan);
                GeneralAndroidClass.ShowToast("Sedang cetak");
                return;
#endif

                //var printImageBll = new PrintImageBll();
                //var bitmap = printImageBll.Akuan(this, _noRujukan);
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
            GetFWCode();
#endif
            new Task(() =>
            {
                try
                {
                    OnPrinting();
                    IsLoading(this, false);
                }
                catch (Exception ex)
                {
                    IsLoading(this, false);
                    GeneralAndroidClass.LogData(LayoutName, "Print", ex.Message, Enums.LogType.Error);
                }
            }).Start();
        }


        public const int MESSAGE_DEVICE_NAME = 1;
        public const int MESSAGE_TOAST = 2;
        public const int MESSAGE_READ = 3;
        public const string DEVICE_NAME = "device_name";

        class ServicetHandler : Handler
        {
            readonly ViewAkuan activity;

            public ServicetHandler(ViewAkuan activity)
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
            var bitmap = printImageBll.Akuan(this, _noRujukan);

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
    }
}