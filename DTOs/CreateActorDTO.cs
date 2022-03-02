using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class CreateActorDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
