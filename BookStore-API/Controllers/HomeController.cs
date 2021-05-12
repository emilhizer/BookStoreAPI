﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using BookStore_API.Contracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers {
  /// <summary>
  /// This is a test API controller
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]

  public class HomeController : ControllerBase {

    private readonly ILoggerService _logger;

    public HomeController(ILoggerService logger) {
      _logger = logger;
    }

    /// <summary>
    /// Get values
    /// </summary>
    /// <returns></returns>
    // GET: api/values
    [HttpGet]
    public IEnumerable<string> Get() {
      _logger.LogInfo("Accessed Home Controller");
      return new string[] { "value1", "value2" };
    }

    /// <summary>
    /// Get a value
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // GET api/values/5
    [HttpGet("{id}")]
    public string Get(int id) {
      _logger.LogDebug("Got a value");
      return "value";
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody] string value) {
      _logger.LogError("This is an error");
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value) {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id) {
      _logger.LogWarn("This is a warning");
    }
  }
}
