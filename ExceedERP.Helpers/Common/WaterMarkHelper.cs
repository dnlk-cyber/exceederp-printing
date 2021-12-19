using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Web.Hosting;


namespace ExceedERP.Helpers.Common
{
    public static class WaterMarkHelper
    {
        public static bool IsWaterMarkNeeded()
        {
            ExceedDbContext db = new ExceedDbContext();

            var water = db.WaterMarks.FirstOrDefault();
            if (water != null && water.IsNeened)
            {
                return true;
            }

            return false;
        }
        public static Image GetImageForWaterMark()
        {
            ExceedDbContext db = new ExceedDbContext();

            var watermark = db.WaterMarks.FirstOrDefault();

            if (watermark != null && watermark.WaterMarkFileName != null && watermark.IsNeened)
            {
                string watermarkFileName = HostingEnvironment.MapPath("~/Content/Image/WaterMark/" + watermark.WaterMarkFileName);
                if (File.Exists(watermarkFileName))
                {
                    byte[] watermarkFileStream = File.ReadAllBytes(watermarkFileName);

                    var image = ByteArrayToImage(watermarkFileStream);

                    return image;
                }
            }

            return null;
        }
        public static Image GetWaterMarkImage()
        {
            ExceedDbContext db = new ExceedDbContext();

            var watermark = db.WaterMarks.FirstOrDefault();

            if (watermark != null && watermark.WaterMarkFileName != null)
            {
                string watermarkFileName = HostingEnvironment.MapPath("~/Content/Image/WaterMark/" + watermark.WaterMarkFileName);
                if (File.Exists(watermarkFileName))
                {
                    byte[] watermarkFileStream = File.ReadAllBytes(watermarkFileName);

                    var image = ByteArrayToImage(watermarkFileStream);

                    return image;
                }
            }

            return null;
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            return null;

        }
    }
}
