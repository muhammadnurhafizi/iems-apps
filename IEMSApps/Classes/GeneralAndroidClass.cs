using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.Services;
using IEMSApps.Utils;
using Plugin.Geolocator;
using Log = IEMSApps.Utils.Log;

namespace IEMSApps.Classes
{
    public class GeneralAndroidClass
    {
        public static void ShowToast(string sMessage)
        {
            Toast.MakeText(Application.Context, sMessage, ToastLength.Long).Show();
        }

        public static List<string> ListPermissions()
        {
            var listPermission = new List<string>();

            if ((int)Build.VERSION.SdkInt < 23)
            {
                ShowToast("Versi Pembangunan Android: " + Build.VERSION.SdkInt.ToString());
                return listPermission;
            }

            foreach (var permission in Constants.Permissions)
            {
                var result = ContextCompat.CheckSelfPermission(Application.Context, permission);
                if (result != Permission.Granted)
                {
                    listPermission.Add(permission);
                }
            }

            return listPermission;
        }


        public static int GetBatteryPercentage()
        {
            try
            {
                var filter = new IntentFilter(Intent.ActionBatteryChanged);
                var battery = Application.Context.RegisterReceiver(null, filter);
                int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
                int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);

                return (int)Math.Floor(level * 100D / scale);
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        public static AlertDialog GetDialogCustom(Context context)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context, Resource.Style.CustomDialogAlert);
            AlertDialog ad = builder.Create();
            ad.SetTitle(Constants.AppName);
            ad.SetIcon(Resource.Drawable.logoicon);



            return ad;
        }

        public static AlertDialog GetReceiptDetail(Context context) {

            AlertDialog.Builder builder = new AlertDialog.Builder(context, Resource.Style.CustomDialogAlert);

            AlertDialog ad = builder.Create();
            ad.SetTitle("Resit Ipayment");
            ad.SetIcon(Resource.Drawable.logoicon);
            return ad;
        }


        public static void ShowModalMessage(Activity ctx, string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(ctx, Resource.Style.CustomDialogAlert);
            AlertDialog ad = builder.Create();
            ad.SetTitle(Constants.AppName);
            ad.SetIcon(Resource.Drawable.logoicon);

            ad.SetMessage(message);
            ad.SetButton2("OK", (s, e) => { });
            ad.Show();
        }

        public static void ShowModalMessageHtml(Activity ctx, string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(ctx, Resource.Style.CustomDialogAlert);
            AlertDialog ad = builder.Create();
            ad.SetTitle(Constants.AppName);
            ad.SetIcon(Resource.Drawable.logoicon);

            ad.SetMessage(Html.FromHtml(message));
            ad.SetButton2("OK", (s, e) => { });
            ad.Show();
        }

        public static void LogData(string className, string functionName, string errorMessage, Enums.LogType logType)
        {
            Log.WriteLogFile(className, functionName, errorMessage, logType);
            ShowToast(errorMessage);
        }

        public static AlertDialog ShowProgressDialog(Context ctx, string message, bool SetCancelable = true)
        {

            int llPadding = 30;
            LinearLayout ll = new LinearLayout(ctx);
            ll.Orientation = Orientation.Vertical;
            ll.SetPadding(llPadding, llPadding, llPadding, llPadding);
            ll.SetGravity(GravityFlags.Center);
            LinearLayout.LayoutParams llParam = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WrapContent,
                LinearLayout.LayoutParams.WrapContent);
            llParam.Gravity = GravityFlags.Center;
            ll.LayoutParameters = llParam;

            ProgressBar progressBar = new ProgressBar(ctx);
            progressBar.Indeterminate = true;
            progressBar.SetPadding(0, 0, llPadding, 10);
            progressBar.LayoutParameters = llParam;

            llParam = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent);
            llParam.Gravity = GravityFlags.Center;

            TextView tvText = new TextView(ctx);
            tvText.SetText(message, TextView.BufferType.Normal);
            tvText.SetTextColor(Color.ParseColor("#000000"));
            tvText.SetTextSize(ComplexUnitType.Dip, 14);
            tvText.LayoutParameters = llParam;

            ll.AddView(progressBar);
            ll.AddView(tvText);

            AlertDialog.Builder builder = new AlertDialog.Builder(ctx);
            builder.SetCancelable(SetCancelable);
            builder.SetView(ll);

            var dialog = builder.Create();
            dialog.Show();
            Window window = dialog.Window;
            if (window != null)
            {
                var layoutParams = new WindowManagerLayoutParams();
                layoutParams.CopyFrom(dialog.Window.Attributes);
                layoutParams.Width = LinearLayout.LayoutParams.WrapContent;
                layoutParams.Height = LinearLayout.LayoutParams.WrapContent;
                dialog.Window.Attributes = layoutParams;
            }
            return dialog;
        }

        public static void StartLocationService(Context ctx)
        {
            try
            {
                ctx.StartService(new Intent(ctx, typeof(LocationService)));
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("GeneralAndroidClass", "StartLocationService", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace", ex.StackTrace, Enums.LogType.Error);
            }
        }

        public static void StopLocationService(Context ctx)
        {
            try
            {
                ctx.StopService(new Intent(ctx, typeof(LocationService)));

            }
            catch (Exception ex)
            {
                Log.WriteLogFile("GeneralAndroidClass", "StopLocationService", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace", ex.StackTrace, Enums.LogType.Error);
            }
        }

        public static bool IsGpsEnable()
        {
            var locator = CrossGeolocator.Current;

            return locator.IsGeolocationEnabled;

        }

        public static bool IsPrinterExist()
        {
            GlobalClass.BluetoothAndroid = new BluetoothAndroid();
//#if DEBUG
//            return true;
//#endif
            var openresult = GlobalClass.BluetoothAndroid.BluetoothOpen();

            if (!openresult.Succes)
            {
                return false;
            }

            var result = GlobalClass.BluetoothAndroid.BluetoothScan();
            return result != 0;
        }

        public static bool IsOnline()
        {
            Java.Lang.Runtime runtime = Java.Lang.Runtime.GetRuntime();
            try
            {
                Java.Lang.Process ipProcess = runtime.Exec("/system/bin/ping -c 1 8.8.8.8");
                int exitValue = ipProcess.WaitFor();
                return (exitValue == 0);
            }
            catch (Java.IO.IOException e)
            {
                Log.WriteLogFile("IOException General IsOnline : " + e.StackTrace);
                //e.PrintStackTrace();
            }
            catch (Java.Lang.InterruptedException e)
            {
                Log.WriteLogFile("InterruptedException General IsOnline : " + e.StackTrace);
                e.PrintStackTrace();
            }

            return false;
        }

        public static void StartBackgroundService(Context ctx)
        {
            try
            {
                //if (GeneralBll.IsBackgroundServiceRunning())
                //{
                //    Log.WriteLogBackgroundService("CheckBackgroundService");
                   
                //}
                //else
                //{
                //    ctx.StartService(new Intent(ctx, typeof(BackgroundService)));
                //    SharedPreferences.SaveString(SharedPreferencesKeys.IsBackgroundServiceRunning, "1");
                //}

              
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("GeneralAndroidClass", "StopLocationService", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace", ex.StackTrace, Enums.LogType.Error);
            }
        }

        public static void StopBackgroundService(Context ctx)
        {
            try
            {
                //ctx.StopService(new Intent(ctx, typeof(BackgroundService)));
                //SharedPreferences.SaveString(SharedPreferencesKeys.IsBackgroundServiceRunning, "0");
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("GeneralAndroidClass", "StopLocationService", ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace", ex.StackTrace, Enums.LogType.Error);
            }
        }

        public static void CheckBackgroundService(Context ctx)
        {
            try
            {
                //Log.WriteLogBackgroundService("CheckBackgroundService");

                //if (!GeneralBll.IsBackgroundServiceRunning())
                //{
                //    StartBackgroundService(ctx);
                //}
            }
            catch (Exception ex)
            {
                //Log.WriteLogBackgroundService("Service stopped");
                //Log.WriteLogBackgroundService("Service stopped");

                Log.WriteLogBackgroundService("CheckBackgroundService : " +  ex.Message);
                //Log.WriteLogBackgroundService("StackTrace", ex.StackTrace, Enums.LogType.Error);
            }
        }

        public static string ValidateRecordByLine(string sLine, int lenStart, int lenEnd, ref int totalLen)
        {
            totalLen = lenStart + lenEnd;
            if (sLine.Length >= totalLen)
                return (sLine.Substring(lenStart, lenEnd)).Trim();

            return (" ".PadLeft(lenEnd)).Trim();

        }

        public static string SetLine(string sLine, string sValue, int iLen)
        {
            if (string.IsNullOrEmpty(sValue))
                sLine += " ".PadLeft(iLen);
            else if (sValue.Length < iLen)
                sLine += sValue + " ".PadLeft(iLen - sValue.Length);
            else if (sValue.Length > iLen)//cut if more than len
                sLine += sValue.Substring(0, iLen);
            else
                sLine += sValue;

            return sLine;
        }
        public static bool IsRegisterPrinter(string strAddress)
        {
            string strPrinterAddress = "";
            bool bReturn = false;

            if (!GeneralBll.IsFileExist(Constants.LASTTRANS, false))
                return bReturn;

            string fullpathfileName = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + Constants.LASTTRANS;

            var objInfo = new StreamReader(fullpathfileName);

            try
            {
                string sLine;
                int len = 0;

                while ((sLine = objInfo.ReadLine()) != null)
                {
                    if (sLine.Length > 0)
                    {
                        strPrinterAddress = ValidateRecordByLine(sLine, 0, 18, ref len).Trim();
                        if (strPrinterAddress == strAddress)
                        {
                            bReturn = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("Error IsRegisterPrinter : " + ex.Message);
            }
            finally
            {

                objInfo.Close();
                objInfo.Dispose();
            }

            return bReturn;
        }

        public static void RegisterPrinter(string strAddress)
        {

            string sLine = "";
            string strFullFileName;

            sLine = SetLine(sLine, strAddress, 18);

            sLine += "\r\n";

            strFullFileName = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath + Constants.LASTTRANS;

            var objInfo = new StreamWriter(strFullFileName, true);

            objInfo.Write(sLine);
            objInfo.Close();
            objInfo.Dispose();
        }

    }
}