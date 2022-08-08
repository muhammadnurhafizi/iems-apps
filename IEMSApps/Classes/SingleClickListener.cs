using System;
using Android.OS;

namespace IEMSApps.Classes
{
    public class SingleClickListener
    {
        public SingleClickListener(Action<object, EventArgs> setOnClick)
        {
            _setOnClick = setOnClick;
        }
        private bool _hasClicked;

        private Action<object, EventArgs> _setOnClick;

        public void OnClick(object v, EventArgs e)
        {
            if (!_hasClicked)
            {
                _setOnClick(v, e);
                _hasClicked = true;
            }
            reset();
        }

        private void reset()
        {
            Handler mHandler = new Android.OS.Handler();
            mHandler.PostDelayed(() => { _hasClicked = false; }, 500);
        }
    }
}