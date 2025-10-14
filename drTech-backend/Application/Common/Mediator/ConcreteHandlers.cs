using MediatR;
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
            return request.Entity;
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
            await _db.SaveChangesAsync(cancellationToken);
            return request.Entity;
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
            await _db.SaveChangesAsync(cancellationToken);
            return request.Entity;
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
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
            await _db.SaveChangesAsync(cancellationToken);
            return request.Entity;
        }
    }
}
