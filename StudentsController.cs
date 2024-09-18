using ElasticsearchDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchDemo.Controllers
{

    public class StudentsController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName = "students";

        public StudentsController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        // GET: api/students
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var searchResponse = await _elasticClient.SearchAsync<Student>(s => s
                .Index(_indexName)
                .Query(q => q.MatchAll())
            );

            return Ok(searchResponse.Documents);
        }

        // GET api/students/5
        [HttpGet]
        public async Task<ActionResult<Student>> Get(int id)
        {
            var searchResponse = await _elasticClient.SearchAsync<Student>(s => s
                .Index(_indexName)
                .Query(q => q
                    .Term(t => t.Field(f => f.Id).Value(id))
                )
            );

            var student = searchResponse.Documents.FirstOrDefault();
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST api/students
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            var indexResponse = await _elasticClient.IndexDocumentAsync(student);

            if (indexResponse.IsValid)
            {
                return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
            }

            return BadRequest(indexResponse.OriginalException.Message);
        }

        // PUT api/students/5
        [HttpPut]
        public async Task<IActionResult> Put(int id, [FromBody] Student student)
        {
            var searchResponse = await _elasticClient.GetAsync<Student>(id, idx => idx.Index(_indexName));

            if (!searchResponse.Found)
            {
                return NotFound();
            }

            student.Id = id; // Ensure the ID is set correctly
            var indexResponse = await _elasticClient.IndexDocumentAsync(student);

            if (indexResponse.IsValid)
            {
                return NoContent();
            }

            return BadRequest(indexResponse.OriginalException.Message);
        }

        // DELETE api/students/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteResponse = await _elasticClient.DeleteAsync<Student>(id, idx => idx.Index(_indexName));

            if (deleteResponse.IsValid)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
