using Android.App;
using Android.Bluetooth;
using IEMSApps.Services;

namespace IEMSApps.Classes
{
    public class GlobalClass : Application
    {
        public static BluetoothAndroid BluetoothAndroid { get; set; }

        public static BluetoothDevice BluetoothDevice { get; set; }

        //public static AFUtil AfUtil { get; set; }

        //public static Context CurrentContext { get; set; }

        public static string FwCode { get; set; }
        public static  BluetoothPrintService printService { get; set; }
    }
}