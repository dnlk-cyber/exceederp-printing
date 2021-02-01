namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bankrecon : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ReconciliationBankServiceCharge", "ReconciliationSchedule_ReconciliationScheduleId", "EXCEED.ReconciliationSchedule");
            DropIndex("EXCEED.ReconciliationBankServiceCharge", new[] { "ReconciliationSchedule_ReconciliationScheduleId" });
            RenameColumn(table: "EXCEED.ReconciliationBankServiceCharge", name: "ReconciliationSchedule_ReconciliationScheduleId", newName: "ReconciliationScheduleId");
            AlterColumn("EXCEED.ReconciliationBankServiceCharge", "ReconciliationScheduleId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.ReconciliationBankServiceCharge", "ReconciliationScheduleId");
            AddForeignKey("EXCEED.ReconciliationBankServiceCharge", "ReconciliationScheduleId", "EXCEED.ReconciliationSchedule", "ReconciliationScheduleId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ReconciliationBankServiceCharge", "ReconciliationScheduleId", "EXCEED.ReconciliationSchedule");
            DropIndex("EXCEED.ReconciliationBankServiceCharge", new[] { "ReconciliationScheduleId" });
            AlterColumn("EXCEED.ReconciliationBankServiceCharge", "ReconciliationScheduleId", c => c.Int());
            RenameColumn(table: "EXCEED.ReconciliationBankServiceCharge", name: "ReconciliationScheduleId", newName: "ReconciliationSchedule_ReconciliationScheduleId");
            CreateIndex("EXCEED.ReconciliationBankServiceCharge", "ReconciliationSchedule_ReconciliationScheduleId");
            AddForeignKey("EXCEED.ReconciliationBankServiceCharge", "ReconciliationSchedule_ReconciliationScheduleId", "EXCEED.ReconciliationSchedule", "ReconciliationScheduleId");
        }
    }
}
