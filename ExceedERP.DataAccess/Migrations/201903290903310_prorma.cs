namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prorma : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ParticipatingSupplierItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ParticipatingSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
