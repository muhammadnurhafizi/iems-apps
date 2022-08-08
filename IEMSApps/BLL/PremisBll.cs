using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using System.Collections.Generic;
using System.Linq;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public class PremisBll
    {
        public static List<PremisDto> GetPremisForListByKodCawangan(string kodCawangan)
        {
            var result = new List<PremisDto>();
            var listData = DataAccessQuery<TbPremis>.GetAll();
            if (listData.Success)
            {
                var listPremis = listData.Datas;
                if (kodCawangan != Constants.DefaultDisplayAllPremis)
                {
                    listPremis = listPremis.Where(m => m.KodCawangan == kodCawangan).ToList();
                }

                foreach (var item in listPremis)
                {
                    result.Add(new PremisDto
                    {
                        Nama = item.NamaPremis,
                        Alamat1 = item.AlamatPremis1,
                        Alamat2 = item.AlamatPremis2,
                        Alamat3 = item.AlamatPremis3
                    });
                }
            }
            return result;
        }
    }
}