
using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbpasukan_trans")]
    public class TbPasukanTrans : BaseEntities
    {
        [Indexed(Name = "IxTbPasukanTransPk", Order = 1, Unique = true)]
        public int JenisTrans { get; set; } //'*Jenis Transaksi*\n1 = ATR\n2 = KPP\n3 = KOTS\n4 = KMP',

        [Indexed(Name = "IxTbPasukanTransPk", Order = 2, Unique = true)]
        [MaxLength(20)]
        public string NoRujukan { get; set; }

        [Indexed(Name = "IxTbPasukanTransPk", Order = 3, Unique = true)]
        public int Id { get; set; }

        public int KodPasukan { get; set; }

        public Enums.StatusOnline IsSendOnline { get; set; }

        [MaxLength(250)]
        public string Catatan { get; set; }
    }
}