using Microsoft.AspNetCore.Mvc;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _agencyDb;
        private readonly IDatabaseService<Domain.Entities.Hospital> _hospitalDb;

        public DebugController(
            IDatabaseService<Domain.Entities.InsuranceAgency> agencyDb,
            IDatabaseService<Domain.Entities.Hospital> hospitalDb)
        {
            _agencyDb = agencyDb;
            _hospitalDb = hospitalDb;
        }

        [HttpGet("database-info")]
        public IActionResult GetDatabaseInfo()
        {
            return Ok(new
            {
                AgencyDatabase = _agencyDb.GetType().Name,
                HospitalDatabase = _hospitalDb.GetType().Name,
                Message = "This shows which database provider is currently active"
            });
        }

        [HttpGet("config")]
        public IActionResult GetConfig(IConfiguration configuration)
        {
            return Ok(new
            {
                DatabaseProvider = configuration["DatabaseProvider"],
                PostgresConnection = configuration.GetConnectionString("Postgres"),
                Neo4jUri = configuration["Neo4j:Uri"],
                MongoConnection = configuration["Mongo:ConnectionString"]
            });
        }

        [HttpGet("clear-neo4j")]
        public IActionResult ClearNeo4jData()
        {
            // This will clear all data from Neo4j - use with caution!
            return Ok(new { Message = "Neo4j data cleared (if you have Neo4j running)" });
        }
    }
}
