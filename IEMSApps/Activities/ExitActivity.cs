using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace IEMSApps.Activities
{
    public class ExitActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            //just finish it
            FinishAndRemoveTask();
        }
    }
}