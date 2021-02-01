using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>   A job title code. </summary>
    ///Used to Store Employees Job Title Code as per thier postion
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class JobTitleCode
    {
        [Key]
        public int JobTitleCodeId { get; set; }
        [Required]
        [Display(Name = "Postion")]
        public int TitleCode { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }

        public string Remark { get; set; }
    }
}