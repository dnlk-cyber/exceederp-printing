using System;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class LaboratoryTests
    {
        public int LaboratoryTestsId { get; set; }
        public int LaboratoryCatagoryId { get; set; }
        public string Test { get; set; }
        public string Reference { get; set; }
        public string Result { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
