using Android.App;
using Android.Content;
using Android.Views;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using System;
using System.Threading.Tasks;
using Android.OS;
using IEMSApps.BLL;
using Android.Widget;
using Android.Util;

namespace IEMSApps.Activities
{
    public class BaseActivity : Activity
    {
        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    //if (GeneralBll.IsUserLogin())
        //    //{
        //    //    GeneralAndroidClass.CheckBackgroundService(this);
        //    //}

        //    base.OnCreate(savedInstanceState);
        //}

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            //#if DEBUG
            //            if (keyCode == Keycode.Back)
            //                return false;
            //#endif


            return base.OnKeyDown(keyCode, e);
        }

        public void CheckNewVersion(Action action)
        {
            var versionDto = Task.Run(async () => await HttpClientService.GetVersionAsync()).Result;
            if (versionDto.Success)
            {
                if (versionDto.Result[0].VersionId_Int > Constants.AppVersionValueForUpdate)
                {
                    var _alertDialog = GeneralAndroidClass.GetDialogCustom(this);
                    _alertDialog.SetCanceledOnTouchOutside(false);
                    _alertDialog.SetMessage(Constants.Messages.HaveNewVersion);

                    if (versionDto.Result[0].Priority == Constants.PriorityUpdate.Required)
                    {
                        _alertDialog.SetButton(Constants.InstallText, (s, e) =>
                        {
                            DownloadNewVersion(versionDto.Result[0].Url);
                            _alertDialog.Dismiss();
                            return;
                        });
                    }
                    else if (versionDto.Result[0].Priority == Constants.PriorityUpdate.NotRequired)
                    {
                        _alertDialog.SetButton(Constants.InstallText, (s, e) =>
                        {
                            DownloadNewVersion(versionDto.Result[0].Url);
                            _alertDialog.Dismiss();
                            return;
                        });
                        _alertDialog.SetButton2(Constants.SkipText, (s, e) =>
                        {
                            action();
                            _alertDialog.Dismiss();
                        });
                    }
                    _alertDialog.Show();
                }
                else
                    action();
            }
            else 
            {
                GeneralAndroidClass.ShowToast(versionDto.Mesage);
                GeneralAndroidClass.LogData("BaseActivity", "CheckNewVersion", versionDto.Mesage, Enums.LogType.Info);
                action(); 
            }
        }

        private void DownloadNewVersion(string url)
        {
            Intent intent = new Intent(this, typeof(LoadingActivity));
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            intent.PutExtra(Constants.LOADING_TYPE, Constants.LOADING_DOWNLOAD);
            intent.PutExtra(Constants.URL_NEW_APK, url);
            StartActivity(intent);
        }

        private AlertDialog _dialog;
        public void IsLoading(Context context, bool value, string mesage = "")
        {
            RunOnUiThread(() =>
            {
                //if (value)
                //{
                //    _dialog?.Dismiss();
                //    _dialog = GeneralAndroidClass.ShowProgressDialog(context, mesage);
                //}
                //else
                //{
                //    _dialog?.Dismiss();
                //}
                IsLoadingWithoutThread(context, value, mesage);
            });
        }

        public void IsLoadingWithoutThread(Context context, bool value, string message = "")
        {
            if (value)
            {
                _dialog?.Dismiss();
                _dialog = GeneralAndroidClass.ShowProgressDialog(context, message);
            }
            else
            {
                _dialog?.Dismiss();
            }
        }
    }
}