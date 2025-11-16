using System.ComponentModel.DataAnnotations;

namespace Csharp3_A3.Validation
{
	public class FutureDateTimeAttribute : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value is DateTime dateTimeValue)
			{
				if (dateTimeValue > DateTime.Now)
				{
					return ValidationResult.Success;
				}
				else
				{
					return new ValidationResult(ErrorMessage ?? "The date and time must be in the future.");
				}
			}
			return new ValidationResult(ErrorMessage ?? "Invalid date and time format.");
		}
	}
}
