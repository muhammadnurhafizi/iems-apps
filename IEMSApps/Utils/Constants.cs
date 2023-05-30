using Android;
using Android.Locations;
using System;
using System.Runtime.Remoting.Messaging;

namespace IEMSApps.Utils
{
    public static class Constants
    {

        public const string AppVersion = "Version: 1.1.3.9";
        public const int AppVersionValueForUpdate = 1139;

        public const string AppName = "IEMS";
       
        public static readonly string Password = "smiEms2020!";
        //public static readonly string Password = "aimForce2019!";
        public static readonly string LicenseKey = "OmNpZDpoc3lpcEBhaW0tZm9yY2UuY29tLm15OnBsYXRmb3JtOjg6ZXhwaXJlOm5ldmVyOnZlcnNpb246MTpobWFjOmEwYzAxYjJlM2Q4M2I3YzVkZWEyYTVlMzk0YTM3ZjdlNDQzMmIyMzA=";

        public const string ProgramPath = "/iemsapp/";
        public const string ImgsPath = "IMGS/";
        public const string DatabasePath = "DATABASE/";
        public const string LogPath = "LOGS/";
        public const string BackupPath = "BACKUP/";
        public const string ConfigPath = "CONFIG/";
        public const string ConfigName = "ConfigApp.xml";
        public const string DatabaseName = "IEMS.db";
        public const string ErrorRecordsPath = "Errorrecord.txt";
        public const string BackupName = "Backup.sql";
        public const string LASTTRANS = "LASTTRANS.TXT";

        public const string APKFolder = "APK/";

        public const string DatabaseDateFormat = "yyyy-MM-dd HH:mm:ss";
        public const string DatabaseDateFormatWithoutSecond = "yyyy-MM-dd HH:mm";
        public const string DateFormatDisplay = "dd/MM/yyyy";
        //public const string TimeFormatDisplay = "hh:mm:ss tt";
        public const string TimeFormatDisplay = "HH:mm:ss";
        public const string TimeFormatDatabase = "HH:mm:ss";
        public const string DecimalFormat = "F2";
        public const string DateBackupFormat = "yyyyMMddHHmmss";
        public const string DecimalFormatZero = "F0";

        public const int MaxPhoto = 5;
        public const int MinPhoto = 2;
        public const int MinPhotoTiadaKes = 1;

        public const int MaxPrintRetry = 3;
        public const int MaxPrint = 2;

        public const string FinishPage = "1";

        public const int Success = 1;
        public const int Error = -1;

        public const string SeparateCharList = " ; ";

        public const string ImageExtension = ".JPG";
        public const string FormatRunningNumber = "00000";
        public const int MaxLengthAddress = 80;
        public const int MaxLengthSentData = 10;

        public static readonly string[] Permissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.Camera,
            Manifest.Permission.AccessNetworkState,
            Manifest.Permission.AccessWifiState,
        };

        public static class ErrorMessages
        {
            public const string InvalidLogin = "Id Pengguna/Kata Laluan tidak sah";
            public const string FailedCreateKompaunIzin = "Gagal membuat kompaun izin";
            public const string NoInternetConnection = "Tiada sambungan internet";
            public const string NeedCheckIn = "Sila MULA TUGAS terlebih dahulu";
            public const string NoPhotoExist = "Tiada gambar";
            public const string PrinterNotFound = "Pencetak tidak ditemui";
            public const string NotFound = "Tidak ada";
            public const string DateEndEqualLessDateStart = "Tarikh dan Masa Tamat perlu lebih lebih besar dari Tarikh dan Masa Mula.";
            public const string PasukanExist = "ID Pengguna yang ingin ditambah telah wujud dalam pasukan ini";
            public const string KompaunNotCompleted = "Maklumat Kompaun masih belum lengkap. Selesai pemeriksaan tidak dibenarkan";
            public const string SiasatLanjutNotCompleted = "Maklumat Siasatan Lanjut masih belum lengkap. Selesai pemeriksaan tidak dibenarkan";
            public const string ErrorApi = "Error Code : {0}. Ralat mendapatkan data dari Sistem Utama IEMS.Sila hubungi admin sistem";
            public const string ErrorApi_Exception = "Error : {0}. Ralat mendapatkan data dari Sistem Utama IEMS.Sila hubungi admin sistem";
            public const string ErrorApiTimeout = "Gagal berhubung ke server IEMS. Sila cuba lagi atau hubungi admin sistem";
            public const string SkipNoInternetConnection = "Tiada sambungan internet, Skip Permohonan Izin KOTS";
            public const string FailedSaveData = "Gagal simpan data";
            public const string NoDataFound = "Tidak ada data";
            public const string NoDataFoundJpnDetail = "Maklumat NOKP : {0}  tidak ditemui";
            public const string ErrorSendRellatedData = "Ralat hantar data ke Sistem Utama IEMS bagi {0}";
        }

        public static class JenisApps
        {
            public const string Manual = "1";
            public const string Apps = "2";
        }

        public static class Status
        {
            public const string Aktif = "1";
            public const string TidakAktif = "2";
        }

        public static class JenisPengguna
        {
            public const string Ketua = "1";
            public const string Ahli = "2";
        }

        public static class Amaran
        {
            public const int Yes = 1;
            public const int No = 0;
        }

        public static class SetujuBayar
        {
            public const int Yes = 1;
            public const int No = 0;
        }

        public static class Tindakan
        {
            public const int TiadaKes = 0;
            public const int Kots = 1;
            public const int SiasatLanjutan = 2;
            public const int SiasatUlangan = 3;
            //public const int SerahanNotis = 4;
        }

        public static class JenisTrans
        {
            public const int Atr = 1;
            public const int Kpp = 2;
            public const int Kots = 3;
            public const int Kmp = 4;
        }

        public static class JenisPesalah
        {
            public const string Individu = "0";
            public const string Syarikat = "1";
        }

        public static class CetakAkuan
        {
            public const string Yes = "1";
            public const string No = "0";
        }

        public static class ArahanSemasa
        {
            public const string Yes = "1";
            public const string No = "0";
        }


        public static class GpsStatus
        {
            public const string GpsOn = "GpsOn";
            public const string GpsOff = "GpsOff";
        }

        public static class Kewarganegraan 
        {
            public const string Warganegara = "Warganegara";
            public const string BukanWarganegara = "Bukan Warganegara";
        
        }

        public static class GambarBayaran 
        {

            public const int Yes = 1;
            public const int No = 0;   
        
        }

        public static class NPMB
        {
            public const int Yes = 1;
            public const int No = 0;   
        
        }

        public static class NB
        {
            public const int Yes = 1;
            public const int No = 0;

        }

        public static class Messages
        {
            public const string SavingData = "Sedang proses simpan data... Sila tunggu";
            public const string SuccessSave = "Data berjaya disimpan";
            public const string ReplaceImageQuestion = "Ganti gambar?";
            public const string SuccessSavePasukan = "Ahli pasukan baru berjaya dimasukkan";
            public const string KompaunIzinApproved = "<b>Keputusan izin Kompaun</b> : DIBENARKAN.<br/><br/><b>Catatan TPR</b> : {0}";
            public const string KompaunIzinDenied = "<b>Keputusan izin Kompaun</b> : TIDAK DIBENARKAN.<br/><br/><b>Catatan TPR</b> : {0}";

            public const string KompaunIzinWaiting =
                    "Permohonan izin kompaun telah dihantar. Sila tunggu dan klik semula butang TINDAKAN untuk mendapatkan keputusan";

            public const string InsertData = "Sedang muat turun";
            public const string SendDataOnline = "Hantar Data";
            public const string DialogRePrint = "Cetak Semula ?";
            public const string SendData = "Hantar Data";
            public const string ReSendData = "Gagal hantar data. Hantar semula?";
            public const string Yes = "Ya";
            public const string No = "Tidak";

            public const string HaveNewVersion = "Terdapat versi baru Aplikasi IEMS";

            public const string BackNotSave = "Data belum disimpan , kembali ?";
            public const string WaitingPlease = "Sila tunggu...";
            
            public const string Downloading = " Muat turun...";

            public const string SuccessPrint = " Cetakan berjaya";
            public const string TurnOnGpsMessage = "Sila ON tetapan GPS";
            public const string GPSSetting = "Tetapan GPS";
            public const string PrintWaitMessage = "Sedang cetak... Sila tunggu";
            public const string SelectYourItem = "Sila buat pilihan";
            public const string SuccessSendData = "Data berjaya dihantar";
            public const string FinishLawatan = "Anda pasti untuk tamatkan lawatan ini?";

            public const string SkipMessage = "Tiada Keputusan Izin, Masa Menunggu Telah Lebih {0} Minit. Skip Permohonan Izin KOTS";
            public const string HapusData = "Hapus";
            public const string Move = "Pindah";
            public const string ConnectionToBluetooth = "Proses capaian sambungan";
            public const string GenerateBitmap = "Cetakan..";
            public const string FaildSendData = "Gagal hantar data.";

            public const string CheckResit = "Menyemak Data Resit Di Sistem IEMS";
            public const string BayarBerjaya = "<div style='text-align:center' > <br> <b> Pembayaran Telah Berjaya Dibuat </b> </div>";
            public const string NoReceiptOnServer = "Tiada Data Resit di Sistem IEMS, Sila Cuba Sebentar lagi"; 
            public const string NoReceipt = "Tiada Data Resit di Sistem IEMS";
            public const string haveReceipt = "Resit Dijumpai";
            public const string FoundReceipt = "<div style='text-align:center' > <br> <b> Resit Telah Dijana </b> </div>";
            public const string SambungAkuan = "<div style='text-align:center' ><b> Tiada Data Akuan </b> <br>" +
                                                  "<br> Hasilkan semula Akuan ? </div>";
            public const string SambungKompaun = "<div style='text-align:center' ><b> Tiada Data Kompaun </b> <br>" +
                                                  "<br> Hasilkan semula Kompaun ? </div>";
        }

        public static class IpaymentMessages
        {
            public const string NoReceiptFoundInServer = "<div style='text-align:center' > <br> <b>Tiada Resit Dijumpai di Sistem IEMS </b>" +
                                                         "<br> Pastikan OKK membuat bayaran di Sistem iPayment dan sila cuba sekali lagi </div>";
            public const string ErrorApiReceipt = "Error : <b> {0} </b>. <br> Ralat mendapatkan data dari Sistem Utama IEMS.Sila hubungi admin sistem";

            public const string RalatFetchData = "Ralat Mengambil Data";
        }

        public const string Close = "Tutup";
        public const string ViewResit = "Lihat Resit";

        public const string InstallText = "Install";
        public const string SkipText = "Skip";

        public const int MaxCallAPIRetry = 3;
        public const int SleepRetryActiveParking = 2000;//in second = 2 detik

        public const string PIRANTIID = "PIRANTI_ID";
        public const string LOADING_TYPE = "LOADING_TYPE";
        public const string LOADING_PREPARE_DOWNLOAD_DATA = "LOADING_PREPARE_DOWNLOAD_DATA";
        public const string LOADING_DOWNLOAD = "LOADING_DOWNLOAD";
        public const string URL_NEW_APK = "URL_NEW_APK";
        public const string NEW_APK_NAME = "IEMSApps.IEMSApps.apk";

        public const int MaxKompaunIzinRetry = 5;
        public const int SleepRetryKompaunIzin = 2000;//in second = 2 detik

        public const string ApiUrl = "API_IEMS/api/";

        public static class ApiUrlAction
        {
            public const string GetRecord = ApiUrl + "getrecord/";
            public const string ExecQuery = ApiUrl + "executestatement/";
            public const string ExecQueryPost = ApiUrl + "executestatementpost?";
            public const string PrepareDownloadData = ApiUrl + "preparedownloaddata3/";
            public const string UploadImage = ApiUrl + "uploadimage";
            public const string GetTableSummary = ApiUrl + "gettablesummary/";
            public const string PrepareDownloadDataSelected = ApiUrl + "preparedownloaddataselected/";
            public const string GetListSsm = ApiUrl + "getssmdetailbynossm/";
            public const string GetJpnDetailByNoIc = ApiUrl + "getjpndetailbynoiclog/";
            public const string GetJpnDetailByNoIcWithErr = ApiUrl + "getjpndetailbynoiclogwitherr/";
            public const string GetKawasan = ApiUrl + "getKawasan/";
            public const string Ipayment = ApiUrl + "ipayment/";
        }

        public const int DefaultStafId = 1;

        public const string FormatSeksyen = "<SEKSYEN KESALAHAN>";
        public const string ReplaceAmoun = "<AMNKMP>";
        public const string ReplaceAmountWord = "<WORDINGAMN>";
        public const string ReplaceTempohHari = "<TEMPOH HARI>";
        public const int DefaultLengthSeparate45 = 45;
        public const int DefaultLengthSeparate50 = 50;
        public const int DefaultLengthSeparate55 = 55;
        public const int DefaultLengthSeparate60 = 60;
        public const int DefaultLengthSeparate70 = 70;
        public const int DefaultLengthSeparate = 75;
        public const int DefaultLengthSeparateTitle = 40;

        public const int DefaultIntervalInSecond = 30;
        public const int DefaultDistanceInMeter = 100;
        //public const string DefaultWebServiceUrl = "http://1.9.46.170:98/";
        //public const string DefaultWebServiceUrl = "http://iemsstag.kpdn.gov.my/";
        public const string DefaultWebServiceUrl = "http://mhdamn.me/";
        public const int DefaultIntervalBackgroundServiceInSecond = 300;

        public static class PriorityUpdate
        {
            public const string Required = "1";
            public const string NotRequired = "2";
        }

        public const string DefaultDisplayAllPremis = "HQR";

        public const int DefaultTujuanLawatan = 1;

        public const int DefaultCountImageData = 68;

        public const string AdminUserValue = "Admin";
        public const string AdminPasswordValue = "!123@";
        public const string AdminNoKpValue = "123456789012";

        public static class PrinterMessage
        {
            public const int MESSAGE_READ = 3;
        }

        public static class TindakanName
        {
            public const string Pemeriksaan = "Pemeriksaan";
            public const string KOTS = "KOTS";
            public const string SiasatLanjut = "Kes Baru dihasilkan (untuk siasatan)";
            public const string SiasatUlangan = "Siasatan Ulangan";
            //public const string SerahanNotis = "Serahan Notis";
        }

        //public static class JenisKad 
        //{
        //    public const string MyKad = "MyKAD";
        //    public const string MyKas = "MyKAS";
        //    public const string MyPR = "MyPR";
        //    public const string Passport = "Passport";
        //    public const string Others = "Lain-lain";
        
        //}

        public const string FWCODE = "1202T1";

        public static class SkipIzin
        {
            public const int Yes = 1;
            public const int No = 0;
        }

        public const int MaxSkipWaitingInMinute = 2;

        public const int MaxLineSeparate20 = 20;

        //public const string AllowedChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,\"\'.=/-_!@#&()?:;<>{}[]";
        public const string AllowedChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890#$&\\_-?!@()=+':%/\" *,.<>{}[];";
        public const string AllowedCharNoIPAndNoEP = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-";
        public const string AllowedCharWithoutSingleQuote = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890#$&\\_-?!@()=+:%/\" *,.<>{}[];";

        public const int AllowAddressCharacter = 35;

        public const string Receipt = "IPRESIT";

        public const int KompaunKots = 1;
        public const int DefaultWaitingMilisecond = 100;
        public const int DefaultWaitingConnectionToBluetooth = 3000;

        public const string TujuanLawatanPermeriksaanBiasa = "pemeriksaan biasa";

        public const int Brightness = 35;

        public static class FtpAccount
        {
            public const string Url = "ftp://192.168.0.2:2121/iemsapp/LOGS/";
            public const string UserName = "aimforce";
            public const string Password = "1234";
        }
    }
}