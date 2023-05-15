using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using Plugin.BxlMpXamarinSDK;
using Plugin.BxlMpXamarinSDK.Abstractions;

namespace IEMSApps.Activities
{
    [Activity(Label = "Kompaun", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Kompaun : BaseActivity
    {

        private const string LayoutName = "Kompaun";

        LinearLayout tabButiran, tabPesalah, tabPenerima;
        TextView lblTabButiran, lblTabPesalah, lblTabPenerima;
        View viewButiran, viewPesalah, viewPenerima;

        private TextView lblNoKpp, txtNoLaporPolis, txtNoLaporCawangan, txtTempat, txtNoEP, txtNoIP;
        private TextView txtButirKesalahan, txtAmaunTawaran, txtArahanSemasa, txtTempohTawaran;
        private RadioButton rdIndividu, rdSyarikat;
        private Spinner spAkta;
        private CheckBox chkArahan;

        private EditText txtKesalahan;
        //private Button btnKesalahan;


        private TextView txtNama, txtNoKp, txtNamaSyarikat, txtNoDaftarSyarikat, txtBarangKompaun;
        private TextView txtAlamatPesalah1, txtAlamatPesalah2, txtAlamatPesalah3;
        private ImageView btnImageNama;

        private TextView txtNamaPenerima, txtNoKpPenerima;
        private TextView txtAlamatPenerima1, txtAlamatPenerima2, txtAlamatPenerima3;
        private ImageView btnImageNamaPenerima;
        private CheckBox chkAmaran;

        //new add ipayment
        private Spinner spJenisKad,spNegeriPenerima;
        private EditText txtNoTelefonPenerima, txtEmailPenerima, txtBandarPenerima, txtPoskodPenerima;
        private Button btnBandarPenerima, btnPoskodPenerima;

        private Button btnOk, btnCamera, btnPrint, btnAkuan, btnTarikh, btnMasa, btnSearchJpn, btnSearchJpnPenerima;
        private TextView lblBtnOk, lblBtnAkuan;

        private AlertDialog _dialog;
        private bool _isSaved = false;

        private string _jenisKompaun;
        private string _noRujukanKpp;
        private string _noKpPenerimaKpp;
        ServicetHandler handler;

        private Enums.ActiveForm _activeForm = Enums.ActiveForm.Kompaun;
        private bool _reprint;
        private HourGlassClass _hourGlass = new HourGlassClass();

        private MPosControllerPrinter _printer;
        MposConnectionInformation _connectionInfo;
        private static SemaphoreSlim _printSemaphore = new SemaphoreSlim(1, 1);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Kompaun);

            _printer = null;
            _connectionInfo = null;

            //SetInit();
            _hourGlass?.StartMessage(this, SetInit);

        }

        private void SetInit()
        {
            try
            {
                _reprint = false;
                var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
                txtHhId.Text = GeneralBll.GetUserHandheld();

                _jenisKompaun = Intent.GetStringExtra("JenisKmp") ?? "";
                _noRujukanKpp = Intent.GetStringExtra("NoRujukanKpp") ?? "";
                _noKpPenerimaKpp = Intent.GetStringExtra("NoKpPenerima") ?? "";

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

                LoadData();

                LoadDataDropdown();

                SetButtonTindakan();

                SetPrintButton();

                var kompaun = KompaunBll.GetKompaunByRujukanKpp(_noRujukanKpp);
                if (kompaun != null)
                {
                    _isSaved = true;

                    LoadDataExisting(kompaun);

                    SetEnableControl(false);

                    btnPrint.SetBackgroundResource(Resource.Drawable.print_icon);
                    btnPrint.Enabled = true;

                    if (chkAmaran.Checked)
                    {
                        btnAkuan.SetBackgroundResource(Resource.Drawable.catatan_icon);
                        btnAkuan.Enabled = true;
                    }

                    lblNoKpp.Text = kompaun.NoKmp;
                }
                else
                {
                    lblNoKpp.Text = GeneralBll.GenerateNoRujukan(Enums.PrefixType.KOTS);
                    LoadKppKesalahan();
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }
            _hourGlass?.StopMessage();
        }

        private void LoadKppKesalahan()
        {
            var kpp = PemeriksaanBll.GetPemeriksaanByRujukan(_noRujukanKpp);
            if (kpp != null)
            {
                rdIndividu.Checked = true;
                if (kpp.JenisPesalah == Constants.JenisPesalah.Syarikat) rdSyarikat.Checked = true;

                var positionAkta = PasukanBll.GetPositionSelected(ListAkta, kpp.KodAkta);
                spAkta.SetSelection(positionAkta);

                var tbKesalahan = MasterDataBll.GetKesalahan(kpp.KodSalah, kpp.KodAkta);
                if (tbKesalahan != null)
                {
                    txtKesalahan.Text = tbKesalahan.Seksyen;
                }

                _kodSalah = kpp.KodSalah;

                txtButirKesalahan.Text = kpp.ButirSalah;
                txtAmaunTawaran.Text = kpp.AmnKmp.ToString(Constants.DecimalFormatZero);
                chkArahan.Checked = kpp.IsArahanSemasa == Constants.ArahanSemasa.Yes;
                if (kpp.TempohTawaran > 0) txtTempohTawaran.Text = kpp.TempohTawaran.ToString();

                rdIndividu.Enabled = false;
                rdSyarikat.Enabled = false;

                spAkta.Enabled = false;
                //btnKesalahan.Enabled = false;
                txtButirKesalahan.Enabled = false;
                txtAmaunTawaran.Enabled = false;
                chkArahan.Enabled = false;
                txtTempohTawaran.Enabled = false;

                //new add
                var positionJenisKad = PasukanBll.GetPositionSelected(ListJenisKad, kpp.ip_identiti_pelanggan_id.ToString());
                spJenisKad.SetSelection(positionJenisKad);
                txtNoTelefonPenerima.Text = kpp.notelpenerima;
                txtEmailPenerima.Text = kpp.emelpenerima;
                var positionNegeriPenerima = PasukanBll.GetPositionSelected(ListNegeri, kpp.negeripenerima);
                spNegeriPenerima.SetSelection(positionNegeriPenerima);
                txtBandarPenerima.Text = kpp.bandarpenerima;
                txtPoskodPenerima.Text = kpp.poskodpenerima;

                SetPrintButton();
            }
        }
        private void LoadData()
        {
            var allowFilter = new FilterChar();
            var allowFilterWithoutSingleQuote = new FilterCharWithoutSingleQuote();

            #region Butiran
            lblNoKpp = FindViewById<TextView>(Resource.Id.lblNoKpp);
            txtNoLaporPolis = FindViewById<TextView>(Resource.Id.txtNoLaporPolis);
            txtNoLaporCawangan = FindViewById<TextView>(Resource.Id.txtNoLaporCawangan);
            btnTarikh = FindViewById<Button>(Resource.Id.btnTarikh);
            btnMasa = FindViewById<Button>(Resource.Id.btnMasa);
            txtTempat = FindViewById<EditText>(Resource.Id.txtTempat);

            txtNoEP = FindViewById<TextView>(Resource.Id.txtNoEP);
            txtNoIP = FindViewById<TextView>(Resource.Id.txtNoIP);
            txtNoEP.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new FilterNoIPAndNoEPChar(), new InputFilterLengthFilter(30) });
            txtNoIP.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new FilterNoIPAndNoEPChar(), new InputFilterLengthFilter(30) });

            rdIndividu = FindViewById<RadioButton>(Resource.Id.rdIndividu);
            rdSyarikat = FindViewById<RadioButton>(Resource.Id.rdSyarikat);

            spAkta = FindViewById<Spinner>(Resource.Id.spAkta);

            txtButirKesalahan = FindViewById<EditText>(Resource.Id.txtButirKesalahan);
            txtAmaunTawaran = FindViewById<EditText>(Resource.Id.txtAmaunTawaran);
            chkArahan = FindViewById<CheckBox>(Resource.Id.chkArahan);
            txtTempohTawaran = FindViewById<EditText>(Resource.Id.txtTempohTawaran);

            txtKesalahan = FindViewById<EditText>(Resource.Id.txtKesalahan);
            //btnKesalahan = FindViewById<Button>(Resource.Id.btnKesalahan);

            //btnKesalahan.Click += BtnKesalahan_Click;

            btnTarikh.Text = GeneralBll.GetLocalDateTime().ToString(Constants.DateFormatDisplay);
            btnMasa.Text = GeneralBll.GetLocalDateTime().ToString(Constants.TimeFormatDisplay);

            txtButirKesalahan.Enabled = false;
            txtButirKesalahan.SetBackgroundResource(Resource.Drawable.textView_bg);


            rdIndividu.Checked = true;
            txtAmaunTawaran.Enabled = false;
            txtTempohTawaran.Enabled = false;
            //txtTempohTawaran.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(2) });

            //chkArahan.CheckedChange += ChkArahan_CheckedChange;
            //rdIndividu.CheckedChange += RdIndividu_CheckedChange;
            //txtAmaunTawaran.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(10) });

            txtTempat.TextChanged += Event_CheckMandatory_Edittext;


            txtNoLaporPolis.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowFilterWithoutSingleQuote });
            txtNoLaporCawangan.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowFilterWithoutSingleQuote });
            txtTempat.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(300), allowFilter });            

            #endregion

            #region Pesalah

            btnImageNama = FindViewById<ImageView>(Resource.Id.btnNama);
            txtNama = FindViewById<EditText>(Resource.Id.txtNama);
            txtNoKp = FindViewById<EditText>(Resource.Id.txtNoKp);
            txtNamaSyarikat = FindViewById<EditText>(Resource.Id.txtNamaSyarikat);
            txtNoDaftarSyarikat = FindViewById<EditText>(Resource.Id.txtNoDaftarSyarikat);
            txtAlamatPesalah1 = FindViewById<EditText>(Resource.Id.txtAlamatPesalah1);
            txtAlamatPesalah2 = FindViewById<EditText>(Resource.Id.txtAlamatPesalah2);
            txtAlamatPesalah3 = FindViewById<EditText>(Resource.Id.txtAlamatPesalah3);

            txtBarangKompaun = FindViewById<EditText>(Resource.Id.txtBarangKompaun);


            txtAlamatPesalah1.TextChanged += Event_CheckMandatory_Edittext;
            txtBarangKompaun.TextChanged += Event_CheckMandatory_Edittext;

            btnImageNama.Click += BtnImageNama_Click;

            txtNama.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
            txtNoKp.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowFilterWithoutSingleQuote });
            txtNamaSyarikat.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(150), allowFilter });

            txtBarangKompaun.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(300), allowFilter });


            txtNoDaftarSyarikat.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(15), allowFilterWithoutSingleQuote });
            txtAlamatPesalah1.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(300), allowFilter });
            txtAlamatPesalah2.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
            txtAlamatPesalah3.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
            btnSearchJpn = FindViewById<Button>(Resource.Id.btnSearchJpn);
            btnSearchJpn.Click += BtnSearchJpn_Click;
            #endregion

            #region Penerima

            btnImageNamaPenerima = FindViewById<ImageView>(Resource.Id.btnNamaPenerima);
            txtNamaPenerima = FindViewById<EditText>(Resource.Id.txtNamaPenerima);
            txtNoKpPenerima = FindViewById<EditText>(Resource.Id.txtNoKpPenerima);

            txtAlamatPenerima1 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima1);
            txtAlamatPenerima2 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima2);
            txtAlamatPenerima3 = FindViewById<EditText>(Resource.Id.txtAlamatPenerima3);

            chkAmaran = FindViewById<CheckBox>(Resource.Id.chkAmaran);

            txtNamaPenerima.TextChanged += Event_CheckMandatory_Edittext;
            txtAlamatPenerima1.TextChanged += Event_CheckMandatory_Edittext;

            btnImageNamaPenerima.Click += BtnImageNamaPenerima_Click;

            txtNamaPenerima.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
            txtNoKpPenerima.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowFilterWithoutSingleQuote });
            txtAlamatPenerima1.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(Constants.AllowAddressCharacter), allowFilter });
            txtAlamatPenerima2.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(Constants.AllowAddressCharacter), allowFilter });
            txtAlamatPenerima3.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(Constants.AllowAddressCharacter), allowFilter });
            btnSearchJpnPenerima = FindViewById<Button>(Resource.Id.btnSearchJpnPenerima);
            btnSearchJpnPenerima.Click += btnSearchJpnPenerima_Click;

            txtNoKpPenerima.Text = _noKpPenerimaKpp;

            //new add
            spJenisKad = FindViewById<Spinner>(Resource.Id.spJenisKad);
            txtNoTelefonPenerima = FindViewById<EditText>(Resource.Id.txtNoTelefonPenerima);
            txtEmailPenerima = FindViewById<EditText>(Resource.Id.txtEmailPenerima);
            spNegeriPenerima = FindViewById<Spinner>(Resource.Id.spNegeriPenerima);
            txtBandarPenerima = FindViewById<EditText>(Resource.Id.txtBandarPenerima);
            btnBandarPenerima = FindViewById<Button>(Resource.Id.btnBandarPenerima);
            btnBandarPenerima.Click += btnBandarPenerima_Click;

            btnPoskodPenerima = FindViewById<Button>(Resource.Id.btnPoskodPenerima);
            btnPoskodPenerima.Click += btnPoskodPenerima_Click; 

            txtPoskodPenerima = FindViewById<EditText>(Resource.Id.txtPoskodPenerima);
            txtPoskodPenerima.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(5), allowFilter });

            #endregion

            #region Button

            btnOk = FindViewById<Button>(Resource.Id.btnOk);
            btnOk.SetBackgroundResource(Resource.Drawable.backblue);

            lblBtnOk = FindViewById<TextView>(Resource.Id.lblBtnOk);
            lblBtnOk.Text = "Kembali";
            btnOk.Click += BtnOk_Click1;
            //btnOk.Visibility = ViewStates.Gone;
            //lblBtnOk.Visibility = ViewStates.Gone;

            btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            btnAkuan = FindViewById<Button>(Resource.Id.btnNote);
            lblBtnAkuan = FindViewById<TextView>(Resource.Id.lblBtnNote);
            lblBtnAkuan.Text = "Akuan";

            //btnOk.Click += BtnOk_Click;
            btnCamera.Click += BtnCamera_Click;
            btnPrint.Click += BtnPrint_Click;
            btnAkuan.Click += BtnAkuan_Click;

            btnTarikh.Click += BtnTarikh_Click;
            btnMasa.Click += BtnMasa_Click;
            #endregion

            //var linearFooterPemeriksaan = FindViewById<LinearLayout>(Resource.Id.linearFooterPemeriksaan);
            //linearFooterPemeriksaan.WeightSum = 3;
            //var linearOk = FindViewById<LinearLayout>(Resource.Id.linearOk);
            //linearOk.Visibility = ViewStates.Gone;

            var pemeriksaan = PemeriksaanBll.GetPemeriksaanByRujukan(_noRujukanKpp);
            if (pemeriksaan != null)
            {
                txtTempat.Text = pemeriksaan.LokasiLawatan;
                txtNama.Text = pemeriksaan.NamaPremis;
                txtAlamatPesalah1.Text = pemeriksaan.AlamatPremis1;
                txtAlamatPesalah2.Text = pemeriksaan.AlamatPremis2;
                txtAlamatPesalah3.Text = pemeriksaan.AlamatPremis3;
                txtNoDaftarSyarikat.Text = pemeriksaan.NoDaftarPremis;
                txtNamaPenerima.Text = pemeriksaan.NamaPenerima;
                txtAlamatPenerima1.Text = pemeriksaan.AlamatPenerima1;
                txtAlamatPenerima2.Text = pemeriksaan.AlamatPenerima2;
                txtAlamatPenerima3.Text = pemeriksaan.AlamatPenerima3;

                txtNamaSyarikat.Text = pemeriksaan.NamaPremis;


                //txtTempohTawaran.Text = "1";

                //var katPremis = MasterDataBll.GetKatPremisName(pemeriksaan.KodKatPremis);
                //if (!string.IsNullOrEmpty(katPremis) && katPremis.Contains("TETAP"))
                //{
                //    txtTempohTawaran.Text = "14";
                //}

                if (pemeriksaan.SetujuByr == Constants.SetujuBayar.Yes)
                {
                    chkAmaran.Checked = true;
                }
            }



        }

        private void btnPoskodPenerima_Click(object sender, EventArgs e)
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

        private void btnBandarPenerima_Click(object sender, EventArgs e)
        {
            try
            {
                ShowBandarPenerima();
            }
            catch (Exception ex)
            {

                GeneralAndroidClass.LogData(LayoutName, "btnBandarPenerima", ex.Message, Enums.LogType.Error);
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

                //var bandarPenerima = MasterDataBll.GetBandarPenerimaByPoskod(txtPoskodPenerima.Text);
                //txtBandarPenerima.Text = bandarPenerima;

                //var IdNegeri = MasterDataBll.GetNegeriPenerimaByBandar(txtBandarPenerima.Text);
                //spNegeriPenerima.SetSelection(IdNegeri);

                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();

        }

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

                //txtAlamatPenerima3.Text = $"{bandarName} {negeriName}";

                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private void LoadDataExisting(TbKompaun kompaun)
        {
            LoadDataButiran(kompaun);
            LoadDataPesalah(kompaun);
            LoadDataPenerima(kompaun);
        }

        private void LoadDataButiran(TbKompaun data)
        {
            txtNoLaporPolis.Text = data.NoLaporanPolis;

            txtNoLaporCawangan.Text = data.NoLaporanCwgn;

            txtNoEP.Text = data.NoEp;
            txtNoIP.Text = data.NoIp;

            var dtData = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhKmp);

            btnTarikh.Text = dtData.ToString(Constants.DateFormatDisplay);
            btnMasa.Text = dtData.ToString(Constants.TimeFormatDisplay);

            txtTempat.Text = data.TempatSalah;

            rdIndividu.Checked = true;
            if (data.JenisPesalah == Constants.JenisPesalah.Syarikat)
            {
                rdSyarikat.Checked = true;
            }

            rdIndividu.Enabled = false;
            rdSyarikat.Enabled = false;

            //txtAkta.Text = MasterDataBll.GetAktaName(data.KodAkta);
            spAkta.SetSelection(GeneralBll.GetPositionSelected(ListAkta, data.KodAkta));

            txtKesalahan.Text = MasterDataBll.GetKesalahanName(data.KodSalah, data.KodAkta);
            _kodSalah = data.KodSalah;

            txtButirKesalahan.Text = data.ButirSalah;

            txtAmaunTawaran.Text = data.AmnByr.ToString(Constants.DecimalFormat);

            if (data.IsArahanSemasa == Constants.ArahanSemasa.Yes)
            {
                chkArahan.Checked = true;
            }
            chkArahan.Enabled = false;

            txtTempohTawaran.Text = data.TempohTawaran.ToString();

        }

        private void LoadDataPesalah(TbKompaun data)
        {
            txtNama.Text = data.NamaOkk;
            ;
            txtNoKp.Text = data.NoKpOkk;

            txtNamaSyarikat.Text = data.NamaPremis;

            txtNoDaftarSyarikat.Text = data.NoDaftarPremis;

            txtAlamatPesalah1.Text = data.AlamatOkk1;
            txtAlamatPesalah2.Text = data.AlamatOkk2;
            txtAlamatPesalah3.Text = data.AlamatOkk3;

            txtBarangKompaun.Text = data.BarangKompaun;

        }

        private void LoadDataPenerima(TbKompaun data)
        {

            txtNamaPenerima.Text = data.NamaPenerima;
            txtNoKpPenerima.Text = data.NoKpPenerima;
            txtAlamatPenerima1.Text = data.AlamatPenerima1;
            txtAlamatPenerima2.Text = data.AlamatPenerima2;
            txtAlamatPenerima3.Text = data.AlamatPenerima3;
            if (data.IsCetakAkuan == Constants.CetakAkuan.Yes)
            {
                chkAmaran.Checked = true;
            }
            chkAmaran.Enabled = false;
        }

        private void BtnOk_Click1(object sender, EventArgs e)
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

        private void BtnKesalahan_Click(object sender, EventArgs e)
        {
            try
            {
                ShowKesalahan();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnKesalahan_Click", ex.Message, Enums.LogType.Error);
            }
        }

        #region Card Reader

        private void BtnImageNamaPenerima_Click(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(Intent.ActionMain);
                intent.SetComponent(new ComponentName("com.aimforce.mykad.woosim", "com.aimforce.mykad.woosim.MainActivity"));
                intent.PutExtra("command", "no-ui");
                intent.PutExtra("command2", "no-photo");
                StartActivityForResult(intent, REQUEST_MYKAD2);
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnImageNamaPenerima_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private static int REQUEST_MYKAD = 1001;
        private static int REQUEST_MYKAD2 = 1002;

        private void BtnImageNama_Click(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(Intent.ActionMain);
                intent.SetComponent(new ComponentName("com.aimforce.mykad.woosim", "com.aimforce.mykad.woosim.MainActivity"));
                intent.PutExtra("result", "no-ui");
                intent.PutExtra("command2", "no-photo");
                StartActivityForResult(intent, REQUEST_MYKAD);
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnImageNama_Click", ex.Message, Enums.LogType.Error);
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
                            SetMyCardPesalah(card);
                        else
                            GeneralAndroidClass.ShowToast(card.Message);

                    }
                }
                else if (requestCode == REQUEST_MYKAD2)
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

        private void ClearMyCardPesalahField()
        {
            txtNama.Text = "";
            txtNoKp.Text = "";
            txtNamaPenerima.Text = "";
            txtNoKpPenerima.Text = "";
            txtAlamatPesalah1.Text = "";
            txtAlamatPesalah2.Text = "";
            txtAlamatPesalah3.Text = "";

            txtAlamatPenerima1.Text = "";
            txtAlamatPenerima2.Text = "";
            txtAlamatPenerima3.Text = "";
        }

        private void SetMyCardPesalah(CardInfoDto cardDto)
        {
            ClearMyCardPesalahField();

            var listAddress = new List<string>();

            txtNama.Text = cardDto.originalName;
            txtNoKp.Text = cardDto.idNum;
            txtNamaPenerima.Text = cardDto.originalName;
            txtNoKpPenerima.Text = cardDto.idNum;

            var address = $"{cardDto.address1} {cardDto.address2}";
            var addressPostCodeCity = $"{cardDto.postcode} {cardDto.city} {cardDto.state}";


            if (string.IsNullOrEmpty(cardDto.address3))
            {
                listAddress = GeneralBll.SeparateText(address, 2, Constants.MaxLengthAddress);
                txtAlamatPesalah1.Text = listAddress[0];
                txtAlamatPenerima1.Text = listAddress[0];

                if (string.IsNullOrEmpty(listAddress[1]))
                {
                    txtAlamatPesalah2.Text = addressPostCodeCity;
                    txtAlamatPenerima2.Text = addressPostCodeCity;
                }
                else
                {
                    txtAlamatPesalah2.Text = listAddress[1];
                    txtAlamatPesalah3.Text = addressPostCodeCity;

                    txtAlamatPenerima2.Text = listAddress[1];
                    txtAlamatPenerima3.Text = addressPostCodeCity;
                }
            }
            else
            {
                if (address.Length <= 80)
                {
                    txtAlamatPesalah1.Text = address;
                    txtAlamatPesalah2.Text = cardDto.address3;
                    txtAlamatPesalah3.Text = addressPostCodeCity;

                    txtAlamatPenerima1.Text = address;
                    txtAlamatPenerima2.Text = cardDto.address3;
                    txtAlamatPenerima3.Text = addressPostCodeCity;
                }
                else
                {
                    address = string.Format("{0} {1} {2}", cardDto.address1, cardDto.address2, cardDto.address3);

                    var listString = GeneralBll.SeparateText(address, 2, Constants.MaxLengthAddress);
                    txtAlamatPesalah1.Text = listString[0].Trim();
                    txtAlamatPesalah2.Text = listString[1].Trim();
                    txtAlamatPesalah3.Text = addressPostCodeCity;

                    txtAlamatPenerima1.Text = listString[0].Trim();
                    txtAlamatPenerima2.Text = listString[1].Trim();
                    txtAlamatPenerima3.Text = addressPostCodeCity;
                }

            }
        }

        private void ClearMyCardField()
        {
            txtNamaPenerima.Text = "";
            txtNoKpPenerima.Text = "";
            txtAlamatPenerima1.Text = "";
            txtAlamatPenerima2.Text = "";
            txtAlamatPenerima3.Text = "";
        }


        private void SetMyCard(CardInfoDto cardDto)
        {
            ClearMyCardField();

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

        #endregion

        //private void BtnButirKesalahan_Click(object sender, EventArgs e)
        //{
        //    var dialogView = LayoutInflater.Inflate(Resource.Layout.ButiranList, null);
        //    AlertDialog alertDialog;
        //    using (var dialog = new AlertDialog.Builder(this))
        //    {
        //        dialog.SetView(dialogView);
        //        dialog.SetNegativeButton("Batal", (s, a) => { });
        //        alertDialog = dialog.Create();
        //    }

        //    var selectedAkta = GeneralBll.GetKeySelected(ListAkta, spAkta.SelectedItem?.ToString() ?? "");
        //    var listButir =
        //        MasterDataBll.GetKesalahanButir(selectedAkta, _kodSalah);

        //    var adapter = new ButiranKesalahanAdapter(this, listButir);
        //    var listView = dialogView.FindViewById<ListView>(Resource.Id.listView1);
        //    listView.Adapter = adapter;
        //    listView.ItemClick += (s, a) =>
        //    {
        //        alertDialog.Dismiss();
        //        txtButirKesalahan.Text = listButir[a.Position];
        //    };

        //    alertDialog.Show();
        //}

        private void RdIndividu_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                SetAmountTawaran();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "RdIndividu_CheckedChange", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnMasa_Click(object sender, EventArgs e)
        {
            try
            {
                TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
                {
                    btnMasa.Text = time.ToString(Constants.TimeFormatDisplay);
                });

                frag.Show(FragmentManager, TimePickerFragment.TAG);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnMasa_Click", ex.Message, Enums.LogType.Error);
            }

        }

        private void BtnTarikh_Click(object sender, EventArgs e)
        {
            try
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    btnTarikh.Text = time.ToString(Constants.DateFormatDisplay);
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnTarikh_Click", ex.Message, Enums.LogType.Error);
            }


        }

        #region Button event

        private void BtnAkuan_Click(object sender, EventArgs e)
        {
            try
            {
                _activeForm = Enums.ActiveForm.Akuan;
                var intent = new Intent(this, typeof(Akuan));
                intent.PutExtra("NoRujukan", lblNoKpp.Text);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnAkuan_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
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
                        ShowConfirmModal();
                    }

                }

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
                if (string.IsNullOrEmpty(lblNoKpp.Text))
                {
                    GeneralAndroidClass.ShowModalMessage(this, "No Rujukan Kosong");
                    return;
                }
                _activeForm = Enums.ActiveForm.Camera;
                var intent = new Intent(this, typeof(IEMSApps.Activities.Camera));
                intent.PutExtra("filename", lblNoKpp.Text);
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

        #endregion

        private void ChkArahan_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                txtTempohTawaran.Text = "";
                if (chkArahan.Checked)
                {
                    txtTempohTawaran.Enabled = true;
                    txtTempohTawaran.SetBackgroundResource(Resource.Drawable.editText_bg);
                }
                else
                {
                    txtTempohTawaran.Enabled = false;
                    txtTempohTawaran.SetBackgroundResource(Resource.Drawable.textView_bg);
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnOk_Click", ex.Message, Enums.LogType.Error);
            }
        }


        private Dictionary<string, string> ListAkta, ListJenisKad, ListNegeri;

        private void LoadDataDropdown()
        {
            ListAkta = KompaunBll.GetAllAkta();
            spAkta.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item,
                ListAkta.Select(c => c.Value).ToList());

            //spAkta.ItemSelected += SpAkta_ItemSelected;

            ListJenisKad = MasterDataBll.GetJenisKad();
            spJenisKad.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, ListJenisKad.Select(c => c.Value).ToList());

            //ListNegeri = MasterDataBll.GetAllNegeri();
            ListNegeri = MasterDataBll.GetAllNegeriNew();
            spNegeriPenerima.Adapter = new ArrayAdapter<string>(this,
                Resource.Layout.support_simple_spinner_dropdown_item, ListNegeri.Select(c => c.Value).ToList());
            //spNegeriPenerima.ItemSelected += SpNegeriPenerima_ItemSelected;
        }

        //private void SpNegeriPenerima_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        //{
        //    try
        //    {
        //        if () { }
        //        txtBandarPenerima.Text = "";
        //        txtPoskodPenerima.Text = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        GeneralAndroidClass.LogData("Kompaun", "SpNegeriPenerima_ItemSelected", ex.Message, Enums.LogType.Error);
        //    }
        //}

        private void SpKesalahan_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                SetAmountTawaran();
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SpAkta_ItemSelected", ex.Message, Enums.LogType.Error);
            }
        }

        private void SpAkta_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                // var selectedAkta = GeneralBll.GetKeySelected(ListAkta, spAkta.SelectedItem?.ToString() ?? "");

                if (!_isSaved)
                {
                    txtKesalahan.Text = "";
                    _kodSalah = 0;
                    txtButirKesalahan.Text = "";
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SpAkta_ItemSelected", ex.Message, Enums.LogType.Error);
            }
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

        private bool ValidateData()
        {
#if DEBUG
            return true;
#endif

            if (string.IsNullOrEmpty(btnTarikh.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Tarikh kosong.");
                return false;
            }
            if (string.IsNullOrEmpty(btnMasa.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Masa kosong.");
                return false;
            }
            if (string.IsNullOrEmpty(txtTempat.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Tempat kosong.");
                return false;
            }
            if (spAkta.SelectedItem == null)
            {
                GeneralAndroidClass.ShowModalMessage(this, "Akta kosong.");
                return false;
            }

            if (_kodSalah == 0)
            {
                GeneralAndroidClass.ShowModalMessage(this, "Kesalahan kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(txtNama.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Nama pesalah kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(txtAlamatPesalah1.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Alamat pesalah kosong.");
                return false;
            }

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

            //var dtTamat = GeneralBll.ConvertDatabaseFormatStringToDateTime(
            //    GeneralBll.ConvertDateDisplayToDatabase(btnTarikh.Text) + " " +
            //    GeneralBll.ConvertTimeDisplayToDatabase(btnMasa.Text));
            //var dtCurrent = GeneralBll.GetLocalDateTime();

            //if (dtTamat <= dtCurrent)
            //{
            //    GeneralAndroidClass.ShowModalMessage(this, "Tarikh tamat lawatan sebelum tidak dibenarkan.");
            //    return false;
            //}


            return true;
        }

        private void SaveData()
        {
            var data = new TbKompaun
            {
                NoKmp = lblNoKpp.Text,
                IdHh = GeneralBll.GetUserHandheld(),
                JenisKmp = _jenisKompaun,
                NoRujukanKpp = _noRujukanKpp,
                KodCawangan = GeneralBll.GetUserCawangan(),
                JenisPesalah = Constants.JenisPesalah.Individu
            };


            if (rdSyarikat.Checked)
                data.JenisPesalah = Constants.JenisPesalah.Syarikat;

            data.TrkhKmp = GeneralBll.ConvertDateDisplayToDatabase(btnTarikh.Text) + " " + GeneralBll.ConvertTimeDisplayToDatabase(btnMasa.Text);

            data.NoLaporanPolis = txtNoLaporPolis.Text;
            data.NoLaporanCwgn = txtNoLaporCawangan.Text;

            data.NoEp = txtNoEP.Text;
            data.NoIp = txtNoIP.Text;

            //data.TrkhSalah //todo ask
            data.TempatSalah = txtTempat.Text;
            data.KodAkta = GeneralBll.GetKeySelected(ListAkta, spAkta.SelectedItem?.ToString() ?? "");
            data.KodSalah = _kodSalah;
            data.ButirSalah = txtButirKesalahan.Text;
            //data.IsArahanSemasa //todo ask
            data.TempohTawaran = GeneralBll.ConvertStringToInt(txtTempohTawaran.Text);
            //data.AmnKmp //todo ask
            //data.AmnByr //todo ask

            data.NamaOkk = txtNama.Text;
            data.NoKpOkk = txtNoKp.Text;
            data.NamaPremis = txtNamaSyarikat.Text;
            data.NoDaftarPremis = txtNoDaftarSyarikat.Text;
            data.AlamatOkk1 = txtAlamatPesalah1.Text;
            data.AlamatOkk2 = txtAlamatPesalah2.Text;
            data.AlamatOkk3 = txtAlamatPesalah3.Text;

            data.BarangKompaun = txtBarangKompaun.Text;

            data.NamaPenerima = txtNamaPenerima.Text;
            data.NoKpPenerima = txtNoKpPenerima.Text;
            data.ip_identiti_pelanggan_id = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListJenisKad, spJenisKad.SelectedItem?.ToString() ?? ""));
            data.notelpenerima = txtNoTelefonPenerima.Text;
            data.emelpenerima = txtEmailPenerima.Text;
            data.negeripenerima = GeneralBll.GetKeySelected(ListNegeri, spNegeriPenerima.SelectedItem?.ToString() ?? "");
            data.bandarpenerima = txtBandarPenerima.Text;
            data.poskodpenerima = txtPoskodPenerima.Text;
            data.AlamatPenerima1 = txtAlamatPenerima1.Text;
            data.AlamatPenerima2 = txtAlamatPenerima2.Text;
            data.AlamatPenerima3 = txtAlamatPenerima3.Text;

            data.IsCetakAkuan = Constants.CetakAkuan.No;

            data.PegawaiPengeluar = GeneralBll.GetUserStaffId();
            data.PgnDaftar = data.PegawaiPengeluar;
            data.PgnAkhir = data.PegawaiPengeluar;

            data.IsCetakAkuan = chkAmaran.Checked ? Constants.CetakAkuan.Yes : Constants.CetakAkuan.No;

            data.TrkhSalah = GeneralBll.ConvertDateDisplayToDatabase(btnTarikh.Text) + " " + GeneralBll.ConvertTimeDisplayToDatabase(btnMasa.Text);
            data.IsArahanSemasa = chkArahan.Checked ? Constants.ArahanSemasa.Yes : Constants.ArahanSemasa.No;
            data.TempohTawaran = GeneralBll.ConvertStringToInt(txtTempohTawaran.Text);
            data.AmnKmp = GeneralBll.ConvertStringToDecimal(txtAmaunTawaran.Text);

            data.TrkhPenerima = GeneralBll.GetLocalDateTimeForDatabase();
            data.TrkhDaftar = data.TrkhPenerima;
            data.TrkhAkhir = data.TrkhPenerima;
            data.Status = Constants.Status.Aktif;

            if (KompaunBll.SaveKompaunTrx(data))
            {
                _isSaved = true;
                SetButtonTindakan();
                GeneralAndroidClass.ShowToast(Constants.Messages.SuccessSave);
                SetEnableControl(false);
                _dialog?.Dismiss();


                Print(true);
            }
            else
            {
                GeneralAndroidClass.ShowModalMessage(this, Constants.ErrorMessages.FailedSaveData);

            }
            _dialog?.Dismiss();
        }

        private void SetButtonTindakan()
        {
            btnAkuan.SetBackgroundResource(Resource.Drawable.catatandisable_icon);
            btnAkuan.Enabled = false;

            if (_isSaved && chkAmaran.Checked)
            {
                btnAkuan.SetBackgroundResource(Resource.Drawable.catatan_icon);
                btnAkuan.Enabled = true;
            }


        }

        private void SetEnableControl(bool blValue)
        {
            #region Butiran

            txtNoLaporPolis.Enabled = blValue;
            txtNoLaporCawangan.Enabled = blValue;
            txtNoEP.Enabled = blValue;
            txtNoIP.Enabled = blValue;

            btnTarikh.Enabled = blValue;
            btnMasa.Enabled = blValue;
            txtTempat.Enabled = blValue;
            rdIndividu.Enabled = blValue;
            rdSyarikat.Enabled = blValue;
            spAkta.Enabled = blValue;

            chkArahan.Enabled = blValue;
            txtTempohTawaran.Enabled = blValue;

            //btnKesalahan.Enabled = blValue;

            #endregion

            #region Pesalah

            txtNama.Enabled = blValue;
            txtNoKp.Enabled = blValue;
            txtNamaSyarikat.Enabled = blValue;
            txtNoDaftarSyarikat.Enabled = blValue;
            txtAlamatPesalah1.Enabled = blValue;
            txtAlamatPesalah2.Enabled = blValue;
            txtAlamatPesalah3.Enabled = blValue;
            txtBarangKompaun.Enabled = blValue;
            #endregion

            #region Penerima

            txtNamaPenerima.Enabled = blValue;
            txtNoKpPenerima.Enabled = blValue;
            txtAlamatPenerima1.Enabled = blValue;
            txtAlamatPenerima2.Enabled = blValue;
            txtAlamatPenerima3.Enabled = blValue;
            chkAmaran.Enabled = blValue;
            txtNoIP.Enabled = blValue;
            txtNoKp.Enabled = blValue;

            spJenisKad.Enabled = blValue;
            txtNoTelefonPenerima.Enabled = blValue; 
            txtEmailPenerima.Enabled = blValue;
            spNegeriPenerima.Enabled = blValue;
            txtBandarPenerima.Enabled = blValue;
            btnBandarPenerima.Enabled = blValue;
            btnPoskodPenerima.Enabled = blValue;
            #endregion

            btnImageNama.Enabled = blValue;
            btnImageNamaPenerima.Enabled = blValue;

            if (blValue)
            {
                #region Butiran

                txtNoLaporPolis.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoLaporCawangan.SetBackgroundResource(Resource.Drawable.editText_bg);

                txtNoEP.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoIP.SetBackgroundResource(Resource.Drawable.editText_bg);

                btnTarikh.SetBackgroundResource(Resource.Drawable.editText_bg);
                btnMasa.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtTempat.SetBackgroundResource(Resource.Drawable.editText_bg);
                spAkta.SetBackgroundResource(Resource.Drawable.spiner_bg);
                //txtButirKesalahan.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtTempohTawaran.SetBackgroundResource(Resource.Drawable.editText_bg);

                if (!chkArahan.Checked)
                {
                    txtTempohTawaran.Enabled = false;
                    txtTempohTawaran.SetBackgroundResource(Resource.Drawable.textView_bg);
                }

                #endregion

                #region Pesalah

                txtNama.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoKp.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNamaSyarikat.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoDaftarSyarikat.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPesalah1.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPesalah2.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPesalah3.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtBarangKompaun.SetBackgroundResource(Resource.Drawable.editText_bg);

                #endregion

                #region Penerima

                txtNamaPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoKpPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima1.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima2.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima3.SetBackgroundResource(Resource.Drawable.editText_bg);

                txtNoIP.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoKp.SetBackgroundResource(Resource.Drawable.editText_bg);

                spJenisKad.SetBackgroundResource(Resource.Drawable.spiner_bg);
                txtNoTelefonPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtEmailPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                spNegeriPenerima.SetBackgroundResource(Resource.Drawable.spiner_bg);
                txtBandarPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtPoskodPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);

                #endregion
            }
            else
            {
                #region Butiran

                txtNoLaporPolis.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoLaporCawangan.SetBackgroundResource(Resource.Drawable.textView_bg);

                txtNoEP.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoIP.SetBackgroundResource(Resource.Drawable.textView_bg);

                btnTarikh.SetBackgroundResource(Resource.Drawable.textView_bg);
                btnMasa.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtTempat.SetBackgroundResource(Resource.Drawable.textView_bg);
                spAkta.SetBackgroundResource(Resource.Drawable.textView_bg);
                //txtButirKesalahan.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtTempohTawaran.SetBackgroundResource(Resource.Drawable.textView_bg);



                #endregion

                #region Pesalah

                txtNama.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoKp.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNamaSyarikat.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoDaftarSyarikat.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPesalah1.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPesalah2.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPesalah3.SetBackgroundResource(Resource.Drawable.textView_bg);

                txtBarangKompaun.SetBackgroundResource(Resource.Drawable.textView_bg);

                #endregion

                #region Penerima

                txtNamaPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoKpPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima1.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima2.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima3.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoIP.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoKp.SetBackgroundResource(Resource.Drawable.textView_bg);

                spJenisKad.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoTelefonPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtEmailPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                spNegeriPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtBandarPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtPoskodPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                #endregion
            }

        }

        EditText txtCarian;
        ListView listView;
        List<KesalahanDto> listOfKesalahan;
        private int _kodSalah;

        private void ShowKesalahan()
        {
            var selectedAkta = GeneralBll.GetKeySelected(ListAkta,
                spAkta.SelectedItem?.ToString() ?? "");

            listOfKesalahan = MasterDataBll.GetKesalahanByAkta(selectedAkta);

            var listOfFiltered = listOfKesalahan;


            var builder = new AlertDialog.Builder(this).Create();
            var view = this.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);

            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Kesalahan";

            listView.Adapter = new CarianKesalahanAdapter(this, listOfKesalahan);

            txtCarian.TextChanged += (send, args) =>
            {
                listOfFiltered = listOfKesalahan
                    .Where(m => m.Prgn.ToLower().Contains(txtCarian.Text.ToLower())).ToList();

                listView.Adapter = new CarianKesalahanAdapter(this, listOfFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                txtKesalahan.Text = listOfFiltered[args.Position]?.Seksyen;
                if (string.IsNullOrEmpty(txtKesalahan.Text))
                {
                    txtKesalahan.Text = " ";
                }

                _kodSalah = listOfFiltered[args.Position] != null
                    ? listOfFiltered[args.Position].KodSalah
                    : 0;
                txtButirKesalahan.Text = listOfFiltered[args.Position]?.OriginalPrgn;
                SetAmountTawaran();
                SetPrintButton();
                //txtButirKesalahan.Text = "";
                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private void SetAmountTawaran()
        {
            var selectedAkta = GeneralBll.GetKeySelected(ListAkta, spAkta.SelectedItem?.ToString() ?? "");
            var data = KompaunBll.GetAmountKesalahan(selectedAkta, _kodSalah,
                rdIndividu.Checked);
            txtAmaunTawaran.Text = data.ToString(Constants.DecimalFormat);
        }

        protected override void OnResume()
        {
            base.OnResume();
            //if (_activeForm == Enums.ActiveForm.Akuan && GeneralBll.GetFinishPage() == Constants.FinishPage)
            //if (_activeForm == Enums.ActiveForm.Akuan)
            //{
            //    GeneralBll.SetFinishPage();
            //    this.Finish();
            //}
            if (_activeForm == Enums.ActiveForm.Camera)
            {
                _activeForm = Enums.ActiveForm.Kompaun;
                SetPrintButton();
            }
        }

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
#if DEBUG
            return true;
#endif

            if (string.IsNullOrEmpty(btnTarikh.Text)) return false;
            if (string.IsNullOrEmpty(btnMasa.Text)) return false;
            if (string.IsNullOrEmpty(txtTempat.Text)) return false;

            if (string.IsNullOrEmpty(spAkta.SelectedItem?.ToString())) return false;
            if (_kodSalah == 0) return false;

            if (string.IsNullOrEmpty(txtButirKesalahan.Text)) return false;
            if (string.IsNullOrEmpty(txtNama.Text)) return false;
            if (string.IsNullOrEmpty(txtAlamatPesalah1.Text)) return false;
            if (string.IsNullOrEmpty(txtNamaPenerima.Text)) return false;
            if (string.IsNullOrEmpty(txtAlamatPenerima1.Text)) return false;
            if (string.IsNullOrEmpty(txtBarangKompaun.Text)) return false;

            //#if !DEBUG
            //            var countPhoto = GeneralBll.GetCountPhotoByRujukan(lblNoKpp.Text);
            //            if (countPhoto < Constants.MinPhoto) return false;
            //#endif
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

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
#if DEBUG
            if (keyCode == Keycode.Back)
                return false;
#endif
            return base.OnKeyDown(keyCode, e);
        }

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
            result.Add(GeneralBll.CreateConfirmDto("Butiran", "", true));
            result.Add(GeneralBll.CreateConfirmDto("No. Lapor Polis", txtNoLaporPolis.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. Lapor Cawangan", txtNoLaporCawangan.Text));
            result.Add(GeneralBll.CreateConfirmDto("Tarikh", btnTarikh.Text));
            result.Add(GeneralBll.CreateConfirmDto("Masa", btnMasa.Text));
            result.Add(GeneralBll.CreateConfirmDto("Tempat", txtTempat.Text));
            result.Add(GeneralBll.CreateConfirmDto("Jenis", rdIndividu.Checked ? "Individu" : "Syarikat"));
            result.Add(GeneralBll.CreateConfirmDto("Akta", spAkta.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Kesalahan", txtKesalahan.Text));
            result.Add(GeneralBll.CreateConfirmDto("Butir Kesalahan", txtButirKesalahan.Text));
            result.Add(GeneralBll.CreateConfirmDto("Amaun Tawaran", txtAmaunTawaran.Text));
            result.Add(GeneralBll.CreateConfirmDto("Arahan Semasa", chkArahan.Checked ? "Ya" : "Tidak"));
            result.Add(GeneralBll.CreateConfirmDto("Tempoh Tawaran", txtTempohTawaran.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. EP", txtNoEP.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. IP", txtNoIP.Text));

            result.Add(GeneralBll.CreateConfirmDto("Pesalah", "", true));
            result.Add(GeneralBll.CreateConfirmDto("Nama", txtNama.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. K/P", txtNoKp.Text));
            result.Add(GeneralBll.CreateConfirmDto("Nama Syarikat/Premis", txtNamaSyarikat.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. Daftar Syarikat", txtNoDaftarSyarikat.Text));

            var alamat =
                GeneralBll.GettOneAlamat(txtAlamatPesalah1.Text, txtAlamatPesalah2.Text, txtAlamatPesalah3.Text);
            result.Add(GeneralBll.CreateConfirmDto("Alamat Pesalah", alamat));

            result.Add(GeneralBll.CreateConfirmDto("Barang yang dikompaun", txtBarangKompaun.Text));

            result.Add(GeneralBll.CreateConfirmDto("Penerima", "", true));
            result.Add(GeneralBll.CreateConfirmDto("Nama", txtNamaPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. K/P", txtNoKpPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Jenis Kad", spJenisKad.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("No. K/P", txtNoKpPenerima.Text));
            //result.Add(GeneralBll.CreateConfirmDto("Kewarganegaraan", spKewarganegaraan.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("No. Telefon", txtNoTelefonPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Email", txtEmailPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Negeri", spNegeriPenerima.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Bandar", txtBandarPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Poskod", txtPoskodPenerima.Text));

            alamat =
                GeneralBll.GettOneAlamat(txtAlamatPenerima1.Text, txtAlamatPenerima2.Text, txtAlamatPenerima3.Text);
            result.Add(GeneralBll.CreateConfirmDto("Alamat", alamat));

            result.Add(GeneralBll.CreateConfirmDto("Cetak Akuan Penerima", chkAmaran.Checked ? "Ya" : "Tidak"));



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
                        _alert.SetMessage(Constants.Messages.SelectYourItem);
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
                        var bitmap = printImageBll.Kompaun(this, lblNoKpp.Text);

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
            readonly Kompaun activity;

            public ServicetHandler(Kompaun activity)
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
                var bitmap = printImageBll.Kompaun(this, lblNoKpp.Text);

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
            var result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(lblNoKpp.Text, Enums.TableType.Kompaun, this)).Result;
            if (result.Success)
            {
                GeneralAndroidClass.ShowToast(Constants.Messages.SuccessSendData);
                _dialog?.Dismiss();
                //if (!chkAmaran.Checked)
                //{
                //    this.Finish();
                //}
            }
            else
            {
                _dialog?.Dismiss();

                var tbSendOnline = MasterDataBll.GetTbSendOnlineByRujukanAndType(lblNoKpp.Text, Enums.TableType.Kompaun);
                if (tbSendOnline != null && tbSendOnline.Status == Enums.StatusOnline.Sent)
                {
                    return;
                }

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
                    SendOnlineBll.SetStatusDataOnline(lblNoKpp.Text, Enums.TableType.Kompaun, Enums.StatusOnline.Error);

                    //if (!chkAmaran.Checked)
                    //{
                    //    this.Finish();
                    //}

                });
                //alertDialog.DismissEvent += (send, args) =>
                //{
                //    alertDialog.Dismiss();
                //    SendOnlineBll.SetStatusDataOnline(lblNoKpp.Text, Enums.TableType.Kompaun, Enums.StatusOnline.Error);                    
                //};

                alertDialog.Show();

            }
        }

        private void BtnSearchJpn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtNoKp.Text))
                {
                    _hourGlass?.StartMessage(this, OnSearchJpnDetail);
                }
                //OnShowCarianJpnDetail();

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnSearchJpn_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void OnSearchJpnDetail()
        {
            var listJpnDetail = PemeriksaanBll.GetListJpnDetail(txtNoKp.Text, SharedPreferences.GetString(SharedPreferencesKeys.UserNoKp));
            txtNama.Text = string.Empty;
            txtAlamatPesalah1.Text = string.Empty;
            txtAlamatPesalah2.Text = string.Empty;
            txtAlamatPesalah3.Text = string.Empty;



            if (listJpnDetail.Success)
            {
                if (listJpnDetail.Result.status == "200")
                {
                    txtNama.Text = listJpnDetail.Result.name;
                    txtAlamatPesalah1.Text = listJpnDetail.Result.address1;
                    txtAlamatPesalah2.Text = listJpnDetail.Result.address2;
                    txtAlamatPesalah3.Text =
                        $"{listJpnDetail.Result.postcode} {listJpnDetail.Result.city} {listJpnDetail.Result.state}";

                    if (!string.IsNullOrEmpty(listJpnDetail.Result.address3))
                    {
                        txtAlamatPesalah3.Text =
                            $"{listJpnDetail.Result.address3} {txtAlamatPesalah3.Text}";
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