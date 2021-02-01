using ExceedERP.Core.Domain.Global;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ExceedERP.Web.Models
{
    public class RoleViewModel
    {
        public string RoleViewModelId { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set;  }
        [Display(Name = "Category")]
        public string Category { get; set;  }
        public bool IsIngroup { get; set;  }
    }

    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            this.RolesList = new List<SelectListItem>();
            this.GroupsList = new List<SelectListItem>();
        }
        
        public string Email { get; set; }
        
        public string FirstName { get; set; }
         
        public string MiddleName { get; set; }
         
        public string LastName { get; set; }
        
        public int PhoneNo { get; set; }
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "User Name")]
        
        public string UserName { get; set; }
        [DisplayName("Is Enabled")]
        public bool IsEnabled { get; set;  }

        // We will still use this, so leave it here:
        public ICollection<SelectListItem> RolesList { get; set; }

        // Add a GroupsList Property:
        public ICollection<SelectListItem> GroupsList { get; set; }
    }

    public class GroupViewModel
    {
        public GroupViewModel()
        {
            this.UsersList = new List<SelectListItem>();
            this.RolesList = new List<SelectListItem>();
        }
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAllowed { get; set;  }
        public Modules GroupModule { get; set; }//new 
        public ICollection<SelectListItem> UsersList { get; set; }
        public ICollection<SelectListItem> RolesList { get; set; }
    }
}