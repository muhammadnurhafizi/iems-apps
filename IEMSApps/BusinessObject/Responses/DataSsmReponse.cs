using System.Collections.Generic;

namespace IEMSApps.BusinessObject.Responses
{
    public class DataSsmReponse
    {
       
        public List<DataSsmDetails> DataDetails { get; set; } = new List<DataSsmDetails>();
    }

    public class DataSsmDetails
    {
        public string NoSyarikat { get; set; }

        public string NamaSyarikat { get; set; }
        public string AlamatDaftar1 { get; set; }
        public string AlamatDaftar2 { get; set; }
        public string AlamatDaftar3 { get; set; }
        public string AlamatNiaga1 { get; set; }
        public string AlamatNiaga2 { get; set; }
        public string AlamatNiaga3 { get; set; }

        public string Jenis { get; set; }
        public string penerangan_status_syrkt { get; set; }
        public string no_tel_syrkt { get; set; }
    }
}