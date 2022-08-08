using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using System.Timers;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.Fragments
{
    public class Ringkasan : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.Ringkasan, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetInit();
        }

        private TextView lblTarikh, lblMasa, lblLatitud, lblLongitud;
        private const string LayoutName = "Ringkasan";
        private Timer _timer;
        private int _index = 0;

        private void SetInit()
        {
            try
            {
                lblTarikh = View.FindViewById<TextView>(Resource.Id.lblTarikh);
                lblMasa = View.FindViewById<TextView>(Resource.Id.lblMasa);

                var lblJumlahKPP = View.FindViewById<TextView>(Resource.Id.lblJumlahKPP);
                var lblJumlahGambarKPP = View.FindViewById<TextView>(Resource.Id.lblJumlahGambarKPP);
                var lblNoKPPAkhir = View.FindViewById<TextView>(Resource.Id.lblNoKPPAkhir);

                var lblJumlahKompaun = View.FindViewById<TextView>(Resource.Id.lblJumlahKompaun);
                var lblJumlahGambarKompaun = View.FindViewById<TextView>(Resource.Id.lblJumlahGambarKompaun);
                var lblNoKompaunAkhir = View.FindViewById<TextView>(Resource.Id.lblNoKompaunAkhir);

                var lblJumlahSiasatanLajut = View.FindViewById<TextView>(Resource.Id.lblJumlahSiasatanLajut);
                var lblJumlahGambarSiasatan = View.FindViewById<TextView>(Resource.Id.lblJumlahGambarSiasatan);
                var lblNoSiasatanAkhir = View.FindViewById<TextView>(Resource.Id.lblNoSiasatanAkhir);

                lblLatitud = View.FindViewById<TextView>(Resource.Id.lblLatitud);
                lblLongitud = View.FindViewById<TextView>(Resource.Id.lblLongitud);

                lblLatitud.Text = GeneralBll.GetLastSaveLatitude();
                lblLongitud.Text = GeneralBll.GetLastSaveLongitude();


                var handheldData = HandheldBll.GetHandheldData();
                if (handheldData.Success && handheldData.Datas != null)
                {
                    var data = handheldData.Datas;

                    lblJumlahKPP.Text = data.Jumlah_Kpp.ToString();
                    lblJumlahGambarKPP.Text = data.Jumlah_Gambar_Kpp.ToString();
                    if (data.NotUrutan_Kpp == 0)
                    {
                        lblNoKPPAkhir.Text = "-";
                    }
                    else
                    {
                        lblNoKPPAkhir.Text = HandheldBll.GenerateNoRujukanForSummary(data, Enums.PrefixType.KPP);
                    }

                    lblJumlahKompaun.Text = data.Jumlah_Kots.ToString();
                    lblJumlahGambarKompaun.Text = data.Jumlah_Gambar_Kots.ToString();
                    if (data.NotUrutan_Kots == 0)
                    {
                        lblNoKompaunAkhir.Text = "-";
                    }
                    else
                    {
                        lblNoKompaunAkhir.Text = HandheldBll.GenerateNoRujukanForSummary(data, Enums.PrefixType.KOTS);
                    }

                    lblJumlahSiasatanLajut.Text = data.Jumlah_DataKes.ToString();
                    lblJumlahGambarSiasatan.Text = data.Jumlah_Gambar_DataKes.ToString();
                    if (data.NotUrutan_DataKes == 0)
                    {
                        lblNoSiasatanAkhir.Text = "-";
                    }
                    else
                    {
                        lblNoSiasatanAkhir.Text = HandheldBll.GenerateNoRujukanForSummary(data, Enums.PrefixType.SiasatLanjutan);
                    }

                }
                else
                {
                    GeneralAndroidClass.ShowModalMessage(this.Activity, "Ralat Mendapatkan Data Handheld");
                }

                UpdateTime();

                _timer = new Timer();
                _timer.Interval = 1000;
                _timer.Elapsed += Timer_Elapsed; ;
                _timer.Start();

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "Timer_Elapsed", ex.Message, Enums.LogType.Error);
            }
          

           

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _index += 1;
               this.Activity.RunOnUiThread(UpdateTime);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "Timer_Elapsed", ex.Message, Enums.LogType.Error);
            }

        }

        private void UpdateTime()
        {
            var localDate = GeneralBll.GetLocalDateTime();
            lblTarikh.Text = localDate.ToString(Constants.DateFormatDisplay);
            lblMasa.Text = localDate.ToString(Constants.TimeFormatDisplay);

            if (_index > 10)
            {
                lblLatitud.Text = GeneralBll.GetLastSaveLatitude();
                lblLongitud.Text = GeneralBll.GetLastSaveLongitude();
                _index = 0;
            }
        }

        public override void OnDestroyView()
        {
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Dispose();
            }
            base.OnDestroyView();
        }
    }
}