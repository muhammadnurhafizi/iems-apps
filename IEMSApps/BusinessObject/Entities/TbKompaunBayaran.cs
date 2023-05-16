
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkompaun_bayaran")]
    public class TbKompaunBayaran
    {
        [PrimaryKey]
        public string nokmp { get; set; }

        public string kodcawangan { get; set; }
        public string amnbyr { get; set; }
        public string pusat_terimaan { get; set; }
        
    }
}