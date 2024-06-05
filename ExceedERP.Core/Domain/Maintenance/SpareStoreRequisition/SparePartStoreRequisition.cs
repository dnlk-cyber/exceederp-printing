using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Maintenance.SpareStoreRequisition
{


    public class SparePartStoreRequisition : SparePartStoreRequisitionBase
    {
        public SparePartStoreRequisition()
        {
            ForBranchId = 0;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SparePartStoreRequisitionID { get; set; }

        [JsonIgnore]
        public virtual ICollection<SparePartStoreRequisitionItem> SparePartStoreRequisitionItem { get; set; }
        [JsonIgnore]
        public virtual ICollection<SparePartStoreRequisitionValidation> SparePartStoreRequisitionValidation { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<SparePartStoreRequisitionPropertyValidation> StoreRequisitionPropertyValidation { get; set; }
    }

    public class SparePartStoreRequisitionItem : SparePartStoreRequisitionItemBase
    {
        public int SparePartStoreRequisitionItemID { get; set; }
        public int SparePartStoreRequisitionID { get; set; }
        public virtual SparePartStoreRequisition SparePartStoreRequisition { get; set; }
    }

    public class SparePartStoreRequisitionValidation : SparePartStoreRequisitionValidationBase
    {
        public int SparePartStoreRequisitionValidationID { get; set; }
        public int SparePartStoreRequisitionID { get; set; }
        public virtual SparePartStoreRequisition SparePartStoreRequisition { get; set; }
    }

    //public class SparePart_StoreRequisitionPropertyValidation : SparePart_StoreRequisitionPropertyValidationBase
    //{
    //    public int StoreRequisitionPropertyValidationID { get; set; }
    //    public int StoreRequisitionID { get; set; }
    //    public virtual SparePart_StoreRequisition StoreRequisition { get; set; }
    //}

}