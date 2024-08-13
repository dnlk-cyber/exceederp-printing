using System;

namespace ExceedERP.Core.Domain.Health
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
