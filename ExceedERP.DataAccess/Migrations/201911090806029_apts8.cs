namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts8 : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.LaboratoryDrugRequest", newSchema: "APTS");
            CreateTable(
                "APTS.LaboratoryDrugRequestItem",
                c => new
                    {
                        LaboratoryDrugRequestItemId = c.Int(nullable: false, identity: true),
                        LaboratoryDrugRequestId = c.Int(nullable: false),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        BegStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedQty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Loss = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QtyRequested = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QtyApproved = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QtyAllowed = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.LaboratoryDrugRequestItemId);
            
            AddColumn("APTS.DrugReceivalModel19", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "Remark", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "IsOnlineTransferredBy", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "OnlineTransferredTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("APTS.DrugReceivalModel19", "OnlineTransferredTime");
            DropColumn("APTS.DrugReceivalModel19", "IsOnlineTransferredBy");
            DropColumn("APTS.DrugReceivalModel19", "IsOnlineTransferred");
            DropColumn("APTS.DrugReceivalModel19", "Remark");
            DropColumn("APTS.DrugReceivalModel19", "IsPostedToSecondLedger");
            DropTable("APTS.LaboratoryDrugRequestItem");
            MoveTable(name: "APTS.LaboratoryDrugRequest", newSchema: "EXCEED");
        }
    }
}
