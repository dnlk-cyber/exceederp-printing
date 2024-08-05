using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
            if(userName!=null)
            {
                byte[] byteArrayIn = null;
                if (InventoryConfigurationHelper.IsElectronicSignatureEnabled())
                {
                    byteArrayIn = GetUserSignatureData(userName);

                }
                
                return ByteArrayToImage(byteArrayIn);
            }
            else
            {
                return null;
            }
          
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

        
        public static byte[] GetUserSignatureData(string userName)
        {
            string userid = "";
            //   string signatures = null;
            byte[] img = null;
            int bufferSize = 81392;
            // The BLOB byte[] buffer to be filled by GetBytes.  
            byte[] signatures = new byte[bufferSize];
            // The bytes returned from GetBytes.  
            long retval;
            // The starting position in the BLOB output.  
            long startIndex = 0;
            BinaryWriter writer;

            string strcon = ConfigurationManager.ConnectionStrings["ExceedERPUserManagment"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            SqlDataReader myReader = null;
            SqlDataReader myReaderTwo = null;

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Id from [User].AspNetUsers where UserName=@userName", con);
                cmd.Parameters.AddWithValue("@userName", userName);

                myReader = cmd.ExecuteReader();
                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {

                        userid = (myReader[0]?.ToString());
                        SqlCommand cmd1 = new SqlCommand("Select Sign from [User].ApplicationUserSignatures where UserId=@userid", con);
                        cmd1.Parameters.AddWithValue("@userid", userid);
                        myReaderTwo = cmd1.ExecuteReader();

                        if (myReaderTwo.HasRows)
                        {
                            while (myReaderTwo.Read())
                            {
                                //no assignemtn, this could be the issue
                                //myReaderTwo.GetBytes(0, 0, signatures, 0, bufferSize);
                                if (myReaderTwo.GetBytes(0, 0, signatures, 0, bufferSize) > 0)
                                {
                                    // Ensure the signatures array is not null and contains valid data
                                    signatures = new byte[bufferSize];
                                    myReaderTwo.GetBytes(0, 0, signatures, 0, bufferSize);
                                }
                            }

                        }
                        else
                        {
                            signatures = null;
                        }


                    }
                }
                else
                {
                    signatures = null;
                }
            }
            catch (Exception exe)
            {
                //log
            }
            finally
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                if (myReaderTwo != null)
                {
                    myReaderTwo.Close();
                }
                if (con != null)
                {
                    con.Close();
                }
            }
            // signature = Encoding.ASCII.GetBytes(signatures);
            return signatures;
        }
    }
}
