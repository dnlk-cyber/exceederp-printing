namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HealthRankAndValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("Health.HealthRankAndValues", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("Health.HealthRankAndValues", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("Health.HealthRankAndValues", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("Health.HealthRankAndValues", "ForwardedFrom", c => c.String());
            AddColumn("Health.HealthRankAndValues", "ForwardedTo", c => c.String());
            AddColumn("Health.HealthRankAndValues", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("Health.HealthRankAndValues", "CurrentApprovalStep", c => c.Int());
            AlterColumn("Health.HealthBenefitSettings", "BenfitValue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("Health.HealthBenefitSettings", "BenfitValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Health.HealthRankAndValues", "CurrentApprovalStep");
            DropColumn("Health.HealthRankAndValues", "IsFinalApproval");
            DropColumn("Health.HealthRankAndValues", "ForwardedTo");
            DropColumn("Health.HealthRankAndValues", "ForwardedFrom");
            DropColumn("Health.HealthRankAndValues", "DepartmentSendForApprovalTime");
            DropColumn("Health.HealthRankAndValues", "DepartmentSendForApprovalBy");
            DropColumn("Health.HealthRankAndValues", "IsDepartmentSendForApproval");
            DropColumn("Health.HealthRankAndValues", "DepartmentOnlineApprovedTime");
            DropColumn("Health.HealthRankAndValues", "DepartmentOnlineApprovedBy");
            DropColumn("Health.HealthRankAndValues", "IsDepartmentOnlineApproved");
        }
    }
}
