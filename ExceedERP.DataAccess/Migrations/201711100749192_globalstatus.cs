namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class globalstatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlobalStatus",
                c => new
                    {
                        GlobalStatusId = c.Int(nullable: false, identity: true),
                        OperationType = c.String(),
                        Step = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.GlobalStatusId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.GlobalStatus");
        }
    }
}
