using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.Responses;
using IEMSApps.Fragments;
using System.Collections.Generic;

namespace IEMSApps.Adapters
{
    public class TableSummaryAdapter : BaseAdapter<TableSummaryResponse>
    {
        private List<TableSummaryResponse> _items;
        private DownloadData _fragment;

        public TableSummaryAdapter(DownloadData fragment, List<TableSummaryResponse> datas)
        {
            _items = datas;
            _fragment = fragment;
        }

        public override TableSummaryResponse this[int position] => _items[position];

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
                view = _fragment.LayoutInflater.Inflate(Resource.Layout.tablesummaryitem, parent, false) as LinearLayout;
                vh = new ViewHolder();

                // here's where we get our subview references
                vh.Initialize(_fragment, view);

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
            private TextView txtCawangan, txtJumlahDalamApp, txtJumlahSepatutnya;
            private CheckBox cbPilih;
            private TableSummaryResponse compaund;
            private DownloadData compaunActivity;

            // this method now handles getting references to our subviews
            public void Initialize(DownloadData activity, View view)
            {
                this.compaunActivity = activity;
                cbPilih = view.FindViewById(Resource.Id.cbPilih) as CheckBox;
                txtCawangan = view.FindViewById(Resource.Id.txtCawangan) as TextView;
                txtJumlahDalamApp = view.FindViewById(Resource.Id.txtJumlahDalamApp) as TextView;
                txtJumlahSepatutnya = view.FindViewById(Resource.Id.txtJumlahSepatutnya) as TextView;
            }

            // this method now handles binding data
            public void Bind(TableSummaryResponse data)
            {
                compaund = data;

                cbPilih.Checked = false;
                if (data.IsModified == 1)
                {
                    cbPilih.Checked = true;
                    if (!data.IsSelected)
                        cbPilih.Checked = false;
                }
                else
                    cbPilih.Checked = data.IsSelected;

                //cbPilih.Checked =  && data.IsSelected;
                txtCawangan.Text = data.RecordDesc;
                txtJumlahDalamApp.Text = data.TotalApp.ToString();
                txtJumlahSepatutnya.Text = data.TotalRec.ToString();

                cbPilih.Focusable = false;
                cbPilih.Click += ChkNoKomp_Click;
                //cbPilih.Enabled = data.TotalApp != data.TotalRec;

                txtCawangan.SetTypeface(null, data.IsModified == 1 ? Android.Graphics.TypefaceStyle.Bold : Android.Graphics.TypefaceStyle.Normal);
                txtJumlahDalamApp.SetTypeface(null, data.IsModified == 1 ? Android.Graphics.TypefaceStyle.Bold : Android.Graphics.TypefaceStyle.Normal);
                txtJumlahSepatutnya.SetTypeface(null, data.IsModified == 1 ? Android.Graphics.TypefaceStyle.Bold : Android.Graphics.TypefaceStyle.Normal);

            }

            private void ChkNoKomp_Click(object sender, System.EventArgs e) => compaunActivity.CompaundIsChange(compaund.TableName, cbPilih.Checked);
        }
    }
}