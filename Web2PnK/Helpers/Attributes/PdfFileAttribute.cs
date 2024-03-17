using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Helpers.Attributes;

[AttributeUsage(AttributeTargets.Property |
                AttributeTargets.Field, AllowMultiple = false)]
public class PdfFileAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(value?.ToString())) return ValidationResult.Success;
        
        return (value?.ToString()).StartsWith("data:application/pdf;base64,JVB") && 100 < value.ToString()?.Length
            ? ValidationResult.Success
            : new ValidationResult("The file must be .pdf");
    }
}