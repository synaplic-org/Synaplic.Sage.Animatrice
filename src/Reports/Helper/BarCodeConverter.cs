using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Uni.Scan.Reports.Helper
{
    public static class BarCodeConverter
    {
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static byte[] GetBarcodeImage(string Text, string CodeType)
        {
            if (string.IsNullOrWhiteSpace(Text) || string.IsNullOrWhiteSpace(CodeType)) return Array.Empty<byte>();

            if (CodeType == "QR")
            {
                return GetQrCodeImage(Text);
            }

            if (CodeType == "128")
            {
                return Get128CodeImage(Text);
            }

            return Array.Empty<byte>();
        }

        private static byte[] Get128CodeImage(string text)
        {
            using (BarcodeLib.Barcode b = new BarcodeLib.Barcode())
            {
                var img1 = b.Encode(BarcodeLib.TYPE.CODE128, text, Color.Black, Color.White, 376, 34);
                return ImageToByteArray(img1);
            }
        }

        private static byte[] GetQrCodeImage(string text)
        {
            var enc = new QRCodeEncoder
            {
                QRCodeScale = 1
            };
            var oBitmap = enc.Encode(text);

            using (var fs = new MemoryStream())
            {
                oBitmap.Save(fs, ImageFormat.Jpeg);
                return fs.ToArray();
            }
        }
    }
}