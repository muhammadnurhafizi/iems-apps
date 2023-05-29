using SQLite.Net.Attributes;
using System;

namespace IEMSApps.BusinessObject.Responses
{
    public class CheckIPResitsResponse
    {
        public string norujukankpp { get; set; }

        public string diterima_drpd { get; set; }

        public string no_resit { get; set; }

        public string byrn_bg_pihak { get; set; }

        public DateTime tarikh_bayaran { get; set; }

        public string no_identiti { get; set; }

        public string mod_pembayaran { get; set; }

        public string alamat_1 { get; set; }

        public string alamat_2 { get; set; }

        public string alamat_3 { get; set; }

        public string poskod { get; set; }

        public string bandar { get; set; }

        public string negeri { get; set; }

        public string rangkaian { get; set; }

        public string emel { get; set; }

        public string no_transaksi_ipayment { get; set; }

        public string no_rujukan_ipayment { get; set; }

        public string no_transaksi_rma { get; set; }

        public string perihal { get; set; }

        public string no_rujukan { get; set; }

        public string kod_akaun { get; set; }

        public string jumlah { get; set; }

        public string jumlah_bayaran { get; set; }
    }
}