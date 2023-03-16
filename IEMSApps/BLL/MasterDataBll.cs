using System.Collections.Generic;
using System.Linq;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public static class MasterDataBll
    {
        public static string GetKategoriKawasan(string kodKatKawasan)
        {
            var listData = DataAccessQuery<TbKategoriKawasan>.GetAll();
            if (listData.Success)
            {
                var data = listData.Datas.FirstOrDefault(c => c.KodKatKawasan == kodKatKawasan);

                if (data != null)
                {
                    return data.Prgn;
                }
            }

            return "";
        }

        public static string GetTujuanLawatan(int kodTujuan)
        {
            var listData = DataAccessQuery<TbTujuanLawatan>.GetAll();
            if (listData.Success)
            {
                var data = listData.Datas.FirstOrDefault(c => c.KodTujuan == kodTujuan);

                if (data != null)
                {
                    return data.Prgn;
                }
            }

            return "";
        }

        public static string GetAsasTindakan(int kodTujuan, int kodAsas)
        {
            var listData = DataAccessQuery<TbAsasTindakan>.GetAll();
            if (listData.Success)
            {
                var data = listData.Datas.FirstOrDefault(c => c.KodTujuan == kodTujuan && c.KodAsas == kodAsas);

                if (data != null)
                {
                    return data.Prgn;
                }
            }

            return "";
        }

        public static string GetAgensiByAsas(int kodTujuan, int kodAsas)
        {
            var listData = DataAccessQuery<TbAsasTindakan>.GetAll();
            if (listData.Success)
            {
                var data = listData.Datas.FirstOrDefault(c => c.KodTujuan == kodTujuan && c.KodAsas == kodAsas);

                if (data != null)
                {
                    return data.Prgn;
                }
            }

            return "";
        }

        public static List<string> GetKesalahanButir(string kodAkta, int kodSalah)
        {
            var result = new List<string>();

            var listData = DataAccessQuery<TbKesalahanButir>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.Where(c => c.KodAkta == kodAkta && c.KodSalah == kodSalah).ToList();
                foreach (var tbKesalahanButir in list)
                {
                    result.Add(tbKesalahanButir.Prgn);
                }
            }


            return result;
        }

        public static string GetKatPremisName(int kodKatPremis)
        {

            var data = DataAccessQuery<TbKategoriPremis>.Get(c => c.KodKatPremis == kodKatPremis);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

        public static string GetKatKawasanName(string kodKatKawasan)
        {

            var data = DataAccessQuery<TbKategoriKawasan>.Get(c => c.KodKatKawasan == kodKatKawasan);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

        public static string GetTujuanLawatanName(int kodTujuan)
        {

            var data = DataAccessQuery<TbTujuanLawatan>.Get(c => c.KodTujuan == kodTujuan);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

        public static string GetAsasTindakanName(int kodTujuan, int kodAsas)
        {

            var data = DataAccessQuery<TbAsasTindakan>.Get(c => c.KodAsas == kodAsas && c.KodTujuan == kodTujuan);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

        public static string GetJenisPerniagaanName(int kod)
        {

            var data = DataAccessQuery<TbJenisPerniagaan>.Get(c => c.KodJenis == kod);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

        public static TbCawangan GetCawanganUser()
        {
            var userCawangan = GeneralBll.GetUserCawangan();

            var data = DataAccessQuery<TbCawangan>.Get(c => c.KodCawangan == userCawangan);

            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }


            return null;
        }

        public static TbCawangan GetCawangan(string kodCawangan)
        {
            var data = DataAccessQuery<TbCawangan>.Get(c => c.KodCawangan == kodCawangan);

            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }


            return null;
        }

        public static string GetNegeriName(int kodNegeri)
        {
            var kodNegeri1 = kodNegeri.ToString();
            var kodNegeri2 = kodNegeri.ToString("00");
            var data = DataAccessQuery<TbNegeri>.Get(c =>
                c.KodNegeri == kodNegeri1 || c.KodNegeri == kodNegeri2);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

        public static string GetAktaName(string kod)
        {

            var data = DataAccessQuery<TbAkta>.Get(c => c.KodAkta == kod);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

        public static string GetKesalahanName(int kodSalah, string kodAkta)
        {

            var data = DataAccessQuery<TbKesalahan>.Get(c => c.KodSalah == kodSalah && c.KodAkta == kodAkta);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

        public static string GetPegawaiSerbuName(int id)
        {

            var data = DataAccessQuery<TbPengguna>.Get(c => c.Id == id);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Nama;
            }


            return "";
        }
        public static string GetPenggunaSingkatanAndName(int id)
        {

            var data = DataAccessQuery<TbPengguna>.Get(c => c.Id == id);

            if (data.Success && data.Datas != null)
            {
                return $"{data.Datas.Singkatan_Jawatan} {data.Datas.Nama}";
            }


            return "";
        }


        public static TbAkta GetAktaByKod(string kod)
        {

            var data = DataAccessQuery<TbAkta>.Get(c => c.KodAkta == kod);

            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }


            return null;
        }

        public static TbKesalahan GetKesalahan(int kodSalah, string kodAkta)
        {

            var data = DataAccessQuery<TbKesalahan>.Get(c => c.KodSalah == kodSalah && c.KodAkta == kodAkta);

            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }


            return null;
        }

        public static List<JenisNiagaDto> GetAllJenisPerniagaan()
        {
            var result = new List<JenisNiagaDto>();

            var listData = DataAccessQuery<TbJenisPerniagaan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var tbJenisPerniagaan in list)
                {
                    var data = new JenisNiagaDto
                    {
                        KodJenis = tbJenisPerniagaan.KodJenis,
                        Prgn = tbJenisPerniagaan.Prgn
                    };

                    result.Add(data);
                }
            }

            return result;
        }

        public static List<AsasTindakanDto> GetAsasTindakanByTujuan()
        {
            var result = new List<AsasTindakanDto>();

            var listData = DataAccessQuery<TbAsasTindakan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.OrderBy(m => m.Prgn);
                foreach (var tbAsas in list)
                {
                    var data = new AsasTindakanDto
                    {
                        KodAsas = tbAsas.KodAsas,
                        Prgn = tbAsas.Prgn,
                        KodTujuan = tbAsas.KodTujuan
                    };

                    result.Add(data);
                }
            }

            return result;
        }

        public static List<KesalahanDto> GetKesalahanByAkta(string kodAkta)
        {
            var result = new List<KesalahanDto>();

            var listData = DataAccessQuery<TbKesalahan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.Where(c => c.KodAkta == kodAkta).ToList();
                foreach (var tbKesalahan in list)
                {
                    var data = new KesalahanDto
                    {
                        KodAkta = tbKesalahan.KodAkta,
                        KodSalah = tbKesalahan.KodSalah,
                        Prgn = $"({tbKesalahan.Seksyen}), {tbKesalahan.Prgn}",
                        Seksyen = tbKesalahan.Seksyen,
                        OriginalPrgn = tbKesalahan.Prgn
                    };

                    result.Add(data);
                }
            }

            return result;
        }

        public static List<KesalahanDto> GetKesalahanByAktaKots(string kodAkta)
        {
            var result = new List<KesalahanDto>();

            var listData = DataAccessQuery<TbKesalahan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.Where(c => c.KodAkta == kodAkta && c.KOTS == Constants.KompaunKots).ToList();
                foreach (var tbKesalahan in list)
                {
                    var data = new KesalahanDto
                    {
                        KodAkta = tbKesalahan.KodAkta,
                        KodSalah = tbKesalahan.KodSalah,
                        Prgn = $"({tbKesalahan.Seksyen}), {tbKesalahan.Prgn}",
                        Seksyen = tbKesalahan.Seksyen,
                        OriginalPrgn = tbKesalahan.Prgn
                    };

                    result.Add(data);
                }
            }

            return result;
        }
        public static TbPengguna GetPenggunaById(int idPengguna)
        {

            var data = DataAccessQuery<TbPengguna>.Get(c => c.Id == idPengguna);

            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }


            return null;
        }

        public static TbSendOnlineData GetTbSendOnlineByRujukanAndType(string noRujukan, Enums.TableType type)
        {
            var data = DataAccessQuery<TbSendOnlineData>.Get(c => c.NoRujukan == noRujukan && c.Type == type);
            if (data.Success && data.Datas != null)
            {
                return data.Datas;
            }

            return null;
        }

        public static string GetKppAsasTindakan(string noRujukan)
        {
            var listData = DataAccessQuery<TbKppAsasTindakan>.GetAll();
            if (listData.Success)
            {
                var listPrgn = new List<string>();
                var data = listData.Datas.Where(c => c.NoRujukanKpp == noRujukan).ToList();

                foreach (var tbKppAsasTindakan in data)
                {
                    var asas = GetAsasTindakanName(tbKppAsasTindakan.KodTujuan, tbKppAsasTindakan.KodAsas);
                    if (!string.IsNullOrEmpty(asas))
                    {
                        listPrgn.Add(asas);
                    }
                }
                if (listPrgn.Count > 0)
                {
                    return string.Join(", ", listPrgn);
                }

            }

            return "";
        }

        public static Dictionary<string, string> GetAllKategoriPerniagaan()
        {
            var result = new Dictionary<string, string>();

            result.Add("-", "");

            var listData = DataAccessQuery<TbKategoriPerniagaan>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var tbKategoriPerniagaan in list)
                {
                    result.Add(tbKategoriPerniagaan.KodKatPerniagaan.ToString(), tbKategoriPerniagaan.Prgn);
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetAllJenama()
        {
            var result = new Dictionary<string, string>();

            result.Add("-", "");

            var listData = DataAccessQuery<TbJenama>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var tbJenama in list)
                {
                    result.Add(tbJenama.KodJenama.ToString(), tbJenama.Prgn);
                }
            }

            return result;
        }

        public static string GetKategoriPerniagaanName(int id)
        {
            var data = DataAccessQuery<TbKategoriPerniagaan>.Get(c => c.KodKatPerniagaan == id);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }
            return "";
        }

        public static string GetJenamaName(int id)
        {
            var data = DataAccessQuery<TbJenama>.Get(c => c.KodJenama == id);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }
            return "";
        }

        public static Dictionary<string, string> GetAllNegeri(bool isAddDefault = true)
        {
            var result = new Dictionary<string, string>();
            if (isAddDefault)
            {
                result.Add("0", "");
            }
            var listData = DataAccessQuery<TbNegeri>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.ToList();
                foreach (var tbNegeri in list)
                {
                    result.Add(tbNegeri.KodNegeri, tbNegeri.Prgn);
                }
            }

            return result;
        }

        public static List<BandarDto> GetBandarByNegeri(string kodNegeri)
        {
            var result = new List<BandarDto>();

            var listData = DataAccessQuery<TbBandar>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas.Where(c => c.KodNegeri == kodNegeri).ToList();
                foreach (var tbBandar in list)
                {
                    var data = new BandarDto
                    {
                        KodNegeri = kodNegeri,
                        KodBandar = tbBandar.KodBandar,
                        Prgn = tbBandar.Prgn
                    };

                    result.Add(data);
                }
            }

            return result;
        }

        public static string GetBandarNameByNegeri(int kodNegeri, int kodBandar)
        {
            var kodNegeri1 = kodNegeri.ToString();
            var kodNegeri2 = kodNegeri.ToString("00");
            var data = DataAccessQuery<TbBandar>.Get(c =>
                (c.KodNegeri == kodNegeri1 || c.KodNegeri == kodNegeri2) && c.KodBandar == kodBandar);

            if (data.Success && data.Datas != null)
            {
                return data.Datas.Prgn;
            }


            return "";
        }

    }
}