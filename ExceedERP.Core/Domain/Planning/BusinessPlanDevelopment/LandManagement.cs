using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning.BusinessPlanDevelopment
{
    public class LandManagement: TrackUserSettingOperation
    {
        public int LandManagementId { get; set; }
        [Display(Name = "Particular Name of the land pilot")]
        public string ParticularNameLandPilot { get; set; }
        public string Zone { get; set; }
        [Display(Name = "City/Town")]
        public string CityOrTown { get; set; }
        [Display(Name = "Area(m*m)")]
        public string Area { get; set; }
        [Display(Name = "Types of Service")]
        public string TypesOfService { get; set; }
        [Display(Name = "Certifcate")]
        public LandHoldingCertifcate HoldingCertifcate { get; set; }
        [Display(Name = "Cert. No.")]
        public string LandHoldingCertifcateNo { get; set; }
        [Display(Name = " Acquired By")]
        public AcquiredBy AcquiredBy { get; set; }
        public IList<LandManagementAttachment> GetAttachments { get; set; }


    }
    public enum LandHoldingCertifcate
    {
        NotYet,
        Certified
    }
    public enum AcquiredBy
    {
        Lease,
        Rent
    }
    public class LandManagementAttachment
    {
        public int LandManagementAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int LandManagementId { get; set; }
        public virtual LandManagement Management { get; set; }
    }
}
