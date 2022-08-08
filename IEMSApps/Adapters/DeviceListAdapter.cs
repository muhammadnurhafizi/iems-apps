using System.Collections.Generic;
using Android.App;
using Android.Bluetooth;
using Android.Views;
using Android.Widget;

namespace IEMSApps.Adapters
{
    public class DeviceListAdapter : BaseAdapter<BluetoothDevice>
    {
        private List<BluetoothDevice> items;
        private Activity context;

        public DeviceListAdapter(Activity context, List<BluetoothDevice> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }

        public override BluetoothDevice this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items == null ? 0 : items.Count; }
        }

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
                view = context.LayoutInflater.Inflate(Resource.Layout.listitem_device, parent, false) as LinearLayout;
                vh = new ViewHolder();

                // here's where we get our subview references
                vh.Initialize(view);

                // push the viewholder reference into the view tag
                view.Tag = vh;
            }

            // get our data object
            var dataObject = items[position];

            // get our viewholder from the tag
            vh = (ViewHolder)view.Tag;

            // bind our data!
            vh.Bind(dataObject);
            return view;
        }

        private class ViewHolder : Java.Lang.Object
        {
            private TextView name, address, bluetoothClass, Uuid;

            // this method now handles getting references to our subviews
            public void Initialize(View view)
            {
                name = view.FindViewById(Resource.Id.device_name) as TextView;
                address = view.FindViewById(Resource.Id.device_address) as TextView;
                bluetoothClass = view.FindViewById(Resource.Id.device_bluetoothClass) as TextView;
                Uuid = view.FindViewById(Resource.Id.device_Uuid) as TextView;
            }

            // this method now handles binding data
            public void Bind(BluetoothDevice device)
            {
                name.Text = device.Name;
                address.Text = device.Address;
                bluetoothClass.Text = device.BluetoothClass.ToString();
                //Uuid.Text = device.GetUuids().ElementAt(0).ToString();
            }
        }
    }
}