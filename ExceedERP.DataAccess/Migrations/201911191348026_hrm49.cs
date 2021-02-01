namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm49 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.DeligationCC",
                c => new
                    {
                        DeligationCCId = c.Int(nullable: false, identity: true),
                        DelegationId = c.Int(nullable: false),
                        CC = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DeligationCCId);
            
            CreateTable(
                "HRM.DelegationReason",
                c => new
                    {
                        DelegationReasonId = c.Int(nullable: false, identity: true),
                        DelegationId = c.Int(nullable: false),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.DelegationReasonId);
            
            AddColumn("HRM.ComplainDetail", "Date", c => c.DateTime(nullable: false));
            AddColumn("HRM.ComplainDetail", "DateAm", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.ComplainDetail", "DateAm");
            DropColumn("HRM.ComplainDetail", "Date");
            DropTable("HRM.DelegationReason");
            DropTable("HRM.DeligationCC");
        }
    }
}
