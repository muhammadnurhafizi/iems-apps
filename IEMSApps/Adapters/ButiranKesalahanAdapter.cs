using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace IEMSApps.Adapters
{
    public class ButiranKesalahanAdapter : BaseAdapter<string>
    {
        private List<string> _butiranKesalahans;
        private Context _context;

        public ButiranKesalahanAdapter(Context context, List<string> list)
        {
            _context = context;
            _butiranKesalahans = list;
        }

        public override string this[int position] => _butiranKesalahans[position];

        public override int Count => _butiranKesalahans.Count();

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
                row = LayoutInflater.From(_context).Inflate(Resource.Layout.ButiranListItem, null, false);
            var butiran = row.FindViewById<TextView>(Resource.Id.lblButiran);
            butiran.Text = _butiranKesalahans[position];
            return row;
        }
    }
}