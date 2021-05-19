using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStore_API.Contracts;
using BookStore_API.Data;


namespace BookStore_API.Services {

  public class AuthorRepository : IAuthorRepository {

    private readonly ApplicationDbContext _db;

    // Constructor for this class AuthorRepository, shorthand is "ctor, tab, tab"
    public AuthorRepository(ApplicationDbContext db) {
      _db = db; // dependency injection
    }

    // Return all authors in the Authors table from the db
    public async Task<IList<Author>> FindAll() {
      var authors = await _db.Authors.ToListAsync();
      return authors;
    }

    // Return just one Author from the Authors table from the db
    public async Task<Author> FindById(int id) {
      var author = await _db.Authors.FindAsync(id);
      return author;
    }

    // Check to see if a record exists in the db for an "id"
    public async Task<bool> DoesExist(int id) {
      return await _db.Authors.AnyAsync(q => q.Id == id);
    }

    // Create (add) a new Author record for the Authors table in the db
    public async Task<bool> Create(Author entity) {
      await _db.Authors.AddAsync(entity);
      return await Save();
    }

    // Update an existing Author record
    public async Task<bool> Update(Author entity) {
      _db.Authors.Update(entity); // this isn't doing anything asynchronously, just updating a local (in-memory) entity
      return await Save(); // this "Save" function is asyncronous, so we do need the await
    }

    public async Task<bool> Delete(Author entity) {
      _db.Authors.Remove(entity); // this isn't doing anything asynchronously, just updating a local (in-memory) entity
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

  } // AuthorRepository

} // namespace
