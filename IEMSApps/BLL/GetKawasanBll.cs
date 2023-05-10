using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using System.Linq;

namespace IEMSApps.BLL
{
    public static class GetKawasanBll
    {
        private const string ClassName = "GetKawasanBll";

        public static bool CheckIpNegeri()
        {
            string sQuery = "SELECT * FROM ip_negeri";
            var result = DataAccessQuery<ip_negeri>.Any(sQuery);
            //if dont have data on db, then return true
            if (!result)
            {
                return true;
            }
            return false;
        }
        public static bool CheckIpBandar()
        {
            string sQuery = "SELECT * FROM ip_bandar";
            var result = DataAccessQuery<ip_bandar>.Any(sQuery);
            if (!result)
            {
                return true;
            }
            return false;
        }
        public static bool CheckIpPoskod()
        {
            string sQuery = "SELECT * FROM ip_poskod";
            var result = DataAccessQuery<ip_poskod>.Any(sQuery);
            if (!result)
            {
                return true;
            }
            return false;
        }
        public static bool CheckIpIdentitiPelanggan()
        {
            string sQuery = "SELECT * FROM ip_identiti_pelanggans";
            var result = DataAccessQuery<ip_identiti_pelanggans>.Any(sQuery);
            if (!result)
            {
                return true;
            }
            return false;
        }
        public static bool CheckIpChargeline()
        {
            string sQuery = "SELECT * FROM ip_chargelines";
            var result = DataAccessQuery<ip_chargelines>.Any(sQuery);
            if (!result)
            {
                return true;
            }
            return false;
        }

    }
}