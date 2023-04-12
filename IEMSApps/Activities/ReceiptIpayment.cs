using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.Classes;
using IEMSApps.Utils;
using System;


namespace IEMSApps.Activities
{
    [Activity(Label = "ReceiptIpayment", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ReceiptIpayment : Activity
    {
        private const string LayoutName = "ReceiptIpayment";
        string _noRujukanKpp;
        TextView txtDiterimaDaripada, txtBayaranBgiPihak, txtNoIdentiti, txtAlamat, txtEmel, txtNoRujukanIpayment, txtPerihalBayaran,
                 txtNoResit, txtTarikh, txtModBayaran, txtRangkaian, txtNoTransaksiIpayment, txtNoTransaksiRMA;
        TextView txtBil, txtKeterangan, txtNoRujukan, txtKodAkaun, txtJumlah, txtAmaun, txtDiskaun, txtAmaunDgnDiskaun, 
                 txtAmaunCukaiPercent, txtAmaunCukai, txtPelarasan, txtJumlahBayaran;
        TextView txtRinggitMalaysia, txtPusatTerimaan, txtPetugasKaunter;

        Button BtnBack;

        private HourGlassClass _hourGlass = new HourGlassClass();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ReceiptIpayment);

            //SetInit();

            _hourGlass?.StartMessage(this, SetInit);
        }

        private void SetInit() 
        {
            try
            {
                BtnBack = FindViewById<Button>(Resource.Id.btnBack);
                BtnBack.Click += BtnBack_Click;

                _noRujukanKpp = Intent.GetStringExtra("NoRujukanKpp") ?? "";

                var IpResit = AkuanBll.GetIpResitsByKPP(_noRujukanKpp);

                #region Header
                txtDiterimaDaripada = FindViewById<TextView>(Resource.Id.txtDiterimaDaripada);
                txtDiterimaDaripada.Text = IpResit.diterima_drpd;

                txtBayaranBgiPihak = FindViewById<TextView>(Resource.Id.txtBayaranBgiPihak);
                txtBayaranBgiPihak.Text = IpResit.byrn_bg_pihak;

                txtNoIdentiti = FindViewById<TextView>(Resource.Id.txtNoIdentiti);
                txtNoIdentiti.Text = IpResit.no_identiti;

                txtAlamat = FindViewById<TextView>(Resource.Id.txtAlamat);
                txtAlamat.Text = IpResit.alamat_1 + "," + IpResit.alamat_2 + "," + IpResit.alamat_3;

                txtEmel = FindViewById<TextView>(Resource.Id.txtEmel);
                txtEmel.Text = IpResit.emel;

                txtNoRujukanIpayment = FindViewById<TextView>(Resource.Id.txtNoRujukanIpayment);
                txtNoRujukanIpayment.Text = IpResit.no_rujukan_ipayment;

                txtPerihalBayaran = FindViewById<TextView>(Resource.Id.txtPerihalBayaran);
                txtPerihalBayaran.Text = IpResit.perihal;

                txtNoResit = FindViewById<TextView>(Resource.Id.txtNoResit);
                txtNoResit.Text = IpResit.no_resit;

                txtTarikh = FindViewById<TextView>(Resource.Id.txtTarikh);
                txtTarikh.Text = IpResit.tarikh_bayaran.ToString();

                txtModBayaran = FindViewById<TextView>(Resource.Id.txtModBayaran);
                txtModBayaran.Text = IpResit.mod_pembayaran;

                txtRangkaian = FindViewById<TextView>(Resource.Id.txtRangkaian);
                txtRangkaian.Text = IpResit.rangkaian;

                txtNoTransaksiIpayment = FindViewById<TextView>(Resource.Id.txtNoTransaksiIpayment);
                txtNoTransaksiIpayment.Text = IpResit.no_transaksi_ipayment;

                txtNoTransaksiRMA = FindViewById<TextView>(Resource.Id.txtNoTransaksiRMA);
                txtNoTransaksiRMA.Text = IpResit.no_transaksi_rma;
                #endregion

                #region table

                txtBil = FindViewById<TextView>(Resource.Id.txtBil);

                txtKeterangan = FindViewById<TextView>(Resource.Id.txtKeterangan);
                txtKeterangan.Text = IpResit.keterangan;

                txtNoRujukan = FindViewById<TextView>(Resource.Id.txtNoRujukan);
                txtNoRujukan.Text = IpResit.no_rujukan;

                txtKodAkaun = FindViewById<TextView>(Resource.Id.txtKodAkaun);
                txtKodAkaun.Text = IpResit.kod_akaun;

                txtJumlah = FindViewById<TextView>(Resource.Id.txtJumlah);
                txtJumlah.Text = IpResit.jumlah;

                txtAmaun = FindViewById<TextView>(Resource.Id.txtAmaun);
                txtAmaun.Text = IpResit.amaun.ToString();

                txtDiskaun = FindViewById<TextView>(Resource.Id.txtDiskaun);
                txtDiskaun.Text = IpResit.diskaun;

                txtAmaunDgnDiskaun = FindViewById<TextView>(Resource.Id.txtAmaunDgnDiskaun);
                txtAmaunDgnDiskaun.Text = IpResit.amaun_dgn_diskaun.ToString();

                txtAmaunCukaiPercent = FindViewById<TextView>(Resource.Id.txtAmaunCukaiPercent);
                txtAmaunCukaiPercent.Text = IpResit.amaun_dgn_cukai.ToString(); 

                txtAmaunCukai = FindViewById<TextView>(Resource.Id.txtAmaunCukai);
                txtAmaunCukai.Text = IpResit.amaun_cukai.ToString();

                txtPelarasan = FindViewById<TextView>(Resource.Id.txtPelarasan);
                txtPelarasan.Text = IpResit.pelarasan_penggenapan.ToString();

                txtJumlahBayaran = FindViewById<TextView>(Resource.Id.txtJumlahBayaran);
                txtJumlahBayaran.Text = IpResit.jumlah_bayaran.ToString();

                #endregion

                #region footer

                txtRinggitMalaysia = FindViewById<TextView>(Resource.Id.txtRinggitMalaysia);

                txtPusatTerimaan = FindViewById<TextView>(Resource.Id.txtPusatTerimaan);
                txtPusatTerimaan.Text = IpResit.pusat_terimaan;

                txtPetugasKaunter = FindViewById<TextView>(Resource.Id.txtPetugasKaunter);
                txtPetugasKaunter.Text = IpResit.petugas;

                #endregion

                _hourGlass?.StopMessage();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("ReceiptIpayment", "SetInit", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnBtnBack_Click", ex.Message, Enums.LogType.Error);
            }
        }
    }
}