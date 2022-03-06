using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;
using System.ComponentModel;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string container = "movies";

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieDTO>>> Get()
        {
            var movies = await context.Movies.ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }

        [HttpGet("{id:int}", Name = "getMovie")]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return mapper.Map<MovieDTO>(movie);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] CreateMovieDTO createMovieDTO)
        {
            var entity = mapper.Map<Movie>(createMovieDTO);

            if (createMovieDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createMovieDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createMovieDTO.Poster.FileName);
                    entity.Poster = await fileStorage
                        .SaveFile(content, extension, container, createMovieDTO.Poster.ContentType);
                }
            }

            context.Add(entity);
            await context.SaveChangesAsync();

            var movieDTO = mapper.Map<MovieDTO>(entity);

            return new CreatedAtRouteResult("getMovie", new { id = entity.Id }, movieDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] CreateMovieDTO createMovieDTO)
        {
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movieDB == null)
            {
                return NotFound();
            }

            movieDB = mapper.Map(createMovieDTO, movieDB);

            if (createMovieDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createMovieDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createMovieDTO.Poster.FileName);
                    movieDB.Poster = await fileStorage
                        .EditFile(content, extension, container, movieDB.Poster, createMovieDTO.Poster.ContentType);
                }
            }
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PatchMovieDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (entityDB == null)
            {
                return NotFound();
            }

            var entityDTO = mapper.Map<PatchMovieDTO>(entityDB);
            patchDocument.ApplyTo(entityDTO, ModelState);

            var isValid = TryValidateModel(entityDTO);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entityDTO, entityDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await context.Movies.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            context.Remove(new Movie() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
