namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supplierfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.SupplierListMinuteFile",
                c => new
                    {
                        SupplierListMinuteFileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Description = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.SupplierListMinuteFileId);
            
            AlterColumn("EXCEED.ProcurementSupplier", "SupplierId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ProcurementSupplier", "SupplierId", c => c.String());
            DropTable("EXCEED.SupplierListMinuteFile");
        }
    }
}
