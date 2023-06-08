
using Android.Locations;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbjenama_stesen_minyak_temp")]
    public class TbJenamaStesenMinyakTemp : BaseEntities
    {
        [PrimaryKey]
        public int kodjenama { get; set; }

        public string prgn{ get; set; }

    }
}