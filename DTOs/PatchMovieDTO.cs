using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class PatchMovieDTO
    {
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public bool InCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
