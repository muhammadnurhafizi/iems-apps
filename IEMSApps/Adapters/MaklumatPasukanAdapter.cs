using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;

namespace IEMSApps.Adapters
{
    public class MaklumatPasukanAdapter : BaseAdapter<MaklumatPasukanDto>
    {
        readonly List<MaklumatPasukanDto> _items;
        readonly Activity _context;

        public MaklumatPasukanAdapter(Activity context, List<MaklumatPasukanDto> items)
        {
            _context = context;
            _items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override MaklumatPasukanDto this[int position] => _items[position];

        public override int Count => _items?.Count ?? 0;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder vh;

            // attempt to recycle existing view
            var view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.ListPasukanMaklumat, parent, false) as LinearLayout;
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
            private TextView lblJenisPasukan;
            private TextView lblNamaPasukan;
            private TextView lblRouteCode;
            private TextView lblRouteName;
            private LinearLayout linearRoot;


            // this method now handles getting references to our subviews
            public void Initialize(View view)
            {
                lblJenisPasukan = view.FindViewById(Resource.Id.lblJenisPasukan) as TextView;
                lblNamaPasukan = view.FindViewById(Resource.Id.lblNamaPasukan) as TextView;
            }

            // this method now handles binding data
            public void Bind(MaklumatPasukanDto data)
            {
                lblJenisPasukan.Text = data.JenisPasukan;
                lblNamaPasukan.Text = data.NamaPasukan;
            }
        }
    }
}