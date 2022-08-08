using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.Activities
{
    [Activity(Label = "View Siasat", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ViewSiasatLanjut : BaseActivity
    {
        private const string LayoutName = "ViewSiasat";
        LinearLayout tabButiran, tabPesalah;
        TextView lblTabButiran, lblTabPesalah;
        View viewButiran, viewPesalah;

        private string _noRujukanKpp;
        private TextView lblNoKpp;
        private HourGlassClass _hourGlass = new HourGlassClass();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ViewSiasatLanjut);

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

                var data = KompaunBll.GetSiasatByRujukanKpp(_noRujukanKpp);
               

                if (data != null)
                {

                    lblNoKpp = FindViewById<TextView>(Resource.Id.lblNoKpp);
                    lblNoKpp.Text = data.NoKes;

                    var dataKesalahan = KompaunBll.GetDataKesKesalahan(data.NoKes);
                    var dataPesalah = KompaunBll.GetDataKesPesalah(data.NoKes);

                    LoadDataButiran(data, dataKesalahan);

                    LoadDataPesalah(data, dataPesalah);

                    LoadButton();
                }
             

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }
            _hourGlass?.StopMessage();
        }

        private void LoadDataButiran(TbDataKes data, TbDataKesKesalahan dataKesalahan)
        {
          
            var dtData = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhSalah);
            var txtTarikh = FindViewById<TextView>(Resource.Id.txtTarikh);
            txtTarikh.Text = dtData.ToString(Constants.DateFormatDisplay);

            var txtMasa = FindViewById<TextView>(Resource.Id.txtMasa);
            txtMasa.Text = dtData.ToString(Constants.TimeFormatDisplay);
            
            var txtAkta = FindViewById<EditText>(Resource.Id.txtAkta);
            txtAkta.Text = "";
         
            SetDisableEditText(txtAkta);

            var txtKesalahan = FindViewById<EditText>(Resource.Id.txtKesalahan);
            txtKesalahan.Text = "";
          
            SetDisableEditText(txtKesalahan);

            var txtButirKesalahan = FindViewById<EditText>(Resource.Id.txtButirKesalahan);
            txtButirKesalahan.Text = "";
           
            SetDisableEditText(txtButirKesalahan);

            var txtTempat = FindViewById<EditText>(Resource.Id.txtTempat);
            txtTempat.Text = data.Tempat;
            SetDisableEditText(txtTempat);

            var txtNoEP = FindViewById<EditText>(Resource.Id.txtNoEP);
            txtNoEP.Text = data.NoEp;
            SetDisableEditText(txtNoEP);

            var txtNoIP = FindViewById<EditText>(Resource.Id.txtNoIP);
            txtNoIP.Text = data.NoIp;
            SetDisableEditText(txtNoIP);

            txtNoEP.SetFilters(new IInputFilter[] { new FilterChar() });
            txtNoIP.SetFilters(new IInputFilter[] { new FilterChar() });

            var txtPegawaiSerbuan = FindViewById<EditText>(Resource.Id.txtPegawaiSerbuan);
            txtPegawaiSerbuan.Text = "";
            if (data.PegawaiSerbuan.HasValue)
            {
                txtPegawaiSerbuan.Text = MasterDataBll.GetPegawaiSerbuName(data.PegawaiSerbuan.Value);
            }
            
            SetDisableEditText(txtPegawaiSerbuan);

            if (dataKesalahan != null)
            {
                txtAkta.Text = MasterDataBll.GetAktaName(dataKesalahan.KodAkta);
                txtKesalahan.Text = MasterDataBll.GetKesalahanName(dataKesalahan.KodSalah, dataKesalahan.KodAkta);
                txtButirKesalahan.Text = dataKesalahan.ButirSalah;
            }

        }

        private void LoadDataPesalah(TbDataKes data, TbDataKesPesalah dataPesalah)
        {
            var txtNama = FindViewById<EditText>(Resource.Id.txtNama);
            txtNama.Text = dataPesalah?.NamaOks;
            SetDisableEditText(txtNama);

            var txtNoKp = FindViewById<EditText>(Resource.Id.txtNoKp);
            txtNoKp.Text = dataPesalah?.NoKpOks;
            SetDisableEditText(txtNoKp);
            
            var txtNamaSyarikat = FindViewById<EditText>(Resource.Id.txtNamaSyarikat);
            txtNamaSyarikat.Text = data.NamaPremis;
            SetDisableEditText(txtNamaSyarikat);

            var txtNoDaftarSyarikat = FindViewById<EditText>(Resource.Id.txtNoDaftarSyarikat);
            txtNoDaftarSyarikat.Text = data.NoDaftarPremis;
            SetDisableEditText(txtNoDaftarSyarikat);

            var txtAlamat1 = FindViewById<EditText>(Resource.Id.txtAlamat1);
            txtAlamat1.Text = dataPesalah?.AlamatOks1;
            SetDisableEditText(txtAlamat1);

            var txtAlamat2 = FindViewById<EditText>(Resource.Id.txtAlamat2);
            txtAlamat2.Text = dataPesalah?.AlamatOks2;
            SetDisableEditText(txtAlamat2);

            var txtAlamat3 = FindViewById<EditText>(Resource.Id.txtAlamat3);
            txtAlamat3.Text = dataPesalah?.AlamatOks3;
            SetDisableEditText(txtAlamat3);

            var txtKategoriPerniagaan = FindViewById<EditText>(Resource.Id.txtKategoriPerniagaan);
            txtKategoriPerniagaan.Text = "";
            if (data.KodKatPerniagaan.HasValue)
            {
                txtKategoriPerniagaan.Text = MasterDataBll.GetKategoriPerniagaanName(data.KodKatPerniagaan.Value);
            }
           
            SetDisableEditText(txtKategoriPerniagaan);

            var txtJenama = FindViewById<EditText>(Resource.Id.txtJenama);
            txtJenama.Text = "";
            if (data.KodJenama.HasValue)
            {
                txtJenama.Text = MasterDataBll.GetJenamaName(data.KodJenama.Value);
            }
           
            SetDisableEditText(txtJenama);

           
        }

        private void SetDisableEditText(EditText data)
        {
            data.SetBackgroundResource(Resource.Drawable.textView_bg);
            data.Enabled = false;
        }

        private void LoadButton()
        {
            var btnBack = FindViewById<Button>(Resource.Id.btnBack);

            btnBack.Click += BtnBack_Click;

         
            var btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            btnCamera.Click += BtnCamera_Click;
            
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnBack_Click", ex.Message, Enums.LogType.Error);
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
    }
}