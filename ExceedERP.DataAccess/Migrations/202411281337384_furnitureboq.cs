namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnitureboq : DbMigration
    {
        public override void Up()
        {
            AddColumn("Manufacturing.DirectLaborCosts", "CostPerMinute", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureBoqRequest", "IsChecked", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureBoqRequest", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureBoqRequest", "IsRejected", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureBoqRequest", "CheckedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureBoqRequest", "ApprovedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureBoqRequest", "RejectedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureBoqRequest", "ApprovedDate", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureBoqRequest", "CheckedDate", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureBoqRequest", "RejectedDate", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureBoqRequest", "Status", c => c.String());
            DropColumn("Manufacturing.DirectLaborCosts", "CostPerHour");
            DropColumn("Manufacturing.DirectLaborCosts", "TaskCategoryId");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.DirectLaborCosts", "TaskCategoryId", c => c.Int(nullable: false));
            AddColumn("Manufacturing.DirectLaborCosts", "CostPerHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Manufacturing.FurnitureBoqRequest", "Status");
            DropColumn("Manufacturing.FurnitureBoqRequest", "RejectedDate");
            DropColumn("Manufacturing.FurnitureBoqRequest", "CheckedDate");
            DropColumn("Manufacturing.FurnitureBoqRequest", "ApprovedDate");
            DropColumn("Manufacturing.FurnitureBoqRequest", "RejectedBy");
            DropColumn("Manufacturing.FurnitureBoqRequest", "ApprovedBy");
            DropColumn("Manufacturing.FurnitureBoqRequest", "CheckedBy");
            DropColumn("Manufacturing.FurnitureBoqRequest", "IsRejected");
            DropColumn("Manufacturing.FurnitureBoqRequest", "IsApproved");
            DropColumn("Manufacturing.FurnitureBoqRequest", "IsChecked");
            DropColumn("Manufacturing.DirectLaborCosts", "CostPerMinute");
        }
    }
}
