using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.Utils;
using Java.Lang;
using Java.Util;


namespace IEMSApps.Services
{
    public class BluetoothPrintService
    {
        const string TAG = "BluetoothPrintService";

        static UUID UUID_SPP = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        public const int STATE_NONE = 0;       // we're doing nothing
        public const int STATE_LISTEN = 1;     // now listening for incoming connections
        public const int STATE_CONNECTING = 2; // now initiating an outgoing connection
        public const int STATE_CONNECTED = 3;  // now Sambungan kea remote device

        int state;
        Handler handler;
        ConnectThread connectThread;
        ConnectedThread connectedThread;

        // Constructor. Prepares a new Bluetooth session.
        public BluetoothPrintService(Handler handler)
        {
            state = STATE_NONE;
            this.handler = handler;
        }

        // Return the current connection state.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetState()
        {
            return state;
        }

        // Start the print service. Called by the Activity onResume().
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            // Cancel any thread attempting to make a connection
            if (connectThread != null)
            {
                connectThread.Cancel();
                connectThread = null;
            }
            // Cancel any thread currently running a connection
            if (connectedThread != null)
            {
                connectedThread.Cancel();
                connectedThread = null;
            }
            state = STATE_LISTEN;
        }

        // Stop all threads.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (connectThread != null)
            {
                connectThread.Cancel();
                connectThread = null;
            }
            if (connectedThread != null)
            {
                connectedThread.Cancel();
                connectedThread = null;
            }
            state = STATE_NONE;
        }

        // Start the ConnectThread to initiate a connection to a remote device.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Connect(BluetoothDevice device)
        {
            // Cancel any thread attempting to make a connection
            if (state == STATE_CONNECTING)
            {
                if (connectThread != null)
                {
                    connectThread.Cancel();
                    connectThread = null;
                }
            }
            // Cancel any thread currently running a connection
            if (connectedThread != null)
            {
                connectedThread.Cancel();
                connectedThread = null;
            }
            // Start the thread to connect with the given device
            connectThread = new ConnectThread(device, this);
            connectThread.Start();
        }

        // Start the ConnectedThread to begin managing a Bluetooth connection
        [MethodImpl(MethodImplOptions.Synchronized)]
        void Connected(BluetoothSocket socket, BluetoothDevice device)
        {
            // Cancel the thread that completed the connection
            if (connectThread != null)
            {
                connectThread.Cancel();
                connectThread = null;
            }
            // Cancel any thread currently running a connection
            if (connectedThread != null)
            {
                connectedThread.Cancel();
                connectedThread = null;
            }
            // Start the thread to manage the connection and perform transmissions
            connectedThread = new ConnectedThread(socket, this);
            connectedThread.Start();

            //// Send the name of the connected device back to the UI Activity
            //var msg = handler.ObtainMessage(MainActivity.MESSAGE_DEVICE_NAME);
            //Bundle bundle = new Bundle();
            //bundle.PutString(MainActivity.DEVICE_NAME, device.Name);
            //msg.Data = bundle;
            //handler.SendMessage(msg);
        }

        // Write to the ConnectedThread in an unsynchronized manner
        public void Write(byte[] data)
        {
            // Create temporary object
            ConnectedThread r;

            // Synchronize a copy of the ConnectedThread
            lock (this)
            {
                if (state != STATE_CONNECTED)
                {
                    return;
                }
                r = connectedThread;
            }
            // Perform the write unsynchronized
            r.Write(data);
        }

        // Indicate that the connection attempt failed and notify the UI Activity.
        void ConnectionFailed()
        {
            // When the application is destroyed, just return
            if (state == STATE_NONE)
            {
                return;
            }

            //handler.ObtainMessage(MainActivity.MESSAGE_TOAST, Resource.String.connect_fail, 0).SendToTarget();

            // Start the service over to restart listening mode
            Start();
        }

        // Indicate that the connection was lost and notify the UI Activity.
        void ConnectionLost()
        {
            // When the application is destroyed, just return
            if (state == STATE_NONE)
            {
                return;
            }

            // handler.ObtainMessage(MainActivity.MESSAGE_TOAST, Resource.String.connect_lost, 0).SendToTarget();

            // Start the service over to restart listening mode
            Start();
        }

        /// <summary>
        /// This thread runs while attempting to make an outgoing connection
        /// with a device. It runs straight through; the connection either
        /// succeeds or fails.
        /// </summary>
        protected class ConnectThread : Thread
        {
            BluetoothSocket socket;
            readonly BluetoothDevice device;
            BluetoothPrintService service;

            public ConnectThread(BluetoothDevice device, BluetoothPrintService service)
            {
                this.device = device;
                this.service = service;
                BluetoothSocket tmp = null;

                try
                {
                    tmp = device.CreateInsecureRfcommSocketToServiceRecord(UUID_SPP);
                }
                catch (Java.IO.IOException e)
                {
                    Log.WriteLogFile(TAG, "create() failed", e.Message, Enums.LogType.Error);
                }
                socket = tmp;
                service.state = STATE_CONNECTING;
            }

            public override void Run()
            {
                // Always cancel discovery because it will slow down connection
                BluetoothAdapter.DefaultAdapter.CancelDiscovery();

                // Make a connection to the BluetoothSocket
                try
                {
                    // This is a blocking call and will only return on a successful connection or an exception
                    socket.Connect();
                }
                catch (Java.IO.IOException e)
                {
                    Log.WriteLogFile(TAG, "unable to connect()", e.Message, Enums.LogType.Error);
                    // Close the socket
                    try
                    {
                        socket.Close();
                    }
                    catch (Java.IO.IOException e2)
                    {
                        Log.WriteLogFile(TAG, "unable to close() socket during connection failure", e2.Message, Enums.LogType.Error);
                    }

                    // Start the service over to restart listening mode
                    service.ConnectionFailed();
                    return;
                }
                // Reset the ConnectThread because we're done
                lock (this)
                {
                    service.connectThread = null;
                }
                // Start the connected thread
                service.Connected(socket, device);
            }

            public void Cancel()
            {
                try
                {
                    socket.Close();
                }
                catch (Java.IO.IOException e)
                {
                    Log.WriteLogFile(TAG, "Cancel() close() of connect socket failed", e.Message, Enums.LogType.Error);
                }
            }
        }

        /// <summary>
        /// This thread runs during a connection with a remote device.
        /// It handles all incoming and outgoing transmissions.
        /// </summary>
        class ConnectedThread : Thread
        {
            BluetoothSocket socket;
            Stream inStream;
            Stream outStream;
            BluetoothPrintService service;

            public ConnectedThread(BluetoothSocket socket, BluetoothPrintService service)
            {
                this.socket = socket;
                this.service = service;
                Stream tmpIn = null;
                Stream tmpOut = null;

                // Get the BluetoothSocket input and output streams
                try
                {
                    tmpIn = socket.InputStream;
                    tmpOut = socket.OutputStream;
                }
                catch (Java.IO.IOException e)
                {
                    Log.WriteLogFile(TAG, "ConnectedThread()", e.Message, Enums.LogType.Error);
                }
                inStream = tmpIn;
                outStream = tmpOut;
                service.state = STATE_CONNECTED;
            }

            public override void Run()
            {
                byte[] buffer = new byte[1024];
                int bytes;

                // Keep listening to the InputStream while connected
                while (service.GetState() == STATE_CONNECTED)
                {
                    try
                    {
                        // Read from the InputStream
                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        // buffer can be over-written by next input stream data, so it should be copied
                        byte[] rcvData = Arrays.CopyOf(buffer, bytes);

                        //// Send the obtained bytes to the UI Activity
                        service.handler.ObtainMessage(Constants.PrinterMessage.MESSAGE_READ, bytes, -1, rcvData).SendToTarget();
                    }
                    catch (Java.IO.IOException e)
                    {
                        Log.WriteLogFile(TAG, "Run()", e.Message, Enums.LogType.Error);
                        service.ConnectionLost();
                        break;
                    }
                }
            }

            // Write to the connected OutStream.
            public void Write(byte[] buffer)
            {
                try
                {
                    outStream.Write(buffer, 0, buffer.Length);
                }
                catch (Java.IO.IOException e)
                {
                    Log.WriteLogFile(TAG, "Write()", e.Message, Enums.LogType.Error);
                }
            }

            public void Cancel()
            {
                try
                {
                    socket.Close();
                }
                catch (Java.IO.IOException e)
                {
                    Log.WriteLogFile(TAG, "Cancel()", e.Message, Enums.LogType.Error);
                }
            }
        }

      
}
}