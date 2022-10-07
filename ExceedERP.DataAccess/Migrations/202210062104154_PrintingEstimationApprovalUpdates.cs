namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintingEstimationApprovalUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("PrintingEstimation.EstimationForms", "CheckedBy", c => c.String());
            AddColumn("PrintingEstimation.EstimationForms", "ApprovedBy", c => c.String());
            AddColumn("ProductFollowUp.Job", "CheckedBy", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "CheckedBy", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "ApprovedBy", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "DateCreated", c => c.DateTime());
            AddColumn("ProductFollowUp.Proforma", "LastModified", c => c.DateTime());
            AddColumn("ProductFollowUp.Proforma", "CreatedBy", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.Proforma", "ModifiedBy");
            DropColumn("ProductFollowUp.Proforma", "CreatedBy");
            DropColumn("ProductFollowUp.Proforma", "LastModified");
            DropColumn("ProductFollowUp.Proforma", "DateCreated");
            DropColumn("ProductFollowUp.Proforma", "ApprovedBy");
            DropColumn("ProductFollowUp.Proforma", "CheckedBy");
            DropColumn("ProductFollowUp.Job", "CheckedBy");
            DropColumn("PrintingEstimation.EstimationForms", "ApprovedBy");
            DropColumn("PrintingEstimation.EstimationForms", "CheckedBy");
        }
    }
}
