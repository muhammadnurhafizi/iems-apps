using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace IEMSApps.Fragments
{
    public class Home : Fragment
    {
        private TextView txtDate, txtTime;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.home_layout, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetInit(view);
        }

        private void SetInit(View view)
        {
            //txtDate = view.FindViewById<TextView>(Resource.Id.txtDate);
            //txtTime = view.FindViewById<TextView>(Resource.Id.txtTime);

            //var txtName = view.FindViewById<TextView>(Resource.Id.txtName);
            //var txtNoKp = view.FindViewById<TextView>(Resource.Id.txtNoKp);

            //if (txtName != null)
            //{
            //    txtName.Text = SharedPreferences.GetString(SharedPreferencesKeys.UserName);
            //}

            //if (txtNoKp != null)
            //{
            //    txtNoKp.Text = SharedPreferences.GetString(SharedPreferencesKeys.UserNoKp);
            //}

            //UpdateTime();

            //var timer = new Timer();
            //timer.Interval = 1000;
            //timer.Elapsed += Timer_Elapsed; ;
            //timer.Start();

        }

        //private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        this.Activity.RunOnUiThread(UpdateTime);
        //    }
        //    catch (Exception ex)
        //    {
        //        GeneralAndroidClass.LogData("Home", "Timer_Elapsed", ex.Message, Enums.LogType.Error);
        //    }

        //}

        //private void UpdateTime()
        //{
        //    var localDate = GeneralBll.GetLocalDateTime();
        //    txtDate.Text = localDate.ToString("dd/MM/yyyy");
        //    txtTime.Text = localDate.ToString("hh:mm:ss tt");
        //}
    }
}