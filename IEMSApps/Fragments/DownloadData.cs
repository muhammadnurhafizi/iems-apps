using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using IEMSApps.Activities;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Responses;
using IEMSApps.Classes;
using IEMSApps.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IEMSApps.Fragments
{
    public class DownloadData : Fragment
    {
        private HourGlassClass _hourGlass = new HourGlassClass();
        private List<TableSummaryResponse> currentDatas;
        private TableSummaryAdapter currentAdapter;
        private ListView dataList;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.tablesummary_layout, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _hourGlass?.StartMessage(this.Activity, SetInit);
        }


        TextView lblInfos, _lblTotal, _lblPercentage;// _lblInfo, lblInfos, ;
        LinearLayout linearHeader, linearSnack;
        private Button btnCheckIn;

        private ProgressBar _progressBar2, _progressBar1;
        //private AlertDialog _alertDialog;

        private void SetInit()
        {
            linearHeader = View.FindViewById<LinearLayout>(Resource.Id.linearHeader);
            //linearProogress = View.FindViewById<LinearLayout>(Resource.Id.linearProogress);

            lblInfos = View.FindViewById<TextView>(Resource.Id.lblInfos);
            linearSnack = View.FindViewById<LinearLayout>(Resource.Id.linearSnack);
            _progressBar1 = View.FindViewById<ProgressBar>(Resource.Id.progressBar1);
            _progressBar2 = View.FindViewById<ProgressBar>(Resource.Id.progressBar2);
            //
            _lblTotal = View.FindViewById<TextView>(Resource.Id.lblTotal);
            _lblPercentage = View.FindViewById<TextView>(Resource.Id.lblPercentage);
            //
            //_alertDialog = GeneralAndroidClass.GetDialogCustom(this.Context);
            dataList = View.FindViewById<ListView>(Resource.Id.listCompaund);

            btnCheckIn = View.FindViewById<Button>(Resource.Id.btnCheckIn);
            btnCheckIn.Click += BtnCheckIn_Click;

            IsLoading(false);
            currentDatas = new List<TableSummaryResponse>();
            //{
            //    new TableSummaryResponse
            //    {
            //        RecordDesc = "Test",
            //        TableName = "a"
            //    }
            //};
            currentAdapter = new TableSummaryAdapter(this, currentDatas);
            dataList.Adapter = currentAdapter;
            linearHeader.Visibility = ViewStates.Invisible;
            linearSnack.Visibility = ViewStates.Gone;

            new Task(() =>
            {
                CheckIn();
            }).Start();
        }

        private void BtnCheckIn_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                PrepareDataSelected();
            }).Start();
        }

        private void IsLoading(bool value)
        {
            Activity.RunOnUiThread(() =>
            {
                //_lblInfo.Visibility = value ? ViewStates.Visible : ViewStates.Invisible;
                //lblInfos.Visibility = value ? ViewStates.Visible : ViewStates.Invisible;
                //linearSnack.Visibility = value ? ViewStates.Visible : ViewStates.Invisible;
                //_progressBar1.Visibility = value ? ViewStates.Visible : ViewStates.Invisible;
                btnCheckIn.Visibility = value ? ViewStates.Gone : ViewStates.Visible;
                _progressBar2.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
                lblInfos.Visibility = value ? ViewStates.Visible : ViewStates.Gone;

                ((MainActivity)Activity).SetMenuDrawer(!value);
            });
        }

        private void ShowErrorMessage(string message)
        {
            Activity.RunOnUiThread(() =>
            {
                lblInfos.Visibility = ViewStates.Visible;
                lblInfos.Text = message;
                btnCheckIn.Visibility = ViewStates.Gone;
            });
        }

        private void ShowLoadingWhenInsertData(bool isProcessInsert)
        {
            Activity.RunOnUiThread(() =>
            {
                linearSnack.Visibility = isProcessInsert ? ViewStates.Visible : ViewStates.Gone;
                _progressBar2.Visibility = isProcessInsert ? ViewStates.Gone : ViewStates.Visible;
            });
        }

        private void CheckIn()
        {
            try
            {
                IsLoading(true);
                var userHandheld = GeneralBll.GetUserHandheld();
                var tbHandheld = DataAccessQuery<TbHandheld>.Get(m => m.IdHh == userHandheld);
                //var time = string.IsNullOrEmpty(tbHandheld.Datas.TrkhUpdateDate) ? tbHandheld.Datas.TrkhHhCheckin : tbHandheld.Datas.TrkhUpdateDate;
                var time = tbHandheld.Datas.TrkhUpdateDate;
                var trkhUpdateDate = GeneralBll.ConvertDatabaseFormatStringToDateTime(tbHandheld.Datas.TrkhUpdateDate);
                var result = HandheldBll.GetTableSummary($"{GeneralBll.GetUserHandheld()}|{trkhUpdateDate.ToString(Constants.DatabaseDateFormatWithoutSecond)}");

                IsLoading(false);
                Activity.RunOnUiThread(() =>
                {
                    currentDatas = new List<TableSummaryResponse>();
                    SetAdapter();

                    if (result.Success)
                    {
                        linearHeader.Visibility = ViewStates.Visible;
                        currentDatas = result.Result;
                        SetAdapter();
                    }
                    else
                    {
                        ShowErrorMessage(result.Mesage);
                        //GeneralAndroidClass.ShowToast(result.Mesage);
                    }
                });
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.ShowToast("Gagal");
                GeneralBll.LogDataWithException("DownloadData", "CheckIn", ex);
            }
            finally
            {
                _hourGlass?.StopMessage();
            }
        }

        internal void CompaundIsChange(string tableName, bool isChecked)
        {
            if (currentDatas.Any())
            {
                var temp = new List<TableSummaryResponse>();
                foreach (var item in currentDatas)
                {
                    if (item.TableName == tableName)
                        item.IsSelected = isChecked;
                    temp.Add(item);
                }
                currentDatas = temp;
                SetAdapter();
            }
        }

        private void SetAdapter()
        {
            currentAdapter = new TableSummaryAdapter(this, currentDatas);
            dataList.Adapter = currentAdapter;
        }

        private void AdapterIsChange()
        {
            if (currentAdapter != null)
            {
                currentAdapter.NotifyDataSetChanged();
            }
        }

        private void PrepareDataSelected()
        {
            if (!currentDatas.Where(m => m.IsSelected).Any())
            {
                ShowToast("Sila Pilih rekod terlebih dahulu");
                IsLoading(false);
                return;
            }

            try
            {
                IsLoading(true);


                UpdateInfo("Servis Dihentikan...");
                GeneralAndroidClass.StopLocationService(this.Context);
                Thread.Sleep(2000);

                var result = HandheldBll.PrepareDownloadDataSelected($"{GeneralBll.GetUserHandheld()}|{string.Join(",", currentDatas.Where(m => m.IsSelected).Select(m => m.TableName))}");
                if (result.Success)
                {
                    ShowLoadingWhenInsertData(true);
                    var totalData = 0;
                    var totalSaved = 0;
                    for (int i = 1; i <= 15; i++)
                    {
                        totalSaved = 0;
                        switch (i)
                        {
                            case 1:
                                if (!result.Result.Tbcawangan.Any()) continue;

                                DataAccessQuery<TbCawangan>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Cawangan");

                                totalData = result.Result.Tbcawangan.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbcawangan.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbCawangan>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbcawangan").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });

                                break;
                            case 2:
                                if (!result.Result.Tbpengguna.Any()) continue;
                                //Insert into Table TbPengguna
                                DataAccessQuery<TbPengguna>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Anggota Penguatkuasa");

                                totalData = result.Result.Tbpengguna.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbpengguna.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbPengguna>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbpengguna").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });

                                break;
                            case 3:
                                if (!result.Result.Tbkategorikawasan.Any()) continue;

                                DataAccessQuery<TbKategoriKawasan>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Kategori Kawasan");

                                totalData = result.Result.Tbkategorikawasan.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbkategorikawasan.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbKategoriKawasan>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbkategorikawasan").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });
                                break;
                            case 4:
                                if (!result.Result.Tbpremis.Any()) continue;

                                DataAccessQuery<TbPremis>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Lokasi");

                                totalData = result.Result.Tbpremis.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbpremis.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbPremis>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbpremis").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });

                                break;
                            case 5:
                                if (!result.Result.Tbtujuanlawatan.Any()) continue;

                                DataAccessQuery<TbTujuanLawatan>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Tujuan Lawatan");

                                totalData = result.Result.Tbtujuanlawatan.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbtujuanlawatan.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbTujuanLawatan>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbtujuanlawatan").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });

                                break;
                            case 6:
                                if (!result.Result.Tbasastindakan.Any()) continue;

                                DataAccessQuery<TbAsasTindakan>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Asas Tindakan");

                                totalData = result.Result.Tbasastindakan.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbasastindakan.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbAsasTindakan>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbasastindakan").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });
                                break;
                            case 7:
                                if (!result.Result.Tbkategoripremis.Any()) continue;

                                DataAccessQuery<TbKategoriPremis>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Kategori Premis");

                                totalData = result.Result.Tbkategoripremis.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbkategoripremis.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbKategoriPremis>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbkategoripremis").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });
                                break;
                            case 8:

                                if (!result.Result.Tbjenisperniagaan.Any()) continue;

                                DataAccessQuery<TbJenisPerniagaan>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Jenis Perniagaan");

                                totalData = result.Result.Tbjenisperniagaan.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbjenisperniagaan.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbJenisPerniagaan>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbjenisperniagaan").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });
                                break;
                            case 9:
                                if (!result.Result.Tbnegeri.Any()) continue;

                                DataAccessQuery<TbNegeri>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Negeri");

                                totalData = result.Result.Tbnegeri.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbnegeri.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbNegeri>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);

                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbnegeri").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;
                                    });
                                    AdapterIsChange();
                                });

                                break;
                            case 10:
                                if (!result.Result.Tbbandar.Any()) continue;

                                DataAccessQuery<TbBandar>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Bandar");

                                totalData = result.Result.Tbbandar.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbbandar.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbBandar>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbbandar").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });
                                break;
                            case 11:
                                if (!result.Result.Tbakta.Any()) continue;

                                DataAccessQuery<TbAkta>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Akta");

                                totalData = result.Result.Tbakta.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbakta.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbAkta>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbakta").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });
                                break;
                            case 12:
                                if (!result.Result.Tbkesalahan.Any()) continue;

                                DataAccessQuery<TbKesalahan>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Kesalahan");

                                totalData = result.Result.Tbkesalahan.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbkesalahan.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbPremis>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbkesalahan").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });
                                break;
                            case 13:
                                if (!result.Result.Tbkategoriperniagaan.Any()) continue;

                                DataAccessQuery<TbKategoriPerniagaan>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Kategori Perniagaan");

                                totalData = result.Result.Tbkategoriperniagaan.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbkategoriperniagaan.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbKategoriPerniagaan>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbkategoriperniagaan").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });
                                break;
                            case 14:
                                if (!result.Result.Tbbarang_jenama.Any()) continue;

                                DataAccessQuery<TbJenama>.DeleteAll();
                                UpdateInfo(Constants.Messages.InsertData + " Jenama");

                                totalData = result.Result.Tbbarang_jenama.Count;
                                _progressBar1.Max = totalData;

                                foreach (var item in result.Result.Tbbarang_jenama.Select((value, index) => new { index, value }))
                                {
                                    UpdateCountAndPercentage(item.index, totalData);
                                    totalSaved += DataAccessQuery<TbJenama>.ExecuteSql(item.value.Value);
                                }
                                UpdateCountAndPercentage(totalData, totalData);
                                Activity.RunOnUiThread(() =>
                                {
                                    currentDatas.Where(m => m.TableName == "tbbarang_jenama").ToList().ForEach(m =>
                                    {
                                        m.TotalApp = totalSaved;

                                    });
                                    AdapterIsChange();
                                });

                                //if (!result.Result.Tbskipcontrol.Any()) continue;
                                //
                                //DataAccessQuery<TbSkipControl>.DeleteAll();
                                //UpdateInfo(Constants.Messages.InsertData + " Skip Control");
                                //
                                //totalData = result.Result.Tbskipcontrol.Count;
                                //_progressBar1.Max = totalData;
                                //
                                //foreach (var item in result.Result.Tbskipcontrol.Select((value, index) => new { index, value }))
                                //{
                                //    UpdateCountAndPercentage(item.index, totalData);
                                //    DataAccessQuery<TbSkipControl>.ExecuteSql(item.value.Value);
                                //}
                                //UpdateCountAndPercentage(totalData, totalData);

                                break;
                            case 15:

                                break;
                        }
                        Thread.Sleep(500);
                    }

                    ShowLoadingWhenInsertData(false);
                    UpdateInfo("Sila Tunggu, sedang mendapatkan data...");

                    currentDatas = HandheldBll.CompareWithExistingData(currentDatas);
                    Activity.RunOnUiThread(() =>
                    {
                        SetAdapter();
                    });

                    var userHandheld = GeneralBll.GetUserHandheld();
                    HandheldBll.UpdateHandheldSetTrkhHhMuatTurunData(userHandheld, this.Context);

                    //var tbHandheld = DataAccessQuery<TbHandheld>.Get(m => m.IdHh == userHandheld);
                    //tbHandheld.Datas.TrkhUpdateDate = GeneralBll.GetLocalDateTimeForDatabase();
                    //DataAccessQuery<TbHandheld>.Update(tbHandheld.Datas);

                    CheckIn();
                    IsLoading(false);
                }
                else
                {
                    IsLoading(false);
                    ShowToast(result.Mesage);
                }
            }
            catch (Exception ex)
            {
                ShowToast("Gagal");
                GeneralBll.LogDataWithException("DownloadData", "CheckIn", ex);

                IsLoading(false);
            }
            finally
            {
                GeneralAndroidClass.StartLocationService(this.Context);
            }
        }

        //private void CheckIn()
        //{
        //    try
        //    {
        //        IsLoading(true);

        //        UpdateInfo("Memanggil API...");
        //        _progressBar1.Progress = 5;
        //        var totalData = 0;

        //        var result = HandheldBll.PrepareDownloadFreshDatas(GeneralBll.GetUserHandheld());
        //        _progressBar1.Progress = 50;

        //        if (result.Success)
        //        {
        //            HandheldBll.UpdateHandheldSetTrkhHhCheckin(GeneralBll.GetUserHandheld());

        //            _progressBar1.Progress = 100;
        //            UpdateCountAndPercentage(0, 0);

        //            for (int i = 1; i <= 15; i++)
        //            {
        //                switch (i)
        //                {
        //                    case 1:
        //                        //DataAccessQuery<TbPasukanHh>.DeleteAll();
        //                        //UpdateInfo(Constants.Messages.InsertData + " Pasukan");

        //                        //totalData = result.Result.Tbpasukan_hh.Count;
        //                        _progressBar1.Max = totalData;

        //                        //foreach (var item in result.Result.Tbpasukan_hh.Select((value, index) => new { index, value }))
        //                        //{
        //                        //    UpdateCountAndPercentage(item.index, totalData);
        //                        //    DataAccessQuery<TbPasukanHh>.ExecuteSql(item.value.Value);
        //                        //}

        //                        //UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                    case 2:
        //                        DataAccessQuery<TbNegeri>.DeleteAll();
        //                        UpdateInfo(Constants.Messages.InsertData + " Negeri");

        //                        totalData = result.Result.Tbnegeri.Count;
        //                        _progressBar1.Max = totalData;

        //                        foreach (var item in result.Result.Tbnegeri.Select((value, index) => new { index, value }))
        //                        {
        //                            UpdateCountAndPercentage(item.index, totalData);
        //                            DataAccessQuery<TbNegeri>.ExecuteSql(item.value.Value);
        //                        }
        //                        UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                    case 3:
        //                        InsertCawangan(result.Result.TbcawanganTemp);
        //                        break;
        //                    case 4:
        //                        //DataAccessQuery<TbPengguna>.DeleteAll();
        //                        //UpdateInfo(Constants.Messages.InsertData + " Pengguna");

        //                        //totalData = result.Result.Tbpengguna.Count;
        //                        _progressBar1.Max = totalData;

        //                        //foreach (var item in result.Result.Tbpengguna.Select((value, index) => new { index, value }))
        //                        //{
        //                        //    UpdateCountAndPercentage(item.index, totalData);
        //                        //    DataAccessQuery<TbPengguna>.ExecuteSql(item.value.Value);
        //                        //}
        //                        //UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                    case 5:
        //                        DataAccessQuery<TbTujuanLawatan>.DeleteAll();
        //                        UpdateInfo(Constants.Messages.InsertData + " Tujuan Lawatan");

        //                        totalData = result.Result.Tbtujuanlawatan.Count;
        //                        _progressBar1.Max = totalData;

        //                        foreach (var item in result.Result.Tbtujuanlawatan.Select((value, index) => new { index, value }))
        //                        {
        //                            UpdateCountAndPercentage(item.index, totalData);
        //                            DataAccessQuery<TbTujuanLawatan>.ExecuteSql(item.value.Value);
        //                        }
        //                        UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                    case 6:
        //                        InsertAsasTindakan(result.Result.TbasastindakanTemp);
        //                        break;
        //                    case 7:
        //                        DataAccessQuery<TbKategoriKawasan>.DeleteAll();
        //                        UpdateInfo(Constants.Messages.InsertData + " Kategori Kawasan");

        //                        totalData = result.Result.Tbkategorikawasan.Count;
        //                        _progressBar1.Max = totalData;

        //                        foreach (var item in result.Result.Tbkategorikawasan.Select((value, index) => new { index, value }))
        //                        {
        //                            UpdateCountAndPercentage(item.index, totalData);
        //                            DataAccessQuery<TbKategoriKawasan>.ExecuteSql(item.value.Value);
        //                        }
        //                        UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                    case 8:
        //                        DataAccessQuery<TbKategoriPremis>.DeleteAll();
        //                        UpdateInfo(Constants.Messages.InsertData + " Kategori Premis");

        //                        totalData = result.Result.Tbkategoripremis.Count;
        //                        _progressBar1.Max = totalData;

        //                        foreach (var item in result.Result.Tbkategoripremis.Select((value, index) => new { index, value }))
        //                        {
        //                            UpdateCountAndPercentage(item.index, totalData);
        //                            DataAccessQuery<TbKategoriPremis>.ExecuteSql(item.value.Value);
        //                        }
        //                        UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                    case 9:
        //                        DataAccessQuery<TbKategoriPerniagaan>.DeleteAll();
        //                        UpdateInfo(Constants.Messages.InsertData + " Kategori Perniagaan");

        //                        totalData = result.Result.Tbkategoriperniagaan.Count;
        //                        _progressBar1.Max = totalData;

        //                        foreach (var item in result.Result.Tbkategoriperniagaan.Select((value, index) => new { index, value }))
        //                        {
        //                            UpdateCountAndPercentage(item.index, totalData);
        //                            DataAccessQuery<TbKategoriPerniagaan>.ExecuteSql(item.value.Value);
        //                        }
        //                        UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                    case 10:
        //                        InsertJenisPerniagaan(result.Result.TbjenisperniagaanTemp);
        //                        break;
        //                    case 11:
        //                        InsertPremis(result.Result.TbpremisTemp);
        //                        break;
        //                    case 12:
        //                        InsertAkta(result.Result.TbaktaTemp);
        //                        break;
        //                    case 13:
        //                        InsertKesalahan(result.Result.TbkesalahanTemp);
        //                        break;
        //                    case 14:
        //                        InsertBandar(result.Result.TbbandarTemp);
        //                        break;
        //                    case 15:
        //                        DataAccessQuery<TbSkipControl>.DeleteAll();
        //                        UpdateInfo(Constants.Messages.InsertData + " Skip Control");

        //                        totalData = result.Result.Tbskipcontrol.Count;
        //                        _progressBar1.Max = totalData;

        //                        foreach (var item in result.Result.Tbskipcontrol.Select((value, index) => new { index, value }))
        //                        {
        //                            UpdateCountAndPercentage(item.index, totalData);
        //                            DataAccessQuery<TbSkipControl>.ExecuteSql(item.value.Value);
        //                        }
        //                        UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                    case 16:
        //                        DataAccessQuery<TbJenama>.DeleteAll();
        //                        UpdateInfo(Constants.Messages.InsertData + " Jenis Jenama");

        //                        totalData = result.Result.Tbbarang_jenama.Count;
        //                        _progressBar1.Max = totalData;

        //                        foreach (var item in result.Result.Tbbarang_jenama.Select((value, index) => new { index, value }))
        //                        {
        //                            UpdateCountAndPercentage(item.index, totalData);
        //                            DataAccessQuery<TbJenama>.ExecuteSql(item.value.Value);
        //                        }
        //                        UpdateCountAndPercentage(totalData, totalData);
        //                        break;
        //                }
        //                Thread.Sleep(500);
        //            }

        //            //Insert into Table TbPengguna
        //            InsertPengguna(result.Result.TbpenggunaTemp);

        //            HandheldBll.UpdatePasukanAsync(GeneralBll.GetUserHandheld());

        //            UpdateCountAndPercentage(0, 0);
        //            _progressBar1.Progress = 100;

        //            UpdateInfo("DONE");
        //            Thread.Sleep(2000);
        //        }
        //        else
        //        {
        //            UpdateInfo("ERROR.");

        //            if (result.Mesage.Contains("Connection"))
        //                _alertDialog.SetMessage("Tiada sambungan ke internet. Data tidak dapat diterima/dihantar");
        //            else
        //                _alertDialog.SetMessage(result.Mesage);
        //            _alertDialog.SetButton2("OK", (s, e) => { });
        //            _alertDialog.DismissEvent += (s, e) =>
        //            {
        //                //GoToLogin();
        //            };

        //            Activity.RunOnUiThread(() =>
        //                _alertDialog.Show()
        //            );
        //        }

        //        IsLoading(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        UpdateInfo("ERROR. " + ex.Message);
        //        GeneralBll.LogDataWithException("CheckIn", "CheckIn", ex);
        //    }
        //    finally
        //    {
        //        IsLoading(false);
        //    }
        //}

        private void UpdateInfo(string message)
        {
            Activity.RunOnUiThread(() => lblInfos.Text = message);
        }

        private void UpdateCountAndPercentage(int index, int total)
        {
            Activity.RunOnUiThread(() =>
            {
                _progressBar1.Progress = index;

                if (index != 0 && total != 0)
                {
                    _lblTotal.Text = $"Data {index}/{total}";
                    _lblPercentage.Text = $"{(int)Math.Round((double)(100 * index) / total)}%";
                }
                else
                {
                    _lblTotal.Text = string.Empty;
                    _lblPercentage.Text = string.Empty;
                }

            });
        }

        private void ShowToast(string message)
        {
            Activity.RunOnUiThread(() =>
            {
                GeneralAndroidClass.ShowToast(message);
            });
        }

        #region Penggguna

        private void InsertPengguna(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Pengguna Temp");
            DataAccessQuery<TbPenggunaTemp>.ExecuteSql("DELETE FROM tbpenggunatemp");

            UpdateInfo(Constants.Messages.InsertData + " Pengguna Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbPenggunaTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Pengguna");
            DataAccessQuery<TbPengguna>.ExecuteSql("DELETE FROM tbpengguna");

            UpdateInfo(Constants.Messages.Move + " Pengguna");
            DataAccessQuery<TbPengguna>.ExecuteSql("INSERT INTO tbpengguna(ID, NoKp, Kata_Laluan, KodCawangan, Nama, Nama_Bahagian, Nama_Gelaran, Nama_Gelaran_Jawatan, Nama_Jawatan, Gred, Singkatan_Jawatan, PgnDaftar, TrkhDaftar) " +
                                                   "SELECT ID, NoKp, Kata_Laluan, KodCawangan, Nama, Nama_Bahagian, Nama_Gelaran, Nama_Gelaran_Jawatan, Nama_Jawatan, Gred, Singkatan_Jawatan, PgnDaftar, TrkhDaftar FROM tbpenggunatemp WHERE Status = 1");
        }

        #endregion

        #region Cawangan
        private void InsertCawangan(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Cawangan");
            DataAccessQuery<TbCawangan>.ExecuteSql("DELETE FROM tbcawangan");

            UpdateInfo(Constants.Messages.InsertData + " Cawangan");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbCawangan>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            //UpdateInfo(Constants.Messages.HapusData + " Cawangan");
            //DataAccessQuery<TbCawangan>.ExecuteSql("DELETE FROM tbcawangan");
            //
            //UpdateInfo(Constants.Messages.Move + " Cawangan");
            //DataAccessQuery<TbCawangan>.ExecuteSql("INSERT INTO tbcawangan(KodCawangan, Prgn, Nama_Cawangan, Alamat1, Alamat2, Poskod, KodNegeri, Bandar, Emel, No_Faks, No_Telefon, PgnDaftar, TrkhDaftar) " +
            //                                       "SELECT KodCawangan, Prgn, Nama_Cawangan, Alamat1, Alamat2, Poskod, KodNegeri, Bandar, Emel, No_Faks, No_Telefon, PgnDaftar, TrkhDaftar FROM tbcawanganTemp  WHERE Status = 1");

        }
        #endregion

        #region tbasastindakantemp
        private void InsertAsasTindakan(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Asas Tindakan Temp");
            DataAccessQuery<TbAsasTindakanTemp>.ExecuteSql("DELETE FROM tbasastindakanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Asas Tindakan Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbAsasTindakanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Asas Tindakan");
            DataAccessQuery<TbAsasTindakan>.ExecuteSql("DELETE FROM tbasastindakan");

            UpdateInfo(Constants.Messages.Move + " Asas Tindakan");
            DataAccessQuery<TbAsasTindakan>.ExecuteSql("INSERT INTO tbasastindakan(KodTujuan, KodAsas, Prgn, PgnDaftar, TrkhDaftar) " +
                                                       "SELECT KodTujuan, KodAsas, Prgn, PgnDaftar, TrkhDaftar FROM tbasastindakanTemp WHERE Status = 1");
        }
        #endregion

        #region tbjenisperniagaantemp
        private void InsertJenisPerniagaan(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Jenis Perniagaan Temp");
            DataAccessQuery<TbJenisPerniagaanTemp>.ExecuteSql("DELETE FROM tbjenisperniagaanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Jenis Perniagaan Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbJenisPerniagaanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Jenis Perniagaan");
            DataAccessQuery<TbJenisPerniagaan>.ExecuteSql("DELETE FROM tbjenisperniagaan ");

            UpdateInfo(Constants.Messages.Move + " Jenis Perniagaan");
            DataAccessQuery<TbJenisPerniagaan>.ExecuteSql("INSERT INTO tbjenisperniagaan(KodJenis, Prgn, PgnDaftar, TrkhDaftar) " +
                                                       "SELECT KodJenis, Prgn, PgnDaftar, TrkhDaftar FROM tbjenisperniagaanTemp WHERE Status = 1");
        }
        #endregion

        #region TbPremistemp
        private void InsertPremis(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Premis Temp");
            DataAccessQuery<TbPremisTemp>.ExecuteSql("DELETE FROM tbpremisTemp");

            UpdateInfo(Constants.Messages.InsertData + " Premis Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbPremisTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Premis");
            DataAccessQuery<TbPremis>.ExecuteSql("DELETE FROM tbpremis");

            UpdateInfo(Constants.Messages.Move + " Premis");
            DataAccessQuery<TbPremis>.ExecuteSql("INSERT INTO tbpremis(IdPremis, KodCawangan, NamaPremis, AlamatPremis1, AlamatPremis2, AlamatPremis3, NoDaftarPremis, PgnDaftar, TrkhDaftar) " +
                                                 "SELECT IdPremis, KodCawangan, NamaPremis, AlamatPremis1, AlamatPremis2, AlamatPremis3, NoDaftarPremis, PgnDaftar, TrkhDaftar FROM tbpremisTemp WHERE Status = 1");
        }
        #endregion

        #region TbAktatemp
        private void InsertAkta(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Akta Temp");
            DataAccessQuery<TbAktaTemp>.ExecuteSql("DELETE FROM tbaktaTemp");

            UpdateInfo(Constants.Messages.InsertData + " Akta Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbAktaTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Akta");
            DataAccessQuery<TbAkta>.ExecuteSql("DELETE FROM tbakta ");

            UpdateInfo(Constants.Messages.Move + " Akta");
            DataAccessQuery<TbAkta>.ExecuteSql("INSERT INTO tbakta(KodAkta, KodAktaKump, Prgn, PerintahPeraturan, Tajuk1, Tajuk2, Tajuk3, Tajuk4, Tajuk5, Daripada1, Daripada2, Perenggan1, Perenggan2, Perenggan3, Perenggan4, Pengeluar1, Pengeluar2, PgnDaftar, TrkhDaftar) " +
                                               "SELECT KodAkta, KodAktaKump, Prgn, PerintahPeraturan, Tajuk1, Tajuk2, Tajuk3, Tajuk4, Tajuk5, Daripada1, Daripada2, Perenggan1, Perenggan2, Perenggan3, Perenggan4, Pengeluar1, Pengeluar2, PgnDaftar, TrkhDaftar FROM tbaktaTemp WHERE Status = 1");
        }
        #endregion

        #region TbKesalahantemp
        private void InsertKesalahan(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Kesalahan Temp");
            DataAccessQuery<TbKesalahanTemp>.ExecuteSql("DELETE FROM tbkesalahanTemp");

            UpdateInfo(Constants.Messages.InsertData + " Kesalahan Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbKesalahanTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Kesalahan");
            DataAccessQuery<TbKesalahan>.ExecuteSql("DELETE FROM tbkesalahan ");

            UpdateInfo(Constants.Messages.Move + " Kesalahan");
            DataAccessQuery<TbKesalahan>.ExecuteSql("INSERT INTO tbkesalahan (KodAkta, KodSalah, Seksyen, Prgn, AmnKmp_Ind, AmnKmp_Ind_Word, AmnKmp_Sya, AmnKmp_Sya_Word, TempohHari_Tetap, TempohHari_Terbuka, KOTS, PgnDaftar, TrkhDaftar) " +
                                               "SELECT KodAkta, KodSalah, Seksyen, Prgn, AmnKmp_Ind, AmnKmp_Ind_Word, AmnKmp_Sya, AmnKmp_Sya_Word, TempohHari_Tetap, TempohHari_Terbuka, KOTS, PgnDaftar, TrkhDaftar FROM tbkesalahanTemp WHERE Status = 1");
        }
        #endregion

        #region TbBandartemp
        private void InsertBandar(List<ValueResponse> items)
        {
            UpdateInfo(Constants.Messages.HapusData + " Bandar Temp");
            DataAccessQuery<TbBandarTemp>.ExecuteSql("DELETE FROM tbbandarTemp");

            UpdateInfo(Constants.Messages.InsertData + " Bandar Temp");
            var totalData = items.Count;
            _progressBar1.Max = totalData;

            foreach (var item in items.Select((value, index) => new { index, value }))
            {
                UpdateCountAndPercentage(item.index, totalData);
                DataAccessQuery<TbBandarTemp>.ExecuteSql(item.value.Value);
            }
            UpdateCountAndPercentage(totalData, totalData);

            UpdateInfo(Constants.Messages.HapusData + " Bandar");
            DataAccessQuery<TbBandar>.ExecuteSql("DELETE FROM tbbandar");

            UpdateInfo(Constants.Messages.Move + " Bandar");
            DataAccessQuery<TbBandar>.ExecuteSql("INSERT INTO tbbandar(KodNegeri, KodBandar, Prgn, PgnDaftar, TrkhDaftar) " +
                                               "SELECT KodNegeri, KodBandar, Prgn, PgnDaftar, TrkhDaftar FROM tbbandarTemp WHERE Status = 1");

        }
        #endregion
    }
}