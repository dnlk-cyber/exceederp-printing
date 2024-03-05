using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public class FurnitureJobCategory : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureJobCategoryId { get; set; }
        public string JobName { get; set; }
        public string JobCode { get; set; }
        public bool HaveParent { get; set; }
        public int FurnitureParentId { get; set; }
        public int GLTaxRateID { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        


    }
 
}
