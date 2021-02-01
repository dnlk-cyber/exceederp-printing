namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GuaranteeMode",
                c => new
                    {
                        GuaranteeModeId = c.Int(nullable: false, identity: true),
                        Mode = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GuaranteeModeId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.GuaranteeMode");
        }
    }
}
