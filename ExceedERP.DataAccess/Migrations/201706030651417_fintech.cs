namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fintech : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItem", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.InventoryItem", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.InventoryItem", "CreatedBy", c => c.String());
            AddColumn("EXCEED.InventoryItem", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.ItemCategory", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ItemCategory", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ItemCategory", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ItemCategory", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.StoreItemAssignment", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.StoreItemAssignment", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.StoreItemAssignment", "CreatedBy", c => c.String());
            AddColumn("EXCEED.StoreItemAssignment", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.StoreKeeperAssignment", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.StoreKeeperAssignment", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.StoreKeeperAssignment", "CreatedBy", c => c.String());
            AddColumn("EXCEED.StoreKeeperAssignment", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.UoM", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.UoM", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.UoM", "CreatedBy", c => c.String());
            AddColumn("EXCEED.UoM", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.PaymentMethod", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentMethod", "ApplyToAr", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentMethod", "ArAccountCode", c => c.String());
            AddColumn("EXCEED.PaymentMethod", "ApplyToAp", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentMethod", "ApAccountCode", c => c.String());
            AddColumn("EXCEED.PaymentMethod", "Enable", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.RevaluationItem", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.RevaluationItem", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.RevaluationItem", "CreatedBy", c => c.String());
            AddColumn("EXCEED.RevaluationItem", "ModifiedBy", c => c.String());
            DropColumn("EXCEED.PaymentMethod", "Branch");
            DropColumn("EXCEED.PaymentMethod", "AccountCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PaymentMethod", "AccountCode", c => c.String(nullable: false));
            AddColumn("EXCEED.PaymentMethod", "Branch", c => c.String());
            DropColumn("EXCEED.RevaluationItem", "ModifiedBy");
            DropColumn("EXCEED.RevaluationItem", "CreatedBy");
            DropColumn("EXCEED.RevaluationItem", "LastModified");
            DropColumn("EXCEED.RevaluationItem", "DateCreated");
            DropColumn("EXCEED.PaymentMethod", "Enable");
            DropColumn("EXCEED.PaymentMethod", "ApAccountCode");
            DropColumn("EXCEED.PaymentMethod", "ApplyToAp");
            DropColumn("EXCEED.PaymentMethod", "ArAccountCode");
            DropColumn("EXCEED.PaymentMethod", "ApplyToAr");
            DropColumn("EXCEED.PaymentMethod", "BranchId");
            DropColumn("EXCEED.UoM", "ModifiedBy");
            DropColumn("EXCEED.UoM", "CreatedBy");
            DropColumn("EXCEED.UoM", "LastModified");
            DropColumn("EXCEED.UoM", "DateCreated");
            DropColumn("EXCEED.StoreKeeperAssignment", "ModifiedBy");
            DropColumn("EXCEED.StoreKeeperAssignment", "CreatedBy");
            DropColumn("EXCEED.StoreKeeperAssignment", "LastModified");
            DropColumn("EXCEED.StoreKeeperAssignment", "DateCreated");
            DropColumn("EXCEED.StoreItemAssignment", "ModifiedBy");
            DropColumn("EXCEED.StoreItemAssignment", "CreatedBy");
            DropColumn("EXCEED.StoreItemAssignment", "LastModified");
            DropColumn("EXCEED.StoreItemAssignment", "DateCreated");
            DropColumn("EXCEED.ItemCategory", "ModifiedBy");
            DropColumn("EXCEED.ItemCategory", "CreatedBy");
            DropColumn("EXCEED.ItemCategory", "LastModified");
            DropColumn("EXCEED.ItemCategory", "DateCreated");
            DropColumn("EXCEED.InventoryItem", "ModifiedBy");
            DropColumn("EXCEED.InventoryItem", "CreatedBy");
            DropColumn("EXCEED.InventoryItem", "LastModified");
            DropColumn("EXCEED.InventoryItem", "DateCreated");
        }
    }
}
