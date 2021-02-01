using Kendo.Mvc.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Kendo.Mvc.Infrastructure;
//using Newtonsoft.Json;

namespace ExceedERP.Web
{
    public class CustomKendoJsonSerializer : IJavaScriptSerializer
    {
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value); //use the serializer you want  here
        }
    }

    public class CustomKendoInitializer : JavaScriptInitializer

    {

        public override IJavaScriptSerializer CreateSerializer()

        {

            return new CustomKendoJsonSerializer();

        }

    }
}