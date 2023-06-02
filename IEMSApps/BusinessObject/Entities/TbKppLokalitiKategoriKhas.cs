
using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkpp_lokaliti_kategori_khas")]
    public class TbKppLokalitiKategoriKhas : BaseEntities
    {
        [PrimaryKey]
        [AutoIncrement]
        public int id { get; set; } 

        public string norujukankpp { get; set; }

        public int tblokaliti_kategori_khas_id { get; set; }

        public Enums.StatusOnline IsSendOnline { get; set; }

    }
}