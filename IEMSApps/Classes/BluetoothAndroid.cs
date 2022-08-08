using System;
using System.Collections.Generic;
using System.Linq;
using Android.Bluetooth;
using Java.IO;
using Java.Util;

namespace IEMSApps.Classes
{
    public class BluetoothAndroid
    {
        private BluetoothSocket socket;
        private BluetoothAdapter adapter;
        private PrintWriter oStream;
        public List<BluetoothDevice> _listDevice;

        public BluetoothAndroid()
        {
            _listDevice = new List<BluetoothDevice>();
        }

        /// <summary>
        /// Checks the adapter.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns></returns>
        private bool CheckAdapterIsNull(BluetoothAdapter adapter)
        {
            return adapter == null;
        }

        /// <summary>
        /// Bluetoothes the open.
        /// </summary>
        public ResponseBluetoothAndroid BluetoothOpen()
        {
            var response = new ResponseBluetoothAndroid();

            // Check if the bletooth adapter is null
            adapter = BluetoothAdapter.DefaultAdapter;
            if (CheckAdapterIsNull(adapter))
            {
                response.Succes = false;
                response.Message = "Tiada Adapter Bluetooth";
            }

            if (!adapter.IsEnabled)
            {
                adapter.Enable();
            }

            return response;
        }

        /// <summary>
        /// Bluetoothes the scan.
        /// </summary>
        /// <returns></returns>
        public int BluetoothScan()
        {
            _listDevice = new List<BluetoothDevice>();

            // Check adapter
            if (CheckAdapterIsNull(adapter))
            {
                return 0;
            }

            // Find object or Bluetooth device are paired with the local adapter
            var devices = adapter.BondedDevices;
            if (!devices.Any())
            {
                return 0;
            }

            foreach (var device in devices)
            {
                _listDevice.Add(device);
            }

            return _listDevice.Count();
        }

        public List<BluetoothDevice> GetListBluetooth()
        {
            var result = new List<BluetoothDevice>();

            // Check adapter
            if (CheckAdapterIsNull(adapter))
            {
                return result;
            }

            // Find object or Bluetooth device are paired with the local adapter
            var devices = adapter.BondedDevices;
            if (!devices.Any())
            {
                return result;
            }

            foreach (var device in devices)
            {
                result.Add(device);
            }

            return result;
        }

        /// <summary>
        /// Bluetoothes the connect.
        /// </summary>
        /// <param name="bluetoothDevice">The bluetooth device.</param>
        public ResponseBluetoothDevices BluetoothConnect(BluetoothDevice bluetoothDevice)
        {
            var response = new ResponseBluetoothDevices();

            try
            {
                // Check adapter
                if (CheckAdapterIsNull(adapter))
                {
                    // If the adapter is null.
                    response.Succes = false;
                    response.Message = "Tiada Adapter Bluetooth";
                    return response;
                }

                socket = bluetoothDevice.CreateRfcommSocketToServiceRecord(UUID.FromString(bluetoothDevice.GetUuids().ElementAt(0).ToString()));
                if (socket.IsConnected)
                    socket.Close();

                socket.Connect();

                response.Succes = true;
                response.Message = string.Format("Sambungan Bluetooth Ke {0}", bluetoothDevice.Name);
            }
            catch (Exception ex)
            {
                response.Succes = false;
                response.Message = string.Format("Tidak Dapat Disambungkan Ke Peranti, Sila Semak Peranti Anda. {0}", ex.Message);
                return response;
            }

            return response;
        }

        /// <summary>
        /// Bluetoothes the send.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public ResponseBluetoothAndroid BluetoothSendChar(byte[] buffer)
        {
            var response = new ResponseBluetoothAndroid();
            try
            {
                if (socket == null)
                {
                    response.Succes = false;
                    response.Message = "Sila Sambung Ke Peranti dan Cuba Semula";
                    return response;
                }

                oStream = new PrintWriter(socket.OutputStream, true);
                for (int i = 0; i < buffer.Length; i++)
                {
                    oStream.Write(buffer[i]);
                };
            }
            catch (Exception ex)
            {
                response.Succes = false;
                response.Message = string.Format("Sila Semak Pencetak, Ralat Cetakan: {0}", ex.Message);
                return response;
            }
            return response;
        }

        /// <summary>
        /// Bluetoothes the send.
        /// </summary>
        /// <param name="text">The text.</param>
        public ResponseBluetoothAndroid BluetoothSendText(string text)
        {
            var response = new ResponseBluetoothAndroid();
            try
            {
                if (socket == null)
                {
                    response.Succes = false;
                    response.Message = "Sila Sambung Ke Peranti dan Cuba Semula";
                    return response;
                }
                oStream = new PrintWriter(socket.OutputStream, true);
                oStream.Write(text);
                oStream.Flush();

                // diconect
                BluetoothDisconnect();
            }
            catch (Exception ex)
            {
                response.Succes = false;
                response.Message = string.Format("Sila Semak Pencetak, Ralat Cetakan: {0}", ex.Message);
                return response;
            }
            return response;
        }

        /// <summary>
        /// Bluetoothes the disconnect.
        /// </summary>
        public void BluetoothDisconnect()
        {
            oStream.Close();
            oStream.Dispose();

            if (socket.IsConnected)
            {
                socket.Close();
            }
        }

        /// <summary>
        /// Bluetoothes the close.
        /// </summary>
        public void BluetoothClose()
        {
            if (socket.IsConnected)
            {
                BluetoothDisconnect();
                socket.Dispose();
            }
        }
    }

    public class ResponseBluetoothAndroid
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ResponseBluetoothAndroid"/> is succes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if succes; otherwise, <c>false</c>.
        /// </value>
        public bool Succes { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseBluetoothAndroid"/> class.
        /// </summary>
        public ResponseBluetoothAndroid()
        {
            Succes = true;
            Message = "Berjaya";
        }
    }

    public class ResponseBluetoothDevices : ResponseBluetoothAndroid
    {
        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        public List<BluetoothDevice> Devices { get; set; }

        public ResponseBluetoothDevices()
        {
            Devices = new List<BluetoothDevice>();
        }
    }
}