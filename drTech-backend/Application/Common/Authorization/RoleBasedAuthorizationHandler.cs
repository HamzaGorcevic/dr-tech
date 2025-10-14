using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace drTech_backend.Application.Common.Authorization
{
    public class RoleBasedAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleBasedAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            var user = context.User;
            var httpContext = _httpContextAccessor.HttpContext;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var userRole = user.FindFirst(ClaimTypes.Role)?.Value ?? user.FindFirst("role")?.Value;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;

                // Check if user has the required role
                if (requirement.AllowedRoles.Contains(userRole))
                {
                // Additional role-specific checks
                if (!string.IsNullOrEmpty(userRole) && !string.IsNullOrEmpty(userId) && httpContext != null && IsAuthorizedForResource(userRole, userId, httpContext))
                {
                    context.Succeed(requirement);
                }
                }
            }

            return Task.CompletedTask;
        }

        private bool IsAuthorizedForResource(string userRole, string userId, HttpContext httpContext)
        {
            if (httpContext?.Request?.RouteValues == null)
                return true;

            // Hospital Admin can only access their own hospital's resources
            if (userRole == "HospitalAdmin")
            {
                return CanAccessHospitalResource(userId, httpContext);
            }

            // Doctor can only access their own department's resources
            if (userRole == "Doctor")
            {
                return CanAccessDoctorResource(userId, httpContext);
            }

            // Insurance Agency can only access their own agency's resources
            if (userRole == "InsuranceAgency")
            {
                return CanAccessAgencyResource(userId, httpContext);
            }

            // Insured User can only access their own patient data
            if (userRole == "InsuredUser")
            {
                return CanAccessPatientResource(userId, httpContext);
            }

            return true;
        }

        private bool CanAccessHospitalResource(string userId, HttpContext httpContext)
        {
            // Extract hospital ID from route or query parameters
            var hospitalId = GetResourceIdFromContext(httpContext, "hospitalId");
            if (string.IsNullOrEmpty(hospitalId))
                return true; // Allow if no specific hospital ID in request

            // In a real implementation, you would check if the user is associated with this hospital
            // For now, we'll allow access (this should be implemented with proper user-hospital associations)
            return true;
        }

        private bool CanAccessDoctorResource(string userId, HttpContext httpContext)
        {
            // Extract doctor ID from route or query parameters
            var doctorId = GetResourceIdFromContext(httpContext, "doctorId");
            if (string.IsNullOrEmpty(doctorId))
                return true; // Allow if no specific doctor ID in request

            // Check if the user is the same doctor
            return userId == doctorId;
        }

        private bool CanAccessAgencyResource(string userId, HttpContext httpContext)
        {
            // Extract agency ID from route or query parameters
            var agencyId = GetResourceIdFromContext(httpContext, "agencyId");
            if (string.IsNullOrEmpty(agencyId))
                return true; // Allow if no specific agency ID in request

            // In a real implementation, you would check if the user is associated with this agency
            // For now, we'll allow access (this should be implemented with proper user-agency associations)
            return true;
        }

        private bool CanAccessPatientResource(string userId, HttpContext httpContext)
        {
            // Extract patient ID from route or query parameters
            var patientId = GetResourceIdFromContext(httpContext, "patientId");
            if (string.IsNullOrEmpty(patientId))
                return true; // Allow if no specific patient ID in request

            // In a real implementation, you would check if the user is associated with this patient
            // For now, we'll allow access (this should be implemented with proper user-patient associations)
            return true;
        }

        private string? GetResourceIdFromContext(HttpContext httpContext, string parameterName)
        {
            // Try to get from route values first
            if (httpContext.Request.RouteValues.TryGetValue(parameterName, out var routeValue))
            {
                return routeValue?.ToString();
            }

            // Try to get from query parameters
            if (httpContext.Request.Query.TryGetValue(parameterName, out var queryValue))
            {
                return queryValue.FirstOrDefault();
            }

            return null;
        }
    }

    public class RoleRequirement : IAuthorizationRequirement
    {
        public string[] AllowedRoles { get; }

        public RoleRequirement(params string[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
        }
    }
}
