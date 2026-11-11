namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExceedAppMigration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PurchaseCommitteeMember",
                c => new
                    {
                        PurchaseCommitteeMemberId = c.Int(nullable: false, identity: true),
                        CommitteeType = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Title = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseCommitteeMemberId);
            
            AddColumn("EXCEED.LotTeam", "StandingCommitteeApplied", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaTeam", "StandingCommitteeApplied", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformaTeam", "StandingCommitteeApplied");
            DropColumn("EXCEED.LotTeam", "StandingCommitteeApplied");
            DropTable("EXCEED.PurchaseCommitteeMember");
        }
    }
}
