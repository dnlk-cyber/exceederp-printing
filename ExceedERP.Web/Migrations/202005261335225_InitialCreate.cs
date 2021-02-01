namespace ExceedERP.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "User.ApplicationGroups",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "User.ApplicationGroupRoles",
                c => new
                    {
                        ApplicationRoleId = c.String(nullable: false, maxLength: 128),
                        ApplicationGroupId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationRoleId, t.ApplicationGroupId })
                .ForeignKey("User.ApplicationGroups", t => t.ApplicationGroupId, cascadeDelete: true)
                .Index(t => t.ApplicationGroupId);
            
            CreateTable(
                "User.ApplicationUserGroups",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        ApplicationGroupId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.ApplicationGroupId })
                .ForeignKey("User.ApplicationGroups", t => t.ApplicationGroupId, cascadeDelete: true)
                .Index(t => t.ApplicationGroupId);
            
            CreateTable(
                "User.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        Category = c.String(),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "User.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("User.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("User.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "User.RoleViewModels",
                c => new
                    {
                        RoleViewModelId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Category = c.String(),
                        IsIngroup = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RoleViewModelId);
            
            CreateTable(
                "User.ApplicationUsersBranch",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(),
                        UserBranchId = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "User.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IsEnabled = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        EmployeeId = c.String(),
                        LastName = c.String(),
                        PhoneNo = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "User.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("User.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "User.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("User.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("User.AspNetUserRoles", "UserId", "User.AspNetUsers");
            DropForeignKey("User.AspNetUserLogins", "UserId", "User.AspNetUsers");
            DropForeignKey("User.AspNetUserClaims", "UserId", "User.AspNetUsers");
            DropForeignKey("User.AspNetUserRoles", "RoleId", "User.AspNetRoles");
            DropForeignKey("User.ApplicationUserGroups", "ApplicationGroupId", "User.ApplicationGroups");
            DropForeignKey("User.ApplicationGroupRoles", "ApplicationGroupId", "User.ApplicationGroups");
            DropIndex("User.AspNetUserLogins", new[] { "UserId" });
            DropIndex("User.AspNetUserClaims", new[] { "UserId" });
            DropIndex("User.AspNetUsers", "UserNameIndex");
            DropIndex("User.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("User.AspNetUserRoles", new[] { "UserId" });
            DropIndex("User.AspNetRoles", "RoleNameIndex");
            DropIndex("User.ApplicationUserGroups", new[] { "ApplicationGroupId" });
            DropIndex("User.ApplicationGroupRoles", new[] { "ApplicationGroupId" });
            DropTable("User.AspNetUserLogins");
            DropTable("User.AspNetUserClaims");
            DropTable("User.AspNetUsers");
            DropTable("User.ApplicationUsersBranch");
            DropTable("User.RoleViewModels");
            DropTable("User.AspNetUserRoles");
            DropTable("User.AspNetRoles");
            DropTable("User.ApplicationUserGroups");
            DropTable("User.ApplicationGroupRoles");
            DropTable("User.ApplicationGroups");
        }
    }
}
