using System.ComponentModel.DataAnnotations;
using VideoGameCatalogue.API.Domain;

namespace VideoGameCatalogue.API.Contracts
{
    /// <summary>
    /// Validates that a string is one of a fixed list of allowed values. Missing values are left
    /// to [Required], so an empty field gets one error rather than two.
    /// </summary>
    public abstract class LookupValueAttribute(IReadOnlyList<string> allowedValues) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string text || text.Length == 0 || allowedValues.Contains(text))
                return ValidationResult.Success;

            var memberNames = validationContext.MemberName is { } name ? new[] { name } : null;
            return new ValidationResult(
                $"{validationContext.DisplayName} must be one of: {string.Join(", ", allowedValues)}.",
                memberNames);
        }
    }

    public sealed class AllowedGenreAttribute() : LookupValueAttribute(Lookups.Genres);

    public sealed class AllowedPlatformAttribute() : LookupValueAttribute(Lookups.Platforms);
}
