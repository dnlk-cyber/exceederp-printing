namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tendertemmember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.LotTeamDecision",
                c => new
                    {
                        LotTeamDecisionId = c.Int(nullable: false, identity: true),
                        LotTeamId = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LotTeamDecisionId)
                .ForeignKey("EXCEED.LotTeam", t => t.LotTeamId, cascadeDelete: true)
                .Index(t => t.LotTeamId);
            
            CreateTable(
                "EXCEED.LotTeam",
                c => new
                    {
                        LotTeamId = c.Int(nullable: false, identity: true),
                        LotID = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LotTeamId)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: true)
                .Index(t => t.LotID);
            
            CreateTable(
                "EXCEED.LotTeamMember",
                c => new
                    {
                        LotTeamMemberId = c.Int(nullable: false, identity: true),
                        LotTeamId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        SignedBy = c.String(),
                        SginTime = c.DateTime(),
                        Signed = c.Boolean(nullable: false),
                        Title = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LotTeamMemberId)
                .ForeignKey("EXCEED.LotTeam", t => t.LotTeamId, cascadeDelete: true)
                .Index(t => t.LotTeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.LotTeamMember", "LotTeamId", "EXCEED.LotTeam");
            DropForeignKey("EXCEED.LotTeamDecision", "LotTeamId", "EXCEED.LotTeam");
            DropForeignKey("EXCEED.LotTeam", "LotID", "EXCEED.Lot");
            DropIndex("EXCEED.LotTeamMember", new[] { "LotTeamId" });
            DropIndex("EXCEED.LotTeam", new[] { "LotID" });
            DropIndex("EXCEED.LotTeamDecision", new[] { "LotTeamId" });
            DropTable("EXCEED.LotTeamMember");
            DropTable("EXCEED.LotTeam");
            DropTable("EXCEED.LotTeamDecision");
        }
    }
}
