using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using IEMSApps.Utils;

namespace IEMSApps.Classes
{
    public class HourGlassClass
    {
        private AlertDialog _dialog;
       
        public void StartMessage(Activity activity, Action action, string message = Constants.Messages.WaitingPlease, bool cancelable = true)
        {
           
            _dialog = ShowProgressDialog(activity, message, cancelable);
            new Thread(() =>
            {
                Thread.Sleep(Constants.DefaultWaitingMilisecond);
                activity.RunOnUiThread(action);
            }).Start();
        }

        public void StopMessage()
        {
            _dialog?.Dismiss();
        }

        public void SetMessage(Activity activity, string message)
        {

           // var messageView = dialoglayout?.FindViewById<TextView>(Resource.Id.lblMessage);
            if (tvText != null)
            {
                //activity.RunOnUiThread(() =>
                //{
                //    messageView.Text = message;
                //    _dialog.Notify();
                //});

                activity.RunOnUiThread(() =>
                {
                    tvText.Text = message;
                });

               
               // tvText.Invalidate();
                // dialoglayout.Invalidate();

            }


        }
        private TextView tvText;
        private  AlertDialog ShowProgressDialog(Context ctx, string message, bool cancelable)
        {

            int llPadding = 30;
            LinearLayout ll = new LinearLayout(ctx);
            ll.Orientation = Orientation.Vertical;
            ll.SetPadding(llPadding, llPadding, llPadding, llPadding);
            ll.SetGravity(GravityFlags.Center);
            LinearLayout.LayoutParams llParam = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WrapContent,
                LinearLayout.LayoutParams.WrapContent);
            llParam.Gravity = GravityFlags.Center;
            ll.LayoutParameters = llParam;

            ProgressBar progressBar = new ProgressBar(ctx) {Indeterminate = true};
            progressBar.SetPadding(0, 0, llPadding, 10);
            progressBar.LayoutParameters = llParam;

            llParam = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent);
            llParam.Gravity = GravityFlags.Center;

            tvText = new TextView(ctx);
            tvText.SetText(message, TextView.BufferType.Normal);
            tvText.SetTextColor(Color.ParseColor("#000000"));
            tvText.SetTextSize(ComplexUnitType.Dip, 14);
            tvText.LayoutParameters = llParam;

            ll.AddView(progressBar);
            ll.AddView(tvText);

            AlertDialog.Builder builder = new AlertDialog.Builder(ctx);
            builder.SetCancelable(cancelable);
            builder.SetView(ll);

            var dialog = builder.Create();
            dialog.Show();
            Window window = dialog.Window;
            if (window != null)
            {
                var layoutParams = new WindowManagerLayoutParams();
                layoutParams.CopyFrom(dialog.Window.Attributes);
                layoutParams.Width = LinearLayout.LayoutParams.WrapContent;
                layoutParams.Height = LinearLayout.LayoutParams.WrapContent;
                dialog.Window.Attributes = layoutParams;
            }
            return dialog;
        }

       
        //private TextView messageView;
        //private AlertDialog ShowProgressDialogCustom(Context ctx, string message)
        //{
        //    LayoutInflater inflater = Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
        //    View dialoglayout = inflater.Inflate(Resource.Layout.ProgressDialog, null);
        //    messageView = dialoglayout.FindViewById<TextView>(Resource.Id.lblMessage);
        //    messageView.Text = message;

        //    AlertDialog.Builder builder = new AlertDialog.Builder(ctx);
        //    builder.SetCancelable(true);
        //    builder.SetView(dialoglayout);
        //    builder.Show();

        //    var dialog = builder.Create();
        //    //dialog.Show();
        //    //Window window = dialog.Window;
        //    //if (window != null)
        //    //{
        //    //    var layoutParams = new WindowManagerLayoutParams();
        //    //    layoutParams.CopyFrom(dialog.Window.Attributes);
        //    //    layoutParams.Width = LinearLayout.LayoutParams.WrapContent;
        //    //    layoutParams.Height = LinearLayout.LayoutParams.WrapContent;
        //    //    dialog.Window.Attributes = layoutParams;
        //    //}
        //    return dialog;
        //}
    }
}