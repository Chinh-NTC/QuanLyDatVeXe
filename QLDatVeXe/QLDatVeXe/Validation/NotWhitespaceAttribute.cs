using System.ComponentModel.DataAnnotations;

namespace QLDatVeXe.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NotWhitespaceAttribute : ValidationAttribute
{
    public NotWhitespaceAttribute()
    {
        ErrorMessage = "Trường này không được để trống hoặc chỉ chứa khoảng trắng.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true; // Use [Required] for null checks
        }

        if (value is string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        return true;
    }
}
