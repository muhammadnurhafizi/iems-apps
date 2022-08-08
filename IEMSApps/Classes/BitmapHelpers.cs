using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;
using Android.Widget;
using IEMSApps.BLL;
using IEMSApps.Utils;
using Stream = System.IO.Stream;

namespace IEMSApps.Classes
{
    public static class BitmapHelpers
    {
        public const int ORIENTATION_NORMAL = 1;          //(0x00000001)
        public const int ORIENTATION_FLIP_HORIZONTAL = 2; //(0x00000002)
        public const int ORIENTATION_ROTATE_180 = 3;      //(0x00000003)
        public const int ORIENTATION_FLIP_VERTICAL = 4;   //(0x00000004)
        public const int ORIENTATION_TRANSPOSE = 5;       //(0x00000005)
        public const int ORIENTATION_ROTATE_90 = 6;       //(0x00000006)
        public const int ORIENTATION_TRANSVERSE = 7;      //(0x00000007)
        public const int ORIENTATION_ROTATE_270 = 8;      //(0x00000008)
        public const int ORIENTATION_UNDEFINED = 0; 	  //(0x00000000) 

        /// <summary>
        /// Load the image from the device, and resize it to the specified dimensions.
        /// </summary>
        /// <returns>The and resize bitmap.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InPurgeable = true,
                InJustDecodeBounds = true
            };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;


            if (outHeight > height || outWidth > width)
            {
                int halfHeight = outHeight / 2;
                int halfWidth = outWidth / 2;
                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) >= height
                       && (halfWidth / inSampleSize) >= width)
                {
                    inSampleSize *= 2;
                }

            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            // Images are being saved in landscape, so rotate them back to portrait if they were taken in portrait
            Matrix mtx = new Matrix();
            ExifInterface exif = new ExifInterface(fileName);
            int orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, ORIENTATION_UNDEFINED);

            Matrix matrix = new Matrix();
            switch (orientation)
            {
                case ORIENTATION_NORMAL:
                    break;
                case ORIENTATION_FLIP_HORIZONTAL:
                    matrix.SetScale(-1, 1);
                    break;
                case ORIENTATION_ROTATE_180:
                    matrix.SetRotate(180);
                    break;
                case ORIENTATION_FLIP_VERTICAL:
                    matrix.SetRotate(180);
                    matrix.PostScale(-1, 1);
                    break;
                case ORIENTATION_TRANSPOSE:
                    matrix.SetRotate(90);
                    matrix.PostScale(-1, 1);
                    break;
                case ORIENTATION_ROTATE_90:
                    matrix.SetRotate(90);
                    break;
                case ORIENTATION_TRANSVERSE:
                    matrix.SetRotate(-90);
                    matrix.PostScale(-1, 1);
                    break;
                case ORIENTATION_ROTATE_270:
                    matrix.SetRotate(-90);
                    break;
                default:
                    break;
            }

            try
            {
                //Bitmap bmRotated = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
                resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);
            }
            catch (Exception ex)
            {
                GeneralBll.LogDataWithException("BitmapHelpers", "LoadAndResizeBitmap", ex);
            }

            return resizedBitmap;
        }

        //Load Image Original size
        public static Bitmap LoadBitmap(this string fileName)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InPurgeable = true,
                InSampleSize = 1,
                InJustDecodeBounds = false
            };

            Bitmap MyBitmap = BitmapFactory.DecodeFile(fileName, options);

            return MyBitmap;
        }


        #region New LoadAndReizeImage
        internal static async void LoadImage(ImageView imageView, string pathImage)
        {
            BitmapFactory.Options options = await BitmapHelpers.GetBitmapOptionsOfImageAsync(pathImage);
            Bitmap bitmapToDisplay = await BitmapHelpers.LoadScaledDownBitmapForDisplayAsync(pathImage, options, 300, 300);
            imageView.SetImageBitmap(bitmapToDisplay);
        }

        internal static async Task<BitmapFactory.Options> GetBitmapOptionsOfImageAsync(string pathImage)
        {
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            // The result will be null because InJustDecodeBounds == true.
            //Bitmap result = await BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.samoyed, options);
            Bitmap result = await BitmapFactory.DecodeFileAsync(pathImage, options);

            int imageHeight = options.OutHeight;
            int imageWidth = options.OutWidth;

            //_originalDimensions.Text = string.Format("Original Size= {0}x{1}", imageWidth, imageHeight);

            return options;
        }

        internal static async Task<Bitmap> LoadScaledDownBitmapForDisplayAsync(string pathImage, BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;

            return await BitmapFactory.DecodeFileAsync(pathImage, options);
        }


        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = options.OutHeight;
            float width = options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = (int)(height / 2);
                int halfWidth = (int)(width / 2);

                // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return (int)inSampleSize;
        }

        #endregion

        public static void SaveCompressImage(string path)
        {
            //rename for copy file
            string pathCopy = path.Replace(Constants.ImageExtension, "Copy");
            pathCopy += Constants.ImageExtension;

            //copy first 
            File.Copy(path, pathCopy, true);

            #region Images are being saved in landscape, so rotate them back to portrait if they were taken in portrait
            Matrix mtx = new Matrix();
            ExifInterface exif = new ExifInterface(pathCopy);
            int orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, ORIENTATION_UNDEFINED);

            Matrix matrix = new Matrix();
            switch (orientation)
            {
                case ORIENTATION_NORMAL:
                    break;
                case ORIENTATION_FLIP_HORIZONTAL:
                    matrix.SetScale(-1, 1);
                    break;
                case ORIENTATION_ROTATE_180:
                    matrix.SetRotate(180);
                    break;
                case ORIENTATION_FLIP_VERTICAL:
                    matrix.SetRotate(180);
                    matrix.PostScale(-1, 1);
                    break;
                case ORIENTATION_TRANSPOSE:
                    matrix.SetRotate(90);
                    matrix.PostScale(-1, 1);
                    break;
                case ORIENTATION_ROTATE_90:
                    matrix.SetRotate(90);
                    break;
                case ORIENTATION_TRANSVERSE:
                    matrix.SetRotate(-90);
                    matrix.PostScale(-1, 1);
                    break;
                case ORIENTATION_ROTATE_270:
                    matrix.SetRotate(-90);
                    break;
                default:
                    break;
            }
            #endregion Images are being saved in landscape, so rotate them back to portrait if they were taken in portrait

            #region Resize, rotate & compress with 50% clearity
            try
            {
                //compress copy file
                //Bitmap resizedBitmap = pathCopy.LoadAndResizeBitmap(480, 640);
                Bitmap OriBitmap = pathCopy.LoadBitmap();
                Bitmap resizedBitmap = Bitmap.CreateScaledBitmap(OriBitmap, (int)(OriBitmap.Width / 2), (int)(OriBitmap.Height / 2), false);
                resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, matrix, false);

                var stream = new FileStream(pathCopy, FileMode.Create);
                resizedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("GeneralBll, Function: SaveCompressImage", Enums.LogType.Error);
                Log.WriteLogFile("Message : " + ex.Message, Enums.LogType.Error);
            }
            finally
            {
                GC.Collect();
            }
            #endregion Resize, rotate & compress with 50% clearity

            //check file size
            var length = new System.IO.FileInfo(pathCopy).Length;
            if (length > 0)
            {
                //copy back original
                File.Copy(pathCopy, path, true);
            }

            //remove copy file
            if (File.Exists(pathCopy))
            {
                File.Delete(pathCopy);
            }
        }

        public static void SaveImageStream(Stream streamFile, string fileName)
        {
            var bitmap = BitmapFactory.DecodeStream(streamFile);

            var stream = new FileStream(fileName, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            stream.Close();
            stream.Dispose();
        }
    }
}