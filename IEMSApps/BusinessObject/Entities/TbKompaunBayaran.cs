
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkompaun_bayaran")]
    public class TbKompaunBayaran
    {
        public string kodcawangan { get; set; }
        public string nokmp { get; set; }
        public string amnbyr { get; set; }
        public string pusat_terimaan { get; set; }
        
    }
}