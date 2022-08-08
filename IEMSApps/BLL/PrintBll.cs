using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.Classes;
using IEMSApps.Utils;
using Java.IO;
using Java.Util;

namespace IEMSApps.BLL
{
    public class PrintBll
    {
        private BluetoothSocket socket;
        private BluetoothAdapter adapter;
        private PrintWriter oStream;
        public List<BluetoothDevice> _listDevice;

        //from vivo mobile
        //maintain for compatible issue
        //ESC   K    1 
        Byte[] ExtechFont0; // = new Byte[3] { 27, 107, 49 };   //36 columns                X

        //ESC   K    2
        Byte[] ExtechFont1 = new Byte[3] { 27, 33, 0 }; //48 columns                0+

        Byte[] ExtechFont2; // = new Byte[3] { 27, 33, 1 };   //57 columns                0-
        Byte[] ExtechFont3; // = new Byte[3] { 27, 107, 52 };   //64 columns                X
        Byte[] ExtechFont4; // = new Byte[3] { 27, 33, 1 };   //72 columns                0-
        Byte[] ExtechFont5; // = new Byte[3] { 27, 107, 54 };   //28 columns - monospace    X

        Byte[] FontBOLDON; // = new Byte[3] { 27, 69, 1 };
        Byte[] FontBOLDOFF; // = new Byte[3] { 27, 69, 0 };
        Byte[] FontDoubleHigh; // = new Byte[3] { 27, 33, 16 };
        Byte[] FontNormal = new Byte[3] { 27, 33, 0 }; //48 columns                0+
        Byte[] Contrast; // = new Byte[3] { 27, 33, 8 };
        Byte[] Peak_Power; // = new Byte[3] { 27, 80, 57 };               //         X
        Byte[] ResetBuffer; // = new Byte[1] { 24 };    

        public PrintBll()
        {
#if DEV
            return;
#endif
            oStream = null;
            adapter = BluetoothAdapter.DefaultAdapter;

            if (!adapter.IsEnabled)
            {
                adapter.Enable();
            }
        }

        private bool CheckAdapterIsNull(BluetoothAdapter adapter)
        {
            return adapter == null;
        }

        public ResponseBluetoothDevices BluetoothConnect(BluetoothDevice bluetoothDevice)
        {
            var response = new ResponseBluetoothDevices();
#if DEV
            response.Succes = true;
            return response;
#endif
            int retries = 0;
            bool isConnected = false;
            string message = "";

            do
            {
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

                    socket = bluetoothDevice.CreateRfcommSocketToServiceRecord(
                        UUID.FromString(bluetoothDevice.GetUuids().ElementAt(0).ToString()));
                    if (socket.IsConnected)
                        socket.Close();

                    socket.Connect();
                    Byte[] woosimInitialize = new Byte[3] { 27, 64, 10 }; //Printer reset + LF
                    PrintChar(woosimInitialize);
                    PrintChar(FontNormal); //set to standard 48 columns font

                    oStream = new PrintWriter(socket.OutputStream, true);

                    response.Succes = true;
                    response.Message = string.Format("Sambungan Bluetooth Ke {0}", bluetoothDevice.Name);

                    isConnected = true;
                }

                catch (Exception ex)
                {
                    message = ex.Message;
                    retries++;
                    Thread.Sleep(100);
                }

            } while (isConnected == false && retries < Constants.MaxPrintRetry);

            if (isConnected == false)
            {
                response.Succes = false;
                response.Message = "Tidak Dapat Disambungkan Ke Peranti, Sila Semak Peranti Anda. ";
                if (message.Length > 0)
                    response.Message += message;
                return response;

            }

            return response;

        }

        public void PrintText(string text)
        {
#if DEV
            Log.WritePrintIntoFile(text);
            return;
#endif


            //int li_out = 0;
            int li_DataCount = 0;

            //Skip printing after error detected, avoid program keep waiting to print & looked like hanged.
            //if (mb_IsPrintError == true)
            //    return;

            if (text.Length > 0)
                li_DataCount = text.Length;

            if (li_DataCount > 0)
            {
                try
                {
                    //oStream.Write(PrintAddedConstant + text);
                    oStream.Write(text);
                    oStream.Flush();

                    //m_MyAsciiCE.Text(as_Output);
                    //ml_BytesSent += mi_DataCount;
                    //frmPrint.StartPrinting(mi_DataCount);
                }
                catch (Exception exc)
                {
                    //PrintException(exc);
                }
            }
        }

        public void PrintTextNoAdded(string text)
        {
            //int li_out = 0;
            int li_DataCount = 0;

            //Skip printing after error detected, avoid program keep waiting to print & looked like hanged.
            //if (mb_IsPrintError == true)
            //    return;

            if (text.Length > 0)
                li_DataCount = text.Length;

            if (li_DataCount > 0)
            {
                try
                {
                    oStream.Write(text);
                    oStream.Flush();

                    //m_MyAsciiCE.Text(as_Output);
                    //ml_BytesSent += mi_DataCount;
                    //frmPrint.StartPrinting(mi_DataCount);
                }
                catch (Exception exc)
                {
                    //PrintException(exc);
                }
            }
        }

        public void PrintChar(byte[] buffer)
        {
            //set default delay to 200 milliseconds
            PrintChar(buffer, 200);
        }

        public void PrintChar(byte[] buffer, int Delay)
        {
#if DEV
            return;
#endif
            //int li_out = 0;
            int li_DataCount = 0;

            //Skip printing after error detected, avoid program keep waiting to print & looked like hanged.
            //if (mb_IsPrintError == true)
            //    return 0;

            if (buffer != null)
                li_DataCount = buffer.Length;

            if (li_DataCount > 0)
            {
                try
                {
                    int i = 0, Len = 0;
                    int size = li_DataCount;
                    //loop for LF & print
                    for (int j = i; j < li_DataCount; j++)
                    {
                        if (buffer[j] == 0x0A)
                        {
                            Len = (j - i) + 1;
                            socket.OutputStream.Write(buffer, i, Len);
                            size -= Len;
                            i = j + 1;
                            Thread.Sleep(Delay);
                        }
                    }
                    //Print the balance buffer if no LF found
                    if (size > 0)
                    {
                        socket.OutputStream.Write(buffer, i, size);
                        size = 0;
                    }

                    //print all buffer in one command
                    //socket.OutputStream.Write(buffer, 0, buffer.Length);

                    //print byte by byte
                    //for (int i = 0; i < buffer.Length; i++)
                    //{
                    //    //oStream.Write(buffer[i]);
                    //    socket.OutputStream.Write(buffer, i, 1);
                    //}
                }
                catch (Exception exc)
                {
                    //PrintException(exc);
                }
            }
        }

        public void PrintClose()
        {
#if DEV
            return;
#endif
            oStream.Close();
            oStream.Dispose();

            if (socket.IsConnected)
            {
                socket.Close();
            }
        }

        private string CenteringText(string as_Title, int countPerLine)
        {
            string sResult = "";
            int textLen = as_Title.Length;
            int iResult;
            int leftSpacing;

            if (textLen < countPerLine)
            {
                iResult = countPerLine - textLen;
                leftSpacing = iResult / 2;
            }
            else
                leftSpacing = 0;

            sResult = as_Title.PadLeft(textLen + leftSpacing);

            return sResult;


        }

        private void PrintTitle(string as_Title)
        {
            int li_len = 48; //PRINTER EXTECH
            string ls_Result = "";
            PrintChar(ExtechFont1);
            //PrintChar(Extech_Contrast);//contrast

            ls_Result = CenteringText(as_Title, li_len);

            PrintChar(FontDoubleHigh);
            PrintText(ls_Result + "\n");
            PrintChar(FontNormal);

        }

        public PrintResultDto Pemeriksaan(string noRujukan)
        {
            var result = new PrintResultDto();
            
         
            var data = PemeriksaanBll.GetPemeriksaanByRujukan(noRujukan);
            if (data == null)
            {
                result.Message = "Data Tidak Dijumpai";
                result.IsSuccess = false;
            }
            else
            {
                PrintTitle("PEMERIKSAAN");
                PrintText("\n");

                string label = "No Rujukan :";
                string printData = data.NoRujukanKpp;

                PrintText("Tarikh Mula :".PadRight(50) + data.TrkhMulaLawatankpp.PadRight(50) + "\n");
                PrintText("Tarikh Tamat :".PadRight(50) + data.TrkhTamatLawatanKpp.PadRight(50) + "\n");
                PrintText(label.PadRight(50) + printData.PadRight(50) + "\n");

                var katKawasan = MasterDataBll.GetKategoriKawasan(data.KodKatKawasan);
                PrintText("Kategori Kawasan :".PadRight(50) + katKawasan.PadRight(50) + "\n");

                PrintText("Lokasi :".PadRight(50) + data.LokasiLawatan.PadRight(50) + "\n");

                var tujuanLawatan = MasterDataBll.GetTujuanLawatan(data.KodTujuan);
                PrintText("Tujuan Lawatan :".PadRight(50) + tujuanLawatan.PadRight(50) + "\n");

                var asasTindakan = MasterDataBll.GetAsasTindakan(data.KodTujuan, data.KodAsas);
                PrintText("Asas Tindakan :".PadRight(50) + asasTindakan.PadRight(50) + "\n");

                PrintText("No Aduan :".PadRight(50) + data.NoAduan.PadRight(50) + "\n");
                PrintText("No Rujukan Atr :".PadRight(50) + data.NoRujukanAtr.PadRight(50) + "\n");

                PrintText("Catatan Lawatan :".PadRight(50) + data.CatatanLawatan.PadRight(50) + "\n");
                PrintText("Hasil Lawatan :".PadRight(50) + data.HasilLawatan.PadRight(50) + "\n");

                PrintText("\n\n\n\n");
            }

            return result;
        }
    }
}