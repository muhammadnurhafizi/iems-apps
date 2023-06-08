
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tblokaliti_kategori_khas_temp")]
    public class TbLokalitiKategoriKhasTemp : BaseEntities
    {
        [PrimaryKey]
        public int id { get; set; }

        public string prgn { get; set; }


    }
}