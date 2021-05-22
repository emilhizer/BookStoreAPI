// Database class for Authors

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookStore_API.Data {

  [Table("Books")]

  public partial class Book {

    public int Id { get; set; }
    public string Title { get; set; }
    public int? Year { get; set; }
    public string Isbn { get; set; }
    public string Summary { get; set; }
    public string Image { get; set; }
    //public double? Price { get; set; } // commented out due to weird decimal type error when reading from db
    public decimal? Price { get; set; }

    public int? AuthorId { get; set; }

    public virtual Author Author { get; set; } // note: may need to set relationship in SSMS/SQL to link Book to Author table


  } // Book 

} // namespace