namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintingPropertyDatatTypeUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.Proforma", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "IsChecked", c => c.Boolean(nullable: false));
            AlterColumn("PrintingEstimation.EstimationForms", "TextPaper", c => c.String(nullable: false));
            AlterColumn("PrintingEstimation.EstimationForms", "TextPrint", c => c.String(nullable: false));
            AlterColumn("ProductFollowUp.Job", "TextWeight", c => c.String());
            AlterColumn("ProductFollowUp.Job", "CoverWeight", c => c.String());
            AlterColumn("ProductFollowUp.Job", "ColorText", c => c.String());
            AlterColumn("ProductFollowUp.Job", "Colorcover", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("ProductFollowUp.Job", "Colorcover", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.Job", "ColorText", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.Job", "CoverWeight", c => c.Double(nullable: false));
            AlterColumn("ProductFollowUp.Job", "TextWeight", c => c.Double(nullable: false));
            AlterColumn("PrintingEstimation.EstimationForms", "TextPrint", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("PrintingEstimation.EstimationForms", "TextPaper", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("ProductFollowUp.Proforma", "IsChecked");
            DropColumn("ProductFollowUp.Proforma", "IsApproved");
        }
    }
}
