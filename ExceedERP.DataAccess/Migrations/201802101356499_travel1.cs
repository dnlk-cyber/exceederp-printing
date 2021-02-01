namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class travel1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPettyCashDistribution", "AccountCode", c => c.String());
            AddColumn("EXCEED.ApPettyCashDistribution", "AccountDesc", c => c.String());
            AddColumn("EXCEED.ApPettyCashDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ApPettyCashDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPettyCashDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApTravelExpenseDistribution", "AccountCode", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseDistribution", "AccountDesc", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApTravelExpenseDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApTravelOrderDistribution", "AccountCode", c => c.String());
            AddColumn("EXCEED.ApTravelOrderDistribution", "AccountDesc", c => c.String());
            AddColumn("EXCEED.ApTravelOrderDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ApTravelOrderDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApTravelOrderDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApTravelOrderDistribution", "Credit");
            DropColumn("EXCEED.ApTravelOrderDistribution", "Debit");
            DropColumn("EXCEED.ApTravelOrderDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ApTravelOrderDistribution", "AccountDesc");
            DropColumn("EXCEED.ApTravelOrderDistribution", "AccountCode");
            DropColumn("EXCEED.ApTravelExpenseDistribution", "Credit");
            DropColumn("EXCEED.ApTravelExpenseDistribution", "Debit");
            DropColumn("EXCEED.ApTravelExpenseDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ApTravelExpenseDistribution", "AccountDesc");
            DropColumn("EXCEED.ApTravelExpenseDistribution", "AccountCode");
            DropColumn("EXCEED.ApPettyCashDistribution", "Credit");
            DropColumn("EXCEED.ApPettyCashDistribution", "Debit");
            DropColumn("EXCEED.ApPettyCashDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ApPettyCashDistribution", "AccountDesc");
            DropColumn("EXCEED.ApPettyCashDistribution", "AccountCode");
        }
    }
}
