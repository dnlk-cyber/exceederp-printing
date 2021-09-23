using ExceedERP.Core.Domain.Common;
using System;

namespace ExceedERP.Core.Domain.Construction
{
    public class UndoableUserOperationBase : TrackUserOperation
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
    }
}
