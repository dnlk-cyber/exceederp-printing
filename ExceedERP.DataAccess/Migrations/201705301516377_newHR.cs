namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newHR : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Payroll.FixedContribution", "LookupId", "common.Lookup");
            DropForeignKey("Payroll.RegularContribution", "LookupId", "common.Lookup");
            DropIndex("Payroll.FixedContribution", new[] { "LookupId" });
            DropIndex("Payroll.RegularContribution", new[] { "LookupId" });
            CreateTable(
                "HRM.AllowanceMapping",
                c => new
                    {
                        AllowanceMappingId = c.Int(nullable: false, identity: true),
                        AllowanceDefinationId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AllowanceMappingId)
                .ForeignKey("HRM.AllowanceDefination", t => t.AllowanceDefinationId, cascadeDelete: true)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.AllowanceDefinationId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.ContributionType",
                c => new
                    {
                        ContributionTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ContributionCategory = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContributionTypeId);
            
            AddColumn("Payroll.FixedContribution", "ContributionTypeId", c => c.Int(nullable: false));
            AddColumn("Payroll.RegularContribution", "ContributionTypeId", c => c.Int(nullable: false));
            AlterColumn("common.Identification", "Description", c => c.String(nullable: false));
            AlterColumn("common.Identification", "IdNumber", c => c.String(nullable: false));
            AlterColumn("common.Identification", "Issuer", c => c.String(nullable: false));
            CreateIndex("Payroll.FixedContribution", "ContributionTypeId");
            CreateIndex("Payroll.RegularContribution", "ContributionTypeId");
            AddForeignKey("Payroll.FixedContribution", "ContributionTypeId", "HRM.ContributionType", "ContributionTypeId", cascadeDelete: true);
            AddForeignKey("Payroll.RegularContribution", "ContributionTypeId", "HRM.ContributionType", "ContributionTypeId", cascadeDelete: true);
            DropColumn("HRM.AllowanceDefination", "LookupId");
            DropColumn("Payroll.FixedContribution", "LookupId");
            DropColumn("Payroll.RegularContribution", "LookupId");
        }
        
        public override void Down()
        {
            AddColumn("Payroll.RegularContribution", "LookupId", c => c.Int(nullable: false));
            AddColumn("Payroll.FixedContribution", "LookupId", c => c.Int(nullable: false));
            AddColumn("HRM.AllowanceDefination", "LookupId", c => c.Int());
            DropForeignKey("Payroll.RegularContribution", "ContributionTypeId", "HRM.ContributionType");
            DropForeignKey("Payroll.FixedContribution", "ContributionTypeId", "HRM.ContributionType");
            DropForeignKey("HRM.AllowanceMapping", "LookupId", "common.Lookup");
            DropForeignKey("HRM.AllowanceMapping", "AllowanceDefinationId", "HRM.AllowanceDefination");
            DropIndex("Payroll.RegularContribution", new[] { "ContributionTypeId" });
            DropIndex("Payroll.FixedContribution", new[] { "ContributionTypeId" });
            DropIndex("HRM.AllowanceMapping", new[] { "LookupId" });
            DropIndex("HRM.AllowanceMapping", new[] { "AllowanceDefinationId" });
            AlterColumn("common.Identification", "Issuer", c => c.String());
            AlterColumn("common.Identification", "IdNumber", c => c.String());
            AlterColumn("common.Identification", "Description", c => c.String());
            DropColumn("Payroll.RegularContribution", "ContributionTypeId");
            DropColumn("Payroll.FixedContribution", "ContributionTypeId");
            DropTable("HRM.ContributionType");
            DropTable("HRM.AllowanceMapping");
            CreateIndex("Payroll.RegularContribution", "LookupId");
            CreateIndex("Payroll.FixedContribution", "LookupId");
            AddForeignKey("Payroll.RegularContribution", "LookupId", "common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Payroll.FixedContribution", "LookupId", "common.Lookup", "LookupId", cascadeDelete: true);
        }
    }
}
