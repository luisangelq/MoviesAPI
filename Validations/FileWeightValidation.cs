using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validations
{
    public class FileWeightValidation : ValidationAttribute
    {
        private readonly int maxWeightMB;

        public FileWeightValidation(int MaxWeightMB)
        {
            maxWeightMB = MaxWeightMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) { return ValidationResult.Success; }

            IFormFile formFile = value as IFormFile;
            if (formFile == null) { return ValidationResult.Success; }

            if (formFile.Length > maxWeightMB * 1024 * 1024) { return new ValidationResult($"The file weight must not be higher than {maxWeightMB}MB"); }

            return ValidationResult.Success;
        }
    }
}
