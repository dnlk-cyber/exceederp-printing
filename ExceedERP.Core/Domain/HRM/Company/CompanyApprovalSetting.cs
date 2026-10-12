using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class CompanyApprovalSetting: TrackUserOperation
    {
        public int CompanyApprovalSettingId { get; set; }

        public ApprovalModule Module { get; set; }

        public ApprovalRole Role { get; set; }

        public int StepOrder { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsMandatory { get; set; }
        public bool AllowAutoPlacement { get; set; }
    }
    public enum ApprovalRole
    {
        DGM,
        GM,
        CEO,
        Board,
        Supervisor,
        DepartmentManager,
        DepartmentHead,
        HROfficer,
        HRManager,
        HRAdmin,
        HumanResourceAdmin,
        HRAdministrationManager,
        FinanceOfficer,
        FinanceManager,
        DailLabour,
        ProjectDepartmentHead,
        ProjectManager,
        ProjectHRAdmin,
        PersonnelTimeKeeper,
        Foreman,
        OfficeEngineer,
        ConstructionAndOperation
    }

    public enum ApprovalModule
    {
        Transfer,
        ManpowerPlanning,
        Loan,
        AttendanceCorrection,
        Leave,
        Recruitment,
        DailLabour,
        Termination
    }

}
