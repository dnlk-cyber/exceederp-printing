namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DciplinemeasuresInmoney : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.DisciplinaryCase", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplinaryCase", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.DisciplinaryCase", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplinaryCase", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.DisciplinaryCase", "ForwardedFrom", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "ForwardedTo", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplinaryCase", "CurrentApprovalStep", c => c.Int());
            AddColumn("HRM.DisciplinaryCase", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.DisciplinaryCase", "Status", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplinaryCase", "VoidBy", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "VoidTime", c => c.DateTime());
            AddColumn("HRM.DisciplinaryCase", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplinaryCase", "OnlineApprovedBy", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.DisciplinaryCase", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplinaryCase", "IsOnlineTransferredBy", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("HRM.DisciplinaryCase", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplinaryCase", "SendForApprovalBy", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "SendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasureByMoney", "Days", c => c.Int());
            AddColumn("HRM.DisciplineMeasureByMoney", "IsDeductedFromSalary", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasureByMoney", "SalaryDeductionPercentage", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.DisciplineMeasureByMoney", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasureByMoney", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasureByMoney", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasureByMoney", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasureByMoney", "ForwardedFrom", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "ForwardedTo", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasureByMoney", "CurrentApprovalStep", c => c.Int());
            AddColumn("HRM.DisciplineMeasureByMoney", "ForwardedTime", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasureByMoney", "Status", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasureByMoney", "VoidBy", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "VoidTime", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasureByMoney", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasureByMoney", "OnlineApprovedBy", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasureByMoney", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasureByMoney", "IsOnlineTransferredBy", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasureByMoney", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasureByMoney", "SendForApprovalBy", c => c.String());
            AddColumn("HRM.DisciplineMeasureByMoney", "SendForApprovalTime", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasuresDetail", "Rank", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.DisciplineMeasuresType", "IsDeductedFromSalary", c => c.Boolean(nullable: false));
            AddColumn("HRM.DisciplineMeasuresType", "SalaryDeductionPercentage", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.DisciplineMeasuresType", "Month", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.DisciplineMeasuresType", "Day", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.DisciplineMeasuresType", "Rank", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.DisciplineMeasuresType", "OrgBranchName", c => c.String());
            AddColumn("HRM.DisciplineMeasuresType", "DateCreated", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasuresType", "LastModified", c => c.DateTime());
            AddColumn("HRM.DisciplineMeasuresType", "CreatedBy", c => c.String());
            AddColumn("HRM.DisciplineMeasuresType", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.DisciplineMeasuresType", "ModifiedBy");
            DropColumn("HRM.DisciplineMeasuresType", "CreatedBy");
            DropColumn("HRM.DisciplineMeasuresType", "LastModified");
            DropColumn("HRM.DisciplineMeasuresType", "DateCreated");
            DropColumn("HRM.DisciplineMeasuresType", "OrgBranchName");
            DropColumn("HRM.DisciplineMeasuresType", "Rank");
            DropColumn("HRM.DisciplineMeasuresType", "Day");
            DropColumn("HRM.DisciplineMeasuresType", "Month");
            DropColumn("HRM.DisciplineMeasuresType", "SalaryDeductionPercentage");
            DropColumn("HRM.DisciplineMeasuresType", "IsDeductedFromSalary");
            DropColumn("HRM.DisciplineMeasuresDetail", "Rank");
            DropColumn("HRM.DisciplineMeasureByMoney", "SendForApprovalTime");
            DropColumn("HRM.DisciplineMeasureByMoney", "SendForApprovalBy");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsSendForApproval");
            DropColumn("HRM.DisciplineMeasureByMoney", "OnlineTransferredTime");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsOnlineTransferredBy");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsOnlineTransferred");
            DropColumn("HRM.DisciplineMeasureByMoney", "OnlineApprovedTime");
            DropColumn("HRM.DisciplineMeasureByMoney", "OnlineApprovedBy");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsOnlineApproved");
            DropColumn("HRM.DisciplineMeasureByMoney", "VoidTime");
            DropColumn("HRM.DisciplineMeasureByMoney", "VoidBy");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsVoid");
            DropColumn("HRM.DisciplineMeasureByMoney", "Status");
            DropColumn("HRM.DisciplineMeasureByMoney", "ForwardedTime");
            DropColumn("HRM.DisciplineMeasureByMoney", "CurrentApprovalStep");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsFinalApproval");
            DropColumn("HRM.DisciplineMeasureByMoney", "ForwardedTo");
            DropColumn("HRM.DisciplineMeasureByMoney", "ForwardedFrom");
            DropColumn("HRM.DisciplineMeasureByMoney", "DepartmentSendForApprovalTime");
            DropColumn("HRM.DisciplineMeasureByMoney", "DepartmentSendForApprovalBy");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsDepartmentSendForApproval");
            DropColumn("HRM.DisciplineMeasureByMoney", "DepartmentOnlineApprovedTime");
            DropColumn("HRM.DisciplineMeasureByMoney", "DepartmentOnlineApprovedBy");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsDepartmentOnlineApproved");
            DropColumn("HRM.DisciplineMeasureByMoney", "SalaryDeductionPercentage");
            DropColumn("HRM.DisciplineMeasureByMoney", "IsDeductedFromSalary");
            DropColumn("HRM.DisciplineMeasureByMoney", "Days");
            DropColumn("HRM.DisciplinaryCase", "SendForApprovalTime");
            DropColumn("HRM.DisciplinaryCase", "SendForApprovalBy");
            DropColumn("HRM.DisciplinaryCase", "IsSendForApproval");
            DropColumn("HRM.DisciplinaryCase", "OnlineTransferredTime");
            DropColumn("HRM.DisciplinaryCase", "IsOnlineTransferredBy");
            DropColumn("HRM.DisciplinaryCase", "IsOnlineTransferred");
            DropColumn("HRM.DisciplinaryCase", "OnlineApprovedTime");
            DropColumn("HRM.DisciplinaryCase", "OnlineApprovedBy");
            DropColumn("HRM.DisciplinaryCase", "IsOnlineApproved");
            DropColumn("HRM.DisciplinaryCase", "VoidTime");
            DropColumn("HRM.DisciplinaryCase", "VoidBy");
            DropColumn("HRM.DisciplinaryCase", "IsVoid");
            DropColumn("HRM.DisciplinaryCase", "Status");
            DropColumn("HRM.DisciplinaryCase", "ForwardedTime");
            DropColumn("HRM.DisciplinaryCase", "CurrentApprovalStep");
            DropColumn("HRM.DisciplinaryCase", "IsFinalApproval");
            DropColumn("HRM.DisciplinaryCase", "ForwardedTo");
            DropColumn("HRM.DisciplinaryCase", "ForwardedFrom");
            DropColumn("HRM.DisciplinaryCase", "DepartmentSendForApprovalTime");
            DropColumn("HRM.DisciplinaryCase", "DepartmentSendForApprovalBy");
            DropColumn("HRM.DisciplinaryCase", "IsDepartmentSendForApproval");
            DropColumn("HRM.DisciplinaryCase", "DepartmentOnlineApprovedTime");
            DropColumn("HRM.DisciplinaryCase", "DepartmentOnlineApprovedBy");
            DropColumn("HRM.DisciplinaryCase", "IsDepartmentOnlineApproved");
        }
    }
}
