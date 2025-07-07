namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsuranceUpdates1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.EmployeePeerToPeerAndInternalCustomerPerformanceSettings",
                c => new
                    {
                        EmployeePeerToPeerAndInternalCustomerPerformanceSettingId = c.Int(nullable: false, identity: true),
                        IsPeerToPeerHasMultipleEvaluator = c.Boolean(nullable: false),
                        IsInternalCustomerHasMultipleEvaluator = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeePeerToPeerAndInternalCustomerPerformanceSettingId);
            
            AddColumn("Performance.EmployeePerformance", "EvaluatorEmployeeId", c => c.Int());
            AddColumn("Performance.EmployeePerformance", "ImmediateSuppurvisorApprovalDate", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "ImmediateSuppurvisorApprover", c => c.String());
            AddColumn("Performance.EmployeePerformance", "ReasonForHRRejection", c => c.String());
            AddColumn("Performance.EmployeePerformance", "SecondLevelApprovalDate", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "SecondLevelApprover", c => c.String());
            AddColumn("Performance.EmployeePerformance", "IsHRRejected", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "FinalApprovalDate", c => c.DateTime());
            AddColumn("Performance.EmployeePerformance", "FinalApprover", c => c.String());
            AddColumn("Performance.EmployeePerformance", "IsPeerAndInternalCustomerHiddenToEmployee", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "EvalutionType", c => c.String());
            AddColumn("Performance.EmployeePerformance", "IsAveraged", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformanceSummary", "ResponsibilityLevel", c => c.Int(nullable: false));
            AddColumn("EXCEED.PerformanceSetting", "IsPeerAndInternalCustomerHide", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PerformanceSetting", "IsPeerAndInternalCustomerHide");
            DropColumn("Performance.EmployeePerformanceSummary", "ResponsibilityLevel");
            DropColumn("Performance.EmployeePerformance", "IsAveraged");
            DropColumn("Performance.EmployeePerformance", "EvalutionType");
            DropColumn("Performance.EmployeePerformance", "IsPeerAndInternalCustomerHiddenToEmployee");
            DropColumn("Performance.EmployeePerformance", "FinalApprover");
            DropColumn("Performance.EmployeePerformance", "FinalApprovalDate");
            DropColumn("Performance.EmployeePerformance", "IsHRRejected");
            DropColumn("Performance.EmployeePerformance", "SecondLevelApprover");
            DropColumn("Performance.EmployeePerformance", "SecondLevelApprovalDate");
            DropColumn("Performance.EmployeePerformance", "ReasonForHRRejection");
            DropColumn("Performance.EmployeePerformance", "ImmediateSuppurvisorApprover");
            DropColumn("Performance.EmployeePerformance", "ImmediateSuppurvisorApprovalDate");
            DropColumn("Performance.EmployeePerformance", "EvaluatorEmployeeId");
            DropTable("HRM.EmployeePeerToPeerAndInternalCustomerPerformanceSettings");
        }
    }
}
