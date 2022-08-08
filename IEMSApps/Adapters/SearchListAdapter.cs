using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.Classes;
using IEMSApps.Fragments;
using IEMSApps.Utils;

namespace IEMSApps.Adapters
{
    public class SearchListAdapter : BaseAdapter<SearchDto>
    {
        readonly List<SearchDto> _items;
        readonly Fragment _context;

        public SearchListAdapter(Fragment context, List<SearchDto> items) : base()
        {
            _context = context;
            _items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override SearchDto this[int position]
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            View view = convertView;
            if (view == null)
                view = _context.LayoutInflater.Inflate(Resource.Layout.SamakanList, null);

            view.FindViewById<TextView>(Resource.Id.lblNoKPPValue).Text = item.NoRujukan;
            view.FindViewById<TextView>(Resource.Id.lblPremisValue).Text = item.Name;
            view.FindViewById<TextView>(Resource.Id.lblTindakanValue).Text = item.Tindakan;

            Button button = view.FindViewById<Button>(Resource.Id.btnSent);
            button.Focusable = false;
            button.Tag = position;
            button.Click -= Button_Click;

            button.Click += Button_Click;
            //button.Click += async delegate
            //{
            //    //await ((Semakan)_context).ReSendDataAsync(item);
            //    Toast.MakeText(_context.Activity, item.Name, ToastLength.Short).Show();
            //};

            button.Visibility = item.IsSent ? ViewStates.Invisible : ViewStates.Visible;

            return view;
            //ViewHolder vh;
            //// get our data object
            //var dataObject = _items[position];
            //
            //// attempt to recycle existing view
            //var view = convertView;
            //if (view == null)
            //{
            //    view = _context.LayoutInflater.Inflate(Resource.Layout.SamakanList, parent, false) as LinearLayout;
            //    vh = new ViewHolder();
            //
            //    // here's where we get our subview references
            //    vh.Initialize(view, _context, dataObject);
            //
            //    // push the viewholder reference into the view tag
            //    view.Tag = vh;
            //
            //
            //}
            //
            //// get our viewholder from the tag
            //vh = (ViewHolder)view.Tag;
            //
            //// bind our data!
            ////vh.Bind(dataObject);
            //
            //
            //
            //return view;
        }

        private void Button_Click(object sender, System.EventArgs e)
        {
            int posstition = (int)((Button)sender).Tag;
            ((Semakan)_context).ReSendData(_items[posstition]);

            //Toast.MakeText(_context.Activity, $"{posstition}", ToastLength.Short).Show();
        }

        //private class ViewHolder : Java.Lang.Object
        //{
        //    private TextView lblNoKPPValue;
        //    private TextView lblPremisValue;
        //    private TextView lblTindakanValue;
        //    //private Button btnSent;
        //    private Fragment _fragment;
        //    private SearchDto _searchDto;
        //
        //    // this method now handles getting references to our subviews
        //    public void Initialize(View view, Fragment fragment, SearchDto searchDto)
        //    {
        //        _fragment = fragment;
        //
        //        lblNoKPPValue = view.FindViewById(Resource.Id.lblNoKPPValue) as TextView;
        //        lblPremisValue = view.FindViewById(Resource.Id.lblPremisValue) as TextView;
        //        lblTindakanValue = view.FindViewById(Resource.Id.lblTindakanValue) as TextView;
        //        //btnSent = view.FindViewById(Resource.Id.btnSent) as Button;
        //        //btnSent.Focusable = false;
        //
        //        BindData(searchDto);
        //
        //        //btnSent.Click -= delegate { };
        //        //btnSent.Click += async delegate {
        //        //    await ((Semakan)_fragment).ReSendDataAsync(searchDto);
        //        //};                
        //    }
        //
        //    //private void BtnSent_Click(object sender, System.EventArgs e)
        //    //{
        //    //    if (_searchDto != null && _fragment != null)
        //    //    {
        //    //        ((Semakan)_fragment).ReSendDataAsync(_searchDto);
        //    //    }               
        //    //}
        //
        //    // this method now handles binding data
        //    private void BindData(SearchDto searchDto)
        //    {
        //        lblNoKPPValue.Text = searchDto.NoRujukan;
        //        lblPremisValue.Text = searchDto.Name;
        //        lblTindakanValue.Text = searchDto.Tindakan;
        //
        //        //if (searchDto.IsSent)
        //        //    btnSent.Visibility = ViewStates.Invisible;
        //        //else
        //        //    btnSent.Visibility = ViewStates.Visible;
        //    }
        //}
    }
}