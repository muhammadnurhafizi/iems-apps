using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{

    [Table("tbkpp_asastindakan")]
    public class TbKppAsasTindakan
    {
        [Indexed(Name = "IxATbKppsasTindakanPk", Order = 1, Unique = true)]
        [MaxLength(20)]
        public string NoRujukanKpp { get; set; }

        [Indexed(Name = "IxATbKppsasTindakanPk", Order = 2, Unique = true)]
        public int KodTujuan { get; set; }

        [Indexed(Name = "IxATbKppsasTindakanPk", Order = 3, Unique = true)]
        public int KodAsas { get; set; }

        public Enums.StatusOnline IsSendOnline { get; set; }

        public int PgnDaftar { get; set; }

        public string TrkhDaftar { get; set; }

        public int PgnAkhir { get; set; }

        public string TrkhAkhir { get; set; }
    }
}