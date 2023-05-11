
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkpp_agensi_serahan")]
    public class TbKppAgensiSerahan : MasterBaseEntities
    {
        [PrimaryKey]
        public int id { get; set; } 

        public int norujukankpp { get; set; }

        public string kodserahagensi { get; set; }


    }
}