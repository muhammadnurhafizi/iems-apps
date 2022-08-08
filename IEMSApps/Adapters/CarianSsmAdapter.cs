using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;

namespace IEMSApps.Adapters
{
    public class CarianSsmAdapter : BaseAdapter<DataSsmDto>
    {
        private readonly List<DataSsmDto> _items;
        private readonly Activity _context;

        public CarianSsmAdapter(Activity activity, List<DataSsmDto> premisDtos)
        {
            _context = activity;
            _items = premisDtos;
        }

        public override DataSsmDto this[int position] => _items[position];

        public override int Count => _items?.Count ?? 0;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder vh;

            // attempt to recycle existing view
            var view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.CarianSsmItem, parent, false) as LinearLayout;
                vh = new ViewHolder();

                // here's where we get our subview references
                vh.Initialize(view);

                // push the viewholder reference into the view tag
                view.Tag = vh;
            }

            // get our data object
            var dataObject = _items[position];

            // get our viewholder from the tag
            vh = (ViewHolder)view.Tag;

            // bind our data!
            vh.Bind(dataObject);

            return view;
        }

        private class ViewHolder : Java.Lang.Object
        {
            private TextView lblNo, lblNoSsm, lblNamaSyarikat;
            private TextView lblAlamat1, lblAlamat2, lblAlamat3;

            private LinearLayout linearAlamat2, linearAlamat3;

            // this method now handles getting references to our subviews
            public void Initialize(View view)
            {
                lblNo = view.FindViewById(Resource.Id.lblNo) as TextView;
                lblNoSsm = view.FindViewById(Resource.Id.lblNoSsm) as TextView;
                lblNamaSyarikat = view.FindViewById(Resource.Id.lblNamaSyarikat) as TextView;
                lblAlamat1 = view.FindViewById(Resource.Id.lblAlamat1) as TextView;
                lblAlamat2 = view.FindViewById(Resource.Id.lblAlamat2) as TextView;
                lblAlamat3 = view.FindViewById(Resource.Id.lblAlamat3) as TextView;

                linearAlamat2 = view.FindViewById(Resource.Id.linearAlamat2) as LinearLayout;
                linearAlamat3 = view.FindViewById(Resource.Id.linearAlamat3) as LinearLayout;

            }

            // this method now handles binding data
            public void Bind(DataSsmDto data)
            {
                var startData = "                ";

                lblNo.Text = data.Number.ToString();
                lblNoSsm.Text = data.NoSyarikat;
                lblNamaSyarikat.Text = data.NamaSyarikat;
                lblAlamat1.Text = data.AlamatNiaga1;

                linearAlamat2.Visibility = ViewStates.Visible;
                linearAlamat3.Visibility = ViewStates.Visible;
                if (string.IsNullOrEmpty(data.AlamatNiaga2))
                {
                    linearAlamat2.Visibility = ViewStates.Gone;
                }
                else
                {
                    lblAlamat2.Text = startData + data.AlamatNiaga2;
                }


                if (string.IsNullOrEmpty(data.AlamatNiaga3))
                {
                    linearAlamat3.Visibility = ViewStates.Gone;
                }
                else
                {
                    lblAlamat3.Text = startData + data.AlamatNiaga3;
                }
               

            }
        }
       
    }
}