using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using IEMSApps.BLL;
using System;
using System.Threading;
using System.Threading.Tasks;
using IEMSApps.Classes;

namespace IEMSApps.Services
{
    [Service]
    public class BackgroundService : Service
    {
        private static readonly string TAG = "X:" + typeof(BackgroundService).Name;
        private static readonly int TimerWait = 5 * 60000;
        private Timer timer;
        private DateTime startTime;
        private bool isStarted = false;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            //Log.Debug(TAG, $"OnStartCommand called at {startTime}, flags={flags}, startid={startId}");
            Utils.Log.WriteLogBackgroundService("Service OnStartCommand");

            if (isStarted)
            {
                //TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
                //Log.Debug(TAG, $"This service was already started, it's been running for {runtime:c}.");
                Utils.Log.WriteLogBackgroundService("Service was already started");
            }
            else
            {
                var interval = GeneralBll.GetIntervalBackgroundService() * 1000;

                startTime = DateTime.UtcNow;
                //Log.Debug(TAG, $"Starting the service, at {startTime}.");
                Utils.Log.WriteLogBackgroundService($"Starting the service, interval : {interval/1000} in second" );
                timer = new Timer(HandleTimerCallback, startTime, 0, interval);
                isStarted = true;
            }
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }

        public override void OnDestroy()
        {
            timer.Dispose();
            timer = null;
            isStarted = false;

            SharedPreferences.SaveString(SharedPreferencesKeys.IsBackgroundServiceRunning, "0");
            Utils.Log.WriteLogBackgroundService("Service stopped");
            base.OnDestroy();
        }

        private void HandleTimerCallback(object state)
        {
            try
            {
                TimeSpan runTime = DateTime.UtcNow.Subtract(startTime);
                //Log.Debug(TAG, $"This service has been running for {runTime:c} (since ${state}).");

                //Log.Debug(TAG, $"Send Data online");
                Utils.Log.WriteLogBackgroundService($"{TAG} \r Sending data {runTime:c}");

                var result = Task.Run(async () => await SendOnlineBll.SendDataOnlineFromService(null)).Result;

                //Log.Debug(TAG, $"Result : {result.Success}, Message : {result.Mesage}");
                Utils.Log.WriteLogBackgroundService($"{TAG} \r SendDataOnlineFromService \r Result : {result.Success}, Message : {result.Mesage}");
            }
            catch (Exception ex)
            {
                Utils.Log.WriteLogBackgroundService($"{TAG} \r SendDataOnlineFromService \r ErrorMessage : {ex.Message}");
                //Log.Debug(TAG, $"ERROR : {ex.Message}");
            }
        }
    }
}