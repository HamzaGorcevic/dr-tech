using MediatR;
using FluentValidation;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Application.Common.Mediator
{
    // Concrete handlers for InsuranceAgency
    public class GetAllInsuranceAgenciesQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.InsuranceAgency>, IReadOnlyList<Domain.Entities.InsuranceAgency>>
    {
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _db;
        public GetAllInsuranceAgenciesQueryHandler(IDatabaseService<Domain.Entities.InsuranceAgency> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.InsuranceAgency>> Handle(GetAllQuery<Domain.Entities.InsuranceAgency> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdInsuranceAgencyQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.InsuranceAgency>, Domain.Entities.InsuranceAgency?>
    {
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _db;
        public GetByIdInsuranceAgencyQueryHandler(IDatabaseService<Domain.Entities.InsuranceAgency> db) { _db = db; }
        public async Task<Domain.Entities.InsuranceAgency?> Handle(GetByIdQuery<Domain.Entities.InsuranceAgency> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateInsuranceAgencyCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.InsuranceAgency>, Domain.Entities.InsuranceAgency>
    {
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _db;
        public CreateInsuranceAgencyCommandHandler(IDatabaseService<Domain.Entities.InsuranceAgency> db) { _db = db; }
        public async Task<Domain.Entities.InsuranceAgency> Handle(CreateCommand<Domain.Entities.InsuranceAgency> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            return request.Entity;
        }
    }

    public class UpdateInsuranceAgencyCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.InsuranceAgency>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _db;
        public UpdateInsuranceAgencyCommandHandler(IDatabaseService<Domain.Entities.InsuranceAgency> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.InsuranceAgency> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            return Unit.Value;
        }
    }

    public class DeleteInsuranceAgencyCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.InsuranceAgency>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.InsuranceAgency> _db;
        public DeleteInsuranceAgencyCommandHandler(IDatabaseService<Domain.Entities.InsuranceAgency> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.InsuranceAgency> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            return Unit.Value;
        }
    }

    // Concrete handlers for Hospital
    public class GetAllHospitalsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Hospital>, IReadOnlyList<Domain.Entities.Hospital>>
    {
        private readonly IDatabaseService<Domain.Entities.Hospital> _db;
        public GetAllHospitalsQueryHandler(IDatabaseService<Domain.Entities.Hospital> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Hospital>> Handle(GetAllQuery<Domain.Entities.Hospital> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdHospitalQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.Hospital>, Domain.Entities.Hospital?>
    {
        private readonly IDatabaseService<Domain.Entities.Hospital> _db;
        public GetByIdHospitalQueryHandler(IDatabaseService<Domain.Entities.Hospital> db) { _db = db; }
        public async Task<Domain.Entities.Hospital?> Handle(GetByIdQuery<Domain.Entities.Hospital> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateHospitalCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Hospital>, Domain.Entities.Hospital>
    {
        private readonly IDatabaseService<Domain.Entities.Hospital> _db;
        public CreateHospitalCommandHandler(IDatabaseService<Domain.Entities.Hospital> db) { _db = db; }
        public async Task<Domain.Entities.Hospital> Handle(CreateCommand<Domain.Entities.Hospital> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            return request.Entity;
        }
    }

    public class UpdateHospitalCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.Hospital>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Hospital> _db;
        public UpdateHospitalCommandHandler(IDatabaseService<Domain.Entities.Hospital> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.Hospital> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    public class DeleteHospitalCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.Hospital>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Hospital> _db;
        public DeleteHospitalCommandHandler(IDatabaseService<Domain.Entities.Hospital> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.Hospital> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    // Concrete handlers for Patient
    public class GetAllPatientsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Patient>, IReadOnlyList<Domain.Entities.Patient>>
    {
        private readonly IDatabaseService<Domain.Entities.Patient> _db;
        public GetAllPatientsQueryHandler(IDatabaseService<Domain.Entities.Patient> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Patient>> Handle(GetAllQuery<Domain.Entities.Patient> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdPatientQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.Patient>, Domain.Entities.Patient?>
    {
        private readonly IDatabaseService<Domain.Entities.Patient> _db;
        public GetByIdPatientQueryHandler(IDatabaseService<Domain.Entities.Patient> db) { _db = db; }
        public async Task<Domain.Entities.Patient?> Handle(GetByIdQuery<Domain.Entities.Patient> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreatePatientCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Patient>, Domain.Entities.Patient>
    {
        private readonly IDatabaseService<Domain.Entities.Patient> _db;
        public CreatePatientCommandHandler(IDatabaseService<Domain.Entities.Patient> db) { _db = db; }
        public async Task<Domain.Entities.Patient> Handle(CreateCommand<Domain.Entities.Patient> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            return request.Entity;
        }
    }

    public class UpdatePatientCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.Patient>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Patient> _db;
        public UpdatePatientCommandHandler(IDatabaseService<Domain.Entities.Patient> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.Patient> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    public class DeletePatientCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.Patient>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Patient> _db;
        public DeletePatientCommandHandler(IDatabaseService<Domain.Entities.Patient> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.Patient> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            return Unit.Value;
        }
    }

    // Concrete handlers for Doctor
    public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Doctor>, IReadOnlyList<Domain.Entities.Doctor>>
    {
        private readonly IDatabaseService<Domain.Entities.Doctor> _db;
        public GetAllDoctorsQueryHandler(IDatabaseService<Domain.Entities.Doctor> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Doctor>> Handle(GetAllQuery<Domain.Entities.Doctor> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class CreateDoctorCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Doctor>, Domain.Entities.Doctor>
    {
        private readonly IDatabaseService<Domain.Entities.Doctor> _db;
        public CreateDoctorCommandHandler(IDatabaseService<Domain.Entities.Doctor> db) { _db = db; }
        public async Task<Domain.Entities.Doctor> Handle(CreateCommand<Domain.Entities.Doctor> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            return request.Entity;
        }
    }

    public class GetByIdDoctorQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.Doctor>, Domain.Entities.Doctor?>
    {
        private readonly IDatabaseService<Domain.Entities.Doctor> _db;
        public GetByIdDoctorQueryHandler(IDatabaseService<Domain.Entities.Doctor> db) { _db = db; }
        public async Task<Domain.Entities.Doctor?> Handle(GetByIdQuery<Domain.Entities.Doctor> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    // Concrete handlers for Department
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Department>, IReadOnlyList<Domain.Entities.Department>>
    {
        private readonly IDatabaseService<Domain.Entities.Department> _db;
        public GetAllDepartmentsQueryHandler(IDatabaseService<Domain.Entities.Department> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Department>> Handle(GetAllQuery<Domain.Entities.Department> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Department>, Domain.Entities.Department>
    {
        private readonly IDatabaseService<Domain.Entities.Department> _db;
        public CreateDepartmentCommandHandler(IDatabaseService<Domain.Entities.Department> db) { _db = db; }
        public async Task<Domain.Entities.Department> Handle(CreateCommand<Domain.Entities.Department> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    public class GetByIdDepartmentQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.Department>, Domain.Entities.Department?>
    {
        private readonly IDatabaseService<Domain.Entities.Department> _db;
        public GetByIdDepartmentQueryHandler(IDatabaseService<Domain.Entities.Department> db) { _db = db; }
        public async Task<Domain.Entities.Department?> Handle(GetByIdQuery<Domain.Entities.Department> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    // Concrete handlers for Payment
    public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Payment>, IReadOnlyList<Domain.Entities.Payment>>
    {
        private readonly IDatabaseService<Domain.Entities.Payment> _db;
        public GetAllPaymentsQueryHandler(IDatabaseService<Domain.Entities.Payment> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Payment>> Handle(GetAllQuery<Domain.Entities.Payment> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class CreatePaymentCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Payment>, Domain.Entities.Payment>
    {
        private readonly IDatabaseService<Domain.Entities.Payment> _db;
        public CreatePaymentCommandHandler(IDatabaseService<Domain.Entities.Payment> db) { _db = db; }
        public async Task<Domain.Entities.Payment> Handle(CreateCommand<Domain.Entities.Payment> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    // Concrete handlers for AuditLog
    public class GetByIdAuditLogQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.AuditLog>, Domain.Entities.AuditLog?>
    {
        private readonly IDatabaseService<Domain.Entities.AuditLog> _db;
        public GetByIdAuditLogQueryHandler(IDatabaseService<Domain.Entities.AuditLog> db) { _db = db; }
        public async Task<Domain.Entities.AuditLog?> Handle(GetByIdQuery<Domain.Entities.AuditLog> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    // Concrete handlers for ErrorLog
    public class GetByIdErrorLogQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.ErrorLog>, Domain.Entities.ErrorLog?>
    {
        private readonly IDatabaseService<Domain.Entities.ErrorLog> _db;
        public GetByIdErrorLogQueryHandler(IDatabaseService<Domain.Entities.ErrorLog> db) { _db = db; }
        public async Task<Domain.Entities.ErrorLog?> Handle(GetByIdQuery<Domain.Entities.ErrorLog> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class DeleteErrorLogCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.ErrorLog>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.ErrorLog> _db;
        public DeleteErrorLogCommandHandler(IDatabaseService<Domain.Entities.ErrorLog> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.ErrorLog> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    // Concrete handlers for Reservation
    public class GetAllReservationsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Reservation>, IReadOnlyList<Domain.Entities.Reservation>>
    {
        private readonly IDatabaseService<Domain.Entities.Reservation> _db;
        public GetAllReservationsQueryHandler(IDatabaseService<Domain.Entities.Reservation> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Reservation>> Handle(GetAllQuery<Domain.Entities.Reservation> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class CreateReservationCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Reservation>, Domain.Entities.Reservation>
    {
        private readonly IDatabaseService<Domain.Entities.Reservation> _db;
        public CreateReservationCommandHandler(IDatabaseService<Domain.Entities.Reservation> db) { _db = db; }
        public async Task<Domain.Entities.Reservation> Handle(CreateCommand<Domain.Entities.Reservation> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            return request.Entity;
        }
    }

    // Concrete handlers for AgencyContract
    public class GetAllAgencyContractsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.AgencyContract>, IReadOnlyList<Domain.Entities.AgencyContract>>
    {
        private readonly IDatabaseService<Domain.Entities.AgencyContract> _db;
        public GetAllAgencyContractsQueryHandler(IDatabaseService<Domain.Entities.AgencyContract> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.AgencyContract>> Handle(GetAllQuery<Domain.Entities.AgencyContract> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class CreateAgencyContractCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.AgencyContract>, Domain.Entities.AgencyContract>
    {
        private readonly IDatabaseService<Domain.Entities.AgencyContract> _db;
        public CreateAgencyContractCommandHandler(IDatabaseService<Domain.Entities.AgencyContract> db) { _db = db; }
        public async Task<Domain.Entities.AgencyContract> Handle(CreateCommand<Domain.Entities.AgencyContract> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    // Concrete handlers for MedicalService
    public class GetAllMedicalServicesQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.MedicalService>, IReadOnlyList<Domain.Entities.MedicalService>>
    {
        private readonly IDatabaseService<Domain.Entities.MedicalService> _db;
        public GetAllMedicalServicesQueryHandler(IDatabaseService<Domain.Entities.MedicalService> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.MedicalService>> Handle(GetAllQuery<Domain.Entities.MedicalService> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class CreateMedicalServiceCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.MedicalService>, Domain.Entities.MedicalService>
    {
        private readonly IDatabaseService<Domain.Entities.MedicalService> _db;
        public CreateMedicalServiceCommandHandler(IDatabaseService<Domain.Entities.MedicalService> db) { _db = db; }
        public async Task<Domain.Entities.MedicalService> Handle(CreateCommand<Domain.Entities.MedicalService> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    // Concrete handlers for PriceListItem
    public class GetAllPriceListItemsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.PriceListItem>, IReadOnlyList<Domain.Entities.PriceListItem>>
    {
        private readonly IDatabaseService<Domain.Entities.PriceListItem> _db;
        public GetAllPriceListItemsQueryHandler(IDatabaseService<Domain.Entities.PriceListItem> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.PriceListItem>> Handle(GetAllQuery<Domain.Entities.PriceListItem> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class CreatePriceListItemCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.PriceListItem>, Domain.Entities.PriceListItem>
    {
        private readonly IDatabaseService<Domain.Entities.PriceListItem> _db;
        public CreatePriceListItemCommandHandler(IDatabaseService<Domain.Entities.PriceListItem> db) { _db = db; }
        public async Task<Domain.Entities.PriceListItem> Handle(CreateCommand<Domain.Entities.PriceListItem> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    // Concrete handlers for Equipment
    public class GetAllEquipmentQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Equipment>, IReadOnlyList<Domain.Entities.Equipment>>
    {
        private readonly IDatabaseService<Domain.Entities.Equipment> _db;
        public GetAllEquipmentQueryHandler(IDatabaseService<Domain.Entities.Equipment> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Equipment>> Handle(GetAllQuery<Domain.Entities.Equipment> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdEquipmentQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.Equipment>, Domain.Entities.Equipment?>
    {
        private readonly IDatabaseService<Domain.Entities.Equipment> _db;
        public GetByIdEquipmentQueryHandler(IDatabaseService<Domain.Entities.Equipment> db) { _db = db; }
        public async Task<Domain.Entities.Equipment?> Handle(GetByIdQuery<Domain.Entities.Equipment> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateEquipmentCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Equipment>, Domain.Entities.Equipment>
    {
        private readonly IDatabaseService<Domain.Entities.Equipment> _db;
        public CreateEquipmentCommandHandler(IDatabaseService<Domain.Entities.Equipment> db) { _db = db; }
        public async Task<Domain.Entities.Equipment> Handle(CreateCommand<Domain.Entities.Equipment> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.Equipment>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Equipment> _db;
        public UpdateEquipmentCommandHandler(IDatabaseService<Domain.Entities.Equipment> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.Equipment> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.Equipment>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Equipment> _db;
        public DeleteEquipmentCommandHandler(IDatabaseService<Domain.Entities.Equipment> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.Equipment> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    // Concrete handlers for Appointment
    public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Appointment>, IReadOnlyList<Domain.Entities.Appointment>>
    {
        private readonly IDatabaseService<Domain.Entities.Appointment> _db;
        public GetAllAppointmentsQueryHandler(IDatabaseService<Domain.Entities.Appointment> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Appointment>> Handle(GetAllQuery<Domain.Entities.Appointment> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdAppointmentQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.Appointment>, Domain.Entities.Appointment?>
    {
        private readonly IDatabaseService<Domain.Entities.Appointment> _db;
        public GetByIdAppointmentQueryHandler(IDatabaseService<Domain.Entities.Appointment> db) { _db = db; }
        public async Task<Domain.Entities.Appointment?> Handle(GetByIdQuery<Domain.Entities.Appointment> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateAppointmentCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Appointment>, Domain.Entities.Appointment>
    {
        private readonly IDatabaseService<Domain.Entities.Appointment> _db;
        public CreateAppointmentCommandHandler(IDatabaseService<Domain.Entities.Appointment> db) { _db = db; }
        public async Task<Domain.Entities.Appointment> Handle(CreateCommand<Domain.Entities.Appointment> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.Appointment>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Appointment> _db;
        public UpdateAppointmentCommandHandler(IDatabaseService<Domain.Entities.Appointment> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.Appointment> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.Appointment>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Appointment> _db;
        public DeleteAppointmentCommandHandler(IDatabaseService<Domain.Entities.Appointment> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.Appointment> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    // Concrete handlers for User
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.User>, IReadOnlyList<Domain.Entities.User>>
    {
        private readonly IDatabaseService<Domain.Entities.User> _db;
        public GetAllUsersQueryHandler(IDatabaseService<Domain.Entities.User> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.User>> Handle(GetAllQuery<Domain.Entities.User> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdUserQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.User>, Domain.Entities.User?>
    {
        private readonly IDatabaseService<Domain.Entities.User> _db;
        public GetByIdUserQueryHandler(IDatabaseService<Domain.Entities.User> db) { _db = db; }
        public async Task<Domain.Entities.User?> Handle(GetByIdQuery<Domain.Entities.User> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.User>, Domain.Entities.User>
    {
        private readonly IDatabaseService<Domain.Entities.User> _db;
        public CreateUserCommandHandler(IDatabaseService<Domain.Entities.User> db) { _db = db; }
        public async Task<Domain.Entities.User> Handle(CreateCommand<Domain.Entities.User> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.User>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.User> _db;
        public UpdateUserCommandHandler(IDatabaseService<Domain.Entities.User> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.User> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.User>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.User> _db;
        public DeleteUserCommandHandler(IDatabaseService<Domain.Entities.User> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.User> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    // Concrete handlers for Discount
    public class GetAllDiscountsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.Discount>, IReadOnlyList<Domain.Entities.Discount>>
    {
        private readonly IDatabaseService<Domain.Entities.Discount> _db;
        public GetAllDiscountsQueryHandler(IDatabaseService<Domain.Entities.Discount> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.Discount>> Handle(GetAllQuery<Domain.Entities.Discount> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdDiscountQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.Discount>, Domain.Entities.Discount?>
    {
        private readonly IDatabaseService<Domain.Entities.Discount> _db;
        public GetByIdDiscountQueryHandler(IDatabaseService<Domain.Entities.Discount> db) { _db = db; }
        public async Task<Domain.Entities.Discount?> Handle(GetByIdQuery<Domain.Entities.Discount> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateDiscountCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.Discount>, Domain.Entities.Discount>
    {
        private readonly IDatabaseService<Domain.Entities.Discount> _db;
        public CreateDiscountCommandHandler(IDatabaseService<Domain.Entities.Discount> db) { _db = db; }
        public async Task<Domain.Entities.Discount> Handle(CreateCommand<Domain.Entities.Discount> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    public class UpdateDiscountCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.Discount>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Discount> _db;
        public UpdateDiscountCommandHandler(IDatabaseService<Domain.Entities.Discount> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.Discount> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    public class DeleteDiscountCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.Discount>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.Discount> _db;
        public DeleteDiscountCommandHandler(IDatabaseService<Domain.Entities.Discount> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.Discount> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    // Concrete handlers for DiscountRequest
    public class GetAllDiscountRequestsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.DiscountRequest>, IReadOnlyList<Domain.Entities.DiscountRequest>>
    {
        private readonly IDatabaseService<Domain.Entities.DiscountRequest> _db;
        public GetAllDiscountRequestsQueryHandler(IDatabaseService<Domain.Entities.DiscountRequest> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.DiscountRequest>> Handle(GetAllQuery<Domain.Entities.DiscountRequest> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdDiscountRequestQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.DiscountRequest>, Domain.Entities.DiscountRequest?>
    {
        private readonly IDatabaseService<Domain.Entities.DiscountRequest> _db;
        public GetByIdDiscountRequestQueryHandler(IDatabaseService<Domain.Entities.DiscountRequest> db) { _db = db; }
        public async Task<Domain.Entities.DiscountRequest?> Handle(GetByIdQuery<Domain.Entities.DiscountRequest> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateDiscountRequestCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.DiscountRequest>, Domain.Entities.DiscountRequest>
    {
        private readonly IDatabaseService<Domain.Entities.DiscountRequest> _db;
        public CreateDiscountRequestCommandHandler(IDatabaseService<Domain.Entities.DiscountRequest> db) { _db = db; }
        public async Task<Domain.Entities.DiscountRequest> Handle(CreateCommand<Domain.Entities.DiscountRequest> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    public class UpdateDiscountRequestCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.DiscountRequest>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.DiscountRequest> _db;
        public UpdateDiscountRequestCommandHandler(IDatabaseService<Domain.Entities.DiscountRequest> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.DiscountRequest> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    public class DeleteDiscountRequestCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.DiscountRequest>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.DiscountRequest> _db;
        public DeleteDiscountRequestCommandHandler(IDatabaseService<Domain.Entities.DiscountRequest> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.DiscountRequest> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    // Concrete handlers for EquipmentStatusLog
    public class GetAllEquipmentStatusLogsQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.EquipmentStatusLog>, IReadOnlyList<Domain.Entities.EquipmentStatusLog>>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentStatusLog> _db;
        public GetAllEquipmentStatusLogsQueryHandler(IDatabaseService<Domain.Entities.EquipmentStatusLog> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.EquipmentStatusLog>> Handle(GetAllQuery<Domain.Entities.EquipmentStatusLog> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdEquipmentStatusLogQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.EquipmentStatusLog>, Domain.Entities.EquipmentStatusLog?>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentStatusLog> _db;
        public GetByIdEquipmentStatusLogQueryHandler(IDatabaseService<Domain.Entities.EquipmentStatusLog> db) { _db = db; }
        public async Task<Domain.Entities.EquipmentStatusLog?> Handle(GetByIdQuery<Domain.Entities.EquipmentStatusLog> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateEquipmentStatusLogCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.EquipmentStatusLog>, Domain.Entities.EquipmentStatusLog>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentStatusLog> _db;
        public CreateEquipmentStatusLogCommandHandler(IDatabaseService<Domain.Entities.EquipmentStatusLog> db) { _db = db; }
        public async Task<Domain.Entities.EquipmentStatusLog> Handle(CreateCommand<Domain.Entities.EquipmentStatusLog> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return request.Entity;
        }
    }

    public class UpdateEquipmentStatusLogCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.EquipmentStatusLog>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentStatusLog> _db;
        public UpdateEquipmentStatusLogCommandHandler(IDatabaseService<Domain.Entities.EquipmentStatusLog> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.EquipmentStatusLog> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    public class DeleteEquipmentStatusLogCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.EquipmentStatusLog>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentStatusLog> _db;
        public DeleteEquipmentStatusLogCommandHandler(IDatabaseService<Domain.Entities.EquipmentStatusLog> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.EquipmentStatusLog> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            // Save handled by transactional pipeline
            return Unit.Value;
        }
    }

    // Concrete handlers for EquipmentServiceOrder
    public class GetAllEquipmentServiceOrdersQueryHandler : IRequestHandler<GetAllQuery<Domain.Entities.EquipmentServiceOrder>, IReadOnlyList<Domain.Entities.EquipmentServiceOrder>>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentServiceOrder> _db;
        public GetAllEquipmentServiceOrdersQueryHandler(IDatabaseService<Domain.Entities.EquipmentServiceOrder> db) { _db = db; }
        public async Task<IReadOnlyList<Domain.Entities.EquipmentServiceOrder>> Handle(GetAllQuery<Domain.Entities.EquipmentServiceOrder> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdEquipmentServiceOrderQueryHandler : IRequestHandler<GetByIdQuery<Domain.Entities.EquipmentServiceOrder>, Domain.Entities.EquipmentServiceOrder?>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentServiceOrder> _db;
        public GetByIdEquipmentServiceOrderQueryHandler(IDatabaseService<Domain.Entities.EquipmentServiceOrder> db) { _db = db; }
        public async Task<Domain.Entities.EquipmentServiceOrder?> Handle(GetByIdQuery<Domain.Entities.EquipmentServiceOrder> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateEquipmentServiceOrderCommandHandler : IRequestHandler<CreateCommand<Domain.Entities.EquipmentServiceOrder>, Domain.Entities.EquipmentServiceOrder>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentServiceOrder> _db;
        public CreateEquipmentServiceOrderCommandHandler(IDatabaseService<Domain.Entities.EquipmentServiceOrder> db) { _db = db; }
        public async Task<Domain.Entities.EquipmentServiceOrder> Handle(CreateCommand<Domain.Entities.EquipmentServiceOrder> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            return request.Entity;
        }
    }

    public class UpdateEquipmentServiceOrderCommandHandler : IRequestHandler<UpdateCommand<Domain.Entities.EquipmentServiceOrder>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentServiceOrder> _db;
        public UpdateEquipmentServiceOrderCommandHandler(IDatabaseService<Domain.Entities.EquipmentServiceOrder> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<Domain.Entities.EquipmentServiceOrder> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            return Unit.Value;
        }
    }

    public class DeleteEquipmentServiceOrderCommandHandler : IRequestHandler<DeleteCommand<Domain.Entities.EquipmentServiceOrder>, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.EquipmentServiceOrder> _db;
        public DeleteEquipmentServiceOrderCommandHandler(IDatabaseService<Domain.Entities.EquipmentServiceOrder> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<Domain.Entities.EquipmentServiceOrder> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            return Unit.Value;
        }
    }
}