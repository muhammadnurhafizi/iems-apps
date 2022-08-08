using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Inputs;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.Activities
{
    [Activity(Label = "Pasukan", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class PasukanForm : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        
            SetContentView(Resource.Layout.PasukanLayout);

            SetInit();
        }

        private List<PasukanAhliDto> _listPasukanAhli;
        private ListView _lvResult;

        private void SetInit()
        {
            try
            {
                //var pasukan = PasukanBll.GetPasukanDetailUser();

                var txtIdPasukan = FindViewById<TextView>(Resource.Id.txtIdPasukan);
                var txtKetuaPasukan = FindViewById<TextView>(Resource.Id.txtKetuaPasukan);

                txtIdPasukan.Text = GeneralBll.GetKodPasukanUser();

                txtKetuaPasukan.Text = GeneralBll.GetKetuaPasukanName();

                _lvResult = FindViewById<ListView>(Resource.Id.lView);


                LoadListPasukanAhli();

                var imgAdd = FindViewById<ImageView>(Resource.Id.imgAdd);
                imgAdd.Click += ImgAdd_Click;
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pasukan", "SetInit", ex.Message, Enums.LogType.Error);
            }

        }

        private void LoadListPasukanAhli()
        {
            _listPasukanAhli = PasukanBll.GetListPasukanAhliByUser();

            _lvResult.Adapter = new PasukanAhliAdapter(this, _listPasukanAhli);
            _lvResult.FastScrollEnabled = true;
        }
        private void ImgAdd_Click(object sender, System.EventArgs e)
        {
            try
            {
                OnAddPasukanAhli();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Pasukan", "ImgAdd_Click", ex.Message, Enums.LogType.Error);
            }
        }

        AutoCompleteTextView actCarian;
        RadioButton rbCawanganIni, rbCawanganLain;
        TextView lblNamaValue, lblNoKPValue, lblNegeriValue, lblCawanganValue;


        AutoCompleteTextView actNegeri, actCawangan, actPenguatkuasaan;
        Dictionary<string, string> listNegeri = PasukanBll.GetAllNegeri();
        Dictionary<string, string> listCawangan = new Dictionary<string, string>();
        Dictionary<string, string> listPengguna = new Dictionary<string, string>();
        EditText txtNoKp;
        Button btnOk;
        private void OnAddPasukanAhli()
        {
            //var intent = new Intent(this.Activity, typeof(Camera));
            //intent.PutExtra("filename", DateTime.Now.ToString("yyyyMMddHHmm"));
            //StartActivity(intent);

            //return;
            var builder = new AlertDialog.Builder(this).Create();
            var view = this.LayoutInflater.Inflate(Resource.Layout.TambahAhliPasukanModal, null);
            builder.SetView(view);

            actCarian = view.FindViewById<AutoCompleteTextView>(Resource.Id.actCarian);
            actCarian.Threshold = 0;

            actCarian.ItemClick += ActCarian_ItemClick;

            rbCawanganIni = view.FindViewById<RadioButton>(Resource.Id.rbCawanganIni);
            rbCawanganLain = view.FindViewById<RadioButton>(Resource.Id.rbCawanganLain);

            rbCawanganIni.Click += (send, args) =>
            {
                SetAutoCompliteAdapter(true);
            };
            rbCawanganLain.Click += (send, args) =>
            {
                SetAutoCompliteAdapter(false);
            };

            lblNamaValue = view.FindViewById<TextView>(Resource.Id.lblNamaValue);
            lblNoKPValue = view.FindViewById<TextView>(Resource.Id.lblNoKPValue);
            lblNegeriValue = view.FindViewById<TextView>(Resource.Id.lblNegeriValue);
            lblCawanganValue = view.FindViewById<TextView>(Resource.Id.lblCawanganValue);

            btnOk = view.FindViewById<Button>(Resource.Id.btnOk);
            var btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);

            SetAutoCompliteAdapter(true);
            UpdateImageAddModal(lblNoKPValue.Text, lblNegeriValue.Text, lblCawanganValue.Text, lblNamaValue.Text);

            btnOk.Click += (sent, args) =>
            {
                try
                {

                    var input = new PasukanAhliInput
                    {
                        SelectedCawangan = lblCawanganValue.Text,
                        NoKp = lblNoKPValue.Text
                    };

                    var response = PasukanBll.AddPasukanAhli(input);
                    if (response.Success)
                    {
                        GeneralAndroidClass.ShowModalMessage(this, Constants.Messages.SuccessSavePasukan);
                        LoadListPasukanAhli();
                    }
                    else
                    {
                        var _alertDialog = GeneralAndroidClass.GetDialogCustom(this);
                        if (response.Datas.Status == Constants.Status.Aktif)
                        {
                            _alertDialog.SetMessage(Constants.ErrorMessages.PasukanExist);
                            _alertDialog.SetButton2("OK", (s, e) => { });
                        }
                        else if (response.Datas.Status == Constants.Status.TidakAktif)
                        {
                            _alertDialog.SetMessage("Aktifkan semula ID ini?");
                            _alertDialog.SetButton("Ya", (s, e) =>
                            {
                                PasukanBll.UpdateStatusAktifPasukan(response.Datas.NoKp);
                                LoadListPasukanAhli();
                            });
                            _alertDialog.SetButton2("Tidak", (s, e) => { });
                        }
                        _alertDialog.Show();
                    }


                }
                catch (Exception ex)
                {
                    GeneralAndroidClass.LogData("AddPasukan", "btnOk.Click", ex.Message, Enums.LogType.Error);
                }


                builder.Dismiss();
            };
            btnCancel.Click += (sent, args) =>
            {
                builder.Dismiss();
            };



            builder.Show();
        }

        private void CleanText()
        {
            lblNamaValue.Text = string.Empty;
            lblNoKPValue.Text = string.Empty;
            lblNegeriValue.Text = string.Empty;
            lblCawanganValue.Text = string.Empty;
            actCarian.Text = string.Empty;
        }

        private void ActCarian_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var pengguana = PenggunaBll.GetPenggunaByName(actCarian.Text);
            if (pengguana == null)
                CleanText();
            else
            {
                lblNamaValue.Text = actCarian.Text;
                lblNoKPValue.Text = pengguana.NoKp;
                //var negeri = NegeriBll.GetPenggunaByID(pengguana?.IdNegeri);
                //lblNegeriValue.Text = ""; //negeri?.Prgn;
                var cawangan = CawanganBll.GetPenggunaByID(pengguana?.KodCawangan);
                lblCawanganValue.Text = cawangan?.Prgn;

                lblNegeriValue.Text = "";
                if (cawangan != null)
                {
                    var negeri = NegeriBll.GetNegeriByKodNegeri(GeneralBll.ConvertStringToInt(cawangan.KodNegeri));
                    lblNegeriValue.Text = negeri?.Prgn;
                }

            }

            UpdateImageAddModal(lblNoKPValue.Text, lblNegeriValue.Text, lblCawanganValue.Text, lblNamaValue.Text);
        }

        private void SetAutoCompliteAdapter(bool basedCawanganUser)
        {
            CleanText();
            var listPengguna = PenggunaBll.GetNamaPengguna(basedCawanganUser);
            actCarian.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, listPengguna);

            UpdateImageAddModal(lblNoKPValue.Text, lblNegeriValue.Text, lblCawanganValue.Text, lblNamaValue.Text);
        }

        private void UpdateImageAddModal(string txtNoKp, string spKodeNegeri, string spKodCawangan, string spPenguatKuasa)
        {

            if (txtNoKp.Length > 0 && !string.IsNullOrEmpty(spKodeNegeri) && !string.IsNullOrEmpty(spKodCawangan) && !string.IsNullOrEmpty(spPenguatKuasa))
            {
                btnOk.Enabled = true;
                btnOk.SetBackgroundResource(Resource.Drawable.save_icon);
            }
            else
            {
                btnOk.Enabled = false;
                btnOk.SetBackgroundResource(Resource.Drawable.save_icon_disabled);
            }
        }

        public void RemovePasukanAhli(int kodPasukan, string noKp)
        {
            if (noKp != SharedPreferences.GetString(SharedPreferencesKeys.UserNoKp))
                ShowHapusPasukan(noKp);
            else
            {
                var ad = GeneralAndroidClass.GetDialogCustom(this);

                ad.SetMessage("ID Pengguna sedang digunakan");
                ad.SetButton("OK", (s, ev) => { });
                ad.Show();
            }
        }

        private void ShowHapusPasukan(string noKp)
        {
            var pasukan = PasukanBll.GetPasukanByNoKp(noKp);

            if (pasukan == null) return;

            var builder = new AlertDialog.Builder(this).Create();
            var view = this.LayoutInflater.Inflate(Resource.Layout.HapusAlihPasukanModal, null);
            builder.SetView(view);


            var btnOk = view.FindViewById<Button>(Resource.Id.btnOk);
            var btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
            var txtNoKPValue = view.FindViewById<TextView>(Resource.Id.txtNoKPValue);
            var txtNameValue = view.FindViewById<TextView>(Resource.Id.txtNameValue);
            var txtAlasan = view.FindViewById<TextView>(Resource.Id.txtAlasan);
            txtAlasan.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(250) });

            btnOk.Enabled = false;
            btnOk.SetBackgroundResource(Resource.Drawable.save_icon_disabled);

            txtNoKPValue.Text = pasukan.NoKp;
            txtNameValue.Text = LoginBll.GetNamaPenggunaByNoKp(pasukan.NoKp);
            txtAlasan.Text = "";

            txtAlasan.TextChanged += (sent, args) =>
            {
                if (string.IsNullOrEmpty(txtAlasan.Text))
                {
                    btnOk.Enabled = false;
                    btnOk.SetBackgroundResource(Resource.Drawable.save_icon_disabled);
                }
                else
                {
                    btnOk.Enabled = true;
                    btnOk.SetBackgroundResource(Resource.Drawable.save_icon);
                }
            };

            btnOk.Click += (sent, args) =>
            {
                try
                {

                    var ad = GeneralAndroidClass.GetDialogCustom(this);

                    ad.SetMessage("Hapus ahli pasukan " + txtNameValue.Text + " ? ");
                    // Positive

                    ad.SetButton("Tidak", (s, ev) => { });
                    ad.SetButton2("Ya", (s, ev) =>
                    {
                        if (PasukanBll.RemovePasukanByNoKp(txtNoKPValue.Text, txtAlasan.Text))
                        {
                            LoadListPasukanAhli();
                            GeneralAndroidClass.ShowToast("Berjaya");
                        }
                        else
                        {
                            GeneralAndroidClass.ShowToast("Gagal");
                        }

                    });
                    ad.Show();

                }
                catch (Exception ex)
                {
                    GeneralAndroidClass.LogData("AddPasukan", "btnOk.Click", ex.Message, Enums.LogType.Error);
                }


                builder.Dismiss();
            };
            btnCancel.Click += (sent, args) =>
            {
                builder.Dismiss();
            };



            builder.Show();
        }

        private void ActNegeri_Click(object sender, EventArgs e)
        {
            if (!actNegeri.IsPopupShowing)
                actNegeri.ShowDropDown();
        }

        private void SetCawanganDefaultAndAdapter()
        {
            actCawangan.Text = "";
            var userCawangan = PasukanBll.GetCawanganUser();

            var selectedNegeri = GeneralBll.GetKeySelected(listNegeri, actNegeri.Text);

            listCawangan = PasukanBll.GetCawanganByNegeri(selectedNegeri);
            var displayCawangan = listCawangan.Select(c => c.Value).ToList();

            actCawangan.Adapter = new ArrayAdapter<string>(this,
                Resource.Layout.support_simple_spinner_dropdown_item, displayCawangan);

            actCawangan.SetText(listCawangan.SingleOrDefault(m => m.Key == userCawangan).Value, true);
            if (string.IsNullOrEmpty(actCawangan.Text))
                if (listCawangan.Any()) actCawangan.SetText(listCawangan.FirstOrDefault().Value, true);

            if (!string.IsNullOrEmpty(actCawangan.Text))
                SetPenguatKuasaDefaultAndAdapter();
        }

        private void SetPenguatKuasaDefaultAndAdapter()
        {
            actPenguatkuasaan.Text = "";
            var selectedCawangan = GeneralBll.GetKeySelected(listCawangan, actCawangan.Text);

            listPengguna = PasukanBll.GetPenguatKuasaByCawangan(selectedCawangan);
            actPenguatkuasaan.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, listPengguna.Select(c => c.Value).ToList());
            if (listPengguna.Any())
                actPenguatkuasaan.SetText(listPengguna.FirstOrDefault().Value, true);

            if (!string.IsNullOrEmpty(actPenguatkuasaan.Text))
                SetNoKP();
            UpdateImageAddModal(txtNoKp.Text, actNegeri.Text, actCawangan.Text, actPenguatkuasaan.Text);
        }

        private void SetNoKP()
        {
            txtNoKp.Text = "";
            txtNoKp.Text = listPengguna.FirstOrDefault(c => c.Value == actPenguatkuasaan.Text).Key;
            UpdateImageAddModal(txtNoKp.Text, actNegeri.Text, actCawangan.Text, actPenguatkuasaan.Text);
        }
    }
}