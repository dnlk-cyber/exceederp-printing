namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction40 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ConstructionHoliday",
                c => new
                    {
                        ConstructionHolidayId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        HolidayId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateHoAmhaic = c.String(),
                        Name = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConstructionHolidayId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "Construction.ProjectPriceAdjustmentFormulaType",
                c => new
                    {
                        ProjectPriceAdjustmentFormulaTypeId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        FormulaType = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectPriceAdjustmentFormulaTypeId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            AddColumn("Construction.ProjectCalander", "WorkingHourType", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectCalander", "SettingCalendarId", c => c.Int(nullable: false));
            AddColumn("Construction.Task", "ProjectSpecificId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemPerProject", "IsVariationWorkItem", c => c.Boolean(nullable: false));
            AddColumn("Construction.WorkItemPerProject", "IsSupplementaryWorkItem", c => c.Boolean(nullable: false));
            AddColumn("Construction.WorkItemPerProject", "AddedQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemPerProject", "OmittedQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ConsReceivableInvoiceDetail", "MaterialOnSite", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericWorkItemPerProject", "IsVariationWorkItem", c => c.Boolean(nullable: false));
            AddColumn("Construction.GenericWorkItemPerProject", "IsSupplementaryWorkItem", c => c.Boolean(nullable: false));
            AddColumn("Construction.GenericWorkItemPerProject", "AddedQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.GenericWorkItemPerProject", "OmittedQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.PaymentCertificate", "MaterialOnSite", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.Calander", "WorkingHourType", c => c.Int(nullable: false));
            AlterColumn("Construction.ProjectEstimnWorkItem", "LabourHourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.LaborRateIndexFactorAttribute", "Index", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("Construction.LaborRateIndexFactorAttribute", "ApprovedIndex", c => c.Decimal(precision: 18, scale: 4));
            DropColumn("Construction.Task", "TaskCode");
        }
        
        public override void Down()
        {
            AddColumn("Construction.Task", "TaskCode", c => c.String());
            DropForeignKey("Construction.ProjectPriceAdjustmentFormulaType", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ConstructionHoliday", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ConstructionHoliday", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("Construction.ProjectPriceAdjustmentFormulaType", new[] { "ProjectInformationId" });
            DropIndex("Construction.ConstructionHoliday", new[] { "GlFiscalYearId" });
            DropIndex("Construction.ConstructionHoliday", new[] { "ProjectInformationId" });
            AlterColumn("Construction.LaborRateIndexFactorAttribute", "ApprovedIndex", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Construction.LaborRateIndexFactorAttribute", "Index", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstimnWorkItem", "LabourHourlyOutPut", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Construction.Calander", "WorkingHourType");
            DropColumn("Construction.PaymentCertificate", "MaterialOnSite");
            DropColumn("Construction.GenericWorkItemPerProject", "OmittedQuantityOfWork");
            DropColumn("Construction.GenericWorkItemPerProject", "AddedQuantityOfWork");
            DropColumn("Construction.GenericWorkItemPerProject", "IsSupplementaryWorkItem");
            DropColumn("Construction.GenericWorkItemPerProject", "IsVariationWorkItem");
            DropColumn("Construction.ConsReceivableInvoiceDetail", "MaterialOnSite");
            DropColumn("Construction.WorkItemPerProject", "OmittedQuantityOfWork");
            DropColumn("Construction.WorkItemPerProject", "AddedQuantityOfWork");
            DropColumn("Construction.WorkItemPerProject", "IsSupplementaryWorkItem");
            DropColumn("Construction.WorkItemPerProject", "IsVariationWorkItem");
            DropColumn("Construction.Task", "ProjectSpecificId");
            DropColumn("Construction.ProjectCalander", "SettingCalendarId");
            DropColumn("Construction.ProjectCalander", "WorkingHourType");
            DropTable("Construction.ProjectPriceAdjustmentFormulaType");
            DropTable("Construction.ConstructionHoliday");
        }
    }
}
