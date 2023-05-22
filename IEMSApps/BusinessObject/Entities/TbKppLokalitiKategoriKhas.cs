
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkpp_lokaliti_kategori_khas")]
    public class TbKppLokalitiKategoriKhas : MasterBaseEntities
    {
        [PrimaryKey]
        public int id { get; set; } 

        public int norujukankpp { get; set; }

        public string kodlokalitikategorikhas { get; set; }


    }
}