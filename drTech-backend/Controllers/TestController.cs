using Microsoft.AspNetCore.Mvc;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Test endpoint to verify API is working
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                message = "DrTech Healthcare Management API is running!",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                status = "Healthy"
            });
        }

        /// <summary>
        /// Get API information and available roles
        /// </summary>
        [HttpGet("info")]
        public IActionResult GetApiInfo()
        {
            return Ok(new
            {
                title = "DrTech Healthcare Management System",
                description = "A comprehensive healthcare management system with role-based access control",
                version = "1.0.0",
                availableRoles = new[]
                {
                    "HospitalAdmin - Full system access",
                    "Doctor - Medical services access", 
                    "InsuranceAgency - Contract & payment management",
                    "InsuredUser - Patient services access"
                },
                howToTest = new[]
                {
                    "1. Go to /api/roleselection/roles to see available roles",
                    "2. Use POST /api/roleselection/select-role to get a JWT token",
                    "3. Copy the token and click 'Authorize' in Swagger",
                    "4. Paste: Bearer {your-token}",
                    "5. Test different endpoints based on your role!"
                },
                swaggerUrl = "/swagger",
                corsEnabled = true,
                features = new[]
                {
                    "Hospital Management",
                    "Equipment Tracking", 
                    "Appointment Scheduling",
                    "Insurance Contracts",
                    "Payment Processing",
                    "Audit Logging",
                    "Request Monitoring",
                    "Role-based Access Control"
                }
            });
        }

        /// <summary>
        /// Test CORS functionality
        /// </summary>
        [HttpOptions("cors-test")]
        public IActionResult CorsTest()
        {
            return Ok(new { message = "CORS is working! This endpoint can be called from any domain." });
        }
    }
}
