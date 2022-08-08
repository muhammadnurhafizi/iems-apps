using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using System.Linq;

namespace IEMSApps.BLL
{
    public class CawanganBll
    {
        public static TbCawangan GetPenggunaByID(string kodCawangan)
        {
            var listData = DataAccessQuery<TbCawangan>.GetAll();
            if (listData.Success)
            {
                return listData.Datas.SingleOrDefault(m => m.KodCawangan == kodCawangan);
            }
            return null;
        }
    }
}