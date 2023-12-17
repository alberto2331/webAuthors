using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Validations
{
    public class FirstCapitalLetterAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //value = is the value of the field that I want to validate
            //validationContext = we have access to the complete entity that we want to validate. In this case it would be Author
            if( value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            
            var firstLetter = value.ToString()[0].ToString();
            
            if(firstLetter != firstLetter.ToUpper())
            {
                return new ValidationResult("The first letter of this field must be capitalized");
            }
            
            return ValidationResult.Success;
        }
    }
}
