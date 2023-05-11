namespace IEMSApps.BusinessObject.DTOs
{
    public class MaklumatChargelineDto
    {
        public int id { get; set; }

        public string kod_penjenisan { get; set; }

        public string pejabat { get; set; }

        public string vot_dana { get; set; }

        public string pegawai_pengawal_dipertanggung { get; set; }

        public string kumpulan_ptj_dan_ptj_dipertanggung { get; set; }

        public string program_aktiviti { get; set; }

        public string projek { get; set; }

        public string kod_akaun { get; set; }
            
        public string kodcawangan { get; set; }

        public string created_at { get; set; }

        public string updated_at { get; set; }

        public string deleted_at { get; set; }
    }
}