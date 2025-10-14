using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using drTech_backend.Infrastructure.Auth;

namespace drTech_backend.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                var pg = configuration.GetConnectionString("Postgres");
                if (string.IsNullOrWhiteSpace(pg))
                {
                    var envPg = Environment.GetEnvironmentVariable("DRTECH_PG");
                    if (!string.IsNullOrWhiteSpace(envPg)) pg = envPg;
                }
                if (!string.IsNullOrWhiteSpace(pg))
                {
                    options.UseNpgsql(pg);
                }
            });

            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Register concrete MediatR handlers
            services.AddScoped<Application.Common.Mediator.GetAllInsuranceAgenciesQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdInsuranceAgencyQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateInsuranceAgencyCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateInsuranceAgencyCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteInsuranceAgencyCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllHospitalsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdHospitalQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateHospitalCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateHospitalCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteHospitalCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllPatientsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdPatientQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreatePatientCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdatePatientCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeletePatientCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllDoctorsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateDoctorCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllDepartmentsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateDepartmentCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllPaymentsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreatePaymentCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllReservationsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateReservationCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllAgencyContractsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateAgencyContractCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllMedicalServicesQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateMedicalServiceCommandHandler>();

            services.AddScoped<Application.Common.Mediator.GetAllPriceListItemsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreatePriceListItemCommandHandler>();

            // Equipment handlers
            services.AddScoped<Application.Common.Mediator.GetAllEquipmentQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdEquipmentQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateEquipmentCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateEquipmentCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteEquipmentCommandHandler>();

            // Appointment handlers
            services.AddScoped<Application.Common.Mediator.GetAllAppointmentsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdAppointmentQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateAppointmentCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateAppointmentCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteAppointmentCommandHandler>();

            // User handlers
            services.AddScoped<Application.Common.Mediator.GetAllUsersQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdUserQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateUserCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateUserCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteUserCommandHandler>();

            // Auth handlers
            services.AddScoped<Application.Common.Mediator.RegisterCommandHandler>();
            services.AddScoped<Application.Common.Mediator.LoginCommandHandler>();
            services.AddScoped<Application.Common.Mediator.RefreshTokenCommandHandler>();
            services.AddScoped<Application.Common.Mediator.GoogleLoginCommandHandler>();
            services.AddScoped<Application.Common.Mediator.GetUserProfileQueryHandler>();

            // Discount handlers
            services.AddScoped<Application.Common.Mediator.GetAllDiscountsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdDiscountQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateDiscountCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateDiscountCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteDiscountCommandHandler>();

            // DiscountRequest handlers
            services.AddScoped<Application.Common.Mediator.GetAllDiscountRequestsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdDiscountRequestQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateDiscountRequestCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateDiscountRequestCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteDiscountRequestCommandHandler>();

            // EquipmentStatusLog handlers
            services.AddScoped<Application.Common.Mediator.GetAllEquipmentStatusLogsQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdEquipmentStatusLogQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateEquipmentStatusLogCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateEquipmentStatusLogCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteEquipmentStatusLogCommandHandler>();

            // EquipmentServiceOrder handlers
            services.AddScoped<Application.Common.Mediator.GetAllEquipmentServiceOrdersQueryHandler>();
            services.AddScoped<Application.Common.Mediator.GetByIdEquipmentServiceOrderQueryHandler>();
            services.AddScoped<Application.Common.Mediator.CreateEquipmentServiceOrderCommandHandler>();
            services.AddScoped<Application.Common.Mediator.UpdateEquipmentServiceOrderCommandHandler>();
            services.AddScoped<Application.Common.Mediator.DeleteEquipmentServiceOrderCommandHandler>();

            // Database abstraction layer
            var dbProvider = Enum.Parse<Abstractions.DatabaseProvider>(configuration["DatabaseProvider"] ?? "PostgreSQL");
            services.AddSingleton(typeof(Abstractions.DatabaseProvider), dbProvider);
            
            // Register generic database service for each entity
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Hospital>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Hospital>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Department>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Department>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Doctor>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Doctor>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Patient>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Patient>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Equipment>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Equipment>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Appointment>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Appointment>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Reservation>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Reservation>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.InsuranceAgency>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.InsuranceAgency>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.AgencyContract>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.AgencyContract>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.MedicalService>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.MedicalService>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.PriceListItem>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.PriceListItem>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Payment>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Payment>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.User>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.User>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.RefreshToken>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.RefreshToken>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.AuditLog>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.AuditLog>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.RequestLog>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.RequestLog>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.ErrorLog>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.ErrorLog>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.ThrottleLog>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.ThrottleLog>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.Discount>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.Discount>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.DiscountRequest>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.DiscountRequest>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.EquipmentStatusLog>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.EquipmentStatusLog>(dbProvider, provider));
            services.AddScoped<Abstractions.IDatabaseService<Domain.Entities.EquipmentServiceOrder>>(provider => 
                new Abstractions.DatabaseService<Domain.Entities.EquipmentServiceOrder>(dbProvider, provider));

            services.Configure<Middleware.ThrottlingOptions>(configuration.GetSection("Throttling"));

            // Auth services
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Mongo
            services.Configure<Mongo.MongoSettings>(configuration.GetSection("Mongo"));
            services.AddSingleton<Mongo.IMongoContext, Mongo.MongoContext>();
            
            // Register generic MongoDB repository
            services.AddScoped(typeof(Mongo.IMongoRepository<>), typeof(Mongo.MongoRepository<>));

            // Mongo repositories
            services.AddScoped<Mongo.IMongoHospitalRepository, Mongo.MongoHospitalRepository>();
            services.AddScoped<Mongo.IMongoPatientRepository, Mongo.MongoPatientRepository>();
            services.AddScoped<Mongo.IMongoDoctorRepository, Mongo.MongoDoctorRepository>();
            services.AddScoped<Mongo.IMongoDepartmentRepository, Mongo.MongoDepartmentRepository>();
            services.AddScoped<Mongo.IMongoEquipmentRepository, Mongo.MongoEquipmentRepository>();
            services.AddScoped<Mongo.IMongoReservationRepository, Mongo.MongoReservationRepository>();
            services.AddScoped<Mongo.IMongoAgencyRepository, Mongo.MongoAgencyRepository>();
            services.AddScoped<Mongo.IMongoContractRepository, Mongo.MongoContractRepository>();
            services.AddScoped<Mongo.IMongoServiceRepository, Mongo.MongoServiceRepository>();
            services.AddScoped<Mongo.IMongoPriceListRepository, Mongo.MongoPriceListRepository>();
            services.AddScoped<Mongo.IMongoPaymentRepository, Mongo.MongoPaymentRepository>();
            services.AddScoped<Mongo.IMongoUserRepository, Mongo.MongoUserRepository>();

            // Neo4j
            services.Configure<Neo4j.Neo4jSettings>(configuration.GetSection("Neo4j"));
            services.AddSingleton<Neo4j.INeo4jContext, Neo4j.Neo4jContext>();
            services.AddScoped<Neo4j.INeo4jRepository, Neo4j.Neo4jRepository>();

            return services;
        }


    }
}


