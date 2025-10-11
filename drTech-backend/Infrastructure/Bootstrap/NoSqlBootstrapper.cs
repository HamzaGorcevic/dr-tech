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

            // Mongo: ensure collections exist
            var mongo = scope.ServiceProvider.GetRequiredService<Mongo.IMongoContext>();
            var db = mongo.Database;
            var requiredCollections = new[] { "auditlogs", "users", "payments" };
            var existing = (await db.ListCollectionNamesAsync(cancellationToken: ct)).ToList(ct);
            foreach (var name in requiredCollections)
            {
                if (!existing.Contains(name))
                {
                    await db.CreateCollectionAsync(name, cancellationToken: ct);
                    logger.LogInformation("Mongo collection created: {Name}", name);
                }
            }

            // Neo4j: ensure constraints/indexes and seed data
            try
            {
                var neo = scope.ServiceProvider.GetRequiredService<Neo4j.INeo4jContext>();
                var cypherStatements = new[]
                {
                    "CREATE CONSTRAINT hospital_id IF NOT EXISTS FOR (h:Hospital) REQUIRE h.id IS UNIQUE",
                    "CREATE CONSTRAINT doctor_id IF NOT EXISTS FOR (d:Doctor) REQUIRE d.id IS UNIQUE",
                    "CREATE CONSTRAINT patient_id IF NOT EXISTS FOR (p:Patient) REQUIRE p.id IS UNIQUE",
                    "CREATE CONSTRAINT user_id IF NOT EXISTS FOR (u:User) REQUIRE u.id IS UNIQUE",
                    "CREATE CONSTRAINT insurance_agency_id IF NOT EXISTS FOR (ia:InsuranceAgency) REQUIRE ia.id IS UNIQUE"
                };
                foreach (var cypher in cypherStatements)
                {
                    await neo.Session.ExecuteWriteAsync(async tx => { await tx.RunAsync(cypher); });
                }
                
                // Check if we need to seed data
                var count = await neo.Session.ExecuteReadAsync(async tx =>
                {
                    var result = await tx.RunAsync("MATCH (ia:InsuranceAgency) RETURN count(ia) as count");
                    var records = await result.ToListAsync();
                    return records.FirstOrDefault()?["count"].As<int>() ?? 0;
                });
                
                if (count == 0)
                {
                    // Seed sample data
                    var sampleAgencyId = Guid.NewGuid();
                    var sampleHospitalId = Guid.NewGuid();
                    
                    var seedQueries = new[]
                    {
                        $"CREATE (ia:InsuranceAgency {{id: '{sampleAgencyId}', name: 'Sample Insurance Agency', city: 'Belgrade'}})",
                        $"CREATE (h:Hospital {{id: '{sampleHospitalId}', name: 'General Hospital', city: 'Belgrade'}})",
                        $"CREATE (d:Department {{id: '{Guid.NewGuid()}', name: 'Cardiology', doctorsCount: 5, hospitalId: '{sampleHospitalId}'}})",
                        $"CREATE (d2:Department {{id: '{Guid.NewGuid()}', name: 'Surgery', doctorsCount: 8, hospitalId: '{sampleHospitalId}'}})"
                    };
                    
                    foreach (var query in seedQueries)
                    {
                        await neo.Session.ExecuteWriteAsync(async tx => { await tx.RunAsync(query); });
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


