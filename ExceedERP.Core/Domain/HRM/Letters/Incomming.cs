////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	domain\hrm\letters.cs
//
// summary:	Implements the letters class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Letters
{
   ////////////////////////////////////////////////////////////////////////////////////////////////////
   /// <summary>    A letters. </summary>
   ///
   /// <remarks>    Mekanehiwot, 10/31/2016. </remarks>
   ////////////////////////////////////////////////////////////////////////////////////////////////////

   public   class Incomming:TrackUserOperation 
    {
  
       [Key]
       public int IncommingId { get; set; }
         [Required]
         [Display(Name = "HR_Letters_Incomming_LettersRefNo", ResourceType = typeof(Localization.Resources))]
       public string LettersRefNo { get; set; }
     
         [Required]
         [Display(Name = "HR_Letters_Incomming_Owner", ResourceType = typeof(Localization.Resources))]
         public LetterOwner Owner { get; set; }
       [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_Letters_Incomming_LetterDate", ResourceType = typeof(Localization.Resources))]
       public DateTime LetterDate { get; set; }
       [Display(Name = "HR_Letters_Incomming_LetterDate", ResourceType = typeof(Localization.Resources))]
       public string LetterDateAmmharic { get; set; }
       [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
       [Required]
       [Display(Name = "HR_Letters_Incomming_RecivedDate", ResourceType = typeof(Localization.Resources))]
       public DateTime RecivedDate { get; set; }
       [Display(Name = "HR_Letters_Incomming_RecivedDate", ResourceType = typeof(Localization.Resources))]
       public string RecivedDateAmharic { get; set; } 
       [Required]
       [Display(Name = "HR_Letters_Incomming_Subject", ResourceType = typeof(Localization.Resources))]
       public string Subject { get; set; }
       [Display(Name = "HR_Letters_Incomming_Folder", ResourceType = typeof(Localization.Resources))]
       public string Folder { get; set; }
         [Display(Name = "HR_Letters_Incomming_Cluster", ResourceType = typeof(Localization.Resources))]
       public string Cluster { get; set; }
       [Display(Name = "HR_Letters_Incomming_NoOfAttachment", ResourceType = typeof(Localization.Resources))]
    
       public int NoOfAttachment { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
       public string Remark { get; set; }



    }
}
