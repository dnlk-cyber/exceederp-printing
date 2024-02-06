using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public class FurnitureProductionCustomerType
    {
        public int FurnitureProductionCustomerTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Remark { get; set; }
    }

    public class FurnitureCommertialItems
    {
        public int FurnitureCommertialItemsId { get; set; }
        [Display(Name = "Job Type")]
        public FurnitureJob_Type Job_Type { get; set; }
        [Display(Name="Sub Category")]
        [StringLength(50)]
        public string SubCategory { get; set; }
        [StringLength(150)]
        public string Description { get; set; }
    }
}
