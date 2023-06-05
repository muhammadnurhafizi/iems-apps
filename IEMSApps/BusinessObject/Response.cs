using System.Collections.Generic;

namespace IEMSApps.BusinessObject
{
    public class Response<T> : BaseResponse
    {
        public Response()
        {
            Result = default(T);
        }

        public T Result { get; set; }
    }

    public class BaseResponse
    {
        public bool Success { get; set; } = true;
        public string Mesage { get; set; }
    }

    public class DownloadDataResponse
    {
        public List<ValueResponse> Tbpasukan_hh { get; set; }
        public List<ValueResponse> TbnegeriTemp { get; set; }
        public List<ValueResponse> TbcawanganTemp { get; set; }
        public List<ValueResponse> TbpenggunaTemp { get; set; }
        public List<ValueResponse> TbtujuanlawatanTemp { get; set; }
        public List<ValueResponse> TbasastindakanTemp { get; set; }
        public List<ValueResponse> TbkategorikawasanTemp { get; set; }
        public List<ValueResponse> TbkategoripremisTemp { get; set; }
        public List<ValueResponse> TbkategoriperniagaanTemp { get; set; }
        public List<ValueResponse> TbjenisperniagaanTemp { get; set; }
        public List<ValueResponse> TbpremisTemp { get; set; }
        public List<ValueResponse> TbaktaTemp { get; set; }
        public List<ValueResponse> TbkesalahanTemp { get; set; }

        public List<ValueResponse> TbbandarTemp { get; set; }
        public List<ValueResponse> Tbskipcontrol { get; set; }
        public List<ValueResponse> Tbbarang_jenamaTemp { get; set; }
        public List<ValueResponse> TbAgensiSerahan { get; set; }
        public List<ValueResponse> TbJenama_Stesen_Minyak { get; set; }
    }

    //public class DownloadDataFreshResponse
    //{
    //    public List<ValueResponse> Tbnegeri { get; set; }
    //    public List<ValueResponse> TbcawanganTemp { get; set; }
    //    public List<ValueResponse> TbpenggunaTemp { get; set; }
    //    public List<ValueResponse> Tbtujuanlawatan { get; set; }
    //    public List<ValueResponse> TbasastindakanTemp { get; set; }
    //    public List<ValueResponse> Tbkategorikawasan { get; set; }
    //    public List<ValueResponse> Tbkategoripremis { get; set; }
    //    public List<ValueResponse> Tbkategoriperniagaan { get; set; }
    //    public List<ValueResponse> TbjenisperniagaanTemp { get; set; }
    //    public List<ValueResponse> TbpremisTemp { get; set; }
    //    public List<ValueResponse> TbaktaTemp { get; set; }
    //    public List<ValueResponse> TbkesalahanTemp { get; set; }
    //    //public List<ValueResponse> Tbkesalahan_butir { get; set; }
    //    //public List<ValueResponse> Tbkesalahan_aktiviti { get; set; }

    //    public List<ValueResponse> TbbandarTemp { get; set; }
    //    public List<ValueResponse> Tbskipcontrol { get; set; }
    //    public List<ValueResponse> Tbbarang_jenama { get; set; }
    //}

    public class PrepareDownloadDataSelectedResponse
    {
        public List<ValueResponse> Tbnegeri { get; set; }
        public List<ValueResponse> Tbcawangan { get; set; }
        public List<ValueResponse> Tbpengguna { get; set; }
        public List<ValueResponse> Tbtujuanlawatan { get; set; }
        public List<ValueResponse> Tbasastindakan { get; set; }
        public List<ValueResponse> Tbkategorikawasan { get; set; }
        public List<ValueResponse> Tbkategoripremis { get; set; }
        public List<ValueResponse> Tbkategoriperniagaan { get; set; }
        public List<ValueResponse> Tbjenisperniagaan { get; set; }
        public List<ValueResponse> Tbpremis { get; set; }
        public List<ValueResponse> Tbakta { get; set; }
        public List<ValueResponse> Tbkesalahan { get; set; }

        public List<ValueResponse> Tbbandar { get; set; }
        public List<ValueResponse> Tbskipcontrol { get; set; }
        public List<ValueResponse> Tbbarang_jenama { get; set; }
    }

    public class ValueResponse
    {
        public string Value { get; set; }
    }
}