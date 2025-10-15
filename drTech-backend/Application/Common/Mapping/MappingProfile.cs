using AutoMapper;
using drTech_backend.Application.Common.DTOs;

namespace drTech_backend.Application.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Core
            CreateMap<Domain.Entities.Hospital, HospitalDto>()
                .ForMember(d => d.DepartmentsCount, o => o.MapFrom(s => s.Departments.Count));
            CreateMap<HospitalCreateDto, Domain.Entities.Hospital>();

            CreateMap<Domain.Entities.Department, DepartmentDto>();
            CreateMap<DepartmentCreateDto, Domain.Entities.Department>();

            CreateMap<Domain.Entities.Doctor, DoctorDto>();
            CreateMap<DoctorCreateDto, Domain.Entities.Doctor>();

            CreateMap<Domain.Entities.Equipment, EquipmentDto>();
            CreateMap<EquipmentCreateDto, Domain.Entities.Equipment>();

            CreateMap<Domain.Entities.Patient, PatientDto>();
            CreateMap<PatientCreateDto, Domain.Entities.Patient>();

            CreateMap<Domain.Entities.Appointment, AppointmentDto>();
            CreateMap<AppointmentCreateDto, Domain.Entities.Appointment>();

            // Business
            CreateMap<Domain.Entities.InsuranceAgency, InsuranceAgencyDto>()
                .ForMember(d => d.ContractsCount, o => o.MapFrom(s => s.Contracts.Count));
            CreateMap<InsuranceAgencyCreateDto, Domain.Entities.InsuranceAgency>();

            CreateMap<Domain.Entities.AgencyContract, AgencyContractDto>();
            CreateMap<AgencyContractCreateDto, Domain.Entities.AgencyContract>();

            CreateMap<Domain.Entities.MedicalService, MedicalServiceDto>();
            CreateMap<MedicalServiceCreateDto, Domain.Entities.MedicalService>();

            CreateMap<Domain.Entities.PriceListItem, PriceListItemDto>();
            CreateMap<PriceListItemCreateDto, Domain.Entities.PriceListItem>();

            CreateMap<Domain.Entities.Reservation, ReservationDto>();
            CreateMap<ReservationCreateDto, Domain.Entities.Reservation>();

            CreateMap<Domain.Entities.PreContract, PreContractDto>();
            CreateMap<PreContractCreateDto, Domain.Entities.PreContract>();

            CreateMap<Domain.Entities.Payment, PaymentDto>();
            CreateMap<PaymentCreateDto, Domain.Entities.Payment>();

            CreateMap<Domain.Entities.EquipmentStatusLog, EquipmentStatusLogDto>();
            CreateMap<EquipmentStatusLogCreateDto, Domain.Entities.EquipmentStatusLog>();

            CreateMap<Domain.Entities.EquipmentServiceOrder, EquipmentServiceOrderDto>();
            CreateMap<EquipmentServiceOrderCreateDto, Domain.Entities.EquipmentServiceOrder>();

            CreateMap<Domain.Entities.Discount, DiscountDto>();
            CreateMap<DiscountCreateDto, Domain.Entities.Discount>();

            CreateMap<Domain.Entities.DiscountRequest, DiscountRequestDto>();
            CreateMap<DiscountRequestCreateDto, Domain.Entities.DiscountRequest>();

            // Logs/Auth
            CreateMap<Domain.Entities.ErrorLog, ErrorLogDto>();
            CreateMap<Domain.Entities.RequestLog, RequestLogDto>();
            CreateMap<Domain.Entities.ThrottleLog, ThrottleLogDto>();
            CreateMap<Domain.Entities.AuditLog, AuditLogDto>();

            CreateMap<Domain.Entities.User, UserDto>();
            CreateMap<Domain.Entities.RefreshToken, RefreshTokenDto>();
        }
    }

    // Inline DTO previously declared here was moved to Application.Common.DTOs
}


