using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Global;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExceedERP.Web.Models
{
    // You will not likely need to customize there, but it is necessary/easier to create our own 
    // project-specific implementations, so here they are:
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }

    // Must be expressed in terms of our custom Role and other types:
    public class ApplicationUser
        : IdentityUser<string, ApplicationUserLogin,
        ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();

            // Add any custom User properties/code here
        }
        public bool IsEnabled { get; set; }
        public string Email { get; set; }
        
        public string FirstName { get; set; }
      
        public string MiddleName { get; set; }

        public int EmployeeId { get; set;  }
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }

        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public async Task<ClaimsIdentity>
            GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            var userIdentity = await manager
                .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }


    // Must be expressed in terms of our custom UserRole:
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Description { get; set; }
        public string Category { get; set; }
        public ApplicationRole(string name , string desc )
            : this()
        {
            this.Name = name;
            this.Description = desc; 
            
        } 
        public ApplicationRole(string name , string desc , string group )
            : this()
        {
            this.Name = name;
            this.Description = desc;
            this.Category = group; 

        }

        // Add any custom Role properties/code here
    }


    // Must be expressed in terms of our custom types:
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole,
        string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext()
            : base("ExceedERPUserManagment")
            {
                //ApplicationDbInitializer dbInitializer = new ApplicationDbInitializer();

                //Database.SetInitializer(dbInitializer);
            }

        static ApplicationDbContext()
            
        {

            //ApplicationDbInitializer dbInitializer = new ApplicationDbInitializer();

            //Database.SetInitializer(dbInitializer);
            }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
         
        // Add the ApplicationGroups property:
        public virtual IDbSet<ApplicationGroup> ApplicationGroups { get; set; }
        public virtual IDbSet<UserBranchGroup> UserBranches { get; set;  }
        // Override OnModelsCreating:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("User");

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationUserGroup>(g => g.ApplicationUsers)
                .WithRequired().HasForeignKey(ag => ag.ApplicationGroupId);
            modelBuilder.Entity<ApplicationUserGroup>()
                .HasKey(r =>
                    new
                    {
                        ApplicationUserId = r.ApplicationUserId,
                        ApplicationGroupId = r.ApplicationGroupId
                    }).ToTable("ApplicationUserGroups");

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationGroupRole>(g => g.ApplicationRoles)
                .WithRequired().HasForeignKey(ap => ap.ApplicationGroupId);
            modelBuilder.Entity<ApplicationGroupRole>().HasKey(gr =>
                new
                {
                    ApplicationRoleId = gr.ApplicationRoleId,
                    ApplicationGroupId = gr.ApplicationGroupId
                }).ToTable("ApplicationGroupRoles");

                
            modelBuilder.Entity<UserBranchGroup>().HasKey(gr =>
                new
                {
                    Id = gr.Id,
                    //UserBranchId = gr.UserBranchId,
                    //ApplicationUserId = gr.ApplicationUserId
                }).ToTable("ApplicationUsersBranch");

        }

        public System.Data.Entity.DbSet<ExceedERP.Web.Models.RoleViewModel> RoleViewModels { get; set; }

         
    }


    // Most likely won't need to customize these either, but they were needed because we implemented
    // custom versions of all the other types:
    public class ApplicationUserStore
        : UserStore<ApplicationUser, ApplicationRole, string,
            ApplicationUserLogin, ApplicationUserRole,
            ApplicationUserClaim>,
        IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }


    public class ApplicationRoleStore
    : RoleStore<ApplicationRole, string, ApplicationUserRole>
    {
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
    }


    public class ApplicationGroup
    {
        public ApplicationGroup()
        {
            this.Id = Guid.NewGuid().ToString();
            this.ApplicationRoles = new List<ApplicationGroupRole>();
            this.ApplicationUsers = new List<ApplicationUserGroup>();
        }

        public ApplicationGroup(string name)
            : this()
        {
            this.Name = name;
        }

        public ApplicationGroup(string name, string description, Modules groupModule)
            : this(name)
        {
            this.Description = description;
            this.GroupModule = groupModule;
        }

        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Modules GroupModule { get; set; }//new 
        public virtual ICollection<ApplicationGroupRole> ApplicationRoles { get; set; }
        public virtual ICollection<ApplicationUserGroup> ApplicationUsers { get; set; }
    }

    

    public class ApplicationUserGroup
    {
        public string ApplicationUserId { get; set; }
        public string ApplicationGroupId { get; set; }
    }
    public class UserBranchGroup
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string UserBranchId { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ApplicationGroupRole
    {
        public string ApplicationGroupId { get; set; }
        public string ApplicationRoleId { get; set; }
    }

}