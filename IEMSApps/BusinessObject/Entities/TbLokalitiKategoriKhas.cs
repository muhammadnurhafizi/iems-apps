﻿
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tblokaliti_kategori_khas")]
    public class TbLokalitiKategoriKhas : MasterBaseEntities
    {
        [PrimaryKey]
        public int id { get; set; }

        public string prgn { get; set; }

    }
}