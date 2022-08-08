using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using IEMSApps.Activities;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.Classes;
using IEMSApps.Fragments;
using IEMSApps.Utils;

namespace IEMSApps.Adapters
{
    public class PasukanAhliAdapter : BaseAdapter<PasukanAhliDto>
    {
        readonly List<PasukanAhliDto> _items;
        readonly Activity _context;
        readonly Fragment _fragment;

        public PasukanAhliAdapter(Activity context, Fragment fragment, List<PasukanAhliDto> items)
        {
            _context = context;
            _items = items;
            _fragment = fragment;
        }
        public PasukanAhliAdapter(Activity context, List<PasukanAhliDto> items)
        {
            _context = context;
            _items = items;

        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override PasukanAhliDto this[int position] => _items[position];

        public override int Count => _items?.Count ?? 0;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder vh;

            // attempt to recycle existing view
            var view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.ListPasukanAhli, parent, false) as LinearLayout;
                vh = new ViewHolder();

                // here's where we get our subview references
                vh.Initialize(view);

                // push the viewholder reference into the view tag
                view.Tag = vh;

                var imgRemove = (ImageView)view.FindViewById(Resource.Id.imgRemove);
                imgRemove.Click += (sender, e) =>
                {
                    if (_items[position] != null)
                    {
                        if (_fragment != null)
                        {
                            ((Pasukan)_fragment).RemovePasukanAhli(this._items[position].KodPasukan,
                                this._items[position].NoKp);
                        }
                        else
                        {
                            ((PasukanForm)_context).RemovePasukanAhli(this._items[position].KodPasukan,
                                this._items[position].NoKp);
                        }
                    }

                };
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
            private TextView lblBil;
            private TextView lblNama;
            private TextView lblStatus;
            private ImageView imgRemove;
            private LinearLayout linearRoot;


            // this method now handles getting references to our subviews
            public void Initialize(View view)
            {
                lblBil = view.FindViewById(Resource.Id.lblBil) as TextView;
                lblNama = view.FindViewById(Resource.Id.lblNama) as TextView;
                lblStatus = view.FindViewById(Resource.Id.lblStatus) as TextView;
                imgRemove = view.FindViewById(Resource.Id.imgRemove) as ImageView;

                linearRoot = view.FindViewById(Resource.Id.linearRoot) as LinearLayout;

                //imgRemove.Click += (sender, e) =>
                //{
                //    //GeneralAndroidClass.ShowToast(lblBil.Text);
                //    ((Pasukan)fragment).RemovePasukanAhli(data.KodPasukan, data.NoKp);
                //};

            }

            // this method now handles binding data
            public void Bind(PasukanAhliDto data)
            {
                lblBil.Text = data.NoUrut + ".";
                lblNama.Text = data.Nama;
                lblStatus.Text = data.Status == Constants.Status.Aktif ? "Aktif" : "Tidak Aktif";

                linearRoot.SetBackgroundResource(Resource.Color.transparent);
                if (data.NoUrut % 2 == 0)
                {
                    linearRoot.SetBackgroundResource(Resource.Color.rowgrey);
                }

                //imgRemove.Click += (sender, e) =>
                //{
                //   //GeneralAndroidClass.ShowToast(lblBil.Text);
                //   ((Pasukan)fragment).RemovePasukanAhli(data.KodPasukan, data.NoKp);
                //};
            }
        }
    }
}
