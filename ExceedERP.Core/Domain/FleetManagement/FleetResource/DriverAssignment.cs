using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public class DriverAssignment : Operations
    {
        public Guid DriverAssignmentId { get; set; }
        [Required]
        [DisplayName("Fleet")]
        public Guid FleetId { get; set; }
        [Required]
        [DisplayName("Equipment Name")]
        public string EquipmentName { get; set; }
        [DisplayName(" From Operator")]
        public string FromOperatorName { get; set; }
        
        [DisplayName("To Operator")]
        public string ToOperatorName { get; set; }
        [DisplayName("Starting Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName("To Date")]
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ToDate { get; set; }

        public decimal Odometer { get; set; }
        [Display(Name ="Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }
        [Display(Name ="Operator One")]
        public Guid OperatorId { get; set; }
        [Display(Name = "Operator Two")]
        public Guid? SecondOperatorId { get; set; }
        [Display(Name ="Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameId { get; set; }

    }
    public class DriverAssignmentDocument
    {
        public int DriverAssignmentDocumentId { get; set; }
        public string DriverAssignmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
