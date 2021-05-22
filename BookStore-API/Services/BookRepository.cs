using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStore_API.Contracts;
using BookStore_API.Data;


namespace BookStore_API.Services {

  public class BookRepository : IBookRepository {

    private readonly ApplicationDbContext _db;

    // Constructor for this class BookRepository, shorthand is "ctor, tab, tab"
    public BookRepository(ApplicationDbContext db) {
      _db = db; // dependency injection
    }

    // Return all books in the Books table from the db
    public async Task<IList<Book>> FindAll() {
      var books = await _db.Books.ToListAsync();
      return books;
    }

    // Return just one book from the Books table from the db
    public async Task<Book> FindById(int id) {
      var book = await _db.Books.FindAsync(id);
      return book;
    }

    // Check to see if a record exists in the db for an "id"
    public async Task<bool> DoesExist(int id) {
      return await _db.Books.AnyAsync(q => q.Id == id);
    }

    // Create (add) a new book record for the Books table in the db
    public async Task<bool> Create(Book entity) {
      await _db.Books.AddAsync(entity);
      return await Save();
    }

    // Update an existing book record
    public async Task<bool> Update(Book entity) {
      _db.Books.Update(entity); // this isn't doing anything asynchronously, just updating a local (in-memory) entity
      return await Save(); // this "Save" function is asyncronous, so we do need the await
    }

    public async Task<bool> Delete(Book entity) {
      _db.Books.Remove(entity); // this isn't doing anything asynchronously, just updating a local (in-memory) entity
      return await Save(); // this "Save" function is asyncronous, so we do need the await
    }

    // 1. make function asynchronous with "async" declarition
    // 2. add "await" before calling the _db.SaveChangesAsync() method
    // 3. then "changes" is an int and not a Task<int> -- Task<int> means it's an async variable waiting for a response
    // Note: "var" is lazy way of letting Visual Studio figure out the type of the variable (in this case "int")
    public async Task<bool> Save() {
      var changes = await _db.SaveChangesAsync();
      return changes > 0;
    }

  } // BookRepository

} // namespace
