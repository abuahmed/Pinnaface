using System;
using System.Drawing;
using BarcodeLib;

namespace PinnaFace.Core
{
    public class BarcodeProcess
    {
        public BarcodeProcess(){}

        readonly Barcode _b = new Barcode();

        public Image GetBarcode(string dataToBeEncoded, int w, int h, bool includeLabel)
        {
            Image encodeData = null;

            const AlignmentPositions align = AlignmentPositions.CENTER;
            const TYPE type = TYPE.CODE128A;

            try
            {
                _b.IncludeLabel = includeLabel;
                _b.Alignment = align;

                var rotate = Enum.GetNames(typeof(RotateFlipType))[1];
                _b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), rotate, true);
                _b.LabelPosition = LabelPositions.BOTTOMCENTER;

                //===== Encoding performed here =====
                encodeData = _b.Encode(type, dataToBeEncoded.Trim(), Color.Black, Color.White, w, h);
                //===================================

            }//try
            catch
            {
                //MessageBox.Show(ex.Message);
            }//catch

            return encodeData;
        }
    }
}
