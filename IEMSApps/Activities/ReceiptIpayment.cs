using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEMSApps.Activities
{
    [Activity(Label = "ReceiptIpayment", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ReceiptIpayment : Activity
    {
        private const string LayoutName = "ReceiptIpayment";
        string _noRujukanKpp;
        TextView txtDiterimaDaripada, txtBayaranBgiPihak, txtNoIdentiti, txtAlamat, txtEmel, txtNoRujukanIpayment, txtPerihalBayaran,
                 txtNoResit, txtTarikh, txtModBayaran, txtRangkaian, txtNoTransaksiIpayment, txtNoTransaksiRMA; 

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
                _noRujukanKpp = Intent.GetStringExtra("NoRujukanKpp") ?? "";

                var IpResit = AkuanBll.GetIpResitsByKPP(_noRujukanKpp);

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

                _hourGlass?.StopMessage();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("ReceiptIpayment", "SetInit", ex.Message, Enums.LogType.Error);
            }
        }
    }
}