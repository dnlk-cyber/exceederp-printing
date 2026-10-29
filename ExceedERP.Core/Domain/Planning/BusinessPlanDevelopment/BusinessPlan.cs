using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning.BusinessPlanDevelopment
{
    public class BusinessPlan: TrackUserSettingOperation
    {
        public int BusinessPlanId { get; set; }
        public string NameOfBusinessPlan  { get; set; }
        public ProductTypeOfPlan ProductType { get; set; }
        public MethodOfStudy StudyMethod { get; set; }
        public PlanStatus Status { get; set; }

    }
    public enum ProductTypeOfPlan
    {
        Construction,
        Production,
        Industry,
        Service
    }
    public enum MethodOfStudy
    {
        OwnForce,
        OutSource
    }
    public enum PlanStatus
    {
        OnProcess,
        Approved
    }
    public class BusinessPlanValidation
    {
        public int BusinessPlanValidationId { get; set; }
        public PlanStatus Status { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Description;
        public int BusinessPlanId { get; set; }
        public virtual BusinessPlan Business { get; set; }
    }
    public class BusinessPlanAttachment
    {
        public int BusinessPlanAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int BusinessPlanId { get; set; }
        public virtual BusinessPlan Business { get; set; }
    }

}
