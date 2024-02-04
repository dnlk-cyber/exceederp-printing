using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class JobCategory : TrackUserSettingOperation
    {
        public int JobCategoryId { get; set; }
        public string JobName { get; set; }
        public string JobCode { get; set; }
        public bool HaveParent { get; set; }
        public int ParentId { get; set; }
        public int GLTaxRateID { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        


    }
 
}
