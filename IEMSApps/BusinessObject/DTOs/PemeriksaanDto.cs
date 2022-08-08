
namespace IEMSApps.BusinessObject.DTOs
{
    public class PemeriksaanDto
    {
        public string NoRujukanKpp { get; set; }
        public string KodCawangan { get; set; }
        public int KodTujuan { get; set; }
        public int KodAsas { get; set; }
        public string CatatanLawatan { get; set; }
        public string KodKatKawasan { get; set; }
        public string NoRujukanAtr { get; set; }
        public string NoSiriBorangKpp { get; set; }
        public string TrkhMulaLawatankpp { get; set; }
        public string TrkhTamatLawatanKpp { get; set; }
        public int KodKatPremis { get; set; }
        public string NamaPremis { get; set; }
        public string AlamatPremis1 { get; set; }
        public string AlamatPremis2 { get; set; }
        public string AlamatPremis3 { get; set; }
        public string NoDaftarPremis { get; set; }
        public string NoLesenBKP_PDA { get; set; }
        public string NoLesenMajlis_Permit { get; set; }
        public string NoTelefonPremis { get; set; }
        public string CatatanPremis { get; set; }
        public string LongitudPremis { get; set; }
        public string LatitudPremis { get; set; }
        public int Amaran { get; set; } //1 yes, 2=no
        public string LokasiLawatan1 { get; set; }
        public string LokasiLawatan2 { get; set; }
        public string LokasiLawatan3 { get; set; }
        public string NoAduan { get; set; }
        public string HasilLawatan { get; set; }
        public string GambarLawatan1 { get; set; }
        public string GambarLawatan2 { get; set; }
        public int Tindakan { get; set; } //'*Tindakan*\n1 = KOTS\n2 = Siasatan Lanjut\n3 = Tiada Kes',
        public string NamaPenerima { get; set; }
        public string NoKpPenerima { get; set; }
        public string Jawatanpenerima { get; set; }
        public string AlamatPenerima1 { get; set; }
        public string AlamatPenerima2 { get; set; }
        public string AlamatPenerima3 { get; set; }
        public string TrkhPenerima { get; set; }
        public int SetujuByr { get; set; } //'*Setuju Bayar*\n1 = X\n2 = \n',
    }
}