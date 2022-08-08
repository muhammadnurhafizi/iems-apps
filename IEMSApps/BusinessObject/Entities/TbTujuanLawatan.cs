using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbtujuanlawatan")]
    public class TbTujuanLawatan : MasterBaseEntities
    {
        [PrimaryKey, AutoIncrement]
        public int KodTujuan { get; set; }

        public string Prgn { get; set; }
    }

    [Table("tbtujuanlawatanTemp")]
    public class TbTujuanLawatanTemp : TbTujuanLawatan
    {
        public int Status { get; set; }
    }
}