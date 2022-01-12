namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProdFollowUpMod : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ProductFollowUp.Job", "CustomerName", c => c.String());
            AlterColumn("ProductFollowUp.Job", "MachineId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("ProductFollowUp.Job", "MachineId", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.Job", "CustomerName", c => c.String(maxLength: 50));
        }
    }
}
