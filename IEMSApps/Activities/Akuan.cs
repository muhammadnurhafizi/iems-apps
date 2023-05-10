using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Woosim.Printer;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Inputs;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using Plugin.BxlMpXamarinSDK;
using Plugin.BxlMpXamarinSDK.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static IEMSApps.Utils.Constants;
using static IEMSApps.Utils.Enums;

namespace IEMSApps.Activities
{
    [Activity(Label = "Akuan", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Akuan : BaseActivity
    {
        private HourGlassClass _hourGlass = new HourGlassClass();

        //initialize printer bixolon
        private MPosControllerPrinter _printer;
        MposConnectionInformation _connectionInfo;
        private static SemaphoreSlim _printSemaphore = new SemaphoreSlim(1, 1);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Akuan);

            _printer = null;
            _connectionInfo = null;

            //SetInit();
            _hourGlass?.StartMessage(this, SetInit);

        }
        private const string LayoutName = "Akuan";
        private Button btnNamaPenerima;
        private TextView txtNamaPenerima, txtNoKpPenerima, txtNoResit, txtAmounBayaran;
        private TextView txtAlamatPenerima1, txtAlamatPenerima2, txtAlamatPenerima3;
        private Button btnOk, btnCamera, btnPrint, btnAkuan, btnSearchJpnPenerima, btnReceipt;

        private Spinner spJenisKad, spNegeriPenerima;
        private EditText txtNoTelefonPenerima, txtEmailPenerima, txtBandarPenerima, txtPoskodPenerima;
        private Button btnBandarPenerima, btnPoskodPenerima;
        private CheckBox chkGambarBayaran, chkBayarGunaIpayment;

        private AlertDialog _dialog;
        private bool _isSaved = false;
        private bool _reprint;
        private string _noRujukan;
        private string _trkhPenerima = "";

        ServicetHandler handler;


        private void SetInit()
        {
            try
            {
                var allowFilter = new FilterChar();
                var allowFilterWithoutSingleQuote = new FilterCharWithoutSingleQuote();

                var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
                txtHhId.Text = GeneralBll.GetUserHandheld();

                _noRujukan = Intent.GetStringExtra("NoRujukan") ?? "";

                btnNamaPenerima = FindViewById<Button>(Resource.Id.btnNamaPenerima);
                txtNamaPenerima = FindViewById<EditText>(Resource.Id.txtNamaPenerima);
                txtNoKpPenerima = FindViewById<EditText>(Resource.Id.txtNoKpPenerima);

                //newadd
                spJenisKad = FindViewById<Spinner>(Resource.Id.spJenisKad);
                txtNoTelefonPenerima = FindViewById<EditText>(Resource.Id.txtNoTelefonPenerima);
                txtEmailPenerima = FindViewById<EditText>(Resource.Id.txtEmailPenerima);
                spNegeriPenerima = FindViewById<Spinner>(Resource.Id.spNegeriPenerima);
                txtBandarPenerima = FindViewById<EditText>(Resource.Id.txtBandarPenerima);
                btnBandarPenerima = FindViewById<Button>(Resource.Id.btnBandarPenerima);
                btnBandarPenerima.Click += BtnBandarPenerima_Click;
                txtPoskodPenerima = FindViewById<EditText>(Resource.Id.txtPoskodPenerima);
                btnPoskodPenerima = FindViewById<Button>(Resource.Id.btnPoskodPenerima);
                btnPoskodPenerima.Click += BtnPoskodPenerima_Click;

                txtAlamatPenerima1 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima1);
                txtAlamatPenerima2 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima2);
                txtAlamatPenerima3 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima3);

                chkGambarBayaran = FindViewById<CheckBox>(Resource.Id.chkGambarBayaran);
                chkBayarGunaIpayment = FindViewById<CheckBox>(Resource.Id.chkBayarGunaIpayment);

                txtNoResit = FindViewById<EditText>(Resource.Id.txtNoResit);
                txtAmounBayaran = FindViewById<EditText>(Resource.Id.txtAmounBayaran);

                btnOk = FindViewById<Button>(Resource.Id.btnOk);
                btnOk.Click += BtnOk_Click;

                btnOk.Enabled = false;
                btnOk.SetBackgroundResource(Resource.Drawable.backdisable);

                btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
                btnPrint.Click += BtnPrint_Click;

                btnReceipt = FindViewById<Button>(Resource.Id.btnReceipt);
                btnReceipt.Click += BtnReceipt_Click;

                btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
                btnCamera.Click += btnCamera_Click;

                chkGambarBayaran.CheckedChange += chkGambarBayaran_Click;
                chkBayarGunaIpayment.CheckedChange += chkBayarGunaIpayment_Click;

                btnNamaPenerima.Click += BtnNamaPenerima_Click;

                txtNamaPenerima.TextChanged += Event_CheckMandatory_Edittext;
                txtAlamatPenerima1.TextChanged += Event_CheckMandatory_Edittext;
                txtNoResit.TextChanged += Event_CheckMandatory_Edittext;
                txtAmounBayaran.TextChanged += Event_CheckMandatory_Edittext;

                txtNamaPenerima.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
                txtNoKpPenerima.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowFilterWithoutSingleQuote });
                txtAlamatPenerima1.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(Constants.AllowAddressCharacter), allowFilter });
                txtAlamatPenerima2.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(Constants.AllowAddressCharacter), allowFilter });
                txtAlamatPenerima3.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(Constants.AllowAddressCharacter), allowFilter });
                txtNoResit.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(20), allowFilterWithoutSingleQuote });
                txtPoskodPenerima.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(5), allowFilter });
                //txtAmounBayaran.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(20) });
                txtAmounBayaran.Enabled = false;

                btnSearchJpnPenerima = FindViewById<Button>(Resource.Id.btnSearchJpnPenerima);
                btnSearchJpnPenerima.Click += btnSearchJpnPenerima_Click;

                loadDropdownData();
                SetPrintButton();

                var kompaun = KompaunBll.GetKompaunByRujukan(_noRujukan);
                if (kompaun.Success)
                {
                    var positionJenisKad = PasukanBll.GetPositionSelected(ListJenisKad, kompaun.Datas.ip_identiti_pelanggan_id.ToString());
                    var positionNegeriPenerima = PasukanBll.GetPositionSelected(ListNegeri, kompaun.Datas.negeripenerima);

                    txtNamaPenerima.Text = kompaun.Datas.NamaPenerima;
                    txtNoKpPenerima.Text = kompaun.Datas.NoKpPenerima;
                    spJenisKad.SetSelection(positionJenisKad);
                    txtNoTelefonPenerima.Text = kompaun.Datas.notelpenerima;
                    txtEmailPenerima.Text = kompaun.Datas.emelpenerima;
                    spNegeriPenerima.SetSelection(positionNegeriPenerima);
                    txtBandarPenerima.Text = kompaun.Datas.bandarpenerima;
                    txtPoskodPenerima.Text = kompaun.Datas.poskodpenerima;
                    txtAlamatPenerima1.Text = kompaun.Datas.AlamatPenerima1;
                    txtAlamatPenerima2.Text = kompaun.Datas.AlamatPenerima2;
                    txtAlamatPenerima3.Text = kompaun.Datas.AlamatPenerima3;
                    _trkhPenerima = kompaun.Datas.TrkhPenerima;
                    txtAmounBayaran.Text = kompaun.Datas.AmnKmp.ToString(Constants.DecimalFormatZero);

                    if (!string.IsNullOrEmpty(kompaun.Datas.NoResit))
                    {
                        _isSaved = true;
                        SetEnableControl(false);

                        LoadDataExisting(kompaun.Datas);

                        btnOk.Enabled = true;
                        btnOk.SetBackgroundResource(Resource.Drawable.backblue);
                    }
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }
            _hourGlass?.StopMessage();
        }

        private void chkBayarGunaIpayment_Click(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (e.IsChecked)
                {
                    btnReceipt.SetBackgroundResource(Resource.Drawable.receipt_icon_enable);
                    btnReceipt.Enabled = true;
                }
                else
                {
                    btnReceipt.SetBackgroundResource(Resource.Drawable.receipt_icon);
                    btnReceipt.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "chkBayarGunaIpayment_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnPoskodPenerima_Click(object sender, EventArgs e)
        {
            try
            {
                ShowPoskodPenerima();
            }
            catch (Exception ex)
            {

                GeneralAndroidClass.LogData(LayoutName, "BtnPoskodPenerima_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void chkGambarBayaran_Click(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (e.IsChecked)
                {
                    btnCamera.SetBackgroundResource(Resource.Drawable.camera_icon);
                    btnCamera.Enabled = true;
                } 
                else 
                {
                    btnCamera.SetBackgroundResource(Resource.Drawable.camera_disable);
                    btnCamera.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "chkGambarBayaran_Check", ex.Message, Enums.LogType.Error);
            }

        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            
            //string resit = "IPRESIT";
              
            try
            {
                var kompaun = KompaunBll.GetKompaunByRujukan(_noRujukan);

                if (string.IsNullOrEmpty(kompaun.Datas.NoRujukanKpp))
                {
                    GeneralAndroidClass.ShowModalMessage(this, "No Rujukan Kosong");
                    return;
                }
                var _noResit = Constants.Receipt + kompaun.Datas.NoRujukanKpp;
                var intent = new Intent(this, typeof(Camera));
                intent.PutExtra("filename", _noResit);
                if (_isSaved)
                {
                    intent.PutExtra("allowtakepicture", false);
                    intent.PutExtra("allowreplace", false);
                }

                StartActivity(intent);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnCamera_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private Dictionary<string, string> ListJenisKad, ListNegeri;
        private void loadDropdownData() {

            ListJenisKad = MasterDataBll.GetJenisKad();
            spJenisKad.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, ListJenisKad.Select(c => c.Value).ToList());

            //ListNegeri = MasterDataBll.GetAllNegeri();
            ListNegeri = MasterDataBll.GetAllNegeriNew();
            spNegeriPenerima.Adapter = new ArrayAdapter<string>(this,
                Resource.Layout.support_simple_spinner_dropdown_item, ListNegeri.Select(c => c.Value).ToList());

        }

        private void BtnBandarPenerima_Click(object sender, EventArgs e)
        {
            try
            {
                ShowBandarPenerima();
            }
            catch (Exception ex)
            {

                GeneralAndroidClass.LogData(LayoutName, "BtnBandarPenerima_Click", ex.Message, Enums.LogType.Error);
            }
        }

        List<PoskodPenerimaDto> listOfPoskod;
        public void ShowPoskodPenerima()
        {
            string selectedBandar = txtBandarPenerima.Text;

            var listOfPoskodFiltered = new List<PoskodPenerimaDto>();
            if (selectedBandar == "")
            {
                listOfPoskod = MasterDataBll.GetAllPoskod();
                listOfPoskodFiltered = listOfPoskod;
            }
            else
            {
                listOfPoskod = MasterDataBll.GetPoskodByBandar(selectedBandar);
                listOfPoskodFiltered = listOfPoskod;
            }


            var builder = new AlertDialog.Builder(this).Create();
            var view = this.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);

            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Poskod";

            listView.Adapter = new CarianPoskodPenerimaAdapter(this, listOfPoskod);

            txtCarian.TextChanged += (send, args) =>
            {
                listOfPoskodFiltered = listOfPoskod
                    .Where(m => m.name.ToLower().Contains(txtCarian.Text.ToLower())).ToList();

                listView.Adapter = new CarianPoskodPenerimaAdapter(this, listOfPoskodFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                txtPoskodPenerima.Text = listOfPoskodFiltered[args.Position]?.name;

                var kodBandar = listOfPoskodFiltered[args.Position] != null
                    ? listOfPoskodFiltered[args.Position].ip_bandar_id
                    : 0;

                var bandarPenerima = MasterDataBll.GetBandarPenerimaByPoskod(txtPoskodPenerima.Text);
                txtBandarPenerima.Text = bandarPenerima;

                var IdNegeri = MasterDataBll.GetNegeriPenerimaByBandar(txtBandarPenerima.Text);
                spNegeriPenerima.SetSelection(IdNegeri);

                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();

        }

        EditText txtCarian;
        ListView listView;
        List<BandarDto> listOfBandar;
        private void ShowBandarPenerima()
        {
            var selectedNegeri = GeneralBll.GetKeySelected(ListNegeri, spNegeriPenerima.SelectedItem?.ToString() ?? "");

            listOfBandar = MasterDataBll.GetBandarByNegeri(selectedNegeri);


            var listOfBandarFiltered = listOfBandar;


            var builder = new AlertDialog.Builder(this).Create();
            var view = this.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);

            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Bandar";

            listView.Adapter = new CarianBandarAdapter(this, listOfBandar);

            txtCarian.TextChanged += (send, args) =>
            {
                listOfBandarFiltered = listOfBandar
                    .Where(m => m.Prgn.ToLower().Contains(txtCarian.Text.ToLower())).ToList();

                listView.Adapter = new CarianBandarAdapter(this, listOfBandarFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                txtBandarPenerima.Text = listOfBandarFiltered[args.Position]?.Prgn;

                var negeriName = MasterDataBll.GetNegeriName(GeneralBll.ConvertStringToInt(selectedNegeri));
                var kodBandar = listOfBandarFiltered[args.Position] != null
                    ? listOfBandarFiltered[args.Position].KodBandar
                    : 0;

                var bandarName = MasterDataBll.GetBandarNameByNegeri(GeneralBll.ConvertStringToInt(selectedNegeri), kodBandar);

                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private void LoadDataExisting(TbKompaun data)
        {

            txtNamaPenerima.Text = data.NamaPenerima_Akuan;
            txtNoKpPenerima.Text = data.NoKpPenerima_Akuan;
            txtAlamatPenerima1.Text = data.AlamatPenerima1_Akuan;
            txtAlamatPenerima2.Text = data.AlamatPenerima2_Akuan;
            txtAlamatPenerima3.Text = data.AlamatPenerima3_Akuan;
            txtNoResit.Text = data.NoResit;
            txtAmounBayaran.Text = data.AmnByr.ToString(Constants.DecimalFormat);

            btnPrint.SetBackgroundResource(Resource.Drawable.print_icon);
            btnPrint.Enabled = true;
        }

        [Obsolete]
        private void BtnReceipt_Click(object sender, EventArgs e)
        {

            try
            {
                _dialog = GeneralAndroidClass.ShowProgressDialog(this, Constants.Messages.CheckResit);
                new Thread(() =>
                {
                    Thread.Sleep(1000);
                    this.RunOnUiThread(CheckReceiptOnline);
                }).Start();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnReceipt_Click", ex.Message, Enums.LogType.Error);
            }

        }

        [Obsolete]
        private void CheckReceiptOnline()
        {
            var kompaun = KompaunBll.GetKompaunByRujukan(_noRujukan);
            var norujukan = kompaun.Datas.NoRujukanKpp;

            var data = AkuanBll.CheckIpResitsData(norujukan);

            if (data.Success)
            {
                if (data.Datas == null)
                {
                    var service = AkuanBll.CheckServiceReceiptIP(norujukan, this);
                    if (service.Success) 
                    {
                        if (service.Result != null) 
                        {
                            txtNoResit.Text = service.Result.no_resit;

                            var message = Constants.Messages.BayarBerjaya;
                            var ad = GeneralAndroidClass.GetDialogCustom(this);
                            ad.SetMessage(Html.FromHtml(message));
                            ad.SetButton(Constants.Close, (s, ev) => { });
                            ad.SetButton2(Constants.ViewResit, (s, ev) =>
                            {
                                _hourGlass.StartMessage(this, ShowReceipt);
                            });
                            ad.Show();
                        } else
                        {
                            var message = "Gagal Mendapatkan Data Dari Sistem IEMS";
                            var ad = GeneralAndroidClass.GetDialogCustom(this);
                            ad.SetMessage(Html.FromHtml(message));
                            ad.SetButton("Tutup", (s, ev) => { });
                            ad.Show();
                        }
                    } 
                    else
                    {
                        //GeneralAndroidClass.ShowToast(service.Mesage);
                        var message = Constants.Messages.NoReceiptOnServer;
                        var ad = GeneralAndroidClass.GetDialogCustom(this);
                        ad.SetMessage(Html.FromHtml(message));
                        ad.SetButton("Tutup", (s, ev) => { });
                        ad.Show();
                    }
                    
                }
                else
                { 
                    var message = "Resit telah Dijana";

                    var ad = GeneralAndroidClass.GetDialogCustom(this);
                    ad.SetMessage(Html.FromHtml(message));
                    ad.SetButton2("OK", (s, ev) =>
                    {
                        _hourGlass.StartMessage(this, ShowReceipt);

                        _hourGlass.StopMessage();
                    });
                    ad.Show();
                }
            }
            _dialog.Dismiss();
        }

        private void ShowReceipt()
        {
            var kompaun = KompaunBll.GetKompaunByRujukan(_noRujukan);
            var norujukan = kompaun.Datas.NoRujukanKpp;

            var data = AkuanBll.CheckIpResitsData(norujukan);
            if (data.Datas == null) 
            {
                GeneralAndroidClass.ShowModalMessage(this, "Gagal memaparkan resit dari Database Gajet");

                _hourGlass.StopMessage();
                return;
                //check logic here, jika data gagal masuk dalam db gajet jadi tiada data, jika dah masuk tapi tak boleh nk display , lain masalah.
            } else 
            {
                txtNoResit.Text = data.Datas.no_resit;
            }
            
            var intent = new Intent(this, typeof(Activities.ReceiptIpayment));
            intent.PutExtra("NoRujukanKpp", norujukan);
            StartActivity(intent);

            _hourGlass.StopMessage();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isSaved)
                {
                    Finish();
                }
                else
                {
                    var ad = GeneralAndroidClass.GetDialogCustom(this);

                    ad.SetMessage(Constants.Messages.BackNotSave);
                    // Positive

                    ad.SetButton("Tidak", (s, ev) => { });
                    ad.SetButton2("Ya", (s, ev) =>
                    {
                        Finish();
                    });
                    ad.Show();
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnOk_Click1", ex.Message, Enums.LogType.Error);
            }
        }

        private static int REQUEST_MYKAD = 1001;

        private void BtnNamaPenerima_Click(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(Intent.ActionMain);
                intent.SetComponent(new ComponentName("com.aimforce.mykad.woosim", "com.aimforce.mykad.woosim.MainActivity"));
                intent.PutExtra("command", "no-ui");
                intent.PutExtra("command2", "no-photo");
                StartActivityForResult(intent, REQUEST_MYKAD);
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnNamaPenerima_Click", ex.Message, Enums.LogType.Error);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                string result;

                if (requestCode == REQUEST_MYKAD)
                {
                    if (resultCode == Result.Ok)
                    {
                        result = data.GetStringExtra("result");
                        var card = GeneralBll.ReadMyKadInfo(result);
                        if (card.IsSuccessRead)
                            SetMyCard(card);
                        else
                            GeneralAndroidClass.ShowToast(card.Message);

                    }
                }
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "OnActivityResult", ex.Message, Enums.LogType.Error);
            }

        }

        private void SetMyCard(CardInfoDto cardDto)
        {
            var listAddress = new List<string>();

            txtNamaPenerima.Text = cardDto.originalName;
            txtNoKpPenerima.Text = cardDto.idNum;

            var address = $"{cardDto.address1} {cardDto.address2}";
            var addressPostCodeCity = $"{cardDto.postcode} {cardDto.city} {cardDto.state}";


            if (string.IsNullOrEmpty(cardDto.address3))
            {
                listAddress = GeneralBll.SeparateText(address, 2, Constants.MaxLengthAddress);
                txtAlamatPenerima1.Text = listAddress[0];
                if (string.IsNullOrEmpty(listAddress[1]))
                    txtAlamatPenerima2.Text = addressPostCodeCity;
                else
                {
                    txtAlamatPenerima2.Text = listAddress[1];
                    txtAlamatPenerima3.Text = addressPostCodeCity;
                }
            }
            else
            {
                if (address.Length <= 80)
                {
                    txtAlamatPenerima1.Text = address;
                    txtAlamatPenerima2.Text = cardDto.address3;
                    txtAlamatPenerima3.Text = addressPostCodeCity;
                }
                else
                {
                    address = string.Format("{0} {1} {2}", cardDto.address1, cardDto.address2, cardDto.address3);

                    var listString = GeneralBll.SeparateText(address, 2, Constants.MaxLengthAddress);
                    txtAlamatPenerima1.Text = listString[0].Trim();
                    txtAlamatPenerima2.Text = listString[1].Trim();
                    txtAlamatPenerima3.Text = addressPostCodeCity;
                }

            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {

                if (_isSaved)
                {
                    var ad = GeneralAndroidClass.GetDialogCustom(this);

                    ad.SetMessage(Constants.Messages.DialogRePrint);
                    // Positive

                    ad.SetButton("Tidak", (s, ev) => { });
                    ad.SetButton2("Ya", (s, ev) =>
                    {
                        _reprint = true;
                        Print(true);
                    });
                    ad.Show();


                }
                else
                {
                    if (ValidateData())
                    {
                        _reprint = false;
                        ShowConfirmModal();
                    }

                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnPrint_Click", ex.Message, Enums.LogType.Error);
            }
        }



        private bool ValidateData()
        {


            if (string.IsNullOrEmpty(txtNamaPenerima.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Nama penerima kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(txtAlamatPenerima1.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Alamat penerima kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(txtNoResit.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "No resit kosong.");
                return false;
            }
            if (string.IsNullOrEmpty(txtAmounBayaran.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Amaun bayar kosong.");
                return false;
            }
            if (spJenisKad.SelectedItem == null) 
            {
                GeneralAndroidClass.ShowModalMessage(this, "Jenis Kad Kosong.");
                return false;
            }
            if (spNegeriPenerima.SelectedItem == null) 
            {
                GeneralAndroidClass.ShowModalMessage(this, "Negeri Penerima Kosong.");
                return false;
            }
            if (string.IsNullOrEmpty(txtBandarPenerima.Text)) 
            {
                GeneralAndroidClass.ShowModalMessage(this, "Bandar Penerima Kosong.");
                return false;
            }
            if (string.IsNullOrEmpty(txtBandarPenerima.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Poskod Penerima Kosong.");
                return false;
            }

            return true;
        }

        private void SaveData()
        {
            var input = new SaveAkuanInput
            {
                NoRujukan = _noRujukan,
                NamaPenerima = txtNamaPenerima.Text,
                jeniskad = GeneralBll.GetKeySelected(ListJenisKad, spJenisKad.SelectedItem?.ToString() ?? ""),
                NoKpPenerima = txtNoKpPenerima.Text,
                notelpenerima = txtNoTelefonPenerima.Text,
                emelpenerima = txtEmailPenerima.Text,
                negeripenerima = GeneralBll.GetKeySelected(ListNegeri, spNegeriPenerima.SelectedItem?.ToString() ?? ""),
                bandarpenerima = txtBandarPenerima.Text,
                poskodpenerima = txtPoskodPenerima.Text,
                AlamatPenerima1 = txtAlamatPenerima1.Text,
                AlamatPenerima2 = txtAlamatPenerima2.Text,
                AlamatPenerima3 = txtAlamatPenerima3.Text,
                NoResit = txtNoResit.Text,
                AmountByr = GeneralBll.ConvertStringToDecimal(txtAmounBayaran.Text),
                TrkhPenerima = _trkhPenerima,
                isbayarmanual = chkGambarBayaran.Checked ? Constants.GambarBayaran.Yes : Constants.GambarBayaran.No,
            };

            if (KompaunBll.SaveDataAkuanTrx(input))
            {
                SetEnableControl(false);
                _isSaved = true;

                btnOk.Enabled = true;
                btnOk.SetBackgroundResource(Resource.Drawable.backblue);

                _dialog?.Dismiss();

                Print(true);
            }
            else
            {
                _dialog?.Dismiss();
                GeneralAndroidClass.ShowModalMessage(this, Constants.ErrorMessages.FailedSaveData);
            }


        }

        private void OnModalDialog(string message)
        {
            var ad = GeneralAndroidClass.GetDialogCustom(this);

            ad.SetMessage(message);
            // ad.DismissEvent += Ad_DismissEvent;
            // Positive
            ad.SetButton2("OK", (s, e) => { this.Finish(); });
            ad.Show();
        }

        //private void Ad_DismissEvent(object sender, EventArgs e)
        //{
        //    Finish();
        //}

        private void SetPrintButton()
        {

            if (IsAllDataRequiredNoEmpty())
            {
                btnPrint.SetBackgroundResource(Resource.Drawable.print_icon);
                btnPrint.Enabled = true;
            }
            else
            {
                btnPrint.SetBackgroundResource(Resource.Drawable.printicon_disable);
                btnPrint.Enabled = false;
            }
        }

        private bool IsAllDataRequiredNoEmpty()
        {
            if (string.IsNullOrEmpty(txtNamaPenerima.Text)) return false;
            if (spJenisKad.SelectedItem == null) return false;
            if (string.IsNullOrEmpty(txtAlamatPenerima1.Text)) return false;
            if (string.IsNullOrEmpty(txtNoResit.Text)) return false;
            if (string.IsNullOrEmpty(txtAmounBayaran.Text)) return false;
            if (spNegeriPenerima.SelectedItem == null) return false;
            if (string.IsNullOrEmpty(txtBandarPenerima.Text)) return false;
            if (string.IsNullOrEmpty(txtPoskodPenerima.Text)) return false;
            return true;
        }


        private void Event_CheckMandatory_Edittext(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "Event_CheckMandatory_Dropdown_Edittext", ex.Message, Enums.LogType.Error);
            }
        }

        //public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        //{
        //    if (keyCode == Keycode.Back)
        //        return false;

        //    return base.OnKeyDown(keyCode, e);
        //}

        private void ShowConfirmModal()
        {
            var dialogView = View.Inflate(this, Resource.Layout.ConfirmLayout, null);
            var alertDialog = new AlertDialog.Builder(this).Create();

            var lvResult = dialogView.FindViewById<ListView>(Resource.Id.lView);
            var lblTitle = dialogView.FindViewById<TextView>(Resource.Id.lblTitle);
            lblTitle.Text = LayoutName;

            var listConfirm = GetListConfirm();

            lvResult.Adapter = new ConfirmListAdapter(this, listConfirm);
            lvResult.FastScrollEnabled = true;

            var btnCancel = dialogView.FindViewById<Button>(Resource.Id.btnCancel);
            btnCancel.Click += (sender, e) =>
            {
                alertDialog.Dismiss();
            };

            var btnCetak = dialogView.FindViewById<Button>(Resource.Id.btnCetak);
            btnCetak.Click += (sender, e) =>
            {
                _dialog = GeneralAndroidClass.ShowProgressDialog(this, Constants.Messages.SavingData);
                new Thread(() =>
                {
                    alertDialog.Dismiss();
                    Thread.Sleep(1000);
                    this.RunOnUiThread(SaveData);
                }).Start();
            };

            alertDialog.SetView(dialogView);
            alertDialog.Show();

        }

        private List<ConfirmDto> GetListConfirm()
        {
            var result = new List<ConfirmDto>();

            result.Add(GeneralBll.CreateConfirmDto("Nama", txtNamaPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Jenis Kad", spJenisKad.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("No. K/P", txtNoKpPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("No Telefon", txtNoTelefonPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Email", txtEmailPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Negeri", spNegeriPenerima.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Bandar", txtBandarPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Poskod", txtPoskodPenerima.Text));
            var alamat = GeneralBll.GettOneAlamat(txtAlamatPenerima1.Text, txtAlamatPenerima2.Text,
                txtAlamatPenerima3.Text);
            result.Add(GeneralBll.CreateConfirmDto("Alamat", alamat));

            result.Add(GeneralBll.CreateConfirmDto("No. Resit", txtNoResit.Text));
            result.Add(GeneralBll.CreateConfirmDto("Amaun Bayar", txtAmounBayaran.Text));
            result.Add(GeneralBll.CreateConfirmDto("Bayar Menggunakan Ipayment ? ", chkBayarGunaIpayment.Checked ? "Ya" : "Tidak"));
            result.Add(GeneralBll.CreateConfirmDto("Ambil Gambar Pembayaran Secara Manual", chkGambarBayaran.Checked ? "Ya" : "Tidak"));

            return result;
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
                    new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        Thread.CurrentThread.IsBackground = true;
                        RunOnUiThread(PrepareSendDataOnline);
                    }).Start();
                }
                else
                {
                    if (GlobalClass.BluetoothDevice == null)
                    {
                        lvResult = new ListView(this);
                        var adapter = new DeviceListAdapter(this, GlobalClass.BluetoothAndroid._listDevice);
                        lvResult.Adapter = adapter;
                        lvResult.ItemClick += lvResult_ItemClick;

                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        _alert = builder.Create();
                        _alert.SetMessage("Select your item");
                        _alert.SetView(lvResult);

                        _alert.DismissEvent += (s, e) =>
                        {
                            if (GlobalClass.BluetoothDevice == null)
                                PrepareSendDataOnline();
                        };

                        _alert.Show();
                    }
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "PreparePrinterDevice", ex.Message, Enums.LogType.Error);
                if (GlobalClass.BluetoothDevice == null)
                    PrepareSendDataOnline();
            }

        }

        async Task<MPosControllerDevices> OpenPrinterService(MposConnectionInformation connectionInfo)
        {
            if (connectionInfo == null)
                return null;

            if (_printer != null)
                return _printer;

            _printer = MPosDeviceFactory.Current.createDevice(MPosDeviceType.MPOS_DEVICE_PRINTER) as MPosControllerPrinter;

            switch (connectionInfo.IntefaceType)
            {
                case MPosInterfaceType.MPOS_INTERFACE_BLUETOOTH:
                case MPosInterfaceType.MPOS_INTERFACE_WIFI:
                case MPosInterfaceType.MPOS_INTERFACE_ETHERNET:
                    _printer.selectInterface((int)connectionInfo.IntefaceType, connectionInfo.Address);
                    _printer.selectCommandMode((int)(false ? MPosCommandMode.MPOS_COMMAND_MODE_DEFAULT : MPosCommandMode.MPOS_COMMAND_MODE_BYPASS));
                    break;
                default:
                    //await DisplayAlert("Connection Fail", "Not Supported Interface", "OK");
                    return null;
            }

            await _printSemaphore.WaitAsync();

            try
            {
                var result = await _printer.openService();
                if (result != (int)MPosResult.MPOS_SUCCESS)
                {
                    _printer = null;
                    //await DisplayAlert("Connection Fail", "openService failed. (" + result.ToString() + ")", "OK");
                }
            }
            finally
            {
                _printSemaphore.Release();
            }

            return _printer;
        }

        async void lvResult_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                string bx = GlobalClass.BluetoothAndroid._listDevice[e.Position].Name.ToString();
                _alert.Dismiss();
                if (bx == "SPP-R410") {

                    try
                    {

                        _connectionInfo = new MposConnectionInformation();

                        _connectionInfo.IntefaceType = MPosInterfaceType.MPOS_INTERFACE_BLUETOOTH;
                        _connectionInfo.Name = GlobalClass.BluetoothAndroid._listDevice[e.Position].Name.ToString();
                        _connectionInfo.MacAddress = GlobalClass.BluetoothAndroid._listDevice[e.Position].Address.ToString();

                        if (!GeneralAndroidClass.IsRegisterPrinter(_connectionInfo.MacAddress))
                        {
                            GeneralAndroidClass.RegisterPrinter(_connectionInfo.MacAddress);
                        }

                        // Prepares to communicate with the printer
                        _printer = await OpenPrinterService(_connectionInfo) as MPosControllerPrinter;

                        if (_printer == null)
                            return;

                        await _printSemaphore.WaitAsync();

                        await ShowMessageNew(true, Constants.Messages.GenerateBitmap);

                        var printImageBll = new PrintImageBll();
                        var bitmap = printImageBll.Akuan(this, _noRujukan);

                        await ShowMessageNew(true, Constants.Messages.ConnectionToBluetooth);

                        // note : Page mode and transaction mode cannot be used together between IN and OUT.
                        // When "setTransaction" function called with "MPOS_PRINTER_TRANSACTION_IN", print data are stored in the buffer.
                        await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_IN);

                        await _printer.directIO(new byte[] { 0x1b, 0x40 });

                        await _printer.printBitmap(bitmap, -2, 1, Constants.Brightness, true, true);

                        // Feed to tear-off position (Manual Cutter Position)
                        await _printer.directIO(new byte[] { 0x1b, 0x4a, 0xaf });
                    }
                    catch (Exception ex)
                    {
                        //DisplayAlert("Exception", ex.Message, "OK");
                        GeneralAndroidClass.LogData(LayoutName, "Printer error : ", ex.Message, Enums.LogType.Error);
                    }
                    finally
                    {
                        // Printer starts printing by calling "setTransaction" function with "MPOS_PRINTER_TRANSACTION_OUT"
                        await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_OUT);
                        // If there's nothing to do with the printer, call "closeService" method to disconnect the communication between Host and Printer.
                        _printSemaphore.Release();

                        await ShowMessageNew(true, Constants.Messages.SuccessPrint);
                        Thread.Sleep(Constants.DefaultWaitingMilisecond);
                        await ShowMessageNew(false, "");
                    }

                }
                else {

                    if (e.Position > GlobalClass.BluetoothAndroid._listDevice.Count)
                        return;

                    _alert.Dismiss();
                    GlobalClass.BluetoothDevice = GlobalClass.BluetoothAndroid._listDevice[e.Position];
                    Print(false);

                }
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

            GetFWCode();

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

        public const int MESSAGE_DEVICE_NAME = 1;
        public const int MESSAGE_TOAST = 2;
        public const int MESSAGE_READ = 3;
        public const string DEVICE_NAME = "device_name";

        class ServicetHandler : Handler
        {
            readonly Akuan activity;

            public ServicetHandler(Akuan activity)
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
            try
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
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "OnPrinting", ex.Message, Enums.LogType.Error);
            }
            finally
            {
#if !DEBUG
                PrepareSendDataOnline();
#endif
            }

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

        private void PrepareSendDataOnline()
        {

            //return;
            _dialog?.Dismiss();
            if (_reprint) return;
            if (!GeneralAndroidClass.IsOnline())
                return;

            _dialog = GeneralAndroidClass.ShowProgressDialog(this, Constants.Messages.SendDataOnline);
            new Thread(() =>
            {
                Thread.Sleep(1000);
                this.RunOnUiThread(SendDataOnline);
            }).Start();
        }

        private void SendDataOnline()
        {
            var result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(_noRujukan, Enums.TableType.Akuan_UpdateKompaun, this)).Result;
            if (result.Success)
            {
                GeneralAndroidClass.ShowToast(Constants.Messages.SuccessSendData);
                _dialog?.Dismiss();
                OnModalDialog(Constants.Messages.SuccessSave);
            }
            else
            {
                _dialog?.Dismiss();

                var alertDialog = GeneralAndroidClass.GetDialogCustom(this);
                alertDialog.SetTitle(Constants.Messages.SendData);
                alertDialog.SetMessage(Constants.Messages.ReSendData);
                alertDialog.SetCanceledOnTouchOutside(false);
                alertDialog.SetButton2(Constants.Messages.Yes, (c, ev) =>
                {
                    alertDialog.Dismiss();
                    _dialog = GeneralAndroidClass.ShowProgressDialog(this, Constants.Messages.SendDataOnline);
                    new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        this.RunOnUiThread(SendDataOnline);
                    }).Start();

                });
                alertDialog.SetButton(Constants.Messages.No, (c, ev) =>
                {
                    alertDialog.Dismiss();
                    OnModalDialog(Constants.Messages.SuccessSave);
                });

                alertDialog.Show();

            }
        }

        private void SetEnableControl(bool blValue)
        {
            txtNamaPenerima.Enabled = blValue;
            spJenisKad.Enabled = blValue;
            txtNoKpPenerima.Enabled = blValue;
            txtNoTelefonPenerima.Enabled = blValue;
            txtEmailPenerima.Enabled = blValue;
            spNegeriPenerima.Enabled = blValue;
            txtBandarPenerima.Enabled = blValue;
            txtPoskodPenerima.Enabled = blValue;
            chkGambarBayaran.Enabled = blValue;
            txtAlamatPenerima1.Enabled = blValue;
            txtAlamatPenerima2.Enabled = blValue;
            txtAlamatPenerima3.Enabled = blValue;

            btnNamaPenerima.Enabled = blValue;
            btnCamera.Enabled = blValue;

            txtNoResit.Enabled = blValue;
            txtAmounBayaran.Enabled = blValue;

            if (blValue)
            {
                txtNamaPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                spJenisKad.SetBackgroundResource(Resource.Drawable.spiner_bg);
                txtNoTelefonPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtEmailPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                spNegeriPenerima.SetBackgroundResource(Resource.Drawable.spiner_bg);
                txtBandarPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtPoskodPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoKpPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima1.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima2.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima3.SetBackgroundResource(Resource.Drawable.editText_bg);

                txtNoResit.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAmounBayaran.SetBackgroundResource(Resource.Drawable.editText_bg);
                chkGambarBayaran.SetBackgroundResource(@Resource.Drawable.editText_bg);
            }
            else
            {
                txtNamaPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                spJenisKad.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoTelefonPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtEmailPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                spNegeriPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtBandarPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtPoskodPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoKpPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima1.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima2.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima3.SetBackgroundResource(Resource.Drawable.textView_bg);

                txtNoResit.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAmounBayaran.SetBackgroundResource(Resource.Drawable.textView_bg);
                chkGambarBayaran.SetBackgroundResource(Resource.Drawable.textView_bg);
            }
        }

        private void btnSearchJpnPenerima_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtNoKpPenerima.Text))
                {
                    _hourGlass?.StartMessage(this, OnSearchJpnPenerimaDetail);
                }
                //OnShowCarianJpnDetail();

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnSearchJpn_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void OnSearchJpnPenerimaDetail()
        {
            var listJpnDetail = PemeriksaanBll.GetListJpnDetail(txtNoKpPenerima.Text, SharedPreferences.GetString(SharedPreferencesKeys.UserNoKp));

            txtNamaPenerima.Text = string.Empty;
            txtAlamatPenerima1.Text = string.Empty;
            txtAlamatPenerima2.Text = string.Empty;
            txtAlamatPenerima3.Text = string.Empty;

            if (listJpnDetail.Success)
            {
                if (listJpnDetail.Result.status == "200")
                {
                    txtNamaPenerima.Text = listJpnDetail.Result.name;
                    txtAlamatPenerima1.Text = listJpnDetail.Result.address1;
                    txtAlamatPenerima2.Text = listJpnDetail.Result.address2;
                    txtAlamatPenerima3.Text =
                        $"{listJpnDetail.Result.postcode} {listJpnDetail.Result.city} {listJpnDetail.Result.state}";

                    if (!string.IsNullOrEmpty(listJpnDetail.Result.address3))
                    {
                        txtAlamatPenerima3.Text =
                            $"{listJpnDetail.Result.address3} {txtAlamatPenerima3.Text}";
                    }

                    if (!string.IsNullOrEmpty(listJpnDetail.Result.messageCode))
                    {
                        GeneralAndroidClass.ShowModalMessage(this, listJpnDetail.Result.messageCode);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(listJpnDetail.Result.messageCode))
                        GeneralAndroidClass.ShowModalMessage(this, listJpnDetail.Result.messageCode);
                    else if (string.IsNullOrEmpty(listJpnDetail.Mesage))
                        GeneralAndroidClass.ShowModalMessage(this, listJpnDetail.Mesage);
                    else
                        GeneralAndroidClass.ShowModalMessage(this, string.Format(Constants.ErrorMessages.NoDataFoundJpnDetail, txtNoKpPenerima.Text));
                }
            }
            else
            {
                GeneralAndroidClass.ShowModalMessage(this, listJpnDetail.Mesage);
            }

            _hourGlass?.StopMessage();

        }
    }
}