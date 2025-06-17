using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace ExceedERP.Helpers.Common
{
    public static class BarcodeGenerator
    {
        public static bool IsValidISBN13(string isbn)
        {
            if (isbn.Length != 13 || !long.TryParse(isbn, out _))
                return false;

            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = isbn[i] - '0';
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit == (isbn[12] - '0');
        }

        public static Bitmap GenerateISBNBarcode(string isbn)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.EAN_13,
                Options = new EncodingOptions
                {
                    Height = 100,
                    Width = 250,
                    Margin = 10,
                    PureBarcode = false
                }
            };
            return writer.Write(isbn);
        }

        public static string GenerateISBNBarcodeBase64(string isbn)
        {
            using (var bitmap = GenerateISBNBarcode(isbn))
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
