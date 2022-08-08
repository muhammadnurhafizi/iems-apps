using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using System.Collections.Generic;
using System.Linq;

namespace IEMSApps.BLL
{
    public class PenggunaBll
    {
        public static List<string> GetNamaPengguna(bool basedUserKodCawangan = true)
        {
            var kodCawangan = SharedPreferences.GetString(SharedPreferencesKeys.UserKodCawangan);

            var listData = DataAccessQuery<TbPengguna>.GetAll();
            if (listData.Success)
            {
                if (basedUserKodCawangan)
                    return listData.Datas.Where(m => m.KodCawangan == kodCawangan)?.Select(m => m.Nama).ToList();
                else
                    return listData.Datas.Where(m => m.KodCawangan != kodCawangan)?.Select(m => m.Nama).ToList();
            }

            return new List<string>();
        }

        public static TbPengguna GetPenggunaByName (string name)
        {
            var listData = DataAccessQuery<TbPengguna>.GetAll();
            if (listData.Success)
            {
                return listData.Datas.SingleOrDefault(m => m.Nama == name);
            }
            return null;
        }
    }
}