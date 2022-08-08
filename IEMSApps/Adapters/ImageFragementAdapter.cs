using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.App;
using Java.IO;

namespace IEMSApps.Adapters
{
    public class ImageFragementAdapter : FragmentStatePagerAdapter
    {

        List<File> fileImages;
        Context context;
        bool allowdelete;
        public ImageFragementAdapter(Context context, Android.Support.V4.App.FragmentManager fm, List<File> files, bool bDelete)
            : base(fm)
        {
            this.fileImages = files;
            this.context = context;
            this.allowdelete = bDelete;
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return new Adapters.ImageFragment(context, position, fileImages, allowdelete);
        }

        public override int Count
        {
            get { return fileImages.Count; }
        }

        public void Remove()
        {
            NotifyDataSetChanged();
        }
    }
}