
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ExceedERP.Core.Domain.Global
{
    public class UserBusinessUnitMap
    {
        public int UserBusinessUnitMapId { get; set; }
        public string UserId { get; set; }
        public string BusinessUnitId { get; set; }
    }
  
}
