using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.Activities;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.BusinessObject.Inputs;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.Fragments
{
    public class Semakan : Fragment
    {
        private const string LayoutName = "Semakan";
        private HourGlassClass _hourGlass = new HourGlassClass();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.Samakan, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            SetInit();
        }

        private Spinner spJenisCarian, spJenisTindakan;
        private EditText txtMaklumatCarian;
        private Button btnCari;
        private ListView lvResult;
        private List<SearchDto> _listData;

        private void SetInit()
        {
            try
            {
                spJenisCarian = View.FindViewById<Spinner>(Resource.Id.spJenisCarian);
                txtMaklumatCarian = View.FindViewById<EditText>(Resource.Id.txtMaklumatCarian);
                spJenisTindakan = View.FindViewById<Spinner>(Resource.Id.spJenisTindakan);
                btnCari = View.FindViewById<Button>(Resource.Id.btnCari);
                lvResult = View.FindViewById<ListView>(Resource.Id.lvResult);

                btnCari.Click += BtnCari_Click;

                lvResult.ItemClick += LvResult_ItemClick; ;

                LoadDataDropdown();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "SetInit", ex.Message, Enums.LogType.Error);
            }

        }

        private void LvResult_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                var data = _listData[e.Position];
                var intent = new Intent(Application.Context, typeof(ViewPemeriksaan));
                intent.PutExtra("norujukan", data.NoRujukan);
                intent.PutExtra("tindakan", data.Tindakan);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "LvResult_ItemClick", ex.Message, Enums.LogType.Error);
            }
        }



        private void BtnCari_Click(object sender, EventArgs e)
        {
            try
            {

                _hourGlass?.StartMessage(this.Activity, OnSearch);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(LayoutName, "BtnCari_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void OnSearch()
        {
            var input = new SearchDataInput();
            input.SearchValue = txtMaklumatCarian.Text;
            input.CarianType = (Enums.SearchCarianType)spJenisCarian.SelectedItemPosition;

            input.TindakanType = (Enums.SearchTindakanType)spJenisTindakan.SelectedItemPosition;


            _listData = SearchBll.GetSearchData(input);
            lvResult.Adapter = new SearchListAdapter(this, _listData);
            lvResult.FastScrollEnabled = true;

            _hourGlass?.StopMessage();
        }

        private void LoadDataDropdown()
        {
            var listJenisCarian = new List<string>
            {
                "",
                "No. KPP",
                "Nama Premis",
                "No. Daftar Syarikat",
                "No.Aduan",
                "No. IC Penerima"
            };

            spJenisCarian.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item, listJenisCarian);

            var listJenisTindakan = new List<string>
            {
                "",
                Constants.TindakanName.Pemeriksaan,
                Constants.TindakanName.KOTS,
                Constants.TindakanName.SiasatLanjut,
                Constants.TindakanName.SiasatUlangan,
            };

            spJenisTindakan.Adapter = new ArrayAdapter<string>(this.Activity,
                Resource.Layout.support_simple_spinner_dropdown_item, listJenisTindakan);

        }

        public void ReSendData(SearchDto searchDto)
        {
            if (!GeneralAndroidClass.IsOnline())
            {
                GeneralAndroidClass.ShowToast("Tiada Sambungan Internet");
                return;
            }

            _hourGlass?.StartMessage(this.Activity, async () =>
            {
                try
                {
                    var result = new BusinessObject.Response<string>();
                    var successSendData = true;

                    if (searchDto.Tindakan == Constants.TindakanName.Pemeriksaan || searchDto.Tindakan == Constants.TindakanName.SiasatUlangan)
                    {
                        result = await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.KPP, this.Activity);
                        if (!result.Success) successSendData = false;
                    }
                    else if (searchDto.Tindakan == Constants.TindakanName.KOTS)
                    {
                        result = await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.KPP, this.Activity);
                        if (!result.Success) successSendData = false;
                        result = await SendOnlineBll.SendDataOnlineAsync(GetNoRujukanKompaun(searchDto.NoRujukan), Enums.TableType.Kompaun, this.Activity);
                        if (!result.Success) successSendData = false;
                        result = await SendOnlineBll.SendDataOnlineAsync(GetNoRujukanDataKes(searchDto.NoRujukan), Enums.TableType.DataKes, this.Activity);
                        if (!result.Success) successSendData = false;
                        result = await SendOnlineBll.SendDataOnlineAsync(GetNoRujukanKompaun(searchDto.NoRujukan), Enums.TableType.Akuan_UpdateKompaun, this.Activity);
                        if (!result.Success) successSendData = false;
                    }
                    else if (searchDto.Tindakan == Constants.TindakanName.SiasatLanjut)
                    {
                        result = await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.KPP, this.Activity);
                        if (!result.Success) successSendData = false;
                        result = await SendOnlineBll.SendDataOnlineAsync(GetNoRujukanDataKes(searchDto.NoRujukan), Enums.TableType.DataKes, this.Activity);
                        if (!result.Success) successSendData = false;
                        result = await SendOnlineBll.SendDataOnlineAsync(GetNoRujukanKompaun(searchDto.NoRujukan), Enums.TableType.Kompaun, this.Activity);
                        if (!result.Success) successSendData = false;
                    }

                    GeneralAndroidClass.ShowToast(successSendData ? Constants.Messages.SuccessSendData : Constants.Messages.FaildSendData);
                    if (successSendData)
                        OnSearch();
                }
                catch (Exception ex)
                {
                    GeneralAndroidClass.LogData(LayoutName, "ReSendDataAsync", ex.Message, Enums.LogType.Error);
                    GeneralAndroidClass.ShowToast(Constants.Messages.FaildSendData);
                }

                _hourGlass?.StopMessage();
            }, Constants.Messages.SendData, cancelable: false);
        }

        private string GetNoRujukanKompaun(string noRujukanKpp)
        {
            var kompaund = DataAccessQuery<TbKompaun>.Get(m => m.NoRujukanKpp == noRujukanKpp);
            if (kompaund.Success && kompaund.Datas != null)
                return kompaund.Datas.NoKmp;
            return noRujukanKpp;
        }

        private string GetNoRujukanDataKes(string noRujukanKpp)
        {
            var kompaund = DataAccessQuery<TbDataKes>.Get(m => m.NoKpp == noRujukanKpp);
            if (kompaund.Success && kompaund.Datas != null)
                return kompaund.Datas.NoKmp ?? kompaund.Datas.NoKes;
            return noRujukanKpp;
        }
    }

    //        public void ReSendDataAsync(SearchDto searchDto)
    //        {
    //            var _dialog = GeneralAndroidClass.ShowProgressDialog(this.Activity, Constants.Messages.SendDataOnline, false);
    //            try
    //            {
    //                new Thread(() =>
    //                {
    //                    Thread.Sleep(1000);
    //                    this.Activity.RunOnUiThread(() =>
    //                    {
    //                        var result = new BusinessObject.Response<string>();
    //                        var successSendData = true;

    //                        if (searchDto.Tindakan == Constants.TindakanName.Pemeriksaan || searchDto.Tindakan == Constants.TindakanName.SiasatUlangan)
    //                        {
    //                            result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.KPP, this.Activity)).Result;
    //                            if (!result.Success) successSendData = false;

    //#if DEBUG
    //                            var datas = DataAccessQuery<BusinessObject.Entities.TbSendOnlineData>.GetAll(m => m.NoRujukan == "KPPKCH0012000096" && (m.Type == Enums.TableType.KPP || m.Type == Enums.TableType.KPP_HH));
    //                            if (datas.Success && datas.Datas.Any())
    //                            {
    //                                foreach (var item in datas.Datas)
    //                                {
    //                                    item.Status = Enums.StatusOnline.Sent;
    //                                    DataAccessQuery<BusinessObject.Entities.TbSendOnlineData>.Update(item);
    //                                }
    //                            }

    //                            successSendData = true;
    //#endif
    //                        }
    //                        else if (searchDto.Tindakan == Constants.TindakanName.KOTS)
    //                        {
    //                            result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.KPP, this.Activity)).Result;
    //                            if (!result.Success) successSendData = false;
    //                            result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.Kompaun, this.Activity)).Result;
    //                            if (!result.Success) successSendData = false;
    //                            result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.DataKes, this.Activity)).Result;
    //                            if (!result.Success) successSendData = false;
    //                            result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.Akuan_UpdateKompaun, this.Activity)).Result;
    //                            if (!result.Success) successSendData = false;
    //                        }
    //                        else if (searchDto.Tindakan == Constants.TindakanName.SiasatLanjut)
    //                        {
    //                            result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.KPP, this.Activity)).Result;
    //                            if (!result.Success) successSendData = false;
    //                            result = Task.Run(async () => await SendOnlineBll.SendDataOnlineAsync(searchDto.NoRujukan, Enums.TableType.DataKes, this.Activity)).Result;
    //                            if (!result.Success) successSendData = false;
    //                        }

    //                        GeneralAndroidClass.ShowToast(successSendData ? Constants.Messages.SuccessSendData : Constants.Messages.FaildSendData);
    //                        if (successSendData)
    //                            OnSearch();

    //                        _dialog?.Dismiss();

    //                    });
    //                }).Start();
    //            }
    //            catch (Exception ex)
    //            {
    //                GeneralAndroidClass.LogData(LayoutName, "ReSendDataAsync", ex.Message, Enums.LogType.Error);
    //                _dialog?.Dismiss();
    //                GeneralAndroidClass.ShowToast(Constants.Messages.FaildSendData);
    //            }

    //        }
    //    }
}