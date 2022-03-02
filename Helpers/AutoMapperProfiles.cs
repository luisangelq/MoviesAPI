﻿using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<CreateGenreDTO, Genre>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<CreateActorDTO, Actor>();
        }
    }
}
