namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payroll : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Absence", "EmployeeCode", c => c.String());
            AddColumn("HRM.AllowanceDefination", "LookupId", c => c.Int(nullable: false));
            AddColumn("HRM.AllowancePerEmployee", "EmployeeCode", c => c.String());
            AddColumn("HRM.ApplicantionForm", "EmployeeCode", c => c.String());
            AddColumn("HRM.Attendance", "EmployeeCode", c => c.String());
            AddColumn("HRM.Clearance", "EmployeeCode", c => c.String());
            AddColumn("HRM.CourseTracking", "FromAmharic", c => c.String());
            AddColumn("HRM.CourseTracking", "ToAmharic", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "EmployeeCode", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "TransgressionDateAm", c => c.String());
            AddColumn("HRM.DisciplineMeasuresType", "IsByMoney", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasuresType", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.EmpDisciplinaryHearing", "HearingDateAmharic", c => c.String());
            AddColumn("HRM.EmpDisciplineMeasure", "DecisionDateAm", c => c.String());
            AddColumn("Payroll.EmpPayrollNoDaysWorked", "EmployeeCode", c => c.String());
            AddColumn("HRM.ExitInterview", "EmployeeCode", c => c.String());
            AddColumn("HRM.ExperienceRequest", "EmployeeCode", c => c.String());
            AddColumn("HRM.LateComming", "EmployeeCode", c => c.String());
            AddColumn("HRM.LeavePaid", "EmployeeCode", c => c.String());
            AddColumn("HRM.LeavePlan", "EmployeeCode", c => c.String());
            AddColumn("HRM.LeavePlan", "FromAmharic", c => c.String(nullable: false));
            AddColumn("HRM.LeavePlan", "ToAmharic", c => c.String(nullable: false));
            AddColumn("HRM.LeaveRequest", "EmployeeCode", c => c.String());
            AddColumn("HRM.LeaveRequest", "FromAmharic", c => c.String(nullable: false));
            AddColumn("HRM.LeaveRequest", "ToAmharic", c => c.String(nullable: false));
            AddColumn("HRM.Leave", "FromAmharic", c => c.String(nullable: false));
            AddColumn("HRM.Leave", "ToAmharic", c => c.String(nullable: false));
            AddColumn("HRM.LeaveEntitle", "EmployeeCode", c => c.String());
            AddColumn("HRM.LeaveTransfer", "EmployeeCode", c => c.String());
            AddColumn("Archive.IncommingLetter", "VolumeNo", c => c.String());
            AddColumn("Archive.IncommingLetter", "OutgoingVolumeNo", c => c.String());
            AddColumn("Health.MedicalRequest", "EmployeeCode", c => c.String());
            AddColumn("Health.MedicalRequest", "DateAmharic", c => c.String());
            AddColumn("HRM.OverTimeRequestEmployee", "EmployeeCode", c => c.String());
            AddColumn("Health.PatientCard", "DateAmharic", c => c.String());
            AddColumn("Health.PatientCardDetail", "DateAmharic", c => c.String(nullable: false));
            AddColumn("Health.PatientsLeave", "DateAmharic", c => c.String(nullable: false));
            AddColumn("Health.PatientsLeave", "FromAmharic", c => c.String(nullable: false));
            AddColumn("Health.PatientsLeave", "ToAmharic", c => c.String(nullable: false));
            AddColumn("Payroll.PayrollProcessAdditions", "AdditionId", c => c.Int(nullable: false));
            AddColumn("Health.Prescription", "DateAmharic", c => c.String());
            AddColumn("Health.ReferralForm", "ReferredDateAmharic", c => c.String());
            AddColumn("HRM.Termination", "EmployeeCode", c => c.String());
            AddColumn("HRM.Termination", "ApplicationDateAm", c => c.String());
            AddColumn("HRM.Termination", "TerminationDateAm", c => c.String());
            AddColumn("HRM.TrainingApplicant", "DateAmharic", c => c.String());
            CreateIndex("HRM.AllowanceDefination", "LookupId");
            //AddForeignKey("HRM.AllowanceDefination", "LookupId", "common.Lookup", "LookupId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.AllowanceDefination", "LookupId", "common.Lookup");
            DropIndex("HRM.AllowanceDefination", new[] { "LookupId" });
            DropColumn("HRM.TrainingApplicant", "DateAmharic");
            DropColumn("HRM.Termination", "TerminationDateAm");
            DropColumn("HRM.Termination", "ApplicationDateAm");
            DropColumn("HRM.Termination", "EmployeeCode");
            DropColumn("Health.ReferralForm", "ReferredDateAmharic");
            DropColumn("Health.Prescription", "DateAmharic");
            DropColumn("Payroll.PayrollProcessAdditions", "AdditionId");
            DropColumn("Health.PatientsLeave", "ToAmharic");
            DropColumn("Health.PatientsLeave", "FromAmharic");
            DropColumn("Health.PatientsLeave", "DateAmharic");
            DropColumn("Health.PatientCardDetail", "DateAmharic");
            DropColumn("Health.PatientCard", "DateAmharic");
            DropColumn("HRM.OverTimeRequestEmployee", "EmployeeCode");
            DropColumn("Health.MedicalRequest", "DateAmharic");
            DropColumn("Health.MedicalRequest", "EmployeeCode");
            DropColumn("Archive.IncommingLetter", "OutgoingVolumeNo");
            DropColumn("Archive.IncommingLetter", "VolumeNo");
            DropColumn("HRM.LeaveTransfer", "EmployeeCode");
            DropColumn("HRM.LeaveEntitle", "EmployeeCode");
            DropColumn("HRM.Leave", "ToAmharic");
            DropColumn("HRM.Leave", "FromAmharic");
            DropColumn("HRM.LeaveRequest", "ToAmharic");
            DropColumn("HRM.LeaveRequest", "FromAmharic");
            DropColumn("HRM.LeaveRequest", "EmployeeCode");
            DropColumn("HRM.LeavePlan", "ToAmharic");
            DropColumn("HRM.LeavePlan", "FromAmharic");
            DropColumn("HRM.LeavePlan", "EmployeeCode");
            DropColumn("HRM.LeavePaid", "EmployeeCode");
            DropColumn("HRM.LateComming", "EmployeeCode");
            DropColumn("HRM.ExperienceRequest", "EmployeeCode");
            DropColumn("HRM.ExitInterview", "EmployeeCode");
            DropColumn("Payroll.EmpPayrollNoDaysWorked", "EmployeeCode");
            DropColumn("HRM.EmpDisciplineMeasure", "DecisionDateAm");
            DropColumn("HRM.EmpDisciplinaryHearing", "HearingDateAmharic");
            DropColumn("HRM.DisciplineMeasuresType", "Amount");
            DropColumn("HRM.DisciplineMeasuresType", "IsByMoney");
            DropColumn("HRM.DisciplinaryCase", "TransgressionDateAm");
            DropColumn("HRM.DisciplinaryCase", "EmployeeCode");
            DropColumn("HRM.CourseTracking", "ToAmharic");
            DropColumn("HRM.CourseTracking", "FromAmharic");
            DropColumn("HRM.Clearance", "EmployeeCode");
            DropColumn("HRM.Attendance", "EmployeeCode");
            DropColumn("HRM.ApplicantionForm", "EmployeeCode");
            DropColumn("HRM.AllowancePerEmployee", "EmployeeCode");
            DropColumn("HRM.AllowanceDefination", "LookupId");
            DropColumn("HRM.Absence", "EmployeeCode");
        }
    }
}
