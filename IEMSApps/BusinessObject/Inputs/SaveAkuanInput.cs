namespace IEMSApps.BusinessObject.Inputs
{
    public class SaveAkuanInput
    {
        public string NoRujukan { get; set; }

        public string NamaPenerima { get; set; }

        public string jeniskad { get; set; }

        public string poskodpenerima { get; set; }
        
        public string bandarpenerima { get; set; }

        public string negeripenerima { get; set; }

        public string negarapenerima { get; set; }

        public string notelpenerima { get; set; }

        public string emelpenerima { get; set; }

        public string NoKpPenerima { get; set; }

        public string AlamatPenerima1 { get; set; }

        public string AlamatPenerima2 { get; set; }

        public string AlamatPenerima3 { get; set; }

        public string NoResit { get; set; }

        public decimal AmountByr { get; set; }

        public string TrkhPenerima { get; set; }

        public int isbayarmanual { get; set; }
    }
}