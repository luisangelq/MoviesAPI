using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class CreateGenreDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
