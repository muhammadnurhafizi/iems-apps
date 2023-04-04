using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;

namespace IEMSApps.Adapters
{
   
    public class CarianPoskodPenerimaAdapter : BaseAdapter<PoskodPenerimaDto>
    {
        private readonly List<PoskodPenerimaDto> _items;
        private readonly Activity _context;

        public CarianPoskodPenerimaAdapter(Activity activity, List<PoskodPenerimaDto> premisDtos)
        {
            _context = activity;
            _items = premisDtos;
        }

        public override PoskodPenerimaDto this[int position] => _items[position];

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
                view = _context.LayoutInflater.Inflate(Resource.Layout.CarianType1, parent, false) as LinearLayout;
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

            // this method now handles getting references to our subviews
            public void Initialize(View view)
            {
                lblNama = view.FindViewById(Resource.Id.lblNama) as TextView;

            }

            // this method now handles binding data
            public void Bind(PoskodPenerimaDto data)
            {
                lblNama.Text = data.name;

            }
        }

    }
}