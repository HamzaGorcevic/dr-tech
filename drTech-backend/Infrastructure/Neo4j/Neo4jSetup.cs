using Neo4j.Driver;
using Microsoft.Extensions.Options;

namespace drTech_backend.Infrastructure.Neo4j
{
    public class Neo4jSettings
    {
        public string Uri { get; set; } = "neo4j://localhost:7687";
        public string User { get; set; } = "neo4j";
        public string Password { get; set; } = "password";
        public string Database { get; set; } = "neo4j";
    }

    public interface INeo4jContext : IAsyncDisposable
    {
        IAsyncSession Session { get; }
    }

    public class Neo4jContext : INeo4jContext
    {
        private readonly IDriver _driver;
        public IAsyncSession Session { get; }

        public Neo4jContext(IOptions<Neo4jSettings> options)
        {
            var uri = options.Value.Uri;
            
            // Handle different Neo4j connection schemes
            if (uri.StartsWith("neo4j://", StringComparison.OrdinalIgnoreCase))
            {
                // Local development - use bolt without encryption
                uri = uri.Replace("neo4j://", "bolt://", StringComparison.OrdinalIgnoreCase);
                _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(options.Value.User, options.Value.Password), o =>
                {
                    o.WithEncryptionLevel(EncryptionLevel.None);
                });
            }
            else if (uri.StartsWith("neo4j+s://", StringComparison.OrdinalIgnoreCase))
            {
                // Hosted Neo4j with encryption - use bolt+s
                uri = uri.Replace("neo4j+s://", "bolt+s://", StringComparison.OrdinalIgnoreCase);
                _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(options.Value.User, options.Value.Password));
            }
            else if (uri.StartsWith("bolt://", StringComparison.OrdinalIgnoreCase))
            {
                // Direct bolt connection without encryption
                _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(options.Value.User, options.Value.Password), o =>
                {
                    o.WithEncryptionLevel(EncryptionLevel.None);
                });
            }
            else if (uri.StartsWith("bolt+s://", StringComparison.OrdinalIgnoreCase))
            {
                // Direct bolt+s connection with encryption
                _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(options.Value.User, options.Value.Password));
            }
            else
            {
                // Default case - assume no encryption
                _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(options.Value.User, options.Value.Password), o =>
                {
                    o.WithEncryptionLevel(EncryptionLevel.None);
                });
            }
            
            Session = _driver.AsyncSession(cfg => cfg.WithDatabase(options.Value.Database));
        }

        public async ValueTask DisposeAsync()
        {
            await Session.CloseAsync();
            await _driver.DisposeAsync();
        }
    }
}


