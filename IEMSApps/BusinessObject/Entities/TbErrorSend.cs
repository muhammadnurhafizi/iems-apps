using SQLite.Net.Attributes;
using System;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("TbError_Send")]
    public class TbErrorSend
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(6)]
        public string IdHh { get; set; }

        [MaxLength(20)]
        public string NoRujukan { get; set; }

        [MaxLength(20)]
        public string Type { get; set; }

        public string SqlStmt { get; set; }
    }
}