namespace IEMSApps.Utils
{
    public class Enums
    {
        public enum LogType
        {
            Error = 1,
            Info = 2,
            Debug = 3
        }

        public enum FileLogType
        {
            Apps = 1,
            CompoundService = 2,
            GpsService = 3
        }

        public enum JenisApps
        {
            Manual = 1,
            Apps = 2,
        }

        public enum Status
        {
            Aktif = 1,
            TidakAktif = 2,
        }

        public enum JenisPengguna
        {
            Ketua = 1,
            Ahli = 2,
        }

        public enum LoginType
        {
            Admin = 1,
            User = 2,
        }

        public enum PrefixType
        {
            // All = 0,
            KPP = 1,
            KOTS = 2,
            SiasatLanjutan = 3,
            KOTSAndSiasatLanjutan = 4,
        }

        public enum JenisKompaun
        {
            KOTS = 0,
            KompaunBiasa = 2,
        }

        public enum ActiveForm
        {
            Pemeriksaan = 1,
            Kompaun = 2,
            Akuan = 3,
            Camera = 4,
            SiasatLanjutan = 5,
            SerahanNotis = 6,
        }

        public enum SearchCarianType
        {
            All = 0,
            NoKpp = 1,
            NamaPremis = 2,
            NoAduan = 3,
            NoIcPenerima = 4
        }

        public enum SearchTindakanType
        {
            All = 0,
            TiadaKes = 1,
            Kots = 2,
            SiasatLanjutan = 3,
            SiasatUlangan = 4,
        }

        public enum StatusIzinKompaun
        {
            Waiting = 1,
            Approved = 2,
            Denied = 3,
        }

        public enum TableType
        {
            // All = 0,
            KPP = 1,
            Kompaun = 2,
            DataKes = 3,
            KompaunBayaran = 4,
            KPP_HH = 5,
            Kompaun_HH = 6,
            DataKes_HH = 7,

            Akuan_UpdateKompaun = 8,
            Akuan_UpdateKompaun_HH = 9
        }

        public enum StatusOnline
        {
            New = 0,
            Sent = 1,
            Error = 2
        }

        public enum Tindakan
        {
            TiadaKes = 0,
            Kots = 1,
            SiasatLanjutan = 2,
            SiasatUlangan = 3,
            SerahanNotis = 4
        }
    }
}