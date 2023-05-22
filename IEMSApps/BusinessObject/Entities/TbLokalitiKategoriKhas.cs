
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tblokaliti_kategori_khas")]
    public class TbLokalitiKategoriKhas : MasterBaseEntities
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Prgn { get; set; }


    }
}