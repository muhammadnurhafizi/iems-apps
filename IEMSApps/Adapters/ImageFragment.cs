using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using IEMSApps.Classes;
using Java.IO;

namespace IEMSApps.Adapters
{
    public class ImageFragment : Android.Support.V4.App.Fragment
    {

        private ImageView _imageView;
        private int _position;
        List<File> files;
        Context _context;
        bool bDelete;

        public ImageFragment() { }
        public ImageFragment(Context context, int position, List<File> files, bool allowdelete)
        {
            _position = position;
            RetainInstance = true;
            this.files = files;
            this._context = context;
            this.bDelete = allowdelete;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ImageFragment, container, false);
            view.Id = _position;

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;

            _imageView = view.FindViewById<ImageView>(Resource.Id.imageView);
            BitmapHelpers.LoadImage(_imageView, files[_position].Path);

            //ImageView iv = view.FindViewById<ImageView>(Resource.Id.imgIconRemoveImage);
            //if (!bDelete)
            //    iv.Visibility = ViewStates.Invisible;
            //iv.Click += (o, e) =>
            //{
            //    //var find = new Intent(_context, typeof(DetailCameraActivity));

            //    //find.PutExtra("locationImage", files[_position].Path);

            //    //StartActivity(find);
                
            //};
            return view;
        }



    }
}