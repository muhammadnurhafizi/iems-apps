
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkpp_agensi_terlibat")]
    public class TbKppAgensiTerlibat : BaseEntities
    {
        [PrimaryKey]
        public int id { get; set; } 

        public string norujukankpp { get; set; }

        public string kodserahagensi { get; set; }


    }
}