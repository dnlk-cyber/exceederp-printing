using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore
{
            public class CannibalizationApproval   : TrackUserOperation
            {
                public int CannibalizationApprovalID { get; set; }
                public Guid CannibalizationId { get; set; }
                public virtual Cannibalization Cannibalization { get; set; }
                public string Status { get; set; }
                public string Remark { get; set; }
            }

        }
