using Microsoft.AspNetCore.Authorization;

namespace drTech_backend.Application.Common.Authorization
{
    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }

    public class RequireHospitalAdminAttribute : RequireRoleAttribute
    {
        public RequireHospitalAdminAttribute() : base("HospitalAdmin") { }
    }

    public class RequireDoctorAttribute : RequireRoleAttribute
    {
        public RequireDoctorAttribute() : base("Doctor") { }
    }

    public class RequireInsuranceAgencyAttribute : RequireRoleAttribute
    {
        public RequireInsuranceAgencyAttribute() : base("InsuranceAgency") { }
    }

    public class RequireInsuredUserAttribute : RequireRoleAttribute
    {
        public RequireInsuredUserAttribute() : base("InsuredUser") { }
    }

    public class RequireHospitalAdminOrDoctorAttribute : RequireRoleAttribute
    {
        public RequireHospitalAdminOrDoctorAttribute() : base("HospitalAdmin", "Doctor") { }
    }

    public class RequireHospitalAdminOrInsuranceAgencyAttribute : RequireRoleAttribute
    {
        public RequireHospitalAdminOrInsuranceAgencyAttribute() : base("HospitalAdmin", "InsuranceAgency") { }
    }

    public class RequireAnyRoleAttribute : RequireRoleAttribute
    {
        public RequireAnyRoleAttribute() : base("HospitalAdmin", "Doctor", "InsuranceAgency", "InsuredUser") { }
    }
}
