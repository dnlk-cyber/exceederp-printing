namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class localization : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy");
            DropIndex("HRM.ApplicantRegistration", new[] { "VaccancyCode" });
            AddColumn("HRM.RecruitmentPlan", "JobtitleName", c => c.String());
            AddColumn("HRM.Placement", "RankName", c => c.String());
            AddColumn("HRM.Placement", "StepName", c => c.String());
            AlterColumn("HRM.AllowanceDefination", "AllowanceName", c => c.String());
            AlterColumn("HRM.ApplicantExperience", "OrganizationName", c => c.String());
            AlterColumn("HRM.ApplicantExperience", "Postion", c => c.String());
            AlterColumn("HRM.ApplicantRegistration", "VaccancyCode", c => c.String(maxLength: 128));
            AlterColumn("HRM.ApplicantRegistration", "ApplicantName", c => c.String());
            AlterColumn("HRM.ApplicantRegistration", "RegistrationPalce", c => c.String());
            AlterColumn("HRM.ApplicantRegistration", "RegisterBy", c => c.String());
            AlterColumn("HRM.Rank", "RankName", c => c.String());
            AlterColumn("HRM.Commitment", "Reason", c => c.String());
            AlterColumn("HRM.Commitment", "Value", c => c.String());
            AlterColumn("HRM.Committee", "Role", c => c.String());
            AlterColumn("HRM.Committee", "CommitteeType", c => c.String());
            AlterColumn("HRM.Cosign", "FullName", c => c.String());
            AlterColumn("HRM.Cosign", "BirthPlace", c => c.String());
            AlterColumn("HRM.Disability", "Type", c => c.String());
            AlterColumn("HRM.Education", "OrganizationName", c => c.String());
            AlterColumn("HRM.Education", "OrgAddress", c => c.String());
            AlterColumn("HRM.Training", "OrganizationName", c => c.String());
            AlterColumn("HRM.Training", "OrgAddress", c => c.String());
            AlterColumn("HRM.Experience", "Organozation", c => c.String());
            AlterColumn("HRM.Experience", "OrgFullAddress", c => c.String());
            AlterColumn("HRM.Experience", "EmploymentType", c => c.String());
            AlterColumn("HRM.OtherBenefit", "TimeInterval", c => c.String());
            AlterColumn("HRM.PlacementHistory", "JobTitle", c => c.String());
            AlterColumn("HRM.PlacementHistory", "Rank", c => c.String());
            AlterColumn("HRM.PlacementHistory", "Step", c => c.String());
            AlterColumn("HRM.Placement", "JobTitleName", c => c.String());
            AlterColumn("HRM.Step", "StepName", c => c.String());
            CreateIndex("HRM.ApplicantRegistration", "VaccancyCode");
            AddForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy", "VaccancyCode");
            DropColumn("HRM.AllowancePerEmployee", "OrgBranchName");
            DropColumn("HRM.AllowancePerEmployee", "DateCreated");
            DropColumn("HRM.AllowancePerEmployee", "LastModified");
            DropColumn("HRM.AllowancePerEmployee", "CreatedBy");
            DropColumn("HRM.AllowancePerEmployee", "ModifiedBy");
            DropColumn("HRM.AllowanceTypeRule", "OrgBranchName");
            DropColumn("HRM.AllowanceTypeRule", "DateCreated");
            DropColumn("HRM.AllowanceTypeRule", "LastModified");
            DropColumn("HRM.AllowanceTypeRule", "CreatedBy");
            DropColumn("HRM.AllowanceTypeRule", "ModifiedBy");
        }
        
        public override void Down()
        {
            AddColumn("HRM.AllowanceTypeRule", "ModifiedBy", c => c.String());
            AddColumn("HRM.AllowanceTypeRule", "CreatedBy", c => c.String());
            AddColumn("HRM.AllowanceTypeRule", "LastModified", c => c.DateTime());
            AddColumn("HRM.AllowanceTypeRule", "DateCreated", c => c.DateTime());
            AddColumn("HRM.AllowanceTypeRule", "OrgBranchName", c => c.String());
            AddColumn("HRM.AllowancePerEmployee", "ModifiedBy", c => c.String());
            AddColumn("HRM.AllowancePerEmployee", "CreatedBy", c => c.String());
            AddColumn("HRM.AllowancePerEmployee", "LastModified", c => c.DateTime());
            AddColumn("HRM.AllowancePerEmployee", "DateCreated", c => c.DateTime());
            AddColumn("HRM.AllowancePerEmployee", "OrgBranchName", c => c.String());
            DropForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy");
            DropIndex("HRM.ApplicantRegistration", new[] { "VaccancyCode" });
            AlterColumn("HRM.Step", "StepName", c => c.String(nullable: false));
            AlterColumn("HRM.Placement", "JobTitleName", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "Step", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "Rank", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "JobTitle", c => c.String(nullable: false));
            AlterColumn("HRM.OtherBenefit", "TimeInterval", c => c.String(nullable: false));
            AlterColumn("HRM.Experience", "EmploymentType", c => c.String(nullable: false));
            AlterColumn("HRM.Experience", "OrgFullAddress", c => c.String(nullable: false));
            AlterColumn("HRM.Experience", "Organozation", c => c.String(nullable: false));
            AlterColumn("HRM.Training", "OrgAddress", c => c.String(nullable: false));
            AlterColumn("HRM.Training", "OrganizationName", c => c.String(nullable: false));
            AlterColumn("HRM.Education", "OrgAddress", c => c.String(nullable: false));
            AlterColumn("HRM.Education", "OrganizationName", c => c.String(nullable: false));
            AlterColumn("HRM.Disability", "Type", c => c.String(nullable: false));
            AlterColumn("HRM.Cosign", "BirthPlace", c => c.String(nullable: false));
            AlterColumn("HRM.Cosign", "FullName", c => c.String(nullable: false));
            AlterColumn("HRM.Committee", "CommitteeType", c => c.String(nullable: false));
            AlterColumn("HRM.Committee", "Role", c => c.String(nullable: false));
            AlterColumn("HRM.Commitment", "Value", c => c.String(nullable: false));
            AlterColumn("HRM.Commitment", "Reason", c => c.String(nullable: false));
            AlterColumn("HRM.Rank", "RankName", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantRegistration", "RegisterBy", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantRegistration", "RegistrationPalce", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantRegistration", "ApplicantName", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantRegistration", "VaccancyCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("HRM.ApplicantExperience", "Postion", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantExperience", "OrganizationName", c => c.String(nullable: false));
            AlterColumn("HRM.AllowanceDefination", "AllowanceName", c => c.String(nullable: false));
            DropColumn("HRM.Placement", "StepName");
            DropColumn("HRM.Placement", "RankName");
            DropColumn("HRM.RecruitmentPlan", "JobtitleName");
            CreateIndex("HRM.ApplicantRegistration", "VaccancyCode");
            AddForeignKey("HRM.ApplicantRegistration", "VaccancyCode", "HRM.Vacancy", "VaccancyCode", cascadeDelete: true);
        }
    }
}
