using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.BusinessObject.DTOs;
using IEMSApps.BusinessObject.Entities;
using IEMSApps.Classes;
using IEMSApps.Utils;

namespace IEMSApps.BLL
{
    public class PrintImageBll
    {
        private const int Text24 = 24;
        private const int Text22 = 22;
        private const int Text16 = 16;

        private const string FontArial = "ARIAL";

        private const int DefaultWidth = 776;
        private const int BitmapDefaultWidth = 800;

        private int _fontSize = 12;
        private bool _isFontBold = false;
        private string _fontType = "ARIAL";


        private void SetFontSize(int fontSize, string fontType = FontArial)
        {
            _fontSize = fontSize;
            _fontType = fontType;
        }
        private void SetFontNormal()
        {
            _fontSize = Text22;
            _fontType = FontArial;
        }
        private void SetFontBold(bool value)
        {
            _isFontBold = value;
        }

        public Paint SetTextPaint(Paint.Align align = null)
        {
            var textPaint = new Paint();

            textPaint.Color = Color.Black;
            //textPaint.TextAlign = align ?? Paint.Align.Left;
            textPaint.TextSize = _fontSize;
            textPaint.AntiAlias = true;
            textPaint.FakeBoldText = _isFontBold;
            textPaint.FontFeatureSettings = _fontType;

            return textPaint;
        }

        public Bitmap PrepareBitmap(int iLastPositionY)
        {
            var bmp = Bitmap.CreateBitmap(BitmapDefaultWidth, iLastPositionY, Bitmap.Config.Rgb565);
            bmp.EraseColor(Color.White);

            return bmp;
        }

        public Canvas CreateCanvas(Bitmap bmp)
        {
            var canvas = new Canvas(bmp);
            return canvas;
        }

        public void DrawText(Canvas canvas, List<PrintImageDto> listData)
        {

            foreach (var printImageDto in listData)
            {
                if (printImageDto.IsLine)
                {
                    canvas.DrawLine(printImageDto.PositionX, printImageDto.PositionY, printImageDto.StopX,
                        printImageDto.StopY, printImageDto.TextPaint);
                }
                else if (printImageDto.IsRoundRectangle)
                {
                    canvas.DrawRoundRect(printImageDto.PositionLeft, printImageDto.PositionTop, printImageDto.PositionRight,
                        printImageDto.PositionBottom, 10, 10, printImageDto.TextPaint);
                }
                else if (printImageDto.IsRectangle)
                {
                    canvas.DrawRect(printImageDto.PositionLeft, printImageDto.PositionTop, printImageDto.PositionRight,
                        printImageDto.PositionBottom, printImageDto.TextPaint);
                }
                else if (printImageDto.IsLogo && printImageDto.Bitmap != null)
                {
                    if (printImageDto.SignKompaun)
                    {
                        var positionX = printImageDto.PositionX + 150;
                        var positionY = printImageDto.PositionY - 90;
                        //var bitmapWidth = printImageDto.Bitmap.Width;
                        var rect = new Rect(positionX, positionY, positionX + 200, positionY + 100);
                        canvas.DrawBitmap(printImageDto.Bitmap, null, rect, null);
                    }
                    else
                    {
                        if (printImageDto.Alignment == Paint.Align.Center)
                        {
                            //rect = new Rect(0, 0, 100, 100);
                            var bitmapWidth = printImageDto.Bitmap.Width;

                            var posX = (int)(DefaultWidth - bitmapWidth) / 2;
                            var rect = new Rect(posX + 80, 0, posX + 180, 100);
                            canvas.DrawBitmap(printImageDto.Bitmap, null, rect, null);

                            //canvas.DrawBitmap(printImageDto.Bitmap, posX, 0, null);


                            //var rect = new Rect(0, 0, 100, 100);
                            //var rectDest = new Rect(0, 0, 100, 100);
                            //canvas.DrawBitmap(printImageDto.Bitmap, rect, rectDest, null);
                        }
                        else
                        {
                            Rect rect = new Rect(0, 0, 100, 100);
                            canvas.DrawBitmap(printImageDto.Bitmap, null, rect, null);
                        }
                    }

                    printImageDto.Bitmap.Recycle();

                }
                else
                {
                    if (printImageDto.Width > 0)
                    {
                        //1. Measure the output text width. If output width > printImageDto.Width, truncate the output.
                        //2. Check the alignment
                        //3. Draw the output text.
                        string output = SetTextMeasure(printImageDto.Text, printImageDto.TextPaint.Typeface, (int)printImageDto.TextPaint.TextSize, printImageDto.Width);
                        int outputPosX = printImageDto.PositionX;
                        if (printImageDto.Alignment == Paint.Align.Right)
                        {
                            outputPosX += GetTextAlignRight(output, printImageDto.TextPaint.Typeface, (int)printImageDto.TextPaint.TextSize, printImageDto.Width);
                            canvas.DrawText(output, outputPosX, printImageDto.PositionY, printImageDto.TextPaint);

                        }
                        else if (printImageDto.Alignment == Paint.Align.Center)
                        {
                            outputPosX += GetTextAlignCenter(output, printImageDto.TextPaint.Typeface, (int)printImageDto.TextPaint.TextSize, printImageDto.Width);
                            canvas.DrawText(output, outputPosX, printImageDto.PositionY, printImageDto.TextPaint);
                        }
                        else     //default Align = LEFT
                        {
                            canvas.DrawText(output, outputPosX, printImageDto.PositionY, printImageDto.TextPaint);
                        }
                    }
                    else
                    {
                        var text = printImageDto.Text;

                        if (printImageDto.IsJustified)
                        {
                            var textWidth = GetTextMeasure(text, printImageDto.TextPaint.Typeface,
                                (int)printImageDto.TextPaint.TextSize);
                            if ((printImageDto.JustifiedMaxWidth - textWidth) <= 60)
                            {
                                text = AddTextSpace(printImageDto.Text, printImageDto.JustifiedMaxWidth);
                            }
                        }


                        canvas.DrawText(text, printImageDto.PositionX, printImageDto.PositionY,
                            printImageDto.TextPaint);
                    }
                }

            }
        }

        private string AddTextSpace(string text, int maxWidth)
        {
            var sData = text.Split(' ').ToList();
            for (int i = 0; i < sData.Count; i++)
            {
                sData[i] += " ";
                var tempData = string.Join(" ", sData);

                var paint = SetTextPaint();
                var textWidth = GetTextMeasure(tempData, paint.Typeface, (int)paint.TextSize);
                if (maxWidth - textWidth <= 5)
                {
                    break;
                }
            }
            return string.Join(" ", sData);


        }
        public PrintImageDto SetDataPrint(string text, int positionX, int positionY, Paint textPaint)
        {
            return SetDataPrint(text, positionX, positionY, textPaint, 0, null);
        }
        public PrintImageDto SetDataPrint(string text, int positionX, int positionY, Paint textPaint, int maxWidthJustified, bool isJustified)
        {
            return SetDataPrint(text, positionX, positionY, textPaint, 0, null, maxWidthJustified, isJustified);
        }
        public PrintImageDto SetDataPrint(string text, int positionX, int positionY, Paint textPaint, int Width,
            Paint.Align Align, int maxWidthJustified = 0, bool isJustified = false)
        {
            var printImage = new PrintImageDto();
            printImage.Text = text;
            printImage.PositionX = positionX;
            printImage.PositionY = positionY;
            printImage.TextPaint = textPaint;
            printImage.Width = Width;
            printImage.Alignment = Align ?? Paint.Align.Left;
            printImage.IsJustified = isJustified;
            printImage.JustifiedMaxWidth = maxWidthJustified;
            return printImage;
        }

        public string CreateFileBitmap(Bitmap bmp, string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = GeneralBll.GetInternalImagePath() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            }
            else
            {
                fileName = GeneralBll.GetInternalImagePath() + fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".png";
            }
            var stream = new FileStream(fileName, FileMode.Create);

            bmp.Compress(Bitmap.CompressFormat.Jpeg, 85, stream);
            //bmp.Recycle();
            stream.Close();
            return fileName;
        }



        private PrintImageDto CreateText(string text, int positionX, int positionY)
        {
            text = GeneralBll.GetStringOrEmpty(text);

            var textPaint = SetTextPaint();
            return SetDataPrint(text, positionX, positionY, textPaint);
        }

        private PrintImageDto CreateText(string text, int positionX, int positionY, int maxWidthJustified, bool isJustified)
        {
            text = GeneralBll.GetStringOrEmpty(text);

            var textPaint = SetTextPaint();
            return SetDataPrint(text, positionX, positionY, textPaint, maxWidthJustified, isJustified);
        }

        private PrintImageDto CreateText(string text, int positionX, int positionY, Paint.Align align, int Width)
        {
            var textPaint = SetTextPaint(align);
            return SetDataPrint(text, positionX, positionY, textPaint, Width, align);
        }

        private PrintImageDto CreateLine(int positionX, int positionY, int stopX, int stopY)
        {
            var paint = new Paint();
            paint.AntiAlias = true;
            paint.StrokeWidth = 2f;
            paint.Color = Color.Black;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeJoin = Paint.Join.Round;

            var printImage = new PrintImageDto();
            printImage.IsLine = true;
            printImage.PositionX = positionX;
            printImage.PositionY = positionY;
            printImage.StopX = stopX;
            printImage.StopY = stopY;
            printImage.TextPaint = paint;

            return printImage;

        }

        private PrintImageDto CreateRectangle(int left, int top, int bottom, int right)
        {
            var paint = new Paint();
            paint.AntiAlias = true;
            paint.StrokeWidth = 2f;
            //paint.Color = Color.White;
            paint.SetStyle(Paint.Style.Stroke);

            var printImage = new PrintImageDto();
            printImage.IsRectangle = true;
            printImage.PositionLeft = left;
            printImage.PositionTop = top;
            printImage.PositionBottom = bottom;
            printImage.PositionRight = right;
            printImage.TextPaint = paint;

            printImage.PositionX = right;
            printImage.PositionY = bottom;

            return printImage;

        }

        private PrintImageDto CreateRoundRectangle(int left, int top, int bottom, int right)
        {
            var paint = new Paint();
            paint.AntiAlias = true;
            paint.StrokeWidth = 2f;
            //paint.Color = Color.White;
            paint.SetStyle(Paint.Style.Stroke);

            var printImage = new PrintImageDto();
            printImage.IsRoundRectangle = true;
            printImage.PositionLeft = left;
            printImage.PositionTop = top;
            printImage.PositionBottom = bottom;
            printImage.PositionRight = right;
            printImage.TextPaint = paint;

            printImage.PositionX = right;
            printImage.PositionY = bottom;

            return printImage;

        }

        private PrintImageDto CreateLogoHeader(Context ctx)
        {
            var bitmap = BitmapFactory.DecodeResource(ctx.Resources, Resource.Drawable.logo);

            var printImage = new PrintImageDto();
            printImage.IsLogo = true;
            printImage.Bitmap = bitmap;

            return printImage;

        }

        private List<PrintImageDto> CreateHeaderData(Context ctx)
        {
            int positionX = 120;
            int positionY = 20;

            var listData = new List<PrintImageDto>();

            listData.Add(CreateLogoHeader(ctx));

            SetFontSize(Text16);
            listData.Add(CreateText("VIVO GROUP COMPANIES", positionX, positionY));

            positionY += 20;
            listData.Add(CreateText("NO. 81 MIN THEIKDI KYAW SWAR  STREET", positionX, positionY));

            positionY += 20;
            listData.Add(CreateText("INDUSTRIAL (3), HLIANG THAR YAR", positionX, positionY));

            positionY += 20;
            listData.Add(CreateText("TOWNSHIP, YANGON", positionX, positionY));

            positionY += 20;
            listData.Add(CreateText("TEL:01-685789,09-451777 202/262/272/373", positionX, positionY));
            SetFontNormal();


            int startLineX = 0;
            positionY += 15;
            listData.Add(CreateLine(startLineX, positionY, DefaultWidth, positionY));


            return listData;
        }

        public static int GetApproxXToCenterText(string text, Typeface typeface,
            int fontSize, int widthToFitStringInto)
        {
            Paint p = new Paint();
            p.SetTypeface(typeface);
            p.TextSize = fontSize;
            float textWidth = p.MeasureText(text);
            int xOffset = (int)((widthToFitStringInto - textWidth) / 2f) - (int)(fontSize / 2f);
            return xOffset;
        }

        public static int GetTextAlignLeft(string text, Typeface typeface,
            int fontSize, int maxWidth)
        {
            return 0;
        }

        public static int GetTextAlignRight(string text, Typeface typeface, int fontSize, int maxWidth)
        {
            string output = SetTextMeasure(text, typeface, fontSize, maxWidth);
            Paint p = new Paint();
            p.SetTypeface(typeface);
            p.TextSize = fontSize;
            float textWidth = p.MeasureText(output);
            int xOffset = maxWidth - (int)textWidth;
            return xOffset;
        }

        public static int GetTextAlignCenter(string text, Typeface typeface, int fontSize, int maxWidth)
        {
            string output = SetTextMeasure(text, typeface, fontSize, maxWidth);
            Paint p = new Paint();
            p.SetTypeface(typeface);
            p.TextSize = fontSize;
            float textWidth = p.MeasureText(output);
            int xOffset = (maxWidth - (int)textWidth) / 2;
            return xOffset;
        }

        public static string SetTextMeasure(string text, Typeface typeface, int fontSize, int maxWidth)
        {
            string output = "";
            Paint p = new Paint();
            p.SetTypeface(typeface);
            p.TextSize = fontSize;
            float textWidth = p.MeasureText(text);
            if (textWidth > ((float)maxWidth))
            {
                //Truncate the excess replace with ... 
                string truncated = "...";
                float truncatedLength = (float)maxWidth - p.MeasureText(truncated);
                float y = 0;
                for (int x = 1; x < text.Length; x++)
                {
                    output += text.Substring(x, 1);
                    y = p.MeasureText(output);
                    if (y > truncatedLength)
                    {
                        output = text.Substring(1, x - 1) + truncated;
                        break;
                    }
                }
            }
            else
            {
                output = text;
            }

            return output;
        }

        private PrintImageDto CreateTitle(string textData, int positionX, int positionY)
        {
            SetFontSize(Text24);

            Typeface typeface = Typeface.SansSerif;
            var xOffSet = GetApproxXToCenterText(textData, typeface, Text24, DefaultWidth + 60);
            var imageDto = CreateText(textData, xOffSet, positionY);
            SetFontNormal();
            return imageDto;
        }

        private int GetLastPositionY(List<PrintImageDto> listData)
        {
            var lastData = listData.LastOrDefault();
            if (lastData != null)
            {
                return lastData.PositionY;
            }
            return 0;
        }

        private List<PrintImageDto> CreateHeaderBitmap(Context ctx)
        {
            int positionX = 120;
            int positionY = 20;



            var listData = new List<PrintImageDto>();

            listData.Add(CreateLogoHeader(ctx));

            SetFontSize(Text24);
            listData.Add(CreateText("KEMENTERIAN PERDAGANGAN DALAM NEGERI", positionX, positionY));

            positionY += 20;
            listData.Add(CreateText("DAN KOS SARA HIDUP ", positionX, positionY));

            //positionY += 20;
            //listData.Add(CreateText("BAHAGIAN PENGUATKUASA", positionX, positionY));

            var tbCawangan = MasterDataBll.GetCawanganUser();

            if (tbCawangan != null)
            {
                //positionY += 20;
                var alamat1 = string.IsNullOrEmpty(tbCawangan.Alamat1) ? "" : tbCawangan.Alamat1.ToUpper();
                var listString = GeneralBll.SeparateText(alamat1, 10, Constants.DefaultLengthSeparateTitle);
                foreach (var s in listString)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    positionY += 20;
                    listData.Add(CreateText(s.ToUpper(), positionX, positionY));

                }

                var alamat2 = string.IsNullOrEmpty(tbCawangan.Alamat2) ? "" : tbCawangan.Alamat2.ToUpper();
                alamat2 += ", " + tbCawangan.Bandar;

                listString = GeneralBll.SeparateText(alamat2, 10, Constants.DefaultLengthSeparateTitle);
                foreach (var s in listString)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    positionY += 20;
                    listData.Add(CreateText(s.ToUpper(), positionX, positionY));

                }

                //listData.Add(CreateText(alamat1 ,positionX, positionY));

                //positionY += 20;
                //listData.Add(CreateText(alamat2 + ", " + tbCawangan.Bandar,positionX, positionY));

                var negerName = MasterDataBll.GetNegeriName(GeneralBll.ConvertStringToInt(tbCawangan.KodNegeri));
                positionY += 20;
                listData.Add(CreateText(tbCawangan.Poskod + " " + negerName, positionX, positionY));

                positionY += 40;
                listData.Add(CreateText("NO TELEFON : " + tbCawangan.No_Telefon, positionX, positionY));
                positionY += 20;
                listData.Add(CreateText("NO FAKS : " + tbCawangan.No_Faks, positionX, positionY));
            }



            SetFontNormal();


            int startLineX = 0;
            positionY += 15;
            listData.Add(CreateLine(startLineX, positionY, DefaultWidth, positionY));

            return listData;

        }

        public Bitmap Pemeriksaan(Context ctx, string noRujukan, bool blOnce = false)
        {

            var data = PemeriksaanBll.GetPemeriksaanByRujukan(noRujukan);
            if (data == null)
            {
                return null;
            }

            int positionX = 10;
            int addLine = 40;
            int addLine20 = 20;
            int addX1 = 50;

            var listData = new List<PrintImageDto>();
            listData.AddRange(CreateHeaderBitmap(ctx));

            int lastPositionY = GetLastPositionY(listData);

            lastPositionY += addLine;

            SetFontBold(true);
            listData.Add(CreateTitle("KENYATAAN PEMERIKSAAN", 120, lastPositionY));
            SetFontBold(false);

            SetFontSize(Text22);
            lastPositionY += addLine;
            listData.Add(CreateText("No Rujukan : ", positionX + 400, lastPositionY));
            SetFontBold(true);
            listData.Add(CreateText(data.NoRujukanKpp, positionX + 530, lastPositionY));
            SetFontBold(false);

            var dtMula = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhMulaLawatankpp);

            lastPositionY += addLine;
            //listData.Add(CreateText("Tarikh : " + dtMula.ToString(Constants.DateFormatDisplay), positionX, lastPositionY));
            SetFontBold(true);
            listData.Add(CreateText("Tarikh : ", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(dtMula.ToString(Constants.DateFormatDisplay), positionX + 75, lastPositionY));
            SetFontBold(true);
            listData.Add(CreateText("Waktu : ", positionX + 400, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(dtMula.ToString(Constants.TimeFormatDisplay).ToUpper(), positionX + 400 + 75, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Kepada :", positionX, lastPositionY));
            SetFontBold(false);

            lastPositionY += addLine20;
            listData.Add(CreateText(data.NamaPremis, positionX, lastPositionY));
            lastPositionY += addLine20;
            listData.Add(CreateText(data.AlamatPremis1, positionX, lastPositionY));
            lastPositionY += addLine20;
            listData.Add(CreateText(data.AlamatPremis2, positionX, lastPositionY));
            lastPositionY += addLine20;
            listData.Add(CreateText(data.AlamatPremis3, positionX, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("No. Daftar Syarikat/Perniagaan : ", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(data.NoDaftarPremis, positionX + 320, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("No. Lesen BK/PDA : ", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(data.NoLesenBKP_PDA, positionX + 200, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("No. Lesen Majlis/Permit : ", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(data.NoLesenMajlis_Permit, positionX + 250, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("No. Telefon : ", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(data.NoTelefonPremis, positionX + 130, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Lain-lain : ", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(data.CatatanPremis, positionX + 100, lastPositionY));


            lastPositionY += addLine;
            lastPositionY += addLine;
            SetFontBold(true);
            listData.Add(CreateText("Nama Pegawai Yang Menjalankan Pemeriksaan / Siasatan :", positionX, lastPositionY));
            SetFontBold(false);

            var listPasukan = PemeriksaanBll.GetPasukanAhliByRujukan(data.NoRujukanKpp);
            int index = 1;
            lastPositionY += addLine20;
            foreach (var pasukan in listPasukan)
            {

                lastPositionY += addLine20;
                var pasukanName = GeneralBll.GetAlphabetByNumber(index) + ") " + pasukan;
                listData.Add(CreateText(pasukanName.ToUpper(), positionX, lastPositionY));
                index++;
            }

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Tujuan Lawatan / Pemeriksaan / Siasatan  :", positionX, lastPositionY));
            SetFontBold(false);

            //var tujuanLawatan = MasterDataBll.GetTujuanLawatan(data.KodTujuan);
            var tujuanLawatan = string.Empty;
            if (data.KodTujuan == Constants.DefaultTujuanLawatan && !string.IsNullOrEmpty(data.NoAduan))
            {
                tujuanLawatan += $" (No. Aduan : {data.NoAduan})";
            }
            if (!string.IsNullOrEmpty(tujuanLawatan))
            {
                lastPositionY += addLine;
                listData.Add(CreateText(tujuanLawatan.ToUpper(), positionX, lastPositionY));
            }
            if (data.Tindakan == Constants.Tindakan.SiasatUlangan)
            {
                var siasatUlangan = "";
                if (!string.IsNullOrEmpty(data.NoEp))
                {
                    siasatUlangan = $"NO EP : {data.NoEp}";
                }

                if (!string.IsNullOrEmpty(data.NoIp))
                {
                    if (!string.IsNullOrEmpty(siasatUlangan))
                    {
                        siasatUlangan += ", ";
                    }
                    siasatUlangan += $"NO IP : {data.NoIp}";
                }

                if (!string.IsNullOrEmpty(siasatUlangan))
                {
                    lastPositionY += addLine20;
                    listData.Add(CreateText(siasatUlangan.ToUpper(), positionX, lastPositionY));
                }
            }
            //var asasTindakan = MasterDataBll.GetKppAsasTindakan(data.NoRujukanKpp);// MasterDataBll.GetAsasTindakan(data.KodTujuan, data.KodAsas);
            //lastPositionY += addLine20;
            //listData.Add(CreateText(asasTindakan.ToUpper(), positionX, lastPositionY));
            var asasTindakan = MasterDataBll.GetKppAsasTindakan(data.NoRujukanKpp);// MasterDataBll.GetAsasTindakan(data.KodTujuan, data.KodAsas);
            if (string.IsNullOrEmpty(asasTindakan))
            {
                asasTindakan = MasterDataBll.GetAsasTindakan(data.KodTujuan, data.KodAsas);
                lastPositionY += addLine20;
                listData.Add(CreateText(asasTindakan.ToUpper(), positionX, lastPositionY));
            }
            else
            {
                asasTindakan = $"{asasTindakan}";
                var listAsas = GeneralBll.SeparateText(asasTindakan, Constants.DefaultLengthSeparate - 20);
                if (listAsas.Count > 0)
                {
                    foreach (var s in listAsas)
                    {
                        if (string.IsNullOrEmpty(s)) continue;

                        lastPositionY += addLine20;
                        listData.Add(CreateText(s.ToUpper().Trim(), positionX, lastPositionY));
                    }
                }
            }


            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Hasil Lawatan / Pemeriksaan / Siasatan  :", positionX, lastPositionY));
            SetFontBold(false);

            var lawatan = GeneralBll.GetStringOrEmpty(data.HasilLawatan);

            var listLawatan = GeneralBll.SeparateText(lawatan, Constants.MaxLineSeparate20, Constants.DefaultLengthSeparate - 20);
            if (listLawatan.Count > 0)
            {
                foreach (var s in listLawatan)
                {
                    if (string.IsNullOrEmpty(s)) break;

                    lastPositionY += addLine20;
                    listData.Add(CreateText(s, positionX, lastPositionY));
                }
            }
            //lastPositionY += addLine20;
            //listData.Add(CreateText(data.HasilLawatan, positionX, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            lastPositionY += addLine;
            listData.Add(CreateText("Pegawai Pengeluar  :", positionX, lastPositionY));

            lastPositionY += addLine;
            listData.Add(CreateText("Tandatangan : _____________________________", positionX, lastPositionY));


            lastPositionY += addLine;
            listData.Add(CreateText("Nama  :", positionX, lastPositionY));
            SetFontBold(false);

            var tbPengguna = MasterDataBll.GetPenggunaById(data.PengeluarKpp);
            var namaPengeluar = "";
            if (tbPengguna != null)
            {
                namaPengeluar = $"{tbPengguna.Singkatan_Jawatan} {tbPengguna.Nama}";
            }

            listData.Add(CreateText(namaPengeluar.ToUpper(), positionX + 100, lastPositionY));

            lastPositionY += addLine20;

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Waktu Meninggalkan Premis  :", positionX, lastPositionY));
            SetFontBold(false);

            if (!string.IsNullOrEmpty(data.TrkhTamatLawatanKpp))
            {
                var dtAkhir = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhTamatLawatanKpp);

                listData.Add(CreateText(
                    dtAkhir.ToString(Constants.DateFormatDisplay) + " " +
                    dtAkhir.ToString(Constants.TimeFormatDisplay).ToUpper(), positionX + 300, lastPositionY));

            }

            lastPositionY += addLine;
            lastPositionY += addLine;
            listData.Add(CreateText("Akuan Penerimaan Salinan Pendua Oleh Pekerja/Tuanpunya /Orang Yang", positionX, lastPositionY));

            lastPositionY += addLine20;
            listData.Add(CreateText("Bertanggungjawab : ", positionX, lastPositionY));

            if (data.Tindakan == Constants.Tindakan.Kots)
            {
                lastPositionY += addLine;
                listData.Add(CreateText("Pengakuan Orang Kena Kompaun (OKK) ", positionX, lastPositionY));

                lastPositionY += addLine20;
                listData.Add(CreateText("*Saya bersetuju untuk membayar amaun kompaun yang ditawarkan. ", positionX, lastPositionY));
                SetFontBold(true);
                var setuju = "Tidak";
                if (data.SetujuByr == Constants.SetujuBayar.Yes)
                {
                    setuju = " Ya";
                }
                listData.Add(CreateText(setuju, positionX + 700, lastPositionY));
                SetFontBold(false);
                listData.Add(CreateRectangle(positionX + 680, lastPositionY - 30, lastPositionY + 20, DefaultWidth));
            }

            if (data.Tindakan == Constants.Tindakan.TiadaKes && data.Amaran == Constants.Amaran.Yes)
            {
                lastPositionY += addLine;
                listData.Add(CreateText("Amaran diberikan".ToUpper(), positionX, lastPositionY));
            }

            SetFontBold(true);
            lastPositionY += addLine;
            lastPositionY += addLine;
            listData.Add(CreateText("Tandatangan : _____________________________", positionX, lastPositionY));

            addX1 = 215;

            lastPositionY += addLine;
            listData.Add(CreateText("Nama", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(" : " + data.NamaPenerima, positionX + addX1, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("No K/P atau Passport", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(" : " + data.NoKpPenerima, positionX + addX1, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Jawatan", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(" : " + data.Jawatanpenerima, positionX + addX1, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Tarikh", positionX, lastPositionY));
            SetFontBold(false);

            if (!string.IsNullOrEmpty(data.TrkhPenerima))
            {
                var dtPenerima = GeneralBll.ConvertDatabaseFormatStringToDateTime(data.TrkhPenerima);

                listData.Add(CreateText(" : " +
                    dtPenerima.ToString(Constants.DateFormatDisplay), positionX + addX1, lastPositionY));

            }

            lastPositionY += addLine;
            listData.Add(CreateText("Meterai Syarikat / Perniagaan :	", positionX, lastPositionY));

            lastPositionY += addLine;
            listData.Add(CreateText("", positionX, lastPositionY));
            lastPositionY += addLine;
            listData.Add(CreateText("", positionX, lastPositionY));
            if (blOnce)
            {
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));

                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));


            }

            if (listData.Count < Constants.DefaultCountImageData)
            {
                for (int i = listData.Count; i < Constants.DefaultCountImageData; i++)
                {
                    lastPositionY += addLine;
                    listData.Add(CreateText("", positionX, lastPositionY));
                }
            }
            var bitmap = PrepareBitmap(lastPositionY);
            var canvas = CreateCanvas(bitmap);
            DrawText(canvas, listData);

#if DEBUG
            CreateFileBitmap(bitmap, "APemeriksaanBitmap_" + noRujukan);
#endif
            return bitmap;
        }

        private PrintImageDto CreateLogoHeaderKompaun(Context ctx)
        {

            var bitmap = BitmapFactory.DecodeResource(ctx.Resources, Resource.Drawable.logo);

            var printImage = new PrintImageDto
            {
                IsLogo = true,
                Bitmap = bitmap,
                Alignment = Paint.Align.Center,
            };

            return printImage;

        }

        private PrintImageDto CreateSignKompaun(Context ctx, int posX, int posY)
        {

            var bitmap = BitmapFactory.DecodeResource(ctx.Resources, Resource.Drawable.sign);

            var printImage = new PrintImageDto
            {
                IsLogo = true,
                Bitmap = bitmap,
                SignKompaun = true,
                PositionX = posX,
                PositionY = posY
            };

            return printImage;

        }

        private List<PrintImageDto> CreateTitleAkta(int lastPositionY, TbAkta akta, bool isKompaun = true)
        {
            var listData = new List<PrintImageDto>();

            int positionX = 120;
            int addLine = 40;
            int addLine20 = 20;


            if (akta != null)
            {
                lastPositionY += addLine;
                listData.Add(CreateTitle(GeneralBll.GetStringOrEmpty(akta.Tajuk1), positionX, lastPositionY));

                var tajuk = GeneralBll.GetStringOrEmpty(akta.Tajuk2);
                tajuk = tajuk.Replace(")(", ") ("); ;

                var listTajuk = GeneralBll.SeparateText(tajuk, 10, Constants.DefaultLengthSeparateTitle);

                lastPositionY += addLine20;
                foreach (var s in listTajuk)
                {
                    if (string.IsNullOrEmpty(s)) continue;

                    lastPositionY += addLine20;
                    listData.Add(CreateTitle(s.ToUpper(), positionX, lastPositionY));
                }

                SetFontBold(true);
                lastPositionY += addLine;
                listData.Add(CreateTitle(GeneralBll.GetStringOrEmpty(akta.Tajuk3), positionX, lastPositionY));

                if (isKompaun)
                {
                    lastPositionY += addLine;
                    listData.Add(CreateTitle("BORANG I", positionX, lastPositionY));

                    lastPositionY += addLine;
                    listData.Add(CreateTitle(GeneralBll.GetStringOrEmpty(akta.Tajuk5), positionX, lastPositionY));

                }
                else
                {
                    lastPositionY += addLine;
                    listData.Add(CreateTitle("BORANG II", positionX, lastPositionY));

                    lastPositionY += addLine;
                    listData.Add(CreateTitle("PENERIMAAN KOMPAUN", positionX, lastPositionY));
                }
                SetFontBold(false);
            }

            return listData;
        }

        private List<PrintImageDto> CreateDataAkta(int lastPositionY, TbAkta akta, bool isKompaun = true)
        {
            var listData = new List<PrintImageDto>();

            int positionX = 10;
            int addLine = 40;
            int addLine20 = 20;
            try
            {
                if (akta != null)
                {
                    if (isKompaun)
                    {
                        lastPositionY += addLine;
                    }
                    else
                    {
                        lastPositionY += addLine20;
                    }
                    listData.Add(CreateText(GeneralBll.GetStringOrEmpty(akta.Daripada1), positionX, lastPositionY));

                    if (!string.IsNullOrEmpty(akta.Daripada2))
                    {
                        var drpada = akta.Daripada2;
                        var pos1 = drpada.IndexOf(",");

                        //var drpada1 = drpada.Substring(0, pos1 + 1);
                        var drpada2 = drpada.Substring(pos1 + 1, drpada.Length - pos1 - 1);

                        //lastPositionY += addLine20;
                        //listData.Add(CreateText(drpada1.Trim(), positionX, lastPositionY));
                        lastPositionY += addLine20;
                        listData.Add(CreateText(drpada2.Trim(), positionX, lastPositionY));
                    }

                }
                var tbCawangan = MasterDataBll.GetCawanganUser();
                lastPositionY += addLine20;
                var cawangan = "";
                if (!string.IsNullOrEmpty(tbCawangan?.Bandar))
                {
                    cawangan = tbCawangan.Bandar;
                }
                listData.Add(CreateText(cawangan, positionX, lastPositionY));
            }
            catch (Exception ex)
            {
                Log.WriteLogFile("PrintImageBll", "CreateDataAkta", ex.Message, Enums.LogType.Error);
            }



            return listData;
        }


        public Bitmap Kompaun(Context ctx, string noRujukan, bool blOnce = false)
        {

            var data = KompaunBll.GetKompaunByRujukan(noRujukan);
            if (!data.Success || data.Datas == null)
            {
                return null;
            }

            var kompaun = data.Datas;
            var maxWidth = 0;
            int positionX = 10;
            int addLine = 40;
            int addLine20 = 20;
            int addLine25 = 25;

            var listData = new List<PrintImageDto>();
            listData.Add(CreateLogoHeaderKompaun(ctx));

            int lastPositionY = GetLastPositionY(listData);
            lastPositionY += 100;

            var akta = MasterDataBll.GetAktaByKod(kompaun.KodAkta);

            listData.AddRange(CreateTitleAkta(lastPositionY, akta));
            lastPositionY = GetLastPositionY(listData);

            SetFontSize(Text22);
            lastPositionY += addLine;
            lastPositionY += addLine;
            listData.Add(CreateText("NO. KOMPAUN : ", positionX + 400, lastPositionY));
            SetFontBold(true);
            listData.Add(CreateText(kompaun.NoKmp, positionX + 560, lastPositionY));
            SetFontBold(false);

            listData.AddRange(CreateDataAkta(lastPositionY, akta));
            lastPositionY = GetLastPositionY(listData);

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Kepada : ", positionX, lastPositionY));
            SetFontBold(false);

            var nama = kompaun.NamaOkk;
            if (!string.IsNullOrEmpty(kompaun.NoKpOkk))
            {
                nama += " (No K/P: " + kompaun.NoKpOkk + ")";
            }

            lastPositionY += addLine20;
            listData.Add(CreateText(nama.ToUpper(), positionX, lastPositionY));

            lastPositionY += addLine25;
            listData.Add(CreateText(GeneralBll.GetStringOrEmpty(kompaun.AlamatOkk1), positionX, lastPositionY));
            lastPositionY += addLine25 + 5;
            listData.Add(CreateText(GeneralBll.GetStringOrEmpty(kompaun.AlamatOkk2), positionX, lastPositionY));
            lastPositionY += addLine25;
            listData.Add(CreateText(GeneralBll.GetStringOrEmpty(kompaun.AlamatOkk3), positionX, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Cawangan", positionX, lastPositionY));
            SetFontBold(false);

            int addX1 = 250;
            var tbCawangan = MasterDataBll.GetCawanganUser();
            var dataString = "";
            if (!string.IsNullOrEmpty(tbCawangan?.Nama_Cawangan))
            {
                dataString = tbCawangan.Nama_Cawangan;
            }
            listData.Add(CreateText(": " + dataString.ToUpper(), positionX + addX1, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("Tempat", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(": " + GeneralBll.GetStringOrEmpty(kompaun.TempatSalah), positionX + addX1,
                lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("Tarikh", positionX, lastPositionY));
            SetFontBold(false);

            DateTime tarikhSalah = DateTime.Now;
            if (!string.IsNullOrEmpty(kompaun.TrkhSalah))
            {
                tarikhSalah = GeneralBll.ConvertDatabaseFormatStringToDateTime(kompaun.TrkhSalah);
            }
            listData.Add(CreateText(": " + tarikhSalah.ToString(Constants.DateFormatDisplay), positionX + addX1, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("No. Laporan Polis", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(": " + GeneralBll.GetStringOrEmpty(kompaun.NoLaporanPolis), positionX + addX1,
                lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("No. Laporan Cawangan", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(": " + GeneralBll.GetStringOrEmpty(kompaun.NoLaporanCwgn), positionX + addX1,
                lastPositionY));

            var tbKesalahan = MasterDataBll.GetKesalahan(kompaun.KodSalah, kompaun.KodAkta);

            if (akta != null)
            {
                lastPositionY += addLine;
                listData.Add(CreateText(GeneralBll.GetStringOrEmpty(akta.Perenggan1), positionX, lastPositionY));

                lastPositionY += addLine20;
                var aktaDetail = GeneralBll.GetStringOrEmpty(akta.Perenggan2);

                if (tbKesalahan != null)
                {
                    var seksyen = GeneralBll.GetStringOrEmpty(tbKesalahan.Seksyen);
                    aktaDetail = aktaDetail.Replace(Constants.FormatSeksyen, seksyen);
                    var listString = GeneralBll.SeparateText(aktaDetail, 10, Constants.DefaultLengthSeparate);

                    //maxWidth = GetMaxWidth(listString);
                    //NormalizeListData(listString, maxWidth);

                    foreach (var s in listString)
                    {
                        if (string.IsNullOrEmpty(s)) continue;
                        lastPositionY += addLine20;
                        listData.Add(CreateText(s, positionX, lastPositionY));
                        //listData.Add(CreateText(s, positionX , lastPositionY, maxWidth, true));
                    }
                }
            }

            int addX2 = 120;

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Tarikh", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(": " + tarikhSalah.ToString(Constants.DateFormatDisplay), positionX + addX2, lastPositionY));

            SetFontBold(true);
            listData.Add(CreateText("Masa", positionX + 450, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(": " + tarikhSalah.ToString(Constants.TimeFormatDisplay).ToUpper(), positionX + 550, lastPositionY));

            dataString = $"{kompaun.AlamatOkk1} {kompaun.AlamatOkk2} {kompaun.AlamatOkk3}";

            var listAlamat = GeneralBll.SeparateText(dataString, 5, Constants.DefaultLengthSeparate50);
            bool isFirst = true;

            if (listAlamat.Count > 0)
            {
                SetFontBold(true);
                lastPositionY += addLine;
                listData.Add(CreateText("Tempat", positionX, lastPositionY));
                SetFontBold(false);
                foreach (var s in listAlamat)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    if (isFirst)
                    {
                        listData.Add(CreateText(": " + s, positionX + addX2, lastPositionY));
                        isFirst = false;
                    }
                    else
                    {
                        lastPositionY += addLine20;
                        listData.Add(CreateText("  " + s, positionX + addX2, lastPositionY));
                    }

                }

            }

            dataString = ": ";
            if (tbKesalahan != null)
            {
                dataString += GeneralBll.GetStringOrEmpty(tbKesalahan.Prgn);
            }
            var listButir = GeneralBll.SeparateText(dataString, 10, Constants.DefaultLengthSeparate60);

            //for (int i = 0; i < listButir.Count; i++)
            //{
            //    if (string.IsNullOrEmpty(listButir[i])) continue;
            //    if (i != 0)
            //    {
            //        listButir[i] = "  " + listButir[i];
            //    }
            //}

            //maxWidth = GetMaxWidth(listButir);
            //NormalizeListData(listButir, maxWidth);

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Kesalahan", positionX, lastPositionY));
            SetFontBold(false);

            isFirst = true;
            foreach (var s in listButir)
            {
                //if (string.IsNullOrEmpty(s)) continue;

                //if (isFirst)
                //{
                //    listData.Add(CreateText(s, positionX + addX2, lastPositionY, maxWidth, true));
                //    isFirst = false;
                //}
                //else
                //{
                //    lastPositionY += addLine20;
                //    listData.Add(CreateText(s, positionX + addX2, lastPositionY, maxWidth, true));
                //}
                if (string.IsNullOrEmpty(s)) continue;
                if (isFirst)
                {
                    listData.Add(CreateText(s, positionX + addX2, lastPositionY));
                    // listData.Add(CreateText(s, positionX + addX2, lastPositionY, maxWidth, true));
                    isFirst = false;
                }
                else
                {
                    lastPositionY += addLine20;
                    listData.Add(CreateText("  " + s, positionX + addX2, lastPositionY));
                    //listData.Add(CreateText("  " + s, positionX + addX2, lastPositionY, maxWidth, true));
                }
            }

            if (akta != null)
            {
                var dataAkta3 = GeneralBll.GetStringOrEmpty(akta.Perenggan3);

                if (tbKesalahan != null)
                {
                    if (kompaun.JenisPesalah == Constants.JenisPesalah.Individu)
                    {
                        dataAkta3 = dataAkta3.Replace(Constants.ReplaceAmoun,
                            tbKesalahan.AmnKmp_Ind.ToString(Constants.DecimalFormat));
                        dataAkta3 = dataAkta3.Replace(Constants.ReplaceAmountWord,
                            GeneralBll.GetStringOrEmpty(tbKesalahan.AmnKmp_Ind_Word));
                    }
                    else
                    {
                        dataAkta3 = dataAkta3.Replace(Constants.ReplaceAmoun,
                            tbKesalahan.AmnKmp_Sya.ToString(Constants.DecimalFormat));
                        dataAkta3 = dataAkta3.Replace(Constants.ReplaceAmountWord,
                            GeneralBll.GetStringOrEmpty(tbKesalahan.AmnKmp_Sya_Word));
                    }

                }

                var listStringKesalahan = GeneralBll.SeparateText(dataAkta3, 10, Constants.DefaultLengthSeparate);

                //maxWidth = GetMaxWidth(listStringKesalahan);
                //NormalizeListData(listStringKesalahan, maxWidth);

                lastPositionY += addLine20;
                foreach (var s in listStringKesalahan)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    lastPositionY += addLine20;
                    //listData.Add(CreateText(s, positionX, lastPositionY,maxWidth, true));
                    listData.Add(CreateText(s, positionX, lastPositionY));

                }

                var dataAkta4 = GeneralBll.GetStringOrEmpty(akta.Perenggan4);

                dataAkta4 = dataAkta4.Replace(Constants.ReplaceTempohHari,
                    kompaun.TempohTawaran.ToString());


                var listStringKesalahan2 = GeneralBll.SeparateText(dataAkta4, 10, Constants.DefaultLengthSeparate);

                //maxWidth = GetMaxWidth(listStringKesalahan2);
                //NormalizeListData(listStringKesalahan2, maxWidth);


                lastPositionY += addLine20;
                foreach (var s in listStringKesalahan2)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    lastPositionY += addLine20;
                    listData.Add(CreateText(s, positionX, lastPositionY));
                    //listData.Add(CreateText(s, positionX, lastPositionY, maxWidth, true));
                }

            }
            DateTime tarikhKmp = DateTime.Now;
            if (!string.IsNullOrEmpty(kompaun.TrkhKmp))
            {
                tarikhKmp = GeneralBll.ConvertDatabaseFormatStringToDateTime(kompaun.TrkhKmp);
            }
            int addX3 = 260;

            lastPositionY += addLine;
            lastPositionY += addLine;
            listData.Add(CreateText(tarikhKmp.ToString(Constants.DateFormatDisplay), positionX, lastPositionY));

            //listData.Add(CreateSignKompaun(ctx, positionX + addX3 , lastPositionY));

            lastPositionY += addLine20;
            listData.Add(CreateText("--------------------", positionX, lastPositionY));
            //listData.Add(CreateText("---------------------------------------------------------------------------------", positionX+ addX3, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("     Tarikh    ", positionX, lastPositionY));
            SetFontBold(false);

            var pengeluar = "";
            var pengeluar2 = "";
            if (akta != null)
            {
                if (!string.IsNullOrEmpty(akta.Pengeluar1))
                {
                    pengeluar = akta.Pengeluar1;
                }
                if (!string.IsNullOrEmpty(akta.Pengeluar2))
                {
                    pengeluar2 = akta.Pengeluar2;
                }
            }
            if (!string.IsNullOrEmpty(pengeluar))
            {
                pengeluar = $"({pengeluar})";
            }

            listData.Add(CreateText(pengeluar, positionX + addX3, lastPositionY));


            lastPositionY += addLine20;
            listData.Add(CreateText(pengeluar2, positionX + addX3, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            lastPositionY += addLine;
            listData.Add(CreateText("Pegawai Penyiasat", positionX, lastPositionY));
            SetFontBold(false);
            var pegawai = MasterDataBll.GetPenggunaSingkatanAndName(kompaun.PegawaiPengeluar);
            listData.Add(CreateText(": " + pegawai.ToUpper(), positionX + 200, lastPositionY));

            lastPositionY += addLine;
            listData.Add(CreateText("Cetakan berkomputer ini tidak memerlukan tandatangan.", positionX, lastPositionY));

            lastPositionY += addLine;
            listData.Add(CreateText("", positionX, lastPositionY));
            lastPositionY += addLine;
            listData.Add(CreateText("", positionX, lastPositionY));
            lastPositionY += addLine;
            listData.Add(CreateText("", positionX, lastPositionY));


            if (blOnce)
            {
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));

                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));

                //lastPositionY += addLine;
                //listData.Add(CreateText("", positionX, lastPositionY));
                //lastPositionY += addLine;
                //listData.Add(CreateText("", positionX, lastPositionY));
                //lastPositionY += addLine;
                //listData.Add(CreateText("", positionX, lastPositionY));
                //lastPositionY += addLine;
                //listData.Add(CreateText("", positionX, lastPositionY));
                //lastPositionY += addLine;
                //listData.Add(CreateText("", positionX, lastPositionY));
                //lastPositionY += addLine;
                //listData.Add(CreateText("", positionX, lastPositionY));
            }

            var bitmap = PrepareBitmap(lastPositionY);
            var canvas = CreateCanvas(bitmap);
            DrawText(canvas, listData);

#if DEBUG
            CreateFileBitmap(bitmap, "AKompaunBitmap_" + noRujukan);
#endif

            return bitmap;
        }

        private int GetTextMeasure(string text, Typeface typeface, int fontSize)
        {
            Paint p = new Paint();
            p.SetTypeface(typeface);
            p.TextSize = fontSize;
            float textWidth = p.MeasureText(text);
            return (int)textWidth;
        }

        private int GetMaxWidth(List<string> listData)
        {
            int maxWidth = 0;
            foreach (var data in listData)
            {
                var paint = SetTextPaint();
                var textWidth = GetTextMeasure(data, paint.Typeface, (int)paint.TextSize);
                if (textWidth > maxWidth) maxWidth = textWidth;
            }
            return maxWidth;
        }

        private int NormalizeListData(List<string> listData, int maxWidth)
        {
            for (int i = 0; i < listData.Count; i++)
            {
                if (i + 1 < listData.Count)
                {
                    var paint = SetTextPaint();
                    if (string.IsNullOrEmpty(listData[i])) break;
                    var textWidth1 = GetTextMeasure(listData[i], paint.Typeface, (int)paint.TextSize);
                    var textWidth2 = GetTextMeasure(" " + listData[i + 1], paint.Typeface, (int)paint.TextSize);
                    if (textWidth1 + textWidth2 <= maxWidth)
                    {
                        listData[i] += " " + listData[i + 1];
                        listData[i + 1] = "";
                    }
                }
            }
            foreach (var data in listData)
            {
                var paint = SetTextPaint();
                var textWidth = GetTextMeasure(data, paint.Typeface, (int)paint.TextSize);
                if (textWidth > maxWidth) maxWidth = textWidth;
            }
            return maxWidth;
        }


        public Bitmap Akuan(Context ctx, string noRujukan, bool blOnce = false)
        {

            var data = KompaunBll.GetKompaunByRujukan(noRujukan);
            if (!data.Success || data.Datas == null)
            {
                return null;
            }

            var kompaun = data.Datas;

            int positionX = 10;
            int addLine = 40;
            int addLine20 = 20;

            var listData = new List<PrintImageDto>();
            listData.Add(CreateLogoHeaderKompaun(ctx));

            int lastPositionY = GetLastPositionY(listData);
            lastPositionY += 100;

            var akta = MasterDataBll.GetAktaByKod(kompaun.KodAkta);

            listData.AddRange(CreateTitleAkta(lastPositionY, akta, false));
            lastPositionY = GetLastPositionY(listData);

            SetFontSize(Text22);
            lastPositionY += addLine;
            listData.Add(CreateText("NO. AKUAN : ", positionX + 400, lastPositionY));
            SetFontBold(true);
            listData.Add(CreateText(kompaun.NoKmp, positionX + 530, lastPositionY));

            lastPositionY += addLine;
            listData.Add(CreateText("Kepada : ", positionX, lastPositionY));
            SetFontBold(false);

            listData.AddRange(CreateDataAkta(lastPositionY, akta, false));
            lastPositionY = GetLastPositionY(listData);

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateTitle("PENERIMAAN KOMPAUN", positionX, lastPositionY));
            SetFontBold(false);

            lastPositionY += addLine;
            lastPositionY += addLine20;
            listData.Add(CreateText("Apabila menjawab sila nyatakan :", positionX, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            listData.Add(CreateText("Cawangan", positionX, lastPositionY));
            SetFontBold(false);

            int addX1 = 250;
            var tbCawangan = MasterDataBll.GetCawanganUser();
            var dataString = "";
            if (!string.IsNullOrEmpty(tbCawangan?.Nama_Cawangan))
            {
                dataString = tbCawangan.Nama_Cawangan;
            }
            listData.Add(CreateText(": " + dataString.ToUpper(), positionX + addX1, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("Tempat", positionX, lastPositionY));
            SetFontBold(false);
            listData.Add(CreateText(": " + GeneralBll.GetStringOrEmpty(kompaun.TempatSalah), positionX + addX1,
                lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("Tarikh", positionX, lastPositionY));
            SetFontBold(false);

            DateTime tarikhSalah = DateTime.Now;
            if (!string.IsNullOrEmpty(kompaun.TrkhSalah))
            {
                tarikhSalah = GeneralBll.ConvertDatabaseFormatStringToDateTime(kompaun.TrkhSalah);
            }
            listData.Add(CreateText(": " + tarikhSalah.ToString(Constants.DateFormatDisplay), positionX + addX1, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("No. Laporan Polis", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(": " + GeneralBll.GetStringOrEmpty(kompaun.NoLaporanPolis), positionX + addX1,
                lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("No. Laporan Cawangan", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(": " + GeneralBll.GetStringOrEmpty(kompaun.NoLaporanCwgn), positionX + addX1,
                lastPositionY));

            var daripada1 = "";
            if (!string.IsNullOrEmpty(akta?.Daripada1))
            {
                daripada1 = akta.Daripada1;
            }

            lastPositionY += addLine;
            lastPositionY += addLine20;
            //listData.Add(CreateText("Pengawal Perihal Dagangan", positionX, lastPositionY));
            listData.Add(CreateText(daripada1, positionX, lastPositionY));

            DateTime tarikhKompaun = DateTime.Now;
            if (!string.IsNullOrEmpty(kompaun.TrkhKmp))
            {
                tarikhKompaun = GeneralBll.ConvertDatabaseFormatStringToDateTime(kompaun.TrkhKmp);
            }


            var terbilang = "";
            if (kompaun.AmnByr > 0)
            {
                terbilang = GeneralBll.DecimalToWord(kompaun.AmnByr);
            }

            dataString =
                "1.  Saya merujuk kepada tawaran untuk mengkompaun dalam surat tuan dengan nombor rujukan " +
                kompaun.NoLaporanCwgn + " bertarikh " + tarikhKompaun.ToString(Constants.DateFormatDisplay) +
                ". Saya terima tawaran itu dan menyertakan bersama ini wang tunai sebanyak RM " +
                kompaun.AmnByr.ToString(Constants.DecimalFormatZero) +
                " (Ringgit Malaysia " + terbilang + " Sahaja) bagi menjelaskan kompaun itu.";

            var listString = GeneralBll.SeparateText(dataString, 10, Constants.DefaultLengthSeparate);
            lastPositionY += addLine20;
            foreach (var s in listString)
            {
                if (string.IsNullOrEmpty(s)) continue;
                lastPositionY += addLine20;
                listData.Add(CreateText(s, positionX, lastPositionY));

            }

            SetFontBold(true);
            lastPositionY += addLine;
            lastPositionY += addLine;
            listData.Add(CreateText("Tandatangan", positionX, lastPositionY));
            listData.Add(CreateText(": ", positionX + addX1, lastPositionY));


            lastPositionY += addLine20;
            listData.Add(CreateText("Nama", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(": " + kompaun.NamaPenerima_Akuan, positionX + addX1, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine20;
            listData.Add(CreateText("Alamat", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(": " + kompaun.AlamatPenerima1_Akuan, positionX + addX1, lastPositionY));

            lastPositionY += addLine20;
            listData.Add(CreateText("  " + kompaun.AlamatPenerima2_Akuan, positionX + addX1, lastPositionY));

            lastPositionY += addLine20;
            listData.Add(CreateText("  " + kompaun.AlamatPenerima3_Akuan, positionX + addX1, lastPositionY));

            var dtMula = GeneralBll.ConvertDatabaseFormatStringToDateTime(kompaun.TrkhKmp);

            SetFontBold(true);
            lastPositionY += addLine;
            lastPositionY += addLine;
            //listData.Add(CreateText("Tarikh : " + dtMula.ToString(Constants.DateFormatDisplay), positionX, lastPositionY));
            listData.Add(CreateText("Tarikh : ", positionX, lastPositionY));
            SetFontBold(false);

            listData.Add(CreateText(dtMula.ToString(Constants.DateFormatDisplay), positionX + 100, lastPositionY));

            SetFontBold(true);
            lastPositionY += addLine;
            lastPositionY += addLine;
            listData.Add(CreateText("Pegawai Penyiasat", positionX, lastPositionY));
            SetFontBold(false);

            var pegawai = MasterDataBll.GetPenggunaSingkatanAndName(kompaun.PegawaiPengeluar);
            listData.Add(CreateText(": " + pegawai.ToUpper(), positionX + 200, lastPositionY));

            lastPositionY += addLine;
            listData.Add(CreateText("", positionX, lastPositionY));

            var haveIP = AkuanBll.GetIpResitsByKPP(kompaun.NoRujukanKpp);
            var yes = haveIP != null ? haveIP.norujukankpp : "";
            //temporary solution
            //var resit = DataAccessQuery<ip_resits>.GetAll();
            //var count = resit.Datas.Count;
            if (kompaun.isbayarmanual == 1 || (yes == kompaun.NoRujukanKpp))
            {
                SetFontBold(true);
                lastPositionY += addLine;
                listData.Add(CreateText("Pembayaran Kompaun ini telah dibuat melalui Sistem Terimaan Eletronik Kerajaan Persekutuan (iPayment)", positionX, lastPositionY));
                SetFontBold(false);
                lastPositionY += addLine20;
                listData.Add(CreateText("No Resit: " + kompaun.NoResit, positionX, lastPositionY));
                lastPositionY += addLine20;
                listData.Add(CreateText("Tarikh Resit: " + GeneralBll.GetLocalDateTime().ToString(), positionX, lastPositionY));
                lastPositionY += addLine;
            }

            lastPositionY += addLine;
            listData.Add(CreateText("", positionX, lastPositionY));
            lastPositionY += addLine;
            listData.Add(CreateText("", positionX, lastPositionY));

            if (blOnce)
            {
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));

                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
                lastPositionY += addLine;
                listData.Add(CreateText("", positionX, lastPositionY));
            }



            var bitmap = PrepareBitmap(lastPositionY);
            var canvas = CreateCanvas(bitmap);
            DrawText(canvas, listData);

#if DEBUG
            CreateFileBitmap(bitmap, "AkuanBitmap_" + noRujukan);
#endif

            return bitmap;
        }
    }
}