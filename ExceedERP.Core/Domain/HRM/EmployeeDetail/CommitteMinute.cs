using ExceedERP.Core.Domain.HRM.Discipline;
using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
	public class CommitteMinute:TrackUserOperation
	{
		[Key]
		public int CommitteMinuteId { get; set; }
		
		[Display(Name = "EmployeeDetail_Committee_FileNo", ResourceType = typeof(Localization.Resources))]
		public string FileNo { get; set; }
		[Display(Name = "EmployeeDetail_Committee_Agenda", ResourceType = typeof(Localization.Resources))]
		public string Agenda { get; set; }
		[Display(Name = "EmployeeDetail_Committee_PlaceOfMetting", ResourceType = typeof(Localization.Resources))]
		public string PlaceOfMetting { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		[Display(Name = "EmployeeDetail_Committee_Date", ResourceType = typeof(Localization.Resources))]
		public DateTime Date { get; set; }
		[Display(Name = "EmployeeDetail_Committee_Date", ResourceType = typeof(Localization.Resources))]
		public string DateAmharic { get; set; }
		[Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
		public string Remark { get; set; }
		public int CommitteeId { get; set; }
		public Committee Committee { get; set; }
		public int DisciplinaryCaseId { get; set; }
		public DisciplinaryCase DisciplinaryCase { get; set; }

	}
}