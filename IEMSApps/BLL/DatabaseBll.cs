using System;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public static class DatabaseBll
    {
        private const bool WithDefaultData = false;

        public static bool CreateDefaultDatabase()
        {
            CreateTbAkta();
            CreateTbAsasTindakan();
            CreateTbCawangan();
            CreateTbDataKes();
            CreateTbHandheld();
            CreateTbJenisPerniagaan();
            CreateTbKategoriKawasan();
            CreateTbKategoriPerniagaan();
            CreateTbKategoriPremis();
            CreateTbKesalahan();
            CreateTbKesalahanButir();
            CreateTbKompaun();
            CreateTbNegeri();
            CreateTbPasukanHh();
            CreateTbPengguna();
            CreateTbTujuanLawatan();

            CreateTbKpp();
            CreateTbPasukanTrans();
            CreateTbKompaunIzin();
            CreateTbPremis();
            CreateTbSendOnlineData();
            CreateTbSendOnlineGambar();
            CreateTbGpsLog();
            CreateTbKppAsasTindakan();
            CreateTbJenama();
            CreateTbSkipControl();
            CreateTbBandar();
            CreateTbDataKesKesalahan();
            CreateTbDataKesAsasTindakan();
            CreateTbDataKesPesalah();
            CreateTbPenggunaTemp();
            CreateTbCawanganTemp();
            CreateTbAsasTindakanTemp();
            CreateTbJenisPerniagaanTemp();
            CreateTbPremisTemp();
            CreateTbAktaTemp();
            CreateTbKesalahanTemp();
            CreateTbBandarTemp();

            CreateTbNegeriTemp();
            CreateTbTujuanLawatanTemp();
            CreateTbKategoryKawasanTemp();
            CreateTbKategoryPremisTemp();
            CreateTbKategoryPerniagaanTemp();
            CreateTbBarangJenamaTemp();

            //new add ipayment
            CreateIpIdentitiPelanggan();
            CreateIpBandar();
            CreateIpNegeri();
            CreateIpPoskod();
            CreateIpResits(); 
            CreateIpChargeline();
            CreateTbKompaunBayaran(); //new to add because pusat_terimaan

            return true;

            //if (!CreateTbAkta()) return false;
            //if (!CreateTbAsasTindakan()) return false;
            //if (!CreateTbCawangan()) return false;
            //if (!CreateTbDataKes()) return false;
            //if (!CreateTbHandheld()) return false;
            //if (!CreateTbJenisPerniagaan()) return false;
            //if (!CreateTbKategoriKawasan()) return false;
            //if (!CreateTbKategoriPerniagaan()) return false;
            //if (!CreateTbKategoriPremis()) return false;
            //if (!CreateTbKesalahan()) return false;
            //if (!CreateTbKesalahanButir()) return false;
            //if (!CreateTbKompaun()) return false;
            //if (!CreateTbNegeri()) return false;
            //if (!CreateTbPasukanHh()) return false;
            //if (!CreateTbPengguna()) return false;
            //if (!CreateTbTujuanLawatan()) return false;


            //if (!CreateTbPasukan()) return false;
            ////if (!CreateTbKonfigurasi()) return false;

            //if (!CreateTbKpp()) return false;
            //if (!CreateTbPasukanTrans()) return false;
            //if (!CreateTbKompaunIzin()) return false;
            //if (!CreateTbPremis()) return false;

            //if (!CreateTbSendOnlineData()) return false;
            //if (!CreateTbSendOnlineGambar()) return false;

            //if (!CreateTbGpsLog()) return false;

            //if (!CreateTbKppAsasTindakan()) return false;
            //if (!CreateTbJenama()) return false;
            //if (!CreateTbSkipControl()) return false;
            //if (!CreateTbBandar()) return false;

            //if (!CreateTbDataKesKesalahan()) return false;
            //if (!CreateTbDataKesAsasTindakan()) return false;
            //if (!CreateTbDataKesPesalah()) return false;

            //return true;
        }

        public static bool AlterDatabase()
        {
            if (CreateTbNegeriTemp())
            {
                CreateTbTujuanLawatanTemp();
                CreateTbKategoryKawasanTemp();
                CreateTbKategoryPremisTemp();
                CreateTbKategoryPerniagaanTemp();
                CreateTbBarangJenamaTemp();
                AlterTbHandheldAddTrkhUpdateDate();
                CreateTbBandarTemp();
                CreateTbKompaunIzin();
                CreateTbPremis();
                CreateTbSendOnlineData();
                CreateTbSendOnlineGambar();
                CreateTbGpsLog();
                AlterTbPasukanTrans();
                AlterTbHandheld();
                AlterTbKompaunIzin();
                CreateTbKppAsasTindakan();
                AlterTbKpp();
                AlterTbKppSiasatUlangan();
                CreateTbJenama();
                AlterTbDataKes();
                AlterTbKppSkipIzin();
                CreateTbSkipControl();
                CreateTbBandar();

                AlterTbKompaunNoEpIp();
                AlterTbDataKesNewStructure();
                CreateTbDataKesKesalahan();
                CreateTbDataKesAsasTindakan();
                CreateTbDataKesPesalah();
                AlterTbKesalahan();
                AlterTbKompaunIzinCatatan();
                CreateTbPenggunaTemp();
                CreateTbCawanganTemp();
                CreateTbAsasTindakanTemp();
                CreateTbJenisPerniagaanTemp();
                CreateTbPremisTemp();
                CreateTbAktaTemp();
                CreateTbKesalahanTemp();
            }
            //new
            AlterKodKatPerniagaanOnTbKpp();

            AlterTbKompaunBarangKompaun();
            CreateTbDataKesPesalahKesalahan();
            DropTbDataKesPesalahKesalahan();
            CreateTbError();

            //new add
            AlterTbKpp();
            AlterTbKpp2();
            AlterTbKompaun();
            if(CheckTableIpaymentExist()) 
            {
                CreateIpIdentitiPelanggan();
                CreateIpBandar();
                CreateIpNegeri();
                CreateIpPoskod();
                CreateIpResits();
                CreateIpChargeline();
                CreateTbKompaunBayaran(); //new add
            }
            

            return true;

            //if (!CreateTbKompaunIzin()) return false;
            //if (!CreateTbPremis()) return false;
            //if (!CreateTbSendOnlineData()) return false;
            //if (!CreateTbSendOnlineGambar()) return false;
            //if (!CreateTbGpsLog()) return false;
            //if (!AlterTbPasukanTrans()) return false;
            //if (!AlterTbHandheld()) return false;
            //if (!AlterTbKompaunIzin()) return false;
            //if (!CreateTbKppAsasTindakan()) return false;
            //if (!AlterTbKpp()) return false;
            //if (!AlterTbKppSiasatUlangan()) return false;
            //if (!CreateTbJenama()) return false;
            //if (!AlterTbDataKes()) return false;
            //if (!AlterTbKppSkipIzin()) return false;
            //if (!CreateTbSkipControl()) return false;
            //if (!CreateTbBandar()) return false;

            //if (!AlterTbKompaunNoEpIp()) return false;
            //if (!AlterTbDataKesNewStructure()) return false;
            //if (!CreateTbDataKesKesalahan()) return false;
            //if (!CreateTbDataKesAsasTindakan()) return false;
            //if (!CreateTbDataKesPesalah()) return false;
            //if (!AlterTbKesalahan()) return false;
            //if (!AlterTbKompaunIzinCatatan()) return false;

            //return true;
        }

        private static bool CheckTableIpaymentExist()
        {
            var result = DataAccessQuery<ip_identiti_pelanggans>.Count(c => c.jenis_identiti != "");
            if (result < 0)
            {
                return true;
            }
            return false;
        }

        public static bool CreateIpIdentitiPelanggan()
        {
            string sQuery = "SELECT * FROM ip_identiti_pelanggans";
            var result = DataAccessQuery<ip_identiti_pelanggans>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<ip_identiti_pelanggans>.CreateTable();

                return true;
            }
            return false;
        }

        public static bool CreateIpBandar()
        {
            string sQuery = "SELECT * FROM ip_bandar";
            var result = DataAccessQuery<ip_bandar>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<ip_bandar>.CreateTable();

                return true;
            }
            return false;
        }

        public static bool CreateIpNegeri()
        {
            string sQuery = "SELECT * FROM ip_negeri";
            var result = DataAccessQuery<ip_negeri>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<ip_negeri>.CreateTable();

                return true;
            }
            return false;
        }

        public static bool CreateIpPoskod()
        {
            string sQuery = "SELECT * FROM ip_poskod";
            var result = DataAccessQuery<ip_poskod>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<ip_poskod>.CreateTable();

                return true;
            }
            return false;
        }

        public static bool CreateTbAkta()
        {
            string sQuery = "SELECT * FROM tbakta";
            var result = DataAccessQuery<TbAkta>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbAkta>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbakta` VALUES ('ACO2000','ACO','AKTA CAKERA OPTIK 2000',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('AFS98','AFS','AKTA FRANCAIS 1998',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('AHC87','AHC','AKTA HAK CIPTA 1987',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('AJL11','AJLSAP','AKTA JUALAN LANGSUNG 2011',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('AJL72','AJL','AKTA JUALAN LANGSUNG 1972',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('AKB61','AKB','AKTA KAWALAN BEKALAN 1961',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('AKH46','AKH','AKTA KAWALAN HARGA 1946',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('AKHAP11','AKHAP','AKTA KAWALAN HARGA DAN ANTI PENCATUTAN 2011',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD11','APD','AKTA PERIHAL DAGANGAN 2011','Peraturan- Peraturan Perihal Dagangan (Harga Jualan Murah) 1997 ','AKTA PERIHAL DAGANGAN 2011','PERATURAN-PERATURAN PERIHAL DAGANGAN (PENGKOMPAUNAN KESALAHAN-KESALAHAN)(PINDAAN) 2010','[P.U.(A) 317/2010]','BORANG 1','TAWARAN UNTUK MENGKOMPAUN','Pengawal/Timbalan Pengawal Perihal Dagangan','Bahagian Penguatkuasa, Kementerian Perdagangan Dalam Negeri dan Hal Ehwal Pengguna,<CAWANGAN, NEGERI','Tuan/Puan,','Suatu laporan telah dibuat terhadap anda yang mengatakan anda telah melakukan kesalahan yang berikut di bawah <SEKSYEN KESALAHAN>','2. Anda adalah dengan ini dimaklumkan bahawa di bawah kuasa-kuasa yang terletak hak pada saya melalui seksyen 63 Akta Perihal Dagangan 2011, saya bersedia, dan dengan ini menawarkan untuk mengkompaun kesalahan itu dnegan bayaran sebanyak RM <AMNKMP> (Ringgit Malaysia : <WORDINGAMN>). Jika tawaran ini diterima, pembayaran hendaklah dibuat dengan wang tunai atau kiriman wang, wang pos atau draf bank yang dibuat untuk dibayar kepada Pengawal Perihal Dagangan di pejabat yang tersebut di atas. Suatu ','3. Tawaran ini akan terus berkuat kuasa selama <TEMPOH HARI> hari sahaja dari tarikh notis ini dan jika tiada pembayaran dibuat dalam tempoh itu pendakwaan akan dimulakan tanpa notis selanjutnya. ',NULL,'Pengawal Bekalan','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD12','APD','AKTA PERIHAL DAGANGAN 2012','Peraturan- Peraturan Perihal Dagangan (Harga Jualan Murah) 1997 ','AKTA PERIHAL DAGANGAN 2011','PERATURAN-PERATURAN PERIHAL DAGANGAN (PENGKOMPAUNAN KESALAHAN-KESALAHAN)(PINDAAN) 2010','[P.U.(A) 317/2010]','BORANG 1','TAWARAN UNTUK MENGKOMPAUN','Pengawal/Timbalan Pengawal Perihal Dagangan','Bahagian Penguatkuasa, Kementerian Perdagangan Dalam Negeri dan Hal Ehwal Pengguna,<CAWANGAN, NEGERI','Tuan/Puan,','Suatu laporan telah dibuat terhadap anda yang mengatakan anda telah melakukan kesalahan yang berikut di bawah <SEKSYEN KESALAHAN>','2. Anda adalah dengan ini dimaklumkan bahawa di bawah kuasa-kuasa yang terletak hak pada saya melalui seksyen 63 Akta Perihal Dagangan 2011, saya bersedia, dan dengan ini menawarkan untuk mengkompaun kesalahan itu dnegan bayaran sebanyak RM <AMNKMP> (Ringgit Malaysia : <WORDINGAMN>). Jika tawaran ini diterima, pembayaran hendaklah dibuat dengan wang tunai atau kiriman wang, wang pos atau draf bank yang dibuat untuk dibayar kepada Pengawal Perihal Dagangan di pejabat yang tersebut di atas. Suatu ','3. Tawaran ini akan terus berkuat kuasa selama <TEMPOH HARI> hari sahaja dari tarikh notis ini dan jika tiada pembayaran dibuat dalam tempoh itu pendakwaan akan dimulakan tanpa notis selanjutnya. ',NULL,'Pengawal Bekalan','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD13','APD','AKTA PERIHAL DAGANGAN 2013','Peraturan- Peraturan Perihal Dagangan (Harga Jualan Murah) 1997 ','AKTA PERIHAL DAGANGAN 2011','PERATURAN-PERATURAN PERIHAL DAGANGAN (PENGKOMPAUNAN KESALAHAN-KESALAHAN)(PINDAAN) 2010','[P.U.(A) 317/2010]','BORANG 1','TAWARAN UNTUK MENGKOMPAUN','Pengawal/Timbalan Pengawal Perihal Dagangan','Bahagian Penguatkuasa, Kementerian Perdagangan Dalam Negeri dan Hal Ehwal Pengguna,<CAWANGAN, NEGERI','Tuan/Puan,','Suatu laporan telah dibuat terhadap anda yang mengatakan anda telah melakukan kesalahan yang berikut di bawah <SEKSYEN KESALAHAN>','2. Anda adalah dengan ini dimaklumkan bahawa di bawah kuasa-kuasa yang terletak hak pada saya melalui seksyen 63 Akta Perihal Dagangan 2011, saya bersedia, dan dengan ini menawarkan untuk mengkompaun kesalahan itu dnegan bayaran sebanyak RM <AMNKMP> (Ringgit Malaysia : <WORDINGAMN>). Jika tawaran ini diterima, pembayaran hendaklah dibuat dengan wang tunai atau kiriman wang, wang pos atau draf bank yang dibuat untuk dibayar kepada Pengawal Perihal Dagangan di pejabat yang tersebut di atas. Suatu ','3. Tawaran ini akan terus berkuat kuasa selama <TEMPOH HARI> hari sahaja dari tarikh notis ini dan jika tiada pembayaran dibuat dalam tempoh itu pendakwaan akan dimulakan tanpa notis selanjutnya. ',NULL,'Pengawal Bekalan','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APP99','APP','AKTA PERLINDUNGAN PENGGUNA 1999',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('ASB67','ASB','AKTA SEWA BELI 1967',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('ATS72','ATS','AKTA TIMBANG SUKAT 1972',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";
                    result = DataAccessQuery<TbAkta>.ExecuteSql(script);
                    if (result == Constants.Error) return false;
                }
                return true;
            }
            return false;
        }

        public static bool CreateTbAsasTindakan()
        {
            string sQuery = "SELECT * FROM tbasastindakan";
            var result = DataAccessQuery<TbAsasTindakan>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbAsasTindakan>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbasastindakan` VALUES (1,1,'ADUAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(1,2,'ADUAN ONLINE','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,1,'ALAT TIMBANG SUKAT (GETAH)','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,2,'ALAT TIMBANG SUKAT DAN TANDA HARGA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,3,'BARANG TIRUAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,4,'BARCODE','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,5,'BATERI PRIMER','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,6,'BEKALAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,7,'BEKALAN AYAM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,8,'BENGKEL KENDERAAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,9,'BERSEPADU APMM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,10,'BERSEPADU ATM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,11,'BERSEPADU CAWANGAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,12,'BERSEPADU FAMA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,13,'BERSEPADU IMIGRESEN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,14,'BERSEPADU KASTAM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,15,'BERSEPADU NEGERI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,16,'BERSEPADU PDRM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,17,'BERSEPADU PGA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,18,'BERSEPADU PGM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,19,'BERSEPADU RELA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,20,'BERSEPADU SPRM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,21,'BERSEPADU UPP','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,22,'BONGKAR','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,23,'CETAK ROMPAK (GGCR)','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,24,'CROSS BORDER','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,25,'CROSS DISTRICT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,26,'DATA PROFILING','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,27,'DDR','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,28,'GMT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,29,'HALAL','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,30,'HARGA TERANGKUM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,31,'JUALAN KREDIT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,32,'JUALAN LANGSUNG','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,33,'JUALAN MURAH','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,34,'LESEN CSA/PERMIT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,35,'LPG','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,36,'NACCOL','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,37,'OPERASI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,38,'OPERASI BERSAMA KESIHATAN BAHAGIAN PENGURUSAN HALAL JHEAIPP','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,39,'OPS BANJIR','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,40,'OPS BAZAR RAMADHAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,41,'OPS BEKALAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,42,'OPS BERJADUAL','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,43,'OPS BONGKAR','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,44,'OPS BUNGKUS (CETAK ROMPAK)','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,45,'OPS CARGAS','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,46,'OPS CATUT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,47,'OPS GASAK','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,48,'OPS GORES 2','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,49,'OPS HARGA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,50,'OPS IGAK','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,51,'OPS LPG','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,52,'OPS PAKAIAN RAYA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,53,'OPS PDRM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,54,'OPS SURI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,55,'OPS TAUKE','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,56,'OPS TOTAL ENFORCEMENT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,57,'PASCA BANJIR','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,58,'PEMATUHAN AKHAP','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,59,'PEMATUHAN STANDARD BREK PAD','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,60,'PEMATUHAN STANDARD MAINAN KANAK-KANAK','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,61,'PEMATUHAN STANDARD TAYAR','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,62,'PEMATUHAN TOPI KELEDAR','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,63,'PEMBAIK ALAT TIMBANG','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,64,'PERNIAGAAN ATAS TALIAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,65,'PRE PACKAGE','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,66,'SEWA BELI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,67,'SHMMP-DEEPAVALI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,68,'SHMMP-HARI RAYA PUASA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,69,'SHMMP-KEAMATAN/GAWAI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,70,'SHMMP-KRISMAS','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,71,'SHMMP-TBC','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,72,'SK-ALAT TIMBANG GANDAR DI JPJ','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,73,'SK-BATERI PRIMER','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,74,'SK-BENGKEL KENDERAAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,75,'SK-HALAL','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,76,'SK-JUALAN MURAH','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,77,'SK-KACA KESELAMATAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,78,'SK-KONTRAK PERKHIDMATAN HADAPAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,79,'SK-MAINAN KANAK-KANAK','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,80,'SK-MESIN PENIMBANG KENDERAAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,81,'SK-OMNIPRESENCE-EVENT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,82,'SK-OMNIPRESENCE-HYPERMARKET','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,83,'SK-OMNIPRESENCE-PASAR MALAM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,84,'SK-OMNIPRESENCE-PASAR PAGI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,85,'SK-OMNIPRESENCE-SHOPINGMALL','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,86,'SK-OMNIPRESENCE-SUPERMARKET','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,87,'SK-PAM DISPENSER DI BENGKEL KENDERAAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,88,'SK-PASAR DALAM TALIAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,89,'SK-PEJABAT MCM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,90,'SK-PELAPIK BREK','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,91,'SK-PEMBAIK ALAT TIMBANG SUKAT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,92,'SK-PENJUAL ALAT TIMBANG SUKAT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,93,'SK-PUSPAKOM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,94,'SK-STAMPING POINT/SCR BERSAMA MCM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,100,'SK-STOKIS & SYARIKAT JUALAN LANGSUNG BERLESEN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,101,'SK-TAYAR CELUP','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,102,'SK-TAYAR PNEUMATIK','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,103,'SK-TOPI KELEDAR','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,104,'SK-URUSAN JUALAN KREDIT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,105,'SK-URUSAN SEWABELI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,106,'STAMPING POINT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,107,'STESEN MINYAK','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,108,'TANDA HARGA DAN JUALAN MURAH','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,109,'TIMBANG TEPAT HARGA JELAS (TTHJ)','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,110,'TOPENG MUKA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(2,111,'ZPT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(3,1,'BIASA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(3,2,'PEMERIKSAAN BERKALA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(3,3,'PEMERIKSAAN BIASA','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(3,4,'PEMERIKSAAN CILI MERAH','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(3,5,'PEMERIKSAAN GHOST SMOKE','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(3,6,'PEMERIKSAAN KANTIN SEKOLAH','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(3,7,'PEMERIKSAAN MINUMAN HEINEKEN ZERO ALKOHOL','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(3,8,'PREMIT BARANG KAWALAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(4,1,'NAZIRAN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(4,2,'NAZIRAN AKB','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(4,3,'NAZIRAN LESEN','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(4,4,'NAZIRAN MCM','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(4,5,'NAZIRAN PERMIT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(4,6,'NAZIRAN SURATKUASA/PERMIT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(5,1,'0% DEPOSIT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(5,2,'SERAHAN AGENSI','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(5,3,'SERAHAN KES','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00'),(5,4,'SIASATAN LANJUT','1','admin','2019-09-12 00:00:00','admin','2019-09-12 00:00:00');";
                    result = DataAccessQuery<TbAsasTindakan>.ExecuteSql(script);
                    if (result == Constants.Error) return false;
                }
                return true;
            }
            return false;
        }

        public static bool CreateTbCawangan()
        {
            string sQuery = "SELECT * FROM tbcawangan";
            var result = DataAccessQuery<TbCawangan>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbCawangan>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbcawangan` VALUES ('AGJ','ALOR GAJAH',107,NULL,'KPDNHEP Alor Gajah','Melaka','Alor Gajah','40150','4','Melaka','alorgajah@kpdnkk.gov.my','062345822','062345822','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('AST','ALOR SETAR',43,'KPDNKK ALOR SETAR','KPDNHEP Alor Setar','ARAS G, ZON C, WISMA PERSEKUTUAN, PUSAT PENTADBIRAN KERAJAAN PERSEKUTUAN','BANDAR MU\''ADZAM SHAH','06550','2','ALOR SETAR','asgroup@kpdnkk.gov.my','047001801','047001800','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('BDY','BARAT DAYA',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('BER','BERA',104,'','KPDNHEP Bera',NULL,NULL,NULL,'6',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('BFT','BEAUFORT',6,'KPDNKK BEAUFORT ','KPDNHEP Beaufort','KPDNKK CAWANGAN BEAUFORT, TINGKAT BAWAH DAN SATU','LOT T5, CERAH LIGHT INDUSTRIAL CENTRE','89800','12','BEAUFORT','beaufort@kpdnkk.gov.my','087222271','087222240','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('BLG','BALING',109,'KPDNKK BALING','KPDNHEP Baling','NO 1, BANGUNAN MAJLIS AGAMA ISLAM NEGERI KEDAH','JALAN BADLISHAH','09100','2','BALING','baling@kpdnkk.gov.my','044700121','044700120','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('BPT','BATU PAHAT',62,'KPDNKK Batu Pahat','KPDNHEP Batu Pahat','Aras 6, Wisma Sin Long','Jalan Zabedah','83000','1','Batu Pahat','kpdnkkbatupahat@kpdnkk.gov.my','074355497','074355478','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('BST','BESUT',53,'KPDNKK Besut','KPDNHEP Besut','Lot 5122 & 5123, Wisma Koperasi Guru-Guru Melayu Besut Bhd','Nyior Tujuh','22000','11','Jerteh, Besut','besut.kpdnkk@1govuc.gov.my','096979300','096902203','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('BTB','BINTULU',42,'BINTULU','KPDNHEP BINTULU','Lot 2141, Bintulu Town District,','Jalan Tun Razak,','97000','13','Bintulu','bintulu@kpdnkk.gov.my','086338252','086332176','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('CHE','CHERAS',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('DGN','DUNGUN',32,'KPDNKK Dungun','KPDNHEP Dungun','Lot 7932','Jalan Baru Pak Sabah','23000','11','Dungun','dungun.kpdnkk@1govuc.gov.my','098453575','098453586','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('GBK','GOMBAK',85,'KPDNKK Gombak','KPDNHEP Gombak','Unit D-40-G & 40-1, Blok D, Medan Selayang,','Jalan Medan Selayang 1, Medan Selayang,','68100','10','Batu Caves','nazirah.kpdnkk@1govuc.gov.my','0361863701','0361863653','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('GMG','GUA MUSANG',138,'KPDNKK Gua Musang','KPDNHEP Gua Musang','Cawangan Gua Musang, PT 11803 dan PT 11804','Taman Wawasan','18300','3','Gua Musang','guamusang@kpdnkk.gov.my','099120021','099120022','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('GRK','GERIK',49,'','KPDNHEP Grik',NULL,NULL,NULL,'8',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('HQR','WP PUTRAJAYA',74,'P','Bahagian Penguatkuasa','Aras 2&3/Menara','Kementerian Perdagangan Dalam Negeri, Koperasi dan Kepenggunaan, No. 13, Persiaran Perdana','62623','16','Presint 2, Putrajaya',NULL,NULL,NULL,'1',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('IPH','IPOH',103,'KPDNKK PERAK','KPDNHEP Perak','TINGKAT 1, BLOK A, BANGUNAN PERSEKUTUAN GREENTOWN','JALAN DATO\'' SERI AHMAD SAID','30450','8','IPOH PERAK','perak@kpdnkk.gov.my','052541192','052414611','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('JOH','JOHOR BAHRU',66,'kpdnkkjb','KPDNHEP Johor Bahru','ARAS 17 MENARA ANSAR','JALAN TRUS','80000','1','JOHOR BAHRU','jb@kpdnkk.gov.my','072235252','072272828','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('JRT','JERANTUT',12,'','KPDNHEP Jerantut',NULL,NULL,NULL,'6',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KAJ','KAJANG',70,'','KPDNHEP Kajang',NULL,NULL,NULL,'10',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KBH','KOTA BHARU',13,'MHBN','KPDNHEP Kota Bharu','2030, Lorong Meranti','Jalan Pengkala Chepa','15400','3','Kota Bharu','syedmuzammil@kpdnkk.gov.my','097416100','0176980469','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KCH','KUCHING',86,'KUCHING','KPDNHEP KUCHING','No. 41-51,','Jalan Tun Jugah,','93350','13','Kuching','kuching@kpdnkk.gov.my','082466024','082466012','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KDT','KUDAT',10,'','KPDNHEP Kudat',NULL,NULL,NULL,'12',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KGR','KANGAR',33,'KPDNKK Perlis','KPDNHEP Perlis','LOT 83','JALAN PANGLIMA','01000','9','KANGAR','kangar@kpdnkk.gov.my','049762490','049795000','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KGU','KENINGAU',4,'','KPDNHEP Keningau',NULL,NULL,NULL,'12',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KKB','KUALA KUBU BHARU',69,'','KPDNHEP Kuala Kubu Bharu',NULL,NULL,NULL,'10',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KKL','KOTA KINABALU',11,'KPDNKK NEGERI SABAH','KPDNHEP Kota Kinabalu','ARAS 4 DAN 6, KPKPS','JLN UMS','88400','12','KOTA KINABALU','kk@kpdnkk.gov.my','088484541','088484500','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KKR','KUALA KRAI',14,'KPDNKK','KPDNHEP Kuala Krai','PT 5378','GUCHIL 6 LUAR','18020','3','KUALA KRAI','kkrai@kpdnkk.gov.my','099663624','099602260','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KKS','KUALA KANGSAR',87,'Cawangan Kuala Kangs','KPDNHEP Kuala Kangsar','PT. 5074 ','Jalan Dato Sagor ','33000','8','Kuala Kangsar, PERAK DARUL RID','kualakangsar@kpdnkk.gov.my','057768117','057768068','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KLG','KLUANG',64,'KPDNKK Cawangan Klua','KPDNHEP Kluang','Tingkat 1, Bangunan Persekutuan','Km. 4 Jalan Batu Pahat','86000','1','Kluang','kpdnkkkluang@kpdnkk.gov.my','077730352','077736877','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KLM','KULIM',17,'KPDNKK KULIM','KPDNHEP Kulim',' LOT 104-110 LORONG KENARI 5/1','TAMAN KENARI','09000','2','KULIM','kulim@kpdnkk.gov.my','044915898','044963700','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KMN','KEMAMAN',16,'KPDNKK Kemaman','KPDNHEP Kemaman','Tingkat 3 ,  Bangunan Persekutuan','Jalan Melor','24000','11','Kemaman','kemaman.kpdnkk@1govuc.gov.my','098598188','098502457','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KPH','KUALA PILAH',96,'KPDNKK Kuala Pilah','KPDNHEP Kuala Pilah','No 6&7','Jalan Angkasa Jaya','72000','5','Kuala Pilah',NULL,NULL,'064821052','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KPS','KUBANG PASU',55,'KPDNKK KUBANG PASU','KPDNHEP Kubang Pasu','NO 105, LOT 3519, PUSAT BANDAR BARAT','BANDAR DARULAMAN','06000','2','JITRA','kubangpasu@kpdnkk.gov.my','049184001','049184002','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KPT','KAPIT',39,'KAPIT','KPDNHEP KAPIT','Lot 562,','Jalan Penghulu Nyanggau, Kapit By-Pass,','96800','13','Kapit','kapit@kpdnkk.gov.my','084797664','084799678','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KSL','KUALA SELANGOR',68,'','KPDNHEP Kuala Selangor',NULL,NULL,NULL,'10',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KTG','KOTA TINGGI',60,'','KPDNHEP Kota Tinggi',NULL,NULL,NULL,'1',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KTN','KUANTAN',77,'KPDNKK PAHANG','KPDNHEP NEGERI PAHANG','BLOCK C, KOMPLEKS WISMA BELIA','INDERA MAHKOTA','25200','6','KUANTAN','pahang@kpdnkk.gov.my','095717780','095717777','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('KTR','KUALA TERENGGANU',15,'KPDNKK K.Terengganu','KPDNHEP Kuala Terengganu','Lot 3657, ','Jalan Sultan Sulaiman','20000','11','Kuala Terengganu','terengganu@kpdnkk.gov.my','096244100','096204700','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('LBN','WP LABUAN',72,'KPDNKK WP Labuan','KPDNHEP WP Labuan','15B, Tingkat 15, Blok 4, Kompleks Ujana Kewangan,','Jalan Merdeka','87000','15','WP Labuan',NULL,NULL,'087423152','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('LDT','LAHAD DATU',9,'','KPDNHEP Lahad Datu',NULL,NULL,NULL,'12',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('LKW','LANGKAWI',19,'KPDNKK LANGKAWI','KPDNHEP Langkawi','LOT 120 &122','PERSIARAN BUNGA RAYA','07000','2','LANGKAWI','langkawi@kpdnkk.gov.my','049660791','049660420','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('LMB','LIMBANG',40,'LIMBANG','KPDNHEP LIMBANG','Lot 1987,','Jalan Buang Siol,','98700','13','Limbang','limbang@kpdnkk.gov.my','085217415','085217414','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('MIR','MIRI',56,'MIRI','KPDNHEP MIRI','Level 5 & 6, Wisma Yu Lan,','Jalan Brooke,','98000','13','*Miri','miri@kpdnkk.gov.my','085411862','085412862','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('MKH','MUKAH',132,'MUKAH','KPDNHEP MUKAH','Tingkat 1, Pusat Pentadbiran Baru Mukah,','Lot 722, Blok 68, Mukah Land District,','96400','13','Mukah','mukah@kpdnkk.gov.my','084872239','084872726','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('MLK','MELAKA',59,'KPDNKK Melaka','KPDNHEP Melaka','Aras 6, Wisma Persekutuan,','Jalan MITC, Hang Tuah Jaya','75450','4','Ayer Keroh','melaka@kpdnkk.gov.my','062345811','062345822','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('MNG','MANJUNG',48,'KPDNKK Manjung','KPDNHEP Manjung','Cawangan Manjong Tingkat 2,  ','Bangunan Persekutuan 32040 ','32040','8','MANJONG,','manjung@kpdnkk.gov.my','056871109','056871100','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('MSG','MERSING',63,'','KPDNHEP Mersing',NULL,NULL,NULL,'1',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PAG','PAGOH',65,'PPDNKK Muar','KPDNHEP Muar',NULL,NULL,NULL,'1',NULL,'PPDNKKMuar','111','1112','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PBR','PARIT BUNTAR',89,'Cawangan Parit Bunta','KPDNHEP Parit Buntar ','Jalan Maharaja, ','Pusat Bandar 34200 Parit Buntar, PERAK DARUL RIDZUAN ','34200','8','Parit Buntar','paritbuntar@kpdnkk.gov.my','057174515','057176055','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PDN','PORT DICKSON',57,'','KPDNHEP Port Dickson',NULL,NULL,NULL,'5',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PJY','PETALING JAYA',81,'','KPDNHEP Petaling Jaya',NULL,NULL,NULL,'10',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PKK','PENGKALAN KUBUR',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PKN','PEKAN',134,'KPDNKK PEKAN','KPDNHEP Pekan','Blok C, Kompleks Wisma Belia','Bandar Indera Mahkota','25200','6','Kuantan','pekan@kpdnkk.gov.my','095739904','095732153','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PMS','PASIR MAS',95,'','KPDNHEP Pasir Mas',NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PPG','PULAU PINANG',93,'KPDNKK PULAU PINANG ','KPDNHEP Pulau Pinang','TINGKAT 9, BANGUNAN TUANKU SYED PUTRA, ','LEBUH DOWNING, ','10300','7','GEORGETOWN ','penang@kpdnkk.gov.my','042552525','042552555','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('PTN','PONTIAN',83,'KPDNKK Cawangan Pont','KPDNHEP Pontian','No. 39, Tingkat Bawah dan Satu','Jalan Delima 3, Pusat Perdagangan Pontian','82000','1','Pontian','kpdnkkpontian@kpdnkk.gov.my','078826808','078826800','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('RPN','ROMPIN',105,'','KPDNHEP Rompin',NULL,NULL,NULL,'6',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('RUB','RAUB',90,'','KPDNHEP Raub',NULL,NULL,NULL,'6',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SAL','SHAH ALAM',71,'KPDNKK Shah Alam','KPDNHEP Shah Alam','Tingkat 15-17, Menara MRCB','Jalan Majlis 14/10, Seksyen 14','40622','10','Shah Alam','shahalam@kpdnkk.gov.my','0355195255','0355107426','2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SBN','SEREMBAN',3,'','KPDNHEP Seremban',NULL,NULL,NULL,'5',NULL,NULL,NULL,NULL,'2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SBU','SIBU',45,'SIBU','KPDNHEP SIBU','Tingkat 1, Wisma Persekutuan Blok 3,','Persiaran Brooke,','96000','13','*Sibu','sibu@kpdnkk.gov.my','084340212','084329202','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SDN','SANDAKAN',7,'','KPDNHEP Sandakan',NULL,NULL,NULL,'12',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SGP','SUNGAI PETANI',21,'KPDNKK SUNGAI PETANI','KPDNHEP Sungai Petani','Tingkat 2, Wisma Ria','Taman Ria','08000','2','Sungai Petani','sgpetani.kpdnkk@1govuc.gov.my','044217995','044205108','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SGT','SEGAMAT',61,'KPDNKK SEGAMAT','KPDNHEP Segamat','NO.30 & 32, ','JALAN PUTRA 2/23, BANDAR PUTRA','85000','1','SEGAMAT','kpdnkksegamat@kpdnkk.gov.my','079433898','079433810','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SPG','SEPANG',67,'','KPDNHEP Sepang',NULL,NULL,NULL,'10',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SPN','SEMPORNA',22,'KPDNKK Semporna','KPDNHEP Semporna','Tingkat Bawah, Lot 37 & 38, Blok F','Bandar Baru Semporna','91308','12','Semporna','ppdnkk.spna@kpdnkk.gov.my','089782822','089782855','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SPS','SEBERANG PERAI SELATAN',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SPT','SEBERANG PERAI TENGAH',46,'KPDNKK Caw. SPT','KPDNHEP Seberang Perai Tengah','Unit 5, Tingkat 1, Kompleks Sempilai Jaya','Jalan Sempilai','13700','7','Seberang Perai Tengah','spt@kpdnkk.gov.my','043995517','043801700','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SPU','SEBERANG PERAI UTARA',80,'','KPDNHEP Seberang Perai Utara',NULL,NULL,NULL,'7',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SRA','SRI AMAN',37,'SRI AMAN','KPDNHEP SRI AMAN','Blok II & III, Bangunan Persekutuan Gunasama,','Jalan Kejatau, P.O. Box 425,','95000','13','Sri Aman','sriaman@kpdnkk.gov.my','083323150','083323836','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('SRK','SARIKEI',41,'SARIKEI','KPDNHEP SARIKEI','23A & 25,','Jalan Jubli Mutiara,','96100','13','Sarikei','sarikei@kpdnkk.gov.my','084654415','084657751','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('TIN','TELUK INTAN',47,'','KPDNHEP Teluk Intan',NULL,NULL,NULL,'8',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('TLH','TEMERLOH',58,'','KPDNHEP Temerloh',NULL,NULL,NULL,'6',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('TMR','TANAH MERAH',108,'','KPDNHEP Tanah Merah',NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('TPG','TAIPING',51,'','KPDNHEP Taiping',NULL,NULL,NULL,'8',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('TPH','TAPAH',88,'','KPDNHEP Tapah',NULL,NULL,NULL,'8',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('TRN','TUARAN',5,'','KPDNHEP Tuaran',NULL,NULL,NULL,'12',NULL,NULL,NULL,NULL,'3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('TWU','TAWAU',8,'KPDNKK Tawau','KPDNHEP Tawau','Tingkat 1, Wisma Persekutuan Tawau','Jalan Dunlop-Sabindo','91000','12','Tawau',NULL,NULL,'0897776690','3',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('WKL','WPKL',73,'','KPDNHEP WP Kuala Lumpur',NULL,NULL,NULL,'14',NULL,NULL,NULL,NULL,'2',NULL,NULL,NULL,NULL,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";
                    result = DataAccessQuery<TbCawangan>.ExecuteSql(script);

                    if (result == Constants.Error) return false;
                }

                return true;
            }
            return false;
        }

        public static bool CreateTbDataKes()
        {
            string sQuery = "SELECT * FROM tbdatakes";
            var result = DataAccessQuery<TbDataKes>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbDataKes>.CreateTable();

                return true;
            }
            return false;
        }

        public static bool CreateTbHandheld()
        {
            string sQuery = "SELECT * FROM tbhandheld";
            var result = DataAccessQuery<TbHandheld>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbHandheld>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbhandheld` VALUES ('WKL001','WKL','WKL001','PT001','1.0',0,0,0,0,0,0,0,0,0,0,0,NULL,NULL,'1','admin','2019-09-12 15:05:00','admin','2019-09-12 15:05:00'),('WKL002','WKL','WKL002',NULL,'1.0',0,0,0,0,0,0,0,0,0,0,0,NULL,NULL,'1','admin','2019-09-12 15:05:00','admin','2019-09-12 15:05:00'),('WKL003','WKL','WKL003',NULL,'1.0',0,0,0,0,0,0,0,0,0,0,0,NULL,NULL,'1','admin','2019-09-12 15:05:00','admin','2019-09-12 15:05:00'),('WKL004','WKL','WKL004',NULL,'1.0',0,0,0,0,0,0,0,0,0,0,0,NULL,NULL,'1','admin','2019-09-12 15:05:00','admin','2019-09-12 15:05:00'),('WKL005','WKL','WKL005',NULL,'1.0',0,0,0,0,0,0,0,0,0,0,0,NULL,NULL,'1','admin','2019-09-12 15:05:00','admin','2019-09-12 15:05:00');";
                    result = DataAccessQuery<TbHandheld>.ExecuteSql(script);

                    if (result == Constants.Error) return false;
                }

                return true;
            }
            return false;
        }

        public static bool CreateTbJenisPerniagaan()
        {
            string sQuery = "SELECT * FROM tbjenisperniagaan";
            var result = DataAccessQuery<TbJenisPerniagaan>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbJenisPerniagaan>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbjenisperniagaan` VALUES (1,'AKSESORI KENDERAAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(2,'BAZAR RAMADHAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(3,'BENGKEL KENDERAAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(4,'BENGKEL TIMBANG SUKAT','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(5,'BORONG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(6,'BOT','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(7,'CYBER CAFE','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(8,'GERAI CD/DVD KAKI LIMA','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(9,'GERAI MAKAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(10,'GUDANG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(11,'HARDWARE','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(12,'HIGHWAY','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(13,'HYPERMARKET','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(14,'INSTITUSI KEWANGAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(15,'KAPAL','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(16,'KAWASAN TERBUKA','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(17,'KEDAI ALAT GANTI KENDERAAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(18,'KEDAI CD/DVD','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(19,'KEDAI ELEKTRIK','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(20,'KEDAI EMAS','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(21,'KEDAI KEK DAN ROTI','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(22,'KEDAI KOMPUTER','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(23,'KEDAI MAKAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(24,'KEDAI MENJUAL KERETA','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(25,'KEDAI MENJUAL MOTOR','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(26,'KEDAI PAKAIAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(27,'KEDAI PERABOT','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(28,'KEDAI RUNCIT','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(29,'KEDAI SERBANIKA','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(30,'KEDAI TELEKOMUNIKASI','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(31,'KENDERAAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(32,'KERETA TERPAKAI','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(33,'KILANG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(34,'KIOSK RUANG LEGAR KOMPLEKS MEMBELI BELAH','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(35,'KUARI','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(36,'LADANG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(37,'MEDAN SELERA','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(38,'MINI MARKET','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(39,'PAJAK GADAI','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(40,'PASAR BASAH','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(41,'PASAR MALAM/TAMU','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(42,'PASAR TANI','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(43,'PEJABAT MCM','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(44,'PEMBUNGKUS','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(45,'PENGILANG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(46,'PENGILANG CAKERA OPTIK','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(47,'PENGIMPORT','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(48,'RESTORAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(49,'RNR','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(50,'RUMAH','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(51,'STESEN MINYAK','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(52,'STOKIS JUALAN LANGSUNG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(53,'STOR','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(54,'STOR CAKERA OPTIK','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(55,'SUPERMARKET','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(56,'SYARIKAT JUALAN LANGSUNG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(57,'SYARIKAT PEMBERI KREDIT','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(58,'SYARIKAT PERNIAGAAN ATAS TALIAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(59,'SYARIKAT TELCO','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(60,'LAIN-LAIN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";

                    result = DataAccessQuery<TbKategoriKawasan>.ExecuteSql(script);

                    if (result == Constants.Error) return false;
                }

                return true;
            }
            return false;
        }

        public static bool CreateTbKategoriKawasan()
        {
            string sQuery = "SELECT * FROM tbkategorikawasan";
            var result = DataAccessQuery<TbKategoriKawasan>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbKategoriKawasan>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbkategorikawasan` VALUES ('B','BANDAR','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('LB','LUAR BANDAR','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";
                    result = DataAccessQuery<TbKategoriKawasan>.ExecuteSql(script);

                    if (result == Constants.Error) return false;
                }

                return true;
            }
            return false;
        }

        public static bool CreateTbKategoriPerniagaan()
        {
            string sQuery = "SELECT * FROM tbkategoriperniagaan";
            var result = DataAccessQuery<TbKategoriPerniagaan>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbKategoriPerniagaan>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbkategoriperniagaan` VALUES (1,'BORONG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(2,'RUNCIT','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(3,'PENGILANG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";
                    result = DataAccessQuery<TbKategoriPerniagaan>.ExecuteSql(script);

                    if (result == Constants.Error) return false;
                }

                return true;
            }
            return false;
        }

        public static bool CreateTbKategoriPremis()
        {
            string sQuery = "SELECT * FROM tbkategoripremis";
            var result = DataAccessQuery<TbKategoriPremis>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbKategoriPremis>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbkategoripremis` VALUES (1,'PREMIS TERBUKA','1','AIMFORCE','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(2,'PREMIS TETAP','1','AIMFORCE','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";
                    result = DataAccessQuery<TbKategoriPremis>.ExecuteSql(script);

                    if (result == Constants.Error) return false;
                }

                return true;
            }
            return false;
        }

        public static bool CreateTbKesalahan()
        {
            string sQuery = "SELECT * FROM tbkesalahan";
            var result = DataAccessQuery<TbKesalahan>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbKesalahan>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbkesalahan` VALUES ('APD11',1,'Peraturan 3 (6)','Pelanggaran Peraturan 3 (1) iaitu gagal mengemukakan notis secara bertulis kepada Pengawal. ','Y',200.00,'DUA RATUS',300.00,'TIGA RATUS',14,1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD12',1,'Peraturan 3 (2)','Pelanggaran Peraturan 3 (2) iaitu meminda notis tanpa pemberitahuan secara bertulis kepada Pengawal. ','Y',200.00,'DUA RATUS',300.00,'TIGA RATUS',14,1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD13',1,'Peraturan 3 (5)','Pelanggaran Peraturan 3 (5) iaitu menarik balik notis tanpa pemberitahuan secara bertulis kepada Pengawal. ','Y',200.00,'DUA RATUS',300.00,'TIGA RATUS',14,1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD13',2,'Kesalahan di bawah Peraturan 15 (4) ','Pelanggaran Peraturan 8 iaitu gagal menunjukkan harga sebelum dan selepas harga jualan murah,','Y',200.00,'DUA RATUS',300.00,'TIGA RATUS',14,1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD13',3,'Kesalahan di bawah Peraturan 15 (4) ','Pelanggaran Peraturan12 A iaitu gagal mempamerkan notis,','Y',200.00,'DUA RATUS',300.00,'TIGA RATUS',14,1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD13',4,'Kesalahan di bawah Peraturan 15 (4) ','Peraturan 13 (1) iaitu gagal menyatakan no. pendaftaran jualan murah dan jangka masa jualan itu diadakan, ','Y',200.00,'DUA RATUS',300.00,'TIGA RATUS',14,1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD13',5,'Kesalahan di bawah Peraturan 15 (4) ','Pelanggaran  Peraturan 13 (2) iaitu mengiklankan jumlah pengurangan harga pada harga jualan murah,','Y',200.00,'DUA RATUS',300.00,'TIGA RATUS',14,1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";
                    result = DataAccessQuery<TbKesalahan>.ExecuteSql(script);

                    if (result == Constants.Error) return false;
                }

                return true;
            }
            return false;
        }

        public static bool CreateTbKesalahanButir()
        {
            string sQuery = "SELECT * FROM tbkesalahan_butir";
            var result = DataAccessQuery<TbKesalahanButir>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbKesalahanButir>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbkesalahan_butir` VALUES ('APD11',1,1,'gagal mengemukakan notis secara bertulis kepada Pengawal ','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('APD11',1,2,'gagal membuat pembetulan kerja bedasarkan notis yang dikeluarkan oleh Pengawal','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";
                    result = DataAccessQuery<TbKesalahanButir>.ExecuteSql(script);

                    if (result == Constants.Error) return false;
                }

                return true;
            }
            return false;
        }

        public static bool CreateTbNegeri()
        {
            string sQuery = "SELECT * FROM tbnegeri";
            var result = DataAccessQuery<TbNegeri>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbNegeri>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbnegeri` VALUES ('01','JOHOR D. T.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('02','KEDAH D. A.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('03','KELANTAN D. N.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('04','MELAKA','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('05','N. SEMBILAN D. K.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('06','PAHANG D. M.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('07','PULAU_PINANG','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('08','PERAK D. R.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('09','PERLIS I. K.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('10','SELANGOR D. E.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('11','TERENGGANU D. I.','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('12','SABAH','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('13','SARAWAK','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('14','W. P. KUALA LUMPUR','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('15','W. P. LABUAN','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('16','W. P. PUTRAJAYA','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";

                    result = DataAccessQuery<TbNegeri>.ExecuteSql(script);
                    if (result == Constants.Error) return false;
                }
                return true;
            }
            return false;
        }

        public static bool CreateTbKompaun()
        {
            string sQuery = "SELECT * FROM tbkompaun";
            var result = DataAccessQuery<TbKompaun>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbKompaun>.CreateTable();

                return true;
            }
            return false;
        }

        public static bool CreateTbPasukanHh()
        {
            string sQuery = "SELECT * FROM tbpasukan_hh";
            var result = DataAccessQuery<TbPasukanHh>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbPasukanHh>.CreateTable();
                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbpasukan_hh` VALUES ('WKL001',1,2,'830712015810','WKL',2,4,'','1','admin','2019-07-04 00:00:00','admin','2019-07-04 00:00:00')," +
                                                          "('WKL001',1,2,'850331015815','WKL',2,2,'','1','admin','2019-07-04 00:00:00','admin','2019-07-04 00:00:00')," +
                                                          "('WKL001',1,2,'890824085105','WKL',1,1,'','1','admin','2019-07-04 00:00:00','admin','2019-07-04 00:00:00')," +
                                                          "('WKL001',1,2,'900411126075','WKL',2,3,'','1','admin','2019-07-04 00:00:00','admin','2019-07-04 00:00:00');";

                    result = DataAccessQuery<TbPasukanHh>.ExecuteSql(script);
                    if (result == Constants.Error) return false;
                }
                return true;
            }
            return false;
        }

        public static bool CreateTbPengguna()
        {
            string sQuery = "SELECT * FROM tbpengguna";
            var result = DataAccessQuery<TbPengguna>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbPengguna>.CreateTable();
                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbpengguna` VALUES ('660309107415',1118,NULL,'Mohd.Fuzi Hadi bin Abd Latif',NULL,NULL,NULL,NULL,'Pegawai Penguatkuasa','KP48',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2',74,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'1',3,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('720117025427',5100,'SAL','Ahmad Yusoff bin Ramli','L','KPDNHEP Shah Alam','En.','Penolong Pegawai Penguatkuasa','Penolong Pegawai Penguatkuasa','KP32',90,'0355107426','0194725665','ahmadyusoff@kpdnhep.gov.my',21,NULL,'c07007b451ae53e3f7ff9f9346d9cf37','1',71,'3','9','2','10','No, 30,  Jalan Putra Permai 8D,','43300','10','1972-01-17','Seri Kembangan',NULL,NULL,NULL,'2',3,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('721102015554',164,'HQR','Nurul Izzati Binti Sahar','P','Bahagian Penguatkuasa','Pn.','Penolong Pegawai Penguatkuasa','Penolong Pegawai Penguatkuasa','N29',3,'0388825698','0197321777','nurulizzati@kpdnhep.gov.my',295,'','009042f302550574e8f825bbb02e68ee','1',74,'3','12','2','16','No.10D-02-03,Blok D, Fasa 10 Jalan P9C, Presint 9','62250','16','1972-11-02','Putrajaya',NULL,NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('760118016115',230,'HQR','Janendran A/l Sugumar','L','Bahagian Penguatkuasa','En.','Pegawai Penguatkuasa','Pegawai Penguatkuasa','N44',13,'0388826125','0173954595','janendran@kpdnhep.gov.my',111,'','32250170a0dca92d53ec9624f336ca24','1',74,'2','12',NULL,'16','adf asdfasdfa','asdfa','16','1976-01-18','asdfadsf',NULL,NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('770102135055',2693,'HQR','Saifulbahri Bin Abdul Kadir','L','Bahagian Penguatkuasa','En.','Ketua Penolong Pengarah Kanan','Pegawai Penguatkuasa','KP54',96,'0388825386','0132273668','saifulbahari@kpdnhep.gov.my',68,'','bededdfc81d97e2136e87ed871817da0','1',74,'2','9','2','16','No 128 Jalan 8A Taman D\''Mawar Residensi Bandar Baru Salak','43900','10','1977-01-02','Sepang',NULL,NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('770925115135',319,'HQR','Aminuddin Bin Muhammad','L','Bahagian Penguatkuasa','En.','Ketua Penolong Pengarah','Pegawai Penguatkuasa','KP48',94,'0388826712','0199618199','aminuddin@kpdnhep.gov.my',83,'','8d3fec665cbaf6e6f0558e5f35351e84','1',74,'2','9','2','16','NO 45 JALAN SURIA 3 TAMAN SURIA JALAN BANDAR BARU SUNGAI LONG','43000','10','1977-09-25','CHERAS',NULL,NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('790614104121',1117,NULL,'MohammadHisyam bin Salleh',NULL,NULL,NULL,NULL,'Pembantu Penguatkuasa',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,74,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'PmP','1',3,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('800503148129',1116,NULL,'Mohd Salmizi bin Mat Nor',NULL,NULL,NULL,NULL,'Pembantu Penguatkuasa',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,74,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'PmP','1',3,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('800704065123',1114,NULL,'Azlan bin Ag Talip',NULL,NULL,NULL,NULL,'Pembantu Penguatkuasa',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,74,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'PmP','1',3,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('810312065124',143,'HQR','Nurul Syarina Binti Md Anuar','P','Bahagian Penguatkuasa','Pn.','Ketua Penolong Pengarah','Pegawai Penguatkuasa','KP48',94,'0388826636','0192128056','syrina@kpdnhep.gov.my',84,'','b2c453b5e5cc52b7072dc09b2ee0ec4e','1',74,'2','9','2','16','NO. 26, JALAN LEP 7/9, TAMAN LESTARI PUTRA SEKSYEN 7,','43300','10','1981-03-12','SERI KEMBANGAN SELANGOR',NULL,NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('810702143127',1111,NULL,'Syamsur Asraf binMohammad',NULL,NULL,NULL,NULL,'Penolong Pegawai Penguatkuasa',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,74,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'PPP','1',3,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('810815100212',1113,NULL,'Nor Zanita binti Ghazali',NULL,NULL,NULL,NULL,'Penolong Pegawai Penguatkuasa',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,74,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'PPP','1',3,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('811012103165',1112,NULL,'Syaiful Azli bin Daud',NULL,NULL,NULL,NULL,'Penolong Pegawai Penguatkuasa',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,74,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'PPP','1',3,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('830712015810',2398,'WKL','Nur Syuhaida Binti Mat Kasri','P','KPDNHEP WP Kuala Lumpur','Pn.','Pembantu Penguatkuasa','Pembantu Penguatkuasa','KP19',85,'0361863653','0133331127','syuhaida@kpdnhep.gov.my',17,'','02844f2b968a8159bc4f2d67819b3ee3','1',73,'4','9','2','14','3A-08-05, DESAKU 3 KONDOMINIUM, JALAN MELATI INDAH 2 OFF KEMENSAH HEIGHTS,','53100','10','1983-07-12','KUALA LUMPUR',NULL,NULL,'PP','2',3,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('830724035085',117,'HQR','Mohd Muhaimi Bin Mohamad','L','Bahagian Penguatkuasa','En.','Penolong Pegawai Penguatkuasa','Penolong Pegawai Penguatkuasa','KP32',90,'0388826267','0124567196','muhaimi@kpdnhep.gov.my',266,'','dfc49e4b3d724a8da72524911db75fb2','1',74,'3','9','2','16','NO. 46 JALAN WARISAN BESTRAI 4 TAMAN WARISAN BESTARI','43800','10','1983-07-24','DENGKIL',NULL,NULL,NULL,'1',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('840911104873',1115,NULL,'Mohammad Syahrul Nizam b. Naim Az\''zaim',NULL,NULL,NULL,NULL,'Pembantu Penguatkuasa',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,74,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'PmP','1',3,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('850331015815',3939,'WKL','MOHD NASARUDDIN BIN MOKUTI','L','KPDNHEP WP Kuala Lumpur','En.','Penolong Pegawai Penguatkuasa','Penolong Pegawai Penguatkuasa','KP29',89,'0340454598','0123280621','mnasaruddin@kpdnhep.gov.my',43,NULL,'a48652e184b3138ce02192dc125943b4','1',73,'3','9','2','14','B-03-08, Apartment Sri Astana, Jalan Sg Tua','68100','10','1985-03-31','Batu Caves','Seksyen Penguatkuasa','Unit Operasi','PPP','2',3,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('850825145644',251,'HQR','Nurul Atiqah Binti Mohd Zukni','P','Bahagian Penguatkuasa','Pn.','Pegawai Penguatkuasa','Pegawai Penguatkuasa','KP41',92,'0388826216','0167165171','nurulatiqah@kpdnhep.gov.my',248,'','d4dbb2c2910b10674346efa2183ced84','1',74,'2','9','2','16','9-4-16 JLN 6/112A, TAMAN ANGKASA,','59200','14','1985-08-25','KUALA LUMPUR',NULL,NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('870427085082',6302,'HQR','Syazwani binti Zawawi','P','Bahagian Undang-Undang','Pn.','Peguam Persekutuan','Pegawai Undang-Undang','L44',70,'0388826967','0176813270','syazwani_z@kpdnhep.gov.my',7,NULL,'9e6fb5c598959f956c81f96052e3e4cd','1',76,'2','10','2','16','No.1, Jalan Melur 1/1 Taman Sri Ramal','43000','10','1987-04-27','Kajang','Seksyen Gubalan',NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('890331016005',6091,'HQR','Tan Chiew King','L','Bahagian Undang-Undang','En.','Timbalan Pendakwa Raya','Pegawai Undang-Undang','L41',30,'0388826929','0176996734','cktan@kpdnhep.gov.my',23,NULL,'52b7163df5051e78b8a40262c8c10d55','1',76,'2','10','2','16','A13A-06 PPA1M','57000','14','1989-03-31','Bukit Jalil','Seksyen Pendakwaan',NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('890824085105',3914,'WKL','Ahmad Abrar bin Ahmad Shahrudin','L','KPDNHEP WP Kuala Lumpur','Tn.','Pegawai Penguatkuasa','Pegawai Penguatkuasa','KP41',92,'0340454797','0175438435','abrar@kpdnhep.gov.my',19,NULL,'0f685e93cee3e62e2d8eccb3c2388c93','1',73,'2','9','2','14','29-09-08, Residensi Sentulmas, Jalan 1/48A, Bandar Baru Sentul','51000','14','1989-08-24','Kuala Lumpur','Harga/Bekalan','Unit Anti Pencatutan','PGP','2',2,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('900411126075',3610,'WKL','Roslan Bin Bade','L','KPDNHEP WP Kuala Lumpur','En.','Penolong Pegawai Penguatkuasa','Penolong Pegawai Penguatkuasa','KP29',89,'0340432105','0168486078','roslan@kpdnhep.gov.my',41,'3610ROSLAN BIN BADE  900411 12 6075 2.jpg','cf1d7f62aeca55ad4662c04ce5623768','1',73,'3','9','2','14','No.37 Jalan P11F/18','62300','16','1990-04-11','Putajaya','SEKSYEN PENGUATKUASA','Unit Harga','PPP','2',3,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('920917065984',4767,'HQR','NURUL ASYIKIN BINTI MOHMAD ASRI','P','Bahagian Penguatkuasa','Cik','Pembantu Penguatkuasa','Pembantu Penguatkuasa','KP19',85,'0388826499','0134251946','nurulasyikin@kpdnhep.gov.my',82,NULL,'8e281bf69f6dfd594eb9d55184776bd3','1',74,'4','9','2','16',NULL,'43650','10','1992-09-17','Bandar Baru Bangi','Naziran dan Tugas Khas','Unit Naziran',NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('960309025686',4870,'HQR','NURUL SYAFAWATI SYAIRAH BINTI YUSOFF ZAKI','P','Bahagian Penguatkuasa','Pn.','Pembantu Penguatkuasa','Pembantu Penguatkuasa','KP19',85,'0388825500','01111111111','syafawati@kpdnhep.gov.my',229,NULL,'43c4ba146efcccad2d8ba038355a1ff9','1',74,'4','9','2','16','ARAS 3 BAHAGIAN PENGUATKUASA PRESINT 2','62623','16','1996-03-09','PUTRAJAYA',NULL,NULL,NULL,'2',1,'1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),('admin',0,'HQR','ADMIN',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,'2','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";

                    result = DataAccessQuery<TbPengguna>.ExecuteSql(script);
                    if (result == Constants.Error) return false;
                }
                return true;
            }
            return false;
        }

        public static bool CreateTbTujuanLawatan()
        {
            string sQuery = "SELECT * FROM tbtujuanlawatan";
            var result = DataAccessQuery<TbTujuanLawatan>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbTujuanLawatan>.CreateTable();
                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbtujuanlawatan` VALUES (1,'Siasatan Aduan','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(2,'Operasi','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(3,'Pemeriksaan Biasa','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(4,'Naziran','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00'),(5,'Lain-lain','1','admin','2019-09-11 00:00:00','admin','2019-09-11 00:00:00');";

                    result = DataAccessQuery<TbTujuanLawatan>.ExecuteSql(script);
                    if (result == Constants.Error) return false;
                }
                return true;
            }
            return false;
        }


        //public static bool CreateTbPasukan()
        //{
        //    var result = DataAccessQuery<TbPasukanHh>.Count(c => c.IdHh != "");
        //    if (result < 0)
        //    {
        //        DataAccessQuery<TbPasukanHh>.CreateTable();
        //        return true;
        //    }
        //    return false;


        //    //var result = DataAccessQuery<TbPasukanHh>.GetAll();
        //    //if (!result.Success)
        //    //{
        //    //    DataAccessQuery<TbPasukanHh>.CreateTable();

        //    //    if (WithDefaultData)
        //    //    {
        //    //        CretaeDefaultPasukan();
        //    //    }


        //    //    return true;
        //    //}
        //    //return true;
        //}

        private static void CretaeDefaultPasukan()
        {
            var data = new TbPasukanHh();
            data.KodPasukan = 2;
            data.NoKp = "790614104121";
            data.JenisPengguna = Enums.JenisPengguna.Ahli;

            //data.Nama = "MohammadHisyam bin Salleh";
            //data.KataLaluan = "1234";
            data.Status = Constants.Status.Aktif;
            data.KodCawangan = "MLK";
            data.PgnDaftar = 0;
            data.TrkhDaftar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
            //data.PgnAkhir = 0;
            //data.TrkhAkhir = DateTime.Now.AddDays(-1).ToString(Constants.DatabaseDateFormat);

            DataAccessQuery<TbPasukanHh>.Insert(data);

            data = new TbPasukanHh();
            data.KodPasukan = 2;
            data.NoKp = "800503148129";
            data.JenisPengguna = Enums.JenisPengguna.Ahli;
            //data.StafUrutan = 6;
            // data.Nama = "Syamsur Asraf binMohammad";
            //data.KataLaluan = "2345";
            data.Status = Constants.Status.Aktif;
            data.PgnDaftar = 0;
            data.TrkhDaftar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
            //data.PgnAkhir = 0;
            //data.TrkhAkhir = DateTime.Now.AddDays(-1).ToString(Constants.DatabaseDateFormat);

            DataAccessQuery<TbPasukanHh>.Insert(data);

            data = new TbPasukanHh();
            data.KodPasukan = 2;
            data.NoKp = "800704065123";
            data.JenisPengguna = Enums.JenisPengguna.Ahli;
            //data.StafUrutan = 4;
            //data.Nama = "Mohd Salmizi bin Mat Nor";
            //data.KataLaluan = "3456";
            data.Status = Constants.Status.Aktif;
            data.PgnDaftar = 0;
            data.TrkhDaftar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
            //data.PgnAkhir = 0;
            //data.TrkhAkhir = DateTime.Now.AddDays(-1).ToString(Constants.DatabaseDateFormat);

            DataAccessQuery<TbPasukanHh>.Insert(data);

            data = new TbPasukanHh();
            data.KodPasukan = 2;
            data.NoKp = "810702143127";
            data.JenisPengguna = Enums.JenisPengguna.Ketua;
            //data.StafUrutan = 5;
            //data.Nama = "Azlan bin Ag Talip";
            //data.KataLaluan = "4567";
            data.Status = Constants.Status.Aktif;
            data.PgnDaftar = 0;
            data.TrkhDaftar = DateTime.Now.ToString(Constants.DatabaseDateFormat);
            //data.PgnAkhir = 0;
            //data.TrkhAkhir = DateTime.Now.AddDays(-1).ToString(Constants.DatabaseDateFormat);

            DataAccessQuery<TbPasukanHh>.Insert(data);
        }

        //public static bool CreateTbKonfigurasi()
        //{
        //    string sQuery = "SELECT * FROM tbkonfigurasi";
        //    var result = DataAccessQuery<TbKonfigurasi>.ExecuteSql(sQuery);
        //    if (result == Constants.Error)
        //    {
        //        DataAccessQuery<TbKonfigurasi>.CreateTable();

        //        return true;
        //    }
        //    return false;
        //}



        public static bool CreateTbKpp()
        {
            string sQuery = "SELECT * FROM tbkpp";
            var result = DataAccessQuery<TbKpp>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbKpp>.CreateTable();

                return true;
            }
            return false;
        }

        public static bool CreateTbPasukanTrans()
        {
            string sQuery = "SELECT * FROM tbpasukan_trans";
            var result = DataAccessQuery<TbPasukanTrans>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbPasukanTrans>.CreateTable();

                return true;
            }
            return false;
        }

        private static bool CreateTbPremis()
        {
            //string sQuery = "SELECT * FROM tbpremis";
            var result = DataAccessQuery<TbPremis>.Count(c => c.NoDaftarPremis != "");
            if (result < 0)
            {
                DataAccessQuery<TbPremis>.CreateTable();

                if (WithDefaultData)
                {
                    var script =
                        "INSERT INTO `tbpremis` VALUES (1,'WKL','Metrology Corporation Malaysia Sdn. Bhd (Cawangan Kuala Lumpur)','No. 3 Jln 33/10A','Kawasan Perindustrian IKS','Mukim Batu 68100 Kuala Lumpur','',1,1,1570717297,1,1570765844),(2,'SAL','Metrology Corporation Malaysia Sdn. Bhd  (Cawangan Shah Alam)','No. 15, Jalan Sejat 8/10','Seksyen 8','40000 Shah Alam, SELANGOR','',1,1,1570765857,1,1570765857),(3,'WKL','Puspakom Wangsa Maju','Jalan Genting Klang','53300 Setapak','Kuala Lumpur','',1,1,1570876354,1,1570876381),(4,'WKL','Puspakom Cheras','No. 1 & 3, Jalan 1/118C','Desa Tun Razak Industrial Park','56000 Cheras, Kuala Lumpur','',1,1,1570876354,1,1570876381);";
                    var resultQuery = DataAccessQuery<TbAkta>.ExecuteSql(script);
                    if (resultQuery == Constants.Error) return false;
                }
                return true;
            }
            return false;
        }

        private static bool CreateTbKompaunIzin()
        {
            var result = DataAccessQuery<TbKompaunIzin>.Count(c => c.NoRujukanKpp != "");
            if (result < 0)
            {
                DataAccessQuery<TbKompaunIzin>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbSendOnlineData()
        {
            var result = DataAccessQuery<TbSendOnlineData>.Count(c => c.NoRujukan != "");
            if (result < 0)
            {
                DataAccessQuery<TbSendOnlineData>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbSendOnlineGambar()
        {

            var result = DataAccessQuery<TbSendOnlineGambar>.Count(c => c.NoRujukan != "");
            if (result < 0)
            {
                DataAccessQuery<TbSendOnlineGambar>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbGpsLog()
        {
            var result = DataAccessQuery<TbGpsLog>.Count(c => c.IdHh != "");
            if (result < 0)
            {
                DataAccessQuery<TbGpsLog>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool AlterTbPasukanTrans()
        {
            var query = "SELECT Catatan FROM tbpasukan_trans";
            var result = DataAccessQuery<TbPasukanTrans>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbpasukan_trans ADD COLUMN Catatan varchar(250)";
                DataAccessQuery<TbGpsLog>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool AlterTbHandheld()
        {
            var query = "SELECT Year FROM tbhandheld";
            var result = DataAccessQuery<TbHandheld>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbhandheld ADD COLUMN Year int DEFAULT 0";
                var resultExec = DataAccessQuery<TbHandheld>.ExecuteSql(query);

                if (resultExec != Constants.Error)
                {
                    HandheldBll.UpdateHandheldFirst();
                }
                return true;
            }
            return false;
        }

        private static bool AlterTbKompaunIzin()
        {
            var query = "SELECT KodCawangan FROM tbkompaun_izin";
            var result = DataAccessQuery<TbKompaunIzin>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkompaun_izin ADD COLUMN KodCawangan varchar(3)";
                var resultExec = DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool CreateTbKppAsasTindakan()
        {

            var result = DataAccessQuery<TbKppAsasTindakan>.Count(c => c.NoRujukanKpp != "");
            if (result < 0)
            {
                DataAccessQuery<TbKppAsasTindakan>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool AlterTbKpp()
        {
            var query = "SELECT JenisPesalah FROM tbkpp";
            var result = DataAccessQuery<TbKpp>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkpp ADD COLUMN JenisPesalah varchar(1)";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN KodAkta varchar(10)";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN KodSalah int";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN ButirSalah varchar(1000)";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN IsArahanSemasa varchar(1)";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN TempohTawaran int";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN AmnKmp float";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool AlterTbKpp2()
        {
            var query = "SELECT ip_identiti_pelanggan_id FROM tbkpp";
            var result = DataAccessQuery<TbKpp>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkpp ADD COLUMN ip_identiti_pelanggan_id int";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN poskod varchar(5)";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN bandarpenerima varchar(35)";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN negeripenerima varchar(2)";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN notelpenerima varchar(15)";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN emelpenerima varchar(100)";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN negarapenerima varchar(2)";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool AlterKodKatPerniagaanOnTbKpp()
        {
            var query = "SELECT kodkategoriperniagaan FROM tbkpp";
            var result = DataAccessQuery<TbKpp>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkpp ADD COLUMN kodkategoriperniagaan int";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool AlterTbKompaun()
        {
            var query = "SELECT ip_identiti_pelanggan_id FROM tbkompaun";
            var result = DataAccessQuery<TbKompaun>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkompaun ADD COLUMN ip_identiti_pelanggan_id int";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN poskodpenerima varchar(5)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN bandarpenerima varchar(35)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN negeripenerima varchar(2)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN notelpenerima varchar(15)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN emelpenerima varchar(100)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN negarapenerima varchar(2)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN poskodpenerima_akuan varchar(5)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN bandarpenerima_akuan varchar(35)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN negeripenerima_akuan varchar(2)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN notelpenerima_akuan varchar(15)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN emelpenerima_akuan varchar(100)";
                DataAccessQuery<TbKompaun   >.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN negarapenerima_akuan varchar(2)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN ip_identiti_pelanggan_id_akuan varchar";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN isbayarmanual int";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN gambarbuktibayaran varchar";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool AlterTbKppSiasatUlangan()
        {
            var query = "SELECT NoEp FROM tbkpp";
            var result = DataAccessQuery<TbKpp>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkpp ADD COLUMN NoEp varchar(30)";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                query = "ALTER TABLE tbkpp ADD COLUMN NoIp varchar(30)";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);


                return true;
            }
            return false;
        }

        private static bool CreateTbJenama()
        {

            var result = DataAccessQuery<TbJenama>.Count(c => c.Prgn != "");
            if (result < 0)
            {
                DataAccessQuery<TbJenama>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool AlterTbDataKes()
        {
            var query = "SELECT KodKatPerniagaan FROM tbdatakes";
            var result = DataAccessQuery<TbDataKes>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbdatakes ADD COLUMN KodKatPerniagaan int";
                DataAccessQuery<TbDataKes>.ExecuteSql(query);

                query = "ALTER TABLE tbdatakes ADD COLUMN KodJenama int";
                DataAccessQuery<TbDataKes>.ExecuteSql(query);


                return true;
            }
            return false;
        }

        private static bool AlterTbKppSkipIzin()
        {
            var query = "SELECT IsSkipIzin FROM tbkpp";
            var result = DataAccessQuery<TbKpp>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkpp ADD COLUMN IsSkipIzin int";
                DataAccessQuery<TbKpp>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool CreateTbSkipControl()
        {

            var result = DataAccessQuery<TbSkipControl>.Count(c => c.IsSkip == 0);
            if (result < 0)
            {
                DataAccessQuery<TbSkipControl>.CreateTable();
                return true;
            }
            return false;
        }

        //private static bool AlterTbKppNegeriBandar()
        //{
        //    var query = "SELECT KodNegeri FROM tbkpp";
        //    var result = DataAccessQuery<TbKpp>.ExecuteCheckSelectSql(query);

        //    if (!result)
        //    {
        //        query = "ALTER TABLE tbkpp ADD COLUMN KodNegeri varchar(2)";
        //        DataAccessQuery<TbKpp>.ExecuteSql(query);

        //        query = "ALTER TABLE tbkpp ADD COLUMN KodBandar int";
        //        DataAccessQuery<TbKpp>.ExecuteSql(query);

        //        return true;
        //    }
        //    return true;
        //}

        private static bool CreateTbBandar()
        {
            var result = DataAccessQuery<TbBandar>.Count(c => c.KodNegeri != "");
            if (result < 0)
            {
                DataAccessQuery<TbBandar>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool AlterTbKompaunNoEpIp()
        {
            var query = "SELECT NoEp FROM tbkompaun";
            var result = DataAccessQuery<TbKompaun>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkompaun ADD COLUMN NoEp varchar(30)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbkompaun ADD COLUMN NoIp varchar(30)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                return true;
            }
            return false;
        }


        private static bool AlterTbDataKesNewStructure()
        {
            var query = "SELECT NoKmp FROM tbdatakes";
            var result = DataAccessQuery<TbDataKes>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbdatakes ADD COLUMN NoKmp varchar(20)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbdatakes ADD COLUMN KodKatKawasan varchar(2)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbdatakes ADD COLUMN KodTujuan int";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbdatakes ADD COLUMN NoLaporanPolis varchar(50)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbdatakes ADD COLUMN KelasKes varchar(45)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbdatakes ADD COLUMN KodStatusKes varchar(2)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                query = "ALTER TABLE tbdatakes ADD COLUMN KodStatusKes_Det varchar(6)";
                DataAccessQuery<TbKompaun>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool CreateTbDataKesKesalahan()
        {

            var result = DataAccessQuery<TbDataKesKesalahan>.Count(c => c.NoKes != "");
            if (result < 0)
            {
                DataAccessQuery<TbDataKesKesalahan>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbDataKesAsasTindakan()
        {

            var result = DataAccessQuery<TbDataKesAsasTindakan>.Count(c => c.NoKes != "");
            if (result < 0)
            {
                DataAccessQuery<TbDataKesAsasTindakan>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbDataKesPesalah()
        {

            var result = DataAccessQuery<TbDataKesPesalah>.Count(c => c.NoKes != "");
            if (result < 0)
            {
                DataAccessQuery<TbDataKesPesalah>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool AlterTbKesalahan()
        {
            var query = "SELECT KOTS FROM tbkesalahan";
            var result = DataAccessQuery<TbKesalahan>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkesalahan ADD COLUMN KOTS int";
                DataAccessQuery<TbKesalahan>.ExecuteSql(query);

                return true;
            }
            return false;
        }


        private static bool AlterTbKompaunIzinCatatan()
        {
            var query = "SELECT Catatan FROM tbkompaun_izin";
            var result = DataAccessQuery<TbKompaunIzin>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkompaun_izin ADD COLUMN Catatan varchar(250)";
                DataAccessQuery<TbKompaunIzin>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        public static bool TestBeginTrx()
        {
            var insAccess = new DataAccessQueryTrx();
            if (!insAccess.BeginTrx()) return false;

            if (!insAccess.ExecuteQueryTrx("INSERT INTO tbskipcontrol (IsSkip) VALUES (16)"))
            {
                insAccess.RollBackTrx();
            }
            return false;
            var skipControl = new TbSkipControl();
            skipControl.IsSkip = 16;
            if (!insAccess.InsertTrx(skipControl))
            {
                insAccess.RollBackTrx();
            }

            var tbTujuan = new TbTujuanLawatan();
            tbTujuan.KodTujuan = 11;
            tbTujuan.Prgn = "TTTssssss";
            if (!insAccess.InsertTrx(tbTujuan))
            {
                insAccess.RollBackTrx();
            }

            insAccess.RollBackTrx();

            //if (!insAccess.ExecuteQueryTrx("INSERT INTO tbskipcontrol (IsSkip) VALUES (2)"))
            //{
            //    insAccess.RollBackTrx();
            //}
            //if (!insAccess.ExecuteQueryTrx("INSERT INTO tbskipcontrol (IsSkip) VALUES (3)"))
            //{
            //    insAccess.RollBackTrx();
            //}

            //insAccess.CommitTrx();

            return true;
        }

        private static bool CreateTbPenggunaTemp()
        {
            var result = DataAccessQuery<TbPenggunaTemp>.Count(c => c.NoKp != "");
            if (result < 0)
            {
                DataAccessQuery<TbPenggunaTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbCawanganTemp()
        {
            var result = DataAccessQuery<TbCawanganTemp>.Count(c => c.KodCawangan != "");
            if (result < 0)
            {
                DataAccessQuery<TbCawanganTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbAsasTindakanTemp()
        {
            var result = DataAccessQuery<TbAsasTindakanTemp>.Count(c => c.KodAsas != 0);
            if (result < 0)
            {
                DataAccessQuery<TbAsasTindakanTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbJenisPerniagaanTemp()
        {
            var result = DataAccessQuery<TbJenisPerniagaanTemp>.Count(c => c.KodJenis != 0);
            if (result < 0)
            {
                DataAccessQuery<TbJenisPerniagaanTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbPremisTemp()
        {
            var result = DataAccessQuery<TbPremisTemp>.Count(c => c.IdPremis != 0);
            if (result < 0)
            {
                DataAccessQuery<TbPremisTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbAktaTemp()
        {
            var result = DataAccessQuery<TbAktaTemp>.Count(c => c.KodAkta != "");
            if (result < 0)
            {
                DataAccessQuery<TbAktaTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbKesalahanTemp()
        {
            var result = DataAccessQuery<TbKesalahanTemp>.Count(c => c.KodAkta != "");
            if (result < 0)
            {
                DataAccessQuery<TbKesalahanTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbBandarTemp()
        {
            var result = DataAccessQuery<TbBandarTemp>.Count(c => c.KodNegeri != "");
            if (result < 0)
            {
                DataAccessQuery<TbBandarTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool AlterTbHandheldAddTrkhUpdateDate()
        {
            var query = "SELECT TrkhUpdateDate FROM tbhandheld";
            var result = DataAccessQuery<TbHandheld>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbhandheld ADD COLUMN TrkhUpdateDate";
                DataAccessQuery<TbHandheld>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool CreateTbNegeriTemp()
        {
            var result = DataAccessQuery<TbNegeriTemp>.Count(c => c.KodNegeri != "");
            if (result < 0)
            {
                DataAccessQuery<TbNegeriTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbTujuanLawatanTemp()
        {
            var result = DataAccessQuery<TbTujuanLawatanTemp>.Count(c => c.KodTujuan != 0);
            if (result < 0)
            {
                DataAccessQuery<TbTujuanLawatanTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbKategoryKawasanTemp()
        {
            var result = DataAccessQuery<TbKategoriKawasanTemp>.Count(c => c.Status != 0);
            if (result < 0)
            {
                DataAccessQuery<TbKategoriKawasanTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbKategoryPremisTemp()
        {
            var result = DataAccessQuery<TbKategoriPremisTemp>.Count(c => c.Status != 0);
            if (result < 0)
            {
                DataAccessQuery<TbKategoriPremisTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbKategoryPerniagaanTemp()
        {
            var result = DataAccessQuery<TbKategoriPerniagaanTemp>.Count(c => c.Status != 0);
            if (result < 0)
            {
                DataAccessQuery<TbKategoriPerniagaanTemp>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateTbBarangJenamaTemp()
        {
            var result = DataAccessQuery<TbJenamaTemp>.Count(c => c.Status != 0);
            if (result < 0)
            {
                DataAccessQuery<TbJenamaTemp>.CreateTable();
                return true;
            }
            return false;
        }


        private static bool AlterTbKompaunBarangKompaun()
        {
            var query = "SELECT BarangKompaun FROM tbkompaun";
            var result = DataAccessQuery<TbHandheld>.ExecuteCheckSelectSql(query);

            if (!result)
            {
                query = "ALTER TABLE tbkompaun ADD COLUMN BarangKompaun varchar(300)";
                DataAccessQuery<TbHandheld>.ExecuteSql(query);

                return true;
            }
            return false;
        }

        private static bool CreateTbDataKesPesalahKesalahan()
        {
            //var result = DataAccessQuery<TbDataKesPesalahKesalahan>.Count(c => c.ID != 0);
            //if (result < 0)
            //{
            //    DataAccessQuery<TbDataKesPesalahKesalahan>.CreateTable();
            //    return true;
            //}
            return false;
        }

        private static bool CreateTbIP()
        {
            var result = DataAccessQuery<TbIP>.Count(c => c.Id != 0);
            if (result < 0)
            {
                DataAccessQuery<TbIP>.CreateTable();
                DataAccessQuery<TbIP>.Insert(new TbIP
                {
                    DnsName = "http://iemsstag.kpdnhep.gov.my/",
                    Ip = "http://163.53.152.98/"
                });
                return true;
            }
            return false;
        }

        private static bool DropTbDataKesPesalahKesalahan()
        {
            //var result = DataAccessQuery<TbDataKesPesalahKesalahan>.GetAll();
            //if (result.Success)
            //{
            DataAccessQuery<TbDataKesPesalahKesalahan>.ExecuteSql("drop table if exists tbdatakes_pesalah_kesalahan");
            //}
            return true;
        }

        private static bool CreateTbError()
        {
            var result = DataAccessQuery<TbError>.Count(c => c.Id != 0);
            if (result < 0)
            {
                DataAccessQuery<TbError>.CreateTable();
                return true;
            }
            return false;
        }

        private static bool CreateIpResits() 
        {
            string sQuery = "SELECT * FROM ip_resits";
            var result = DataAccessQuery<ip_resits>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<ip_resits>.CreateTable();
                return true;
            }
            return false;

        }

        private static bool CreateIpChargeline()
        {
            string sQuery = "SELECT * FROM ip_chargelines";
            var result = DataAccessQuery<ip_resits>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<ip_chargelines>.CreateTable();
                return true;
            }
            return false;

        }

        private static bool CreateTbKompaunBayaran()
        {
            string sQuery = "SELECT * FROM tbkompaun_bayaran";
            var result = DataAccessQuery<TbKompaunBayaran>.ExecuteSql(sQuery);
            if (result == Constants.Error)
            {
                DataAccessQuery<TbKompaunBayaran>.CreateTable();
                return true;
            }
            return false;

        }
    }

}