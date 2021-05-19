using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Data;


namespace BookStore_API.DTOs {

  public class AuthorDTO {

    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Bio { get; set; }

    public virtual IList<BookDTO> Books { get; set; }
  } // AuthorDTO


  // Only allow a few inputs when creating an Author
  //  i.e., user cannot specify the Id or the List of Books connected to this Author
  public class AuthorCreateDTO {
    // Data annotations
    [Required]
    public string Firstname { get; set; }
    [Required]
    public string Lastname { get; set; }
    // Optional
    public string Bio { get; set; }
  } // AuthorCreateDTO

  // Update Author DTO
  public class AuthorUpdateDTO {
    [Required]
    public int Id { get; set; }
    [Required]
    public string Firstname { get; set; }
    [Required]
    public string Lastname { get; set; }
    public string Bio { get; set; }
  } // AuthorUpdateDTO



} // namespace
