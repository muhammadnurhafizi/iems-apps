# Release Note For iems-apps 

Version: 1.1.4.2
====================================================================================
1. Fix boleh print jika kategori perniagaan tidak diisi
2. Fix download percent bar jadi negative selepas 79%

Version: 1.1.4.1
====================================================================================
1. Fix KPP perlu ambil 2 gambar kepada gambar

Version: 1.1.4.0
====================================================================================
1. Feature for Change Request (CR) 

Version: 1.1.3.9
====================================================================================
1. Fix Printer Problem with new gadget (Samsung A53 5G)
2. Can continue KOTS after crash on SEMAKAN.

Version: 1.1.3.8
====================================================================================
1. Add function and make Connection to Bixolon Printer for latest Samsung A53 Gadget
2. Add function and Connection to MyID Reader Apk 

Version: 1.1.3.7
====================================================================================
1. Change Logo Jata-KPDNHEP to Jata-KPDN
2. Update Domain config file

Version: 1.1.3.6
====================================================================================
1. Add deleteLog() semasa tamat tugas. 
2. reduce thread send tbGpsLog(500) to tbGpsLog(100)

Version: 1.1.3.3
====================================================================================
1. Change status to pasukan from 1 and 2 to Aktif and Tidak Aktif
2. Fix BUG Clean Data Checkout

Version: 1.1.3.2
====================================================================================
1. Change getjpndetailbynoic to getjpndetailbynoiclogwitherr in Akuan, Kompaun, SiasatLanjut
2. Create TbError
3. Make Readable Log when Clean Data

Version: 1.1.3.1
====================================================================================
1. when send the data and sent image, and the image not exist on IMGS folder, it will set status to Send
2. replace \ with \\
3. checkout : delete image only when status is sent and have primari data (kpp, kompaun, etc)
4. show Cleaning Page only for Admin and TbPasukanHh is empty
5. Cleaning Page

Version: 1.1.3.0
====================================================================================
1. Fix problem on Siasat Lanjut
2. Drop table TbDataKesPesalahKesalahan
3. Update KodStatusKes and KodStatusKes_Det Value for Tindakan KOST
4. Send data to API for KOST only

Version: 1.1.2.9
====================================================================================
1. Fix Checkout Function
2. Fix Filter

Version: 1.1.2.8
====================================================================================
1. Change function for send data online (tbkpp_asastindakan, tbdatakes_asastindakan and tbdatakes_kesalahan )
2. Change Connection for SQLite

Version: 1.1.2.7
====================================================================================
1. Show Samakan for Admin
2. Add Resend Button on Samakan Screen
3. Add Table Data kes Pesalah Kesalahan
	add function to save it to server
4. Remove () in Asas Tindakan - Kesalahan Print
5. Fix Search in Samakan Screen
6. Change message

Version: 1.1.2.5
====================================================================================
Version: 1.1.2.6
====================================================================================
1. Comment the code in Kompaun Print 20201221
2. Remove the txtTujuanLawatan 20201229

Version: 1.1.2.5
====================================================================================
1. Remove siggnature 20201218

Version: 1.1.2.4
====================================================================================
1. Disable space for No EP and No IP, and set maxlength 30 - 20201124

Version: 1.1.2.3
====================================================================================
2. Disable No EP and No IP - 20201125

Version: 1.1.2.3
====================================================================================
3. Change JPN URL to getjpndetailbynoiclog - 20201130
====================================================================================
1. clear field when search after Search NoK/P
2. change login logic, remove the function for disable user and password
3. change login logic, et UID and Password for no pasukan only
4. Add parameter for GetListJpnDetail
5. Add filter char only for No EP and No IP
6. Allow admin to login
7. fix pawagaiserbuan 

Version: 1.1.2.2
====================================================================================
1. Fix Loading muat turun data
2. Fix TrkhHhCheckin when call GetSummary
3. Change Check Out to Tamat Tugas
4. Fix problem for tbdatakes_asastindakan when send api
5. Fix tick Senerai data pilihan
6. move the update field field trkhhhcheckin and trkhupdatedate after finish mula tugas (SQLite)
7. update trkhhhcheckin after finish mula tugas (API)
8. send trkhupdatedate when cal getsummary
9. update trkhupdatedate (sqlite and api) after mula proses finish
10. remove trkhupdatedate (api) after mula proses finish
11. change message when printing process
12. change printing process and fix some layout
13. fix take picture and replace after save
14. add validation when data is exits on server
15. fix printing part

Version: 1.1.2.1
====================================================================================
2020-09-01
1. Update kompaun form add new field BarangKompaun(300)
2. add BarangKompaun when send data to API
2. add loading icon on Mula Tugas and Muat Turun Data
3. change timeout (15 second) when call api
4. change message when no have connect to server

Version: 1.1.2.0
====================================================================================
2020-07-20
1. Update message error in CheckRellatedKompaunIzinDataAsync

Version: 1.1.1.9
====================================================================================
2020-07-17
1. Add Button Search JPN in Akuan, Pesalah, Siasat Lanjut
2. Fix Error Tindakan

Version: 1.1.1.8
====================================================================================
2020-07-06
1. Fix Backup function
2. Delete Log file after 2 month
3. Delete Backup folder afte 2 month
4. KPP Tindakan = pemeriksaan make compulsary to take image at least 1 image
5. Fix error on Location Service

Version: 1.1.1.7
====================================================================================
2020-05-26
1. Stop Service when call executestatement

Version: 1.1.1.6
====================================================================================
2020-05-08
1. Update View Kompaun, and View SiasatLanjutan , fix bug show image

Version: 1.1.1.5
====================================================================================
2020-05-06
1. Update pemeriksaan , image is mandatory when Tindakan != Pemeriksaan (in tab Penerima)

Version: 1.1.1.5
====================================================================================
2020-05-06
1. Update pemeriksaan , image is mandatory for only when user choose Tindakan = Pemeriksaan (in tab Penerima)
2. Update length for tbkpp.alamatpremis1, tbkompaun.alamatokk1 , tbdatakes_pesalah.alamatoks1 from 80 to 300 (form)

Version: 1.1.1.4
====================================================================================
2020-05-05
1. Update pemeriksaan , update Carian JPNDetail result for address3

Version: 1.1.1.3
====================================================================================
2020-04-28
1. Update pemeriksaan , add Carian JPNDetail

Version: 1.1.1.2
====================================================================================
2020-04-23
1. Update NoEp and NoIP
2. Create temporary table for (tbNegeri, tbTujuanLawatan, tbKategoryKawasan, tbKategoryPremis, tbKategoryPerniagaan, tbSkipControl, tbbarangJenama)
3. Change screen Muat Turun Page
4. Change All message

Version: 1.1.1.1
====================================================================================
2020-04-17
1. Update pemeriksaan , add Carian SSM

Version: 1.1.1.0
====================================================================================
2020-04-14
1. add new field TrkhUpdateDate in TbHandheld table
2. add IsModified in response for Summary table
3. Update Pemeriksaan, Kompaun dan akuan when print is not exist need sent data online

Version: 1.1.0.9
====================================================================================
2020-04-09
1. Muat Turun Data
	- Change page
	- Call api to get summary total data from server
	- Show data from api
	- Add function call data based on what table selected user, and save it into local DB
2. Update Handheld default value for year (current year)
3. Add Check year when 0 update to current year after user login

Version: 1.1.0.7
====================================================================================
2020-03-23
1. KPP > Tab Lawatan
	- add horizontal scroll bar in Hasil Lawatan field
	- gambar is optional (not compulsary) for only when user choose Tujuan Lawatan = Pemeriksaan Biasa
2. Data Kes > Gambar
	- taking picture is optional (no more 2 pictures compulsary)	
3. Kompaun > Gambar
	- taking picture is optional (no more 2 pictures compulsary)

Version: 1.1.0.6
====================================================================================
2020-03-16
1. Allow admin to open Muat Turun Data page

Version: 1.1.0.5
====================================================================================
2020-02-28
1. Update SiasatLanjut and Kompaun for NamaPremis

Version: 1.1.0.4
====================================================================================
2020-02-27
1. fix problem about progress not working and disable menu when download is loading

Version: 1.1.0.3
====================================================================================
2020-02-26
1. Update SendOnlineBll

Version: 1.1.0.2
====================================================================================
2020-02-26
1. Update GeneralBll Function ProcessRcvData (mr pek)

Version: 1.1.0.1
====================================================================================
2020-02-26
1. Update PrintImageBll and Print Function

Version: 1.1.0.0
====================================================================================
2020-02-25
1. Update TbDataKes, nullable for field PegawaiSerbuan,KodKatPerniagaan and KodJenama
2. Update SendOnlineBll

Version: 1.0.9.9
====================================================================================
2020-02-24
1. refactor code

Version: 1.0.9.8
====================================================================================
2020-02-24
1. Add New Menu (harizal)

Version: 1.0.9.7
====================================================================================
2020-02-14
1. Refactor code

Version: 1.0.9.6
====================================================================================
2020-02-12
1. Refactor code Checkin and LoginBll

Version: 1.0.9.5
====================================================================================
2020-02-12
1. Update Checkin and SendOnlineBll

Version: 1.0.9.4
====================================================================================
2020-02-12
1. Fix delete data KPP on Checkout
2. Add Temporary tables for Akta, AsasTindakan, Bandar, Cawangan, JenisPerniagaan, Kesalahan, Pengguna and Premis
- it will configure on checkin process
3. Add Loading screen in pemeriksaan and view data

Version: 1.0.9.3
====================================================================================
2020-02-11
1. Update Checkout and SendOnlineBll

Version: 1.0.9.2
====================================================================================
2020-02-10
1. Update View SiasatLanjutan Form
2. Update View Pemeriksaan 

Version: 1.0.9.1
====================================================================================
2020-02-07
1. Update Pemeriksaan, kompaun and siasatlanjutan for clear first mycard before assign
2. Update Pemeriksaan for check kompaun izin for Denied message

Version: 1.0.9.0
====================================================================================
2020-02-07
1. Update Check kompaun izin for modal message
2. Update icon save in Pemeriksaan - KOTS

Version: 1.0.8.9
====================================================================================
2020-02-06
1. Update KompaunBll for kompaun trx and akuan
2. Update Message dialog for Check kompaun izin
3. Update icon save in Pemeriksaan - KOTS
4. Update LoginBll

Version: 1.0.8.8
====================================================================================
2020-02-05
1. Update Kompaun Form , for tab pesalah, nama, nama syarikat and nokp 
2. Update Save Transaction , Kpp, Kompaun and SiasatLanjutan
3. Update Pemeriksaan for NoDaftarSyarikat, NoLesenMajlis allowed char
4. Update Screen Show KOTS in pemeriksaan , change icon to save icon
5. Update Check Kompaun izin

Version: 1.0.8.7
====================================================================================
2020-02-04
1. Update HttpClientService for CheckKompaunIzin result
2. Update ViewPemeriksaan
3. For input only Allowed char = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890#$&\\_-?!@()=+':%/\" *,.<>{}[];"

Version: 1.0.8.6
====================================================================================
2020-02-03
1. Update Pemeriksaan , use Transaction when save data.
2. Update TbKesalahan , add new field KOTS(int)
3. Update Pemeriksaan , for tindakan KOTS , get from TbKesalahan where KOTS = 1
4. Update TbKompaunIzin , add field catatan(250). shown when approved and denied

Version: 1.0.8.5
====================================================================================
2020-01-29
1. Remark BackgroundService
2. Update pemeriksaan, jenis perniagaan is mandatory
3. Update Pemeriksaan when tindakan = KOTS , for checkizinkompaun.
   Check all rellated table , that need to send first.

Version: 1.0.8.4
====================================================================================
2020-01-28
1. Remark for allowed char
2. Add some log in backgroundservice
3. LoadingActivity - Move InsertTbPengguna before InsertTbPasukan
4. Checkout - Fix backupImage
5. HttpClientService - add sleep and remove write log for send image

Version: 1.0.8.3
====================================================================================
2020-01-27
1. Update Pemeriksaan , for input text, 
   only allow char = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,\".=/-_!@#&()?:;<>{}[]"

Version: 1.0.8.2
====================================================================================
2020-01-24
1. Update TbHandheld
2. Update SendOnlineBll, Checkin (harizal)

Version: 1.0.8.1
====================================================================================
2020-01-23
1. Update Pemeriksaan for kompaunizin.
2. Update checkout (harizal)

Version: 1.0.8.0
====================================================================================
2020-01-22
1. Update PemeriksaanBll, KompaunBll, for save data, when faild, will rollback tbhandheld
2. Put Log detail for data access insert and update
3. Reduce Image Camera to CustomPhotoSize = 30 , and CompressionQuality = 30
4. Update SendOnlineBll

Version: 1.0.7.9
====================================================================================
2020-01-16
1. Update APK in splash and Checkin
2. Update SendOnline Kompaun and DataKes
3. Update Checkout , Save error script to file 

====================================================================================
Version: 1.0.7.8
====================================================================================
2020-01-16
1. Update Pemeriksaan , for check Tamat condition.
2. Update Print for HasilLawatan to max 20 lines

====================================================================================
Version: 1.0.7.6
====================================================================================
2020-01-13
1. Update SendOnlineBll and CheckOut (harizal)

====================================================================================
Version: 1.0.7.5
====================================================================================
2020-01-13
1. Add Id as PK in table TbDataKesPesalah, TbDataKesKesalahan and TbDataKesAsasTindakan

====================================================================================
Version: 1.0.7.4
====================================================================================
2020-01-11
1. Update ConfigApp.xml default website url to : http://iems.kpdnhep.gov.my/

====================================================================================
Version: 1.0.7.3
====================================================================================
2020-01-10
1. Change default website url to : http://iems.kpdnhep.gov.my/

====================================================================================
Version: 1.0.7.2
====================================================================================
2020-01-10
1. Update SendOnlineBll and CheckOut (harizal)
1. Update password database (zihan)

====================================================================================
Version: 1.0.7.1
====================================================================================
2020-01-08
1. Update TbDataKes when insert data from page and kompaun
2. Update Field length

====================================================================================
Version: 1.0.7.0
====================================================================================
2020-01-07
1. Update TbKompaun add 2 new field NoEP and NoIP
2. Update TbDataKes and add 3 new table for DataKes
3. Update Kompaun when save , will insert into TbDataKes too
4. Update View Kompaun

====================================================================================
Version: 1.0.6.9
====================================================================================
2020-01-03
1. Add Flow Skip Permohonan
2. Add New Field in tbKpp (IsSkipIzin)
3. Add New Table TbSkipControl
4. Update Permohonan Form add 2 field Negeri and Bandar
5. Add New Table TbBandar

====================================================================================
Version: 1.0.6.8
====================================================================================
2019-12-10
1. Update print module, GeneralBll (mr pek)

====================================================================================
Version: 1.0.6.7
====================================================================================
2019-12-09
1. Update print module (mr pek)

====================================================================================
Version: 1.0.6.6
====================================================================================
2019-12-04
1. Update Pemeriksaan , update Button Kesalahan
2. Update Print Kompaun 
3. Update Fix for error print

====================================================================================
Version: 1.0.6.5
====================================================================================
2019-12-02
1. Update Print Pemeriksaan for SiasatUlangan NoEP and NoIP
2. Update Pemeriksaan, button kesalahan only visible for Tindakan = KOTS
3. Update Pemeriksaan, move Amaran ad Jika OKK setuju bayar after Kesalahan and No EP and No IP
4. change Wording Title from Siasatan Lanjut to "Kes Baru", 
	Change "Kes dihasilkan (untuk siasatan)" to "Kes Baru dihasilkan (untuk siasatan)"
5. Add 2 new field TbDataKes, KodKatPerniagaan and KodJenama
6. Update SiasatanLanjut add 2 dropdownlist, KodKatPerniagaan and KodJenama
7. Update View SiasatanLanjut
8. Update print module (mr pek)

====================================================================================
Version: 1.0.6.4
====================================================================================
2019-11-28
1. Update login Admin for menu
2. Update Pemeriksaan, change Tindakan to dropdown and add 1 tindakan = Siasatan Ulangan
3. Update pemeriksaan when siasatulanagn , add 2 new field, NoEp and NoIp
4. Alter TbKpp add 2 new field, NoEp and NoIp
5. Updat ViewPemeriksaan
6. Update Semakan
7. Update sendonline



====================================================================================
Version: 1.0.6.3
====================================================================================
2019-11-27
1. Remove Print for QueryDeviceVersion(mr pek request)

====================================================================================
Version: 1.0.6.2
====================================================================================
2019-11-27
1. Display all dates in screen and printing using 24 hours
2. Update Print Pemeriksaan for asastindakan add "()"
3. Update Admin Login , only can access "Tamat Tugas" menu
4. Update Print for QueryDeviceVersion(mr pek request)

====================================================================================
Version: 1.0.6.1
====================================================================================
2019-11-26
1. Update SendOnline for Kpp Kesalahan

====================================================================================
Version: 1.0.6.0
====================================================================================
2019-11-26
1. Update pemeriksaan , add kesalahan
2. Update TbKpp
3. Update Kompaun

====================================================================================
Version: 1.0.5.9
====================================================================================
2019-11-25
1. add Admin login

====================================================================================
Version: 1.0.5.8
====================================================================================
2019-11-22
1. Update Pemeriksaan , for asatindakan can be multiple and add 1 line more

====================================================================================
Version: 1.0.5.7
====================================================================================
2019-11-22
1. Update Pemeriksaan , for asatindakan can be multiple
2. Add new table tbkpp_asastindakan
3. Update View Pemeriksaan , and print pemeriksaan
4. SendOnline for Kpp AsasTindakan

====================================================================================
Version: 1.0.5.6
====================================================================================
2019-11-20
1. Disable Main Menu after save/print pemeriksaan and enable back when click selesai
2. Update Kompaun and SiasatanLanjut , default empty for akta

====================================================================================
Version: 1.0.5.5
====================================================================================
2019-11-19
1. Update sign image on kompaun
2. Remove time for tarikh at the bottom page. Only remain date
3. Add 1space line and info of no telefon and fax below alamat3 at the top
4. Change caption "No K/P" to "No K/P atau Passport" on Pemeriksaan print
5. if user click on KOTS, automatically tick the Checkbox "Jika OKK setuju bayar".
6. in Akuan screen, disable kembali button by default. Only enable if cetakan has been made

====================================================================================
Version: 1.0.5.4
====================================================================================
2019-11-18
1. Update Kompaun print , add signature

====================================================================================
Version: 1.0.5.3
====================================================================================
2019-11-18
1. Update Pemeriksaan Print,Remove 1 empty line before alamat
2. If user click Jika okk setuju bayar, apps need to automatically tick the cetak akuan penerima as below
3. Disable Amnbyr in screen Akaun and value take from tbkompaun. Amnkmp.
4.Printing kompaun for No K/P and pengeluar 1. Don't print anything if value get from db is empty

====================================================================================
Version: 1.0.5.2
====================================================================================
2019-11-17
1. Update Pemeriksaan Print
2. Update Kompaun and Akuan Print

====================================================================================
Version: 1.0.5.1
====================================================================================
2019-11-17
1. It supposed to be tindakan = tiada kes/Pemeriksaan and checkbox ticked. Just like in the screen
2. Printing must standard in cutting line. Put extra line as before so that user no need to press feed to feed up the paper before tear
3. Pemeriksaan,  am waktu should be Upper. AM
4. Printing for kompaun, please make value to UPPERCASE FOR am to AM, pm to PM and cawangan valUR
5. Please make uppercase for value cawangan and pegawai penyiasa
6. kpp + siasatan lanjut, data berjaya hantar but keep on error with message "msg gagal hantar,
7. Add singkatan_jawatan in printing akuan pegawai penyiasat before nama
8. Add singkatan_jawatan in printing kompaun pegawai penyiasat before nama
9. This message only appear if izin kompaun diberikan. So kompaun is forced to be captured. If no response from izin or izin tidak diberi apps should allow user to click selesai

====================================================================================
Version: 1.0.5.0
====================================================================================
2019-11-17
1. Update Pemeriksaan, camera form and set new date when new pemeriksaan


====================================================================================
Version: 1.0.4.9
====================================================================================
2019-11-15
1. Printing VALUES only for all documents (KPP, kompaun and AKuan) must be CAPS LOCK
2. Masa Mula and Masa Tamat in 2 lines
3. Kompaun > Tab Penerima, "No. K/P" change to "No K/P atau No Passport"
4. Change Message "ID telah wujud dalam pasukan ini" to "ID Pengguna yang ingin ditambah telah wujud dalam pasukan ini"
5. Change length (in db and screen max length) from tbdatakes.NOEP and tbdatakes.NOIP to 30
6. Validate button Selesai in Pemeriksaan
	- If Tindakan = Pemeriksaan then display message "Anda Pasti untuk tamatkan lawatan..."
	- if Tindakan = KOTs need to check the existence kompaun 
	if norecord then prompt message "Maklumat Kompaun masih belum lengkap. Selesai pemeriksaan tidak dibenarkan"
	else display message "Anda Pasti untuk tamatkan lawatan..."
	- if tindakan = Kes dihasilan need to check the existence kompaun 
	if norecord then prompt message "Maklumat Siasatan Lanjut masih belum lengkap. Selesai pemeriksaan tidak dibenarkan"
	else display message "Anda Pasti untuk tamatkan lawatan..."
7. Pengeluar1 truncated.Move Pengeluar1 and Pengeluar2 to the left a bit
8. Update Printing Routine change function to PrintStdModeColorBitmap()

====================================================================================
Version: 1.0.4.8
====================================================================================
2019-11-15
1. Update log Screen 
	- IEMS change to Integrated Enforcement Management System (IEMS)
	- Check In change to  Mula Tugas	
2. Update Main Menu
	- Check Out change to  Tamat Tugas
	- Log Keluar put at the bottom of the menu (far from Tamat Tugas)	
3. Pemeriksaan
	- move Amaran from tab Premis to Tab Penerima (on the left next to checkbox Jika OKK setuju bayar)
		Amaran checkbox only enable if user click Tiada Kes
	- Tab Lawatan 
		- change Tarikh to Tarikh Mula
		- Tarikh and Masa is enable (user can change). refresh everytime new KPP
		- Move Tarikh Tamat Lawatan from tab Penerima to tab Lawatan
			- Just put Tarikh Tamat : <>  	Masa : <>
			- compulsary			
			then if tarikhmasatamat <= (less or equal) than tarikhmasamula prompt message "Tarikh dan Masa Tamat perlu lebih lebih besar dari Tarikh dan Masa Mula"
			
		- Catatan lawatan make it 3 lines
		- Hasil Lawatan make it 6 lines
	- Tab penerima 
		- change caption at Tindakan portion
			- Tiada Kes to "Pemeriksaan"
			- Siasatan Lanjut to "Kes dihasilkan (untuk siasatan)"
4. Typing input CAPS LOC
5. Cetakan KPP
	- change caption "Pegawai Yang Mengetuai Lawatan" to "Pegawai Pengeluar"
	- value for Nama change to nama getting from tbkpp.pengeluarkpp
	- make 1 line space after Nama (before Waktu Meninggalkan Premis)	
6. Pemeriksaan Tab penerima No. K/P change ti "No K/P atau No Passport		
7. Cetakan KPP
- Line "Pengakuan Orang kena kompaun (OKK) ..... " and "*Saya bersetuju... " and textbox 
only print when Tindakan = KOTS
- If Tujuan Lawatan = 1 and checkbox is ticked, print "Amaran diberikan"
- Print <tbpengguna.singkatan_jawatan> before nama pegawai and pegawai pengeluar

8. Cetakan KPP
	- Print <tbpengguna.singkatan_jawatan> before nama pegawai from tbpasukan trans  and pegawai pengeluar (update in number 5)
	- Hasil lawatan is truncated	
9. Tab Lawatan KPP, If Tujuan Lawatan = 1 and No aduan is compulsary (validate when printing)
	
10. Cetakan KPP, Print (as shown in the image) only if Tujuan Lawatan = 1 and No aduan <> ''
11. Kompaun > Tab Butiran (when click ? on Kesalahan) , List of Kesalahan Display tbkesalahan.seksyen in the line before tbkesalahan.keterangan
12. Cetakan Kompaun Kesalahan print from tbkesalahan.prgn (not from butirkesalahan)
13. Kompaun > Tab Butiran (afere user click ? on Kesalahan and the page go back to Skrin tab butiran)
	- Display tbkesalahan.seksyen in the Kesalahan Field
	- Display tbkesalahan.prgn in Butir Kesalahan Field
	- Disabled field Butir Kesalahan 
	- remove ? in Butir Kesalahan. make the box bigger
14. Cetakan Kompaun
	- the first line is printing value from tbakta.pengeluar1
	- the second line is printing value from tbakta.pengeluar2
16. Add close (X) in every loop up list screen for user to close the screen
17. Cetakan KPP Remove line print "BAHAGIAN PENGUATKUASA" on top
18. Cetakan Kompaun Remove line print "BAHAGIAN PENGUATKUASA" on print
19. Update SendOnline , replace single quote

====================================================================================
Version: 1.0.4.7
====================================================================================
2019-11-13
1. Update Print Pemeriksaan and Kompaun
2, Add ConfigApp.xml
3. Update Background and Checkout

====================================================================================
Version: 1.0.4.6
====================================================================================
2019-11-12
1. Update Checkout, check line before start process
2. Update convert time 12 am = 00 , 12 pm = 12
3. remove iems4 from generated script when send online
4. add logging in execute query API when status not success

====================================================================================
Version: 1.0.4.5
====================================================================================
2019-11-12
1. Update Background Timer, 5 minute

====================================================================================
Version: 1.0.4.4
====================================================================================
2019-11-12
1. Update Print Dialog

====================================================================================
Version: 1.0.4.3
====================================================================================
2019-11-11
(Mr. Pek)

====================================================================================
Version: 1.0.4.2
====================================================================================
2019-11-11
1. Update Pemeriksaan for Get Premis , when HQR , get all premis
2. Update Print for null text

====================================================================================
Version: 1.0.4.1
====================================================================================
2019-11-11
1. Update Print Kompaun , for tempoh tawaran , get from tbkompaun,Tempohtawaran
2. Remove UnUsed log in HttpClientService
3. Update Print Function

====================================================================================
Version: 1.0.4.0
====================================================================================
2019-11-10
1. Update Pemeriksaan , for CheckKompaunIzin
2. Update Kompaun , Set No Kp Penerima from Pemeriksaan
3. for jenis pesalah, please change the value. Individual = 0, syarikat = 2. 
4. for tbkompaun.iscetakakuan please also change the value. uncheck = 0, checked = 1
5. for sending online to tbkompaun_hh and tbkompaun, please change then value for ishh to 0. currently is 1
6. tbkompaun.jeniskmp change to 0
7. tbkompaun. Isarahansemasa , uncheck=0, checked=1
8. tbkpp.amaran,uncheck = 0,checked= 1	
   tbkpp.setujubayar,uncheck = 0,checked= 1
   tbkpp.tindakan,Tiada Kes= 0,	KOTS= 1, Siasatan Lanjut=2


====================================================================================
Version: 1.0.3.9
====================================================================================
2019-11-09
1. Update Kompaun, clear butir kesalahan when kesalahan change
2  Rename "Success send data" to "Data berjaya
3 Update MyCard in Kompaun , Pesalah , will update pesalah and Penerima,
  when in penerima only update penerima
4. Update SendOnline

====================================================================================
Version: 1.0.3.8
====================================================================================
2019-11-09
1. Update convert time 
2. update some word message
3. update pemeriksaan for okk checkbox
4. update print kompaun for individu/syarikat amoun
5. Fix Upload Image,
6. Add Status when update Kompaun and Kompaun_HH


====================================================================================
Version: 1.0.3.7
====================================================================================
2019-11-09
1. Update Pasukan , for insert data
2. Update Print Layout pemeriksaan
3. update ViewSiasatanLanjut for button
4. Update camera , for wording photonotexist
5. Update androidmanifest for camera function

====================================================================================
Version: 1.0.3.6
====================================================================================
2019-11-09
1. Update Kompaun Form , nama pesalah(mandatory)
2. View Kompaun, update get from AmnByr
3. Update wording kompaun izin
4. Update Camera , change gambar to Kamera on icon
5. Update Print Pemeriksaan, kompaun and akuan
6. Update Flow for Pemeriksaan, Kompaun, Akuan and SiasatanLanjut
7. Add menu Kembali for Kompaun, Akuan and SiasatanLanjut
8. SendOnline

Version: 1.0.3.5
====================================================================================
2019-11-08
1. Remove create bitmap print

====================================================================================
Version: 1.0.3.4
====================================================================================
2019-11-07
1. Update Akuan for SendOnline

====================================================================================
Version: 1.0.3.3
====================================================================================
2019-11-07
1. Rollback Android option for supported architecture
2. Update print for all print only 1

====================================================================================
Version: 1.0.3.2
====================================================================================
2019-11-07
1. Update Android option for supported architecture
2. Update print layout
3. Remark validate in pemeriksaan for tarikh tamat
4. remark validate in kompaun and akuan
5. add debug line in on printing function to know which line is error
6. create bitmap file and saved into imgs folder
7. Update Checkout, remove pasukan_hh
8. Change send image flow


====================================================================================
Version: 1.0.3.1
====================================================================================
2019-11-07
1. Update print to
2. Update  camera for change label and icon to Keluar
3. Change title confirm dialog siasat lanutan to Pengesahan Siasatan Lanjut
4. Update Pemeriksaan , for init set empty for masa
5. validate for date input cannot input back date
6. update login for battery status
7. Update Print in View Kompaun and View Akuan for 2 times
8. Send Online

====================================================================================
Version: 1.0.3.0
====================================================================================
2019-11-06
1. Update Remove Pasukan Modal , No. KP to ID Pengguna
2. Update Confirm Dialog Pemeriksaan Title to Pengesahan KPP
3. Update Print Pemeriksaan, Kompaun and Akuan
4. Add Checkout Form
5. Update SendOnline
6. Update Word in check kompaun izin

====================================================================================
Version: 1.0.2.9
====================================================================================
2019-11-05
1. Fix bug time format when insert to database

====================================================================================
Version: 1.0.2.8
====================================================================================
2019-11-05
1. Update Print pemeriksaan
2. Update SendOnline

====================================================================================
Version: 1.0.2.7
====================================================================================
2019-11-04
1. Change JenisPerniagaan , AsasTindakan and Kesalahan to Search
2. Change Word Re-Print and Send Online Failed message box
3. add checkbox bayaran
4. and Tamat Lawatan date and time
5. update view pemeriksaan
6. update SendOnline


====================================================================================
Version: 1.0.2.6
====================================================================================
2019-10-30
1. Update get cawanganUser from tbhandheld
2. Update Kompaun , when go to akuan , will close first
3. Hapus Pasukan, can not delete pasukan that on login
4. update Print image for bitmap recycle
5. update siasatan lanjut
6. add field catatan(250) in tbpasukanhh
7. when remove pasukan , add catatam with current date
8. remove field KOTS in TbKesalahan
9. Update Print logo on kompaun and akuan
10. Send Data for Siasat lanjutan
11. Add Log for Background Service

====================================================================================
Version: 1.0.2.5
====================================================================================
2019-10-31
1. Send Kompaun Bayaran
2. Change Function for Upload image

2019-10-30
1. Update Feedback
2. Update Pasukan
3. Update TbKompaun_Izin , add kodCawangan
4. Add TbGpsLog

====================================================================================
Version: 1.0.2.4
====================================================================================
2019-10-29
1. Update PrintImageBll Kompaun
2. Akuan

====================================================================================
Version: 1.0.2.2
====================================================================================
2019-10-29
1. Update PrintImageBll, Pemeriksaan
2. Update Print for Kompaun and Akuan

====================================================================================
Version: 1.0.2.1
====================================================================================
2019-10-28
1. Update PrintImageBll
2. Add Max Length for EditText
