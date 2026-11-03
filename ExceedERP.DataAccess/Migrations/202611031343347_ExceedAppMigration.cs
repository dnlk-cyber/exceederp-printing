namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExceedAppMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.AllowanceMapping", "LookupId", "Common.Lookup");
            DropForeignKey("HRM.OtherBenefits", "LookupId", "Common.Lookup");
            DropIndex("HRM.AllowanceMapping", new[] { "LookupId" });
            DropIndex("HRM.OtherBenefits", new[] { "LookupId" });
            CreateTable(
                "HRM.EducationFieldSettings",
                c => new
                    {
                        EducationFieldSettingId = c.Int(nullable: false, identity: true),
                        EducationField = c.String(),
                        EducationLevelId = c.Int(nullable: false),
                        RelevantFieldPoint = c.Int(nullable: false),
                        RelatedFieldPoint = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EducationFieldSettingId);
            
            CreateTable(
                "HRM.InternalApplicantEducations",
                c => new
                    {
                        InternalApplicantEducationId = c.Int(nullable: false, identity: true),
                        ApplicationFormId = c.Int(nullable: false),
                        InternalApplicantsId = c.Int(nullable: false),
                        EducationLevelSettingId = c.Int(nullable: false),
                        EducationFieldSettingId = c.Int(nullable: false),
                        EducationInstitution = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InternalApplicantEducationId);
            
            CreateTable(
                "HRM.InternalApplicantExperiences",
                c => new
                    {
                        InternalApplicantExperienceId = c.Int(nullable: false, identity: true),
                        ApplicationFormId = c.Int(nullable: false),
                        InternalApplicantsId = c.Int(nullable: false),
                        OrganizationType = c.Int(nullable: false),
                        JobTitleId = c.Int(nullable: false),
                        Jobtitle = c.Int(),
                        Year = c.Int(),
                        OrganizationName = c.String(),
                        From = c.DateTime(),
                        To = c.DateTime(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InternalApplicantExperienceId);
            
            AddColumn("HRM.AllowanceDefination", "Category", c => c.Int(nullable: false));
            AddColumn("HRM.AllowanceMapping", "BranchId", c => c.Int());
            AddColumn("HRM.AllowanceMapping", "RankId", c => c.Int());
            AddColumn("HRM.AllowanceTypeRule", "DeductOnAbsence", c => c.Boolean(nullable: false));
            AddColumn("HRM.AllowanceTypeRule", "DeductOnLeave", c => c.Boolean(nullable: false));
            AddColumn("HRM.ApplicantEducation", "GraduationDate", c => c.DateTime());
            AddColumn("HRM.ApplicantEducation", "CGPA", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.ApplicantExamResult", "ActualValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.ApplicantRegistration", "NumberOfApplicant", c => c.Int());
            AddColumn("EXCEED.ApplicantScreening", "TotalResult", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApplicantScreening", "Experience", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApplicantScreening", "Remark", c => c.String());
            AddColumn("EXCEED.ApplicantScreening", "Status", c => c.String());
            AddColumn("EXCEED.ApplicantScreening", "IsPassed", c => c.Boolean(nullable: false));
            AddColumn("HRM.EmployeeCards", "FromOrgStructureId", c => c.Int());
            AddColumn("HRM.EmployeeCards", "FromBusinessUnitId", c => c.Int());
            AddColumn("HRM.EmployeeCards", "FromBranchId", c => c.Int(nullable: false));
            AddColumn("HRM.EmployeeCards", "FromLookupId", c => c.Int());
            AddColumn("HRM.EmployeeCards", "ActionTakenBy", c => c.String());
            AddColumn("HRM.EmployeeCards", "ActionTakenDate", c => c.DateTime());
            AddColumn("HRM.InternalApplicantExamResults", "ActualValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.InternalApplicants", "BranchId", c => c.Int());
            AddColumn("HRM.InternalApplicants", "OrgStructureId", c => c.Int());
            AddColumn("HRM.InternalApplicants", "LookupId", c => c.Int());
            AddColumn("HRM.InternalApplicants", "BusinessUnitId", c => c.Int());
            AddColumn("HRM.ScreeningSettings", "IsBasedOnMaxExperience", c => c.Boolean(nullable: false));
            AddColumn("HRM.ScreeningSettings", "ApplyEducationRanking", c => c.Boolean(nullable: false));
            AddColumn("HRM.ScreeningSettings", "MiniExperiencePercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.ScreeningSettings", "IsPercentage", c => c.Boolean(nullable: false));
            AlterColumn("HRM.AllowanceMapping", "LookupId", c => c.Int());
            AlterColumn("HRM.EmployeeCards", "BusinessUnitId", c => c.Int());
            AlterColumn("HRM.OtherBenefits", "LookupId", c => c.Int());
            AlterColumn("HRM.OtherBenefits", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("HRM.AllowanceMapping", "LookupId");
            CreateIndex("HRM.OtherBenefits", "LookupId");
            AddForeignKey("HRM.AllowanceMapping", "LookupId", "Common.Lookup", "LookupId");
            AddForeignKey("HRM.OtherBenefits", "LookupId", "Common.Lookup", "LookupId");
            DropColumn("HRM.ScreeningSettings", "Gender");
            DropColumn("HRM.ScreeningSettings", "ExperienceYear");
            DropColumn("HRM.ScreeningSettings", "Language");
        }
        
        public override void Down()
        {
            AddColumn("HRM.ScreeningSettings", "Language", c => c.Int(nullable: false));
            AddColumn("HRM.ScreeningSettings", "ExperienceYear", c => c.Int(nullable: false));
            AddColumn("HRM.ScreeningSettings", "Gender", c => c.Int(nullable: false));
            DropForeignKey("HRM.OtherBenefits", "LookupId", "Common.Lookup");
            DropForeignKey("HRM.AllowanceMapping", "LookupId", "Common.Lookup");
            DropIndex("HRM.OtherBenefits", new[] { "LookupId" });
            DropIndex("HRM.AllowanceMapping", new[] { "LookupId" });
            AlterColumn("HRM.OtherBenefits", "Quantity", c => c.Int(nullable: false));
            AlterColumn("HRM.OtherBenefits", "LookupId", c => c.Int(nullable: false));
            AlterColumn("HRM.EmployeeCards", "BusinessUnitId", c => c.Int(nullable: false));
            AlterColumn("HRM.AllowanceMapping", "LookupId", c => c.Int(nullable: false));
            DropColumn("HRM.ScreeningSettings", "IsPercentage");
            DropColumn("HRM.ScreeningSettings", "MiniExperiencePercentage");
            DropColumn("HRM.ScreeningSettings", "ApplyEducationRanking");
            DropColumn("HRM.ScreeningSettings", "IsBasedOnMaxExperience");
            DropColumn("HRM.InternalApplicants", "BusinessUnitId");
            DropColumn("HRM.InternalApplicants", "LookupId");
            DropColumn("HRM.InternalApplicants", "OrgStructureId");
            DropColumn("HRM.InternalApplicants", "BranchId");
            DropColumn("HRM.InternalApplicantExamResults", "ActualValue");
            DropColumn("HRM.EmployeeCards", "ActionTakenDate");
            DropColumn("HRM.EmployeeCards", "ActionTakenBy");
            DropColumn("HRM.EmployeeCards", "FromLookupId");
            DropColumn("HRM.EmployeeCards", "FromBranchId");
            DropColumn("HRM.EmployeeCards", "FromBusinessUnitId");
            DropColumn("HRM.EmployeeCards", "FromOrgStructureId");
            DropColumn("EXCEED.ApplicantScreening", "IsPassed");
            DropColumn("EXCEED.ApplicantScreening", "Status");
            DropColumn("EXCEED.ApplicantScreening", "Remark");
            DropColumn("EXCEED.ApplicantScreening", "Experience");
            DropColumn("EXCEED.ApplicantScreening", "TotalResult");
            DropColumn("HRM.ApplicantRegistration", "NumberOfApplicant");
            DropColumn("HRM.ApplicantExamResult", "ActualValue");
            DropColumn("HRM.ApplicantEducation", "CGPA");
            DropColumn("HRM.ApplicantEducation", "GraduationDate");
            DropColumn("HRM.AllowanceTypeRule", "DeductOnLeave");
            DropColumn("HRM.AllowanceTypeRule", "DeductOnAbsence");
            DropColumn("HRM.AllowanceMapping", "RankId");
            DropColumn("HRM.AllowanceMapping", "BranchId");
            DropColumn("HRM.AllowanceDefination", "Category");
            DropTable("HRM.InternalApplicantExperiences");
            DropTable("HRM.InternalApplicantEducations");
            DropTable("HRM.EducationFieldSettings");
            CreateIndex("HRM.OtherBenefits", "LookupId");
            CreateIndex("HRM.AllowanceMapping", "LookupId");
            AddForeignKey("HRM.OtherBenefits", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("HRM.AllowanceMapping", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
        }
    }
}
