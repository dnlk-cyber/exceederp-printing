namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventoryupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Issue", "IsLoan", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Issue", "External", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Issue", "ExternalRequesterName", c => c.String());
            AddColumn("EXCEED.IssueItem", "AgreementPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "SupplierId", c => c.String());
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "IsLoan", c => c.Boolean(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionItemArchive", "CurrentBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionItemArchive", "CurrentAPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionItemArchive", "AgreementPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisition", "SupplierId", c => c.String());
            AddColumn("EXCEED.StoreRequisition", "IsLoan", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisitionItem", "CurrentBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisitionItem", "CurrentAPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisitionItem", "AgreementPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisitionItem", "AgreementPrice");
            DropColumn("EXCEED.StoreRequisitionItem", "CurrentAPrice");
            DropColumn("EXCEED.StoreRequisitionItem", "CurrentBalance");
            DropColumn("EXCEED.StoreRequisition", "IsLoan");
            DropColumn("EXCEED.StoreRequisition", "SupplierId");
            DropColumn("Inventory_Archive.StoreRequisitionItemArchive", "AgreementPrice");
            DropColumn("Inventory_Archive.StoreRequisitionItemArchive", "CurrentAPrice");
            DropColumn("Inventory_Archive.StoreRequisitionItemArchive", "CurrentBalance");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "IsLoan");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "SupplierId");
            DropColumn("EXCEED.IssueItem", "AgreementPrice");
            DropColumn("EXCEED.Issue", "ExternalRequesterName");
            DropColumn("EXCEED.Issue", "External");
            DropColumn("EXCEED.Issue", "IsLoan");
        }
    }
}
