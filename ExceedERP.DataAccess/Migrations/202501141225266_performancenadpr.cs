namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class performancenadpr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Performance.EmployeePerformanceSummary",
                c => new
                    {
                        EmployeePerformanceSummaryId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GlPeriodId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Bonus = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeePerformanceSummaryId);
            
            AddColumn("EXCEED.PurchaseRequisition", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisition", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisition", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "CurrentApprovalStep", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "CurrentApprovalStep");
            DropColumn("EXCEED.PurchaseRequisition", "IsFinalApproval");
            DropColumn("EXCEED.PurchaseRequisition", "ForwardedTo");
            DropColumn("EXCEED.PurchaseRequisition", "ForwardedFrom");
            DropColumn("EXCEED.PurchaseRequisition", "DepartmentSendForApprovalTime");
            DropColumn("EXCEED.PurchaseRequisition", "DepartmentSendForApprovalBy");
            DropColumn("EXCEED.PurchaseRequisition", "IsDepartmentSendForApproval");
            DropColumn("EXCEED.PurchaseRequisition", "DepartmentOnlineApprovedTime");
            DropColumn("EXCEED.PurchaseRequisition", "DepartmentOnlineApprovedBy");
            DropColumn("EXCEED.PurchaseRequisition", "IsDepartmentOnlineApproved");
            DropTable("Performance.EmployeePerformanceSummary");
        }
    }
}
