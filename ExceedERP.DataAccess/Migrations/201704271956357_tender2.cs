namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tender2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BidPParticipatingSuppliers", "BidSubmissionDate", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.BidPParticipatingSuppliers", "BidSubmissionTime", c => c.String());
            AddColumn("EXCEED.BidPParticipatingSuppliers", "NumberOfDocument", c => c.Int(nullable: false));
            DropColumn("EXCEED.BidPParticipatingSuppliers", "BidSubmitionDate");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "BidSubmitionTime");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.BidPParticipatingSuppliers", "BidSubmitionTime", c => c.String());
            AddColumn("EXCEED.BidPParticipatingSuppliers", "BidSubmitionDate", c => c.DateTime(nullable: false));
            DropColumn("EXCEED.BidPParticipatingSuppliers", "NumberOfDocument");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "BidSubmissionTime");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "BidSubmissionDate");
        }
    }
}
