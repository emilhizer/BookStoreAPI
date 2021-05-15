using System;
using System.Collections.Generic;
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

} // namespace
