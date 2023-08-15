using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Common
{
    public class SignatureHelper
    {
        public static Image GetUserSignature(string userName)
        {
            byte[] byteArrayIn = null;
            if(InventoryConfigurationHelper.IsElectronicSignatureEnabled())
            {
                byteArrayIn = UserHelper.GetUserSignature(userName);

            }
            return ByteArrayToImage(byteArrayIn);
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
