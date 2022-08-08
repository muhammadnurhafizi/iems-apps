using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using System.Linq;

namespace IEMSApps.BLL
{
    public class NegeriBll
    {
        public static TbNegeri GetNegeriByKodNegeri(int kodNegeri)
        {
            var listData = DataAccessQuery<TbNegeri>.GetAll();
            if (listData.Success)
            {
                return listData.Datas.SingleOrDefault(m =>
                    m.KodNegeri == kodNegeri.ToString() || m.KodNegeri == kodNegeri.ToString("00"));
            }
            return null;
        }
    }
}