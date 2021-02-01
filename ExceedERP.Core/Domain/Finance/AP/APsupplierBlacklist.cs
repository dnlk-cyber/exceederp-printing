using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class APsupplierBlacklist : TrackUserOperation
    {
        public int APsupplierBlacklistID { get; set; }
        public int APSupplierRegistrationID { get; set; }
        public virtual APSupplierRegistration APSupplierRegistration { get; set; }
        //blacklisted or unblacklisted
        public string Opertion { get; set; }
        public DateTime Date { get; set; }
        public string Remark { get; set; }
    }
}
