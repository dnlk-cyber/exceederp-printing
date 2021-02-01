namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class goodfriday : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FleetInsuranceRegistration", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropForeignKey("EXCEED.InsuranceAttachment", "FleetInsuranceRegistrationID", "EXCEED.FleetInsuranceRegistration");
            DropForeignKey("HRM.Prohibition", "LookupId", "common.Lookup");
            DropIndex("EXCEED.FleetInsuranceRegistration", new[] { "AssetNumber" });
            DropIndex("EXCEED.InsuranceAttachment", new[] { "FleetInsuranceRegistrationID" });
            DropIndex("HRM.Prohibition", new[] { "LookupId" });
            CreateTable(
                "HRM.AnnouncmentDetail",
                c => new
                    {
                        AnnouncDetailId = c.Int(nullable: false, identity: true),
                        Detail = c.String(),
                        AnnouncmentId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnouncDetailId)
                .ForeignKey("HRM.Announcment", t => t.AnnouncmentId, cascadeDelete: true)
                .Index(t => t.AnnouncmentId);
            
            CreateTable(
                "HRM.Announcment",
                c => new
                    {
                        AnnouncmentId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Date = c.DateTime(nullable: false),
                        DateAnnAmharic = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnouncmentId);
            
            CreateTable(
                "Insurance.InsuranceCategory",
                c => new
                    {
                        InsuranceCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Remark = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InsuranceCategoryId);
            
            CreateTable(
                "Insurance.InsuranceType",
                c => new
                    {
                        InsuranceTypeId = c.Int(nullable: false, identity: true),
                        InsuranceCategoryId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InsuranceTypeId)
                .ForeignKey("Insurance.InsuranceCategory", t => t.InsuranceCategoryId, cascadeDelete: true)
                .Index(t => t.InsuranceCategoryId);
            
            CreateTable(
                "Insurance.OtherInsurance",
                c => new
                    {
                        OtherInsuranceId = c.Int(nullable: false, identity: true),
                        InsuranceCategoryId = c.Int(nullable: false),
                        InsuranceTypeId = c.Int(),
                        From = c.DateTime(nullable: false),
                        FromInsuranceAmhric = c.String(),
                        To = c.DateTime(nullable: false),
                        ToInsuranceAmharic = c.String(),
                        SumofInsured = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Premium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Stamp = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ThirdParty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OtherInsuranceId)
                .ForeignKey("Insurance.InsuranceCategory", t => t.InsuranceCategoryId, cascadeDelete: true)
                .Index(t => t.InsuranceCategoryId);
            
            AddColumn("HRM.Vacancy", "FromVacAm", c => c.String());
            AddColumn("HRM.Vacancy", "ToVacAm", c => c.String());
            AddColumn("HRM.Commitment", "FromCometAm", c => c.String(nullable: false));
            AddColumn("HRM.Commitment", "ToCometAm", c => c.String(nullable: false));
            AddColumn("HRM.CommitteMinute", "DateAmharic", c => c.String());
            AddColumn("HRM.Competency", "FromComAm", c => c.String());
            AddColumn("HRM.Competency", "ToCompAm", c => c.String());
            AddColumn("HRM.Contract", "FromContAm", c => c.String());
            AddColumn("HRM.Contract", "ToContAm", c => c.String());
            AddColumn("HRM.Education", "FromAmharic", c => c.String());
            AddColumn("HRM.Education", "ToAmharic", c => c.String());
            AddColumn("HRM.Training", "FromTrAm", c => c.String());
            AddColumn("HRM.Training", "ToTraAm", c => c.String());
            AddColumn("HRM.Experience", "FromExAmharic", c => c.String());
            AddColumn("HRM.Experience", "ToExAmharic", c => c.String());
            AddColumn("HRM.Experience", "ClearanceDateTimeAm", c => c.String());
            AddColumn("HRM.ExperienceRequest", "ApplicationDateAm", c => c.String(nullable: false));
            AddColumn("common.Identification", "IssueDateIdAm", c => c.String());
            AddColumn("common.Identification", "ExpiryDateAm", c => c.String());
            AddColumn("Health.ReferralSheet", "DateAmharic", c => c.String());
            AddColumn("Health.ReferralSheet", "MainCompanyId", c => c.Int(nullable: false));
            CreateIndex("Health.ReferralSheet", "MainCompanyId");
            AddForeignKey("Health.ReferralSheet", "MainCompanyId", "HRM.MainCompany", "MainCompanyId", cascadeDelete: true);
            DropColumn("Health.ReferralSheet", "ReferredTo");
            DropTable("EXCEED.FleetInsuranceRegistration");
            DropTable("EXCEED.InsuranceAttachment");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.InsuranceAttachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Extension = c.String(),
                        FleetInsuranceRegistrationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "EXCEED.FleetInsuranceRegistration",
                c => new
                    {
                        FleetInsuranceRegistrationID = c.Int(nullable: false, identity: true),
                        AssetNumber = c.String(maxLength: 128),
                        PlateNumber = c.String(),
                        Premium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DueDate = c.DateTime(nullable: false),
                        ExpireDate = c.DateTime(),
                        Company = c.String(nullable: false),
                        InsuranceType = c.Int(nullable: false),
                        MobileNumber = c.String(),
                        City = c.String(),
                        EmailAddress = c.String(),
                        Website = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetInsuranceRegistrationID);
            
            AddColumn("Health.ReferralSheet", "ReferredTo", c => c.String());
            DropForeignKey("Health.ReferralSheet", "MainCompanyId", "HRM.MainCompany");
            DropForeignKey("Insurance.OtherInsurance", "InsuranceCategoryId", "Insurance.InsuranceCategory");
            DropForeignKey("Insurance.InsuranceType", "InsuranceCategoryId", "Insurance.InsuranceCategory");
            DropForeignKey("HRM.AnnouncmentDetail", "AnnouncmentId", "HRM.Announcment");
            DropIndex("Health.ReferralSheet", new[] { "MainCompanyId" });
            DropIndex("Insurance.OtherInsurance", new[] { "InsuranceCategoryId" });
            DropIndex("Insurance.InsuranceType", new[] { "InsuranceCategoryId" });
            DropIndex("HRM.AnnouncmentDetail", new[] { "AnnouncmentId" });
            DropColumn("Health.ReferralSheet", "MainCompanyId");
            DropColumn("Health.ReferralSheet", "DateAmharic");
            DropColumn("common.Identification", "ExpiryDateAm");
            DropColumn("common.Identification", "IssueDateIdAm");
            DropColumn("HRM.ExperienceRequest", "ApplicationDateAm");
            DropColumn("HRM.Experience", "ClearanceDateTimeAm");
            DropColumn("HRM.Experience", "ToExAmharic");
            DropColumn("HRM.Experience", "FromExAmharic");
            DropColumn("HRM.Training", "ToTraAm");
            DropColumn("HRM.Training", "FromTrAm");
            DropColumn("HRM.Education", "ToAmharic");
            DropColumn("HRM.Education", "FromAmharic");
            DropColumn("HRM.Contract", "ToContAm");
            DropColumn("HRM.Contract", "FromContAm");
            DropColumn("HRM.Competency", "ToCompAm");
            DropColumn("HRM.Competency", "FromComAm");
            DropColumn("HRM.CommitteMinute", "DateAmharic");
            DropColumn("HRM.Commitment", "ToCometAm");
            DropColumn("HRM.Commitment", "FromCometAm");
            DropColumn("HRM.Vacancy", "ToVacAm");
            DropColumn("HRM.Vacancy", "FromVacAm");
            DropTable("Insurance.OtherInsurance");
            DropTable("Insurance.InsuranceType");
            DropTable("Insurance.InsuranceCategory");
            DropTable("HRM.Announcment");
            DropTable("HRM.AnnouncmentDetail");
            CreateIndex("HRM.Prohibition", "LookupId");
            CreateIndex("EXCEED.InsuranceAttachment", "FleetInsuranceRegistrationID");
            CreateIndex("EXCEED.FleetInsuranceRegistration", "AssetNumber");
            AddForeignKey("HRM.Prohibition", "LookupId", "common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("EXCEED.InsuranceAttachment", "FleetInsuranceRegistrationID", "EXCEED.FleetInsuranceRegistration", "FleetInsuranceRegistrationID", cascadeDelete: true);
            AddForeignKey("EXCEED.FleetInsuranceRegistration", "AssetNumber", "EXCEED.FleetEquipmentRegistration", "AssetNumber");
        }
    }
}
