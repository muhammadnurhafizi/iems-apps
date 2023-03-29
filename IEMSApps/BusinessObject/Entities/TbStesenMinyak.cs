using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbjenama_stesen_minyak")]
    public class TbStesenMinyak : BaseEntities
    {
        public int Id { get; set; }
        public string Prgn { get; set; }

    }
}