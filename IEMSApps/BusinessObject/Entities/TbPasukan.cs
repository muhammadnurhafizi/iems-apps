using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbpasukan_header")]
    public class TbPasukanHeader
    {
        [PrimaryKey, AutoIncrement]
        public int KodPasukan { get; set; }

        public string TrkhDaftarPasukan { get; set; }

        public string JenisApps { get; set; }

        public string IdBahagian { get; set; }

        public string Status { get; set; }

        public string PgnDaftar { get; set; }

        public string TrkhDaftar { get; set; }
        public DateTime DateTrkhDaftar =>
            DateTime.ParseExact(TrkhDaftar, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        public string TrkhAkhir { get; set; }

        public DateTime DateTrkhAkhir =>
            DateTime.ParseExact(TrkhAkhir, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
}