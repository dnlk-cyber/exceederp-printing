namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.GoodReceive", "ReciveTypeId", "EXCEED.ReciveType");
            DropIndex("EXCEED.GoodReceive", new[] { "ReciveTypeId" });
            CreateTable(
                "EXCEED.ReceiveType",
                c => new
                    {
                        ReceiveTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        Desctiption = c.String(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReceiveTypeId);
            
            AddColumn("EXCEED.InventoryStore", "Remark", c => c.String());
            AddColumn("EXCEED.GoodReceive", "ReceiveTypeId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.GoodReceive", "ReceiveTypeId");
            AddForeignKey("EXCEED.GoodReceive", "ReceiveTypeId", "EXCEED.ReceiveType", "ReceiveTypeId", cascadeDelete: true);
            DropColumn("EXCEED.GoodReceive", "ReciveTypeId");
            DropTable("EXCEED.ReciveType");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.ReciveType",
                c => new
                    {
                        ReciveTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        Desctiption = c.String(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReciveTypeId);
            
            AddColumn("EXCEED.GoodReceive", "ReciveTypeId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.GoodReceive", "ReceiveTypeId", "EXCEED.ReceiveType");
            DropIndex("EXCEED.GoodReceive", new[] { "ReceiveTypeId" });
            DropColumn("EXCEED.GoodReceive", "ReceiveTypeId");
            DropColumn("EXCEED.InventoryStore", "Remark");
            DropTable("EXCEED.ReceiveType");
            CreateIndex("EXCEED.GoodReceive", "ReciveTypeId");
            AddForeignKey("EXCEED.GoodReceive", "ReciveTypeId", "EXCEED.ReciveType", "ReciveTypeId", cascadeDelete: true);
        }
    }
}
