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
using Microsoft.AspNetCore.Authorization;

namespace BookStore_API.Controllers {

  /// <summary>
  /// Endpoint used to interact with the Authors in the book store's db
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]

  // Improve external documentation
  // Notate that at a minimum this controller returns a status code of "200" (200 is "ok")
  [ProducesResponseType(StatusCodes.Status200OK)]

  public class AuthorsController : ControllerBase {

    private readonly IAuthorRepository _authorRepository;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;

    // Initializer for this class
    public AuthorsController(
      IAuthorRepository authorRepository,
      ILoggerService logger,
      IMapper mapper) {

      _authorRepository = authorRepository;
      _logger = logger;
      _mapper = mapper;

    } // AuthorsController initialization


    // Resuble return result method
    private ObjectResult InternalError(string message) {
      _logger.LogError(message);
      return StatusCode(500, "Something wehn wrong. Please contact the administrator.");
    }


    // Let controller know through built-in annotations that I'm defining an HTTP GET
    /// <summary>
    /// Get All Authors
    /// </summary>
    /// <returns>List of Authors</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    // Using MVC, async returns lots of info into IActionResult
    public async Task<IActionResult> GetAuthors() {
      try {
        _logger.LogInfo("Attempted GET all authors");
        // Interact with DTO only (not directly to Data object)
        // Automapper maps DTO to Data object
        var authors = await _authorRepository.FindAll();
        // Map to data transfer object
        // Map authors to the Author DTO
        var response = _mapper.Map<IList<AuthorDTO>>(authors);

        _logger.LogInfo("Successfully got all authors");
        // Respond with requested info with a "200" response code ("ok")
        return Ok(response);
      }
      catch (Exception ex) {
        //_logger.LogError($"{ex.Message} - {ex.InnerException}");
        // Internal Server error is "500"
        //return StatusCode(500, "Something wehn wrong. Please contact the administrator.");
        return InternalError($"{ex.Message} - {ex.InnerException}");
      }
    } // GetAuthors


    /// <summary>
    /// Get One Author by Author Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>One Author</returns>
    [HttpGet("{id}")]
    [Authorize] // Any authenticated user can access Author by Id
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]


    // Get one author
    public async Task<IActionResult> GetAuthor(int id) {
      try {
        _logger.LogInfo($"Attempted GET one author with Id: {id}");

        var author = await _authorRepository.FindById(id);

        // If nothing found, then provide Error 404 error
        if (author == null) {
          _logger.LogWarn($"No author found with Id: {id}");
          return NotFound(); // NotFound is error type 404
        }

        var response = _mapper.Map<AuthorDTO>(author);

        _logger.LogInfo($"Successfully got author with Id: {id}");
        // Respond with requested info with a "200" response code ("ok")
        return Ok(response);
      }

      catch (Exception ex) {
        return InternalError($"{ex.Message} - {ex.InnerException}");
      }
    } // GetAuthor

    // Note <param name> below should say authorDTO because that's what we're matching to internally
    //  but we don't want our external users to know about this detail, so just call it "author"
    //  We may get a VS warning, but that's ok [project (right-click) > options > compiler > ignore warnings: 1572;1573]
    // Create
    /// <summary>
    /// Create a New Author
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Administrator")] // Separate roles with commas inside of quotes
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO) {
      _logger.LogInfo($"Author Submittion Attempted");
      try {
        if (authorDTO == null) {
          _logger.LogWarn($"Empty Request was submitted");
          // ModelState is DTOs required fields
          return BadRequest(ModelState);
        }
        if (!ModelState.IsValid) {
          _logger.LogWarn($"Author Data was Incomplete");
          return BadRequest(ModelState);
        }

        // Map all the data from the authorDTO and map into the Author data set
        var author = _mapper.Map<Author>(authorDTO);
        var isSuccess = await _authorRepository.Create(author);
        // Did something outside user's control fail?
        if (!isSuccess) {
          return InternalError($"Author creation failed");
        }
        _logger.LogInfo($"Author Created");
        return Created("Create", new { author });
      } // try

      catch (Exception ex) {
        return InternalError($"{ex.Message} - {ex.InnerException}");
      } // catch
    } // Create (author)


    // Update
    /// <summary>
    /// Update an Existing Author
    /// </summary>
    /// <param name="id"></param>
    /// <param name="author"></param>
    /// <returns></returns>
    [HttpPut("{id}")] // Put is the "post" version of update
    [Authorize(Roles = "Administrator, Customer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO) {
      _logger.LogInfo($"Author Update Attempted with Id: {id}");
      try {
        if (id < 1 || authorDTO == null || id != authorDTO.Id) {
          _logger.LogWarn($"Bad Id or Empty Request was submitted");
          return BadRequest();
        }
        //if (await _authorRepository.FindById(id) == null) {
        var doesExist = await _authorRepository.DoesExist(id);
        if (!doesExist) { 
          _logger.LogWarn($"Update Author Not Found with Id: {id}");
          return NotFound();
        }

        if (!ModelState.IsValid) {
          _logger.LogWarn($"Author Data was Incomplete");
          return BadRequest(ModelState);
        }

        // Map all the data from the authorDTO and map into the Author data set
        var author = _mapper.Map<Author>(authorDTO);
        var isSuccess = await _authorRepository.Update(author);
        // Did something outside user's control fail?
        if (!isSuccess) {
          return InternalError($"Author Update Operation Failed");
        }
        _logger.LogInfo($"Author Update Success with Id: {id}");
        return NoContent(); // "Ok" (204) result returned but with nothing additional to report back to user
      } // try

      catch (Exception ex) {
        return InternalError($"{ex.Message} - {ex.InnerException}");
      } // catch
    } // Update (author)


    // Delete
    /// <summary>
    /// Delete an Existing Author
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Customer")] // Yes, it's weird that only Customer can delete - just testing
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id) {
      _logger.LogInfo($"Delete Author Attempted with Id: {id}");
      try {
        if (id < 1) {
          _logger.LogWarn($"Author Delete Failed with Bad Data Id: {id}");
          return BadRequest();
        }

        var author = await _authorRepository.FindById(id);
        if (author == null) {
          _logger.LogWarn($"Delete Author Not Found with Id: {id}");
          return NotFound();
        }
        var isSuccess = await _authorRepository.Delete(author);

        // Did something outside user's control fail?
        if (!isSuccess) {
          return InternalError($"Author Delete Operation Failed");
        }
        _logger.LogInfo($"Author Delete Success for Id: {id}");
        return NoContent(); // "Ok" (204) author deleted, but with nothing additional to report back to user
      } // try

      catch (Exception ex) {
        return InternalError($"{ex.Message} - {ex.InnerException}");
      } // catch
    } // Delete (author)



  } // AuthorsController

} // namespace
