using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Controllers
{

    [ApiController]
    [Route("api/genres")]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var entities = await context.Genres.ToListAsync();
            var dtos = mapper.Map<List<GenreDTO>>(entities);

            return dtos;
        }

        [HttpGet("{id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDTO>> GetById(int id)
        {
            var entity = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<GenreDTO>(entity);

            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateGenreDTO createGenreDTO)
        {
                var entity = mapper.Map<Genre>(createGenreDTO); 
                context.Add(entity);

                await context.SaveChangesAsync();

                var genreDTO = mapper.Map<GenreDTO>(entity);

                return new CreatedAtRouteResult(
                    "getGenre", new { id = entity.Id }, genreDTO);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateGenreDTO createGenreDTO)
        {
            var entity = mapper.Map<Genre>(createGenreDTO);
            entity.Id = id;
            context.Entry(entity).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await context.Genres.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            context.Remove(new Genre() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
