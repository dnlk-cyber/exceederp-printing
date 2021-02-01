namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerg : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.BranchCostCenter", newSchema: "HRM");
            DropForeignKey("Archive.InstitutionArchiveNo", "ZoneId", "Archive.Zone");
            DropForeignKey("Archive.PersonalArchiveNo", "ZoneId", "Archive.Zone");
            DropIndex("Archive.InstitutionArchiveNo", new[] { "ZoneId" });
            DropIndex("Archive.PersonalArchiveNo", new[] { "ZoneId" });
            CreateTable(
                "HRM.ApplicantEducation",
                c => new
                    {
                        ApplicantEducationId = c.Int(nullable: false, identity: true),
                        ApplicantRegistrationId = c.Int(nullable: false),
                        EduLevel = c.Int(nullable: false),
                        EducField = c.Int(nullable: false),
                        EducationInstitution = c.String(nullable: false),
                        From = c.DateTime(nullable: false),
                        FromAmharic = c.String(),
                        To = c.DateTime(nullable: false),
                        ToAmahric = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ApplicantEducationId)
                .ForeignKey("HRM.ApplicantRegistration", t => t.ApplicantRegistrationId, cascadeDelete: true)
                .Index(t => t.ApplicantRegistrationId);
            
            AddColumn("Insurance.AccidentRecord", "DateAmharic", c => c.String());
            AddColumn("HRM.ApplicantExperience", "FromAmharic", c => c.String());
            AddColumn("HRM.ApplicantExperience", "ToAmahric", c => c.String());
            AddColumn("HRM.Vacancy", "VacancyType", c => c.Int(nullable: false));
            AddColumn("HRM.Branch", "BranchTypes", c => c.Int(nullable: false));
            AddColumn("Insurance.CourtInsurance", "DateAssocAmharic", c => c.String());
            AddColumn("Insurance.CourtInsurance", "DateInsuAmhairc", c => c.String());
            AddColumn("HRM.Incomming", "LetterDateAmmharic", c => c.String());
            AddColumn("HRM.Incomming", "RecivedDateAmharic", c => c.String());
            AddColumn("HRM.IncommingStatus", "DateAmharic", c => c.String());
            AddColumn("Insurance.Insurance", "FromAmharic", c => c.String());
            AddColumn("Insurance.Insurance", "ToAmharic", c => c.String());
            AddColumn("EXCEED.PlanItem", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.PlanItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.PlanItem", "BudgetSource", c => c.Int(nullable: false));
            AddColumn("EXCEED.PlanItem", "ProcurementCategory", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveTransfer", "Check", c => c.Boolean(nullable: false));
            AddColumn("Archive.LetterSender", "DateAmharic", c => c.String());
            AddColumn("Archive.IncommingLetter", "LetterAmharic", c => c.String());
            AddColumn("Archive.IncommingLetter", "RecivedDateAmharic", c => c.String());
            AddColumn("Archive.IncommingLetter", "OutgoingLetterDateAm", c => c.String());
            AddColumn("Archive.LetterStatus", "DateAmharic", c => c.String());
            AddColumn("HRM.Outgoing", "DateAmharic", c => c.String());
            AddColumn("HRM.RecruitmentPlan", "VacancyCode", c => c.String());
            AlterColumn("EXCEED.PlanItem", "ItemCategory", c => c.String(nullable: false));
            DropColumn("EXCEED.PlanItem", "FundSource");
            DropColumn("EXCEED.PlanItem", "PurchaseCategory");
            DropColumn("EXCEED.PlanItem", "BudgetType");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PlanItem", "BudgetType", c => c.Int(nullable: false));
            AddColumn("EXCEED.PlanItem", "PurchaseCategory", c => c.String());
            AddColumn("EXCEED.PlanItem", "FundSource", c => c.Int(nullable: false));
            DropForeignKey("HRM.ApplicantEducation", "ApplicantRegistrationId", "HRM.ApplicantRegistration");
            DropIndex("HRM.ApplicantEducation", new[] { "ApplicantRegistrationId" });
            AlterColumn("EXCEED.PlanItem", "ItemCategory", c => c.String());
            DropColumn("HRM.RecruitmentPlan", "VacancyCode");
            DropColumn("HRM.Outgoing", "DateAmharic");
            DropColumn("Archive.LetterStatus", "DateAmharic");
            DropColumn("Archive.IncommingLetter", "OutgoingLetterDateAm");
            DropColumn("Archive.IncommingLetter", "RecivedDateAmharic");
            DropColumn("Archive.IncommingLetter", "LetterAmharic");
            DropColumn("Archive.LetterSender", "DateAmharic");
            DropColumn("HRM.LeaveTransfer", "Check");
            DropColumn("EXCEED.PlanItem", "ProcurementCategory");
            DropColumn("EXCEED.PlanItem", "BudgetSource");
            DropColumn("EXCEED.PlanItem", "InventoryItemDesc");
            DropColumn("EXCEED.PlanItem", "ItemCategoryDesc");
            DropColumn("Insurance.Insurance", "ToAmharic");
            DropColumn("Insurance.Insurance", "FromAmharic");
            DropColumn("HRM.IncommingStatus", "DateAmharic");
            DropColumn("HRM.Incomming", "RecivedDateAmharic");
            DropColumn("HRM.Incomming", "LetterDateAmmharic");
            DropColumn("Insurance.CourtInsurance", "DateInsuAmhairc");
            DropColumn("Insurance.CourtInsurance", "DateAssocAmharic");
            DropColumn("HRM.Branch", "BranchTypes");
            DropColumn("HRM.Vacancy", "VacancyType");
            DropColumn("HRM.ApplicantExperience", "ToAmahric");
            DropColumn("HRM.ApplicantExperience", "FromAmharic");
            DropColumn("Insurance.AccidentRecord", "DateAmharic");
            DropTable("HRM.ApplicantEducation");
            CreateIndex("Archive.PersonalArchiveNo", "ZoneId");
            CreateIndex("Archive.InstitutionArchiveNo", "ZoneId");
            AddForeignKey("Archive.PersonalArchiveNo", "ZoneId", "Archive.Zone", "ZoneId", cascadeDelete: true);
            AddForeignKey("Archive.InstitutionArchiveNo", "ZoneId", "Archive.Zone", "ZoneId", cascadeDelete: true);
            MoveTable(name: "HRM.BranchCostCenter", newSchema: "EXCEED");
        }
    }
}
