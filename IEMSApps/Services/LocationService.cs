using System;
using Android.App;
using Android.Content;
using Android.OS;
using IEMSApps.BLL;
using IEMSApps.Classes;
using IEMSApps.Utils;
using Plugin.Geolocator;

namespace IEMSApps.Services
{
    [Service]
    public class LocationService : Service
    {

        public override IBinder OnBind(Intent intent) { return null; }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                InitGeoPlugin();
            }
            catch (Exception ex)
            {
                Log.WriteLogFileServices("Service NOT started : " + ex.Message);
                Log.WriteLogFileServices("Stack Trace : " + ex.StackTrace);
            }

            return StartCommandResult.NotSticky;

        }

        private void InitGeoPlugin()
        {
            if (GeneralBll.GetStatusGps() == Constants.GpsStatus.GpsOn)
            {
                return;
            }
            var configDto = ConfigAccess.GetConfigAccess();

            Log.WriteLogFileServices("Location Service Status, IsListening : " + CrossGeolocator.Current.IsListening);

            if (!CrossGeolocator.Current.IsListening)
            {
                CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(configDto.IntervalInSecond), configDto.DistanceInMeter, true,
                   new Plugin.Geolocator.Abstractions.ListenerSettings
                   {
                       ActivityType = Plugin.Geolocator.Abstractions.ActivityType.AutomotiveNavigation,
                       AllowBackgroundUpdates = true,
                       DeferLocationUpdates = true,
                       DeferralDistanceMeters = 1,
                       DeferralTime = TimeSpan.FromSeconds(1),
                       ListenForSignificantChanges = true,
                       PauseLocationUpdatesAutomatically = false
                   }).GetAwaiter().GetResult();

                //Log.WriteLogFileServices("GPS Service Started");
                CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
            }
        }

        private void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {

            var position = e.Position;
            if (position != null)
            {
                var lastLongitude = GeneralBll.GetLastSaveLongitude();
                var lastLatitude = GeneralBll.GetLastSaveLatitude();

                var currentLongitude = position.Longitude.ToString("f6");
                var currentLatitude = position.Latitude.ToString("f6");

                if (lastLongitude != currentLongitude || lastLatitude != currentLatitude)
                {

                    SharedPreferences.SaveString(SharedPreferencesKeys.LocationLongitude, currentLongitude);
                    SharedPreferences.SaveString(SharedPreferencesKeys.LocationLatitude, currentLatitude);

                    GpsLogBll.Save(currentLongitude, currentLatitude);

                }

            }
            //Log.WriteLogFileServices("GPS Current_PositionChanged");
        }


        public override void OnDestroy()
        {

            CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;

            base.OnDestroy();

            if (CrossGeolocator.Current.IsListening)
            {
                CrossGeolocator.Current.StopListeningAsync().GetAwaiter().GetResult();
            }
            SharedPreferences.SaveString(SharedPreferencesKeys.StatusGps, Constants.GpsStatus.GpsOff);
            //Log.WriteLogFileServices("GPS OnDestroy");
        }
    }
}