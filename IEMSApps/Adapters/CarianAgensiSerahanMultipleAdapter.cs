using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;

namespace IEMSApps.Adapters
{
    public class CarianAgensiSerahanMultipleAdapter : BaseAdapter<AgensiSerahanDto>
    {
        private readonly List<AgensiSerahanDto> _items;
        private readonly Activity _context;

        public CarianAgensiSerahanMultipleAdapter(Activity activity, List<AgensiSerahanDto> listData)
        {
            _context = activity;
            _items = listData;
        }

        public override AgensiSerahanDto this[int position] => _items[position];

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
                view = _context.LayoutInflater.Inflate(Resource.Layout.CarianTypeMultiple, parent, false) as LinearLayout;
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
            private TextView lblNama;
            private LinearLayout linear1;

            // this method now handles getting references to our subviews
            public void Initialize(View view)
            {
                lblNama = view.FindViewById(Resource.Id.lblNama) as TextView;
                linear1 = view.FindViewById(Resource.Id.linear1) as LinearLayout;
            }

            // this method now handles binding data
            public void Bind(AgensiSerahanDto data)
            {
                lblNama.Text = data.prgn;
                linear1.SetBackgroundColor(data.IsSelected ? Color.LightGray : Color.White);
            }
        }
    }
}