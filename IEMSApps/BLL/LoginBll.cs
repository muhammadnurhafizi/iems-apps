using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public static class LoginBll
    {
        
        public static LoginDto GetUserLogin(string userName, string passwd)
        {
            if (userName == Constants.AdminUserValue || passwd == Constants.AdminPasswordValue)
            {
                return SetDataForAdmin();
            }
            var hashPassword = CreateMd5(passwd);

            string query = "SELECT a.NoKp as UserId, b.Nama, a.NoKp, c.KodCawangan, a.KodPasukan," +
                           " a.IdHh as HandheldId, b.Id as StaffId, c.KodNegeri FROM tbpasukan_hh a" +
                           " inner join tbpengguna b on a.nokp = b.nokp" +
                           " inner join tbcawangan c on a.KodCawangan = c.KodCawangan" +
                           $" WHERE a.STATUS = '{Constants.Status.Aktif}' AND a.NoKp = '{userName}'" +
                           $" and b.Kata_Laluan = '{hashPassword}'";

#if DEBUG
            query = "SELECT a.NoKp as UserId, b.Nama, a.NoKp, c.KodCawangan, a.KodPasukan," +
                           " a.IdHh as HandheldId, b.Id as StaffId, c.KodNegeri FROM tbpasukan_hh a" +
                           " inner join tbpengguna b on a.nokp = b.nokp" +
                           " inner join tbcawangan c on a.KodCawangan = c.KodCawangan LIMIT 1";
#endif

            var listData = DataAccessQuery<LoginDto>.ExecuteSelectSql(query);
            return listData.Count > 0 ? listData[0] : null;
        }
        private static LoginDto SetDataForAdmin()
        {
            var loginDto = new LoginDto
            {
                UserId = Constants.AdminUserValue,
                Nama = Constants.AdminUserValue,
                NoKp = Constants.AdminNoKpValue,
                KodCawangan = "1",
                KodPasukan = 1,
                HandheldId = GeneralBll.GetUserHandheld(),
                StaffId = 0 //  pengguna.IdStaf
            };


            return loginDto;
        }

        public static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        public static TbPasukanHh GetInfoPasukan()
        {
            string query = "SELECT * FROM tbpasukan_hh WHERE JenisPengguna = " + (int)Enums.JenisPengguna.Ketua;

            var listData = DataAccessQuery<TbPasukanHh>.ExecuteSelectSql(query);
          
            if (listData.Count > 0)
            {
                return listData[0];
            }

            return null;

        }

        public static string GetNamaPenggunaByNoKp(string noKp)
        {
            var pengguna = DataAccessQuery<TbPengguna>.Get(c => c.NoKp == noKp);
            if (pengguna.Success && pengguna.Datas != null)
            {
                return pengguna.Datas.Nama;
            }
            return "";
        }
       

        public static List<MaklumatPasukanDto> ListMaklumatLogin()
        {
            var result = new List<MaklumatPasukanDto>();

            string query = "SELECT a.*, b.Nama FROM tbpasukan_hh a " +
                           "INNER JOIN tbpengguna b on a.nokp = b.nokp WHERE a.STATUS = '" + Constants.Status.Aktif + "'" +
                           " ORDER BY a.JenisPengguna ";
           

            var listPasukan = DataAccessQuery<LoginDto>.ExecuteSelectSql(query);
            if (listPasukan.Count > 0)
            {
                foreach (var tbPasukan in listPasukan)
                {
                    if (tbPasukan.JenisPengguna == Enums.JenisPengguna.Ketua)
                    {
                        result.Add(new MaklumatPasukanDto
                        {
                            JenisPasukan = "ID Pasukan",
                            NamaPasukan = tbPasukan.KodPasukan.ToString()
                        });

                        result.Add(new MaklumatPasukanDto
                        {
                            JenisPasukan = "Ketua Pasukan",
                            NamaPasukan = tbPasukan.Nama
                        });
                    }
                    else
                    {
                        result.Add(new MaklumatPasukanDto
                        {
                            JenisPasukan = "Ahli",
                            NamaPasukan = tbPasukan.Nama
                        });
                    }
                }
            }

            return result;
        }
    }
}
