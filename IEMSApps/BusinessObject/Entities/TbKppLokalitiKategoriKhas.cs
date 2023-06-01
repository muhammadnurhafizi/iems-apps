
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkpp_lokaliti_kategori_khas")]
    public class TbKppLokalitiKategoriKhas : MasterBaseEntities
    {
        [PrimaryKey]
        public int id { get; set; } 

        public string norujukankpp { get; set; }

        public string tblokaliti_kategori_khas_id { get; set; }


    }
}