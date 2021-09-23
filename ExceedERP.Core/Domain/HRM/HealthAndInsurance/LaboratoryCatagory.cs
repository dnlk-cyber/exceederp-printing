using System;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class LaboratoryCatagory
    {
        public int LaboratoryCatagoryId { get; set; }
        public string Catagory { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
