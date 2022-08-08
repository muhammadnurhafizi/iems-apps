using Android.Text;
using IEMSApps.Utils;
using Java.Lang;

namespace IEMSApps.Classes
{
    public class FilterChar : Java.Lang.Object, IInputFilter
    {
        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            try
            {
                //string val = dest.ToString().Insert(dstart, source.ToString());
                ////int input = int.Parse(val);
                ////if (IsInRange(_min, _max, input))
                var data = source.ToString();
                if (data.Length > 1)
                {
                    if (IsAllowString(data))
                    {
                        return null;
                    }
                }
                else
                {
                    if (Constants.AllowedChar.Contains(data))
                        return null;
                }
               
            }
            catch (System.Exception ex)
            {
               
            }

            return new Java.Lang.String(string.Empty);
        }

        private bool IsAllowString(string data)
        {
            foreach (var ch in data)
            {
                if (!Constants.AllowedChar.Contains(ch.ToString()))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsInRange(int min, int max, int input)
        {
            return max > min ? input >= min && input <= max : input >= max && input <= min;
        }
    }

    public class FilterNoIPAndNoEPChar : Java.Lang.Object, IInputFilter
    {
        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            try
            {
                //string val = dest.ToString().Insert(dstart, source.ToString());
                ////int input = int.Parse(val);
                ////if (IsInRange(_min, _max, input))
                var data = source.ToString();
                if (data.Length > 1)
                {
                    if (IsAllowNoIPAndNoEPString(data))
                    {
                        return null;
                    }
                }
                else
                {
                    if (Constants.AllowedCharNoIPAndNoEP.Contains(data))
                        return null;
                }

            }
            catch (System.Exception ex)
            {

            }

            return new Java.Lang.String(string.Empty);
        }

        private bool IsAllowString(string data)
        {
            foreach (var ch in data)
            {
                if (!Constants.AllowedChar.Contains(ch.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsAllowNoIPAndNoEPString(string data)
        {
            foreach (var ch in data)
            {
                if (!Constants.AllowedCharNoIPAndNoEP.Contains(ch.ToString()))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsInRange(int min, int max, int input)
        {
            return max > min ? input >= min && input <= max : input >= max && input <= min;
        }
    }

    public class FilterCharWithoutSingleQuote : Java.Lang.Object, IInputFilter
    {
        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            try
            {
                //string val = dest.ToString().Insert(dstart, source.ToString());
                ////int input = int.Parse(val);
                ////if (IsInRange(_min, _max, input))
                var data = source.ToString();
                if (data.Length > 1)
                {
                    if (IsAllowString(data))
                    {
                        return null;
                    }
                }
                else
                {
                    if (Constants.AllowedCharWithoutSingleQuote.Contains(data))
                        return null;
                }

            }
            catch (System.Exception ex)
            {

            }

            return new Java.Lang.String(string.Empty);
        }

        private bool IsAllowString(string data)
        {
            foreach (var ch in data)
            {
                if (!Constants.AllowedCharWithoutSingleQuote.Contains(ch.ToString()))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsInRange(int min, int max, int input)
        {
            return max > min ? input >= min && input <= max : input >= max && input <= min;
        }
    }
}