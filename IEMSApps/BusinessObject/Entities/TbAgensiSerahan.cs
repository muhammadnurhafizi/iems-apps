﻿using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbagensiserahan")]
    public class TbAgensiSerahan : BaseEntities
    {
        [PrimaryKey]
        [MaxLength(10)]
        public string kodserahagensi { get; set; }

        [MaxLength(500)]
        public string prgn { get; set; }

    }

}