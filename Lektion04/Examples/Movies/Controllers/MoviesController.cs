using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.models;
using Movies.services;

namespace Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService = new MovieService();

        // GET: api/<MoviesController>
        [HttpGet]
        public ActionResult<IEnumerable<Movie>> Get()
        {
            return Ok(_movieService.Movies);
        }

        // GET api/<MoviesController>/5
        [HttpGet("{id}")]
        public ActionResult<Movie> Get(int id)
        {
            var movie = _movieService.Movies.FirstOrDefault(m => m.Id == id);
            if  (movie == null) return NotFound();
            return Ok(movie);
        }

        // POST api/<MoviesController>
        [HttpPost]
        public void Post([FromBody] String value)
        {
        }

        // PUT api/<MoviesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}