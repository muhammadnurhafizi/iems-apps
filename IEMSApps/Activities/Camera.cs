using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.Classes;
using IEMSApps.Utils;
using File = Java.IO.File;
using Android.Support.V4.App;
using IEMSApps.Adapters;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace IEMSApps.Activities
{
    [Activity(Label = "Camera", Theme = "@style/LoginTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Camera : FragmentActivity
    {
        ViewPager _imagePager;
        private TextView lblImageCount;
        private TextView lblFileName;

        private List<File> FileImages;

        private bool _viewMode = false;
        private bool _allowdelete = false;
        private string _filename;
        private string _dirImagePath;
        private bool _allowTakePicture;
        private Button btnCamera, btnReplace;
        private TextView lblBtnCamera;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.CameraLayout);
            
            var btnOk = FindViewById<Button>(Resource.Id.btnOk);
            btnOk.Click += BtnOk_Click;

            btnReplace = FindViewById<Button>(Resource.Id.btnReplace);
            btnReplace.Click += BtnReplace_Click;

            btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            lblBtnCamera = FindViewById<TextView>(Resource.Id.lblBtnCamera);

            btnCamera.Click += BtnCamera_Click;

            _imagePager = FindViewById<ViewPager>(Resource.Id.imagePager);

            lblImageCount = FindViewById<TextView>(Resource.Id.lblImageCount);
            //lblFileName = FindViewById<TextView>(Resource.Id.lblfilename);
            lblImageCount.Text = "Jumlah Gambar : 0";
            //lblFileName.Text = "";

            SetInit();
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileImages.Count == 0)
                {
                    GeneralAndroidClass.ShowModalMessage(this, Constants.ErrorMessages.NoPhotoExist);
                    return;
                }
             
                var ad = GeneralAndroidClass.GetDialogCustom(this);

                ad.SetMessage(Constants.Messages.ReplaceImageQuestion);
                // Positive

                ad.SetButton("Tidak", (s, ev) => { });
                ad.SetButton2("Ya", (s, ev) =>
                {
                    TakeAPictureUsePlugin(FileImages[_imagePager.CurrentItem].Path, true, _imagePager.CurrentItem);
                });
                ad.Show();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Camera", "BtnReplace_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
              this.Finish();
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Camera", "BtnOk_Click", ex.Message, Enums.LogType.Error);
            }
        }

        private void BtnCamera_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileImages.Count >= Constants.MaxPhoto)
                {
                    GeneralAndroidClass.ShowModalMessage(this, "Gambar Maksimum Telah Dicapai : " + Constants.MaxPhoto);
                }
                else
                {
                    TakeAPictureUsePlugin(_dirImagePath + GetFileName());
                }
            }
            catch (Exception ex)
            {
                GeneralAndroidClass.LogData("Camera", "BtnCamera_Click", ex.Message, Enums.LogType.Error);
            }

        }

      
        private void SetViewPager()
        {
            lblImageCount.Text = "Jumlah Gambar : " + FileImages.Count;

            _imagePager.Adapter = new ImageFragementAdapter(this,  SupportFragmentManager, FileImages, _allowdelete);

        }

        private string GetFileName()
        {
            return _filename + (FileImages.Count + 1).ToString("00") + Constants.ImageExtension;
        }

        private void SetInit()
        {
            try
            {
                var txtHhId = FindViewById<TextView>(Resource.Id.txtHhId);
                txtHhId.Text = GeneralBll.GetUserHandheld();

                _dirImagePath = GeneralBll.GetInternalImagePath();
              
                _viewMode = Intent.GetBooleanExtra("viewMode", false);
                _allowdelete = Intent.GetBooleanExtra("deletemode", true);
                _filename = Intent.GetStringExtra("filename");
                _allowTakePicture = Intent.GetBooleanExtra("allowtakepicture", true);

                var allowReplace = Intent.GetBooleanExtra("allowreplace", true);

                FileImages = new List<File>();

                SetFileImages();

                SetViewPager();

                if (!_allowTakePicture)
                {
                    btnCamera.Enabled = false;
                    btnCamera.SetBackgroundResource(Resource.Drawable.camera_disable);
                }

                if (!allowReplace)
                {
                    btnReplace.Enabled = false;
                    btnReplace.SetBackgroundResource(Resource.Drawable.iconreplace_disable);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogFile(Title, "SetInit", ex.Message, Enums.LogType.Error);
                GeneralAndroidClass.ShowToast(ex.Message);
            }
        }

        private void SetFileImages()
        {
            DirectoryInfo di = new DirectoryInfo(_dirImagePath);
            FileInfo[] rgFiles = di.GetFiles(_filename + "*.*");

            foreach (var fileInfo in rgFiles)
            {
                FileImages.Add(new File(fileInfo.FullName));
            }
        }

        private async Task TakeAPictureUsePlugin(string fullFileName, bool isReplace = false, int position = 0)
        {
            try
            {
                //StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
                //StrictMode.SetVmPolicy(builder.Build());

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "",
                    Name = fullFileName,
                    PhotoSize = PhotoSize.Custom,
                    CustomPhotoSize = 30,
                    CompressionQuality = 30

                });

                if (file == null)
                {
                    return;
                }
                
                BitmapHelpers.SaveImageStream(file.GetStream(), fullFileName);

                file.Dispose();

                File newFile = new File(fullFileName);
                if (newFile.Exists())
                {
                    if (isReplace)
                    {
                        FileImages[position] = newFile;
                    }
                    else
                    {
                        FileImages.Add(newFile);
                    }
                   
                }
                SetViewPager();

            }
            catch (Exception ex)
            {
                Log.WriteLogFile(Title, "TakeAPictureUsePlugin", ex.Message, Enums.LogType.Error);
                GeneralAndroidClass.ShowToast(ex.Message);
            }
        }

        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    try
        //    {

        //        base.OnActivityResult(requestCode, resultCode, data);

        //        if (resultCode == Result.Ok)
        //        {
        //            var fullFileName = _dirImagePath + GetFileName();

        //            BitmapHelpers.SaveCompressImage(fullFileName);
        //            File newFile = new File(fullFileName);
        //            if (newFile.Exists())
        //            {
        //                FileImages.Add(newFile);
        //            }
        //            SetViewPager();
        //        }
        //        else
        //        {
        //            Finish();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.WriteLogFile(Title, "OnActivityResult", ex.Message, Enums.LogType.Error);
        //        GeneralAndroidClass.ShowToast(ex.Message);
        //    }
        //}

    }
}