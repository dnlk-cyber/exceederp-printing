using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.FixedAsset.Counts
{
    public class FixedAssetCount:Operations
    {
        public int FixedAssetCountId { get; set; }
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [DisplayName("Business Unit")]
        public int ? BusinessUnitId { get; set; }        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CountDate { get; set; }
       
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public bool IsClosed { get; set; }
        public List<FixedAssetCountItem> FixedAssetCountItems { get; set; }
        public List<FixedAssetCountValidation> FixedAssetCountValidations { get; set; }
    }
}
