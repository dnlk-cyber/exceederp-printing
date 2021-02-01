namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assettransfer4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.MachineryAndVehicleTransferValidation",
                c => new
                    {
                        MachineryAndVehicleTransferValidationId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MachineryAndVehicleTransferValidationId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransfer", t => t.MachineryAndVehicleTransferId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.MachineryAndVehicleTransferValidation", "MachineryAndVehicleTransferId", "EXCEED.MachineryAndVehicleTransfer");
            DropIndex("EXCEED.MachineryAndVehicleTransferValidation", new[] { "MachineryAndVehicleTransferId" });
            DropTable("EXCEED.MachineryAndVehicleTransferValidation");
        }
    }
}
