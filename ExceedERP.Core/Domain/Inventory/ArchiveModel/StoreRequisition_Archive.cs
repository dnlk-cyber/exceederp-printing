using ExceedERP.Core.Domain.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Inventory
{

    
    public class StoreRequisitionArchive : StoreRequisitionBase
    {
        
        public int StoreRequisitionArchiveId { get; set; }
        public int StoreRequisitionID { get; set; }
        public virtual ICollection<StoreRequisitionItemArchive> StoreRequisitionItemArchive { get; set; }
        public virtual ICollection<StoreRequisitionValidationArchive> StoreRequisitionValidationArchive { get; set; }
        public virtual ICollection<StoreRequisitionPropertyValidationArchive> StoreRequisitionPropertyValidationArchive { get; set; }
    }

    public class StoreRequisitionItemArchive : StoreRequisitionItemBase
    {
        public int StoreRequisitionItemArchiveId { get; set; }
        public int StoreRequisitionArchiveId { get; set; }
        public virtual StoreRequisitionArchive StoreRequisitionArchive { get; set; }
        public int StoreRequisitionItemID { get; set; }
    }

    public class StoreRequisitionValidationArchive : StoreRequisitionValidationBase
    {
        public int StoreRequisitionValidationArchiveId { get; set; }
        public int StoreRequisitionArchiveId { get; set; }
        public virtual StoreRequisitionArchive StoreRequisitionArchive { get; set; }
        public int StoreRequisitionValidationID { get; set; }
    }

    public class StoreRequisitionPropertyValidationArchive : StoreRequisitionPropertyValidationBase
    {
        public int StoreRequisitionPropertyValidationArchiveId { get; set; }
        public int StoreRequisitionArchiveId { get; set; }
        public virtual StoreRequisitionArchive StoreRequisitionArchive { get; set; }
        public int StoreRequisitionPropertyValidationID { get; set; }
    }

}