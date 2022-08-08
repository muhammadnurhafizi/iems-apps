using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using IEMSApps.Adapters;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Services;
using IEMSApps.Utils;
using Timer = System.Timers.Timer;

namespace IEMSApps.Activities
{

    [Activity(Label = "Login", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Login : BaseActivity
    {
        private EditText txtUser, txtPasswd;
        private TextView txtDate, txtTime;
        private bool _isCheckIn;
        // private AlertDialog _dialog;
        private HourGlassClass _hourGlass = new HourGlassClass();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            txtUser = FindViewById<EditText>(Resource.Id.txtUserName);
            txtPasswd = FindViewById<EditText>(Resource.Id.txtPassword);

            //txtUser.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(10) });
            //txtPasswd.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(10) });

            txtDate = FindViewById<TextView>(Resource.Id.txtDate);
            txtTime = FindViewById<TextView>(Resource.Id.txtTime);

            var btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            btnLogin.Click += BtnLogin_Click;

            var imgMaklumat = FindViewById<ImageView>(Resource.Id.imgMaklumat);
            imgMaklumat.Click += ImgMaklumat_Click;

            var btnCheckin = FindViewById<Button>(Resource.Id.btnCheckin);
            btnCheckin.Click += BtnCheckin_Click;

            //_dialog = GeneralAndroidClass.ShowProgressDialog(this, Constants.Messages.WaitingPlease);
            //new Thread(() =>
            //{

            //    Thread.Sleep(Constants.DefaultWaitingMilisecond);
            //    this.RunOnUiThread(SetInit);
            //}).Start();
            SetInit();

#if DEBUG
            txtUser.Text = "750730105115";// "870208025493";730315016109;830724035085;//750730105115
            txtPasswd.Text = "750730105115";

            //var resultData = Task.Run(async () => await HttpClientService.GetListJpnDetail("OTYwNTEzMDM1MjQw")).Result;
            var imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            imageView.Click += ImageView_Click;

            //new PrintImageBll().Kompaun(this, "KTSKCH0032000001");
            //var result = Task.Run(async () => await HttpClientService.UploadFileAsync(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Constants.ProgramPath +
            //                                   Constants.DatabasePath + Constants.DatabaseName)).Result;
#endif

        }

        private void ImageView_Click(object sender, EventArgs e)
        {
            // _isTindakanClick = true;
            ShowMaklumatModal();
        }

        private void SetDisableEditText(EditText data)
        {
            data.SetBackgroundResource(Resource.Drawable.textView_bg);
            data.Enabled = false;
        }

        private void BtnCheckin_Click(object sender, EventArgs e)
        {
            try
            {
                CheckNewVersion(OnCheckIn);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "BtnCheckin_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void ImgMaklumat_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMaklumatModal();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "ImgMaklumat_Click", ex.Message, Enums.LogType.Error);
            }
        }



        private void ShowMaklumatModal()
        {
            var dialogView = View.Inflate(this, Resource.Layout.MaklumatLayout, null);
            var alertDialog = new AlertDialog.Builder(this).Create();

            var lvResult = dialogView.FindViewById<ListView>(Resource.Id.lView);

            var listMaklumatPasukan = LoginBll.ListMaklumatLogin();

            lvResult.Adapter = new MaklumatPasukanAdapter(this, listMaklumatPasukan);
            lvResult.FastScrollEnabled = true;

            var btnClose = dialogView.FindViewById<Button>(Resource.Id.btnClose);
            btnClose.Click += (sender, e) =>
            {
                alertDialog.Dismiss();
            };

            alertDialog.SetView(dialogView);
            alertDialog.Show();

        }
        private void SetInit()
        {
            try
            {
                Log.WriteLogFile(Constants.AppVersion, Enums.LogType.Info);
                var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
                txtHhId.Text = "";

                var handheld = HandheldBll.GetActiveHandheld();
                if (handheld != null)
                {
                    txtHhId.Text = handheld.IdHh;
                    SharedPreferences.SaveString(SharedPreferencesKeys.UserHandheld, handheld.IdHh);
                    SharedPreferences.SaveString(SharedPreferencesKeys.UserKodCawangan, handheld.KodCawangan);
                }
                //var idHh = HandheldBll.GetHandheldIdHh();
                //SharedPreferences.SaveString(SharedPreferencesKeys.UserHandheld, idHh);

                //var kodCawangan = HandheldBll.GetHandheldCawangan();
                //SharedPreferences.SaveString(SharedPreferencesKeys.UserKodCawangan, kodCawangan);

                var lblVersion = FindViewById<TextView>(Resource.Id.lblVersion);
                lblVersion.Text = Constants.AppVersion;

                var lblBattery = FindViewById<TextView>(Resource.Id.lblBattery);
                lblBattery.Text = "Bateri 100%";

                var iBatt = GeneralAndroidClass.GetBatteryPercentage();
                if (iBatt > 0)
                {
                    lblBattery.Text = $"Bateri {iBatt}%";
                }

                UpdateTime();

                var timer = new Timer();
                timer.Interval = 1000;
                timer.Elapsed += Timer_Elapsed; ;
                timer.Start();

                var linearInfo = FindViewById<LinearLayout>(Resource.Id.linearInfo);
                var linearCheckin = FindViewById<LinearLayout>(Resource.Id.linearCheckin);

                //var pasukan = LoginBll.GetInfoPasukan();
                var pasukan = PasukanBll.GetPasukanKetua();

                if (pasukan != null)
                {
                    SharedPreferences.SaveString(SharedPreferencesKeys.KetuaPasukanName, pasukan.Nama);

                    _isCheckIn = false;

                    linearInfo.Visibility = ViewStates.Visible;
                    linearCheckin.Visibility = ViewStates.Gone;

                    FindViewById<TextView>(Resource.Id.lblIdPasukan).Text = pasukan.KodPasukan.ToString();
                    FindViewById<TextView>(Resource.Id.lblKetuaPasukan).Text = pasukan.Nama;
                    //FindViewById<TextView>(Resource.Id.lblKetuaPasukan).Text =
                    //    LoginBll.GetNamaPenggunaByNoKp(pasukan.NoKp);
                }
                else
                {
                    _isCheckIn = true;
                    linearInfo.Visibility = ViewStates.Gone;
                    linearCheckin.Visibility = ViewStates.Visible;
                    //SetDisableEditText(txtUser);
                    //SetDisableEditText(txtPasswd);
                }

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "SetInit", ex.Message, Enums.LogType.Error);
            }
            // _dialog?.Dismiss();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                RunOnUiThread(UpdateTime);
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "Timer_Elapsed", ex.Message, Enums.LogType.Error);
            }

        }

        private void UpdateTime()
        {
            var localDate = GeneralBll.GetLocalDateTime();
            txtDate.Text = localDate.ToString(Constants.DateFormatDisplay);
            txtTime.Text = localDate.ToString(Constants.TimeFormatDisplay);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                //var message = string.Format(Constants.Messages.KompaunIzinApproved, "Lorem Ipsum is simply dummy text of the printing and typesetting");
                //GeneralAndroidClass.ShowModalMessageHtml(this, message);
                //return;
                //var result = Task.Run(async () => await HttpClientService.CheckKompaunIzin("KPPKCH0032000013")).Result;
                //GeneralAndroidClass.ShowToast("message " + result.Mesage);
                //return;
                //var message = string.Format(Constants.Messages.KompaunIzinDenied, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure");
                //GeneralAndroidClass.ShowModalMessage(this, message);

                ////DatabaseBll.TestBeginTrx();
                ////GeneralAndroidClass.ShowToast("done");
                //return;

                if (txtUser.Text == Constants.AdminUserValue || txtPasswd.Text == Constants.AdminPasswordValue)
                {
                    if (GeneralAndroidClass.IsGpsEnable())
                    {
                        _hourGlass?.StartMessage(this, OnLogin);
                    }
                    else
                    {
                        OnDialogGps();
                    }
                }
                else
                {
                    if (_isCheckIn)
                    {
                        GeneralAndroidClass.ShowModalMessage(this, Constants.ErrorMessages.NeedCheckIn);
                    }
                    else
                    {
                        if (GeneralAndroidClass.IsGpsEnable())
                        {
                            _hourGlass?.StartMessage(this, OnLogin);
                        }
                        else
                        {
                            OnDialogGps();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "BtnLogin_Click", ex.Message, Enums.LogType.Error);
            }

        }

        private void OnCheckIn()
        {
            Intent intent = new Intent(this, typeof(LoadingActivity));
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            intent.PutExtra(Constants.PIRANTIID, GeneralBll.GetUserHandheld());
            intent.PutExtra(Constants.LOADING_TYPE, Constants.LOADING_PREPARE_DOWNLOAD_DATA);

            StartActivity(intent);
        }

        private void OnDialogStay(string message)
        {
            //var ad = GeneralAndroidClass.GetDialogCustom(this);
            //ad.SetMessage(message);
            //ad.SetButton2("OK", (s, e) => { });
            //ad.Show();

        }

        private void OnDialogGps()
        {
            var ad = GeneralAndroidClass.GetDialogCustom(this);
            ad.SetMessage(Constants.Messages.TurnOnGpsMessage);
            // Positive

            ad.SetButton2(Constants.Messages.No, (s, ev) =>
            {

            });
            ad.SetButton(Constants.Messages.GPSSetting, (s, ev) =>
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                StartActivity(intent);
            });
            ad.Show();
        }

        private void ShowInvalidLogin()
        {
            GeneralAndroidClass.ShowModalMessage(this, Constants.ErrorMessages.InvalidLogin);
            txtUser.Text = "";
            txtPasswd.Text = "";

            txtUser.RequestFocus(FocusSearchDirection.Left);
        }

        private void OnLogin()
        {

            if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtPasswd.Text))
            {
                ShowInvalidLogin();
            }
            else
            {
                var userLogin = LoginBll.GetUserLogin(txtUser.Text, txtPasswd.Text);
                if (userLogin != null)
                {
                    HandheldBll.CheckRunningZeroYear();

                    SharedPreferences.SaveString(SharedPreferencesKeys.UserId, txtUser.Text);
                    SharedPreferences.SaveString(SharedPreferencesKeys.UserName, userLogin.Nama);
                    SharedPreferences.SaveString(SharedPreferencesKeys.UserNoKp, userLogin.NoKp);
                    SharedPreferences.SaveString(SharedPreferencesKeys.UserKodPasukan, userLogin.KodPasukan.ToString());
                    //SharedPreferences.SaveString(SharedPreferencesKeys.UserKodCawangan, userLogin.KodCawangan);
                    SharedPreferences.SaveString(SharedPreferencesKeys.UserNegeri, userLogin.KodNegeri.ToString());

                    SharedPreferences.SaveString(SharedPreferencesKeys.UserStaffld, userLogin.StaffId.ToString());

                    SharedPreferences.SaveString(SharedPreferencesKeys.SaveStatePemeriksaan, "");
                    //SharedPreferences.SaveString(SharedPreferencesKeys.IsUserLogin, "1");


                    //#if DEBUG
                    //                    RunBackgroundService();
                    //#else
                    //                    RunBackgroundService();
                    //#endif

#if DEBUG
                    //var input = new TbKompaun();
                    //input.NoRujukanKpp = "KOTS11";
                    //KompaunBll.SaveKompaunTrx(input);
#endif
                    ShowMain();
                }
                else
                {
                    ShowInvalidLogin();
                }

            }
            _hourGlass?.StopMessage();
        }

        private void ShowMain()
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
            Finish();
        }

        private void RunBackgroundService()
        {
            GeneralAndroidClass.StartBackgroundService(this);
        }
    }
}