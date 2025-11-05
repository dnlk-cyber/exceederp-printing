namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printfurnituremig : DbMigration
    {
        public override void Up()
        {
            AddColumn("Manufacturing.FurnitureUnfinishedMaterialTransferForm", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Manufacturing.FurnitureUnfinishedMaterialTransferForm", "Remark");
        }
    }
}
