using SQLite.Net.Attributes;
using System;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("ip_resits")]
    public class ip_resits
    {
        [PrimaryKey]
        public string norujukankpp { get; set; }

        public string diterima_drpd { get; set; }

        [MaxLength(20)]
        public string no_resit { get; set; }

        public string byrn_bg_pihak { get; set; }

        public DateTime tarikh_bayaran { get; set; }

        [MaxLength(20)]
        public string no_identiti { get; set; }

        [MaxLength(2)]
        public string mod_pembayaran { get; set; }

        [MaxLength(35)]
        public string alamat_1 { get; set; }

        [MaxLength(35)]
        public string alamat_2 { get; set; }

        [MaxLength(35)]
        public string alamat_3 { get;set; }

        [MaxLength(5)]
        public string poskod { get; set; }

        [MaxLength(35)]
        public string bandar { get; set; }

        [MaxLength(35)]
        public string negeri { get; set; }

        [MaxLength(100)]
        public string emel { get;set; }

        [MaxLength(20)]
        public string no_transaksi_ipayment { get; set; }

        [MaxLength(36)]
        public string no_rujukan_ipayment { get;set; }

        [MaxLength(50)]
        public string no_transaksi_rma { get; set; }

        [MaxLength(255)]
        public string perihal { get; set;}

        public string no_rujukan { get; set; }

        [MaxLength(8)]
        public string kod_akaun { get; set; }

        [MaxLength(13)]
        public string jumlah { get; set; }

        public string jumlah_bayaran { get; set; }

    }
}