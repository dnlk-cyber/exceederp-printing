namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Partner", "PartnerCode", c => c.String());
            AddColumn("EXCEED.ReconciliationBankAccount", "ReconciledDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReconciliationBankAccount", "ReconciledDate");
            DropColumn("EXCEED.Partner", "PartnerCode");
        }
    }
}
