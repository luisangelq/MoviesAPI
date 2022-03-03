using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] validTypes;

        public FileTypeValidation(string[] validTypes)
        {
            this.validTypes = validTypes;
        }

        public FileTypeValidation(FileTypeGroup fileTypeGroup)
        {
            if (fileTypeGroup == FileTypeGroup.Image)
            {
                validTypes = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) { return ValidationResult.Success; }

            IFormFile formFile = value as IFormFile;
            if (formFile == null) { return ValidationResult.Success; }

            if (!validTypes.Contains(formFile.ContentType))
            {
                List<string> formats = new List<string>();
                foreach (string format in validTypes) { formats.Add("." + format.Split("/").Last()); }

                return new ValidationResult($"File type must be {string.Join(", ", formats)}");
            }

            return ValidationResult.Success;
        }
    }
}
