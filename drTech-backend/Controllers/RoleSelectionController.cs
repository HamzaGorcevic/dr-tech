using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleSelectionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RoleSelectionController> _logger;

        public RoleSelectionController(IConfiguration configuration, ILogger<RoleSelectionController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Get available roles for testing
        /// </summary>
        [HttpGet("roles")]
        public IActionResult GetAvailableRoles()
        {
            var roles = new[]
            {
                new { 
                    Name = "HospitalAdmin", 
                    Description = "Hospital Administrator - Full access to hospital management, equipment, appointments, and staff",
                    Permissions = new[] { "Manage hospitals", "Manage equipment", "View all appointments", "Manage staff", "View audit logs" }
                },
                new { 
                    Name = "Doctor", 
                    Description = "Medical Doctor - Access to patient records, appointments, and medical services",
                    Permissions = new[] { "View patient records", "Manage appointments", "Access medical services", "Update patient information" }
                },
                new { 
                    Name = "InsuranceAgency", 
                    Description = "Insurance Agency - Manage contracts, discounts, and payment processing",
                    Permissions = new[] { "Manage contracts", "Process payments", "Manage discounts", "View payment history" }
                },
                new { 
                    Name = "InsuredUser", 
                    Description = "Patient/Insured User - Browse services, book appointments, and manage payments",
                    Permissions = new[] { "Browse services", "Book appointments", "View payment history", "Manage personal information" }
                }
            };

            return Ok(roles);
        }

        /// <summary>
        /// Select a role and get a JWT token for testing
        /// </summary>
        [HttpPost("select-role")]
        public IActionResult SelectRole([FromBody] RoleSelectionRequest request)
        {
            var validRoles = new[] { "HospitalAdmin", "Doctor", "InsuranceAgency", "InsuredUser" };
            
            if (!validRoles.Contains(request.Role))
            {
                return BadRequest(new { message = "Invalid role. Available roles: HospitalAdmin, Doctor, InsuranceAgency, InsuredUser" });
            }

            var token = GenerateJwtToken(request.Role, request.UserName ?? "TestUser");
            
            return Ok(new
            {
                message = $"Successfully logged in as {request.Role}",
                token = token,
                role = request.Role,
                userName = request.UserName ?? "TestUser",
                expiresIn = "24 hours",
                instructions = "Copy the token and click 'Authorize' in Swagger, then paste: Bearer " + token
            });
        }

        /// <summary>
        /// Get role information and permissions
        /// </summary>
        [HttpGet("role-info/{role}")]
        public IActionResult GetRoleInfo(string role)
        {
            var roleInfo = role.ToLower() switch
            {
                "hospitaladmin" => new
                {
                    Role = "HospitalAdmin",
                    Description = "Hospital Administrator",
                    AccessLevel = "Full System Access",
                    CanAccess = new[]
                    {
                        "All Hospital Management APIs",
                        "Equipment Management",
                        "Staff Management", 
                        "Appointment Management",
                        "Audit Logs",
                        "Error Logs",
                        "Request Logs",
                        "Throttle Logs"
                    },
                    SampleEndpoints = new[]
                    {
                        "GET /api/hospitals",
                        "POST /api/equipment",
                        "GET /api/audit",
                        "GET /api/error-logs"
                    }
                },
                "doctor" => new
                {
                    Role = "Doctor",
                    Description = "Medical Doctor",
                    AccessLevel = "Medical Services Access",
                    CanAccess = new[]
                    {
                        "Patient Records",
                        "Appointment Management",
                        "Medical Services",
                        "Equipment Status"
                    },
                    SampleEndpoints = new[]
                    {
                        "GET /api/patients",
                        "GET /api/appointments",
                        "POST /api/appointments",
                        "GET /api/services"
                    }
                },
                "insuranceagency" => new
                {
                    Role = "InsuranceAgency",
                    Description = "Insurance Agency",
                    AccessLevel = "Contract & Payment Management",
                    CanAccess = new[]
                    {
                        "Contract Management",
                        "Payment Processing",
                        "Discount Management",
                        "Agency Reports"
                    },
                    SampleEndpoints = new[]
                    {
                        "GET /api/contracts",
                        "POST /api/payments",
                        "GET /api/discounts",
                        "GET /api/agencies"
                    }
                },
                "insureduser" => new
                {
                    Role = "InsuredUser",
                    Description = "Patient/Insured User",
                    AccessLevel = "Patient Services Access",
                    CanAccess = new[]
                    {
                        "Browse Services",
                        "Book Appointments",
                        "View Payment History",
                        "Personal Information"
                    },
                    SampleEndpoints = new[]
                    {
                        "GET /api/user/hospitals",
                        "GET /api/user/services",
                        "POST /api/user/appointments",
                        "GET /api/user/payments"
                    }
                },
                _ => null
            };

            if (roleInfo == null)
            {
                return NotFound(new { message = "Role not found" });
            }

            return Ok(roleInfo);
        }

        private string GenerateJwtToken(string role, string userName)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                jwtKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm";
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role),
                new Claim("sub", userName),
                new Claim("role", role),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer ?? "DrTechAPI",
                audience: jwtAudience ?? "DrTechUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24), // Token valid for 24 hours
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RoleSelectionRequest
    {
        public string Role { get; set; } = string.Empty;
        public string? UserName { get; set; }
    }
}
