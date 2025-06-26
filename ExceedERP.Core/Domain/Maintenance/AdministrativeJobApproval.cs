using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class AdministrativeJobApproval : TrackUserOperation
    {
       public int AdministrativeJobApprovalID { get; set; }
       public Guid AdministrativeJobId { get; set; }
       public ApprovalStatuses Status { get; set; }
       public string Remark { get; set; }
    }
    public class AdministrativeJobApprovalVM : TrackUserOperation
    {
       public int AdministrativeJobApprovalID { get; set; }
       public Guid AdministrativeJobId { get; set; }
       public ApprovalStatuses Status { get; set; }
        public string KMReading { get; set; }
        public string GPSInfo { get; set; }
        public string Remark { get; set; }
    }
      public class AdministrativeJobDepartmentApproval : TrackUserOperation
    {
       public int AdministrativeJobDepartmentApprovalID { get; set; }
       public Guid AdministrativeJobId { get; set; }
       public ApprovalStatuses Status { get; set; }
       public string Remark { get; set; }
    }
    public class CanibalizationDepartmentApproval : TrackUserOperation
    {
        [Key]
       public int CanibalizationDepartmentApprovalId { get; set; }
       public int CanibalizationId { get; set; }
       public ApprovalStatuses Status { get; set; }
       public string Remark { get; set; }
        //public string Desk { get; set; }

    }

    public class CanibalizationApproval : TrackUserOperation
    {
        public int CanibalizationApprovalId { get; set; }
        public int CanibalizationId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
