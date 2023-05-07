using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;

namespace IEMSApps.Activities
{
    [Activity(MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : BaseActivity
    {
        private TextView _lblInfo;
        private static int PERMISSION_REQUEST_CODE = 10;
        private AlertDialog _dialog;
        private EditText _txtPiranti;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SplashScreen);

            var lblVersion = FindViewById<TextView>(Resource.Id.lblVersion);
            lblVersion.Text = Constants.AppVersion;

            var lblAppName = FindViewById<TextView>(Resource.Id.lblTitle);
            lblAppName.Text = Constants.AppName;

            //SharedPreferences.SaveString(SharedPreferencesKeys.IsUserLogin, "");
            //SharedPreferences.SaveString(SharedPreferencesKeys.IsBackgroundServiceRunning, "0");

            _lblInfo = FindViewById<TextView>(Resource.Id.lblInfo);
            new Handler().PostDelayed(SetInit, 2000L);
        }

        private void SetInit()
        {
            try
            {

                var listPermission = GeneralAndroidClass.ListPermissions();
                //check permission first
                if (listPermission.Count > 0)
                {
                    bool isRationale = false;

                    foreach (var permission in listPermission)
                    {
                        if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
                        {
                            isRationale = true;
                            break;
                        }
                    }
                    if (isRationale)
                    {
                        Log.WriteLogFile(Class.Name, "Permission", listPermission.ToString(), Enums.LogType.Error);

                        var view = FindViewById(Resource.Id.linearSnack);
                        //Explain to the user why we need to read the contacts
                        Snackbar.Make(view, "Semua Kebenaran Diperlukan Untuk Aplikasi Ini.", Snackbar.LengthIndefinite)
                            .SetAction("OK", v => ActivityCompat.RequestPermissions(this, listPermission.ToArray(), PERMISSION_REQUEST_CODE))
                            .Show();
                        //GeneralAndroidClass.ShowToast("All Permission are needed to grant to use this application.");
                        return;
                    }

                    ActivityCompat.RequestPermissions(this, listPermission.ToArray(), PERMISSION_REQUEST_CODE);

                }
                else
                {
                    GeneralBll.InitFolder();
                    GeneralBll.InitConfig();
                    if (!GeneralBll.IsDatabaseExist())
                    {

                        var result = DatabaseBll.CreateDefaultDatabase();

                        //GeneralAndroidClass.ShowToast("result Create : " + result);

                        //var data = DataAccessQuery<TbPasukanHeader>.ExecuteSelectSql("select * from tbpasukan_header");
                        //GeneralAndroidClass.ShowToast("Data : " + data.Count);
                    }
                    else
                    {
                        var result = DatabaseBll.AlterDatabase();
                        //GeneralAndroidClass.ShowToast("result Alter : " + result);
                    }

                    CheckVersion();
                }


            }
            catch (Exception ex)
            {
                //GeneralAndroidClass.ShowToast(ex.Message);
                //Log.WriteLogFile(Class.Name, "SetInit", ex.Message, Enums.LogType.Error);
                StartLogin();
            }
        }

        private void CheckVersion()
        {
            CheckNewVersion(StartLoginOrShowConfig);
        }

        private void StartLoginOrShowConfig()
        {
            if (GeneralBll.IsHandheldExists())
            {
                StartLogin();
            }
            else
            {
                ShowNoConfigModal();
            }
        }

        private void ShowNoConfigModal()
        {
            var dialogView = View.Inflate(this, Resource.Layout.NoConfigLayout, null);
            var alertDialog = new AlertDialog.Builder(this).Create();

            var btnStart = dialogView.FindViewById<Button>(Resource.Id.btnStart);
            _txtPiranti = dialogView.FindViewById<EditText>(Resource.Id.txtPiranti);
            _txtPiranti.Text = "";
            bool isStartClick = false;

            btnStart.Click += (sender, e) =>
            {
                if (string.IsNullOrEmpty(_txtPiranti.Text))
                {
                    GeneralAndroidClass.ShowModalMessage(this, "ID Peranti tidak ditemui. Sila masukkan ID Peranti Yang Sah");
                    return;
                }
                isStartClick = true;

                //_dialog = GeneralAndroidClass.ShowProgressDialog(this, "Get data...Please wait.");
                //var result = HandheldBll.GetRecordHandheldAsync(_txtPiranti?.Text);
                //
                //if (result.Success)
                //{
                Intent intent = new Intent(this, typeof(LoadingActivity));
                intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                intent.PutExtra(Constants.PIRANTIID, _txtPiranti.Text);
                StartActivity(intent);

                alertDialog.Dismiss();
                //}
                //else
                //{
                //    Toast.MakeText(this, result.Mesage, ToastLength.Long).Show();
                //    _dialog?.Dismiss();
                //}
            };

            alertDialog.SetView(dialogView);

            alertDialog.DismissEvent += (sender, e) =>
            {
                if (!isStartClick)
                {
                    ShowNoConfigModal();
                }

            };

            alertDialog.Show();

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            var view = FindViewById(Resource.Id.linearSnack);
            if (requestCode == PERMISSION_REQUEST_CODE)
            {
                bool isGranted = true;

                foreach (var grantResult in grantResults)
                {
                    if (grantResult == Permission.Denied)
                    {
                        isGranted = false;
                        break;
                    }
                }
                if (isGranted)
                {
                    SetInit();
                }
                else
                {
                    //Snackbar.Make(view, "Permission Denied ! Please restart application", Snackbar.LengthLong).Show();
                    Snackbar.Make(view, "Tiada Kebenaran! Sila Mula Semula Aplikasi", Snackbar.LengthIndefinite)
                        .SetAction("OK", v => ExitApps())
                        .Show();

                }


            }
        }

        private void ExitApps()
        {
            //var intent = new Intent(this, typeof(ExitActivity));        
            //intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
            //StartActivity(intent);
            Finish();

        }

        private void StartLogin()
        {
            try
            {
                var intent = new Intent(this, typeof(Login));
                intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
                Finish();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("HttpClientService", "GetRecordAsync", ex.Message, Enums.LogType.Error);
            }
            finally
            {
                _dialog?.Dismiss();
            }
        }

    }
}