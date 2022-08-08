using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbdatakes_asastindakan")]
    public class TbDataKesAsasTindakan
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(20)]
        public string NoKes { get; set; }
       
        public int KodTujuan { get; set; }
       
        public int KodAsas { get; set; }

        public Enums.StatusOnline IsSendOnline { get; set; }

        public int PgnDaftar { get; set; }

        public string TrkhDaftar { get; set; }
    }
}