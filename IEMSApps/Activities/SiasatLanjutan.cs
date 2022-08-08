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
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.Activities
{
    [Activity(Label = "Siasat Lanjutan", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SiasatLanjutan : BaseActivity
    {
        private const string LayoutName = "SiasatLanjutan";

        LinearLayout tabButiran, tabPesalah;
        TextView lblTabButiran, lblTabPesalah;
        View viewButiran, viewPesalah;

        private TextView lblNoKpp;
        private EditText txtTempat, txtButirKesalahan, txtNoEP, txtNoIP;
        private Spinner spAkta, spPegawaiSerbuan;

        private EditText txtKesalahan;
        private Button btnKesalahan, btnSearchJpn;

        private Button btnNama, btnTarikh, btnMasa;
        private EditText txtNama, txtNoKp, txtNamaSyarikat, txtNoDaftarSyarikat;
        private EditText txtAlamat1, txtAlamat2, txtAlamat3;

        private Button btnOk, btnCamera, btnBack;

        private AlertDialog _dialog;
        private bool _isSaved = false;

        //private ImageView btnButirKesalahan;

        private Dictionary<string, string> ListAkta, ListPegawai;
        private Enums.ActiveForm _activeForm = Enums.ActiveForm.SiasatLanjutan;

        private Spinner spKategoriPerniagaan, spJenama;
        private Dictionary<string, string> ListKategoriPerniagaan, ListJenama;
        private HourGlassClass _hourGlass = new HourGlassClass();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SiasatanLanjut);

            //SetInit();
            _hourGlass?.StartMessage(this, SetInit);
        }

        private void SetInit()
        {
            try
            {
                var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
                txtHhId.Text = GeneralBll.GetUserHandheld();

                _noRujukanKpp = Intent.GetStringExtra("NoRujukanKpp") ?? "";

                tabButiran = FindViewById<LinearLayout>(Resource.Id.tabButiran);
                tabPesalah = FindViewById<LinearLayout>(Resource.Id.tabPesalah);

                lblTabButiran = FindViewById<TextView>(Resource.Id.lblTabButiran);
                lblTabPesalah = FindViewById<TextView>(Resource.Id.lblTabPesalah);

                viewButiran = FindViewById<View>(Resource.Id.viewButiran);
                viewPesalah = FindViewById<View>(Resource.Id.viewPesalah);


                tabButiran.Click += TabButiran_Click;
                tabPesalah.Click += TabPesalah_Click;

                SetLayoutVisible(viewButiran, lblTabButiran, tabButiran);
                SetLayoutInvisible(viewPesalah, lblTabPesalah, tabPesalah);

                LoadData();

                LoadDataDropdown();

                var siasatanLanjut = KompaunBll.GetSiasatByRujukanKpp(_noRujukanKpp);
                if (siasatanLanjut != null)
                {
                    _isSaved = true;

                    LoadDataExisting(siasatanLanjut);

                    SetEnableControl(false);

                    lblNoKpp.Text = siasatanLanjut.NoKes;
                }
                else
                {
                    lblNoKpp.Text = GeneralBll.GenerateNoRujukan(Enums.PrefixType.SiasatLanjutan);
                }

                SetPrintButton();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }
            _hourGlass?.StopMessage();
        }

        private void LoadDataExisting(TbDataKes data)
        {
            LoadDataButiran(data);
            LoadDataPesalah(data);

        }

        private void LoadDataButiran(TbDataKes data)
        {
            var dataKesalahan = KompaunBll.GetDataKesKesalahan(data.NoKes);

            var dtData = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhSalah);

            btnTarikh.Text = dtData.ToString(Constants.DateFormatDisplay);

            btnMasa.Text = dtData.ToString(Constants.TimeFormatDisplay);

            spAkta.SetSelection(GeneralBll.GetPositionSelected(ListAkta, dataKesalahan.KodAkta));

            txtKesalahan.Text = MasterDataBll.GetKesalahanName(dataKesalahan.KodSalah, dataKesalahan.KodAkta);
            _kodSalah = dataKesalahan.KodSalah;

            txtButirKesalahan.Text = dataKesalahan.ButirSalah;
            txtTempat.Text = data.Tempat;
            txtNoEP.Text = data.NoEp;
            txtNoIP.Text = data.NoIp;

            spPegawaiSerbuan.SetSelection(GeneralBll.GetPositionSelected(ListPegawai, data.PegawaiSerbuan.ToString()));

        }

        private void LoadDataPesalah(TbDataKes data)
        {
            var dataPesalah = KompaunBll.GetDataKesPesalah(data.NoKes);

            //txtNama.Text = data.NamaPremis;
            //txtNoKp.Text = dataPesalah.NoKpOks;
            //txtNamaSyarikat.Text = dataPesalah.NamaOks;
            //txtNoDaftarSyarikat.Text = data.NoDaftarPremis;
            //txtAlamat1.Text = dataPesalah.AlamatOks1;
            //txtAlamat2.Text = dataPesalah.AlamatOks2;
            //txtAlamat3.Text = dataPesalah.AlamatOks3;

            txtNama.Text = dataPesalah.NamaOks;
            txtNoKp.Text = dataPesalah.NoKpOks;

            txtNoDaftarSyarikat.Text = data.NoDaftarPremis;
            txtAlamat1.Text = dataPesalah.AlamatOks1;
            txtAlamat2.Text = dataPesalah.AlamatOks2;
            txtAlamat3.Text = dataPesalah.AlamatOks3;

            txtNamaSyarikat.Text = data.NamaPremis;

        }


        private string _noRujukanKpp;

        private void LoadData()
        {
            var allowFilter = new FilterChar();
            var allowFilterWithoutSingleQuote = new FilterCharWithoutSingleQuote();

            #region Butiran
            lblNoKpp = FindViewById<TextView>(Resource.Id.lblNoKpp);

            btnTarikh = FindViewById<Button>(Resource.Id.btnTarikh);
            btnMasa = FindViewById<Button>(Resource.Id.btnMasa);


            spAkta = FindViewById<Spinner>(Resource.Id.spAkta);
            // spKesalahan = FindViewById<Spinner>(Resource.Id.spKesalahan);

            txtButirKesalahan = FindViewById<EditText>(Resource.Id.txtButirKesalahan);
            //btnButirKesalahan = FindViewById<ImageView>(Resource.Id.btnButirKesalahan);

            txtTempat = FindViewById<EditText>(Resource.Id.txtTempat);

            txtNoEP = FindViewById<EditText>(Resource.Id.txtNoEP);
            txtNoIP = FindViewById<EditText>(Resource.Id.txtNoIP);
            txtNoEP.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new FilterNoIPAndNoEPChar(), new InputFilterLengthFilter(30) });
            txtNoIP.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new FilterNoIPAndNoEPChar(), new InputFilterLengthFilter(30) });

            spPegawaiSerbuan = FindViewById<Spinner>(Resource.Id.spPegawaiSerbuan);

            txtKesalahan = FindViewById<EditText>(Resource.Id.txtKesalahan);
            btnKesalahan = FindViewById<Button>(Resource.Id.btnKesalahan);

            btnKesalahan.Click += BtnKesalahan_Click;

            btnTarikh.Text = GeneralBll.GetLocalDateTime().ToString(Constants.DateFormatDisplay);
            btnMasa.Text = GeneralBll.GetLocalDateTime().ToString(Constants.TimeFormatDisplay);

            txtButirKesalahan.Enabled = false;
            txtButirKesalahan.SetBackgroundResource(Resource.Drawable.textView_bg);


            //txtButirKesalahan.TextChanged += Event_CheckMandatory_Dropdown_Edittext;
            txtTempat.TextChanged += Event_CheckMandatory_Dropdown_Edittext;

            //btnButirKesalahan.Click += BtnButirKesalahan_Click;

            //txtButirKesalahan.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(1000) });
            txtTempat.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(150), allowFilter });

            #endregion

            #region Pesalah

            btnNama = FindViewById<Button>(Resource.Id.btnNama);
            txtNama = FindViewById<EditText>(Resource.Id.txtNama);
            txtNoKp = FindViewById<EditText>(Resource.Id.txtNoKp);
            txtNamaSyarikat = FindViewById<EditText>(Resource.Id.txtNamaSyarikat);
            txtNoDaftarSyarikat = FindViewById<EditText>(Resource.Id.txtNoDaftarSyarikat);
            txtAlamat1 = FindViewById<EditText>(Resource.Id.txtAlamat1);
            txtAlamat2 = FindViewById<EditText>(Resource.Id.txtAlamat2);
            txtAlamat3 = FindViewById<EditText>(Resource.Id.txtAlamat3);

            txtAlamat1.TextChanged += Event_CheckMandatory_Dropdown_Edittext;

            btnNama.Click += BtnNama_Click;

            txtNama.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
            txtNoKp.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(50), allowFilterWithoutSingleQuote });
            txtNamaSyarikat.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
            txtNoDaftarSyarikat.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(15), allowFilterWithoutSingleQuote });
            txtAlamat1.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(300), allowFilter });
            txtAlamat2.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
            txtAlamat3.SetFilters(new IInputFilter[] { new InputFilterAllCaps(), new InputFilterLengthFilter(80), allowFilter });
            btnSearchJpn = FindViewById<Button>(Resource.Id.btnSearchJpn);
            btnSearchJpn.Click += btnSearchJpn_Click;
            spKategoriPerniagaan = FindViewById<Spinner>(Resource.Id.spKategoriPerniagaan);
            spJenama = FindViewById<Spinner>(Resource.Id.spJenama);

            #endregion


            #region Button

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnOk = FindViewById<Button>(Resource.Id.btnOk);
            btnCamera = FindViewById<Button>(Resource.Id.btnCamera);

            btnBack.Click += BtnBack_Click;
            btnOk.Click += BtnOk_Click; ;
            btnCamera.Click += BtnCamera_Click; ;

            btnTarikh.Click += BtnTarikh_Click;
            btnMasa.Click += BtnMasa_Click;

            #endregion

            var pemeriksaan = PemeriksaanBll.GetPemeriksaanByRujukan(_noRujukanKpp);
            if (pemeriksaan != null)
            {
                txtTempat.Text = pemeriksaan.LokasiLawatan;
                txtNama.Text = pemeriksaan.NamaPremis;
                txtAlamat1.Text = pemeriksaan.AlamatPremis1;
                txtAlamat2.Text = pemeriksaan.AlamatPremis2;
                txtAlamat3.Text = pemeriksaan.AlamatPremis3;
                txtNoDaftarSyarikat.Text = pemeriksaan.NoDaftarPremis;
                txtNamaSyarikat.Text = pemeriksaan.NamaPremis;
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
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
                GeneralAndroidClass.LogData(LayoutName, "BtnBack_Click", ex.Message, Enums.LogType.Error);
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
                txtButirKesalahan.Text = listOfFiltered[args.Position]?.Prgn;

                SetPrintButton();

                builder.Dismiss();
            };
            var close_button = view.FindViewById<ImageView>(Resource.Id.close_button);
            close_button.Click += (send, args) =>
            {
                builder.Dismiss();
            };

            builder.Show();
        }

        private static int REQUEST_MYKAD = 1001;

        private void BtnNama_Click(object sender, EventArgs e)
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

        private void ClearMyCardField()
        {
            txtNama.Text = "";
            txtNoKp.Text = "";
            txtAlamat1.Text = "";
            txtAlamat2.Text = "";
            txtAlamat3.Text = "";
        }


        private void SetMyCard(CardInfoDto cardDto)
        {
            ClearMyCardField();

            var listAddress = new List<string>();

            txtNama.Text = cardDto.originalName;
            txtNoKp.Text = cardDto.idNum;

            var address = $"{cardDto.address1} {cardDto.address2}";
            var addressPostCodeCity = $"{cardDto.postcode} {cardDto.city} {cardDto.state}";


            if (string.IsNullOrEmpty(cardDto.address3))
            {
                listAddress = GeneralBll.SeparateText(address, 2, Constants.MaxLengthAddress);
                txtAlamat1.Text = listAddress[0];
                if (string.IsNullOrEmpty(listAddress[1]))
                    txtAlamat2.Text = addressPostCodeCity;
                else
                {
                    txtAlamat2.Text = listAddress[1];
                    txtAlamat3.Text = addressPostCodeCity;
                }
            }
            else
            {
                if (address.Length <= 80)
                {
                    txtAlamat1.Text = address;
                    txtAlamat2.Text = cardDto.address3;
                    txtAlamat3.Text = addressPostCodeCity;
                }
                else
                {
                    address = string.Format("{0} {1} {2}", cardDto.address1, cardDto.address2, cardDto.address3);

                    var listString = GeneralBll.SeparateText(address, 2, Constants.MaxLengthAddress);
                    txtAlamat1.Text = listString[0].Trim();
                    txtAlamat2.Text = listString[1].Trim();
                    txtAlamat3.Text = addressPostCodeCity;
                }

            }
        }

        private void BtnButirKesalahan_Click(object sender, EventArgs e)
        {
            var dialogView = LayoutInflater.Inflate(Resource.Layout.ButiranList, null);
            AlertDialog alertDialog;
            using (var dialog = new AlertDialog.Builder(this))
            {
                dialog.SetTitle("Butir Kesalahan");
                dialog.SetView(dialogView);
                dialog.SetNegativeButton("Batal", (s, a) => { });
                alertDialog = dialog.Create();
            }

            var selectedAkta = GeneralBll.GetKeySelected(ListAkta, spAkta.SelectedItem?.ToString() ?? "");
            var listButir =
                MasterDataBll.GetKesalahanButir(selectedAkta, _kodSalah);

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, listButir);
            var listView = dialogView.FindViewById<ListView>(Resource.Id.listView1);
            listView.Adapter = adapter;
            listView.ItemClick += (s, a) =>
            {
                //Toast.MakeText(this, listButir[a.Position], ToastLength.Short).Show();
                alertDialog.Dismiss();
                txtButirKesalahan.Text = listButir[a.Position];
            };

            alertDialog.Show();
        }

        private void LoadDataDropdown()
        {
            ListAkta = KompaunBll.GetAllAkta();
            spAkta.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item,
                ListAkta.Select(c => c.Value).ToList());

            ListPegawai = PemeriksaanBll.GetAllPasukanSerbu();
            spPegawaiSerbuan.Adapter = new ArrayAdapter<string>(this,
                Resource.Layout.support_simple_spinner_dropdown_item, ListPegawai.Select(c => c.Value).ToList());

            spPegawaiSerbuan.SetSelection(GeneralBll.GetPositionSelected(ListPegawai, GeneralBll.GetUserId()));

            spAkta.ItemSelected += SpAkta_ItemSelected;

            spPegawaiSerbuan.ItemSelected += Event_CheckMandatory_Dropdown;

            ListKategoriPerniagaan = MasterDataBll.GetAllKategoriPerniagaan();
            spKategoriPerniagaan.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item,
                ListKategoriPerniagaan.Select(c => c.Value).ToList());

            ListJenama = MasterDataBll.GetAllJenama();
            spJenama.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item,
                ListJenama.Select(c => c.Value).ToList());
        }

        private void SpAkta_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                if (!_isSaved)
                {
                    txtKesalahan.Text = "";
                    _kodSalah = 0;
                    txtButirKesalahan.Text = "";
                }

                SetPrintButton();

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SpAkta_ItemSelected", ex.Message, Enums.LogType.Error);
            }
        }

        #region Button Event

        private void BtnMasa_Click(object sender, EventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                btnMasa.Text = time.ToString(Constants.TimeFormatDisplay);
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        private void BtnTarikh_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                btnTarikh.Text = time.ToString(Constants.DateFormatDisplay);
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
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

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateData())
                {
                    //var ad = GeneralAndroidClass.GetDialogCustom(this);

                    //ad.SetMessage("Save data ? ");
                    //// Positive

                    //ad.SetButton("No", (s, ev) => { });
                    //ad.SetButton2("Yes", (s, ev) =>
                    //{
                    //    _dialog = GeneralAndroidClass.ShowProgressDialog(this, "Saving data...Please wait");
                    //    new Thread(() =>
                    //    {
                    //        Thread.Sleep(1000);
                    //        this.RunOnUiThread(SaveData);
                    //    }).Start();

                    //});
                    //ad.Show();
                    ShowConfirmModal();
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnOk_Click", ex.Message, Enums.LogType.Error);
            }
        }


        #endregion

        #region Layout 

        private void TabPesalah_Click(object sender, EventArgs e)
        {
            SetLayoutInvisible(viewButiran, lblTabButiran, tabButiran);
            SetLayoutVisible(viewPesalah, lblTabPesalah, tabPesalah);
        }

        private void TabButiran_Click(object sender, EventArgs e)
        {
            SetLayoutVisible(viewButiran, lblTabButiran, tabButiran);
            SetLayoutInvisible(viewPesalah, lblTabPesalah, tabPesalah);
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

            if (string.IsNullOrEmpty(txtButirKesalahan.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Butiran kesalahan kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(txtTempat.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Tempat kosong.");
                return false;
            }

            if (spPegawaiSerbuan.SelectedItem == null)
            {
                GeneralAndroidClass.ShowModalMessage(this, "Pegawai serbuan kosong.");
                return false;
            }

            if (string.IsNullOrEmpty(txtAlamat1.Text))
            {
                GeneralAndroidClass.ShowModalMessage(this, "Alamat pesalah kosong.");
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
            var data = new TbDataKes
            {
                KodCawangan = GeneralBll.GetUserCawangan(),
                NoKes = lblNoKpp.Text,
                NoKpp = _noRujukanKpp,
                TrkhSalah = GeneralBll.ConvertDateDisplayToDatabase(btnTarikh.Text) + " " + GeneralBll.ConvertTimeDisplayToDatabase(btnMasa.Text),
                //KodAkta = GeneralBll.GetKeySelected(ListAkta, spAkta.SelectedItem?.ToString() ?? ""),
                //KodSalah = _kodSalah,
                //ButirSalah = txtButirKesalahan.Text,
                Tempat = txtTempat.Text,
                NoEp = txtNoEP.Text,
                NoIp = txtNoIP.Text,
                PegawaiSerbuan = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListPegawai,
                    spPegawaiSerbuan.SelectedItem?.ToString() ?? "")),
                //NamaOks = txtNama.Text,
                //NoKpOks = txtNoKp.Text,
                NamaPremis = txtNamaSyarikat.Text,
                NoDaftarPremis = txtNoDaftarSyarikat.Text,
                //AlamatOks1 = txtAlamat1.Text,
                //AlamatOks2 = txtAlamat2.Text,
                //AlamatOks3 = txtAlamat3.Text,
                PgnDaftar = GeneralBll.GetUserStaffId(),
                KodKatPerniagaan = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListKategoriPerniagaan,
                    spKategoriPerniagaan.SelectedItem?.ToString() ?? "")),
                KodJenama = GeneralBll.ConvertStringToInt(GeneralBll.GetKeySelected(ListJenama,
                    spJenama.SelectedItem?.ToString() ?? "")),

                KelasKes = "",
                KodStatusKes = "BS",
                KodStatusKes_Det = "BS01"
            };

            if (data.PegawaiSerbuan == 0)
            {
                data.PegawaiSerbuan = null;
            }
            if (data.KodKatPerniagaan == 0)
            {
                data.KodKatPerniagaan = null;
            }
            if (data.KodJenama == 0)
            {
                data.KodJenama = null;
            }

            data.PgnAkhir = data.PgnDaftar;

            data.TrkhDaftar = GeneralBll.GetLocalDateTimeForDatabase();
            data.TrkhAkhir = GeneralBll.GetLocalDateTimeForDatabase();
            data.Status = Constants.Status.Aktif;

            var pemeriksaan = PemeriksaanBll.GetPemeriksaanByRujukan(_noRujukanKpp);
            if (pemeriksaan != null)
            {
                //data.NamaPremis = pemeriksaan.NamaPremis;
                data.KodKatKawasan = pemeriksaan.KodKatKawasan;
                data.KodTujuan = pemeriksaan.KodTujuan;
            }

            var inputDataKesKesalahan = new TbDataKesKesalahan()
            {
                NoKes = data.NoKes,
                KodCawangan = data.KodCawangan,
                KodAkta = GeneralBll.GetKeySelected(ListAkta, spAkta.SelectedItem?.ToString() ?? ""),
                KodSalah = _kodSalah,
                ButirSalah = txtButirKesalahan.Text,
                IsSendOnline = Enums.StatusOnline.New,
                PgnDaftar = data.PgnDaftar,
                TrkhDaftar = data.TrkhDaftar
            };

            var inputDataKesPesalah = new TbDataKesPesalah()
            {
                NoKes = data.NoKes,
                KodCawangan = data.KodCawangan,
                NoKpOks = txtNoKp.Text,
                NamaOks = txtNama.Text,
                AlamatOks1 = txtAlamat1.Text,
                AlamatOks2 = txtAlamat2.Text,
                AlamatOks3 = txtAlamat3.Text,
                IsSendOnline = Enums.StatusOnline.New,
                PgnDaftar = data.PgnDaftar,
                TrkhDaftar = data.TrkhDaftar
            };

            if (KompaunBll.SaveSiasatLanjutanTrx(data, inputDataKesKesalahan, inputDataKesPesalah))
            {
                _isSaved = true;
                btnOk.SetBackgroundResource(Resource.Drawable.save_icon_disabled);
                btnOk.Enabled = false;
                SetEnableControl(false);
                //PrepareSendDataOnline();
                _dialog?.Dismiss();
            }
            else
            {
                GeneralAndroidClass.ShowModalMessage(this, Constants.ErrorMessages.FailedSaveData);
            }

        }

        //private void OnModalDialog(string message)
        //{
        //    var ad = GeneralAndroidClass.GetDialogCustom(this);

        //    ad.SetMessage(message);
        //    ad.DismissEvent += Ad_DismissEvent;
        //    // Positive
        //    ad.SetButton2("OK", (s, e) => { this.Finish(); });
        //    ad.Show();
        //}

        //private void Ad_DismissEvent(object sender, EventArgs e)
        //{
        //    Finish();
        //}

        private void SetPrintButton()
        {
            if (_isSaved)
            {
                btnOk.SetBackgroundResource(Resource.Drawable.save_icon_disabled);
                btnOk.Enabled = false;
            }
            else
            {
                if (IsAllDataRequiredNoEmpty())
                {
                    btnOk.SetBackgroundResource(Resource.Drawable.save_icon);
                    btnOk.Enabled = true;
                }
                else
                {
                    btnOk.SetBackgroundResource(Resource.Drawable.save_icon_disabled);
                    btnOk.Enabled = false;
                }
            }

        }

        private bool IsAllDataRequiredNoEmpty()
        {
            if (string.IsNullOrEmpty(spAkta.SelectedItem?.ToString())) return false;
            if (_kodSalah == 0) return false;
            if (string.IsNullOrEmpty(txtButirKesalahan.Text)) return false;
            if (string.IsNullOrEmpty(txtTempat.Text)) return false;
            if (spPegawaiSerbuan.SelectedItem == null) return false;

            if (string.IsNullOrEmpty(txtAlamat1.Text)) return false;

            //#if !DEBUG
            //            var countPhoto = GeneralBll.GetCountPhotoByRujukan(lblNoKpp.Text);
            //            if (countPhoto < Constants.MinPhoto) return false;
            //#endif
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
            var dialogView = View.Inflate(this, Resource.Layout.ConfirmLayout, null);
            var alertDialog = new AlertDialog.Builder(this).Create();

            var lvResult = dialogView.FindViewById<ListView>(Resource.Id.lView);
            var lblTitle = dialogView.FindViewById<TextView>(Resource.Id.lblTitle);
            lblTitle.Text = "Pengesahan Siasatan Lanjut";

            var lblBtnCetak = dialogView.FindViewById<TextView>(Resource.Id.lblBtnCetak);
            lblBtnCetak.Text = "Simpan";

            var listConfirm = GetListConfirm();

            lvResult.Adapter = new ConfirmListAdapter(this, listConfirm);
            lvResult.FastScrollEnabled = true;

            var btnCancel = dialogView.FindViewById<Button>(Resource.Id.btnCancel);
            btnCancel.Click += (sender, e) =>
            {
                alertDialog.Dismiss();
            };

            var btnCetak = dialogView.FindViewById<Button>(Resource.Id.btnCetak);
            btnCetak.SetBackgroundResource(Resource.Drawable.save_icon);
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
            result.Add(GeneralBll.CreateConfirmDto("Tarikh", btnTarikh.Text));
            result.Add(GeneralBll.CreateConfirmDto("Masa", btnMasa.Text));
            result.Add(GeneralBll.CreateConfirmDto("Akta", spAkta.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Kesalahan", txtKesalahan.Text));
            result.Add(GeneralBll.CreateConfirmDto("Butir Kesalahan", txtButirKesalahan.Text));
            result.Add(GeneralBll.CreateConfirmDto("Tempat", txtTempat.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. EP", txtNoEP.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. IP", txtNoIP.Text));
            result.Add(GeneralBll.CreateConfirmDto("Pegawai Serbuan", spPegawaiSerbuan.SelectedItem?.ToString() ?? ""));

            result.Add(GeneralBll.CreateConfirmDto("Pesalah", "", true));
            result.Add(GeneralBll.CreateConfirmDto("Kategori Perniagaan", spKategoriPerniagaan.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Jenama", spJenama.SelectedItem?.ToString() ?? ""));
            result.Add(GeneralBll.CreateConfirmDto("Nama", txtNama.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. K/P", txtNoKp.Text));
            result.Add(GeneralBll.CreateConfirmDto("Nama Syarikat/Premis", txtNamaSyarikat.Text));
            result.Add(GeneralBll.CreateConfirmDto("No. Daftar Syarikat", txtNoDaftarSyarikat.Text));

            var alamat = GeneralBll.GettOneAlamat(txtAlamat1.Text, txtAlamat2.Text, txtAlamat3.Text);
            result.Add(GeneralBll.CreateConfirmDto("Alamat", alamat));

            return result;
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_activeForm == Enums.ActiveForm.Camera)
            {
                _activeForm = Enums.ActiveForm.SiasatLanjutan;

                SetPrintButton();
            }
        }

        private void PrepareSendDataOnline()
        {
            //return;
            _dialog?.Dismiss();

            if (!GeneralAndroidClass.IsOnline())
                return;
#if DEBUG
            return;
#endif

            _dialog = GeneralAndroidClass.ShowProgressDialog(this, Constants.Messages.SendDataOnline);
            new Thread(() =>
            {
                Thread.Sleep(1000);
                this.RunOnUiThread(SendDataOnline);
            }).Start();
        }

        private void SendDataOnline()
        {
            var result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(lblNoKpp.Text, Enums.TableType.DataKes, this)).Result;
            if (result.Success)
            {
                GeneralAndroidClass.ShowToast(Constants.Messages.SuccessSendData);
                _dialog?.Dismiss();

            }
            else
            {
                _dialog?.Dismiss();

                var tbSendOnline = MasterDataBll.GetTbSendOnlineByRujukanAndType(lblNoKpp.Text, Enums.TableType.DataKes);
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
                    SendOnlineBll.SetStatusDataOnline(lblNoKpp.Text, Enums.TableType.DataKes, Enums.StatusOnline.Error);

                });


                alertDialog.Show();

            }
        }
        private void SetEnableControl(bool blValue)
        {
            #region Butiran


            btnTarikh.Enabled = blValue;
            btnMasa.Enabled = blValue;
            spAkta.Enabled = blValue;
            txtKesalahan.Enabled = blValue;
            btnKesalahan.Enabled = blValue;
            //txtButirKesalahan.Enabled = blValue;
            //btnButirKesalahan.Enabled = blValue;
            txtTempat.Enabled = blValue;
            txtNoEP.Enabled = blValue;
            txtNoIP.Enabled = blValue;
            spPegawaiSerbuan.Enabled = blValue;

            #endregion

            #region Pesalah

            txtNama.Enabled = blValue;
            btnNama.Enabled = blValue;
            txtNoKp.Enabled = blValue;
            txtNamaSyarikat.Enabled = blValue;
            txtNoDaftarSyarikat.Enabled = blValue;
            txtAlamat1.Enabled = blValue;
            txtAlamat2.Enabled = blValue;
            txtAlamat3.Enabled = blValue;

            spKategoriPerniagaan.Enabled = blValue;
            spJenama.Enabled = blValue;

            #endregion



            if (blValue)
            {
                #region Butiran

                btnTarikh.SetBackgroundResource(Resource.Drawable.editText_bg);
                btnMasa.SetBackgroundResource(Resource.Drawable.editText_bg);
                spAkta.SetBackgroundResource(Resource.Drawable.spiner_bg);
                txtKesalahan.SetBackgroundResource(Resource.Drawable.editText_bg);
                //txtButirKesalahan.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtTempat.SetBackgroundResource(Resource.Drawable.editText_bg);

                txtNoEP.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoIP.SetBackgroundResource(Resource.Drawable.editText_bg);
                spPegawaiSerbuan.SetBackgroundResource(Resource.Drawable.spiner_bg);

                #endregion

                #region Pesalah

                txtNama.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoKp.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNamaSyarikat.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtNoDaftarSyarikat.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamat1.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamat2.SetBackgroundResource(Resource.Drawable.editText_bg);
                txtAlamat3.SetBackgroundResource(Resource.Drawable.editText_bg);

                spKategoriPerniagaan.SetBackgroundResource(Resource.Drawable.spiner_bg);
                spJenama.SetBackgroundResource(Resource.Drawable.spiner_bg);

                #endregion


            }
            else
            {
                #region Butiran


                btnTarikh.SetBackgroundResource(Resource.Drawable.textView_bg);
                btnMasa.SetBackgroundResource(Resource.Drawable.textView_bg);
                spAkta.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtKesalahan.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtTempat.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoEP.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoIP.SetBackgroundResource(Resource.Drawable.textView_bg);
                spPegawaiSerbuan.SetBackgroundResource(Resource.Drawable.textView_bg);

                #endregion

                #region Pesalah

                txtNama.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoKp.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNamaSyarikat.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtNoDaftarSyarikat.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamat1.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamat2.SetBackgroundResource(Resource.Drawable.textView_bg);
                txtAlamat3.SetBackgroundResource(Resource.Drawable.textView_bg);
                spKategoriPerniagaan.SetBackgroundResource(Resource.Drawable.textView_bg);
                spJenama.SetBackgroundResource(Resource.Drawable.textView_bg);

                #endregion


            }

        }

        private void btnSearchJpn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtNoKp.Text))
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
            var listJpnDetail = PemeriksaanBll.GetListJpnDetail(txtNoKp.Text, SharedPreferences.GetString(SharedPreferencesKeys.UserNoKp));
            txtNama.Text = string.Empty;
            txtAlamat1.Text = string.Empty;
            txtAlamat2.Text = string.Empty;
            txtAlamat3.Text = string.Empty;

            if (listJpnDetail.Success && listJpnDetail.Result != null)
            {
                if (listJpnDetail.Result.status == "200")
                {
                    txtNama.Text = listJpnDetail.Result.name;
                    txtAlamat1.Text = listJpnDetail.Result.address1;
                    txtAlamat2.Text = listJpnDetail.Result.address2;
                    txtAlamat3.Text =
                        $"{listJpnDetail.Result.postcode} {listJpnDetail.Result.city} {listJpnDetail.Result.state}";

                    if (!string.IsNullOrEmpty(listJpnDetail.Result.address3))
                    {
                        txtAlamat3.Text =
                            $"{listJpnDetail.Result.address3} {txtAlamat3.Text}";
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
                        GeneralAndroidClass.ShowModalMessage(this, string.Format(Constants.ErrorMessages.NoDataFoundJpnDetail, txtNoKp.Text));
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