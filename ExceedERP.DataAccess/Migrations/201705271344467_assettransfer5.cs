namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assettransfer5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.UserCardItems", "UserCard_EmployeeId", "HRM.Employee");
            DropIndex("EXCEED.UserCardItems", new[] { "UserCard_EmployeeId" });
            RenameColumn(table: "EXCEED.UserCardItems", name: "UserCard_EmployeeId", newName: "EmployeeId");
            AddColumn("EXCEED.UserCardItems", "AssetRegistrationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.UserCardItems", "Name", c => c.String());
            AddColumn("EXCEED.UserCardItems", "Active", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.UserCardItems", "EmployeeId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.UserCardItems", "EmployeeId");
            CreateIndex("EXCEED.UserCardItems", "AssetRegistrationId");
            AddForeignKey("EXCEED.UserCardItems", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.UserCardItems", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            DropColumn("EXCEED.UserCardItems", "PersonID");
            DropColumn("EXCEED.UserCardItems", "ItemDesc");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.UserCardItems", "ItemDesc", c => c.String());
            AddColumn("EXCEED.UserCardItems", "PersonID", c => c.String());
            DropForeignKey("EXCEED.UserCardItems", "EmployeeId", "HRM.Employee");
            DropForeignKey("EXCEED.UserCardItems", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.UserCardItems", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.UserCardItems", new[] { "EmployeeId" });
            AlterColumn("EXCEED.UserCardItems", "EmployeeId", c => c.Int());
            DropColumn("EXCEED.UserCardItems", "Active");
            DropColumn("EXCEED.UserCardItems", "Name");
            DropColumn("EXCEED.UserCardItems", "AssetRegistrationId");
            RenameColumn(table: "EXCEED.UserCardItems", name: "EmployeeId", newName: "UserCard_EmployeeId");
            CreateIndex("EXCEED.UserCardItems", "UserCard_EmployeeId");
            AddForeignKey("EXCEED.UserCardItems", "UserCard_EmployeeId", "HRM.Employee", "EmployeeId");
        }
    }
}
