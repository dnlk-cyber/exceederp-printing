namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bincardsecond : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.BinCardLineSecondary",
                c => new
                    {
                        BinCardLineSecondaryId = c.Int(nullable: false, identity: true),
                        StoreItemAssignmentID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ReferenceDoc = c.String(),
                        ReceivedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssuedTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceUPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceTPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Reference = c.String(),
                        TimeStamp = c.DateTime(),
                        IsVoid = c.Boolean(nullable: false),
                        SupplierName = c.String(),
                        IssuedTo = c.String(),
                    })
                .PrimaryKey(t => t.BinCardLineSecondaryId)
                .ForeignKey("EXCEED.StoreItemAssignment", t => t.StoreItemAssignmentID, cascadeDelete: true)
                .Index(t => t.StoreItemAssignmentID);
            
            AddColumn("EXCEED.StoreItemAssignment", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.BinCardLineSecondary", "StoreItemAssignmentID", "EXCEED.StoreItemAssignment");
            DropIndex("EXCEED.BinCardLineSecondary", new[] { "StoreItemAssignmentID" });
            DropColumn("EXCEED.StoreItemAssignment", "Remark");
            DropTable("EXCEED.BinCardLineSecondary");
        }
    }
}
