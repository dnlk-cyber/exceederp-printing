namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm12 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Health.PatientCardDetail", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientsLeave", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.Prescription", "PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.ReferralSheet", "PatientCardId", "Health.PatientCard");
            DropIndex("Health.PatientCardDetail", new[] { "PatientCardId" });
            DropIndex("Health.PatientsLeave", new[] { "PatientCardId" });
            DropIndex("Health.Prescription", new[] { "PatientCardId" });
            DropIndex("Health.ReferralSheet", new[] { "PatientCardId" });
            RenameColumn(table: "Health.PatientCardDetail", name: "PatientCardId", newName: "PatientCard_PatientCardId");
            RenameColumn(table: "Health.PatientsLeave", name: "PatientCardId", newName: "PatientCard_PatientCardId");
            RenameColumn(table: "Health.Prescription", name: "PatientCardId", newName: "PatientCard_PatientCardId");
            RenameColumn(table: "Health.ReferralSheet", name: "PatientCardId", newName: "PatientCard_PatientCardId");
            AddColumn("Health.PatientCard", "PatientId", c => c.Int(nullable: false));
            AddColumn("Health.PatientCardDetail", "PatientId", c => c.Int(nullable: false));
            AddColumn("Health.PatientsLeave", "PatientId", c => c.Int(nullable: false));
            AddColumn("Health.Prescription", "PatientId", c => c.Int(nullable: false));
            AddColumn("Health.ReferralSheet", "PatientId", c => c.Int(nullable: false));
            AlterColumn("Health.PatientCardDetail", "PatientCard_PatientCardId", c => c.String(maxLength: 128));
            AlterColumn("Health.PatientsLeave", "PatientCard_PatientCardId", c => c.String(maxLength: 128));
            AlterColumn("Health.Prescription", "PatientCard_PatientCardId", c => c.String(maxLength: 128));
            AlterColumn("Health.ReferralSheet", "PatientCard_PatientCardId", c => c.String(maxLength: 128));
            CreateIndex("Health.PatientCardDetail", "PatientCard_PatientCardId");
            CreateIndex("Health.PatientsLeave", "PatientCard_PatientCardId");
            CreateIndex("Health.Prescription", "PatientCard_PatientCardId");
            CreateIndex("Health.ReferralSheet", "PatientCard_PatientCardId");
            AddForeignKey("Health.PatientCardDetail", "PatientCard_PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.PatientsLeave", "PatientCard_PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.Prescription", "PatientCard_PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.ReferralSheet", "PatientCard_PatientCardId", "Health.PatientCard", "PatientCardId");
        }
        
        public override void Down()
        {
            DropForeignKey("Health.ReferralSheet", "PatientCard_PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.Prescription", "PatientCard_PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientsLeave", "PatientCard_PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientCardDetail", "PatientCard_PatientCardId", "Health.PatientCard");
            DropIndex("Health.ReferralSheet", new[] { "PatientCard_PatientCardId" });
            DropIndex("Health.Prescription", new[] { "PatientCard_PatientCardId" });
            DropIndex("Health.PatientsLeave", new[] { "PatientCard_PatientCardId" });
            DropIndex("Health.PatientCardDetail", new[] { "PatientCard_PatientCardId" });
            AlterColumn("Health.ReferralSheet", "PatientCard_PatientCardId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("Health.Prescription", "PatientCard_PatientCardId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("Health.PatientsLeave", "PatientCard_PatientCardId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("Health.PatientCardDetail", "PatientCard_PatientCardId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("Health.ReferralSheet", "PatientId");
            DropColumn("Health.Prescription", "PatientId");
            DropColumn("Health.PatientsLeave", "PatientId");
            DropColumn("Health.PatientCardDetail", "PatientId");
            DropColumn("Health.PatientCard", "PatientId");
            RenameColumn(table: "Health.ReferralSheet", name: "PatientCard_PatientCardId", newName: "PatientCardId");
            RenameColumn(table: "Health.Prescription", name: "PatientCard_PatientCardId", newName: "PatientCardId");
            RenameColumn(table: "Health.PatientsLeave", name: "PatientCard_PatientCardId", newName: "PatientCardId");
            RenameColumn(table: "Health.PatientCardDetail", name: "PatientCard_PatientCardId", newName: "PatientCardId");
            CreateIndex("Health.ReferralSheet", "PatientCardId");
            CreateIndex("Health.Prescription", "PatientCardId");
            CreateIndex("Health.PatientsLeave", "PatientCardId");
            CreateIndex("Health.PatientCardDetail", "PatientCardId");
            AddForeignKey("Health.ReferralSheet", "PatientCardId", "Health.PatientCard", "PatientCardId", cascadeDelete: true);
            AddForeignKey("Health.Prescription", "PatientCardId", "Health.PatientCard", "PatientCardId", cascadeDelete: true);
            AddForeignKey("Health.PatientsLeave", "PatientCardId", "Health.PatientCard", "PatientCardId", cascadeDelete: true);
            AddForeignKey("Health.PatientCardDetail", "PatientCardId", "Health.PatientCard", "PatientCardId", cascadeDelete: true);
        }
    }
}
