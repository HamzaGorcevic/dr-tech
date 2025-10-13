using MongoDB.Bson;
using MongoDB.Driver;
using Neo4j.Driver;

namespace drTech_backend.Infrastructure.Bootstrap
{
    public static class NoSqlBootstrapper
    {
        public static async Task InitializeAsync(this IApplicationBuilder app, Microsoft.Extensions.Logging.ILogger logger, CancellationToken ct = default)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var provider = (configuration["DatabaseProvider"] ?? "PostgreSQL").Trim();

            // Mongo: ensure collections exist and seed if provider == MongoDB
            if (string.Equals(provider, "MongoDB", StringComparison.OrdinalIgnoreCase))
            {
                var mongo = scope.ServiceProvider.GetRequiredService<Mongo.IMongoContext>();
                var db = mongo.Database;
                var requiredCollections = new[] { "hospitals", "departments", "doctors", "patients", "equipment", "appointments", "agencies", "contracts", "services", "pricelist", "reservations", "payments", "users", "auditlogs", "precontracts" };
                var existing = (await db.ListCollectionNamesAsync(cancellationToken: ct)).ToList(ct);
                foreach (var name in requiredCollections)
                {
                    if (!existing.Contains(name))
                    {
                        await db.CreateCollectionAsync(name, cancellationToken: ct);
                        logger.LogInformation("Mongo collection created: {Name}", name);
                    }
                }

                // Seed minimal data if no agencies (acts as a proxy for empty DB)
                var agenciesCount = await db.GetCollection<BsonDocument>("agencies").EstimatedDocumentCountAsync(cancellationToken: ct);
                if (agenciesCount == 0)
                {
                    var hospitalId = Guid.NewGuid();
                    var departmentId = Guid.NewGuid();
                    var doctorId = Guid.NewGuid();
                    var patientId = Guid.NewGuid();
                    var equipmentId = Guid.NewGuid();
                    var serviceId = Guid.NewGuid();
                    var reservationId = Guid.NewGuid();
                    var agencyId = Guid.NewGuid();
                    var contractId = Guid.NewGuid();
                    var userId = Guid.NewGuid();
                    var preContractId = Guid.NewGuid();
                    var appointmentId = Guid.NewGuid();

                    await db.GetCollection<BsonDocument>("users").InsertOneAsync(new BsonDocument
                    {
                        { "_id", userId.ToString() },
                        { "Email", "admin@drtech.local" },
                        { "PasswordHash", BCrypt.Net.BCrypt.HashPassword("Admin#123") },
                        { "Role", "Admin" }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("hospitals").InsertOneAsync(new BsonDocument
                    {
                        { "_id", hospitalId.ToString() },
                        { "Name", "City Hospital" },
                        { "City", "Belgrade" }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("departments").InsertOneAsync(new BsonDocument
                    {
                        { "_id", departmentId.ToString() },
                        { "Name", "Surgery" },
                        { "DoctorsCount", 5 },
                        { "HospitalId", hospitalId.ToString() }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("doctors").InsertOneAsync(new BsonDocument
                    {
                        { "_id", doctorId.ToString() },
                        { "FullName", "Dr. John Doe" },
                        { "Specialty", "General Surgery" },
                        { "DepartmentId", departmentId.ToString() }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("patients").InsertOneAsync(new BsonDocument
                    {
                        { "_id", patientId.ToString() },
                        { "FullName", "Jane Smith" },
                        { "InsuranceNumber", "INS-0001" },
                        { "Allergies", "None" }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("equipment").InsertOneAsync(new BsonDocument
                    {
                        { "_id", equipmentId.ToString() },
                        { "SerialNumber", "EQ-1001" },
                        { "Type", "X-Ray" },
                        { "Status", "Operational" },
                        { "DepartmentId", departmentId.ToString() },
                        { "IsWithdrawn", false }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("services").InsertOneAsync(new BsonDocument
                    {
                        { "_id", serviceId.ToString() },
                        { "Code", "SRV-001" },
                        { "Name", "General Checkup" },
                        { "Type", "exam" },
                        { "DepartmentId", departmentId.ToString() }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("pricelist").InsertOneAsync(new BsonDocument
                    {
                        { "_id", Guid.NewGuid().ToString() },
                        { "HospitalId", hospitalId.ToString() },
                        { "MedicalServiceId", serviceId.ToString() },
                        { "Price", 100 },
                        { "ValidFrom", new DateTime(DateTime.UtcNow.Year,1,1).ToString("O") },
                        { "ValidUntil", new DateTime(DateTime.UtcNow.Year,12,31,23,59,59,DateTimeKind.Utc).ToString("O") },
                        { "IsActive", true }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("reservations").InsertOneAsync(new BsonDocument
                    {
                        { "_id", reservationId.ToString() },
                        { "HospitalId", hospitalId.ToString() },
                        { "DepartmentId", departmentId.ToString() },
                        { "PatientId", patientId.ToString() },
                        { "MedicalServiceId", serviceId.ToString() },
                        { "StartsAtUtc", DateTime.UtcNow.AddDays(2).ToString("O") },
                        { "EndsAtUtc", DateTime.UtcNow.AddDays(2).AddHours(1).ToString("O") },
                        { "Status", "Pending" }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("appointments").InsertOneAsync(new BsonDocument
                    {
                        { "_id", appointmentId.ToString() },
                        { "HospitalId", hospitalId.ToString() },
                        { "DepartmentId", departmentId.ToString() },
                        { "DoctorId", doctorId.ToString() },
                        { "PatientId", patientId.ToString() },
                        { "StartsAtUtc", DateTime.UtcNow.AddDays(1).ToString("O") },
                        { "EndsAtUtc", DateTime.UtcNow.AddDays(1).AddHours(1).ToString("O") },
                        { "Type", "exam" },
                        { "IsConfirmed", true }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("agencies").InsertOneAsync(new BsonDocument
                    {
                        { "_id", agencyId.ToString() },
                        { "Name", "Sample Insurance Agency" },
                        { "City", "Belgrade" }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("contracts").InsertOneAsync(new BsonDocument
                    {
                        { "_id", contractId.ToString() },
                        { "InsuranceAgencyId", agencyId.ToString() },
                        { "HospitalId", hospitalId.ToString() },
                        { "CoveragePercent", 0.5 },
                        { "StartsOn", DateTime.UtcNow.Date.ToString("O") },
                        { "EndsOn", DateTime.UtcNow.Date.AddYears(1).ToString("O") },
                        { "Status", "Accepted" }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("precontracts").InsertOneAsync(new BsonDocument
                    {
                        { "_id", preContractId.ToString() },
                        { "HospitalId", hospitalId.ToString() },
                        { "InsuranceAgencyId", agencyId.ToString() },
                        { "PatientId", patientId.ToString() },
                        { "AgreedPrice", 500 },
                        { "PaymentPlan", "2x250" },
                        { "CreatedAtUtc", DateTime.UtcNow.ToString("O") },
                        { "Status", "Active" }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("payments").InsertOneAsync(new BsonDocument
                    {
                        { "_id", Guid.NewGuid().ToString() },
                        { "PreContractId", preContractId.ToString() },
                        { "Amount", 250 },
                        { "DueDateUtc", DateTime.UtcNow.AddDays(7).ToString("O") },
                        { "Confirmed", false },
                        { "LateCount", 0 }
                    }, cancellationToken: ct);

                    await db.GetCollection<BsonDocument>("auditlogs").InsertOneAsync(new BsonDocument
                    {
                        { "_id", Guid.NewGuid().ToString() },
                        { "Actor", "system" },
                        { "Action", "Seed" },
                        { "Path", "/seed" },
                        { "Method", "SYSTEM" },
                        { "StatusCode", 200 },
                        { "OccurredAtUtc", DateTime.UtcNow.ToString("O") },
                        { "Description", "Initial seed completed" }
                    }, cancellationToken: ct);

                    logger.LogInformation("Mongo sample data seeded");
                }
            }

            // Neo4j: ensure constraints/indexes and seed data only when provider == Neo4j
            if (string.Equals(provider, "Neo4j", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var neo = scope.ServiceProvider.GetRequiredService<Neo4j.INeo4jContext>();
                    var cypherStatements = new[]
                    {
                        "CREATE CONSTRAINT hospital_id IF NOT EXISTS FOR (h:Hospital) REQUIRE h.id IS UNIQUE",
                        "CREATE CONSTRAINT department_id IF NOT EXISTS FOR (d:Department) REQUIRE d.id IS UNIQUE",
                        "CREATE CONSTRAINT doctor_id IF NOT EXISTS FOR (d:Doctor) REQUIRE d.id IS UNIQUE",
                        "CREATE CONSTRAINT patient_id IF NOT EXISTS FOR (p:Patient) REQUIRE p.id IS UNIQUE",
                        "CREATE CONSTRAINT user_id IF NOT EXISTS FOR (u:User) REQUIRE u.id IS UNIQUE",
                        "CREATE CONSTRAINT insurance_agency_id IF NOT EXISTS FOR (ia:InsuranceAgency) REQUIRE ia.id IS UNIQUE",
                        "CREATE CONSTRAINT service_id IF NOT EXISTS FOR (s:MedicalService) REQUIRE s.id IS UNIQUE",
                        "CREATE CONSTRAINT appointment_id IF NOT EXISTS FOR (a:Appointment) REQUIRE a.id IS UNIQUE"
                    };
                    foreach (var cypher in cypherStatements)
                    {
                        await neo.Session.ExecuteWriteAsync(async tx => { await tx.RunAsync(cypher); });
                    }
                    
                    var count = await neo.Session.ExecuteReadAsync(async tx =>
                    {
                        var result = await tx.RunAsync("MATCH (d:Doctor) RETURN count(d) as count");
                        var record = await result.SingleAsync();
                        return record["count"].As<int>();
                    });
                    
                    if (count == 0)
                    {
                        var hospitalId = Guid.NewGuid().ToString();
                        var departmentId = Guid.NewGuid().ToString();
                        var doctorId = Guid.NewGuid().ToString();
                        var patientId = Guid.NewGuid().ToString();
                        var serviceId = Guid.NewGuid().ToString();
                        var agencyId = Guid.NewGuid().ToString();
                        var userId = Guid.NewGuid().ToString();
                        var preContractId = Guid.NewGuid().ToString();
                        var appointmentId = Guid.NewGuid().ToString();

                        // Create nodes
                        var seedQueries = new[]
                        {
                            $"CREATE (u:User {{id: '{userId}', email: 'admin@drtech.local', passwordHash: '{BCrypt.Net.BCrypt.HashPassword("Admin#123").Replace("'","\\'")}', role: 'Admin' }})",
                            $"CREATE (ia:InsuranceAgency {{id: '{agencyId}', name: 'Sample Insurance Agency', city: 'Belgrade'}})",
                            $"CREATE (h:Hospital {{id: '{hospitalId}', name: 'City Hospital', city: 'Belgrade'}})",
                            $"CREATE (d:Department {{id: '{departmentId}', name: 'Surgery', doctorsCount: 5, hospitalId: '{hospitalId}'}})",
                            $"CREATE (doc:Doctor {{id: '{doctorId}', fullName: 'Dr. John Doe', specialty: 'General Surgery', departmentId: '{departmentId}'}})",
                            $"CREATE (p:Patient {{id: '{patientId}', fullName: 'Jane Smith', insuranceNumber: 'INS-0001'}})",
                            $"CREATE (s:MedicalService {{id: '{serviceId}', code: 'SRV-001', name: 'General Checkup', type: 'exam', departmentId: '{departmentId}'}})",
                            $"CREATE (pl:PriceListItem {{id: '{Guid.NewGuid()}', hospitalId: '{hospitalId}', medicalServiceId: '{serviceId}', price: 100, validFrom: '{new DateTime(DateTime.UtcNow.Year,1,1).ToString("O")}', validUntil: '{new DateTime(DateTime.UtcNow.Year,12,31,23,59,59,DateTimeKind.Utc).ToString("O")}', isActive: true}})",
                            $"CREATE (r:Reservation {{id: '{Guid.NewGuid()}', hospitalId: '{hospitalId}', departmentId: '{departmentId}', patientId: '{patientId}', medicalServiceId: '{serviceId}', startsAtUtc: '{DateTime.UtcNow.AddDays(2).ToString("O")}', endsAtUtc: '{DateTime.UtcNow.AddDays(2).AddHours(1).ToString("O")}', status: 'Pending'}})",
                            $"CREATE (a:Appointment {{id: '{appointmentId}', hospitalId: '{hospitalId}', departmentId: '{departmentId}', doctorId: '{doctorId}', patientId: '{patientId}', startsAtUtc: '{DateTime.UtcNow.AddDays(1).ToString("O")}', endsAtUtc: '{DateTime.UtcNow.AddDays(1).AddHours(1).ToString("O")}', type: 'exam', isConfirmed: true}})",
                            $"CREATE (pc:PreContract {{id: '{preContractId}', hospitalId: '{hospitalId}', insuranceAgencyId: '{agencyId}', patientId: '{patientId}', agreedPrice: 500, paymentPlan: '2x250', createdAtUtc: '{DateTime.UtcNow.ToString("O")}', status: 'Active'}})",
                            $"CREATE (pay:Payment {{id: '{Guid.NewGuid()}', preContractId: '{preContractId}', amount: 250, dueDateUtc: '{DateTime.UtcNow.AddDays(7).ToString("O")}', confirmed: false, lateCount: 0}})",
                            $"CREATE (eq:Equipment {{id: '{Guid.NewGuid()}', serialNumber: 'EQ-1001', type: 'X-Ray', status: 'Operational', departmentId: '{departmentId}', isWithdrawn: false}})",
                            $"CREATE (al:AuditLog {{id: '{Guid.NewGuid()}', actor: 'system', action: 'Seed', path: '/seed', method: 'SYSTEM', statusCode: 200, occurredAtUtc: '{DateTime.UtcNow.ToString("O")}', description: 'Initial seed completed'}})"
                        };
                        foreach (var query in seedQueries)
                        {
                            await neo.Session.ExecuteWriteAsync(async tx => { await tx.RunAsync(query); });
                        }

                        // Relationships
                        var relQueries = new[]
                        {
                            $"MATCH (h:Hospital {{id: '{hospitalId}'}}), (d:Department {{id: '{departmentId}'}}) CREATE (h)-[:HAS_DEPARTMENT]->(d)",
                            $"MATCH (d:Department {{id: '{departmentId}'}}), (doc:Doctor {{id: '{doctorId}'}}) CREATE (d)-[:HAS_DOCTOR]->(doc)",
                            $"MATCH (d:Department {{id: '{departmentId}'}}), (s:MedicalService {{id: '{serviceId}'}}) CREATE (d)-[:OFFERS]->(s)",
                            $"MATCH (h:Hospital {{id: '{hospitalId}'}}), (eq:Equipment {{departmentId: '{departmentId}'}}) CREATE (h)-[:HAS_EQUIPMENT]->(eq)",
                            $"MATCH (ia:InsuranceAgency {{id: '{agencyId}'}}), (h:Hospital {{id: '{hospitalId}'}}) CREATE (ia)-[:CONTRACT_WITH {{coveragePercent: 0.5, startsOn: '{DateTime.UtcNow.Date.ToString("O")}', endsOn: '{DateTime.UtcNow.Date.AddYears(1).ToString("O")}', status: 'Accepted'}}]->(h)",
                            $"MATCH (pc:PreContract {{id: '{preContractId}'}}), (p:Patient {{id: '{patientId}'}}) CREATE (pc)-[:FOR_PATIENT]->(p)",
                            $"MATCH (pc:PreContract {{id: '{preContractId}'}}), (ia:InsuranceAgency {{id: '{agencyId}'}}) CREATE (pc)-[:WITH_AGENCY]->(ia)",
                            $"MATCH (pc:PreContract {{id: '{preContractId}'}}), (h:Hospital {{id: '{hospitalId}'}}) CREATE (pc)-[:AT_HOSPITAL]->(h)",
                            $"MATCH (pc:PreContract {{id: '{preContractId}'}}), (pay:Payment) CREATE (pc)-[:HAS_PAYMENT]->(pay)"
                        };
                        foreach (var q in relQueries)
                        {
                            await neo.Session.ExecuteWriteAsync(async tx => { await tx.RunAsync(q); });
                        }
                        
                        logger.LogInformation("Neo4j sample data seeded");
                    }
                    
                    logger.LogInformation("Neo4j constraints ensured");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Neo4j bootstrap skipped (service unavailable)");
                }
            }
        }
    }
}


