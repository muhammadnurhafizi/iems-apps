using System;
using System.Collections.Generic;
using System.Linq;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Inputs;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public static class PasukanBll
    {

        public static PasukanAhliDto GetPasukanKetua()
        {  
            var query = "select b.nama, a.status , a.kodpasukan, b.nokp from tbpasukan_hh a" +
                        " inner join tbpengguna b on a.nokp = b.nokp" +
                        $" where a.jenispengguna = {(int)Enums.JenisPengguna.Ketua}" +
                        " order by turutan";

            var listData = DataAccessQuery<PasukanAhliDto>.ExecuteSelectSql(query);
            if (listData.Count > 0)
            {
                return listData[0];
            }

            return null;
        }


        public static List<PasukanAhliDto> GetListPasukanAhliByUserQuery()
        {
            var result = new List<PasukanAhliDto>();

            var kodPasukan = GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.UserKodPasukan));

            var query = "select b.nama, a.status , a.kodpasukan, b.nokp from tbpasukan_hh a" +
                        " inner join tbpengguna b on a.nokp = b.nokp" +
                        $" where a.jenispengguna = {(int)Enums.JenisPengguna.Ahli} and a.kodpasukan = {kodPasukan}" +
                        $" and a.status = '{Constants.Status.Aktif}'" +
                        " order by turutan";

            var listData = DataAccessQuery<PasukanAhliDto>.ExecuteSelectSql(query);
            if (listData.Count > 0)
            {
                int index = 1;
                foreach (var tbPasukanDetail in listData)
                {
                    tbPasukanDetail.NoUrut = index;

                    result.Add(tbPasukanDetail);

                    index++;
                }
            }

            return result;
        }



        public static List<PasukanAhliDto> GetListPasukanAhliByUser()
        {
            var result = new List<PasukanAhliDto>();

            var kodPasukan = GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.UserKodPasukan));

            var listData = DataAccessQuery<TbPasukanHh>.GetAll();
            if (listData.Success)
            {
                var list = listData.Datas
                     .Where(c => c.JenisPengguna == Enums.JenisPengguna.Ahli && c.KodPasukan == kodPasukan &&
                     c.Status == Constants.Status.Aktif).ToList();

                int index = 1;
                foreach (var tbPasukanDetail in list)
                {
                    var data = new PasukanAhliDto();
                    data.NoUrut = index;
                    data.Nama = LoginBll.GetNamaPenggunaByNoKp(tbPasukanDetail.NoKp);
                    data.Status = tbPasukanDetail.Status == Constants.Status.Aktif
                        ? Enums.Status.Aktif.ToString()
                        : Enums.Status.TidakAktif.ToString();

                    data.KodPasukan = tbPasukanDetail.KodPasukan;
                    data.NoKp = tbPasukanDetail.NoKp;

                    result.Add(data);

                    index++;
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetAllNegeri()
        {
            var result = new Dictionary<string, string>();

            var negeris = DataAccessQuery<TbNegeri>.GetAll();
            if (negeris.Success)
            {
                var listNegeri = negeris.Datas.ToList();
                foreach (var tbNegeri in listNegeri)
                {
                    result.Add(tbNegeri.KodNegeri, tbNegeri.Prgn);
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetCawanganByNegeri(string kodNegeri)
        {
            var idNegeri = GeneralBll.ConvertStringToInt(kodNegeri);

            var result = new Dictionary<string, string>();

            var cawangans = DataAccessQuery<TbCawangan>.GetAll();
            if (cawangans.Success)
            {
                var listCawangan = cawangans.Datas
                    .Where(c => c.KodNegeri == idNegeri.ToString() || c.KodNegeri == idNegeri.ToString("00")).ToList();

                foreach (var tbCawangan in listCawangan)
                {
                    result.Add(tbCawangan.KodCawangan, tbCawangan.Prgn);
                }
            }

            return result;
        }

        public static string GetNegeriUser()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.UserNegeri);


            //var negeris = DataAccessQuery<TbNegeri>.GetAll();
            //if (negeris.Success)
            //{
            //    var negeri = negeris.Datas.FirstOrDefault(c =>
            //        c.Status == Constants.Status.Aktif && c.KodNegeri == kodNegeri.ToString() ||
            //        c.KodNegeri == kodNegeri.ToString("00"));
            //    if (negeri != null)
            //    {
            //        return negeri.KodNegeri + Constants.SeparateCharList + negeri.Prgn;
            //    }
            //}

            //return "";
        }

        public static string GetCawanganUser()
        {
            return SharedPreferences.GetString(SharedPreferencesKeys.UserKodCawangan);


            //var listData = DataAccessQuery<TbCawangan>.GetAll();
            //if (listData.Success)
            //{
            //    var tbCawangan = listData.Datas.FirstOrDefault(c =>
            //        c.Status == Constants.Status.Aktif && c.KodCawangan == kodCawangan );
            //    if (tbCawangan != null)
            //    {
            //        return tbCawangan.KodCawangan + Constants.SeparateCharList + tbCawangan.Prgn;
            //    }
            //}

            //return "";
        }

        public static int GetPositionSelected(List<string> listData, string data)
        {
            int index = 0;
            foreach (var sValue in listData)
            {
                if (sValue == data) return index;
                index++;
            }

            return 0;
        }
        public static int GetPositionSelected(Dictionary<string, string> listData, string sValue)
        {
            int index = 0;
            foreach (var data in listData)
            {
                if (data.Key == sValue) return index;
                index++;
            }

            return 0;
        }

        public static int GetPositionNegeriSelected(Dictionary<string, string> listData, string sValue)
        {
            var negeri = GeneralBll.ConvertStringToInt(sValue);
            int index = 0;
            foreach (var data in listData)
            {
                if (data.Key == negeri.ToString() || data.Key == negeri.ToString("00")) return index;
                index++;
            }

            return 0;
        }

        public static Dictionary<string, string> GetPenguatKuasaByCawangan(string kodCawangan)
        {

            var result = new Dictionary<string, string>();

            var listData = DataAccessQuery<TbPengguna>.GetAll();
            if (listData.Success)
            {
                var listPengguna = listData.Datas.Where(c => c.KodCawangan == kodCawangan).ToList();

                foreach (var tbPengguna in listPengguna)
                {
                    result.Add(tbPengguna.NoKp, tbPengguna.Nama);
                }
            }

            return result;
        }

        public static Result<TbPasukanHh> AddPasukanAhli(PasukanAhliInput input)
        {
            var response = new Result<TbPasukanHh>() { Success = true };

            var existingData = GetPasukanByNoKpWithoutStatus(input.NoKp);
            if (existingData == null)
            {
                var tbPengguna = GetPenggunaByNoKp(input.NoKp);
                if (tbPengguna != null)
                {
                    var pasukan = new TbPasukanHh()
                    {
                        KodPasukan = GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.UserKodPasukan)),
                        NoKp = input.NoKp,
                        KodCawangan = GeneralBll.GetUserCawangan(), //input.SelectedCawangan,
                        JenisPengguna = Enums.JenisPengguna.Ahli,
                        Status = Constants.Status.Aktif,
                        PgnDaftar = tbPengguna.PgnDaftar,
                        TrkhDaftar = tbPengguna.TrkhDaftar,
                        Id = tbPengguna.Id,
                        IdHh = GeneralBll.GetUserHandheld(),
                        Turutan = GetCountDataPasukanHh() + 1
                    };

                    var result = DataAccessQuery<TbPasukanHh>.Insert(pasukan);
                }
            }
            else
            {
                response = new Result<TbPasukanHh>() { Success = false, Datas = existingData };
            }

            return response;
        }

        public static int GetCountDataPasukanHh()
        {
            var data = DataAccessQuery<TbPasukanHh>.GetAll();
            if (data.Success)
            {
                return data.Datas.Count;
            }
            return 0;
        }
        public static TbPengguna GetPenggunaByNoKp(string noKp)
        {
            string sQuery = "SELECT * FROM TBPENGGUNA WHERE NOKP = '" + noKp + "'";

            return DataAccessQuery<TbPengguna>.ExecuteSelectSql(sQuery).FirstOrDefault();
        }

        public static TbPasukanHh GetPasukanByNoKp(string noKp)
        {
            var kodPasukan = GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.UserKodPasukan));

            var listData = DataAccessQuery<TbPasukanHh>.GetAll();
            if (listData.Success)
            {
                return listData.Datas.FirstOrDefault(c =>
                    c.Status == Constants.Status.Aktif && c.NoKp == noKp && c.KodPasukan == kodPasukan);

            }

            return null;
        }

        public static TbPasukanHh GetPasukanByNoKpWithoutStatus(string noKp)
        {
            var kodPasukan = GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.UserKodPasukan));

            var listData = DataAccessQuery<TbPasukanHh>.GetAll();
            if (listData.Success)
            {
                return listData.Datas.FirstOrDefault(c => c.NoKp == noKp && c.KodPasukan == kodPasukan);

            }

            return null;
        }

        public static bool RemovePasukanByNoKp(string noKp, string catatan)
        {
            var kodPasukan = GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.UserKodPasukan));
            var dtDate = GeneralBll.GetLocalDateTime();
            catatan += "(" + dtDate.ToString(Constants.DateFormatDisplay) + " " +
                       dtDate.ToString(Constants.TimeFormatDisplay) + ")";

            string sQuery = "UPDATE tbpasukan_hh SET Status = '" + Constants.Status.TidakAktif +
                            "',Catatan = '" + catatan + "'  where NoKp = '" + noKp + "'" +
                            " AND KodPasukan = " + kodPasukan;

            var result = DataAccessQuery<TbPasukanHh>.ExecuteSql(sQuery);

            return result != Constants.Error;
        }

        public static bool UpdateStatusAktifPasukan(string noKp)
        {
            var kodPasukan = GeneralBll.ConvertStringToInt(SharedPreferences.GetString(SharedPreferencesKeys.UserKodPasukan));

            string sQuery = "UPDATE tbpasukan_hh SET Status = '" + Constants.Status.Aktif +
                            "'  where NoKp = '" + noKp + "'" +
                            " AND KodPasukan = " + kodPasukan;

            var result = DataAccessQuery<TbPasukanHh>.ExecuteSql(sQuery);

            return result != Constants.Error;
        }
    }
}