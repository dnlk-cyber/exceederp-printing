namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class localization1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("HRM.ApplicantionForm", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("Health.PatientCardDetail", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientsLeave", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.Prescription", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.ReferralSheet", "PatientCardId", "Health.PatientCard");
            DropIndex("HRM.ApplicantRegistration", new[] { "VaccancyCode" });
            DropIndex("HRM.ApplicantionForm", new[] { "VaccancyCode" });
            DropIndex("Health.PatientCardDetail", new[] { "PatientCardId" });
            DropIndex("Health.PatientsLeave", new[] { "PatientCardId" });
            DropIndex("Health.Prescription", new[] { "PatientCardId" });
            DropIndex("Health.ReferralSheet", new[] { "PatientCardId" });
            CreateTable(
                "HRM.JobTitleMappingDetail",
                c => new
                    {
                        JobTitleMappingDetailId = c.Int(nullable: false, identity: true),
                        MainFunction = c.String(),
                        LookupId = c.Int(nullable: false),
                        RelevantExp = c.String(),
                        Experience = c.String(),
                        SpecialSkill = c.String(),
                        PlannedSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        JobTitleMappingId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobTitleMappingDetailId)
                .ForeignKey("HRM.JobTitleMapping", t => t.JobTitleMappingId, cascadeDelete: true)
                .Index(t => t.JobTitleMappingId);
            
            AlterColumn("HRM.Employee", "EmployeeCode", c => c.String(nullable: false));
            AlterColumn("HRM.Employee", "FirstName", c => c.String(nullable: false));
            AlterColumn("HRM.Employee", "MiddleName", c => c.String(nullable: false));
            AlterColumn("HRM.Employee", "LastName", c => c.String(nullable: false));
            AlterColumn("HRM.Employee", "BirthPlace", c => c.String(nullable: false));
            AlterColumn("common.Period", "PeriodName", c => c.String(nullable: false));
            AlterColumn("Payroll.Advance", "Description", c => c.String(nullable: false));
            AlterColumn("HRM.AllowanceDefination", "AllowanceName", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantExperience", "OrganizationName", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantExperience", "Postion", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantRegistration", "VaccancyCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("HRM.ApplicantRegistration", "ApplicantName", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantRegistration", "RegistrationPalce", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantRegistration", "RegisterBy", c => c.String(nullable: false));
            AlterColumn("HRM.RecruitmentPlan", "EduLevel", c => c.String());
            AlterColumn("HRM.Structure", "Name", c => c.String(nullable: false));
            AlterColumn("HRM.Rank", "RankName", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantionForm", "VaccancyCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("HRM.Branch", "Name", c => c.String(nullable: false));
            AlterColumn("HRM.Clearance", "ReasonOfTermination", c => c.String(nullable: false));
            AlterColumn("HRM.Commitment", "Reason", c => c.String(nullable: false));
            AlterColumn("HRM.Commitment", "Value", c => c.String(nullable: false));
            AlterColumn("HRM.Committee", "Role", c => c.String(nullable: false));
            AlterColumn("HRM.Committee", "CommitteeType", c => c.String(nullable: false));
            AlterColumn("HRM.MainCompany", "TradeName", c => c.String(nullable: false));
            AlterColumn("common.Configuration", "Reference", c => c.String(nullable: false));
            AlterColumn("common.Configuration", "Attribute", c => c.String(nullable: false));
            AlterColumn("common.Configuration", "CurrentValue", c => c.String(nullable: false));
            AlterColumn("HRM.Cosign", "FullName", c => c.String(nullable: false));
            AlterColumn("HRM.Cosign", "BirthPlace", c => c.String(nullable: false));
            AlterColumn("HRM.Disability", "Type", c => c.String(nullable: false));
            AlterColumn("HRM.DisciplinaryCase", "NatureofTransgression", c => c.String(nullable: false));
            AlterColumn("HRM.DisciplinaryCase", "Evidence", c => c.String(nullable: false));
            AlterColumn("HRM.DisciplineMeasures", "MeasuresType", c => c.String(nullable: false));
            AlterColumn("HRM.DisciplineMeasuresDetail", "Description", c => c.String(nullable: false));
            AlterColumn("HRM.Education", "OrganizationName", c => c.String(nullable: false));
            AlterColumn("HRM.Education", "OrgAddress", c => c.String(nullable: false));
            AlterColumn("HRM.EmpDisciplinaryHearing", "HearingOutcome", c => c.String(nullable: false));
            AlterColumn("Performance.Measurement", "MeasurementName", c => c.String(nullable: false));
            AlterColumn("Performance.MeasurementDefinition", "EvaluationCriteria", c => c.String(nullable: false));
            AlterColumn("Performance.PerformanceAction", "ActionName", c => c.String(nullable: false));
            AlterColumn("HRM.Training", "OrganizationName", c => c.String(nullable: false));
            AlterColumn("HRM.Training", "OrgAddress", c => c.String(nullable: false));
            AlterColumn("HRM.Experience", "Organozation", c => c.String(nullable: false));
            AlterColumn("HRM.Experience", "OrgFullAddress", c => c.String(nullable: false));
            AlterColumn("HRM.Experience", "EmploymentType", c => c.String(nullable: false));
            AlterColumn("HRM.Incomming", "LettersRefNo", c => c.String(nullable: false));
            AlterColumn("HRM.Incomming", "Owner", c => c.String(nullable: false));
            AlterColumn("HRM.Incomming", "Subject", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "FileNo", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "InstitutionName", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "Shelve", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "Row", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "Folder", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionType", "InstitutionName", c => c.String(nullable: false));
            AlterColumn("Archive.Zone", "ZoneName", c => c.String(nullable: false));
            AlterColumn("Insurance.Insurance", "InsuranceCorporation", c => c.String(nullable: false));
            AlterColumn("Insurance.InsuranceGroup", "GroupName", c => c.String(nullable: false));
            AlterColumn("Archive.IncommingLetter", "LetterReferenceNo", c => c.String(nullable: false));
            AlterColumn("Archive.IncommingLetter", "OwnerName", c => c.String(nullable: false));
            AlterColumn("Archive.IncommingLetter", "Subject", c => c.String(nullable: false));
            AlterColumn("Archive.IncommingLetter", "Shelve", c => c.String(nullable: false));
            AlterColumn("Archive.IncommingLetter", "Row", c => c.String(nullable: false));
            AlterColumn("Archive.IncommingLetter", "FolderNo", c => c.String(nullable: false));
            AlterColumn("Archive.IncommingLetter", "FileNo", c => c.String(nullable: false));
            AlterColumn("Payroll.Loan", "Description", c => c.String(nullable: false));
            AlterColumn("Archive.OutgoingLetter", "BookNo", c => c.String(nullable: false));
            AlterColumn("Archive.OutgoingLetter", "LetterReferenceNo", c => c.String(nullable: false));
            AlterColumn("Archive.OutgoingLetter", "OwnerName", c => c.String(nullable: false));
            AlterColumn("Archive.OutgoingLetter", "Subject", c => c.String(nullable: false));
            AlterColumn("Archive.OutgoingLetter", "Shelve", c => c.String(nullable: false));
            AlterColumn("Archive.OutgoingLetter", "Row", c => c.String(nullable: false));
            AlterColumn("Archive.OutgoingLetter", "FolderNo", c => c.String(nullable: false));
            AlterColumn("Archive.OutgoingLetter", "FileNo", c => c.String(nullable: false));
            AlterColumn("HRM.OtherBenefit", "TimeInterval", c => c.String(nullable: false));
            AlterColumn("HRM.Outgoing", "Name", c => c.String(nullable: false));
            AlterColumn("HRM.Outgoing", "Owner", c => c.String(nullable: false));
            AlterColumn("Health.PatientCardDetail", "PatientCardId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("Health.PatientsLeave", "PatientCardId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("Payroll.PayrollEmployeeAccount", "CostCenterAccountCode", c => c.String(nullable: false));
            AlterColumn("Payroll.PerformanceRank", "Description", c => c.String(maxLength: 100));
            AlterColumn("Archive.PersonalArchiveNo", "Folder", c => c.String(nullable: false));
            AlterColumn("Archive.PersonalArchiveNo", "Shelve", c => c.String(nullable: false));
            AlterColumn("Archive.PersonalArchiveNo", "Row", c => c.String(nullable: false));
            AlterColumn("Archive.PersonalArchiveNo", "FileNo", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "JobTitle", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "Rank", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "Step", c => c.String(nullable: false));
            AlterColumn("HRM.Placement", "JobTitleName", c => c.String(nullable: false));
            AlterColumn("HRM.Step", "StepName", c => c.String(nullable: false));
            AlterColumn("Health.Prescription", "PatientCardId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("Health.ReferralSheet", "PatientCardId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("HRM.ApplicantRegistration", "VaccancyCode");
            CreateIndex("HRM.ApplicantionForm", "VaccancyCode");
            CreateIndex("Health.PatientCardDetail", "PatientCardId");
            CreateIndex("Health.PatientsLeave", "PatientCardId");
            CreateIndex("Health.Prescription", "PatientCardId");
            CreateIndex("Health.ReferralSheet", "PatientCardId");
            AddForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy", "VaccancyCode", cascadeDelete: true);
            AddForeignKey("HRM.ApplicantionForm", "VaccancyCode", "HRM.Vacancy", "VaccancyCode", cascadeDelete: true);
            AddForeignKey("Health.PatientCardDetail", "PatientCardId", "Health.PatientCard", "PatientCardId", cascadeDelete: true);
            AddForeignKey("Health.PatientsLeave", "PatientCardId", "Health.PatientCard", "PatientCardId", cascadeDelete: true);
            AddForeignKey("Health.Prescription", "PatientCardId", "Health.PatientCard", "PatientCardId", cascadeDelete: true);
            AddForeignKey("Health.ReferralSheet", "PatientCardId", "Health.PatientCard", "PatientCardId", cascadeDelete: true);
            DropColumn("HRM.JobTitleMapping", "MainFunction");
        }
        
        public override void Down()
        {
            AddColumn("HRM.JobTitleMapping", "MainFunction", c => c.String());
            DropForeignKey("Health.ReferralSheet", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.Prescription", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientsLeave", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientCardDetail", "PatientCardId", "Health.PatientCard");
            DropForeignKey("HRM.ApplicantionForm", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy");
            DropForeignKey("HRM.JobTitleMappingDetail", "JobTitleMappingId", "HRM.JobTitleMapping");
            DropIndex("Health.ReferralSheet", new[] { "PatientCardId" });
            DropIndex("Health.Prescription", new[] { "PatientCardId" });
            DropIndex("Health.PatientsLeave", new[] { "PatientCardId" });
            DropIndex("Health.PatientCardDetail", new[] { "PatientCardId" });
            DropIndex("HRM.JobTitleMappingDetail", new[] { "JobTitleMappingId" });
            DropIndex("HRM.ApplicantionForm", new[] { "VaccancyCode" });
            DropIndex("HRM.ApplicantRegistration", new[] { "VaccancyCode" });
            AlterColumn("Health.ReferralSheet", "PatientCardId", c => c.String(maxLength: 128));
            AlterColumn("Health.Prescription", "PatientCardId", c => c.String(maxLength: 128));
            AlterColumn("HRM.Step", "StepName", c => c.String());
            AlterColumn("HRM.Placement", "JobTitleName", c => c.String());
            AlterColumn("HRM.PlacementHistory", "Step", c => c.String());
            AlterColumn("HRM.PlacementHistory", "Rank", c => c.String());
            AlterColumn("HRM.PlacementHistory", "JobTitle", c => c.String());
            AlterColumn("Archive.PersonalArchiveNo", "FileNo", c => c.String());
            AlterColumn("Archive.PersonalArchiveNo", "Row", c => c.String());
            AlterColumn("Archive.PersonalArchiveNo", "Shelve", c => c.String());
            AlterColumn("Archive.PersonalArchiveNo", "Folder", c => c.String());
            AlterColumn("Payroll.PerformanceRank", "Description", c => c.String());
            AlterColumn("Payroll.PayrollEmployeeAccount", "CostCenterAccountCode", c => c.String());
            AlterColumn("Health.PatientsLeave", "PatientCardId", c => c.String(maxLength: 128));
            AlterColumn("Health.PatientCardDetail", "PatientCardId", c => c.String(maxLength: 128));
            AlterColumn("HRM.Outgoing", "Owner", c => c.String());
            AlterColumn("HRM.Outgoing", "Name", c => c.String());
            AlterColumn("HRM.OtherBenefit", "TimeInterval", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "FileNo", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "FolderNo", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "Row", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "Shelve", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "Subject", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "OwnerName", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "LetterReferenceNo", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "BookNo", c => c.String());
            AlterColumn("Payroll.Loan", "Description", c => c.String());
            AlterColumn("Archive.IncommingLetter", "FileNo", c => c.String());
            AlterColumn("Archive.IncommingLetter", "FolderNo", c => c.String());
            AlterColumn("Archive.IncommingLetter", "Row", c => c.String());
            AlterColumn("Archive.IncommingLetter", "Shelve", c => c.String());
            AlterColumn("Archive.IncommingLetter", "Subject", c => c.String());
            AlterColumn("Archive.IncommingLetter", "OwnerName", c => c.String());
            AlterColumn("Archive.IncommingLetter", "LetterReferenceNo", c => c.String());
            AlterColumn("Insurance.InsuranceGroup", "GroupName", c => c.String());
            AlterColumn("Insurance.Insurance", "InsuranceCorporation", c => c.String());
            AlterColumn("Archive.Zone", "ZoneName", c => c.String());
            AlterColumn("Archive.InstitutionType", "InstitutionName", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "Folder", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "Row", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "Shelve", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "InstitutionName", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "FileNo", c => c.String());
            AlterColumn("HRM.Incomming", "Subject", c => c.String());
            AlterColumn("HRM.Incomming", "Owner", c => c.String());
            AlterColumn("HRM.Incomming", "LettersRefNo", c => c.String());
            AlterColumn("HRM.Experience", "EmploymentType", c => c.String());
            AlterColumn("HRM.Experience", "OrgFullAddress", c => c.String());
            AlterColumn("HRM.Experience", "Organozation", c => c.String());
            AlterColumn("HRM.Training", "OrgAddress", c => c.String());
            AlterColumn("HRM.Training", "OrganizationName", c => c.String());
            AlterColumn("Performance.PerformanceAction", "ActionName", c => c.String());
            AlterColumn("Performance.MeasurementDefinition", "EvaluationCriteria", c => c.String());
            AlterColumn("Performance.Measurement", "MeasurementName", c => c.String());
            AlterColumn("HRM.EmpDisciplinaryHearing", "HearingOutcome", c => c.String());
            AlterColumn("HRM.Education", "OrgAddress", c => c.String());
            AlterColumn("HRM.Education", "OrganizationName", c => c.String());
            AlterColumn("HRM.DisciplineMeasuresDetail", "Description", c => c.String());
            AlterColumn("HRM.DisciplineMeasures", "MeasuresType", c => c.String());
            AlterColumn("HRM.DisciplinaryCase", "Evidence", c => c.String());
            AlterColumn("HRM.DisciplinaryCase", "NatureofTransgression", c => c.String());
            AlterColumn("HRM.Disability", "Type", c => c.String());
            AlterColumn("HRM.Cosign", "BirthPlace", c => c.String());
            AlterColumn("HRM.Cosign", "FullName", c => c.String());
            AlterColumn("common.Configuration", "CurrentValue", c => c.String());
            AlterColumn("common.Configuration", "Attribute", c => c.String());
            AlterColumn("common.Configuration", "Reference", c => c.String());
            AlterColumn("HRM.MainCompany", "TradeName", c => c.String());
            AlterColumn("HRM.Committee", "CommitteeType", c => c.String());
            AlterColumn("HRM.Committee", "Role", c => c.String());
            AlterColumn("HRM.Commitment", "Value", c => c.String());
            AlterColumn("HRM.Commitment", "Reason", c => c.String());
            AlterColumn("HRM.Clearance", "ReasonOfTermination", c => c.String());
            AlterColumn("HRM.Branch", "Name", c => c.String());
            AlterColumn("HRM.ApplicantionForm", "VaccancyCode", c => c.String(maxLength: 128));
            AlterColumn("HRM.Rank", "RankName", c => c.String());
            AlterColumn("HRM.Structure", "Name", c => c.String());
            AlterColumn("HRM.RecruitmentPlan", "EduLevel", c => c.Int(nullable: false));
            AlterColumn("HRM.ApplicantRegistration", "RegisterBy", c => c.String());
            AlterColumn("HRM.ApplicantRegistration", "RegistrationPalce", c => c.String());
            AlterColumn("HRM.ApplicantRegistration", "ApplicantName", c => c.String());
            AlterColumn("HRM.ApplicantRegistration", "VaccancyCode", c => c.String(maxLength: 128));
            AlterColumn("HRM.ApplicantExperience", "Postion", c => c.String());
            AlterColumn("HRM.ApplicantExperience", "OrganizationName", c => c.String());
            AlterColumn("HRM.AllowanceDefination", "AllowanceName", c => c.String());
            AlterColumn("Payroll.Advance", "Description", c => c.String());
            AlterColumn("common.Period", "PeriodName", c => c.String());
            AlterColumn("HRM.Employee", "BirthPlace", c => c.String());
            AlterColumn("HRM.Employee", "LastName", c => c.String());
            AlterColumn("HRM.Employee", "MiddleName", c => c.String());
            AlterColumn("HRM.Employee", "FirstName", c => c.String());
            AlterColumn("HRM.Employee", "EmployeeCode", c => c.String());
            DropTable("HRM.JobTitleMappingDetail");
            CreateIndex("Health.ReferralSheet", "PatientCardId");
            CreateIndex("Health.Prescription", "PatientCardId");
            CreateIndex("Health.PatientsLeave", "PatientCardId");
            CreateIndex("Health.PatientCardDetail", "PatientCardId");
            CreateIndex("HRM.ApplicantionForm", "VaccancyCode");
            CreateIndex("HRM.ApplicantRegistration", "VaccancyCode");
            AddForeignKey("Health.ReferralSheet", "PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.Prescription", "PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.PatientsLeave", "PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.PatientCardDetail", "PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("HRM.ApplicantionForm", "VaccancyCode", "HRM.Vacancy", "VaccancyCode");
            AddForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy", "VaccancyCode");
        }
    }
}
