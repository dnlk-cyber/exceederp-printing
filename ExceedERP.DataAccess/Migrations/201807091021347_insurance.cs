namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class insurance : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Insurance.InsuranceType", "InsuranceCategoryId", "Insurance.InsuranceCategory");
            DropIndex("Insurance.InsuranceType", new[] { "InsuranceCategoryId" });
            AddColumn("Insurance.InsuranceGroup", "InsuranceCategoryId", c => c.Int(nullable: false));
            AddColumn("Insurance.InsuranceCategory", "InsuranceTypeId", c => c.Int(nullable: false));
            AddColumn("Insurance.InsuranceType", "InsuranceTypeGroup", c => c.Int(nullable: false));
            AddColumn("Insurance.InsuranceType", "LookupId", c => c.Int(nullable: false));
            AddColumn("Insurance.OtherInsurance", "Name", c => c.String());
            AlterColumn("Insurance.InsuranceGroup", "GroupName", c => c.String());
            AlterColumn("Insurance.InsuranceCategory", "Remark", c => c.String());
            AlterColumn("Insurance.InsuranceType", "Name", c => c.String());
            CreateIndex("Insurance.InsuranceCategory", "InsuranceTypeId");
            CreateIndex("Insurance.InsuranceType", "LookupId");
            AddForeignKey("Insurance.InsuranceType", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Insurance.InsuranceCategory", "InsuranceTypeId", "Insurance.InsuranceType", "InsuranceTypeId", cascadeDelete: true);
            DropColumn("Insurance.InsuranceType", "InsuranceCategoryId");
        }
        
        public override void Down()
        {
            AddColumn("Insurance.InsuranceType", "InsuranceCategoryId", c => c.Int(nullable: false));
            DropForeignKey("Insurance.InsuranceCategory", "InsuranceTypeId", "Insurance.InsuranceType");
            DropForeignKey("Insurance.InsuranceType", "LookupId", "Common.Lookup");
            DropIndex("Insurance.InsuranceType", new[] { "LookupId" });
            DropIndex("Insurance.InsuranceCategory", new[] { "InsuranceTypeId" });
            AlterColumn("Insurance.InsuranceType", "Name", c => c.String(nullable: false));
            AlterColumn("Insurance.InsuranceCategory", "Remark", c => c.String(nullable: false));
            AlterColumn("Insurance.InsuranceGroup", "GroupName", c => c.String(nullable: false));
            DropColumn("Insurance.OtherInsurance", "Name");
            DropColumn("Insurance.InsuranceType", "LookupId");
            DropColumn("Insurance.InsuranceType", "InsuranceTypeGroup");
            DropColumn("Insurance.InsuranceCategory", "InsuranceTypeId");
            DropColumn("Insurance.InsuranceGroup", "InsuranceCategoryId");
            CreateIndex("Insurance.InsuranceType", "InsuranceCategoryId");
            AddForeignKey("Insurance.InsuranceType", "InsuranceCategoryId", "Insurance.InsuranceCategory", "InsuranceCategoryId", cascadeDelete: true);
        }
    }
}
