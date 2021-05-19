using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Data;
using BookStore_API.DTOs;

namespace BookStore_API.Mappings {

  public class Maps : Profile {

    // Use AutoMapper to link the main data class with the DTO class
    public Maps() {
      CreateMap<Author, AuthorDTO>().ReverseMap();
      CreateMap<Author, AuthorCreateDTO>().ReverseMap();
      CreateMap<Author, AuthorUpdateDTO>().ReverseMap();
      CreateMap<Book, BookDTO>().ReverseMap();
    }

  } // Maps

} // namespace
