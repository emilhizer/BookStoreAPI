using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BookStore_API.Contracts {

  public interface IRepositoryBase<T> where T : class {

    // Using asynchronous programming here
    // Task is multithreaded, async function
    //  then, <return type>
    //  then, the function name (outside callers call this function)
    //  with (inputs)

    // Return all entries as an IList type, and the list will have type <T>, which is whatever type was specified when calling this class
    //  and this function has no inputs
    Task<IList<T>> FindAll();

    // Return one item of type <T>, which is whatever type was specified when calling this class
    //  with an input of id
    Task<T> FindById(int id);

    // Check if a record for "id" exists
    Task<bool> DoesExist(int id);

    // Create a new entity of type <T>
    Task<bool> Create(T entity);

    // Update an entity
    Task<bool> Update(T entity);

    // Delete an entity
    Task<bool> Delete(T entity);

    // Must always commit changes to db
    Task<bool> Save();

  } // IRepositoryBase

} // namespace
