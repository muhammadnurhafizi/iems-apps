using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEMSApps.Classes
{
    public class IntegerToWordConverter
    {
        private static string[] units = { "", "SATU", "DUA", "TIGA", "EMPAT", "LIMA", "ENAM", "TUJUH", "LAPAN", "SEMBILAN", "SEPULUH", "SEBELAS", "DUA BELAS", "TIGA BELAS", "EMPAT BELAS", "LIMA BELAS", "ENAM BELAS", "TUJUH BELAS", "LAPAN BELAS", "SEMBILAN BELAS" };
        private static string[] tens = { "", "", "DUA PULUH", "TIGA PULUH", "EMPAT PULUH", "LIMA PULUH", "ENAM PULUH", "TUJUH PULUH", "LAPAN PULUH", "SEMBILAN PULUH" };

        public static string ConvertNumberToWords(double number)
        {
            if (number == 0)
                return "KOSONG";

            string words = "";

            if (number < 0)
            {
                words += "TOLAK ";
                number = -number;
            }

            int intPart = (int)number;
            double decimalPart = number - intPart;

            if (intPart > 0)
            {
                words += ConvertNumberToWordsHelper(intPart);
            }
            else
            {
                words += units[0];
            }

            if (decimalPart > 0)
            {
                words += " POINT ";

                string decimalWords = "";

                foreach (char c in decimalPart.ToString())
                {
                    int digit = int.Parse(c.ToString());
                    decimalWords += units[digit] + " ";
                }

                words += decimalWords.Trim();
            }

            return words.Trim();
        }

        private static string ConvertNumberToWordsHelper(int number)
        {
            if (number < 20)
            {
                return units[number];
            }

            if (number < 100)
            {
                return tens[number / 10] + " " + ConvertNumberToWordsHelper(number % 10);
            }

            if (number < 1000)
            {
                return units[number / 100] + " RATUS " + ConvertNumberToWordsHelper(number % 100);
            }

            if (number < 1000000)
            {
                return ConvertNumberToWordsHelper(number / 1000) + " RIBU " + ConvertNumberToWordsHelper(number % 1000);
            }

            if (number < 1000000000)
            {
                return ConvertNumberToWordsHelper(number / 1000000) + " JUTA " + ConvertNumberToWordsHelper(number % 1000000);
            }

            return "";
        }
    }
}