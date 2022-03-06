using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/actors")]
    public class ActorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string container = "actors";

        public ActorsController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Actors.AsQueryable();
            await HttpContext.InsertPageParameters(queryable, paginationDTO.PageRecordsTotal);

            var entities = await queryable.Page(paginationDTO).ToListAsync();
            var dtos = mapper.Map<List<ActorDTO>>(entities);

            return dtos;
        }

        [HttpGet("{id:int}", Name = "getActor")]
        public async Task<ActionResult<ActorDTO>> GetById(int id)
        {
            var entity = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<ActorDTO>(entity);

            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] CreateActorDTO createActorDTO)
        {
            var entity = mapper.Map<Actor>(createActorDTO);
            if (createActorDTO.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createActorDTO.Photo.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createActorDTO.Photo.FileName);
                    entity.Photo = await fileStorage
                        .SaveFile(content, extension, container, createActorDTO.Photo.ContentType);
                }
            }
            context.Add(entity);

            await context.SaveChangesAsync();

            var actorDTO = mapper.Map<ActorDTO>(entity);

            return new CreatedAtRouteResult(
                "getActor", new { id = entity.Id }, actorDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] CreateActorDTO createActorDTO)
        {
            var actorDB = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actorDB == null)
            {
                return NotFound();
            }

            actorDB = mapper.Map(createActorDTO, actorDB);

            if (createActorDTO.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createActorDTO.Photo.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createActorDTO.Photo.FileName);
                    actorDB.Photo = await fileStorage
                        .EditFile(content, extension, container, actorDB.Photo, createActorDTO.Photo.ContentType);
                }
            }

            await context.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PatchActorDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityDB = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);

            if (entityDB == null)
            {
                return NotFound();
            }

            var entityDTO = mapper.Map<PatchActorDTO>(entityDB);
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
            var exist = await context.Actors.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            context.Remove(new Actor() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
