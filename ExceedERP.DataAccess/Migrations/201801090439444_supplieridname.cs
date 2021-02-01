namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supplieridname : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ParticipatingSupplier", "SupplierId", c => c.String());
            AddColumn("EXCEED.ParticipatingSupplier", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ParticipatingSupplier", "SupplierName", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationResult", "SupplierId", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationResult", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "SupplierName", c => c.String());
            AddColumn("EXCEED.RequestedSupplier", "SupplierId", c => c.String());
            AddColumn("EXCEED.RequestedSupplier", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.RequestedSupplier", "SupplierName", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "SupplierId", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "SupplierName", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformPassFail", "SupplierName", c => c.String());
            AddColumn("EXCEED.ProformTechnical", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformTechnical", "SupplierName", c => c.String());
            AddColumn("EXCEED.PurchaseOrder", "IsNewSupplier", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.ProformPassFail", "SupplierId", c => c.String());
            AlterColumn("EXCEED.ProformTechnical", "SupplierId", c => c.String());
            DropColumn("EXCEED.ParticipatingSupplier", "Supplier");
            DropColumn("EXCEED.ProformaEvaluationResult", "Supplier");
            DropColumn("EXCEED.RequestedSupplier", "Supplier");
            DropColumn("EXCEED.WinningSupplier", "Supplier");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.WinningSupplier", "Supplier", c => c.String(nullable: false));
            AddColumn("EXCEED.RequestedSupplier", "Supplier", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformaEvaluationResult", "Supplier", c => c.String());
            AddColumn("EXCEED.ParticipatingSupplier", "Supplier", c => c.String(nullable: false));
            AlterColumn("EXCEED.ProformTechnical", "SupplierId", c => c.String(nullable: false));
            AlterColumn("EXCEED.ProformPassFail", "SupplierId", c => c.String(nullable: false));
            DropColumn("EXCEED.PurchaseOrder", "IsNewSupplier");
            DropColumn("EXCEED.ProformTechnical", "SupplierName");
            DropColumn("EXCEED.ProformTechnical", "IsNewSupplier");
            DropColumn("EXCEED.ProformPassFail", "SupplierName");
            DropColumn("EXCEED.ProformPassFail", "IsNewSupplier");
            DropColumn("EXCEED.WinningSupplier", "SupplierName");
            DropColumn("EXCEED.WinningSupplier", "IsNewSupplier");
            DropColumn("EXCEED.WinningSupplier", "SupplierId");
            DropColumn("EXCEED.RequestedSupplier", "SupplierName");
            DropColumn("EXCEED.RequestedSupplier", "IsNewSupplier");
            DropColumn("EXCEED.RequestedSupplier", "SupplierId");
            DropColumn("EXCEED.ProformaEvaluationResult", "SupplierName");
            DropColumn("EXCEED.ProformaEvaluationResult", "IsNewSupplier");
            DropColumn("EXCEED.ProformaEvaluationResult", "SupplierId");
            DropColumn("EXCEED.ParticipatingSupplier", "SupplierName");
            DropColumn("EXCEED.ParticipatingSupplier", "IsNewSupplier");
            DropColumn("EXCEED.ParticipatingSupplier", "SupplierId");
        }
    }
}
