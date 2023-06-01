
using Android.Locations;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbjenama_stesen_minyak")]
    public class TbJenamaStesenMinyak : BaseEntities
    {
        [PrimaryKey]
        public int kodjenama { get; set; }

        public string prgn{ get; set; }

    }
}