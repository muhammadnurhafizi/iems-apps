using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Com.Woosim.Printer;
using IEMSApps.Activities;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using System.Threading.Tasks;
using Android.InputMethodServices;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views.InputMethods;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Responses;
using Java.Lang;
using Enum = Java.Lang.Enum;
using Exception = System.Exception;
using Thread = System.Threading.Thread;
using Plugin.BxlMpXamarinSDK.Abstractions;
using System.Threading;
using Plugin.BxlMpXamarinSDK;
using static IEMSApps.Utils.Constants;

namespace IEMSApps.Fragments
{
    public class Pemeriksaan : BaseFragment
    {
        private const string LayoutName = "Pemeriksaan";
        private HourGlassClass _hourGlass = new HourGlassClass();
        ServicetHandler handler;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.Pemeriksaan, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            //SetInit();
            _printer = null;
            _connectionInfo = null;

            _hourGlass?.StartMessage(this.Activity, SetInit);
        }


        LinearLayout tabPremis, tabLawatan, tabPenerima;
        TextView lblTabPremis, lblTabLawatan, lblTabPenerima;
        View viewPremis, viewLawatan, viewPenerima;

        #region Init Premis
        private TextView lblNoKpp;

        private EditText txtNamaPremis, txtAlamat1, txtAlamat2, txtAlamat3;
        private EditText txtNoDaftarSyarikat, txtNoLesenBkPda, txtNoLesenMajelisPremis, txtNoTelefon, txtLainLain;
        private EditText txtJenisNiaga, txtBandar;
        private Button btnJenisNiaga, btnTarikh, btnMasa, btnTarikhMula, btnMasaMula, btnBandar, btnSearchSsm;
        private Spinner spKategoryPremis, spNegeri;

        private CheckBox chkAmaran;

        private LinearLayout linearJenamaStesen;

        private EditText txtLokaliti, txtAgensiSerahan;
        private Spinner spKategoriPerniagaan, spJenamaStesenMinyak;
        private Dictionary<string, string> ListKategoriPerniagaan, ListStesenMinyak;
        #endregion

        #region Init Lawatan

        private Spinner spKategoryKawasan;//, spTujuanLawatan;

        private EditText txtLokasi, txtNoRujukanAtr, txtNoAduan, txtCatatanLawatan, txtHasilLawatan;

        private RelativeLayout relativeAgensiSerahan;
        private EditText txtAsasTindakan;
        private Button btnAsasTindakan;
        private Button btnLokaliti, btnAgensiSerahan;

        #endregion

        #region Init Penerima
        private EditText txtNamaPenerima, txtNoKpPenerima, txtJawatanPenerima, txtNoPassport;

        private EditText txtAlamatPenerima1, txtAlamatPenerima2, txtAlamatPenerima3;
        private Button btnNamaPenerima;
        private CheckBox chkBayar, chkNB, chkNPMB;

        private Dictionary<string, string> ListTujuanLawatan, ListKategoriKawasan;
        private Dictionary<string, string> ListKategoryPremis, ListNegeri;
        //private RadioButton rdTiadaKes, rdKots, rdSiasatanLanjut;
        private Button btnOk, btnCamera, btnPrint, btnNote, btnLokasi, btnSearchJpn;
        private Spinner spTindakan, spKewarganegaraan;
        private Dictionary<string, string> ListTindakan, ListWarganegara;

        private AlertDialog _dialog;

        private string startTime = "";
        private bool _isSaved = false;

        private Enums.ActiveForm _activeForm = Enums.ActiveForm.Pemeriksaan;

        private bool _reprint;

        private Button btnKesalahanKompaun;
        private EditText txtNoIP, txtNoEP;
        private LinearLayout linearSiasatUlangan, linearButtonKesalahan, linearNotisSerahan;
        private RelativeLayout relativeNoPassport;

        private bool _isSkip;

        #endregion

        private MPosControllerPrinter _printer;
        MposConnectionInformation _connectionInfo;
        private static SemaphoreSlim _printSemaphore = new SemaphoreSlim(1, 1);

        private void SetInit()
        {
            try
            {
                //GeneralAndroidClass.CheckBackgroundService(this.Activity);

                tabPremis = View.FindViewById<LinearLayout>(Resource.Id.tabPremis);
                tabLawatan = View.FindViewById<LinearLayout>(Resource.Id.tabLawatan);
                tabPenerima = View.FindViewById<LinearLayout>(Resource.Id.tabPenerima);

                lblTabPremis = View.FindViewById<TextView>(Resource.Id.lblTabPremis);
                lblTabLawatan = View.FindViewById<TextView>(Resource.Id.lblTabLawatan);
                lblTabPenerima = View.FindViewById<TextView>(Resource.Id.lblTabPenerima);

                viewPremis = View.FindViewById<View>(Resource.Id.viewPremis);
                viewLawatan = View.FindViewById<View>(Resource.Id.viewLawatan);
                viewPenerima = View.FindViewById<View>(Resource.Id.viewPenerima);

                tabPremis.Click += TabPremis_Click;
                tabLawatan.Click += TabLawatan_Click;
                tabPenerima.Click += TabPenerima_Click;

                SetLayoutVisible(viewLawatan, lblTabLawatan, tabLawatan);
                SetLayoutInvisible(viewPremis, lblTabPremis, tabPremis);
                SetLayoutInvisible(viewPenerima, lblTabPenerima, tabPenerima);

                _kodAsasSelected = new List<AsasTindakanDto>();
                _lokalitiSelected = new List<LokalitiKategoriKhasDto>();
                _agensiSerahanSelected = new List<AgensiSerahanDto>();

                LoadData();

                LoadDataDropdown();

                SetButtonTindakan();

                SetPrintButton();

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "SetInit", ex.Message, Enums.LogType.Error);
            }
            _hourGlass?.StopMessage();
        }

        //private Timer _timer;

        //private void LoadDateTime()
        //{
        //    startTime = GeneralBll.GetLocalDateTime().ToString(Constants.TimeFormatDatabase);

        //    UpdateTime();

        //    _timer = new Timer { Interval = 1000 };
        //    _timer.Elapsed += Timer_Elapsed; ;
        //    _timer.Start();
        //}



        private void LoadData()
        {
            var allowedFilter = new FilterChar();
            var allowedFilterWithoutSingleQuote = new FilterCharWithoutSingleQuote();

            #region Premis

            lblNoKpp = View.FindViewById<TextView>(Resource.Id.lblNoKpp);

            var localDate = GeneralBll.GetLocalDateTime().ToString(Constants.DateFormatDisplay);
            var localTime = GeneralBll.GetLocalDateTime().ToString(Constants.TimeFormatDisplay);

            btnTarikh = View.FindViewById<Button>(Resource.Id.btnTarikh);
            btnMasa = View.FindViewById<Button>(Resource.Id.btnMasa);
            btnTarikh.Text = localDate;
            btnMasa.Text = localTime;

            btnTarikh.Click += BtnTarikh_Click;
            btnMasa.Click += BtnMasa_Click;

            btnTarikhMula = View.FindViewById<Button>(Resource.Id.btnTarikhMula);
            btnMasaMula = View.FindViewById<Button>(Resource.Id.btnMasaMula);
            btnTarikhMula.Text = localDate;
            btnMasaMula.Text = localTime;

            btnTarikhMula.Click += BtnTarikhMula_Click;
            btnMasaMula.Click += BtnMasaMula_Click;

            spKategoryPremis = View.FindViewById<Spinner>(Resource.Id.spKategoryPremis);
            spNegeri = View.FindViewById<Spinner>(Resource.Id.spNegeri);

            txtNamaPremis = View.FindViewById<EditText>(Resource.Id.txtNamaPremis);

            txtAlamat1 = View.FindViewById<EditText>(Resource.Id.txtAlamat1);
            txtAlamat2 = View.FindViewById<EditText>(Resource.Id.txtAlamat2);
            txtAlamat3 = View.FindViewById<EditText>(Resource.Id.txtAlamat3);
            txtNoDaftarSyarikat = View.FindViewById<EditText>(Resource.Id.txtNoDaftarSyarikat);
            txtNoLesenBkPda = View.FindViewById<EditText>(Resource.Id.txtNoLesenBkPda);
            txtNoLesenMajelisPremis = View.FindViewById<EditText>(Resource.Id.txtNoLesenMajelisPremis);
            txtNoTelefon = View.FindViewById<EditText>(Resource.Id.txtNoTelefon);
            txtLainLain = View.FindViewById<EditText>(Resource.Id.txtLainLain);


            txtJenisNiaga = View.FindViewById<EditText>(Resource.Id.txtJenisNiaga);
            btnJenisNiaga = View.FindViewById<Button>(Resource.Id.btnJenisNiaga);

            btnJenisNiaga.Click += BtnJenisNiaga_Click;

            btnSearchSsm = View.FindViewById<Button>(Resource.Id.btnSearchSsm);
            btnSearchSsm.Click += BtnSearchSsm_Click;

            txtBandar = View.FindViewById<EditText>(Resource.Id.txtBandar);
            btnBandar = View.FindViewById<Button>(Resource.Id.btnBandar);
            btnBandar.Click += BtnBandar_Click;

            lblNoKpp.Text = GeneralBll.GenerateNoRujukan(Enums.PrefixType.KPP);

            spKategoriPerniagaan = View.FindViewById<Spinner>(Resource.Id.spKategoriPerniagaan);

            linearJenamaStesen = View.FindViewById<LinearLayout>(Resource.Id.linearJenamaStesen);
            spJenamaStesenMinyak = View.FindViewById<Spinner>(Resource.Id.spJenamaStesenMinyak);

            txtNamaPremis.TextChanged += Event_CheckMandatory_Dropdown_Edittext;
            txtAlamat1.TextChanged += Event_CheckMandatory_Dropdown_Edittext;

            txtNamaPremis.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(150), allowedFilter });
            txtAlamat1.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(300), allowedFilter });
            txtAlamat2.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowedFilter });
            txtAlamat3.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowedFilter });
            txtNoDaftarSyarikat.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(15), allowedFilterWithoutSingleQuote });
            txtNoLesenBkPda.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(15), allowedFilterWithoutSingleQuote });
            txtNoLesenMajelisPremis.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(15), allowedFilterWithoutSingleQuote });
            txtNoTelefon.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(12), allowedFilterWithoutSingleQuote });
            txtLainLain.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(150), allowedFilter });


            #endregion

            #region Lawatan

            spKategoryKawasan = View.FindViewById<Spinner>(Resource.Id.spKategoryKawasan);
            txtLokasi = View.FindViewById<EditText>(Resource.Id.txtLokasi);
            txtNoRujukanAtr = View.FindViewById<EditText>(Resource.Id.txtNoRujukanAtr);
            //spTujuanLawatan = View.FindViewById<Spinner>(Resource.Id.spTujuanLawatan);
            //spAsasTindakan = View.FindViewById<Spinner>(Resource.Id.spAsasTindakan);
            txtNoAduan = View.FindViewById<EditText>(Resource.Id.txtNoAduan);
            txtCatatanLawatan = View.FindViewById<EditText>(Resource.Id.txtCatatanLawatan);
            txtHasilLawatan = View.FindViewById<EditText>(Resource.Id.txtHasilLawatan);
            btnLokasi = View.FindViewById<Button>(Resource.Id.btnLokasi);
            btnLokasi.Click += BtnLokasi_Click;
            txtLokasi.TextChanged += Event_CheckMandatory_Dropdown_Edittext;
            txtHasilLawatan.TextChanged += Event_CheckMandatory_Dropdown_Edittext;

            txtLokaliti = View.FindViewById<EditText>(Resource.Id.txtLokaliti);

            txtLokasi.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowedFilter });
            txtNoRujukanAtr.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(13), allowedFilterWithoutSingleQuote });
            txtNoAduan.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(15), allowedFilterWithoutSingleQuote });
            txtCatatanLawatan.SetFilters(
                new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(250), allowedFilter });
            txtHasilLawatan.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(1000), allowedFilter });

            txtAsasTindakan = View.FindViewById<EditText>(Resource.Id.txtAsasTindakan);
            btnAsasTindakan = View.FindViewById<Button>(Resource.Id.btnAsasTindakan);

            btnAsasTindakan.Click += BtnAsasTindakan_Click;

            btnLokaliti = View.FindViewById<Button>(Resource.Id.btnLokaliti);
            btnLokaliti.Click += BtnLokaliti_Click;

            relativeAgensiSerahan = View.FindViewById<RelativeLayout>(Resource.Id.relativeAgensiSerahan);
            relativeAgensiSerahan.Visibility = ViewStates.Gone;
            txtAgensiSerahan = View.FindViewById<EditText>(Resource.Id.txtAgensiSerahan);

            btnAgensiSerahan = View.FindViewById<Button>(Resource.Id.btnAgensiSerahan);
            btnAgensiSerahan.Click += BtnAgensiSerahan_Click;
            #endregion

            #region Penerima

            txtNamaPenerima = View.FindViewById<EditText>(Resource.Id.txtNamaPenerima);
            btnNamaPenerima = View.FindViewById<Button>(Resource.Id.btnNamaPenerima);
            txtNoKpPenerima = View.FindViewById<EditText>(Resource.Id.txtNoKpPenerima);
            txtJawatanPenerima = View.FindViewById<EditText>(Resource.Id.txtJawatanPenerima);
            txtAlamatPenerima1 = View.FindViewById<EditText>(Resource.Id.txtAlamatPenerima1);
            txtAlamatPenerima2 = View.FindViewById<EditText>(Resource.Id.txtAlamatPenerima2);
            txtAlamatPenerima3 = View.FindViewById<EditText>(Resource.Id.txtAlamatPenerima3);
            txtNoPassport = View.FindViewById<EditText>(Resource.Id.txtNoPassport);


            //rdTiadaKes = View.FindViewById<RadioButton>(Resource.Id.rdTiadaKes);
            //rdKots = View.FindViewById<RadioButton>(Resource.Id.rdKots);
            //rdSiasatanLanjut = View.FindViewById<RadioButton>(Resource.Id.rdSiasatanLanjut);
            chkBayar = View.FindViewById<CheckBox>(Resource.Id.chkBayar);
            chkAmaran = View.FindViewById<CheckBox>(Resource.Id.chkAmaran);
            chkNB = View.FindViewById<CheckBox>(Resource.Id.chkNB);
            chkNPMB = View.FindViewById<CheckBox>(Resource.Id.chkNPMB);

           btnKesalahanKompaun = View.FindViewById<Button>(Resource.Id.btnKesalahanKompaun);
            btnKesalahanKompaun.Enabled = false;
            linearButtonKesalahan = View.FindViewById<LinearLayout>(Resource.Id.linearButtonKesalahan);
            linearButtonKesalahan.Visibility = ViewStates.Gone;

            btnKesalahanKompaun.Click += BtnKesalahanKompaun_Click;

            //rdTiadaKes.Checked = true;
            chkBayar.Enabled = false;

            txtNamaPenerima.TextChanged += Event_CheckMandatory_Dropdown_Edittext;
            txtNoKpPenerima.TextChanged += TxtNoKpPenerima_TextChanged;

            txtAlamatPenerima1.TextChanged += Event_CheckMandatory_Dropdown_Edittext;

            btnNamaPenerima.Click += BtnNamaPenerima_Click;

            btnSearchJpn = View.FindViewById<Button>(Resource.Id.btnSearchJpn);
            btnSearchJpn.Click += BtnSearchJpn_Click;

            //rdKots.CheckedChange += RdKots_CheckedChange;
            //rdTiadaKes.CheckedChange += RdTiadaKes_CheckedChange;

            txtNamaPenerima.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowedFilter });
            txtNoKpPenerima.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowedFilterWithoutSingleQuote });
            txtJawatanPenerima.SetFilters(
                new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowedFilter });
            txtAlamatPenerima1.SetFilters(
                new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowedFilter });
            txtAlamatPenerima2.SetFilters(
                new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowedFilter });
            txtAlamatPenerima3.SetFilters(
                new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowedFilter });


            spKewarganegaraan = View.FindViewById<Spinner>(Resource.Id.spKewarganegaraan);
            spTindakan = View.FindViewById<Spinner>(Resource.Id.spTindakan);

            txtNoIP = View.FindViewById<EditText>(Resource.Id.txtNoIP);
            txtNoEP = View.FindViewById<EditText>(Resource.Id.txtNoEP);
            linearSiasatUlangan = View.FindViewById<LinearLayout>(Resource.Id.linearSiasatUlangan);

            txtNoIP.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new FilterNoIPAndNoEPChar(), new InputFilterLengthFilter(30) });
            txtNoEP.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new FilterNoIPAndNoEPChar(), new InputFilterLengthFilter(30) });

            linearSiasatUlangan.Visibility = ViewStates.Gone;

            linearNotisSerahan = View.FindViewById<LinearLayout>(Resource.Id.linearNotisSerahan);
            linearNotisSerahan.Visibility = ViewStates.Gone;

            relativeNoPassport = View.FindViewById<RelativeLayout>(Resource.Id.relativeNoPassport);

            #endregion

            #region Button

            btnOk = View.FindViewById<Button>(Resource.Id.btnOk);
            btnCamera = View.FindViewById<Button>(Resource.Id.btnCamera);
            btnPrint = View.FindViewById<Button>(Resource.Id.btnPrint);
            btnNote = View.FindViewById<Button>(Resource.Id.btnNote);

            //lblBtnOk = View.FindViewById<TextView>(Resource.Id.lblBtnOk);
            //lblBtnCamera = View.FindViewById<TextView>(Resource.Id.lblBtnCamera);
            //lblBtnPrint = View.FindViewById<TextView>(Resource.Id.lblBtnPrint);
            //lblBtnNote = View.FindViewById<TextView>(Resource.Id.lblBtnNote);

            btnOk.Click += BtnOk_Click;
            btnCamera.Click += BtnCamera_Click;
            btnPrint.Click += BtnPrint_Click;
            btnNote.Click += BtnNote_Click;



            #endregion
        }

        private void BtnAgensiSerahan_Click(object sender, EventArgs e)
        {
            try
            {
                ShowAgensiTerlibat();
            }
            catch (Exception ex)
            {

                GeneralAndroidClass.LogData(LayoutName, "BtnAgensiSerahan_Click", ex.Message, Enums.LogType.Error);

            }
        }

        private void BtnLokaliti_Click(object sender, EventArgs e)
        {
            try
            {
                ShowLokaliti();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnLokaliti_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnSearchSsm_Click(object sender, EventArgs e)
        {
            try
            {
                OnShowCarianSsm();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnSearchSsm_Click", ex.Message, Enums.LogType.Error);
            }
        }

        //public override ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest,
        //    int dstart, int dend)
        //{
        //    StringBuilder sbText = new StringBuilder(source);

        //    string text = sbText.ToString();

        //    if (text.Contains(" "))
        //    {
        //        return "";
        //    }
        //    return c;
        //}


        private void BtnBandar_Click(object sender, EventArgs e)
        {
            try
            {
                ShowBandar();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnBandar_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnKesalahanKompaun_Click(object sender, EventArgs e)
        {
            try
            {
                ShowKompaunKesalahan();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnKesalahanKompaun_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnMasaMula_Click(object sender, EventArgs e)
        {
            try
            {
                TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
                {
                    btnMasaMula.Text = time.ToString(Constants.TimeFormatDisplay);
                });

                frag.Show(FragmentManager, TimePickerFragment.TAG);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnMasaMula_Click", ex.Message, Enums.LogType.Error);
            }

        }

        private void BtnTarikhMula_Click(object sender, EventArgs e)
        {
            try
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    btnTarikhMula.Text = time.ToString(Constants.DateFormatDisplay);
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnTarikhMula_Click", ex.Message, Enums.LogType.Error);
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

        private void BtnAsasTindakan_Click(object sender, EventArgs e)
        {
            try
            {
                ShowAsasTindakan();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnAsasTindakan_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnJenisNiaga_Click(object sender, EventArgs e)
        {
            try
            {
                ShowJenisNiaga();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnJenisNiaga_Click", ex.Message, Enums.LogType.Error);
            }
        }

        EditText txtCarian;
        ListView listView;
        List<PremisDto> listOfPremis;
        List<JenisNiagaDto> listOfJenisNiaga;
        private int _jenisNiaga;

        List<AsasTindakanDto> listOfAsasTindakan;
        //private int _asasTindakan;
        private List<AsasTindakanDto> _kodAsasSelected;

        private void ShowAsasTindakan()
        {
            listOfAsasTindakan = MasterDataBll.GetAsasTindakanByTujuan();
            var kodAsasSelected = new List<AsasTindakanDto>();
            var newAsasTindakan = new List<AsasTindakanDto>();

            if (_kodAsasSelected.Any())
            {
                //kodAsasSelected = _kodAsasSelected;
                foreach (var item in listOfAsasTindakan)
                {
                    item.IsSelected = _kodAsasSelected.Any(m => m.KodAsas == item.KodAsas && m.KodTujuan == item.KodTujuan);
                    if (item.IsSelected)
                    {
                        kodAsasSelected.Add(item);
                    }
                }
            }

            var listOfFiltered = listOfAsasTindakan;


            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);
            builder.SetCancelable(false);
            builder.SetButton2(Constants.Messages.Yes, (c, ev) =>
            {
                if (kodAsasSelected.Any())
                {
                    txtAsasTindakan.Text = string.Join(", ",
                        listOfAsasTindakan.Where(m => kodAsasSelected.Any(x => m.KodAsas == x.KodAsas && m.KodTujuan == x.KodTujuan)).Select(m => m.Prgn)
                            .ToArray());

                    ShowAgensiTerlibat(txtAsasTindakan.Text);
                }
                else
                {
                    txtAsasTindakan.Text = "";
                }

                _kodAsasSelected = new List<AsasTindakanDto>();
                foreach (var i in kodAsasSelected)
                {
                    _kodAsasSelected.Add(i);
                }

                SetPrintButton();

                builder.Dismiss();
            });
            builder.SetButton(Constants.Messages.No, (c, ev) =>
            {

                kodAsasSelected = new List<AsasTindakanDto>();
                foreach (var i in _kodAsasSelected)
                {
                    kodAsasSelected.Add(i);
                }
                SetPrintButton();
                builder.Dismiss();
            });
            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Asas Tindakan";

            listView.Adapter = new CarianAsasTindakanMultipleAdapter(this.Activity, listOfAsasTindakan);

            txtCarian.TextChanged += (send, args) =>
            {
                listOfFiltered = listOfAsasTindakan
                    .Where(m => m.Prgn.ToLower().Contains(txtCarian.Text.ToLower())).ToList();

                if (kodAsasSelected.Any())
                {
                    foreach (var item in listOfFiltered)
                    {
                        item.IsSelected = kodAsasSelected.Any(m => m.KodAsas == item.KodAsas && m.KodTujuan == item.KodTujuan);
                    }
                }

                listView.Adapter = new CarianAsasTindakanMultipleAdapter(this.Activity, listOfFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                listOfFiltered[args.Position].IsSelected = !listOfFiltered[args.Position].IsSelected;

                if (kodAsasSelected.Any(m => m.KodAsas == listOfFiltered[args.Position].KodAsas && m.KodTujuan == listOfFiltered[args.Position].KodTujuan))
                {
                    kodAsasSelected.Remove(listOfFiltered[args.Position]);
                    if (newAsasTindakan.Any(c => c.KodAsas == listOfFiltered[args.Position].KodAsas && c.KodTujuan == listOfFiltered[args.Position].KodTujuan))
                    {
                        newAsasTindakan.Remove(listOfFiltered[args.Position]);
                    }
                }
                else
                {
                    kodAsasSelected.Add(listOfFiltered[args.Position]);
                    newAsasTindakan.Add(listOfFiltered[args.Position]);
                }

                listView.InvalidateViews();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private void ShowAgensiTerlibat(string show)
        {
            if (show.Contains("OPERASI BERSEPADU BERSAMA AGENSI"))
            {
                relativeAgensiSerahan.Visibility = ViewStates.Visible;
            }
            else
            {
                relativeAgensiSerahan.Visibility = ViewStates.Gone;
            }
        }


        List<LokalitiKategoriKhasDto> listLokaliti;
        private List<LokalitiKategoriKhasDto> _lokalitiSelected;

        private void ShowLokaliti()
        {
            listLokaliti = MasterDataBll.GetLokalitiKategoriKhas();
            var lokalitiSelected = new List<LokalitiKategoriKhasDto>();
            var newlokalitiSelected = new List<LokalitiKategoriKhasDto>();

            if (_lokalitiSelected.Any())
            {
                foreach (var item in listLokaliti)
                {
                    item.IsSelected = _lokalitiSelected.Any(m => m.Id == item.Id && m.Prgn == item.Prgn);
                    if (item.IsSelected)
                    {
                        lokalitiSelected.Add(item);
                    }
                }
            }

            var listFiltered = listLokaliti;


            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);
            builder.SetCancelable(false);
            builder.SetButton2(Constants.Messages.Yes, (c, ev) =>
            {
                if (lokalitiSelected.Any())
                {
                    txtLokaliti.Text = string.Join(", ",
                        listLokaliti.Where(m => lokalitiSelected.Any(x => m.Id == x.Id && m.Prgn == x.Prgn)).Select(m => m.Prgn)
                            .ToArray());
                }
                else
                {
                    txtLokaliti.Text = "";
                }

                _lokalitiSelected = new List<LokalitiKategoriKhasDto>();
                foreach (var i in lokalitiSelected)
                {
                    _lokalitiSelected.Add(i);
                }
                SetPrintButton();
                builder.Dismiss();
            });
            builder.SetButton(Constants.Messages.No, (c, ev) =>
            {
                lokalitiSelected = new List<LokalitiKategoriKhasDto>();
                foreach (var i in _lokalitiSelected)
                {
                    lokalitiSelected.Add(i);
                }
                SetPrintButton();
                builder.Dismiss();
            });
            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Lokaliti/ Kategori Khas";

            listView.Adapter = new CarianLokalitiMultipleAdapter(this.Activity, listLokaliti);

            txtCarian.TextChanged += (send, args) =>
            {
                listFiltered = listLokaliti
                    .Where(m => m.Prgn.ToLower().Contains(txtCarian.Text.ToLower())).ToList();

                if (lokalitiSelected.Any())
                {
                    foreach (var item in listFiltered)
                    {
                        item.IsSelected = lokalitiSelected.Any(m => m.Id == item.Id && m.Prgn == item.Prgn);
                    }
                }
                listView.Adapter = new CarianLokalitiMultipleAdapter(this.Activity, listFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                listFiltered[args.Position].IsSelected = !listFiltered[args.Position].IsSelected;
                if (lokalitiSelected.Any(m => m.Id == listFiltered[args.Position].Id && m.Prgn == listFiltered[args.Position].Prgn))
                {
                    lokalitiSelected.Remove(listFiltered[args.Position]);
                    if (newlokalitiSelected.Any(c => c.Id == listFiltered[args.Position].Id && c.Prgn == listFiltered[args.Position].Prgn))
                    {
                        newlokalitiSelected.Remove(listFiltered[args.Position]);
                    }
                }
                else
                {
                    lokalitiSelected.Add(listFiltered[args.Position]);
                    newlokalitiSelected.Add(listFiltered[args.Position]);
                }

                listView.InvalidateViews();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        List<BandarDto> listOfBandar;
        private void ShowBandar()
        {
            var selectedNegeri = GeneralBll.GetKeySelected(ListNegeri, spNegeri.SelectedItem?.ToString() ?? "");

            listOfBandar = MasterDataBll.GetBandarByNegeri(selectedNegeri);


            var listOfBandarFiltered = listOfBandar;


            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);

            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Bandar";

            listView.Adapter = new CarianBandarAdapter(this.Activity, listOfBandar);

            txtCarian.TextChanged += (send, args) =>
            {
                listOfBandarFiltered = listOfBandar
                    .Where(m => m.Prgn.ToLower().Contains(txtCarian.Text.ToLower())).ToList();

                listView.Adapter = new CarianBandarAdapter(this.Activity, listOfBandarFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                txtBandar.Text = listOfBandarFiltered[args.Position]?.Prgn;

                var negeriName = MasterDataBll.GetNegeriName(GeneralBll.ConvertStringToInt(selectedNegeri));
                var kodBandar = listOfBandarFiltered[args.Position] != null
                    ? listOfBandarFiltered[args.Position].KodBandar
                    : 0;

                var bandarName = MasterDataBll.GetBandarNameByNegeri(GeneralBll.ConvertStringToInt(selectedNegeri), kodBandar);

                txtAlamat3.Text = $"{bandarName} {negeriName}";

                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        List<AgensiSerahanDto> listagensiserahan;
        private List<AgensiSerahanDto> _agensiSerahanSelected;

        private void ShowAgensiTerlibat()
        {
            listagensiserahan = MasterDataBll.GetAgensiSerahan();
            var agensiSerahanSelected = new List<AgensiSerahanDto>();
            var newSerahanSelected = new List<AgensiSerahanDto>();

            if (_agensiSerahanSelected.Any())
            {
                foreach (var item in listagensiserahan)
                {
                    item.IsSelected = _agensiSerahanSelected.Any(m => m.kodserahagensi == item.kodserahagensi && m.prgn == item.prgn);
                    if (item.IsSelected)
                    {
                        agensiSerahanSelected.Add(item);
                    }
                }
            }

            var listFiltered = listagensiserahan;


            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);
            builder.SetCancelable(false);
            builder.SetButton2(Constants.Messages.Yes, (c, ev) =>
            {
                if (agensiSerahanSelected.Any())
                {
                    txtAgensiSerahan.Text = string.Join(", ",
                        listagensiserahan.Where(m => agensiSerahanSelected.Any(x => m.kodserahagensi == x.kodserahagensi && m.prgn == x.prgn)).Select(m => m.prgn)
                            .ToArray());
                }
                else
                {
                    txtAgensiSerahan.Text = "";
                }



                _agensiSerahanSelected = new List<AgensiSerahanDto>();
                foreach (var i in agensiSerahanSelected)
                {
                    _agensiSerahanSelected.Add(i);
                }

                SetPrintButton();

                builder.Dismiss();
            });
            builder.SetButton(Constants.Messages.No, (c, ev) =>
            {

                agensiSerahanSelected = new List<AgensiSerahanDto>();
                foreach (var i in _agensiSerahanSelected)
                {
                    agensiSerahanSelected.Add(i);
                }
                SetPrintButton();
                builder.Dismiss();
            });
            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Agensi Terlibat";

            listView.Adapter = new CarianAgensiSerahanMultipleAdapter(this.Activity, listagensiserahan);

            txtCarian.TextChanged += (send, args) =>
            {
                listFiltered = listagensiserahan
                    .Where(m => m.prgn.ToLower().Contains(txtCarian.Text.ToLower())).ToList();

                if (agensiSerahanSelected.Any())
                {
                    foreach (var item in listFiltered)
                    {
                        item.IsSelected = agensiSerahanSelected.Any(m => m.kodserahagensi == item.kodserahagensi && m.prgn == item.prgn);
                    }
                }

                listView.Adapter = new CarianAgensiSerahanMultipleAdapter(this.Activity, listFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                listFiltered[args.Position].IsSelected = !listFiltered[args.Position].IsSelected;

                if (agensiSerahanSelected.Any(m => m.kodserahagensi == listFiltered[args.Position].kodserahagensi && m.prgn == listFiltered[args.Position].prgn))
                {
                    agensiSerahanSelected.Remove(listFiltered[args.Position]);
                    if (newSerahanSelected.Any(c => c.kodserahagensi == listFiltered[args.Position].kodserahagensi && c.prgn == listFiltered[args.Position].prgn))
                    {
                        newSerahanSelected.Remove(listFiltered[args.Position]);
                    }
                }
                else
                {
                    agensiSerahanSelected.Add(listFiltered[args.Position]);
                    newSerahanSelected.Add(listFiltered[args.Position]);
                }

                listView.InvalidateViews();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private void ShowJenisNiaga()
        {
            if (listOfJenisNiaga == null)
                listOfJenisNiaga = MasterDataBll.GetAllJenisPerniagaan();

            var listOfJenisNiagaFiltered = listOfJenisNiaga;


            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);

            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Jenis Perniagaan";

            listView.Adapter = new CarianJenisNiagaAdapter(this.Activity, listOfJenisNiaga);

            txtCarian.TextChanged += (send, args) =>
            {
                listOfJenisNiagaFiltered = listOfJenisNiaga
                    .Where(m => m.Prgn.ToLower().Contains(txtCarian.Text.ToLower())).ToList();

                listView.Adapter = new CarianJenisNiagaAdapter(this.Activity, listOfJenisNiagaFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                txtJenisNiaga.Text = listOfJenisNiagaFiltered[args.Position]?.Prgn;
                _jenisNiaga = listOfJenisNiagaFiltered[args.Position] != null
                    ? listOfJenisNiagaFiltered[args.Position].KodJenis
                    : 0;

                SetPrintButton();
                builder.Dismiss();
                ShowJenamaStesenMinyak();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                SetPrintButton();
                builder.Dismiss();
            };

            builder.Show();
        }

        private void ShowJenamaStesenMinyak()
        {
            if (txtJenisNiaga.Text.Contains("STESEN MINYAK"))
            {
                linearJenamaStesen.Visibility = ViewStates.Visible;
            }
            else
            {
                spJenamaStesenMinyak.SetSelection(0);
                linearJenamaStesen.Visibility = ViewStates.Gone;
            }
        }

        private void BtnLokasi_Click(object sender, EventArgs e)
        {
            var kodCawangan = GeneralBll.GetUserCawangan();

            if (listOfPremis == null)
                listOfPremis = PremisBll.GetPremisForListByKodCawangan(kodCawangan);
            var listOfPremisFiltered = listOfPremis;

            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);

            txtCarian = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);

            listView.Adapter = new CarianPremisAdapter(this.Activity, listOfPremis);

            txtCarian.TextChanged += (send, args) =>
            {
                listOfPremisFiltered = listOfPremis.Where(m =>
                    m.Nama.ToLower().Contains(txtCarian.Text.ToLower()) ||
                    m.Alamat1.ToLower().Contains(txtCarian.Text.ToLower())).ToList();
                listView.Adapter = new CarianPremisAdapter(this.Activity, listOfPremisFiltered);
            };

            listView.ItemClick += (send, args) =>
            {
                txtLokasi.Text = listOfPremisFiltered[args.Position]?.Nama;
                txtNamaPremis.Text = listOfPremisFiltered[args.Position]?.Nama;
                txtAlamat1.Text = listOfPremisFiltered[args.Position]?.Alamat1;
                txtAlamat2.Text = listOfPremisFiltered[args.Position]?.Alamat2;
                txtAlamat3.Text = listOfPremisFiltered[args.Position]?.Alamat3;

                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private void TxtNoKpPenerima_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (txtNoKpPenerima.Text.Contains("-"))
                {
                    txtNoKpPenerima.Text = txtNoKpPenerima.Text.Replace("-", "");
                    GeneralAndroidClass.ShowToast("Karakter (-) Tidak Dibenarkan Di ruangan No. K/P ");
                    txtNoKpPenerima.SetSelection(txtNoKpPenerima.Length());
                }
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "Event_CheckMandatory_Dropdown_Edittext", ex.Message,
                    Enums.LogType.Error);
            }
        }



        #region Button Click

        private void SetButtonTindakan()
        {
            btnNote.SetBackgroundResource(Resource.Drawable.catatandisable_icon);
            btnNote.Enabled = false;

            var tindakan = (Enums.Tindakan)spTindakan.SelectedItemPosition - 1;
            if (_isSaved && (tindakan == Enums.Tindakan.Kots || tindakan == Enums.Tindakan.SiasatLanjutan))
            {
                btnNote.SetBackgroundResource(Resource.Drawable.catatan_icon);
                btnNote.Enabled = true;
            }

            //btnNote.SetBackgroundResource(Resource.Drawable.catatan_icon);
            //btnNote.Enabled = true;
        }

        private void BtnNote_Click(object sender, EventArgs e)
        {
            try
            {
                var tindakan = (Enums.Tindakan)spTindakan.SelectedItemPosition - 1;

                if (_isSaved && tindakan == Enums.Tindakan.Kots)
                {

                    _dialog = GeneralAndroidClass.ShowProgressDialog(this.Activity, Constants.Messages.WaitingPlease);
                    new Thread(() =>
                    {

                        Thread.Sleep(1000);
                        this.Activity.RunOnUiThread(CheckKompaunIzin);
                    }).Start();


                }
                else if (_isSaved && tindakan == Enums.Tindakan.SiasatLanjutan)
                {
                    _dialog = GeneralAndroidClass.ShowProgressDialog(this.Activity, Constants.Messages.WaitingPlease);
                    new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        this.Activity.RunOnUiThread(ShowSiasatanLanjut);
                    }).Start();
                }



            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "BtnNote_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void ShowSiasatanLanjut()
        {
            //_isTindakanClick = true;
            _activeForm = Enums.ActiveForm.SiasatLanjutan;

            var intent = new Intent(this.Activity, typeof(SiasatLanjutan));
            intent.PutExtra("NoRujukanKpp", lblNoKpp.Text);
            StartActivity(intent);
            _dialog?.Dismiss();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {

                if (ValidateData())
                {
                    if (_isSaved)
                    {
                        var ad = GeneralAndroidClass.GetDialogCustom(this.Activity);

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
                        _reprint = false;
                        ShowConfirmModal();
                    }

                }



            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "BtnPrint_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnCamera_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(lblNoKpp.Text))
                {
                    GeneralAndroidClass.ShowModalMessage(this.Activity, "No Rujukan Kosong");
                    return;
                }
                _activeForm = Enums.ActiveForm.Camera;
                var intent = new Intent(this.Activity, typeof(Activities.Camera));
                intent.PutExtra("filename", lblNoKpp.Text);
                //intent.PutExtra("allowtakepicture", true);
                if (!IsAllowTakePicture())
                {
                    intent.PutExtra("allowtakepicture", false);
                }

                if (_isSaved)
                {
                    intent.PutExtra("allowtakepicture", false);
                    intent.PutExtra("allowreplace", false);
                }

                StartActivity(intent);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "BtnCamera_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                //var ad = GeneralAndroidClass.GetDialogCustom(this.Activity);

                //ad.SetMessage("Anda pasti untuk tamatkan lawatan ini?");
                //// Positive

                //ad.SetButton("Tidak", (s, ev) => { });
                //ad.SetButton2("Ya", (s, ev) =>
                //{

                //    CheckTamatCondition();

                //});
                //ad.Show();
                CheckTamatCondition();

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "BtnOk_Click", ex.Message, Enums.LogType.Error);
            }
        }

        #endregion

        private void LoadDataDropdown()
        {
            ListKategoryPremis = PemeriksaanBll.GetAllKategoriPremis();
            spKategoryPremis.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item, ListKategoryPremis.Select(c => c.Value).ToList());

            ListKategoriKawasan = PemeriksaanBll.GetAllKategoriKawasan();
            spKategoryKawasan.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item,
                ListKategoriKawasan.Select(c => c.Value).ToList());

            ListTujuanLawatan = PemeriksaanBll.GetAllTujuanLawatan();
            //spTujuanLawatan.Adapter = new ArrayAdapter<string>(this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, ListTujuanLawatan.Select(c => c.Value).ToList());
            //spTujuanLawatan.ItemSelected += SpTujuanLawatan_ItemSelected;

            ListTindakan = PemeriksaanBll.GetAllTindakan();
            spTindakan.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item, ListTindakan.Select(c => c.Value).ToList());

            spTindakan.ItemSelected += SpTindakan_ItemSelected;

            ListNegeri = MasterDataBll.GetAllNegeri();
            spNegeri.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item, ListNegeri.Select(c => c.Value).ToList());

            spNegeri.ItemSelected += SpNegeri_ItemSelected;

            ListKategoriPerniagaan = MasterDataBll.GetAllKategoriPerniagaan();
            spKategoriPerniagaan.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item, ListKategoriPerniagaan.Select(c => c.Value).ToList());

            ListStesenMinyak = MasterDataBll.GetJenamaStesenMinyak();
            spJenamaStesenMinyak.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item, ListStesenMinyak.Select(c => c.Value).ToList());
            spJenamaStesenMinyak.ItemSelected += Event_CheckMandatory_Dropdown;

            ListWarganegara = PemeriksaanBll.GetKewarganegaraan();
            spKewarganegaraan.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item, ListWarganegara.Select(c => c.Value).ToList());

            spKategoryKawasan.ItemSelected += Event_CheckMandatory_Dropdown;
            spKategoryPremis.ItemSelected += Event_CheckMandatory_Dropdown;
            spKategoriPerniagaan.ItemSelected += Event_CheckMandatory_Dropdown;

            spKewarganegaraan.ItemSelected += spKewarganegaraan_ItemSelected;
        }

        private void spKewarganegaraan_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                relativeNoPassport.Visibility = ViewStates.Gone;
                var selectedPosition = spKewarganegaraan.SelectedItemPosition + 1;

                if (selectedPosition == Constants.Kewarganegaraan.BukanWarganegara)
                {
                    btnSearchJpn.Visibility = ViewStates.Invisible;
                }
                else
                {
                    btnSearchJpn.Visibility = ViewStates.Visible;
                }

                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "spKewarganegaraan_ItemSelected", ex.Message,
                    Enums.LogType.Error);
            }
        }

        private void SpNegeri_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                txtBandar.Text = "";
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "SpNegeri_ItemSelected", ex.Message,
                    Enums.LogType.Error);
            }
        }

        private void SpTindakan_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                chkAmaran.Checked = false;
                chkAmaran.Enabled = false;

                chkBayar.Enabled = false;
                chkBayar.Checked = false;
                btnKesalahanKompaun.Enabled = false;
                btnKesalahanKompaun.SetBackgroundResource(Resource.Color.buttondisable);
                linearButtonKesalahan.Visibility = ViewStates.Gone;

                linearSiasatUlangan.Visibility = ViewStates.Gone;
                txtNoEP.Text = "";
                txtNoIP.Text = "";

                linearNotisSerahan.Visibility = ViewStates.Gone;

                var selectedPosition = spTindakan.SelectedItemPosition - 1;

                if (selectedPosition == Constants.Tindakan.TiadaKes)
                {
                    chkAmaran.Enabled = true;
                }
                else if (selectedPosition == Constants.Tindakan.Kots)
                {
                    chkBayar.Enabled = true;
                    chkBayar.Checked = true;
                    btnKesalahanKompaun.Enabled = true;
                    linearButtonKesalahan.Visibility = ViewStates.Visible;
                    SetPrintButton();
                    SetButtonKesalahan();
                }
                else if (selectedPosition == Constants.Tindakan.SiasatUlangan)
                {
                    linearSiasatUlangan.Visibility = ViewStates.Visible;
                }
                else if(selectedPosition ==  Constants.Tindakan.SerahanNotis) 
                {
                    linearNotisSerahan.Visibility = ViewStates.Visible;
                }

                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "SpTindakan_ItemSelected", ex.Message,
                    Enums.LogType.Error);
            }
        }

        private void SpTujuanLawatan_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {

                txtAsasTindakan.Text = "";
                _kodAsasSelected = new List<AsasTindakanDto>();
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "SpTujuanLawatan_ItemSelected", ex.Message,
                    Enums.LogType.Error);
            }
        }

        #region Update DateTime()

        //private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        this.Activity.RunOnUiThread(UpdateTime);
        //    }
        //    catch (Exception ex)
        //    {
        //        GeneralAndroidClass.LogData("Pemeriksaan", "Timer_Elapsed", ex.Message, Enums.LogType.Error);
        //    }

        //}

        //private void UpdateTime()
        //{
        //    var localDate = GeneralBll.GetLocalDateTime();
        //    txtTarikh.Text = localDate.ToString(Constants.DateFormatDisplay);
        //    txtMasa.Text = localDate.ToString(Constants.TimeFormatDisplay);
        //}

        #endregion

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
            view.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.WrapContent);
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
            if (spKategoryKawasan.SelectedItem == null)
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Kategory kawasan kosong.");
                return false;
            }


            if (string.IsNullOrEmpty(txtLokasi.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Lokasi kosong.");
                return false;
            }

            //if (spTujuanLawatan.SelectedItem == null)
            //{
            //    GeneralAndroidClass.ShowModalMessage(this.Activity, "Tujuan lawatan kosong.");
            //    return false;
            //}

            if (_kodAsasSelected == null || _kodAsasSelected.Count == 0)
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Asas tindakan kosong.");
                return false;
            }
            //if (_asasTindakan == 0)
            //{
            //    GeneralAndroidClass.ShowModalMessage(this.Activity, "Asas tindakan kosong.");
            //    return false;
            //}

            if (string.IsNullOrEmpty(txtHasilLawatan.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Hasil lawatan kosong.");
                return false;
            }

            if (spKategoryPremis.SelectedItem == null)
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Kategory premis kosong.");
                return false;
            }
            if (string.IsNullOrEmpty(txtNamaPremis.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Nama premis kosong.");
                return false;
            }
            if (string.IsNullOrEmpty(txtAlamat1.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Alamat premis kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(txtNamaPenerima.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Nama penerima kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(txtNoKpPenerima.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "No K/P kosong.");
                return false;
            }

            if (txtNoKpPenerima.Text.Contains("-"))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Not Allowed No K/P (-)");
                return false;
            }


            if (string.IsNullOrEmpty(txtAlamatPenerima1.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Alamat penerima kosong.");
                return false;
            }

            if (spTindakan.SelectedItemPosition == 0)
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Tindakan kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(btnTarikh.Text) || string.IsNullOrEmpty(btnMasa.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, "Tarikh/Masa tamat lawatan kosong.");
                return false;
            }
            var dtMula = GeneralBll.ConvertDatabaseFormatStringToDateTime(
                GeneralBll.ConvertDateDisplayToDatabase(btnTarikhMula.Text) + " " +
                GeneralBll.ConvertTimeDisplayToDatabase(btnMasaMula.Text));


            var dtTamat = GeneralBll.ConvertDatabaseFormatStringToDateTime(
                GeneralBll.ConvertDateDisplayToDatabase(btnTarikh.Text) + " " +
                GeneralBll.ConvertTimeDisplayToDatabase(btnMasa.Text));

            if (dtTamat <= dtMula)
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, Constants.ErrorMessages.DateEndEqualLessDateStart);
                return false;
            }

            //var kodTujuan = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListTujuanLawatan, spTujuanLawatan.SelectedItem?.ToString() ?? ""));
            //if (kodTujuan == Constants.DefaultTujuanLawatan)
            //{
            //    if (string.IsNullOrEmpty(txtNoAduan.Text))
            //    {
            //        GeneralAndroidClass.ShowModalMessage(this.Activity, "No aduan kosong.");
            //        return false;
            //    }
            //}

            return true;
        }

        private void SaveData()
        {
            try
            {
                #region Save Data

                var data = new TbKpp
                {
                    KodCawangan = GeneralBll.GetUserCawangan(),
                    NoRujukanKpp = lblNoKpp.Text,
                    IdHh = GeneralBll.GetUserHandheld(),
                    KodKatPremis = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListKategoryPremis,
                        spKategoryPremis.SelectedItem?.ToString() ?? "")),
                    KodJenis = _jenisNiaga,
                    NamaPremis = txtNamaPremis.Text,
                    AlamatPremis1 = txtAlamat1.Text,
                    AlamatPremis2 = txtAlamat2.Text,
                    AlamatPremis3 = txtAlamat3.Text,
                    NoDaftarPremis = txtNoDaftarSyarikat.Text,
                    NoLesenBKP_PDA = txtNoLesenBkPda.Text,
                    NoLesenMajlis_Permit = txtNoLesenMajelisPremis.Text,
                    NoTelefonPremis = txtNoTelefon.Text,
                    Amaran = chkAmaran.Checked ? Constants.Amaran.Yes : Constants.Amaran.No,
                    KodKatKawasan =
                        GeneralBll.GetKeySelected(ListKategoriKawasan,
                            spKategoryKawasan.SelectedItem?.ToString() ?? ""),
                    LokasiLawatan = txtLokasi.Text,
                    NoRujukanAtr = txtNoRujukanAtr.Text,
                    //KodTujuan = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListTujuanLawatan,
                    //    spTujuanLawatan.SelectedItem?.ToString() ?? "")),
                    KodTujuan = 0,
                    //KodAsas = _asasTindakan,
                    NoAduan = txtNoAduan.Text,
                    CatatanLawatan = txtCatatanLawatan.Text,
                    HasilLawatan = txtHasilLawatan.Text,
                    NamaPenerima = txtNamaPenerima.Text,
                    NoKpPenerima = txtNoKpPenerima.Text,
                    Jawatanpenerima = txtJawatanPenerima.Text,
                    AlamatPenerima1 = txtAlamatPenerima1.Text,
                    AlamatPenerima2 = txtAlamatPenerima2.Text,
                    AlamatPenerima3 = txtAlamatPenerima3.Text,
                    CatatanPremis = txtLainLain.Text,
                    SetujuByr = chkBayar.Checked ? Constants.SetujuBayar.Yes : Constants.SetujuBayar.No,
                    //TrkhTamatLawatanKpp = GeneralBll.ConvertDateDisplayToDatabase(btnTarikh.Text) + " " + GeneralBll.ConvertTimeDisplayToDatabase(btnMasa.Text),
                    NoEp = txtNoEP.Text,
                    NoIp = txtNoIP.Text,
                };

                if (_kodAsasSelected != null && _kodAsasSelected.Count > 0)
                {
                    data.KodAsas = _kodAsasSelected.FirstOrDefault()?.KodAsas ?? 0;
                }

                var trkhDatabase = GeneralBll.ConvertDateDisplayToDatabase(btnTarikh.Text);
                var trkhTime = GeneralBll.ConvertTimeDisplayToDatabase(btnMasa.Text);
                data.TrkhTamatLawatanKpp = trkhDatabase + " " + trkhTime;

                data.Tindakan = spTindakan.SelectedItemPosition - 1;

                if (data.Tindakan == Constants.Tindakan.Kots)
                {
                    //kesalahan
                    data.JenisPesalah = Constants.JenisPesalah.Individu;
                    if (_rdSyarikat)
                        data.JenisPesalah = Constants.JenisPesalah.Syarikat;

                    data.KodAkta = _selectedAkta;
                    data.KodSalah = _kodSalah;
                    data.ButirSalah = _butirKesalahan;
                    data.IsArahanSemasa = _chkArahan ? Constants.ArahanSemasa.Yes : Constants.ArahanSemasa.No;
                    data.TempohTawaran = GeneralBll.ConvertStringToInt(_tempohTawaran);
                    data.AmnKmp = GeneralBll.ConvertStringToDecimal(_amaunTawaran);
                }

                //save data cr
                if (_lokalitiSelected != null && _lokalitiSelected.Count > 0) 
                {
                    data.lokalitikategorikhas = _lokalitiSelected.FirstOrDefault()?.Id ?? 0;
                }
                if (_agensiSerahanSelected != null && _agensiSerahanSelected.Count > 0)
                {
                    data.kodagensiterlibat = _agensiSerahanSelected.FirstOrDefault()?.kodserahagensi ?? "";
                }
                data.kodkatperniagaan = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListKategoriPerniagaan, spKategoriPerniagaan.SelectedItem?.ToString() ?? null));
                data.kodjenama = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListStesenMinyak, spJenamaStesenMinyak.SelectedItem?.ToString() ?? ""));
                data.kewarganegaraan = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListWarganegara, spKewarganegaraan.SelectedItem?.ToString() ?? ""));
                //data.nopassport = txtNoPassport.Text;
                data.nb = chkNB.Checked ? Constants.NotisBertulis.Yes : Constants.NotisBertulis.No;
                data.npmb = chkNPMB.Checked ? Constants.NotisPengesahanMaklumatBarang.Yes : Constants.NotisPengesahanMaklumatBarang.No;

                data.PengeluarKpp = GeneralBll.GetUserStaffId();
                data.PgnDaftar = data.PengeluarKpp;
                data.PgnAkhir = data.PengeluarKpp;
                data.Status = Constants.Status.Aktif;

                var dateStart = GeneralBll.ConvertDateDisplayToDatabase(btnTarikhMula.Text);
                var timeStart = GeneralBll.ConvertTimeDisplayToDatabase(btnMasaMula.Text);

                data.TrkhMulaLawatankpp = $"{dateStart} {timeStart}";

                data.TrkhPenerima = data.TrkhMulaLawatankpp;

                data.TrkhDaftar = data.TrkhMulaLawatankpp;
                data.TrkhAkhir = data.TrkhMulaLawatankpp;

                #endregion

                if (PemeriksaanBll.SavePemeriksaanTrx(data, _kodAsasSelected, _lokalitiSelected, _agensiSerahanSelected))
                {
                    _isSaved = true;
                    SetButtonTindakan();
                    SetEnableControl(false);
                    SetMenuDrawer(false);
                    _dialog?.Dismiss();

                    //#if DEBUG
                    //                    Print(false);
                    //#else
                    //Thread.Sleep(2000);
                    //GeneralAndroidClass.ShowToast("Sedang cetak");
                    //                    _dialog = GeneralAndroidClass.ShowProgressDialog(this.Activity, Constants.Messages.PrintWaitMessage);
                    Print(true);
                    //#endif
                }
                else
                {
                    //GeneralAndroidClass.ShowModalMessage(this.Activity, "Gagal saved data : " + result.Message);
                    GeneralAndroidClass.ShowModalMessage(this.Activity, Constants.ErrorMessages.FailedSaveData);
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SaveData", ex.Message, Enums.LogType.Error);
            }


            _dialog?.Dismiss();
        }

        private void SetMenuDrawer(bool enable)
        {
            //NavigationView navigationView = View.FindViewById<NavigationView>(Resource.Id.nav_view);
            //if (navigationView != null)
            //{
            //    navigationView.Enabled = enable;
            //}
            ((MainActivity)Activity).SetMenuDrawer(enable);
        }

        private void PrepareSendDataOnline()
        {
            _dialog?.Dismiss();
            if (_reprint) return;
            if (!GeneralAndroidClass.IsOnline())
                return;

            var tindakan = (Enums.Tindakan)(spTindakan.SelectedItemPosition - 1);
            if (tindakan != Enums.Tindakan.Kots)
                return;

            _dialog = GeneralAndroidClass.ShowProgressDialog(this.Activity, Constants.Messages.SendDataOnline);
            new Thread(() =>
            {
                Thread.Sleep(1000);
                this.Activity.RunOnUiThread(SendDataOnline);
            }).Start();
        }

        private void SendDataOnline()
        {
            var result = Task.Run(async () =>
                await SendOnlineBll.SendDataOnlineAsync(lblNoKpp.Text, Enums.TableType.KPP, this.Context)).Result;
            if (result.Success)
            {
                GeneralAndroidClass.ShowToast(Constants.Messages.SuccessSendData);
                _dialog?.Dismiss();
            }
            else
            {
                _dialog?.Dismiss();

                var tbSendOnline = MasterDataBll.GetTbSendOnlineByRujukanAndType(lblNoKpp.Text, Enums.TableType.KPP);
                if (tbSendOnline != null && tbSendOnline.Status == Enums.StatusOnline.Sent)
                {
                    return;
                }

                var alertDialog = GeneralAndroidClass.GetDialogCustom(this.Activity);
                alertDialog.SetTitle(Constants.Messages.SendData);
                alertDialog.SetMessage(Constants.Messages.ReSendData);
                alertDialog.SetCanceledOnTouchOutside(false);
                alertDialog.SetButton2(Constants.Messages.Yes, (c, ev) =>
                {
                    alertDialog.Dismiss();
                    _dialog = GeneralAndroidClass.ShowProgressDialog(this.Activity, Constants.Messages.SendDataOnline);
                    new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        this.Activity.RunOnUiThread(SendDataOnline);
                    }).Start();

                });
                alertDialog.SetButton(Constants.Messages.No, (c, ev) =>
                {
                    alertDialog.Dismiss();
                    SendOnlineBll.SetStatusDataOnline(lblNoKpp.Text, Enums.TableType.KPP, Enums.StatusOnline.Error);

                });
                //alertDialog.DismissEvent += (send, args) =>
                //{
                //    alertDialog.Dismiss();
                //    SendOnlineBll.SetStatusDataOnline(lblNoKpp.Text, Enums.TableType.KPP, Enums.StatusOnline.Error);
                //};

                alertDialog.Show();

            }
        }

        private void SetEnableControl(bool blValue)
        {
            #region Lawatan

            btnTarikhMula.Enabled = blValue;
            btnMasaMula.Enabled = blValue;

            btnTarikh.Enabled = blValue;
            btnMasa.Enabled = blValue;

            spKategoryKawasan.Enabled = blValue;
            txtLokasi.Enabled = blValue;
            //spTujuanLawatan.Enabled = blValue;
            //spAsasTindakan.Enabled = blValue;
            txtNoAduan.Enabled = blValue;
            txtNoRujukanAtr.Enabled = blValue;
            txtCatatanLawatan.Enabled = blValue;
            txtHasilLawatan.Enabled = blValue;
            btnAsasTindakan.Enabled = blValue;
            btnLokasi.Enabled = blValue;

            //cr
            btnAgensiSerahan.Enabled = blValue;
            btnLokaliti.Enabled = blValue;
            #endregion

            #region Premis

            spKategoryPremis.Enabled = blValue;

            txtNamaPremis.Enabled = blValue;

            txtAlamat1.Enabled = blValue;
            txtAlamat2.Enabled = blValue;
            txtAlamat3.Enabled = blValue;
            txtNoDaftarSyarikat.Enabled = blValue;
            txtNoLesenBkPda.Enabled = blValue;
            txtNoLesenMajelisPremis.Enabled = blValue;
            txtNoTelefon.Enabled = blValue;
            txtLainLain.Enabled = blValue;
            chkAmaran.Enabled = blValue;

            btnJenisNiaga.Enabled = blValue;
            btnSearchSsm.Enabled = blValue;

            spNegeri.Enabled = blValue;
            btnBandar.Enabled = blValue;

            spKategoriPerniagaan.Enabled = blValue;
            spJenamaStesenMinyak.Enabled = blValue;
          
            #endregion

            #region Penerima

            txtNamaPenerima.Enabled = blValue;
            btnNamaPenerima.Enabled = blValue;
            txtNoKpPenerima.Enabled = blValue;
            txtJawatanPenerima.Enabled = blValue;
            txtAlamatPenerima1.Enabled = blValue;
            txtAlamatPenerima2.Enabled = blValue;
            txtAlamatPenerima3.Enabled = blValue;

            spTindakan.Enabled = blValue;
            chkBayar.Enabled = blValue;
            btnSearchJpn.Enabled = blValue;

            txtNoEP.Enabled = blValue;
            txtNoIP.Enabled = blValue;

            spKewarganegaraan.Enabled = blValue;
            txtNoPassport.Enabled = blValue;
            chkNB.Enabled = blValue;
            chkNPMB.Enabled = blValue;
            #endregion


            if (blValue)
            {
                #region Lawatan

                spKategoryKawasan.SetBackgroundResource(Resource.Drawable.spiner_bg);
                txtLokasi.SetBackgroundResource(Resource.Drawable.editText_bg);
                //spTujuanLawatan.SetBackgroundResource(Resource.Drawable.spiner_bg);
                txtNoAduan.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoRujukanAtr.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtCatatanLawatan.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtHasilLawatan.SetBackgroundResource(Resource.Drawable.editText_bg);

                //cr
                txtLokaliti.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAgensiSerahan.SetBackgroundResource(Resource.Drawable.editText_bg);
                #endregion

                #region Premis

                spKategoryPremis.SetBackgroundResource(Resource.Drawable.spiner_bg);

                txtNamaPremis.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamat1.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamat2.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamat3.SetBackgroundResource(Resource.Drawable.editText_bg);

                txtNoDaftarSyarikat.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoLesenBkPda.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoLesenMajelisPremis.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoTelefon.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtLainLain.SetBackgroundResource(Resource.Drawable.editText_bg);

                spNegeri.SetBackgroundResource(Resource.Drawable.spiner_bg);

                //cr
                spKategoriPerniagaan.SetBackgroundResource(Resource.Drawable.spiner_bg);
                spJenamaStesenMinyak.SetBackgroundResource(Resource.Drawable.spiner_bg);
                #endregion

                #region Penerima

                txtNamaPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoKpPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtJawatanPenerima.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima1.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima2.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamatPenerima3.SetBackgroundResource(Resource.Drawable.editText_bg);

                spTindakan.SetBackgroundResource(Resource.Drawable.spiner_bg);

                txtNoEP.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoIP.SetBackgroundResource(Resource.Drawable.editText_bg);

                spKewarganegaraan.SetBackgroundResource(Resource.Drawable.spiner_bg);
                txtNoPassport.SetBackgroundResource(Resource.Drawable.editText_bg);
                chkNB.SetBackgroundResource(Resource.Drawable.editText_bg);
                chkNPMB.SetBackgroundResource(Resource.Drawable.editText_bg);
                #endregion


            }
            else
            {
                #region Lawatan

                spKategoryKawasan.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtLokasi.SetBackgroundResource(Resource.Drawable.textView_bg);
                //spTujuanLawatan.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoAduan.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoRujukanAtr.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtCatatanLawatan.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtHasilLawatan.SetBackgroundResource(Resource.Drawable.textView_bg);

                //cr
                txtLokaliti.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAgensiSerahan.SetBackgroundResource(Resource.Drawable.textView_bg);
                #endregion

                #region Premis

                spKategoryPremis.SetBackgroundResource(Resource.Drawable.textView_bg);

                txtNamaPremis.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamat1.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamat2.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamat3.SetBackgroundResource(Resource.Drawable.textView_bg);

                txtNoDaftarSyarikat.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoLesenBkPda.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoLesenMajelisPremis.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoTelefon.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtLainLain.SetBackgroundResource(Resource.Drawable.textView_bg);

                spNegeri.SetBackgroundResource(Resource.Drawable.textView_bg);

                //cr
                spKategoriPerniagaan.SetBackgroundResource(Resource.Drawable.textView_bg);
                spJenamaStesenMinyak.SetBackgroundResource(Resource.Drawable.textView_bg);
                #endregion

                #region Penerima

                txtNamaPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoKpPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtJawatanPenerima.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima1.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima2.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamatPenerima3.SetBackgroundResource(Resource.Drawable.textView_bg);

                spTindakan.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoEP.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoIP.SetBackgroundResource(Resource.Drawable.textView_bg);

                spKewarganegaraan.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoPassport.SetBackgroundResource(Resource.Drawable.textView_bg);
                chkNB.SetBackgroundResource(Resource.Drawable.textView_bg);
                chkNPMB.SetBackgroundResource(Resource.Drawable.textView_bg);
                #endregion

            }

        }

        private void ShowTamatDialog()
        {
            var ad = GeneralAndroidClass.GetDialogCustom(this.Activity);
            ad.SetMessage(Constants.Messages.FinishLawatan);
            ad.SetButton(Constants.Messages.No, (s, ev) => { });
            ad.SetButton2(Constants.Messages.Yes, (s, ev) =>
            {
                SetNewPemeriksaan();
            });
            ad.Show();
        }

        private void CheckTamatCondition()
        {

            var tindakan = (Enums.Tindakan)(spTindakan.SelectedItemPosition - 1);

            if (!_isSaved || tindakan == Enums.Tindakan.TiadaKes || tindakan == Enums.Tindakan.SiasatUlangan || tindakan == Enums.Tindakan.SerahanNotis)
            {
                ShowTamatDialog();
                return;
            }

            if (tindakan == Enums.Tindakan.Kots)
            {
                var kompaun = PemeriksaanBll.GetKompaunByRujukanKpp(lblNoKpp.Text);
                if (kompaun != null)
                {
                    ShowTamatDialog();
                }
                else
                {
                    var kompaunIzin = PemeriksaanBll.GetKompaunIzinByRujukanKpp(lblNoKpp.Text);

                    if (kompaunIzin?.Status == Enums.StatusIzinKompaun.Approved)
                    {
                        GeneralAndroidClass.ShowModalMessage(this.Activity, Constants.ErrorMessages.KompaunNotCompleted);
                    }
                    else
                    {
                        ShowTamatDialog();
                    }



                    //if (kompaunIzin != null)
                    //{
                    //    if (kompaunIzin.Status == Enums.StatusIzinKompaun.Waiting ||
                    //        kompaunIzin.Status == Enums.StatusIzinKompaun.Denied)
                    //    {
                    //        ShowTamatDialog();
                    //        return;
                    //    }
                    //}

                    //GeneralAndroidClass.ShowModalMessage(this.Activity, Constants.ErrorMessages.KompaunNotCompleted);
                }
            }
            else if (tindakan == Enums.Tindakan.SiasatLanjutan)
            {
                var siasatLanjut = PemeriksaanBll.GetSiasatLanjutByRujukanKpp(lblNoKpp.Text);
                if (siasatLanjut != null)
                {
                    ShowTamatDialog();
                }
                else
                {
                    GeneralAndroidClass.ShowModalMessage(this.Activity,
                        Constants.ErrorMessages.SiasatLanjutNotCompleted);
                }
            }


        }

        private void SetNewPemeriksaan()
        {
            SetMenuDrawer(true);

            SetEnableControl(true);

            _isSaved = false;
            _reprint = false;
            SetButtonTindakan();
            lblNoKpp.Text = GeneralBll.GenerateNoRujukan(Enums.PrefixType.KPP);

            #region Lawatan

            txtNoAduan.Text = "";
            txtNoRujukanAtr.Text = "";
            txtCatatanLawatan.Text = "";
            txtHasilLawatan.Text = "";

            //spKategoryKawasan.SetSelection(0);
            //txtLokasi.Text = "";
            //txtAsasTindakan.Text = "";

            var localDate = GeneralBll.GetLocalDateTime().ToString(Constants.DateFormatDisplay);
            var localTime = GeneralBll.GetLocalDateTime().ToString(Constants.TimeFormatDisplay);

            btnTarikhMula.Text = localDate;
            btnMasaMula.Text = localTime;

            btnTarikh.Text = localDate;
            btnMasa.Text = localTime;

            //cr
            txtLokaliti.Text = "";
            txtAgensiSerahan.Text = "";
            #endregion

            #region Premis

            if (ListKategoriKawasan.Count > 0)
                spKategoryPremis.SetSelection(0);
            txtNamaPremis.Text = "";
            txtAlamat1.Text = "";
            txtAlamat2.Text = "";
            txtAlamat3.Text = "";
            txtNoDaftarSyarikat.Text = "";
            txtNoLesenBkPda.Text = "";
            txtNoLesenMajelisPremis.Text = "";
            txtNoTelefon.Text = "";
            txtLainLain.Text = "";

            spNegeri.SetSelection(0);
            //_jenisNiaga = 0;
            txtJenisNiaga.Text = "";

            //cr
            spKategoriPerniagaan.SetSelection(0);
            spJenamaStesenMinyak.SetSelection(0);
            linearJenamaStesen.Visibility = ViewStates.Gone;
            #endregion

            #region Penerima

            txtNamaPenerima.Text = "";
            txtNoKpPenerima.Text = "";
            txtJawatanPenerima.Text = "";
            txtAlamatPenerima1.Text = "";
            txtAlamatPenerima2.Text = "";
            txtAlamatPenerima3.Text = "";

            //rdTiadaKes.Checked = true;
            spTindakan.SetSelection(0);

            #endregion

            _kesalahan = "";
            _butirKesalahan = "";
            _amaunTawaran = "";
            _tempohTawaran = "";
            _chkArahan = false;
            _rdIndividu = false;
            _rdSyarikat = false;
            _selectedAkta = "";
            _kodSalah = 0;

            txtNoEP.Text = "";
            txtNoIP.Text = "";
            linearSiasatUlangan.Visibility = ViewStates.Gone;

            txtNoPassport.Text = "";
            chkNB.Checked = false;
            chkNPMB.Checked = false;

            SetPrintButton();
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
                    GeneralAndroidClass.LogData(LayoutName, "PreparePrinterDevice",
                        Constants.ErrorMessages.PrinterNotFound, Enums.LogType.Info);
                    new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        Thread.CurrentThread.IsBackground = true;
                        Activity.RunOnUiThread(PrepareSendDataOnline);
                    }).Start();

                }
                else
                {
                    if (GlobalClass.BluetoothDevice == null)
                    {
                        lvResult = new ListView(this.Activity);
                        var adapter = new DeviceListAdapter(this.Activity, GlobalClass.BluetoothAndroid._listDevice);
                        lvResult.Adapter = adapter;
                        lvResult.ItemClick += lvResult_ItemClick;

                        AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);
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
                    //IsLoading(this.Activity, false);

                    string BluetoothName = GlobalClass.BluetoothDevice.Name;
                    //GeneralAndroidClass.ShowToast("Printer Dipilih : " + BluetoothName);
                    GeneralAndroidClass.LogData(LayoutName, "Print using Device : ", BluetoothName, Enums.LogType.Debug);
                    if (BluetoothName == Constants.BixolonBluetoothName)
                    {
                        OnPrintingBixolon();
                    }
                    else
                    {
                        //RunOnUiThread(() => GetFWCode()) ;
                        GetFWCode();
                        OnPrinting();
                        IsLoading(this.Activity, false);
                    }
                }
                catch (Exception ex)
                {
                    IsLoading(this.Activity, false);
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
            readonly Pemeriksaan activity;

            public ServicetHandler(Pemeriksaan activity)
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
                    Toast.MakeText(this.Activity, $"Sambungan ke{deviceName}.", ToastLength.Short).Show();
                    this.Activity.InvalidateOptionsMenu();
                    break;
                case MESSAGE_TOAST:
                    Toast.MakeText(this.Activity, msg.Arg1, ToastLength.Short).Show();
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
                        Toast.MakeText(this.Activity, "Sila cuba sekali lagi", ToastLength.Short).Show();
                        await ShowMessageNew(false, "");
                        return;
                    }
                    else
                        GeneralAndroidClass.RegisterPrinter(strAddress);
                }

                await ShowMessageNew(true, Constants.Messages.GenerateBitmap);

                var printImageBll = new PrintImageBll();
                var bitmap = printImageBll.Pemeriksaan(this.Activity, lblNoKpp.Text);

                await ShowMessageNew(true, Constants.Messages.ConnectionToBluetooth);


                IsLoading(this.Activity, true, Constants.Messages.ConnectionToBluetooth);

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
                PrepareSendDataOnline();
            }

        }

        private void GetFWCode()
        {
            string strAddress = "";
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
            IsLoading(this.Activity, value, message);
            await Task.Delay(Constants.DefaultWaitingMilisecond);
        }

        #endregion

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                if (_activeForm == Enums.ActiveForm.Camera)
                {
                    _activeForm = Enums.ActiveForm.Pemeriksaan;
                    SetPrintButton();
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "OnResume", ex.Message, Enums.LogType.Error);
            }

        }

        private void SetButtonAfterTindakan()
        {
            //btnPrint.SetBackgroundResource(Resource.Drawable.printicon_disable);
            //btnPrint.Enabled = false;

            //btnNote.SetBackgroundResource(Resource.Drawable.catatandisable_icon);
            //btnNote.Enabled = false;
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
            if (string.IsNullOrEmpty(txtJenisNiaga.Text)) return false;
            return true;
#endif
            if (string.IsNullOrEmpty(spKategoryKawasan?.SelectedItem.ToString())) return false;
            if (string.IsNullOrEmpty(txtLokasi.Text)) return false;
            //var tujuanLawatan = spTujuanLawatan?.SelectedItem.ToString();
            //if (string.IsNullOrEmpty(tujuanLawatan)) return false;
            if (_kodAsasSelected == null || _kodAsasSelected.Count == 0) {return false;}
            if (txtAsasTindakan.Text.Contains("OPERASI BERSEPADU BERSAMA AGENSI") && (_agensiSerahanSelected == null || _agensiSerahanSelected.Count == 0)) {return false;}
            if (string.IsNullOrEmpty(txtHasilLawatan.Text)) return false;
            if (string.IsNullOrEmpty(spKategoryPremis.SelectedItem.ToString())) return false;
            //if (_jenisNiaga == 0) return false;
            if (string.IsNullOrEmpty(txtJenisNiaga.Text)){ return false; }
            if (string.IsNullOrEmpty(spKategoriPerniagaan.SelectedItem.ToString())) return false; 
            if (string.IsNullOrEmpty(spJenamaStesenMinyak.SelectedItem.ToString()) && txtJenisNiaga.Text.Contains("STESEN MINYAK")){return false;} 
            if (string.IsNullOrEmpty(txtNamaPremis.Text)) return false;
            if (string.IsNullOrEmpty(txtAlamat1.Text)) return false;
            if (string.IsNullOrEmpty(txtNamaPenerima.Text)) return false;
            if (string.IsNullOrEmpty(txtAlamatPenerima1.Text)) return false;
            if (spTindakan.SelectedItemPosition == 0) return false;
            var tindakan = (Enums.Tindakan)spTindakan.SelectedItemPosition - 1;
            if (tindakan == Enums.Tindakan.Kots)
            {
                if (string.IsNullOrEmpty(_selectedAkta)) return false;
                if (_kodSalah == 0) return false;
                if (string.IsNullOrEmpty(_butirKesalahan)) return false;
                if (!_rdIndividu && !_rdSyarikat) return false;
            }

            var countPhoto = GeneralBll.GetCountPhotoByRujukan(lblNoKpp.Text);

            if (tindakan != Enums.Tindakan.TiadaKes)
            {
                if (countPhoto < Constants.MinPhoto)
                    return false;
            }
            else
            {
                if (countPhoto < Constants.MinPhotoTiadaKes)
                    return false;
            }
            //if (tujuanLawatan.Trim().ToLower() != Constants.TujuanLawatanPermeriksaanBiasa)
            //{
            //    var countPhoto = GeneralBll.GetCountPhotoByRujukan(lblNoKpp.Text);
            //    if (countPhoto < Constants.MinPhoto) return false;
            //}


            return true;
        }

        private void Event_CheckMandatory_Dropdown(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "Event_CheckMandatory", ex.Message, Enums.LogType.Error);
            }

        }

        private void Event_CheckMandatory_Dropdown_Edittext(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "Event_CheckMandatory_Dropdown_Edittext", ex.Message, Enums.LogType.Error);
            }
        }

        private void ShowConfirmModal()
        {
            var dialogView = View.Inflate(this.Activity, Resource.Layout.ConfirmLayout, null);
            var alertDialog = new AlertDialog.Builder(this.Activity).Create();

            var lvResult = dialogView.FindViewById<ListView>(Resource.Id.lView);
            var lblTitle = dialogView.FindViewById<TextView>(Resource.Id.lblTitle);
            lblTitle.Text = "Pengesahan KPP";

            var listConfirm = GetListConfirm();

            lvResult.Adapter = new ConfirmListAdapter(this.Activity, listConfirm);
            lvResult.FastScrollEnabled = true;

            var btnCancel = dialogView.FindViewById<Button>(Resource.Id.btnCancel);
            btnCancel.Click += (sender, e) =>
            {
                alertDialog.Dismiss();
            };

            var btnCetak = dialogView.FindViewById<Button>(Resource.Id.btnCetak);
            btnCetak.Click += (sender, e) =>
            {
                _dialog = GeneralAndroidClass.ShowProgressDialog(this.Activity, Constants.Messages.SavingData);
                new Thread(() =>
                {
                    alertDialog.Dismiss();
                    Thread.Sleep(1000);
                    this.Activity.RunOnUiThread(SaveData);
                }).Start();
            };

            alertDialog.SetView(dialogView);
            alertDialog.Show();

        }

        private List<ConfirmDto> GetListConfirm()
        {
            var result = new List<ConfirmDto>();
            result.Add(GeneralBll.CreateConfirmDto("Lawatan", "", true));
            result.Add(GeneralBll.CreateConfirmDto("Kategori Kawasan", spKategoryKawasan.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Lokaliti/ Kategori Khas", txtLokaliti.Text));
            result.Add(GeneralBll.CreateConfirmDto("Lokasi", txtLokasi.Text));
            //result.Add(GeneralBll.CreateConfirmDto("Tujuan Lawatan", spTujuanLawatan.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Asas Tindakan", txtAsasTindakan.Text));
            if (txtAsasTindakan.Text.Contains("OPERASI BERSEPADU BERSAMA AGENSI"))
            {
                result.Add(GeneralBll.CreateConfirmDto("Agensi Serahan", txtAgensiSerahan.Text));
            }
            result.Add(GeneralBll.CreateConfirmDto("No. Aduan", txtNoAduan.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. Rujukan ATR", txtNoRujukanAtr.Text));
            result.Add(GeneralBll.CreateConfirmDto("Catatan Lawatan", txtCatatanLawatan.Text));
            result.Add(GeneralBll.CreateConfirmDto("Hasil Lawatan", txtHasilLawatan.Text));

            result.Add(GeneralBll.CreateConfirmDto("Premis", "", true));
            result.Add(GeneralBll.CreateConfirmDto("Kategori Premis", spKategoryPremis.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Kategori Perniagaan", spKategoriPerniagaan.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Jenis Perniagaan", txtJenisNiaga.Text));
            if(txtJenisNiaga.Text.Contains("STESEN MINYAK"))
            {
                result.Add(GeneralBll.CreateConfirmDto("Jenama Stesen Minyak", spJenamaStesenMinyak.SelectedItem?.ToString() ?? ""));
            }
            result.Add(GeneralBll.CreateConfirmDto("Nama Premis", txtNamaPremis.Text));
            var alamat = GeneralBll.GettOneAlamat(txtAlamat1.Text, txtAlamat2.Text, txtAlamat3.Text);
            result.Add(GeneralBll.CreateConfirmDto("Alamat Premis", alamat));

            result.Add(GeneralBll.CreateConfirmDto("No. Daftar Syarikat", txtNoDaftarSyarikat.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. Lesen BK/PDA", txtNoLesenBkPda.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. Lesen Majlis/Premis", txtNoLesenMajelisPremis.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. Telefon", txtNoTelefon.Text));
            result.Add(GeneralBll.CreateConfirmDto("Lain-lain", txtLainLain.Text));
            result.Add(GeneralBll.CreateConfirmDto("Amaran", chkAmaran.Checked ? "Ya" : "Tidak"));

            result.Add(GeneralBll.CreateConfirmDto("Penerima", "", true));
            result.Add(GeneralBll.CreateConfirmDto("Nama", txtNamaPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Kewarganegaraan", spKewarganegaraan.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("No. K/P", txtNoKpPenerima.Text));
            result.Add(GeneralBll.CreateConfirmDto("Jawatan", txtJawatanPenerima.Text));

            alamat = GeneralBll.GettOneAlamat(txtAlamatPenerima1.Text, txtAlamatPenerima2.Text, txtAlamatPenerima3.Text);
            result.Add(GeneralBll.CreateConfirmDto("Alamat", alamat));

            var tindakanName = "";
            var tindakan = (Enums.Tindakan)spTindakan.SelectedItemPosition - 1;
            if (tindakan == Enums.Tindakan.TiadaKes)
            {
                tindakanName = Constants.TindakanName.Pemeriksaan;
            }
            else if (tindakan == Enums.Tindakan.Kots)
            {
                tindakanName = Constants.TindakanName.KOTS;
            }
            else if (tindakan == Enums.Tindakan.SiasatLanjutan)
            {
                tindakanName = Constants.TindakanName.SiasatLanjut;
            }
            else if (tindakan == Enums.Tindakan.SiasatUlangan)
            {
                tindakanName = Constants.TindakanName.SiasatUlangan;
            }
            else if (tindakan == Enums.Tindakan.SerahanNotis)
            {
                tindakanName = Constants.TindakanName.SerahanNotis;
            }

            //if (rdTiadaKes.Checked) tindakan = "Tiada Kes";
            //else if (rdKots.Checked) tindakan = "KOTS";
            //else if (rdSiasatanLanjut.Checked) tindakan = "Siasatan Lanjut";

            result.Add(GeneralBll.CreateConfirmDto("Jika OKK setuju bayar", chkBayar.Checked ? "Ya" : "Tidak"));

            result.Add(GeneralBll.CreateConfirmDto("Tarikh tamat lawatan", btnTarikh.Text + " " + btnMasa.Text));

            result.Add(GeneralBll.CreateConfirmDto("Tindakan", tindakanName));


            if (tindakan == Enums.Tindakan.Kots)
            {
                result.Add(GeneralBll.CreateConfirmDto("Kesalahan", "", true));

                result.Add(GeneralBll.CreateConfirmDto("Jenis", _rdIndividu ? "Individu" : "Syarikat"));
                var aktaName = MasterDataBll.GetAktaName(_selectedAkta);
                result.Add(GeneralBll.CreateConfirmDto("Akta", aktaName));
                result.Add(GeneralBll.CreateConfirmDto("Kesalahan", _kesalahan));
                result.Add(GeneralBll.CreateConfirmDto("Butir Kesalahan", _butirKesalahan));
                result.Add(GeneralBll.CreateConfirmDto("Amaun Tawaran", _amaunTawaran));
                result.Add(GeneralBll.CreateConfirmDto("Arahan Semasa", _chkArahan ? "Ya" : "Tidak"));
                result.Add(GeneralBll.CreateConfirmDto("Tempoh Tawaran", _tempohTawaran));

            }
            else if (tindakan == Enums.Tindakan.SiasatUlangan)
            {
                result.Add(GeneralBll.CreateConfirmDto("No EP", txtNoEP.Text));
                result.Add(GeneralBll.CreateConfirmDto("No IP", txtNoIP.Text));
            }
            else if (tindakan == Enums.Tindakan.SerahanNotis)
            {
                result.Add(GeneralBll.CreateConfirmDto("Notis Bertulis", chkNB.Checked ? "Ya" : "Tidak"));
                result.Add(GeneralBll.CreateConfirmDto("Notis Pengesahan Maklumat Barang", chkNPMB.Checked ? "Ya" : "Tidak"));
            }
            return result;
        }

        private static int REQUEST_MYKAD = 1001;
        private static int REQUEST_MYKAD2 = 1002;

        private void BtnNamaPenerima_Click(object sender, EventArgs e)
        {
            try
            {      
                var model = Android.OS.Build.Model;
                Log.WriteLogFile( "\nModel: " + model, Enums.LogType.Info);
#if DEBUG
                model = "SM-A536E";
#endif

                if (model == "SM-A536E")
                {
                    var ad = GeneralAndroidClass.GetDialogCustom(this.Activity);

                    ad.SetMessage("Mengimbas menggunakan MyID Reader ? ");

                    ad.SetButton("Tidak", (s, ev) => { });
                    ad.SetButton2("Ya", (s, ev) =>
                    {
                        var intent = new Intent(Intent.ActionMain);
                        intent.SetComponent(new ComponentName("com.securemetric.myidreader", "com.securemetric.myidreader.MainActivity"));
                        StartActivityForResult(intent, REQUEST_MYKAD2);
                        SetPrintButton();
                    });
                    ad.Show();

                }
                else {

                    var intent = new Intent(Intent.ActionMain);
                    intent.SetComponent(new ComponentName("com.aimforce.mykad.woosim", "com.aimforce.mykad.woosim.MainActivity"));
                    intent.PutExtra("command", "no-ui");
                    intent.PutExtra("command2", "no-photo");
                    StartActivityForResult(intent, REQUEST_MYKAD);
                    SetPrintButton();
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "BtnNamaPenerima_Click", ex.Message, Enums.LogType.Error);
            }
        }
        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
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
                else if (requestCode == REQUEST_MYKAD2) 
                {
                    if (resultCode == Result.Ok) {

                        result = data.GetStringExtra("data");
                        var card = GeneralBll.ReadMyKadInfo2(result);
                        if (card.IsSuccessRead)
                            SetMyCard2(card);
                        else
                            GeneralAndroidClass.ShowToast(card.Message);

                    }
                }
                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pemeriksaan", "OnActivityResult", ex.Message, Enums.LogType.Error);
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

        private void SetMyCard2(CardInfoDto2 cardDto)
        {
            ClearMyCardField();

            var listAddress = new List<string>();

            var fullName = cardDto.gmpcName;
            fullName = string.Join(" ", fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            txtNamaPenerima.Text = fullName;
            txtNoKpPenerima.Text = cardDto.icNo;

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

        private void CheckKompaunIzin()
        {
            var data = PemeriksaanBll.CheckKompaunIzin(lblNoKpp.Text);

#if DEBUG
            data = new Result<TbKompaunIzin>
            {
                Success = true,
                Datas = new TbKompaunIzin
                {
                    Status = Enums.StatusIzinKompaun.Approved
                }
            };
            ShowKompaun(data.Datas.Catatan);
#endif

            if (data.Success)
            {
                if (data.Datas == null)
                {
                    var dataInsert = PemeriksaanBll.CreateDefaultKompaunIzin(lblNoKpp.Text, this.Context);
                    if (dataInsert.Success)
                    {
                        GeneralAndroidClass.ShowModalMessage(this.Activity, Constants.Messages.KompaunIzinWaiting);
                    }
                    else
                    {
                        if (GeneralBll.IsSkipControl() && dataInsert.Message == Constants.ErrorMessages.NoInternetConnection)
                        {
                            ShowSkipMessage(Constants.ErrorMessages.SkipNoInternetConnection);
                        }
                        else
                        {
                            GeneralAndroidClass.ShowModalMessage(this.Activity, "Error " + dataInsert.Message);
                        }


                    }

                }
                else
                {
                    //check other rellated data first
                    string message = "";
                    switch (data.Datas.Status)
                    {
                        case Enums.StatusIzinKompaun.Approved:
                            ShowKompaun(data.Datas.Catatan);
                            break;
                        case Enums.StatusIzinKompaun.Denied:
                            message = string.Format(Constants.Messages.KompaunIzinDenied, data.Datas.Catatan);
                            GeneralAndroidClass.ShowModalMessageHtml(this.Activity, message);
                            break;
                        default:
                            var service = KompaunBll.CheckServiceKompaunIzin(lblNoKpp.Text, this.Context);

                            if (service.Success)
                            {
                                switch (service.Result.Status)
                                {
                                    case Enums.StatusIzinKompaun.Approved:
                                        ShowKompaun(service.Result.Catatan);
                                        break;
                                    case Enums.StatusIzinKompaun.Denied:
                                        message = string.Format(Constants.Messages.KompaunIzinDenied, service.Result.Catatan);
                                        GeneralAndroidClass.ShowModalMessageHtml(this.Activity, message);
                                        break;
                                    default:
                                        if (GeneralBll.IsSkipControl() && IsKompaunIzinWaitingSkip())
                                        {
                                            ShowSkipMessage(string.Format(Constants.Messages.SkipMessage,
                                                Constants.MaxSkipWaitingInMinute));
                                        }
                                        else
                                        {
                                            GeneralAndroidClass.ShowModalMessage(this.Activity,
                                                Constants.Messages.KompaunIzinWaiting);
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                if (GeneralBll.IsSkipControl() && IsKompaunIzinWaitingSkip())
                                {
                                    ShowSkipMessage(string.Format(Constants.Messages.SkipMessage,
                                        Constants.MaxSkipWaitingInMinute));
                                }
                                else
                                {
                                    GeneralAndroidClass.ShowModalMessage(this.Activity, "Error " + service.Mesage);
                                }

                            }
                            break;
                    }
                }
            }
            else
            {
                //_dialog?.Dismiss();
                GeneralAndroidClass.ShowModalMessage(this.Activity, Constants.ErrorMessages.FailedCreateKompaunIzin);
            }
            _dialog?.Dismiss();

        }

        private bool IsKompaunIzinWaitingSkip()
        {
            var dtNow = GeneralBll.GetLocalDateTime();

            var data = KompaunBll.GetKompaunIzinByRujukanAndStatus(lblNoKpp.Text, Enums.StatusIzinKompaun.Waiting);
            if (data.Success && data.Datas != null)
            {
                var createdDate = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.Datas.TrkhDaftar);

                if ((dtNow - createdDate).TotalMinutes > Constants.MaxSkipWaitingInMinute)
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowSkipMessage(string message)
        {
            _isSkip = false;

            var ad = GeneralAndroidClass.GetDialogCustom(this.Activity);
            ad.DismissEvent += Ad_DismissEventSkip;
            ad.SetMessage(message);
            ad.SetButton("Tidak", (s, ev) =>
            {

            });
            ad.SetButton2("Ya", (s, ev) =>
            {
                _isSkip = true;
                PemeriksaanBll.UpdatePemeriksaanSkipIzin(lblNoKpp.Text, Constants.SkipIzin.Yes);
                ShowKompaunPage();
            });
            ad.Show();

        }

        private void Ad_DismissEventSkip(object sender, EventArgs e)
        {
            if (!_isSkip)
            {
                PemeriksaanBll.UpdatePemeriksaanSkipIzin(lblNoKpp.Text, Constants.SkipIzin.No);
            }
        }

        private void ShowKompaun(string catatan)
        {
            var message = string.Format(Constants.Messages.KompaunIzinApproved, catatan);

            var ad = GeneralAndroidClass.GetDialogCustom(this.Activity);

            ad.SetMessage(Html.FromHtml(message));
            ad.DismissEvent += Ad_DismissEvent;
            ad.SetButton2("OK", (s, ev) =>
            {

            });
            ad.Show();

        }

        private void Ad_DismissEvent(object sender, EventArgs e)
        {
            ShowKompaunPage();
        }

        private void ShowKompaunPage()
        {
            // _isTindakanClick = true;
            _activeForm = Enums.ActiveForm.Kompaun;
            var intent = new Intent(this.Activity, typeof(Kompaun));
            intent.PutExtra("JenisKmp", ((int)Enums.JenisKompaun.KOTS).ToString());
            intent.PutExtra("NoRujukanKpp", lblNoKpp.Text);
            intent.PutExtra("NoKpPenerima", txtNoKpPenerima.Text);

            StartActivity(intent);
        }

        private bool IsAllowTakePicture()
        {
            if (!_isSaved) return true;

            var tindakan = (Enums.Tindakan)spTindakan.SelectedItemPosition - 1;
            if (tindakan == Enums.Tindakan.Kots)
            {
                var kompaun = PemeriksaanBll.GetKompaunByRujukanKpp(lblNoKpp.Text);
                if (kompaun == null) return true;
            }

            if (tindakan == Enums.Tindakan.SiasatLanjutan)
            {
                var siasat = PemeriksaanBll.GetSiasatLanjutByRujukanKpp(lblNoKpp.Text);
                if (siasat == null) return true;
            }

            return false;
        }

        private EditText txtKesalahan, txtButirKesalahan, txtAmaunTawaran, txtTempohTawaran;
        private RadioButton rdIndividu, rdSyarikat;
        private CheckBox chkArahan;

        string _kesalahan, _butirKesalahan, _amaunTawaran, _tempohTawaran;
        bool _rdIndividu, _rdSyarikat, _chkArahan;
        private string _selectedAkta;


        private void ShowKompaunKesalahan()
        {
            var allowFilter = new FilterChar();

            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.KppKesalahan, null);
            builder.SetView(view);

            var listAkta = KompaunBll.GetAllAktaKots();
            var spAkta = view.FindViewById<Spinner>(Resource.Id.spAkta);

            spAkta.Adapter = new ArrayAdapter<string>(this.Activity, Resource.Layout.support_simple_spinner_dropdown_item,
                listAkta.Select(c => c.Value).ToList());

            //spAkta.ItemSelected += SpAkta_ItemSelected;

            var btnKesalahan = view.FindViewById<Button>(Resource.Id.btnKesalahan);

            txtKesalahan = view.FindViewById<EditText>(Resource.Id.txtKesalahan);
            txtButirKesalahan = view.FindViewById<EditText>(Resource.Id.txtButirKesalahan);
            txtAmaunTawaran = view.FindViewById<EditText>(Resource.Id.txtAmaunTawaran);
            txtTempohTawaran = view.FindViewById<EditText>(Resource.Id.txtTempohTawaran);
            rdIndividu = view.FindViewById<RadioButton>(Resource.Id.rdIndividu);
            rdSyarikat = view.FindViewById<RadioButton>(Resource.Id.rdSyarikat);
            chkArahan = view.FindViewById<CheckBox>(Resource.Id.chkArahan);

            txtAmaunTawaran.Enabled = false;
            txtTempohTawaran.Enabled = false;
            txtTempohTawaran.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(2) });

            txtButirKesalahan.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(1000), allowFilter });

            if (!string.IsNullOrEmpty(_kesalahan))
            {
                txtKesalahan.Text = _kesalahan;
            }
            if (!string.IsNullOrEmpty(_butirKesalahan))
            {
                txtButirKesalahan.Text = _butirKesalahan;
            }
            if (!string.IsNullOrEmpty(_amaunTawaran))
            {
                txtAmaunTawaran.Text = _amaunTawaran;
            }

            chkArahan.Checked = _chkArahan;

            if (!string.IsNullOrEmpty(_tempohTawaran) && chkArahan.Checked)
            {
                txtTempohTawaran.Text = _tempohTawaran;

            }
            else
            {
                txtTempohTawaran.Text = "1";

                var katPremis = spKategoryPremis.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(katPremis) && katPremis.Contains("TETAP"))
                {
                    txtTempohTawaran.Text = "14";
                }
            }

            if (!string.IsNullOrEmpty(_selectedAkta))
            {
                var positionAkta = PasukanBll.GetPositionSelected(listAkta, _selectedAkta);
                spAkta.SetSelection(positionAkta);
            }

            rdIndividu.Checked = _rdIndividu;
            rdSyarikat.Checked = _rdSyarikat;

            if (chkArahan.Checked)
            {
                txtTempohTawaran.Enabled = true;
                txtTempohTawaran.SetBackgroundResource(Resource.Drawable.editText_bg);
            }


            if (_isSaved)
            {
                rdIndividu.Enabled = false;
                rdSyarikat.Enabled = false;
                btnKesalahan.Enabled = false;
                // txtKesalahan.Enabled = false;
                txtButirKesalahan.Enabled = false;
                txtButirKesalahan.SetBackgroundResource(Resource.Drawable.textView_bg);
                chkArahan.Enabled = false;

                txtTempohTawaran.Enabled = false;
                txtTempohTawaran.SetBackgroundResource(Resource.Drawable.textView_bg);

                spAkta.Enabled = false;
                spAkta.SetBackgroundResource(Resource.Drawable.textView_bg);
            }
            else
            {
                bool blFirst = true;
                spAkta.ItemSelected += (send, args) =>
                {
                    if (!blFirst)
                    {
                        if (txtKesalahan != null) txtKesalahan.Text = "";
                        _kodSalah = 0;

                        if (txtButirKesalahan != null) txtButirKesalahan.Text = "";
                    }
                    blFirst = false;
                };
                chkArahan.CheckedChange += (send, args) =>
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
                };

                btnKesalahan.Click += (send, args) =>
                {
                    ShowKesalahan(listAkta, spAkta);
                };

                rdIndividu.CheckedChange += (send, args) =>
                {
                    SetAmounTawaran(listAkta, spAkta);
                };
                rdSyarikat.CheckedChange += (send, args) =>
                {
                    SetAmounTawaran(listAkta, spAkta);
                };
            }




            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                if (!_isSaved)
                {
                    _kesalahan = txtKesalahan.Text;
                    _butirKesalahan = txtButirKesalahan.Text;
                    _amaunTawaran = txtAmaunTawaran.Text;
                    _tempohTawaran = txtTempohTawaran.Text;
                    _chkArahan = chkArahan.Checked;
                    _rdIndividu = rdIndividu.Checked;
                    _rdSyarikat = rdSyarikat.Checked;
                    _selectedAkta = GeneralBll.GetKeySelected(listAkta, spAkta.SelectedItem?.ToString() ?? "");
                    SetPrintButton();
                }

                SetButtonKesalahan();

                builder.Dismiss();
            };

            builder.Show();
        }

        private int _kodSalah;

        private void ShowKesalahan(Dictionary<string, string> listAkta, Spinner spAkta)
        {
            var selectedAkta = GeneralBll.GetKeySelected(listAkta,
                spAkta.SelectedItem?.ToString() ?? "");

            var listOfKesalahan = MasterDataBll.GetKesalahanByAktaKots(selectedAkta);

            var listOfFiltered = listOfKesalahan;


            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.LayoutInflater.Inflate(Resource.Layout.CarianPremis, null);
            builder.SetView(view);

            var txtCarianKesalahan = view.FindViewById<EditText>(Resource.Id.txtCarian);
            listView = view.FindViewById<ListView>(Resource.Id.carianPremisListView);
            var lblTitleCarian = view.FindViewById<TextView>(Resource.Id.lblTitleCarian);
            lblTitleCarian.Text = "Kesalahan";


            listView.Adapter = new CarianKesalahanAdapter(this.Activity, listOfKesalahan);

            txtCarianKesalahan.TextChanged += (send, args) =>
            {
                listOfFiltered = listOfKesalahan
                    .Where(m => m.Prgn.ToLower().Contains(txtCarianKesalahan.Text.ToLower())).ToList();

                listView.Adapter = new CarianKesalahanAdapter(this.Activity, listOfFiltered);
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


                //var data = KompaunBll.GetAmountKesalahan(selectedAkta, _kodSalah,
                //    rdIndividu.Checked);
                //txtAmaunTawaran.Text = data.ToString(Constants.DecimalFormat);
                SetAmounTawaran(listAkta, spAkta);

                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private void SetAmounTawaran(Dictionary<string, string> listAkta, Spinner spAkta)
        {
            var selectedAkta = GeneralBll.GetKeySelected(listAkta,
                spAkta.SelectedItem?.ToString() ?? "");

            var data = KompaunBll.GetAmountKesalahan(selectedAkta, _kodSalah,
                rdIndividu.Checked);
            txtAmaunTawaran.Text = data.ToString(Constants.DecimalFormat);

        }

        private void SetButtonKesalahan()
        {
            btnKesalahanKompaun.SetBackgroundResource(Resource.Color.buttongrey);
            btnKesalahanKompaun.SetTextColor(Color.Red);

            if (string.IsNullOrEmpty(_selectedAkta)) return;
            if (_kodSalah == 0) return;
            if (string.IsNullOrEmpty(_butirKesalahan)) return;
            if (!_rdIndividu && !_rdSyarikat) return;

            btnKesalahanKompaun.SetBackgroundResource(Resource.Color.buttonblue);
            btnKesalahanKompaun.SetTextColor(Color.White);


        }

        private Response<List<DataSsmDto>> listSsm = new Response<List<DataSsmDto>>();
        EditText txtCarianSsm;
        private void OnShowCarianSsm()
        {

            listSsm = PemeriksaanBll.GetListSsm("");
            if (!listSsm.Success)
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, listSsm.Mesage);
            }

            var builder = new AlertDialog.Builder(this.Activity).Create();
            var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.CarianSsm, null);
            builder.SetView(view);

            txtCarianSsm = view.FindViewById<EditText>(Resource.Id.txtCarian);
            txtCarianSsm.SetFilters(
                new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(100) });

            listView = view.FindViewById<ListView>(Resource.Id.listView);
            var btnSearch = view.FindViewById<Button>(Resource.Id.btnSearch);

            listView.Adapter = new CarianSsmAdapter(this.Activity, listSsm.Result);

            btnSearch.Click += (send, args) =>
            {
                //btnSearch.RequestFocus();
                _hourGlass?.StartMessage(this.Activity, SearchSsm);

                //listSsm = PemeriksaanBll.GetListSsm(txtCarian.Text);
                //if (listSsm.Success)
                //{
                //    listView.Adapter = new CarianSsmAdapter(this.Activity, listSsm.Result);
                //}
                //else
                //{
                //    GeneralAndroidClass.ShowModalMessage(this.Activity, listSsm.Mesage);
                //    builder?.Dismiss();
                //}

            };

            listView.ItemClick += (send, args) =>
            {
                //txtLokasi.Text = listOfPremisFiltered[args.Position]?.Nama;
                txtNamaPremis.Text = listSsm.Result[args.Position]?.NamaSyarikat;
                txtAlamat1.Text = listSsm.Result[args.Position]?.AlamatNiaga1;
                txtAlamat2.Text = listSsm.Result[args.Position]?.AlamatNiaga2;
                txtAlamat3.Text = listSsm.Result[args.Position]?.AlamatNiaga3;

                txtNoDaftarSyarikat.Text = listSsm.Result[args.Position]?.NoSyarikat;

                builder.Dismiss();
            };

            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private void SearchSsm()
        {
            listSsm = PemeriksaanBll.GetListSsm(txtCarianSsm.Text);

            if (listSsm.Success)
            {
                listView.Adapter = new CarianSsmAdapter(this.Activity, listSsm.Result);
            }
            else
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, listSsm.Mesage);
                listView.Adapter = new CarianSsmAdapter(this.Activity, new List<DataSsmDto>());
            }

            _hourGlass?.StopMessage();
            //HideKeyboard();
        }

        private void HideKeyboard()
        {
            //InputMethodManager inputMethodManager = View.Context.GetSystemService("input_method") as InputMethodManager;
            //inputMethodManager?.HideSoftInputFromWindow(View.WindowToken, HideSoftInputFlags.None);

            InputMethodManager inputManager = (InputMethodManager)this.Activity
                .GetSystemService(Context.InputMethodService);
            View currentFocusedView = this.Activity.CurrentFocus;
            if (currentFocusedView != null)
            {
                inputManager.HideSoftInputFromWindow(currentFocusedView.WindowToken, HideSoftInputFlags.None);
            }

        }

        private void BtnSearchJpn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtNoKpPenerima.Text))
                {
                    _hourGlass?.StartMessage(this.Activity, OnSearchJpnDetail);
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
                        GeneralAndroidClass.ShowModalMessage(this.Activity, listJpnDetail.Result.messageCode);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(listJpnDetail.Result.messageCode))
                        GeneralAndroidClass.ShowModalMessage(this.Activity, listJpnDetail.Result.messageCode);
                    else if (string.IsNullOrEmpty(listJpnDetail.Mesage))
                        GeneralAndroidClass.ShowModalMessage(this.Activity, listJpnDetail.Mesage);
                    else
                        GeneralAndroidClass.ShowModalMessage(this.Activity, string.Format(Constants.ErrorMessages.NoDataFoundJpnDetail, txtNoKpPenerima.Text));
                }
            }
            else
            {
                GeneralAndroidClass.ShowModalMessage(this.Activity, listJpnDetail.Mesage);

            }

            _hourGlass?.StopMessage();
        }

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
                var bitmap = printImageBll.Pemeriksaan(this.Activity, lblNoKpp.Text);

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

                PrepareSendDataOnline();
            }
        }

        #endregion

        //private Response<JpnDetailResponse> listJpnDetail = new Response<JpnDetailResponse>();
        //EditText txtCarianJpnDetail;
        //private AlertDialog builderJpn;
        //private void OnShowCarianJpnDetail()
        //{

        //    builderJpn = new AlertDialog.Builder(this.Activity).Create();
        //    var view = this.Activity.LayoutInflater.Inflate(Resource.Layout.CarianJpnDetail, null);
        //    builderJpn.SetView(view);

        //    txtCarianJpnDetail = view.FindViewById<EditText>(Resource.Id.txtCarian);
        //    txtCarianJpnDetail.SetFilters(
        //        new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(100) });


        //    var btnSearch = view.FindViewById<Button>(Resource.Id.btnSearch);

        //    btnSearch.Click += (send, args) =>
        //    {
        //        _hourGlass?.StartMessage(this.Activity, SearchJpnDetail);
        //    };

        //    var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
        //    close_button.Click += (send, args) =>
        //    {
        //        builderJpn.Dismiss();
        //    };

        //    builderJpn.Show();
        //}

        //private void SearchJpnDetail()
        //{
        //    listJpnDetail = PemeriksaanBll.GetListJpnDetail(txtCarianJpnDetail.Text);

        //    if (listJpnDetail.Success)
        //    {
        //        txtNamaPenerima.Text = listJpnDetail.Result.name;
        //        txtNoKpPenerima.Text = listJpnDetail.Result.icnum;
        //        txtAlamatPenerima1.Text = listJpnDetail.Result.address1;
        //        txtAlamatPenerima2.Text = listJpnDetail.Result.address2;
        //        txtAlamatPenerima3.Text = listJpnDetail.Result.address3;

        //        builderJpn.Dismiss();

        //    }
        //    else
        //    {
        //        GeneralAndroidClass.ShowModalMessage(this.Activity, listJpnDetail.Mesage);

        //    }

        //    _hourGlass?.StopMessage();

        //}       
    }
}