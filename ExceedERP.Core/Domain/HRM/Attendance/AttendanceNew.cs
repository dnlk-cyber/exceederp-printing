using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
   public class AttendanceNew
    {
        public int AttendanceNewId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Employee))]
        public int EmployeeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public DateTime Date { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public string DateAm { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Morning))]
        public bool IsPresentMorning { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Afternoon))]
        public bool IsPresentAfternoon { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.GLPeriod_GlFiscalYearId))]
        public int GlFisicalYearId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.GLPeriod_Name))]
        public int GlPeriodId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Department))]
        public int OrgStructureId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.IsDepartmentApproved))]
        public bool IsDepartmentApproved { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.IsDepartmentApproved))]
        public int DepartmentApproverId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.HR_SelfCare_TrainingApplicant_IsApproved))]
        public bool IsApproved { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.HR_SelfCare_TrainingApplicant_IsApproved))]
        public int ApproverEmpId { get; set; }
        [NotMapped]
        public GenerationType IsGenerated { get; set; }
        [Display(Name = "Morning Attendance")]
        public  TakeAttendance MorningAttendance { get; set; }
        [Display(Name = "Afternoon Attendance")]
        public TakeAttendance AfternoonAttendance { get; set; }
    }
    public enum TakeAttendance
    {
        Present,
        Absent,
        Leave,
        Field,
        Others
    }
   public class AttendanceNewVM
    {
        public int AttendanceNewId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Employee))]
        public int EmployeeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public DateTime Date { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public string DateAm { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Morning))]
        public bool IsPresentMorning { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Afternoon))]
        public bool IsPresentAfternoon { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.GLPeriod_GlFiscalYearId))]
        public int GlFisicalYearId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.GLPeriod_Name))]
        public int GlPeriodId { get; set; }
        [NotMapped]
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Department))]
        public int OrgStructureId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.IsDepartmentApproved))]
        public bool IsDepartmentApproved { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.IsDepartmentApproved))]
        public int DepartmentApproverId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.HR_SelfCare_TrainingApplicant_IsApproved))]
        public bool IsApproved { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.HR_SelfCare_TrainingApplicant_IsApproved))]
        public int ApproverEmpId { get; set; }
        [NotMapped]
        [Display(Name = "Is Generated")]
        public GenerationType IsGenerated { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Morning))]
        public bool IsPresentMorning1 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Afternoon))]
        public bool IsPresentAfternoon1 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public DateTime DayOneDate { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public string DayOneAmharicDate { get; set; }
        [Display(Name = "Morning ")]
        public TakeAttendance MorningAttendance1 { get; set; }
        [Display(Name = "Afternoon ")]
        public TakeAttendance AfternoonAttendance1 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Morning))]
        public bool IsPresentMorning2 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Afternoon))]
        public bool IsPresentAfternoon2 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public DateTime DayTwoDate { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public string DayTwoAmharicDate { get; set; }
        [Display(Name = "Morning ")]
        public TakeAttendance MorningAttendance2 { get; set; }
        [Display(Name = "Afternoon ")]
        public TakeAttendance AfternoonAttendance2 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Morning))]
        public bool IsPresentMorning3 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Afternoon))]
        public bool IsPresentAfternoon3 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public DateTime DayThreeDate { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public string DayThreeAmharicDate { get; set; }
        [Display(Name = "Morning ")]
        public TakeAttendance MorningAttendance3 { get; set; }
        [Display(Name = "Afternoon ")]
        public TakeAttendance AfternoonAttendance3 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Morning))]
        public bool IsPresentMorning4 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Afternoon))]
        public bool IsPresentAfternoon4 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public DateTime DayFourDate { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public string DayFourAmharicDate { get; set; }
        [Display(Name = "Morning ")]
        public TakeAttendance MorningAttendance4 { get; set; }
        [Display(Name = "Afternoon ")]
        public TakeAttendance AfternoonAttendance4 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Morning))]
        public bool IsPresentMorning5 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Afternoon))]
        public bool IsPresentAfternoon5 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public DateTime DayFiveDate { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public string DayFiveAmharicDate { get; set; }
        [Display(Name = "Morning ")]
        public TakeAttendance MorningAttendance5 { get; set; }
        [Display(Name = "Afternoon ")]
        public TakeAttendance AfternoonAttendance5 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Morning))]
        public bool IsPresentMorning6 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Afternoon))]
        public bool IsPresentAfternoon6 { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public DateTime DaySixDate { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Date))]
        public string DaySixAmharicDate { get; set; }
        [Display(Name = "Morning ")]
        public TakeAttendance MorningAttendance6 { get; set; }
        [Display(Name = "Afternoon ")]
        public TakeAttendance AfternoonAttendance6 { get; set; }
        //[Display(Name = "Morning Attendance")]
        //public TakeAttendance MorningAttendance { get; set; }
        //[Display(Name = "Afternoon Attendance")]
        //public TakeAttendance AfternoonAttendance { get; set; }

    }

    public enum GenerationType
    {
        New = 0,
        Existing = 1,
        Fail = 2,
        Future = 3
    }
}
