using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Fleet
{


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class DevLogHelperAttribute : Attribute
    {



        public string Coder { get; set; }
        public string LastModified { get; set; }
        public DevLogHelperAttribute(string coder, string lastModified) { Coder = coder; LastModified = lastModified; }  
    }
   



   





}
