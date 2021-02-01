using ExceedERP.Core.Domain.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Inventory
{

    
    public class StoreRequisition : StoreRequisitionBase
    {
        public StoreRequisition()
        {
            ForBranchId = 0;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int StoreRequisitionID { get; set; }

        [JsonIgnore]
        public virtual ICollection<StoreRequisitionItem> StoreRequisitionItem { get; set; }
        [JsonIgnore]
        public virtual ICollection<StoreRequisitionValidation> StoreRequisitionValidation { get; set; }
        [JsonIgnore]
        public virtual ICollection<StoreRequisitionPropertyValidation> StoreRequisitionPropertyValidation { get; set; }
    }

    public class StoreRequisitionItem : StoreRequisitionItemBase
    {
        public int StoreRequisitionItemID { get; set; }
        public int StoreRequisitionID { get; set; }
        public virtual StoreRequisition StoreRequisition { get; set; }
    }

    public class StoreRequisitionValidation : StoreRequisitionValidationBase
    {
        public int StoreRequisitionValidationID { get; set; }
        public int StoreRequisitionID { get; set; }
        public virtual StoreRequisition StoreRequisition { get; set; }
    }

    public class StoreRequisitionPropertyValidation : StoreRequisitionPropertyValidationBase
    {
        public int StoreRequisitionPropertyValidationID { get; set; }
        public int StoreRequisitionID { get; set; }
        public virtual StoreRequisition StoreRequisition { get; set; }
    }

}