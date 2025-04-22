using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UserForUpdateDTO : IValidatableObject
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Apartment { get; set; }
        [Phone]
        public string? Phone_Number { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name == null && Surname == null && Email == null && Apartment == null && Phone_Number == null)
            {
                yield return new ValidationResult(
                    "Al menos un campo debe estar presente para actualizar la torre.",
                    new[] { nameof(UserForUpdateDTO)});
            }
        }
    }
}
