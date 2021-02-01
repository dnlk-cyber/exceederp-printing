using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Construction
{
    public class SortableEntityBase : UndoableUserOperationBase
    {
        public int OrderIndex { get; set; }
    }
}
