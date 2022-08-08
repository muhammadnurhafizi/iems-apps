
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbakta")]
    public class TbAkta : MasterBaseEntities
    {
        [PrimaryKey]
        [MaxLength(10)]
        public string KodAkta { get; set; }

        [MaxLength(10)]
        public string KodAktaKump { get; set; }
        [MaxLength(250)]
        public string Prgn { get; set; }
        [MaxLength(250)]
        public string PerintahPeraturan { get; set; }
        [MaxLength(150)]
        public string Tajuk1 { get; set; }
        [MaxLength(150)]
        public string Tajuk2 { get; set; }
        [MaxLength(150)]
        public string Tajuk3 { get; set; }
        [MaxLength(150)]
        public string Tajuk4 { get; set; }
        [MaxLength(150)]
        public string Tajuk5 { get; set; }
        [MaxLength(100)]
        public string Daripada1 { get; set; }
        [MaxLength(100)]
        public string Daripada2 { get; set; }
        [MaxLength(500)]
        public string Perenggan1 { get; set; }
        [MaxLength(500)]
        public string Perenggan2 { get; set; }
        [MaxLength(500)]
        public string Perenggan3 { get; set; }
        [MaxLength(500)]
        public string Perenggan4 { get; set; }
        [MaxLength(100)]
        public string Pengeluar1 { get; set; }
        [MaxLength(100)]
        public string Pengeluar2 { get; set; }
    }
}