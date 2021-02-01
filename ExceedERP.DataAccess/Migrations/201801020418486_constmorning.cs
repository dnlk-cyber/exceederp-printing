namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class constmorning : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.MachinaryRent", "RentOtherSuppliersID", "FleetManagement.RentOtherSuppliers");
            DropIndex("Construction.MachinaryRent", new[] { "RentOtherSuppliersID" });
            AddColumn("Construction.MachinaryRent", "FleetsID", c => c.Guid(nullable: false));
            AlterColumn("Construction.InternalMachinaryRent", "Capacity", c => c.String());
            CreateIndex("Construction.MachinaryRent", "FleetsID");
            AddForeignKey("Construction.MachinaryRent", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
            DropColumn("Construction.MachinaryRent", "RentOtherSuppliersID");
        }
        
        public override void Down()
        {
            AddColumn("Construction.MachinaryRent", "RentOtherSuppliersID", c => c.Guid(nullable: false));
            DropForeignKey("Construction.MachinaryRent", "FleetsID", "FleetManagement.Fleets");
            DropIndex("Construction.MachinaryRent", new[] { "FleetsID" });
            AlterColumn("Construction.InternalMachinaryRent", "Capacity", c => c.Int());
            DropColumn("Construction.MachinaryRent", "FleetsID");
            CreateIndex("Construction.MachinaryRent", "RentOtherSuppliersID");
            AddForeignKey("Construction.MachinaryRent", "RentOtherSuppliersID", "FleetManagement.RentOtherSuppliers", "RentOtherSuppliersID", cascadeDelete: true);
        }
    }
}
