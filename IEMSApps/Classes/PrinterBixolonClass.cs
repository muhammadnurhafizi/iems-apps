using IEMSApps.Utils;
using Plugin.BxlMpXamarinSDK;
using Plugin.BxlMpXamarinSDK.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IEMSApps.Classes
{
    public class PrinterBixolonClass
    {
        private MPosControllerPrinter _printer;
        MposConnectionInformation _connectionInfo;
        private static SemaphoreSlim _printSemaphore = new SemaphoreSlim(1, 1);

        public async Task<MPosControllerDevices> OpenPrinterService(MposConnectionInformation connectionInfo)
        {
            if (connectionInfo == null)
            {
                Log.WriteLogFile("OpenPrinterService", "MposConnectionInformation connectionInfo : " + connectionInfo, Enums.LogType.Debug);
                return null;
            }

            if (_printer != null)
                return _printer;

            _printer = MPosDeviceFactory.Current.createDevice(MPosDeviceType.MPOS_DEVICE_PRINTER) as MPosControllerPrinter;

            switch (connectionInfo.IntefaceType)
            {
                case MPosInterfaceType.MPOS_INTERFACE_BLUETOOTH:
                case MPosInterfaceType.MPOS_INTERFACE_WIFI:
                case MPosInterfaceType.MPOS_INTERFACE_ETHERNET:
                    _printer.selectInterface((int)connectionInfo.IntefaceType, connectionInfo.Address);
                    _printer.selectCommandMode((int)(false ? MPosCommandMode.MPOS_COMMAND_MODE_DEFAULT : MPosCommandMode.MPOS_COMMAND_MODE_BYPASS));
                    break;
                default:
                    //await DisplayAlert("Connection Fail", "Not Supported Interface", "OK");
                    return null;
            }

            await _printSemaphore.WaitAsync();

            try
            {
                var result = await _printer.openService();
                if (result != (int)MPosResult.MPOS_SUCCESS)
                {
                    _printer = null;
                    GeneralAndroidClass.ShowToast("openService failed. (" + result.ToString() + ")");
                }
            }
            finally
            {
                _printSemaphore.Release();
            }

            return _printer;
        }

        public async Task<uint> CheckPrinterBixolonStatus(MPosControllerPrinter printer)
        {
            var message = "";
            var status = await printer.checkPrinterStatus();
            switch (status)
            {
                case 0:
                    message = "Printing Is Possible";
                    break;
                case 1:
                    message = "Cover Open";
                    break;
                case 2:
                    message = "No Paper";
                    break;
                case 4:
                    message = "Printing Is Possible";
                    break;
                case 8:
                    message = "Error (Offline or Unkown Error)";
                    break;
                case 128:
                    message = "Offline by the Printer's power off";
                    break;
                case 1006:
                    message = "Failed to connect the device";
                    break;
                case 1005:
                    message = "No Response from the device";
                    break;
                default:
                    message = status.ToString();
                    break;

            }
            GeneralAndroidClass.ShowToast(message);
            Log.WriteLogFile("CheckPrinterBixolonStatus : ", message, Enums.LogType.Debug);

            return status;
        }

        public async Task<int> CheckPrinter(MPosControllerPrinter _printer)
        {
            int status = 1;
            try
            {
                if (_printer == null)
                {
                    GeneralAndroidClass.ShowToast("Printer Not Avalaible");
                    Log.WriteLogFile("CheckPrinter", "OpenPrinterService : Null", Enums.LogType.Debug);
                    status = 2;
                }
                else
                {
                    //GeneralAndroidClass.ShowToast("Printer Avalaible");
                    Log.WriteLogFile("CheckPrinter", "OpenPrinterService : Printer Avalaible", Enums.LogType.Debug);
                    //await ShowMessageNew(true, "Printer Avalaible");
                    Thread.Sleep(Constants.DefaultWaitingMilisecond);
                    status = 1;
                }

            }
            catch (Exception ex)
            {
                Log.WriteLogFile("CheckPrinter", "CatchOn : " + ex.Message, Enums.LogType.Error);
            }

            return status;
        }

        public void ResetPrinterConnection()
        {
            bool success = false;
            var message = "Tidak Berjaya";
            try
            {
                //try to reset connection
                _printer = null;
                if (_printer == null)
                {
                    success = true;
                    message = "Berjaya";
                }
                GeneralAndroidClass.ShowToast("Reset _printer : " + message);
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("CheckPrinterBixolonStatus : ", ex.Message, Enums.LogType.Error);
            }
        }
    }
}