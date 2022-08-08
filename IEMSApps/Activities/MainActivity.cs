using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Fragments;
using IEMSApps.Utils;

namespace IEMSApps.Activities
{

    [Activity(Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private TextView txtNoKp, txtName, txtVersion;
        private ImageView imageLogo, imgBtnHome;
        private TextView txtPageTitle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
            txtHhId.Text = GeneralBll.GetUserHandheld();

            imageLogo = FindViewById<ImageView>(Resource.Id.imageLogo);
            txtPageTitle = FindViewById<TextView>(Resource.Id.txtPageTitle);

            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            View header = navigationView.GetHeaderView(0);
            txtName = header.FindViewById<TextView>(Resource.Id.txtName);
            txtNoKp = header.FindViewById<TextView>(Resource.Id.txtNoKp);
            txtVersion = header.FindViewById<TextView>(Resource.Id.txtVersion);

            imgBtnHome = header.FindViewById<ImageView>(Resource.Id.imgBtnHome);
            imgBtnHome.Click += ImgBtnHome_Click;

            var linearLogout = FindViewById<LinearLayout>(Resource.Id.linearLogout);
            linearLogout.Click += LinearLogout_Click;

            //load default home screen
            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.content_frame, new Home());
            ft.Commit();

            //navigationView.SetCheckedItem(Resource.Id.nav_pasukan);
            var tbPasukanHh = DataAccessQuery<TbPasukanHh>.GetAll();

            var userId = GeneralBll.GetUserId();
            if (userId.ToLower() == Constants.AdminUserValue.ToLower())
                navigationView.Menu.GetItem(6).SetVisible(true);
            else
                navigationView.Menu.GetItem(6).SetVisible(false);

            SetInit();
        }

        private void LinearLogout_Click(object sender, EventArgs e)
        {
            try
            {
                OnLogout();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "LinearLogout_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void ImgBtnHome_Click(object sender, EventArgs e)
        {
            try
            {
                var ft = FragmentManager.BeginTransaction();
                ft.Replace(Resource.Id.content_frame, new Home());
                ft.Commit();

                DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                drawer.CloseDrawer(GravityCompat.Start);

                NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

                int size = navigationView.Menu.Size();
                for (int i = 0; i < size; i++)
                {
                    navigationView.Menu.GetItem(i).SetChecked(false);
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "SetInit", ex.Message, Enums.LogType.Error);
            }
        }

        private void SetInit()
        {
            try
            {
                if (txtName != null)
                {
                    txtName.Text = SharedPreferences.GetString(SharedPreferencesKeys.UserName);
                }

                if (txtNoKp != null)
                {
                    txtNoKp.Text = SharedPreferences.GetString(SharedPreferencesKeys.UserNoKp);
                }

                if (txtVersion != null)
                {
                    txtVersion.Text = Constants.AppVersion;
                }


                GeneralAndroidClass.StartLocationService(this);

                GlobalClass.printService = null;

            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "SetInit", ex.Message, Enums.LogType.Error);
            }
        }

        private void SetVisibleTitle(bool blValue)
        {
            if (blValue)
            {
                imageLogo.Visibility = ViewStates.Visible;
                txtPageTitle.Visibility = ViewStates.Visible;
            }
            else
            {
                imageLogo.Visibility = ViewStates.Invisible;
                txtPageTitle.Visibility = ViewStates.Invisible;
            }
        }
        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                //base.OnBackPressed();
                //OnLogout();

            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            try
            {
                int id = item.ItemId;
                Fragment fragment = null;

                var userId = GeneralBll.GetUserId();
                bool isAdmin = userId.ToLower() == Constants.AdminUserValue.ToLower();

                if (isAdmin)
                {
                    if (id == Resource.Id.nav_checkout)
                    {
                        fragment = new Checkout();
                    }
                    else if (id == Resource.Id.nav_muatturundata)
                    {
                        fragment = new DownloadData();
                    }
                    else if (id == Resource.Id.nav_semakan)
                    {
                        fragment = new Semakan();
                    }
                    else if (id == Resource.Id.nav_cleardatas)
                    {
                        fragment = new CleanData();
                    }
                }
                else
                {
                    if (id == Resource.Id.nav_pasukan)
                    {
                        //DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                        //drawer.CloseDrawer(GravityCompat.Start);


                        //var intent = new Intent(this, typeof(PasukanForm));
                        //StartActivity(intent);

                        fragment = new Fragments.Pasukan();
                    }
                    else if (id == Resource.Id.nav_pemeriksaan)
                    {
                        fragment = new Pemeriksaan();
                    }
                    else if (id == Resource.Id.nav_semakan)
                    {
                        fragment = new Semakan();
                    }
                    else if (id == Resource.Id.nav_summary)
                    {
                        fragment = new Fragments.Ringkasan();
                    }
                    else if (id == Resource.Id.nav_checkout)
                    {
                        fragment = new Checkout();
                    }
                    else if (id == Resource.Id.nav_muatturundata)
                    {
                        fragment = new DownloadData();
                    }
                }

                if (fragment != null)
                {

                    var ft = FragmentManager.BeginTransaction();
                    ft.Replace(Resource.Id.content_frame, fragment);
                    ft.Commit();


                }
                DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                drawer.CloseDrawer(GravityCompat.Start);


            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData(Title, "OnNavigationItemSelected", ex.Message, Enums.LogType.Error);
            }

            return true;
        }

        private void OnLogout()
        {

            var ad = GeneralAndroidClass.GetDialogCustom(this);

            ad.SetMessage("Daftar Keluar ?");
            // Positive

            ad.SetButton("Tidak", (s, ev) => { });
            ad.SetButton2("Ya", (s, ev) =>
            {
                GeneralBll.ClearUserData();
                GeneralAndroidClass.StopLocationService(this);
                GeneralAndroidClass.StopBackgroundService(this);

                GlobalClass.printService?.Stop();
                GlobalClass.printService = null;

                var intent = new Intent(this, typeof(Login));
                intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
                Finish();
            });
            ad.Show();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (GeneralBll.GetStatusGps() == Constants.GpsStatus.GpsOff)
            {
                GeneralAndroidClass.StartLocationService(this);
            }
        }

        public void SetMenuDrawer(bool enable)
        {
            int lockMode = enable ? DrawerLayout.LockModeUnlocked :
                DrawerLayout.LockModeLockedClosed;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            drawer.SetDrawerLockMode(lockMode);
            //toggle.setDrawerIndicatorEnabled(enabled);
        }
    }
}

