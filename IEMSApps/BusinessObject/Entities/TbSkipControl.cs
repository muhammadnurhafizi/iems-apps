using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{

    [Table("tbskipcontrol")]
    public class TbSkipControl
    {
        [PrimaryKey]
        public int IsSkip { get; set; }
    }
}