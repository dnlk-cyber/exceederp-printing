
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ExceedERP.Core.Domain.Global
{
    public class GlobalStatus
    {
        public int GlobalStatusId { get; set; }
        public string OperationType { get; set; }
        public string Step { get; set; }
        public string Status { get; set; }
    }
  
}
