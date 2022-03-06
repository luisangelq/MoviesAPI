using MoviesAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class CreateMovieDTO : PatchMovieDTO
    {
        [FileWeightValidation(MaxWeightMB: 4)]
        [FileTypeValidation(FileTypeGroup.Image)]
        public IFormFile Poster { get; set; }
    }
}
