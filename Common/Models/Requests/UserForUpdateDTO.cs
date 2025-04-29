sing System.ComponentModel.DataAnnotations;

public class UserForUpdateDTO : IValidatableObject
{
    public string? Name { get; set; }
    public string? Surname { get; set; }

    [EmailAddress]
    public string? Email { get; set; } 

    public int? Apartment_Id { get; set; }

    [Phone]
    public string? Phone_Number { get; set; } 

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Name == null && Surname == null && Email == null && Apartment_Id == null && Phone_Number == null)
        {
            yield return new ValidationResult(
                "Al menos un campo debe estar presente para actualizar el usuario.",
                new[] { nameof(UserForUpdateDTO) });
        }
    }
}