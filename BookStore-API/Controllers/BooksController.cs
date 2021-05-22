using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookStore_API.Contracts;
using AutoMapper;
using BookStore_API.DTOs;
using BookStore_API.Data;


namespace BookStore_API.Controllers {

  /// <summary>
  /// Endpoint used to interact with the Books in the book store's db
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]

  // Improve external documentation
  // Notate that at a minimum this controller returns a status code of "200" (200 is "ok")
  [ProducesResponseType(StatusCodes.Status200OK)]

  public class BooksController : ControllerBase {

    private readonly IBookRepository _bookRepository;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;

    // Initializer for this class
    public BooksController(
      IBookRepository bookRepository,
      ILoggerService logger,
      IMapper mapper) {

      _bookRepository = bookRepository;
      _logger = logger;
      _mapper = mapper;

    } // BooksController initialization


    // Resuble return result method
    private ObjectResult InternalError(string message) {
      _logger.LogError(message);
      return StatusCode(500, "Something wehn wrong. Please contact the administrator.");
    }

    // Get Controller Name as a String
    private string GetControllerActionNames() {
      var controller = ControllerContext.ActionDescriptor.ControllerName;
      var action = ControllerContext.ActionDescriptor.ActionName;
      return $"{controller} - {action}";
    }


    // Let controller know through built-in annotations that I'm defining an HTTP GET
    /// <summary>
    /// Get All Books
    /// </summary>
    /// <returns>List of Books</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    // Using MVC, async returns lots of info into IActionResult
    public async Task<IActionResult> GetBooks() {
      var location = GetControllerActionNames();
      try {
        _logger.LogInfo($"{location}: Attempted GET all records");
        // Interact with DTO only (not directly to Data object)
        // Automapper maps DTO to Data object
        var books = await _bookRepository.FindAll();
        // Map to data transfer object
        // Map books to the Book DTO
        var response = _mapper.Map<IList<BookDTO>>(books);

        _logger.LogInfo($"{location}: Successfully got all records");
        // Respond with requested info with a "200" response code ("ok")
        return Ok(response);
      }
      catch (Exception ex) {
        //_logger.LogError($"{ex.Message} - {ex.InnerException}");
        // Internal Server error is "500"
        //return StatusCode(500, "Something wehn wrong. Please contact the administrator.");
        return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
      }
    } // GetBooks


    /// <summary>
    /// Get One Book by Book Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>One Book</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]


    // Get one book
    public async Task<IActionResult> GetBook(int id) {
      var location = GetControllerActionNames();
      try {
        _logger.LogInfo($"{location}: Attempted GET with Id: {id}");

        var book = await _bookRepository.FindById(id);

        // If nothing found, then provide Error 404 error
        if (book == null) {
          _logger.LogWarn($"{location}: No entry found with Id: {id}");
          return NotFound(); // NotFound is error type 404
        }

        var response = _mapper.Map<BookDTO>(book);

        _logger.LogInfo($"{location}: GET success with Id: {id}");
        // Respond with requested info with a "200" response code ("ok")
        return Ok(response);
      }

      catch (Exception ex) {
        return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
      }
    } // GetBook

    // Note <param name> below should say bookDTO because that's what we're matching to internally
    //  but we don't want our external users to know about this detail, so just call it "book"
    //  We may get a VS warning, but that's ok [project (right-click) > options > compiler > ignore warnings: 1572;1573]
    // Create
    /// <summary>
    /// Create a New Book
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO) {
      var location = GetControllerActionNames();
      _logger.LogInfo($"{location}: Create attempted");
      try {
        if (bookDTO == null) {
          _logger.LogWarn($"{location}: Empty Request was submitted");
          // ModelState is DTOs required fields
          return BadRequest(ModelState);
        }
        if (!ModelState.IsValid) {
          _logger.LogWarn($"{location}: Data was incomplete");
          return BadRequest(ModelState);
        }

        // Map all the data from the bookDTO and map into the Book data set
        var book = _mapper.Map<Book>(bookDTO);
        var isSuccess = await _bookRepository.Create(book);
        // Did something outside user's control fail?
        if (!isSuccess) {
          return InternalError($"{location}: Create failed");
        }
        _logger.LogInfo($"{location}: Create success");
        return Created("Create", new { book });
      } // try

      catch (Exception ex) {
        return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
      } // catch
    } // Create (book)


    // Update
    /// <summary>
    /// Update an Existing Book
    /// </summary>
    /// <param name="id"></param>
    /// <param name="book"></param>
    /// <returns></returns>
    [HttpPut("{id}")] // Put is the "post" version of update
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO) {
      var location = GetControllerActionNames();
      _logger.LogInfo($"{location}: Update Attempted with Id: {id}");
      try {
        if (id < 1 || bookDTO == null || id != bookDTO.Id) { // id in parameter (URL) must equal the id in the JSON body/payload received from user
          _logger.LogWarn($"{location}: Bad Id or Empty Request was submitted");
          return BadRequest();
        }
        //if (await _bookRepository.FindById(id) == null) {
        var doesExist = await _bookRepository.DoesExist(id);
        if (!doesExist) { 
          _logger.LogWarn($"{location}: Update record not found with Id: {id}");
          return NotFound();
        }

        if (!ModelState.IsValid) {
          _logger.LogWarn($"{location}: Update data was incomplete");
          return BadRequest(ModelState);
        }

        // Map all the data from the bookDTO and map into the Book data set
        var book = _mapper.Map<Book>(bookDTO);
        var isSuccess = await _bookRepository.Update(book);
        // Did something outside user's control fail?
        if (!isSuccess) {
          return InternalError($"{location}: Update Operation Failed");
        }
        _logger.LogInfo($"{location}: Update Success with Id: {id}");
        return NoContent(); // "Ok" (204) result returned but with nothing additional to report back to user
      } // try

      catch (Exception ex) {
        return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
      } // catch
    } // Update (book)


    // Delete
    /// <summary>
    /// Delete an Existing Book
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id) {
      var location = GetControllerActionNames();
      _logger.LogInfo($"{location}: Delete Attempted with Id: {id}");
      try {
        if (id < 1) {
          _logger.LogWarn($"{location}: Delete Failed with Bad Data Id: {id}");
          return BadRequest();
        }

        var book = await _bookRepository.FindById(id);
        if (book == null) {
          _logger.LogWarn($"{location}: Delete record Not Found with Id: {id}");
          return NotFound();
        }
        var isSuccess = await _bookRepository.Delete(book);

        // Did something outside user's control fail?
        if (!isSuccess) {
          return InternalError($"{location}: Delete operation failed");
        }
        _logger.LogInfo($"{location}: Delete success for Id: {id}");
        return NoContent(); // "Ok" (204) book deleted, but with nothing additional to report back to user
      } // try

      catch (Exception ex) {
        return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
      } // catch
    } // Delete (book)



  } // BooksController

} // namespace
