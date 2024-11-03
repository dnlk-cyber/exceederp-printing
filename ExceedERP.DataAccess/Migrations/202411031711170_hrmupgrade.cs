namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmupgrade : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.ClearanceProvider", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.ClearanceProvider", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("HRM.ClearanceProvider", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.ClearanceProvider", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.ClearanceProvider", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("HRM.ClearanceProvider", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.ClearanceProvider", "ForwardedFrom", c => c.String());
            AddColumn("HRM.ClearanceProvider", "ForwardedTo", c => c.String());
            AddColumn("HRM.ClearanceProvider", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.ClearanceProvider", "CurrentApprovalStep", c => c.Int());
            AddColumn("HRM.ClearanceProvider", "Status", c => c.String());
            AddColumn("HRM.ClearanceProvider", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("HRM.ClearanceProvider", "VoidBy", c => c.String());
            AddColumn("HRM.ClearanceProvider", "VoidTime", c => c.DateTime());
            AddColumn("HRM.ClearanceProvider", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.ClearanceProvider", "OnlineApprovedBy", c => c.String());
            AddColumn("HRM.ClearanceProvider", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.ClearanceProvider", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("HRM.ClearanceProvider", "IsOnlineTransferredBy", c => c.String());
            AddColumn("HRM.ClearanceProvider", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("HRM.ClearanceProvider", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.ClearanceProvider", "SendForApprovalBy", c => c.String());
            AddColumn("HRM.ClearanceProvider", "SendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.Cosign", "Salary", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.Delegation", "IsWithBenefit", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.InternalCustomer", "IsConfirmed", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.InternalCustomerPerformanceEvaluation", "IsConfirmed", c => c.Boolean(nullable: false));
            AddColumn("HRM.JobTitleMappingDetail", "EduField", c => c.Int(nullable: false));
            AddColumn("HRM.JobTitleMappingDetail", "Percentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePlan", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePlan", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("HRM.LeavePlan", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.LeavePlan", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePlan", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("HRM.LeavePlan", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.LeavePlan", "ForwardedFrom", c => c.String());
            AddColumn("HRM.LeavePlan", "ForwardedTo", c => c.String());
            AddColumn("HRM.LeavePlan", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePlan", "CurrentApprovalStep", c => c.Int());
            AddColumn("HRM.LeavePlan", "Status", c => c.String());
            AddColumn("HRM.LeavePlan", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePlan", "VoidBy", c => c.String());
            AddColumn("HRM.LeavePlan", "VoidTime", c => c.DateTime());
            AddColumn("HRM.LeavePlan", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePlan", "OnlineApprovedBy", c => c.String());
            AddColumn("HRM.LeavePlan", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.LeavePlan", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePlan", "IsOnlineTransferredBy", c => c.String());
            AddColumn("HRM.LeavePlan", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("HRM.LeavePlan", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePlan", "SendForApprovalBy", c => c.String());
            AddColumn("HRM.LeavePlan", "SendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.ManPowerPlanningRequest", "NumberOfStaff", c => c.Int(nullable: false));
            AddColumn("HRM.ManPowerPlanningRequest", "Education", c => c.String());
            AddColumn("HRM.ManPowerPlanningRequest", "Experience", c => c.String());
            AddColumn("HRM.ManPowerPlanningRequest", "Reason", c => c.String());
            AddColumn("EXCEED.PeerToPeerPerformanceEvaluation", "IsAsignmentApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.ProbationPerformance", "ResultType", c => c.Int(nullable: false));
            AddColumn("HRM.ProbationPerformance", "Result", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("HRM.Placement", "EmployeeId");
            AddForeignKey("HRM.Placement", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            DropColumn("HRM.ManPowerPlanningRequest", "NoOfVaccantPostion");
            DropColumn("HRM.RetirementSetting", "RetireSetting");
        }
        
        public override void Down()
        {
            AddColumn("HRM.RetirementSetting", "RetireSetting", c => c.Int(nullable: false));
            AddColumn("HRM.ManPowerPlanningRequest", "NoOfVaccantPostion", c => c.Int(nullable: false));
            DropForeignKey("HRM.Placement", "EmployeeId", "HRM.Employee");
            DropIndex("HRM.Placement", new[] { "EmployeeId" });
            DropColumn("HRM.ProbationPerformance", "Result");
            DropColumn("HRM.ProbationPerformance", "ResultType");
            DropColumn("EXCEED.PeerToPeerPerformanceEvaluation", "IsAsignmentApproved");
            DropColumn("HRM.ManPowerPlanningRequest", "Reason");
            DropColumn("HRM.ManPowerPlanningRequest", "Experience");
            DropColumn("HRM.ManPowerPlanningRequest", "Education");
            DropColumn("HRM.ManPowerPlanningRequest", "NumberOfStaff");
            DropColumn("HRM.LeavePlan", "SendForApprovalTime");
            DropColumn("HRM.LeavePlan", "SendForApprovalBy");
            DropColumn("HRM.LeavePlan", "IsSendForApproval");
            DropColumn("HRM.LeavePlan", "OnlineTransferredTime");
            DropColumn("HRM.LeavePlan", "IsOnlineTransferredBy");
            DropColumn("HRM.LeavePlan", "IsOnlineTransferred");
            DropColumn("HRM.LeavePlan", "OnlineApprovedTime");
            DropColumn("HRM.LeavePlan", "OnlineApprovedBy");
            DropColumn("HRM.LeavePlan", "IsOnlineApproved");
            DropColumn("HRM.LeavePlan", "VoidTime");
            DropColumn("HRM.LeavePlan", "VoidBy");
            DropColumn("HRM.LeavePlan", "IsVoid");
            DropColumn("HRM.LeavePlan", "Status");
            DropColumn("HRM.LeavePlan", "CurrentApprovalStep");
            DropColumn("HRM.LeavePlan", "IsFinalApproval");
            DropColumn("HRM.LeavePlan", "ForwardedTo");
            DropColumn("HRM.LeavePlan", "ForwardedFrom");
            DropColumn("HRM.LeavePlan", "DepartmentSendForApprovalTime");
            DropColumn("HRM.LeavePlan", "DepartmentSendForApprovalBy");
            DropColumn("HRM.LeavePlan", "IsDepartmentSendForApproval");
            DropColumn("HRM.LeavePlan", "DepartmentOnlineApprovedTime");
            DropColumn("HRM.LeavePlan", "DepartmentOnlineApprovedBy");
            DropColumn("HRM.LeavePlan", "IsDepartmentOnlineApproved");
            DropColumn("HRM.JobTitleMappingDetail", "Percentage");
            DropColumn("HRM.JobTitleMappingDetail", "EduField");
            DropColumn("EXCEED.InternalCustomerPerformanceEvaluation", "IsConfirmed");
            DropColumn("EXCEED.InternalCustomer", "IsConfirmed");
            DropColumn("HRM.Delegation", "IsWithBenefit");
            DropColumn("HRM.Cosign", "Salary");
            DropColumn("HRM.ClearanceProvider", "SendForApprovalTime");
            DropColumn("HRM.ClearanceProvider", "SendForApprovalBy");
            DropColumn("HRM.ClearanceProvider", "IsSendForApproval");
            DropColumn("HRM.ClearanceProvider", "OnlineTransferredTime");
            DropColumn("HRM.ClearanceProvider", "IsOnlineTransferredBy");
            DropColumn("HRM.ClearanceProvider", "IsOnlineTransferred");
            DropColumn("HRM.ClearanceProvider", "OnlineApprovedTime");
            DropColumn("HRM.ClearanceProvider", "OnlineApprovedBy");
            DropColumn("HRM.ClearanceProvider", "IsOnlineApproved");
            DropColumn("HRM.ClearanceProvider", "VoidTime");
            DropColumn("HRM.ClearanceProvider", "VoidBy");
            DropColumn("HRM.ClearanceProvider", "IsVoid");
            DropColumn("HRM.ClearanceProvider", "Status");
            DropColumn("HRM.ClearanceProvider", "CurrentApprovalStep");
            DropColumn("HRM.ClearanceProvider", "IsFinalApproval");
            DropColumn("HRM.ClearanceProvider", "ForwardedTo");
            DropColumn("HRM.ClearanceProvider", "ForwardedFrom");
            DropColumn("HRM.ClearanceProvider", "DepartmentSendForApprovalTime");
            DropColumn("HRM.ClearanceProvider", "DepartmentSendForApprovalBy");
            DropColumn("HRM.ClearanceProvider", "IsDepartmentSendForApproval");
            DropColumn("HRM.ClearanceProvider", "DepartmentOnlineApprovedTime");
            DropColumn("HRM.ClearanceProvider", "DepartmentOnlineApprovedBy");
            DropColumn("HRM.ClearanceProvider", "IsDepartmentOnlineApproved");
        }
    }
}
