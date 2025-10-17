using Neo4j.Driver;
using System.Text.Json;

namespace drTech_backend.Infrastructure.Neo4j
{
    public interface INeo4jRepository
    {
        Task<T?> GetByIdAsync<T>(Guid id, string label, CancellationToken ct = default) where T : class;
        Task AddAsync<T>(T entity, string label, CancellationToken ct = default) where T : class;
        Task UpdateAsync<T>(T entity, string label, CancellationToken ct = default) where T : class;
        Task DeleteAsync(Guid id, string label, CancellationToken ct = default);
        Task<List<T>> GetAllAsync<T>(string label, CancellationToken ct = default) where T : class;
        Task CreateRelationshipAsync(string fromLabel, Guid fromId, string relationship, string toLabel, Guid toId, Dictionary<string, object>? properties = null, CancellationToken ct = default);
        Task<List<T>> GetRelatedAsync<T>(Guid id, string label, string relationship, string targetLabel, CancellationToken ct = default) where T : class;
    }

    public class Neo4jRepository : INeo4jRepository
    {
        private readonly INeo4jContext _context;

        public Neo4jRepository(INeo4jContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync<T>(Guid id, string label, CancellationToken ct = default) where T : class
        {
            var query = $"MATCH (n:{label}) WHERE n.id = $id RETURN n";
            var result = await _context.Session.RunAsync(query, new { id = id.ToString() });
            var record = await result.SingleAsync();
            var properties = record["n"].As<INode>().Properties;
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(properties));
        }

        public async Task AddAsync<T>(T entity, string label, CancellationToken ct = default) where T : class
        {
            var properties = ConvertToDictionary(entity);
            var query = $"CREATE (n:{label} $props)";
            
            // Debug: Log the properties being sent to Neo4j
            Console.WriteLine($"Neo4j AddAsync - Label: {label}");
            Console.WriteLine($"Neo4j AddAsync - Properties: {System.Text.Json.JsonSerializer.Serialize(properties)}");
            
            await _context.Session.RunAsync(query, new { props = properties });
        }

        public async Task UpdateAsync<T>(T entity, string label, CancellationToken ct = default) where T : class
        {
            var properties = ConvertToDictionary(entity);
            var query = $"MATCH (n:{label}) WHERE n.id = $id SET n += $props";
            await _context.Session.RunAsync(query, new { id = properties["id"], props = properties });
        }

        public async Task DeleteAsync(Guid id, string label, CancellationToken ct = default)
        {
            var query = $"MATCH (n:{label}) WHERE n.id = $id DETACH DELETE n";
            await _context.Session.RunAsync(query, new { id = id.ToString() });
        }

        public async Task<List<T>> GetAllAsync<T>(string label, CancellationToken ct = default) where T : class
        {
            var query = $"MATCH (n:{label}) RETURN n";
            var result = await _context.Session.RunAsync(query);
            var records = await result.ToListAsync();
            return records.Select(r => 
            {
                var properties = r["n"].As<INode>().Properties;
                return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(properties))!;
            }).ToList();
        }

        public async Task CreateRelationshipAsync(string fromLabel, Guid fromId, string relationship, string toLabel, Guid toId, Dictionary<string, object>? properties = null, CancellationToken ct = default)
        {
            var query = $"MATCH (a:{fromLabel}), (b:{toLabel}) WHERE a.id = $fromId AND b.id = $toId CREATE (a)-[r:{relationship}]->(b)";
            if (properties != null)
            {
                query += " SET r += $props";
            }
            await _context.Session.RunAsync(query, new { fromId = fromId.ToString(), toId = toId.ToString(), props = properties });
        }

        public async Task<List<T>> GetRelatedAsync<T>(Guid id, string label, string relationship, string targetLabel, CancellationToken ct = default) where T : class
        {
            var query = $"MATCH (n:{label})-[:{relationship}]->(target:{targetLabel}) WHERE n.id = $id RETURN target";
            var result = await _context.Session.RunAsync(query, new { id = id.ToString() });
            var records = await result.ToListAsync();
            return records.Select(r => 
            {
                var properties = r["target"].As<INode>().Properties;
                return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(properties))!;
            }).ToList();
        }

        private static Dictionary<string, object> ConvertToDictionary<T>(T entity) where T : class
        {
            var dict = new Dictionary<string, object>();
            foreach (var prop in typeof(T).GetProperties())
            {
                var value = prop.GetValue(entity);
                if (value != null)
                {
                    // Skip collections and complex objects - Neo4j can't store them as properties
                    if (IsCollection(prop.PropertyType) || IsComplexObject(prop.PropertyType))
                    {
                        continue; // Skip this property - relationships will be handled separately
                    }
                    
                    dict[prop.Name] = ConvertValue(value);
                }
            }
            return dict;
        }

        private static bool IsCollection(Type type)
        {
            return type != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
        }

        private static bool IsComplexObject(Type type)
        {
            return type.IsClass && type != typeof(string) && type != typeof(DateTime) && type != typeof(Guid) && 
                   !type.IsPrimitive && !type.IsEnum && !IsNullablePrimitive(type);
        }

        private static bool IsNullablePrimitive(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null && (underlyingType.IsPrimitive || underlyingType == typeof(DateTime) || underlyingType == typeof(Guid));
        }

        private static object ConvertValue(object value)
        {
            if (value != null && value.GetType() == typeof(Guid?))
            {
                var nullableGuid = (Guid?)value;
                if (nullableGuid.HasValue)
                    return nullableGuid.Value.ToString();
                return value; // Return null for null values
            }
            
            if (value != null && value.GetType() == typeof(DateTime?))
            {
                var nullableDateTime = (DateTime?)value;
                if (nullableDateTime.HasValue)
                    return nullableDateTime.Value.ToString("O");
                return value; // Return null for null values
            }

            return value switch
            {
                Guid guid => guid.ToString(),
                DateTime dateTime => dateTime.ToString("O"),
                IEnumerable<object> collection => collection.Select(ConvertValue).ToList(),
                _ when value?.GetType()?.IsClass == true && value.GetType() != typeof(string) => ConvertObjectToDictionary(value),
                _ => value ?? string.Empty
            };
        }

        private static Dictionary<string, object> ConvertObjectToDictionary(object obj)
        {
            var dict = new Dictionary<string, object>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    dict[prop.Name] = ConvertValue(value);
                }
            }
            return dict;
        }
    }

    public class Neo4jHospital
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? UserId { get; set; } // For HospitalAdmin role
    }

    public class Neo4jDepartment
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int DoctorsCount { get; set; }
        public string HospitalId { get; set; } = string.Empty;
    }

    public class Neo4jDoctor
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public string UserId { get; set; } = string.Empty;
    }

    public class Neo4jPatient
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }
        public string? CurrentTherapies { get; set; }
        public string? InsuranceAgencyId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class Neo4jEquipment
    {
        public string Id { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Operational";
        public string DepartmentId { get; set; } = string.Empty;
        public bool IsWithdrawn { get; set; }
        public string? LastServiceDate { get; set; }
        public string? NextServiceDate { get; set; }
    }

    public class Neo4jReservation
    {
        public string Id { get; set; } = string.Empty;
        public string HospitalId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string MedicalServiceId { get; set; } = string.Empty;
        public string StartsAtUtc { get; set; } = string.Empty;
        public string EndsAtUtc { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
    }

    public class Neo4jInsuranceAgency
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? UserId { get; set; } // For InsuranceAgency role
    }

    public class Neo4jAgencyContract
    {
        public string Id { get; set; } = string.Empty;
        public string InsuranceAgencyId { get; set; } = string.Empty;
        public string HospitalId { get; set; } = string.Empty;
        public decimal CoveragePercent { get; set; }
        public string StartsOn { get; set; } = string.Empty;
        public string EndsOn { get; set; } = string.Empty;
        public string Status { get; set; } = "Proposed";
        public string? RejectionReason { get; set; }
    }

    public class Neo4jMedicalService
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
    }

    public class Neo4jPriceListItem
    {
        public string Id { get; set; } = string.Empty;
        public string HospitalId { get; set; } = string.Empty;
        public string MedicalServiceId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ValidFrom { get; set; } = string.Empty;
        public string ValidUntil { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class Neo4jPayment
    {
        public string Id { get; set; } = string.Empty;
        public string PreContractId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string DueDateUtc { get; set; } = string.Empty;
        public string? PaidAtUtc { get; set; }
        public string? ProofUrl { get; set; }
        public bool Confirmed { get; set; }
        public int LateCount { get; set; }
    }

    public class Neo4jUser
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "InsuredUser";
        public string FullName { get; set; } = string.Empty;
        public string? HospitalId { get; set; }
        public string? InsuranceAgencyId { get; set; }
        public string? DoctorId { get; set; }
        public string? PatientId { get; set; }
        public bool IsActive { get; set; } = true;
        public string CreatedAtUtc { get; set; } = string.Empty;
    }

    public class Neo4jPreContract
    {
        public string Id { get; set; } = string.Empty;
        public string HospitalId { get; set; } = string.Empty;
        public string InsuranceAgencyId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public decimal AgreedPrice { get; set; }
        public string PaymentPlan { get; set; } = string.Empty;
        public string CreatedAtUtc { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }

    public class Neo4jEquipmentStatusLog
    {
        public string Id { get; set; } = string.Empty;
        public string EquipmentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string LoggedAtUtc { get; set; } = string.Empty;
    }

    public class Neo4jEquipmentServiceOrder
    {
        public string Id { get; set; } = string.Empty;
        public string EquipmentId { get; set; } = string.Empty;
        public string Type { get; set; } = "Service";
        public string ScheduledAtUtc { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled";
    }

    public class Neo4jRefreshToken
    {
        public string Id { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string ExpiresAtUtc { get; set; } = string.Empty;
        public bool Revoked { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class Neo4jAuditLog
    {
        public string Id { get; set; } = string.Empty;
        public string Actor { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string OccurredAtUtc { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class Neo4jAppointment
    {
        public string Id { get; set; } = string.Empty;
        public string HospitalId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DoctorId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string? MedicalServiceId { get; set; }
        public string StartsAtUtc { get; set; } = string.Empty;
        public string EndsAtUtc { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled";
        public bool IsConfirmed { get; set; }
        public int RescheduleCount { get; set; }
        public string? Notes { get; set; }
        public string RequiredEquipmentIds { get; set; } = string.Empty; // JSON array as string
    }

    public class Neo4jDiscount
    {
        public string Id { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string? HospitalId { get; set; }
        public string? InsuranceAgencyId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ValidFrom { get; set; } = string.Empty;
        public string ValidUntil { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Pending";
    }

    public class Neo4jDiscountRequest
    {
        public string Id { get; set; } = string.Empty;
        public string InsuranceAgencyId { get; set; } = string.Empty;
        public string HospitalId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public decimal RequestedDiscountPercent { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string Status { get; set; } = "Pending";
        public string? RejectionReason { get; set; }
        public string RequestedAtUtc { get; set; } = string.Empty;
        public string? RespondedAtUtc { get; set; }
    }

    public class Neo4jErrorLog
    {
        public string Id { get; set; } = string.Empty;
        public string ErrorType { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
        public string? UserId { get; set; }
        public string OccurredAtUtc { get; set; } = string.Empty;
    }

    public class Neo4jRequestLog
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long ResponseTimeMs { get; set; }
        public string TimestampUtc { get; set; } = string.Empty;
    }

    public class Neo4jThrottleLog
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int RequestCount { get; set; }
        public string WindowStartUtc { get; set; } = string.Empty;
        public string WindowEndUtc { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public string? BlockedUntilUtc { get; set; }
    }
}
