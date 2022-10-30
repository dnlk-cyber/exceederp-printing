using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class ExclusiveRange : ValidationAttribute
    {

        public int Min { get; set; }

        public ExclusiveRange(int min)
        {
            this.Min = min;
        }


        public override bool IsValid(object value)
        {
            //return base.IsValid(value);
            return (decimal)value > Min;
        }
        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    //return base.IsValid(value, validationContext);
        //    var valid =(decimal)value > Min;
        //    if (valid)
        //    {
        //        return ValidationResult.Success;
        //    }
        //    else
        //    {
        //        return new ValidationResult("{0} Must be greater than {1}");
        //    }
        //}
        //public override bool IsValid(object value)
        //{
        //    return base.IsValid(value);
        //}
    }
}