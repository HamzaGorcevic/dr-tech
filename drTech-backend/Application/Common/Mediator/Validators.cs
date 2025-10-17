using FluentValidation;
using MediatR;

namespace drTech_backend.Application.Common.Mediator
{
	public class CreateDoctorValidator : AbstractValidator<CreateCommand<Domain.Entities.Doctor>>
	{
		public CreateDoctorValidator()
		{
			RuleFor(x => x.Entity.FullName).NotEmpty().MinimumLength(3).MaximumLength(100);
			RuleFor(x => x.Entity.DepartmentId).NotEmpty();
			RuleFor(x => x.Entity.UserId).NotEmpty();
		}
	}

	public class CreateHospitalValidator : AbstractValidator<CreateCommand<Domain.Entities.Hospital>>
	{
		public CreateHospitalValidator()
		{
			RuleFor(x => x.Entity.Name).NotEmpty().MinimumLength(3);
			RuleFor(x => x.Entity.City).NotEmpty();
		}
	}

	public class CreateDepartmentValidator : AbstractValidator<CreateCommand<Domain.Entities.Department>>
	{
		public CreateDepartmentValidator()
		{
			RuleFor(x => x.Entity.Name).NotEmpty().MinimumLength(2);
			RuleFor(x => x.Entity.HospitalId).NotEmpty();
		}
	}

	public class CreatePatientValidator : AbstractValidator<CreateCommand<Domain.Entities.Patient>>
	{
		public CreatePatientValidator()
		{
			RuleFor(x => x.Entity.UserId).NotEmpty();
			RuleFor(x => x.Entity.FullName).NotEmpty().MinimumLength(3);
		}
	}

	public class CreateMedicalServiceValidator : AbstractValidator<CreateCommand<Domain.Entities.MedicalService>>
	{
		public CreateMedicalServiceValidator()
		{
			RuleFor(x => x.Entity.Name).NotEmpty().MinimumLength(2);
			RuleFor(x => x.Entity.Code).NotEmpty();
			RuleFor(x => x.Entity.DepartmentId).NotEmpty();
		}
	}

	public class CreatePriceListItemValidator : AbstractValidator<CreateCommand<Domain.Entities.PriceListItem>>
	{
		public CreatePriceListItemValidator()
		{
			RuleFor(x => x.Entity.HospitalId).NotEmpty();
			RuleFor(x => x.Entity.MedicalServiceId).NotEmpty();
			RuleFor(x => x.Entity.Price).GreaterThanOrEqualTo(0);
			RuleFor(x => x.Entity.ValidUntil).GreaterThan(x => x.Entity.ValidFrom);
		}
	}

	public class CreateReservationValidator : AbstractValidator<CreateCommand<Domain.Entities.Reservation>>
	{
		public CreateReservationValidator()
		{
			RuleFor(x => x.Entity.PatientId).NotEmpty();
			RuleFor(x => x.Entity.HospitalId).NotEmpty();
			RuleFor(x => x.Entity.StartsAtUtc).LessThan(x => x.Entity.EndsAtUtc);
		}
	}

	public class CreateEquipmentValidator : AbstractValidator<CreateCommand<Domain.Entities.Equipment>>
	{
		public CreateEquipmentValidator()
		{
			RuleFor(x => x.Entity.SerialNumber).NotEmpty().MinimumLength(2);
			RuleFor(x => x.Entity.DepartmentId).NotEmpty();
		}
	}

	public class CreateInsuranceAgencyValidator : AbstractValidator<CreateCommand<Domain.Entities.InsuranceAgency>>
	{
		public CreateInsuranceAgencyValidator()
		{
			RuleFor(x => x.Entity.Name).NotEmpty().MinimumLength(3);
			RuleFor(x => x.Entity.City).NotEmpty();
		}
	}

	public class CreateAgencyContractValidator : AbstractValidator<CreateCommand<Domain.Entities.AgencyContract>>
	{
		public CreateAgencyContractValidator()
		{
			RuleFor(x => x.Entity.InsuranceAgencyId).NotEmpty();
			RuleFor(x => x.Entity.HospitalId).NotEmpty();
			RuleFor(x => x.Entity.CoveragePercent).InclusiveBetween(0, 100);
			RuleFor(x => x.Entity.EndsOn).GreaterThan(x => x.Entity.StartsOn);
		}
	}

	public class CreatePaymentValidator : AbstractValidator<CreateCommand<Domain.Entities.Payment>>
	{
		public CreatePaymentValidator()
		{
			RuleFor(x => x.Entity.PreContractId).NotEmpty();
			RuleFor(x => x.Entity.Amount).GreaterThan(0);
		}
	}

	public class CreateDiscountValidator : AbstractValidator<CreateCommand<Domain.Entities.Discount>>
	{
		public CreateDiscountValidator()
		{
			RuleFor(x => x.Entity.PatientId).NotEmpty();
			RuleFor(x => x.Entity.DiscountPercent).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
			RuleFor(x => x.Entity.MaxDiscountAmount).GreaterThanOrEqualTo(0);
		}
	}

	public class CreateDiscountRequestValidator : AbstractValidator<CreateCommand<Domain.Entities.DiscountRequest>>
	{
		public CreateDiscountRequestValidator()
		{
			RuleFor(x => x.Entity.InsuranceAgencyId).NotEmpty();
			RuleFor(x => x.Entity.PatientId).NotEmpty();
			RuleFor(x => x.Entity.RequestedDiscountPercent).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
		}
	}

	public class CreateAppointmentValidator : AbstractValidator<CreateCommand<Domain.Entities.Appointment>>
	{
		public CreateAppointmentValidator()
		{
			RuleFor(x => x.Entity.DoctorId).NotEmpty();
			RuleFor(x => x.Entity.PatientId).NotEmpty();
			RuleFor(x => x.Entity.StartsAtUtc).LessThan(x => x.Entity.EndsAtUtc);
		}
	}
}


