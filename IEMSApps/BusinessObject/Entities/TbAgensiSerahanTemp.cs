using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbagensiserahantemp")]
    public class TbAgensiSerahanTemp : BaseEntities
    {
        [PrimaryKey]
        [MaxLength(10)]
        public string kodserahagensi { get; set; }

        [MaxLength(500)]
        public string prgn { get; set; }

    }

}