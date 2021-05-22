using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Data;


namespace BookStore_API.DTOs {

  public class BookDTO {
    public int Id { get; set; }
    public string Title { get; set; }
    public int? Year { get; set; }
    public string Isbn { get; set; }
    public string Summary { get; set; }
    public string Image { get; set; }
    public decimal? Price { get; set; } // note, was double, but sql was money/decimal so was causing error

    public int? AuthorId { get; set; }

    public virtual AuthorDTO Author { get; set; } // note: may need to set relationship in SSMS/SQL to link Book to Author table
  } // BookDTO


  // Create a new book
  public class BookCreateDTO {
    [Required]
    public string Title { get; set; }
    // Optional
    public int? Year { get; set; }
    [Required]
    public string Isbn { get; set; }
    [StringLength(500)] // Should match nvarchar max in db
    public string Summary { get; set; }
    // Optional
    public string Image { get; set; }
    // Optional
    public decimal? Price { get; set; }
    [Required]
    public int? AuthorId { get; set; }
  } // BookCreateDTO


  // Update an existsing book
  public class BookUpdateDTO {
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    // Optional
    public int? Year { get; set; }
    //[Required]
    //public string Isbn { get; set; } // Don't let user update/change existing Isbn
    [Required]
    public string Summary { get; set; }
    [Required]
    public string Image { get; set; }
    // Optional
    public decimal? Price { get; set; } // commented out due to weird decimal type error when reading from db
    // Optional (??? really, shouldn't this be required?)
    public int? AuthorId { get; set; } // Can comment out if you don't let user update/change existing author
  } // BookUpdateDTO




} // namespace