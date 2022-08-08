using Android.App;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;
using System;
using System.Collections.Generic;

namespace IEMSApps.Adapters
{
    public class CarianPremisAdapter : BaseAdapter<PremisDto>
    {
        private readonly List<PremisDto> _items;
        private readonly Activity _context;

        public CarianPremisAdapter(Activity activity, List<PremisDto> premisDtos)
        {
            _context = activity;
            _items = premisDtos;
        }

        public override PremisDto this[int position] => _items[position];

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
                view = _context.LayoutInflater.Inflate(Resource.Layout.CarianPremisItem, parent, false) as LinearLayout;
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
            private TextView lblNamaPremis;
            private TextView lblAlamatPremis, lblAlamatPremis2, lblAlamatPremis3;

            // this method now handles getting references to our subviews
            public void Initialize(View view)
            {
                lblNamaPremis = view.FindViewById(Resource.Id.lblNamaPremis) as TextView;
                lblAlamatPremis = view.FindViewById(Resource.Id.lblAlamatPremis) as TextView;
                lblAlamatPremis2 = view.FindViewById(Resource.Id.lblAlamatPremis2) as TextView;
                lblAlamatPremis3 = view.FindViewById(Resource.Id.lblAlamatPremis3) as TextView;
            }

            // this method now handles binding data
            public void Bind(PremisDto data)
            {
                lblNamaPremis.Text = data.Nama;
                lblAlamatPremis.Text = data.Alamat1;
                lblAlamatPremis2.Text = data.Alamat2;
                lblAlamatPremis3.Text = data.Alamat3;
            }
        }

        public void Update(List<PremisDto> listOfPremisFiltered)
        {
            _items.Clear();
            _items.AddRange(listOfPremisFiltered);
            NotifyDataSetChanged();
        }
    }
}