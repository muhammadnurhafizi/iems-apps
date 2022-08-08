using Android.Graphics;

namespace IEMSApps.BusinessObject.DTOs
{
    public class PrintImageDto
    {
        public string Text { get; set; }

        public Paint TextPaint { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public bool IsLine { get; set; }

        public int StopX { get; set; }

        public int StopY { get; set; }
        public bool IsRoundRectangle { get; set; }
        public bool IsRectangle { get; set; }
        public int PositionLeft { get; set; }
        public int PositionTop { get; set; }
        public int PositionBottom { get; set; }
        public int PositionRight { get; set; }

        public bool IsLogo { get; set; }
        public Bitmap Bitmap { get; set; }

        public Paint.Align Alignment { get; set; }
        public int Width { get; set; }

        public bool SignKompaun { get; set; }

        public bool IsJustified { get; set; }
        public int JustifiedMaxWidth { get; set; }
    }
}