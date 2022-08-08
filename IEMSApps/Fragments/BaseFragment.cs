
using Android.App;
using Android.Content;
using IEMSApps.Classes;

namespace IEMSApps.Fragments
{
    public class BaseFragment : Fragment
    {
        private AlertDialog _dialog;
        public void IsLoading(Context context, bool value, string mesage = "")
        {
            Activity.RunOnUiThread(() =>
            {
                if (value)
                {
                    _dialog?.Dismiss();
                    _dialog = GeneralAndroidClass.ShowProgressDialog(context, mesage);
                }
                else
                {
                    _dialog?.Dismiss();
                }
            });
        }
    }
}