using System;
using System.Threading;
using System.Threading.Tasks;

namespace drTech_backend.Infrastructure.Abstractions
{
	public interface IUnitOfWork
	{
		IGenericRepository<Domain.Entities.Hospital> Hospitals { get; }
		IGenericRepository<Domain.Entities.Department> Departments { get; }
		IGenericRepository<Domain.Entities.Doctor> Doctors { get; }
		IGenericRepository<Domain.Entities.Patient> Patients { get; }
		IGenericRepository<Domain.Entities.Equipment> Equipment { get; }
		IGenericRepository<Domain.Entities.Appointment> Appointments { get; }
		IGenericRepository<Domain.Entities.Reservation> Reservations { get; }
		IGenericRepository<Domain.Entities.InsuranceAgency> InsuranceAgencies { get; }
		IGenericRepository<Domain.Entities.AgencyContract> AgencyContracts { get; }
		IGenericRepository<Domain.Entities.MedicalService> MedicalServices { get; }
		IGenericRepository<Domain.Entities.PriceListItem> PriceListItems { get; }
		IGenericRepository<Domain.Entities.Payment> Payments { get; }
		IGenericRepository<Domain.Entities.User> Users { get; }
		IGenericRepository<Domain.Entities.RefreshToken> RefreshTokens { get; }
		IGenericRepository<Domain.Entities.AuditLog> AuditLogs { get; }
		IGenericRepository<Domain.Entities.RequestLog> RequestLogs { get; }
		IGenericRepository<Domain.Entities.ErrorLog> ErrorLogs { get; }
		IGenericRepository<Domain.Entities.ThrottleLog> ThrottleLogs { get; }
		IGenericRepository<Domain.Entities.Discount> Discounts { get; }
		IGenericRepository<Domain.Entities.DiscountRequest> DiscountRequests { get; }
		IGenericRepository<Domain.Entities.EquipmentStatusLog> EquipmentStatusLogs { get; }
		IGenericRepository<Domain.Entities.EquipmentServiceOrder> EquipmentServiceOrders { get; }

		IGenericRepository<T> GetRepository<T>() where T : class;
		Task<int> SaveChangesAsync(CancellationToken ct = default);
		Task BeginTransactionAsync(CancellationToken ct = default);
		Task CommitTransactionAsync(CancellationToken ct = default);
		Task RollbackTransactionAsync(CancellationToken ct = default);
	}

	// Provider-aware Unit of Work that composes existing generic repositories without altering current behavior
	public sealed class UnitOfWork : IUnitOfWork
	{
		private readonly IGenericUnitOfWork _uow;

		public UnitOfWork(DatabaseProvider provider, IServiceProvider services)
		{
			Hospitals = DatabaseFactory.CreateRepository<Domain.Entities.Hospital>(provider, services);
			Departments = DatabaseFactory.CreateRepository<Domain.Entities.Department>(provider, services);
			Doctors = DatabaseFactory.CreateRepository<Domain.Entities.Doctor>(provider, services);
			Patients = DatabaseFactory.CreateRepository<Domain.Entities.Patient>(provider, services);
			Equipment = DatabaseFactory.CreateRepository<Domain.Entities.Equipment>(provider, services);
			Appointments = DatabaseFactory.CreateRepository<Domain.Entities.Appointment>(provider, services);
			Reservations = DatabaseFactory.CreateRepository<Domain.Entities.Reservation>(provider, services);
			InsuranceAgencies = DatabaseFactory.CreateRepository<Domain.Entities.InsuranceAgency>(provider, services);
			AgencyContracts = DatabaseFactory.CreateRepository<Domain.Entities.AgencyContract>(provider, services);
			MedicalServices = DatabaseFactory.CreateRepository<Domain.Entities.MedicalService>(provider, services);
			PriceListItems = DatabaseFactory.CreateRepository<Domain.Entities.PriceListItem>(provider, services);
			Payments = DatabaseFactory.CreateRepository<Domain.Entities.Payment>(provider, services);
			Users = DatabaseFactory.CreateRepository<Domain.Entities.User>(provider, services);
			RefreshTokens = DatabaseFactory.CreateRepository<Domain.Entities.RefreshToken>(provider, services);
			AuditLogs = DatabaseFactory.CreateRepository<Domain.Entities.AuditLog>(provider, services);
			RequestLogs = DatabaseFactory.CreateRepository<Domain.Entities.RequestLog>(provider, services);
			ErrorLogs = DatabaseFactory.CreateRepository<Domain.Entities.ErrorLog>(provider, services);
			ThrottleLogs = DatabaseFactory.CreateRepository<Domain.Entities.ThrottleLog>(provider, services);
			Discounts = DatabaseFactory.CreateRepository<Domain.Entities.Discount>(provider, services);
			DiscountRequests = DatabaseFactory.CreateRepository<Domain.Entities.DiscountRequest>(provider, services);
			EquipmentStatusLogs = DatabaseFactory.CreateRepository<Domain.Entities.EquipmentStatusLog>(provider, services);
			EquipmentServiceOrders = DatabaseFactory.CreateRepository<Domain.Entities.EquipmentServiceOrder>(provider, services);

			_uow = DatabaseFactory.CreateUnitOfWork(provider, services);
		}

		public IGenericRepository<Domain.Entities.Hospital> Hospitals { get; }
		public IGenericRepository<Domain.Entities.Department> Departments { get; }
		public IGenericRepository<Domain.Entities.Doctor> Doctors { get; }
		public IGenericRepository<Domain.Entities.Patient> Patients { get; }
		public IGenericRepository<Domain.Entities.Equipment> Equipment { get; }
		public IGenericRepository<Domain.Entities.Appointment> Appointments { get; }
		public IGenericRepository<Domain.Entities.Reservation> Reservations { get; }
		public IGenericRepository<Domain.Entities.InsuranceAgency> InsuranceAgencies { get; }
		public IGenericRepository<Domain.Entities.AgencyContract> AgencyContracts { get; }
		public IGenericRepository<Domain.Entities.MedicalService> MedicalServices { get; }
		public IGenericRepository<Domain.Entities.PriceListItem> PriceListItems { get; }
		public IGenericRepository<Domain.Entities.Payment> Payments { get; }
		public IGenericRepository<Domain.Entities.User> Users { get; }
		public IGenericRepository<Domain.Entities.RefreshToken> RefreshTokens { get; }
		public IGenericRepository<Domain.Entities.AuditLog> AuditLogs { get; }
		public IGenericRepository<Domain.Entities.RequestLog> RequestLogs { get; }
		public IGenericRepository<Domain.Entities.ErrorLog> ErrorLogs { get; }
		public IGenericRepository<Domain.Entities.ThrottleLog> ThrottleLogs { get; }
		public IGenericRepository<Domain.Entities.Discount> Discounts { get; }
		public IGenericRepository<Domain.Entities.DiscountRequest> DiscountRequests { get; }
		public IGenericRepository<Domain.Entities.EquipmentStatusLog> EquipmentStatusLogs { get; }
		public IGenericRepository<Domain.Entities.EquipmentServiceOrder> EquipmentServiceOrders { get; }

		public IGenericRepository<T> GetRepository<T>() where T : class
		{
			if (typeof(T) == typeof(Domain.Entities.Hospital)) return (IGenericRepository<T>)Hospitals;
			if (typeof(T) == typeof(Domain.Entities.Department)) return (IGenericRepository<T>)Departments;
			if (typeof(T) == typeof(Domain.Entities.Doctor)) return (IGenericRepository<T>)Doctors;
			if (typeof(T) == typeof(Domain.Entities.Patient)) return (IGenericRepository<T>)Patients;
			if (typeof(T) == typeof(Domain.Entities.Equipment)) return (IGenericRepository<T>)Equipment;
			if (typeof(T) == typeof(Domain.Entities.Appointment)) return (IGenericRepository<T>)Appointments;
			if (typeof(T) == typeof(Domain.Entities.Reservation)) return (IGenericRepository<T>)Reservations;
			if (typeof(T) == typeof(Domain.Entities.InsuranceAgency)) return (IGenericRepository<T>)InsuranceAgencies;
			if (typeof(T) == typeof(Domain.Entities.AgencyContract)) return (IGenericRepository<T>)AgencyContracts;
			if (typeof(T) == typeof(Domain.Entities.MedicalService)) return (IGenericRepository<T>)MedicalServices;
			if (typeof(T) == typeof(Domain.Entities.PriceListItem)) return (IGenericRepository<T>)PriceListItems;
			if (typeof(T) == typeof(Domain.Entities.Payment)) return (IGenericRepository<T>)Payments;
			if (typeof(T) == typeof(Domain.Entities.User)) return (IGenericRepository<T>)Users;
			if (typeof(T) == typeof(Domain.Entities.RefreshToken)) return (IGenericRepository<T>)RefreshTokens;
			if (typeof(T) == typeof(Domain.Entities.AuditLog)) return (IGenericRepository<T>)AuditLogs;
			if (typeof(T) == typeof(Domain.Entities.RequestLog)) return (IGenericRepository<T>)RequestLogs;
			if (typeof(T) == typeof(Domain.Entities.ErrorLog)) return (IGenericRepository<T>)ErrorLogs;
			if (typeof(T) == typeof(Domain.Entities.ThrottleLog)) return (IGenericRepository<T>)ThrottleLogs;
			if (typeof(T) == typeof(Domain.Entities.Discount)) return (IGenericRepository<T>)Discounts;
			if (typeof(T) == typeof(Domain.Entities.DiscountRequest)) return (IGenericRepository<T>)DiscountRequests;
			if (typeof(T) == typeof(Domain.Entities.EquipmentStatusLog)) return (IGenericRepository<T>)EquipmentStatusLogs;
			if (typeof(T) == typeof(Domain.Entities.EquipmentServiceOrder)) return (IGenericRepository<T>)EquipmentServiceOrders;

			throw new ArgumentException($"Repository not available for type {typeof(T).FullName}");
		}

		public async Task<int> SaveChangesAsync(CancellationToken ct = default)
		{
			return await _uow.SaveChangesAsync(ct);
		}

		public async Task BeginTransactionAsync(CancellationToken ct = default)
		{
			await _uow.BeginTransactionAsync(ct);
		}

		public async Task CommitTransactionAsync(CancellationToken ct = default)
		{
			await _uow.CommitTransactionAsync(ct);
		}

		public async Task RollbackTransactionAsync(CancellationToken ct = default)
		{
			await _uow.RollbackTransactionAsync(ct);
		}
	}
}


