using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;

namespace IEMSApps.Adapters
{
    public class ConfirmListAdapter : BaseAdapter<ConfirmDto>
    {
        readonly List<ConfirmDto> _items;
        readonly Activity _context;

        public ConfirmListAdapter(Activity context, List<ConfirmDto> items)
        {
            _context = context;
            _items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override ConfirmDto this[int position] => _items[position];

        public override int Count => _items?.Count ?? 0;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder vh;

            // attempt to recycle existing view
            var view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.ConfirmList, parent, false) as LinearLayout;
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
            private TextView lblName;
            private TextView lblValue;
            private TextView lblSep;
            private TextView lblTitle;

            private LinearLayout linearTitle;
            private LinearLayout linearData;
            // this method now handles getting references to our subviews
            public void Initialize(View view)
            {
                lblName = view.FindViewById(Resource.Id.lblName) as TextView;
                lblValue = view.FindViewById(Resource.Id.lblValue) as TextView;
                lblSep = view.FindViewById(Resource.Id.lblSep) as TextView;
                lblTitle = view.FindViewById(Resource.Id.lblTitle) as TextView;

                linearTitle = view.FindViewById(Resource.Id.linearTitle) as LinearLayout;
                linearData = view.FindViewById(Resource.Id.linearData) as LinearLayout;
            }

            // this method now handles binding data
            public void Bind(ConfirmDto data)
            {
                if (data.IsTitle)
                {
                    linearData.Visibility = ViewStates.Gone;
                    linearTitle.Visibility = ViewStates.Visible;
                    lblTitle.Text = data.Label;
                }
                else
                {
                    linearData.Visibility = ViewStates.Visible;
                    linearTitle.Visibility = ViewStates.Gone;
                    lblName.Text = data.Label;
                    lblValue.Text = data.Value;
                }
              

            }
        }
    }
}