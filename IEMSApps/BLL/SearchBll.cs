using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Inputs;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public static class SearchBll
    {
        public static List<SearchDto> GetSearchData(SearchDataInput input)
        {
            var result = new List<SearchDto>();

            var listKpp = DataAccessQuery<TbKpp>.GetAll();
            if (listKpp.Success)
            {
                if (!string.IsNullOrEmpty(input.SearchValue) &&
                    input.CarianType != Enums.SearchCarianType.All)
                {
                    if (input.CarianType == Enums.SearchCarianType.NamaPremis)
                    {
                        listKpp.Datas = listKpp.Datas.Where(c => c.NamaPremis.Contains(input.SearchValue)).ToList();
                    }
                    else if (input.CarianType == Enums.SearchCarianType.NoAduan)
                    {
                        listKpp.Datas = listKpp.Datas.Where(c => c.NoAduan.Contains(input.SearchValue)).ToList();
                    }
                    else if (input.CarianType == Enums.SearchCarianType.NoIcPenerima)
                    {
                        listKpp.Datas = listKpp.Datas.Where(c => c.NoKpPenerima.Contains(input.SearchValue)).ToList();
                    }
                    else if (input.CarianType == Enums.SearchCarianType.NoKpp)
                    {
                        listKpp.Datas = listKpp.Datas.Where(c => c.NoRujukanKpp.Contains(input.SearchValue)).ToList();
                    }
                }

                if (input.TindakanType != Enums.SearchTindakanType.All)
                {
                    if (input.TindakanType == Enums.SearchTindakanType.TiadaKes)
                    {
                        listKpp.Datas = listKpp.Datas.Where(c => c.Tindakan == Constants.Tindakan.TiadaKes).ToList();
                    }
                    else if (input.TindakanType == Enums.SearchTindakanType.Kots)
                    {
                        listKpp.Datas = listKpp.Datas.Where(c => c.Tindakan == Constants.Tindakan.Kots).ToList();
                    }
                    else if (input.TindakanType == Enums.SearchTindakanType.SiasatLanjutan)
                    {
                        listKpp.Datas = listKpp.Datas.Where(c => c.Tindakan == Constants.Tindakan.SiasatLanjutan).ToList();
                    }
                    else if (input.TindakanType == Enums.SearchTindakanType.SiasatUlangan)
                    {
                        listKpp.Datas = listKpp.Datas.Where(c => c.Tindakan == Constants.Tindakan.SiasatUlangan).ToList();
                    }
                }

                foreach (var tbKpp in listKpp.Datas)
                {
                    var searchDto = new SearchDto
                    {
                        NoRujukan = tbKpp.NoRujukanKpp,
                        Name = tbKpp.NamaPremis,
                        IsSent = DataAlreadySend(tbKpp.NoRujukanKpp, tbKpp.Tindakan)
                    };

                    if (tbKpp.Tindakan == Constants.Tindakan.TiadaKes)
                    {
                        searchDto.Tindakan = Constants.TindakanName.Pemeriksaan;
                    }
                    else if (tbKpp.Tindakan == Constants.Tindakan.Kots)
                    {
                        searchDto.Tindakan = Constants.TindakanName.KOTS;
                    }
                    else if (tbKpp.Tindakan == Constants.Tindakan.SiasatLanjutan)
                    {
                        searchDto.Tindakan = Constants.TindakanName.SiasatLanjut;
                    }
                    else if (tbKpp.Tindakan == Constants.Tindakan.SiasatUlangan)
                    {
                        searchDto.Tindakan = Constants.TindakanName.SiasatUlangan;
                    }
                    else if (tbKpp.Tindakan == Constants.Tindakan.SerahanNotis)
                    {
                        searchDto.Tindakan = Constants.TindakanName.SerahanNotis;
                    }
                    result.Add(searchDto);
                }

            }

            if (result.Any())
                return result.OrderBy(m => m.NoRujukan).ToList();
            return result;
        }

        public static bool DataAlreadySend(string noRujukanKpp, int tindakan)
        {
            //This script for
            // just check type 5
            var sqlTindakan = "" +
                $" SELECT 1 FROM tbsendonline_data WHERE NoRujukan = '{noRujukanKpp}' AND TYPE IN(1) AND Status<> 1 " +
                $" UNION ALL " +
                $" SELECT 1 FROM tbpasukan_trans WHERE NoRujukan = '{noRujukanKpp}' AND JenisTrans == 2 AND IsSendOnline<> 1 " +
                $" UNION ALL " +
                $" SELECT 1 FROM tbsendonline_gambar WHERE NoRujukan = '{noRujukanKpp}' AND Status<> 1 " +
                $" UNION ALL " +
                $" SELECT 1 FROM tbkpp_asastindakan WHERE NoRujukanKpp = '{noRujukanKpp}' AND IsSendOnline<> 1 " +
                $" ";

            if (tindakan == Constants.Tindakan.Kots)
            {
                // just check type 5
                //for KOTS, exclude 5 (kpp_hh) and datakes_hh (7)
                sqlTindakan = "" +
                    $" SELECT 1 FROM tbsendonline_data WHERE NoRujukan = '{noRujukanKpp}' AND TYPE IN(1) AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 1 FROM tbpasukan_trans WHERE NoRujukan = '{noRujukanKpp}' AND JenisTrans == 2 AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 1 FROM tbsendonline_gambar WHERE NoRujukan = '{noRujukanKpp}' AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 1 FROM tbkpp_asastindakan WHERE NoRujukanKpp = '{noRujukanKpp}' AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 2 FROM tbsendonline_data WHERE NoRujukan IN(SELECT NoKmp FROM tbkompaun WHERE NoRujukanKpp = '{noRujukanKpp}') AND TYPE IN(2, 6) AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 2 FROM tbsendonline_gambar WHERE NoRujukan = '{noRujukanKpp}' AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 2 FROM tbsendonline_data WHERE NoRujukan IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND TYPE IN(3) AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbsendonline_data WHERE NoRujukan IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND TYPE IN(3) AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbdatakes_pesalah WHERE NoKes IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbdatakes_asastindakan WHERE NoKes IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbdatakes_kesalahan WHERE NoKes IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbsendonline_gambar WHERE NoRujukan = '{noRujukanKpp}' AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 4 FROM tbsendonline_data WHERE NoRujukan IN(SELECT NoKmp FROM tbkompaun WHERE NoRujukanKpp = '{noRujukanKpp}') AND TYPE IN(2, 6) AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 4 FROM tbsendonline_data WHERE NoRujukan IN(SELECT NoKmp FROM tbkompaun WHERE NoRujukanKpp = '{noRujukanKpp}') AND TYPE IN(4) AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 4 FROM tbsendonline_data WHERE NoRujukan IN(SELECT NoKmp FROM tbkompaun WHERE NoRujukanKpp = '{noRujukanKpp}') AND TYPE IN(8, 9) AND Status<> 1";
            }
            if (tindakan == Constants.Tindakan.SiasatLanjutan)
            {
                sqlTindakan = "" +
                    $" SELECT 1 FROM tbsendonline_data WHERE NoRujukan = '{noRujukanKpp}' AND TYPE IN(1) AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 1 FROM tbpasukan_trans WHERE NoRujukan = '{noRujukanKpp}' AND JenisTrans == 2 AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 1 FROM tbsendonline_gambar WHERE NoRujukan = '{noRujukanKpp}' AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 1 FROM tbkpp_asastindakan WHERE NoRujukanKpp = '{noRujukanKpp}' AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbsendonline_data WHERE NoRujukan IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND TYPE IN(3) AND Status<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbdatakes_pesalah WHERE NoKes IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbdatakes_asastindakan WHERE NoKes IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbdatakes_kesalahan WHERE NoKes IN(SELECT NoKes FROM tbdatakes WHERE NoKpp = '{noRujukanKpp}') AND IsSendOnline<> 1 " +
                    $" UNION ALL " +
                    $" SELECT 3 FROM tbsendonline_gambar WHERE NoRujukan = '{noRujukanKpp}' AND Status<> 1 ";
            }

            var haveFaildData = DataAccessQuery<List<int>>.ExecuteSelectSql(sqlTindakan);
            return !haveFaildData.Any();

            //var dataSentOnline = DataAccessQuery<TbSendOnlineData>.GetAll(m => m.NoRujukan == noRujukanKpp);
            //if (dataSentOnline.Success)
            //{
            //
            //    // tindakan will check data for KPP and KPP_HH
            //    var types = new List<Enums.TableType>()
            //    {
            //        Enums.TableType.KPP,
            //        Enums.TableType.KPP_HH
            //    };
            //
            //    //if tindakan KOST it will check all table type
            //    if (tindakan == Constants.Tindakan.Kots)
            //    {
            //        types.Add(Enums.TableType.Kompaun);
            //        types.Add(Enums.TableType.DataKes);
            //        types.Add(Enums.TableType.KompaunBayaran);
            //        types.Add(Enums.TableType.Kompaun_HH);
            //        types.Add(Enums.TableType.DataKes_HH);
            //        types.Add(Enums.TableType.Akuan_UpdateKompaun);
            //        types.Add(Enums.TableType.Akuan_UpdateKompaun_HH);
            //    }
            //
            //    //if tindakan Kes Baru it will check KPP, KPP_HH, DataKes and DataKes_HH
            //    if (tindakan == Constants.Tindakan.SiasatLanjutan)
            //    {
            //        types.Add(Enums.TableType.DataKes);
            //        types.Add(Enums.TableType.DataKes_HH);
            //    }
            //
            //    return !dataSentOnline.Datas.Any(m => types.Any(x => x == m.Type) && m.Status != Enums.StatusOnline.Sent);
            //}
            //
            //return false;

        }
    }
}